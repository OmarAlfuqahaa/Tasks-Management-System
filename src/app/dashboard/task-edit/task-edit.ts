import {Component, OnDestroy, OnInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ReactiveFormsModule, FormBuilder, FormGroup, Validators} from '@angular/forms';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {MatButtonModule} from '@angular/material/button';
import {RouterModule, ActivatedRoute, Router} from '@angular/router';
import {Subscription, finalize} from 'rxjs';
import {HttpErrorResponse} from '@angular/common/http';
import {TasksService, UpdateTaskPayload, TaskDto} from '../../core/tasks.service';
import {AuthService, AuthenticatedUser} from '../../core/auth.service';
import {MatSnackBar} from '@angular/material/snack-bar';
import {UsersService, UserDto} from '../users.service'


type Status = 'todo' | 'in-progress' | 'done';
type Role = AuthenticatedUser['role'] | null;

@Component({
  selector: 'app-task-edit',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    RouterModule
  ],
  templateUrl: './task-edit.html',
  styleUrls: ['./task-edit.css']
})
export class TaskEdit implements OnInit, OnDestroy {
  taskForm!: FormGroup;
  taskId!: number;
  statuses: { value: Status; label: string }[] = [
    {value: 'todo', label: 'To Do'},
    {value: 'in-progress', label: 'In Progress'},
    {value: 'done', label: 'Done'}
  ];

  loading = false;
  apiError: string | null = null;
  successMessage: string | null = null;
  assignees: UserDto[] = [];
  assigneesLoading = false;
  assigneesError: string | null = null;
  private currentUser: AuthenticatedUser | null = null;
  private currentRole: Role = null;
  canEditLoadedTask = false;

  private subscriptions = new Subscription();

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private tasksService: TasksService,
    private usersService: UsersService,
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {
  }

  showSuccess(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      panelClass: ['snackbar-success'], // optional class
      horizontalPosition: 'right',
      verticalPosition: 'bottom',
    });
  }

  showError(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      panelClass: ['snackbar-error'],
      horizontalPosition: 'right',
      verticalPosition: 'bottom',
    });
  }

  ngOnInit() {
    const token = localStorage.getItem('authToken');
    console.log('Auth Token:', token);

    this.taskId = Number(this.route.snapshot.paramMap.get('id'));

    this.taskForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      assignedUserId: ['', Validators.required],
      status: ['todo', Validators.required]
    });

    const authSub = this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      this.currentRole = user?.role ?? null;
      if (!this.canEditLoadedTask && this.taskForm) {
        this.taskForm.disable({emitEvent: false});
      }
    });
    this.subscriptions.add(authSub);

    this.loadTask();
    this.loadAssignees();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  loadTask() {
    this.loading = true;
    this.apiError = null;

    const load$ = this.tasksService.getTaskById(this.taskId).pipe(
      finalize(() => this.loading = false)
    ).subscribe({
      next: (task: TaskDto) => {
        this.taskForm.patchValue({
          title: task.title,
          description: task.description,
          assignedUserId: task.assignedUserId,
          status: task.status
        });
        this.canEditLoadedTask = this.canEditTask(task);
        if (!this.canEditLoadedTask) {
          this.taskForm.disable({emitEvent: false});
          this.apiError = 'You do not have permission to edit this task.';
        } else if (this.taskForm.disabled) {
          this.taskForm.enable({emitEvent: false});
        }
      },
      error: (error: HttpErrorResponse) => {
        this.apiError = error?.error?.message || 'Unable to load task.';
      }
    });

    this.subscriptions.add(load$);
  }

  saveTask() {
    if (!this.canEditLoadedTask) {
      this.apiError = 'You do not have permission to save this task.';
      this.showError(this.apiError);
      return;
    }
    if (this.taskForm.invalid) {
      this.taskForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.apiError = null;
    this.successMessage = null;

    const payload: UpdateTaskPayload = {
      title: this.taskForm.value.title ?? '',
      description: this.taskForm.value.description ?? '',
      assignedUserId: Number(this.taskForm.value.assignedUserId),
      status: (this.taskForm.value.status as Status) ?? 'todo'
    };

    const update$ = this.tasksService.updateTask(this.taskId, payload).pipe(
      finalize(() => this.loading = false)
    ).subscribe({
      next: () => {
        this.successMessage = 'Task updated successfully';
        this.showSuccess(this.successMessage);
        this.router.navigate(['/dashboard/tasks']);
      }

    });

    this.subscriptions.add(update$);
  }


  canEditTask(task: TaskDto): boolean {
    if (!this.currentRole) {
      return false;
    }
    if (this.isAdmin() || this.isProjectManager()) {
      return true;
    }
    return this.isEmployee() && this.isTaskOwner(task);
  }

  canUpdateAssignee(): boolean {
    return this.isAdmin() || this.isProjectManager();
  }

  private loadAssignees() {
    this.assigneesLoading = true;
    this.assigneesError = null;

    const assignees$ = this.usersService.getEmployees().pipe(
      finalize(() => this.assigneesLoading = false)
    ).subscribe({
      next: (users: UserDto[]) => this.assignees = users,
      error: (error: HttpErrorResponse) => {
        this.assigneesError = error?.error?.message || 'Unable to load users.';
        this.assignees = [];
      }
    });

    this.subscriptions.add(assignees$);
  }


  private isTaskOwner(task: TaskDto): boolean {
    const currentName = this.currentUser?.name?.toLowerCase();
    if (!currentName || !task.assignedTo) {
      return false;
    }
    return task.assignedTo.toLowerCase() === currentName;
  }

  private isAdmin() {
    return this.currentRole === 'admin';
  }

  private isProjectManager() {
    return this.currentRole === 'project-manager';
  }

  private isEmployee() {
    return this.currentRole === 'employee';
  }
}
