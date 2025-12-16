import {Component, DestroyRef, OnInit, inject} from '@angular/core';
import {CommonModule, NgIf, NgFor} from '@angular/common';
import {ReactiveFormsModule, FormGroup, FormBuilder, Validators, FormsModule} from '@angular/forms';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {MatTabsModule} from '@angular/material/tabs';
import {Router, RouterModule} from '@angular/router';
import {finalize, forkJoin} from 'rxjs';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {TasksService, CreateTaskPayload} from '../../core/tasks.service';
import {AuthService, AuthenticatedUser} from '../../core/auth.service';
import {MatSnackBar} from '@angular/material/snack-bar';
import {ActivatedRoute} from '@angular/router';
import {UserDto, UsersService} from '../users.service';

type Role = AuthenticatedUser['role'] | null;

@Component({
  selector: 'app-task-create',
  standalone: true,
  imports: [
    CommonModule,
    NgIf,
    NgFor,
    ReactiveFormsModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatTabsModule,
    RouterModule
  ],
  templateUrl: './task-create.html',
  styleUrls: ['./task-create.css']
})
export class TaskCreate implements OnInit {
  private readonly destroyRef = inject(DestroyRef);

  taskForm: FormGroup;
  assignees: UserDto[] = [];
  statuses: ('todo' | 'in-progress' | 'done')[] = ['todo', 'in-progress', 'done'];

  selectedFiles: File[] = [];
  firstComment: string = '';

  loading = false;
  apiError: string | null = null;
  successMessage: string | null = null;
  assigneesLoading = false;
  assigneesError: string | null = null;

  protected currentUser: AuthenticatedUser | null = null;
  private currentRole: Role = null;

  constructor(
    private fb: FormBuilder,
    private tasksService: TasksService,
    private usersService: UsersService,
    private router: Router,
    private authService: AuthService,
    private snackBar: MatSnackBar,
    private route: ActivatedRoute
  ) {
    this.taskForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      assignedUserId: ['', Validators.required],
      status: ['todo', Validators.required]
    });
  }

  ngOnInit() {
    this.authService.currentUser$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(user => {
        this.currentUser = user;
        this.currentRole = user?.role ?? null;

        if (!this.canCreateTask()) this.taskForm.disable({emitEvent: false});
        else if (this.taskForm.disabled) this.taskForm.enable({emitEvent: false});
      });

    this.route.queryParams
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(params => {
        const employeeId = params['employeeId'];
        if (employeeId) {
          this.taskForm.patchValue({assignedUserId: Number(employeeId)});
          this.taskForm.get('assignedUserId')?.disable();
        }
      });

    this.loadAssignees();
  }


  private loadAssignees() {
    this.assigneesLoading = true;
    this.assigneesError = null;

    this.usersService.getEmployees().pipe(
      takeUntilDestroyed(this.destroyRef),
      finalize(() => this.assigneesLoading = false)
    ).subscribe({
      next: (users: UserDto[]) => this.assignees = users,
      error: err => {
        this.assigneesError = err?.error?.message || 'Unable to load users.';
        this.assignees = [];
      }
    });
  }

  canCreateTask(): boolean {
    return this.isAdmin() || this.isProjectManager();
  }

  private isAdmin(): boolean {
    return this.currentRole === 'admin';
  }

  private isProjectManager(): boolean {
    return this.currentRole === 'project-manager';
  }

  showSuccess(message: string) {
    this.snackBar.open(message, 'Close', {duration: 3000, panelClass: ['snackbar-success']});
  }

  showError(message: string) {
    this.snackBar.open(message, 'Close', {duration: 3000, panelClass: ['snackbar-error']});
  }

  onFilesSelected(event: any) {
    this.selectedFiles = Array.from(event.target.files);
  }

  uploadAttachments(taskId: number) {
    const uploads = this.selectedFiles.map(file => this.tasksService.uploadAttachment(taskId, file));
    return forkJoin(uploads);
  }

  createTask() {
    if (!this.canCreateTask()) {
      this.apiError = 'You do not have permission to create tasks.';
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

    const assignedUserIdValue = this.taskForm.getRawValue().assignedUserId;

    const payload: CreateTaskPayload = {
      title: this.taskForm.value.title ?? '',
      description: this.taskForm.value.description ?? '',
      status: this.taskForm.value.status as 'todo' | 'in-progress' | 'done',
      assignedUserId: assignedUserIdValue != null ? Number(assignedUserIdValue) : null
    };

    this.tasksService.createTask(payload).pipe(
      finalize(() => this.loading = false)
    ).subscribe({
      next: (createdTask) => {
        const actions = [];
        if (this.selectedFiles.length > 0) actions.push(this.uploadAttachments(createdTask.id));
        if (this.firstComment) actions.push(this.tasksService.addComment(createdTask.id, this.firstComment));

        if (actions.length > 0) {
          forkJoin(actions).subscribe(() => {
            this.showSuccess('Task created with attachments and comment!');
            this.router.navigate(['/dashboard/tasks']);
          });
        } else {
          this.showSuccess('Task created successfully!');
          this.router.navigate(['/dashboard/tasks']);
        }
      },
      error: (err: any) => {
        const message: string = err?.error?.message ?? 'Failed to create task.';
        this.apiError = message;
        this.showError(message);
      }

    });
  }
}
