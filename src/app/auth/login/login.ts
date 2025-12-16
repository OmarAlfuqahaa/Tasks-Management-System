import {Component, OnInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ReactiveFormsModule, FormGroup, FormControl, Validators} from '@angular/forms';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatButtonModule} from '@angular/material/button';
import {Router, RouterLink} from '@angular/router';
import {finalize} from 'rxjs';
import {HttpErrorResponse} from '@angular/common/http';
import {AuthService, LoginPayload, LoginResponse} from '../../core/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    RouterLink
  ],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class Login implements OnInit {
  // Define login form without rememberMe
  loginForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required])
  });

  loginError: string | null = null;
  loading = false;

  constructor(
    private router: Router,
    private authService: AuthService
  ) {
  }

  ngOnInit() {

  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.loginError = null;
      this.loading = true;

      const {email, password} = this.loginForm.value as LoginPayload;

      const payload: LoginPayload = {
        email: email || '',
        password: password || ''
      };

      this.authService.login(payload).pipe(
        finalize(() => this.loading = false)
      ).subscribe({
        next: (response: LoginResponse) => {
          // Save tokens if available
          if (response.token) {
            localStorage.setItem('authToken', response.token);
          }
          if (response.refreshToken) {
            localStorage.setItem('refreshToken', response.refreshToken);
          }
          if (response.user) {
            this.authService.setCurrentUser(response.user);
          }

          this.router.navigate(['/dashboard']);
        },
        error: (error: HttpErrorResponse) => {
          this.loginError = error?.error?.message || 'Unable to login. Please try again.';
        }
      });
    } else {
      this.loginError = 'Please fill all required fields correctly';
    }
  }
}
