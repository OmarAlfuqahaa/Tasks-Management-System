import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { API_BASE_URL } from '../core/api.config';


// User DTO
export interface UserDto {
  id: number;
  name: string;
  email: string;
  role: 'admin' | 'employee' | 'project-manager';
  createdAt?: string;
}

// Payload for updating user
export interface UpdateUserPayload {
  name?: string;
  email?: string;
  role?: UserDto['role'];
}

// Pagination result
export interface PaginatedUsersResponse {
  items: UserDto[];
  totalItems: number;
  page: number;
  pageSize: number;
}

// Query parameters for filtering users
export interface UsersQuery {
  page?: number;
  pageSize?: number;
  role?: string;
  search?: string;
}

@Injectable({ providedIn: 'root' })
export class UsersService {

  private readonly baseUrl = `${API_BASE_URL}/users`;

  constructor(private http: HttpClient) {}

// Get paginated users with optional search & role filter
  getUsers(query: UsersQuery = {}): Observable<PaginatedUsersResponse> {
    const params = this.buildQueryParams(query);
    return this.http.get<PaginatedUsersResponse>(this.baseUrl, { params });
  }

  getEmployees(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(`${this.baseUrl}/employees`);
  }


// Update a user by ID
  updateUser(id: number, payload: UpdateUserPayload): Observable<UserDto> {
    return this.http.put<UserDto>(`${this.baseUrl}/${id}`, payload);
  }

// Delete a user by ID
  deleteUser(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

// Helper to build HttpParams from query object
  private buildQueryParams(query: UsersQuery): HttpParams {
    let params = new HttpParams();
    if (query.page) params = params.set('page', query.page.toString());
    if (query.pageSize) params = params.set('pageSize', query.pageSize.toString());
    if (query.role && query.role !== 'All') params = params.set('role', query.role);
    if (query.search) params = params.set('search', query.search);
    return params;
  }



}
