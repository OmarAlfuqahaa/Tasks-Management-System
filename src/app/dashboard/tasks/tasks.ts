import { Component, OnDestroy, OnInit } from '@angular/core';
import {ActivatedRoute, Router, RouterModule} from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { NgxPaginationModule } from 'ngx-pagination';
import { finalize, Subscription, firstValueFrom } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { TasksService, TaskDto, UpdateTaskPayload } from '../../core/tasks.service';
import { AuthService, AuthenticatedUser } from '../../core/auth.service';
import Swal from 'sweetalert2';
import { UsersService } from '../users.service';
import { ChangeDetectorRef } from '@angular/core';
import { NgSelectModule } from '@ng-select/ng-select';



type Status = 'todo' | 'in-progress' | 'done' ;
type Role = AuthenticatedUser['role'] | null;

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    NgxPaginationModule,
    NgSelectModule
  ],
  templateUrl: './tasks.html',
  styleUrls: ['./tasks.css'],
})
export class Tasks implements OnInit, OnDestroy {
  private totalItems= 0;

  constructor(
    private router: Router,
    private tasksService: TasksService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private usersService: UsersService,
    private cdr: ChangeDetectorRef

  ) {}

  tasks: TaskDto[] = [];
  showAllTasks: boolean = false;

  selectedOtherAssignees: string[] = [];
  otherAssignees: string[] = [];

  statuses: { value: Status | any ; label: string }[] = [
    { value: 'All', label: 'All' },
    { value: 'todo', label: 'To Do' },
    { value: 'in-progress', label: 'In Progress' },
    { value: 'done', label: 'Done' }
  ];

  assignees: string[] = ['All'];
  selectedStatus: string = 'All';
  selectedAssignees: string[] = [];
  searchTerm: string = '';

  page: number = 1;
  itemsPerPage: number = 5;

  loading = false;
  deletingTaskId: number | null = null;
  error: string | null = null;

  private subscriptions = new Subscription();
  private currentUser: AuthenticatedUser | null = null;
  private currentRole: Role = null;

  private usersMap: Map<string, number> = new Map();


  ngOnInit(): void {
    const authSub = this.authService.currentUser$.subscribe(async user => {
      this.currentUser = user;
      this.currentRole = user?.role ?? null;

      if (this.canManageAllTasks()) {
        const users = await firstValueFrom(this.usersService.getEmployees());

        this.usersMap.clear();
        users.forEach(u => this.usersMap.set(u.name.toLowerCase(), u.id));
        const currentName = this.currentUser?.name ?? '';
        this.otherAssignees = users
          .map(u => u.name)
          .filter(name => name.toLowerCase() !== currentName.toLowerCase());
        this.assignees = ['All', ...this.otherAssignees];

        this.applyQueryParams();

        this.cdr.detectChanges();

        this.loadTasks();
      } else {
        this.applyQueryParams();
        this.loadTasks();
      }
    });

    this.subscriptions.add(authSub);
  }


  private async refreshUsersMap(): Promise<void> {
    const users = await firstValueFrom(this.usersService.getEmployees());
    if (!users) return;

    this.usersMap.clear();
    users.forEach(u => this.usersMap.set(u.name.toLowerCase(), u.id));

    const currentName = this.currentUser?.name ?? '';
    this.otherAssignees = users
      .map(u => u.name)
      .filter(name => name.toLowerCase() !== currentName.toLowerCase());

    this.assignees = ['All', ...this.otherAssignees];
  }

  private applyQueryParams() {
    const params = this.route.snapshot.queryParams;
    if (!params['assignedUserIds']) return;

    const ids = Array.isArray(params['assignedUserIds'])
      ? params['assignedUserIds'].map((id: any) => Number(id))
      : [Number(params['assignedUserIds'])];

    this.selectedAssignees = ids
      .map(id => this.getAssigneeNameById(id))
      .filter((name): name is string => !!name);
  }





  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  protected loadTasks() {
    this.loading = true;
    this.error = null;

    const query: any = {
      page: this.page,
      pageSize: this.itemsPerPage
    };

    if (this.selectedStatus !== 'All') {
      query.status = this.selectedStatus;
    }

    if (this.searchTerm) {
      query.search = this.searchTerm;
    }

    if (this.isEmployee()) {
      query.assignedUserIds = [this.currentUser!.id];
    }

    if (this.canManageAllTasks() && this.selectedAssignees.length) {
      const assigneeIds: number[] = this.selectedAssignees
        .map(name => this.getUserIdByName(name))
        .filter((id): id is number => id !== undefined);
      if (assigneeIds.length) {
        query.assignedUserIds = assigneeIds;
      }
    }

    if (Array.isArray(query.assignedUserIds)) {
      query.assignedUserIds = query.assignedUserIds.map((id: any) => Number(id));
    }

    const load$ = this.tasksService.getTasks(query).pipe(
      finalize(() => this.loading = false)
    ).subscribe({
      next: (result) => {
        this.tasks = result?.items ?? [];
        this.totalItems = result?.totalItems ?? 0;

        if (this.canManageAllTasks()) {
          this.refreshOtherAssigneeList();
        }
      },
      error: (err) => {
        this.error = err?.error?.message || 'Unable to load tasks.';
      }
    });

    this.subscriptions.add(load$);
  }



  private getUserIdByName(name: string): number | undefined {
    return this.usersMap.get(name.toLowerCase());
  }


  deleteTask(task: TaskDto) {
    if (!this.canDeleteTask(task)) {
      Swal.fire('Permission Denied', 'You do not have permission to delete this task.', 'error');
      return;
    }


    Swal.fire({
      title: 'Are you sure?',
      text: `Do you want to delete the task "${task.title}"?`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, delete',
      cancelButtonText: 'Cancel'
    }).then((result) => {
      if (!result.isConfirmed) return;
      if (this.deletingTaskId === task.id) return;

      this.deletingTaskId = task.id;
      const delete$ = this.tasksService.deleteTask(task.id).pipe(
        finalize(() => this.deletingTaskId = null)
      ).subscribe({
        next: () => {
          Swal.fire('Deleted!', 'The task has been deleted successfully.', 'success');
          this.loadTasks();
        },
        error: (error: HttpErrorResponse) => {
          Swal.fire('Error', error?.error?.message || 'Unable to delete task.', 'error');
        }
      });
      this.subscriptions.add(delete$);
    });


  }

  goToEdit(id: number) {
    const task = this.tasks.find(t => t.id === id);
    if (!task || !this.canEditTask(task)) {
      this.error = 'You do not have permission to edit this task.';
      return;
    }
    this.router.navigate(['/dashboard/tasks/edit', id]);
  }

  openAddTaskDialog() {
    if (!this.canCreateTask()) {
      this.error = 'You do not have permission to create tasks.';
      return;
    }
    this.router.navigate(['/dashboard/tasks/create']);
  }

  updateTaskStatus(task: TaskDto) {
    const allowedStatuses = this.getAllowedStatuses(task);
    if (!allowedStatuses.includes(task.status as Status)) {
      task.status = allowedStatuses[0];
      Swal.fire('Invalid status', 'You must update the task status in order.', 'warning');
      return;
    }


  const payload: UpdateTaskPayload = {
    title: task.title,
    description: task.description ?? '',
    assignedUserId: Number(task.assignedUserId),
    status: task.status
  };

  this.tasksService.updateTask(task.id, payload).subscribe({
    next: () => console.log('Status updated to', task.status),
    error: err => this.error = err?.error?.message || 'Unable to update status.'
  });


  }

  canCreateTask(): boolean {
    return this.isAdmin() || this.isProjectManager();
  }

  canEditTask(task: TaskDto): boolean {
    if (!this.isAuthenticated()) return false;
    if (this.canManageAllTasks()) return true;
    return this.isEmployee() && this.isTaskOwner(task);
  }

  canDeleteTask(_: TaskDto): boolean {
    return this.isAdmin() || this.isProjectManager();
  }

  canSeeAnyTaskActions(): boolean {
    return this.isAuthenticated();
  }

  canUseAssigneeFilter(): boolean {
    return this.canManageAllTasks();
  }


  protected getAllowedStatuses(task: TaskDto): Status[] {
    const order: Status[] = ['todo', 'in-progress', 'done'];
    const currentIndex = order.indexOf(task.status as Status);
    const allowed: Status[] = [task.status];
    if (currentIndex < order.length - 1) allowed.push(order[currentIndex + 1]);
    return allowed;
  }


  public isTaskOwner(task: TaskDto): boolean {
    const currentName = this.currentUser?.name?.toLowerCase();
    if (!currentName || !task.assignedTo) return false;
    return task.assignedTo.toLowerCase() === currentName;
  }

  private canManageAllTasks(): boolean {
    return this.isAdmin() || this.isProjectManager();
  }

  private isAdmin(): boolean {
    return this.currentRole === 'admin';
  }

  private isProjectManager(): boolean {
    return this.currentRole === 'project-manager';
  }

  public isEmployee(): boolean {
    return this.currentRole === 'employee';
  }

  private isAuthenticated(): boolean {
    return !!this.currentRole;
  }



  private refreshOtherAssigneeList() {
    this.usersService.getEmployees().subscribe(users => {
      const userNames = users.map(u => u.name);
      const currentName = this.currentUser?.name ?? '';
      this.otherAssignees = userNames.filter(a => a.toLowerCase() !== currentName.toLowerCase());
      users.forEach(u => this.usersMap.set(u.name.toLowerCase(), u.id));

    });
  }

  get pages(): number[] {
    return Array.from({ length: this.totalPages }, (_, i) => i + 1);
  }

  get totalPages(): number {
    return Math.ceil(this.totalItems / this.itemsPerPage);
  }

  changePage(newPage: number) {
    if (newPage < 1 || newPage > this.totalPages) return;
    this.page = newPage;
    this.loadTasks();
  }


  openTask(taskId: number) {
    this.router.navigate(['/dashboard/tasks', taskId]);
  }




  private getAssigneeNameById(id: number): string | undefined {
    for (const [name, userId] of this.usersMap.entries()) {
      if (userId === id) {
        return name;
      }
    }
    return undefined;
  }

}
