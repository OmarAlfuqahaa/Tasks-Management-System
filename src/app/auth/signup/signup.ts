import {Component} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ReactiveFormsModule, FormGroup, FormControl, Validators, AbstractControl} from '@angular/forms';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatButtonModule} from '@angular/material/button';
import {MatSelectModule} from '@angular/material/select';
import {Router} from '@angular/router';
import {finalize} from 'rxjs';
import {HttpErrorResponse} from '@angular/common/http';
import {AuthService, SignupPayload, SignupResponse} from '../../core/auth.service';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule
  ],
  templateUrl: './signup.html',
  styleUrls: ['./signup.css']
})
export class SignupComponent {
  loading = false;
  apiError: string | null = null;
  successMessage: string | null = null;

  constructor(private router: Router, private authService: AuthService) {
  }

  signupForm = new FormGroup(
    {
      name: new FormControl('', [Validators.required, Validators.minLength(3)]),
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required, Validators.minLength(6)]),
      confirmPassword: new FormControl('', Validators.required),
      role: new FormControl('', Validators.required)
    },
    {validators: SignupComponent.passwordsMatchValidator}
  );

  static passwordsMatchValidator(form: AbstractControl) {
    const password = form.get('password')?.value;
    const confirmPassword = form.get('confirmPassword')?.value;
    return password === confirmPassword ? null : {passwordsMismatch: true};
  }

  onSubmit() {
    if (this.signupForm.invalid) {
      this.signupForm.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.apiError = null;
    this.successMessage = null;

    const payload: SignupPayload = {
      name: this.signupForm.value.name || '',
      email: this.signupForm.value.email || '',
      password: this.signupForm.value.password || '',
      role: this.signupForm.value.role || ''
    };

    this.authService.register(payload).pipe(
      finalize(() => this.loading = false)
    ).subscribe({
      next: (response: SignupResponse) => {
        if (response.token) localStorage.setItem('authToken', response.token);
        if (response.refreshToken) localStorage.setItem('refreshToken', response.refreshToken);

        this.successMessage = 'Account created successfully!';
        this.signupForm.reset();
        this.router.navigate(['/login']);
      },
      error: (error: HttpErrorResponse) => {
        this.apiError = error?.error?.message || 'Unable to create account. Please try again.';
      }
    });
  }
}
