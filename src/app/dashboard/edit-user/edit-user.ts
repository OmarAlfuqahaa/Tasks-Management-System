import {Component, Inject} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule, FormGroup, FormControl, Validators} from '@angular/forms';
import {MatButtonModule} from '@angular/material/button';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {MatDialogRef, MAT_DIALOG_DATA} from '@angular/material/dialog';
import {finalize} from 'rxjs';
import {UsersService, UserDto} from '../users.service';
import {MatSnackBar} from '@angular/material/snack-bar';


export interface User {
  id: number;
  name: string;
  email: string;
  role: 'admin' | 'employee' | 'project-manager';
}

@Component({
  selector: 'app-edit-user-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MatInputModule, MatButtonModule, MatSelectModule],
  templateUrl: './edit-user.html',
  styleUrls: ['edit-user.css']
})
export class EditUserDialog {
  editForm: FormGroup;
  roles: string[] = ['admin', 'employee', 'project-manager'];

  loading = false;
  apiError: string | null = null;

  constructor(
    public dialogRef: MatDialogRef<EditUserDialog>,
    @Inject(MAT_DIALOG_DATA) public data: User,
    private usersService: UsersService,
    private snackBar: MatSnackBar
  ) {
    this.editForm = new FormGroup({
      name: new FormControl(data.name, [Validators.required, Validators.minLength(2)]),
      email: new FormControl(data.email, [Validators.required, Validators.email]),
      role: new FormControl(data.role, Validators.required)
    });
  }

  showSuccess(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      panelClass: ['snackbar-success'],
      horizontalPosition: 'right',
      verticalPosition: 'bottom',
    });
  }

  onSave() {
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.apiError = null;

    const {name, email, role} = this.editForm.value;

    this.usersService.updateUser(this.data.id, {name, email, role}).pipe(
      finalize(() => this.loading = false)
    ).subscribe({
      next: (updated: UserDto) => {
        this.showSuccess('User updated successfully');
        this.dialogRef.close(updated);
      },

    });
  }


  onCancel() {
    this.dialogRef.close();
  }
}
