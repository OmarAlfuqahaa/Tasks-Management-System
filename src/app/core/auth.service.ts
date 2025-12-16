import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {BehaviorSubject, Observable} from 'rxjs';
import {API_BASE_URL} from './api.config';

// Payload for login API
export interface LoginPayload {
  email: string;
  password: string;
}

// Interface for authenticated user
export interface AuthenticatedUser {
  id: string;
  name: string;
  email: string;
  role: string;
}

// Response from login API
export interface LoginResponse {
  token: string;
  refreshToken?: string; // Optional refresh token
  user: AuthenticatedUser;
}

// Payload for signup API
export interface SignupPayload {
  name: string;
  email: string;
  password: string;
  role: string;
}

// Response from signup API
export interface SignupResponse {
  user: AuthenticatedUser;
  token?: string; // Optional token
  refreshToken?: string; // Optional refresh token
}

@Injectable({
  providedIn: 'root' // Service is available app-wide
})
export class AuthService {

  private readonly baseUrl = `${API_BASE_URL}/auth`;
  private readonly USER_STORAGE_KEY = 'currentUser'; // Key for storing user in localStorage

  // BehaviorSubject to keep track of current user
  private currentUserSubject = new BehaviorSubject<AuthenticatedUser | null>(this.loadStoredUser());
  readonly currentUser$ = this.currentUserSubject.asObservable(); // Observable for components

  constructor(private http: HttpClient) {
  }

  // Call login API
  login(payload: LoginPayload): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.baseUrl}/login`, payload);
  }

  // Call signup API
  register(payload: SignupPayload): Observable<SignupResponse> {
    return this.http.post<SignupResponse>(`${this.baseUrl}/signup`, payload);
  }

  // Logout user
  logout(): void {
    localStorage.removeItem('authToken'); // Remove token
    localStorage.removeItem('refreshToken'); // Remove refresh token
    this.setCurrentUser(null); // Clear current user
  }

  // Set current user and store in localStorage
  setCurrentUser(user: AuthenticatedUser | null): void {
    if (user) {
      localStorage.setItem(this.USER_STORAGE_KEY, JSON.stringify(user));
    } else {
      localStorage.removeItem(this.USER_STORAGE_KEY);
    }
    this.currentUserSubject.next(user); // Update BehaviorSubject
  }

  // Load stored user from localStorage
  private loadStoredUser(): AuthenticatedUser | null {
    const raw = localStorage.getItem(this.USER_STORAGE_KEY);
    if (!raw) {
      return null; // No user stored
    }
    try {
      return JSON.parse(raw) as AuthenticatedUser; // Parse stored user
    } catch {
      localStorage.removeItem(this.USER_STORAGE_KEY); // Remove invalid entry
      return null;
    }
  }
}
