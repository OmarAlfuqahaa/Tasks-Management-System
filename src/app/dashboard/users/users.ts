import {Component, DestroyRef, OnInit, inject} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MatButtonModule} from '@angular/material/button';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {NgxPaginationModule} from 'ngx-pagination';
import {MatDialog, MatDialogModule} from '@angular/material/dialog';
import {UsersService, UserDto} from '../users.service';
import {finalize} from 'rxjs';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {HttpErrorResponse} from '@angular/common/http';
import {AuthService, AuthenticatedUser} from '../../core/auth.service';
import Swal from 'sweetalert2';


type Role = AuthenticatedUser['role'] | null;

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatInputModule,
    MatSelectModule,
    NgxPaginationModule,
    MatDialogModule
  ],
  templateUrl: './users.html',
  styleUrls: ['./users.css']
})
export class Users implements OnInit {
  private readonly destroyRef = inject(DestroyRef);

  users: UserDto[] = [];
  roles: string[] = ['All', 'admin', 'employee', 'project-manager'];
  selectedRole: string = 'All';
  searchTerm: string = '';

  page: number = 1;
  itemsPerPage: number = 5;
  totalItems: number = 0;

  loadingUsers = false;
  apiError: string | null = null;
  deletingUserIds = new Set<number>();
  private currentUser: AuthenticatedUser | null = null;
  private currentRole: Role = null;

  constructor(
    private dialog: MatDialog,
    private usersService: UsersService,
    private authService: AuthService
  ) {
  }

  ngOnInit() {
    this.authService.currentUser$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(user => {
        this.currentUser = user;
        this.currentRole = user?.role ?? null;
        if (this.canManageUsers()) this.loadUsers();
      });
  }

  private loadUsers() {
    if (!this.canManageUsers()) return;

    this.loadingUsers = true;
    this.apiError = null;

    this.usersService.getUsers({
      role: this.selectedRole,
      search: this.searchTerm,
      page: this.page,
      pageSize: this.itemsPerPage
    })
      .pipe(finalize(() => (this.loadingUsers = false)))
      .subscribe({
        next: res => {
          this.users = res?.items ?? [];
          this.totalItems = res?.totalItems ?? 0;
        },
        error: (error: HttpErrorResponse) => {
          this.apiError = error?.error?.message || 'Unable to load users.';
          this.users = [];
          this.totalItems = 0;
        }
      });
  }

  onSearchChange(term: string) {
    this.searchTerm = term;
    this.page = 1;
    this.loadUsers();
  }

  filterByRole() {
    this.page = 1;
    this.loadUsers();
  }

  onItemsPerPageChange() {
    this.page = 1;
    this.loadUsers();
  }


  canManageUsers(): boolean {
    return this.currentRole === 'admin';
  }


  async editUser(user: UserDto) {
    if (!this.canManageUsers()) {
      this.apiError = 'You do not have permission to edit users.';
      return;
    }

    const {EditUserDialog} = await import('../edit-user/edit-user');

    const dialogRef = this.dialog.open(EditUserDialog, {
      width: '400px',
      data: {...user}
    });

    dialogRef.afterClosed().subscribe((result: UserDto | undefined) => {
      if (result) {
        const index = this.users.findIndex(u => u.id === result.id);
        if (index > -1) {
          this.users[index] = result;
          //this.filterUsers();
        }
      }
      this.loadUsers();

    });
  }


  deleteUser(user: UserDto) {
    if (!this.canManageUsers()) {
      Swal.fire({
        title: 'Permission Denied',
        text: 'You do not have permission to delete users.',
        icon: 'error',
        confirmButtonText: 'OK'
      });
      return;
    }

    Swal.fire({
      title: 'Are you sure?',
      text: `Do you want to delete user "${user.name}"?`,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, delete',
      cancelButtonText: 'Cancel'
    }).then((result) => {
      if (!result.isConfirmed) return;

      if (this.deletingUserIds.has(user.id)) return;

      this.apiError = null;
      this.deletingUserIds.add(user.id);

      this.usersService.deleteUser(user.id)
        .pipe(
          takeUntilDestroyed(this.destroyRef),
          finalize(() => this.deletingUserIds.delete(user.id))
        )
        .subscribe({
          next: () => {
            this.users = this.users.filter(u => u.id !== user.id);

            Swal.fire({
              title: 'Deleted!',
              text: 'The user has been deleted successfully.',
              icon: 'success',
              confirmButtonText: 'OK'
            });
          },
          error: (error: HttpErrorResponse) => {
            this.apiError = error?.error?.message || 'Unable to delete user.';


          }
        });
    });
  }


  isDeleting(userId: number): boolean {
    return this.deletingUserIds.has(userId);
  }

  get pages(): number[] {
    return Array.from({length: this.totalPages}, (_, i) => i + 1);
  }

  get totalPages(): number {
    return Math.ceil(this.totalItems / this.itemsPerPage);
  }

  changePage(newPage: number) {
    if (newPage < 1 || newPage > this.totalPages) return;
    this.page = newPage;
    this.loadUsers();
  }
}
