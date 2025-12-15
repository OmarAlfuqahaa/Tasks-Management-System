import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_BASE_URL } from './api.config';
import { PagedResult } from './models/paged-result';

// Task data transfer object
export interface Comment {
  author: string;
  text: string;
  createdAt: string;
}

export interface Attachment {
  id: number;
  taskId: number;
  name: string;
  url: string;
  uploadedById: number;
}


export interface TaskDto {
  id: number;
  title: string;
  description?: string;
  assignedUserId: number;
  assignedTo: string;
  status: 'todo' | 'in-progress' | 'done';
  createdAt: string;
  createdBy?: string;
  attachments?: Attachment[];
  comments?: Comment[];
}


// Payload for creating a new task
export interface CreateTaskPayload {
  title: string;
  description?: string;
  assignedUserId: number | null;
  status: TaskDto['status'];
  createdBy?: string;
  attachments?: Attachment[];
}

// Payload for updating a task
export interface UpdateTaskPayload {
  title: string;
  description?: string;
  assignedUserId: number;
  status: TaskDto['status'];
  attachments?: Attachment[];
}

export interface CommentDto {
  id: number;
  taskId: number;
  authorId: number;
  authorName: string;
  text: string;
  createdAt: string;
}





@Injectable({
  providedIn: 'root' // Service available app-wide
})
export class TasksService {

  private readonly baseUrl = `${API_BASE_URL}/tasks`; // Base API endpoint

  constructor(private http: HttpClient) {}

  // Create a new task
  createTask(payload: CreateTaskPayload): Observable<TaskDto> {
    return this.http.post<TaskDto>(this.baseUrl, payload);
  }

  // Get tasks with pagination/filtering
  getTasks(query: {
    page?: number;
    pageSize?: number;
    status?: string;
    search?: string;
    assignedUserIds?: number[]
  } = {}): Observable<PagedResult<TaskDto>> {
    let params = new HttpParams();
    if (query.page) params = params.set('page', query.page.toString());
    if (query.pageSize) params = params.set('pageSize', query.pageSize.toString());
    if (query.status) params = params.set('status', query.status);
    if (query.search) params = params.set('search', query.search);

    if (query.assignedUserIds && query.assignedUserIds.length) {
      query.assignedUserIds.forEach(id => {
        params = params.append('assignedUserIds', id.toString());
      });
    }

    return this.http.get<PagedResult<TaskDto>>(this.baseUrl, { params });
  }



  // Get task by ID
  getTaskById(id: number): Observable<TaskDto> {
    return this.http.get<TaskDto>(`${this.baseUrl}/${id}`);
  }

  // Update a task
  updateTask(id: number, payload: UpdateTaskPayload): Observable<TaskDto> {
    return this.http.put<TaskDto>(`${this.baseUrl}/${id}`, payload);
  }

  // Delete a task
  deleteTask(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  getComments(taskId: number): Observable<CommentDto[]> {
    return this.http.get<CommentDto[]>(`${this.baseUrl}/${taskId}/comments`);
  }

  addComment(taskId: number, text: string): Observable<CommentDto> {
    return this.http.post<CommentDto>(`${this.baseUrl}/${taskId}/comments`, { text });
  }





  // TasksService
  uploadAttachment(taskId: number, file: File): Observable<TaskDto> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<TaskDto>(`${this.baseUrl}/${taskId}/attachments`, formData);
  }


  deleteAttachment(taskId: number, attachmentId: number) {
    return this.http.delete(`${this.baseUrl}/${taskId}/attachments/${attachmentId}`);
  }


}
