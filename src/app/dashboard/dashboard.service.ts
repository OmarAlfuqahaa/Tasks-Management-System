import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {BehaviorSubject} from 'rxjs';
import {IdleEmployee} from './idle-employees/idle-employees';


export interface DashboardStats {
  employees: number;
  projectManagers: number;
  employeesWithActiveTasks: number;
  employeesWithoutActiveTasks: number;
  todo: number;
  inProgress: number;
  done: number;
  taskCounts: TaskCount[];
}

export interface TaskCount {
  userId: number;
  userName: string;
  todoCount: number;
  inProgressCount: number;
  doneCount: number;
  totalTasks: number;
}

@Injectable({providedIn: 'root'})
export class DashboardService {
  private baseUrl = 'http://localhost:5258/api/dashboard';
  private statsSubject = new BehaviorSubject<DashboardStats>({
    employees: 0,
    projectManagers: 0,
    employeesWithActiveTasks: 0,
    employeesWithoutActiveTasks: 0,
    todo: 0,
    inProgress: 0,
    done: 0,
    taskCounts: []
  });

  dashboardStats$ = this.statsSubject.asObservable();

  constructor(private http: HttpClient) {
  }

  loadStats() {
    this.http
      .get<DashboardStats>(`${this.baseUrl}/welcome`)
      .subscribe(stats => this.statsSubject.next(stats));
  }


  getIdleEmployees() {
    return this.http.get<IdleEmployee[]>(`${this.baseUrl}/idle-employees`);
  }
}
