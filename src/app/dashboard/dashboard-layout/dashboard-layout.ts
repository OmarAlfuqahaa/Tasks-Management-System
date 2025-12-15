import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { AuthService, AuthenticatedUser } from '../../core/auth.service';
import {DashboardService} from '../dashboard.service';
import {MatTooltipModule} from '@angular/material/tooltip';
import * as signalR from '@microsoft/signalr';



// Interface for sidebar menu items
interface MenuItem {
  label: string; // Menu text
  icon: string; // Menu icon
  route?: string; // Optional route path
  action?: () => void; // Optional function to execute
  roles: Role[]; // Roles that can see this item
}

interface TaskCount {
  userId: number;
  userName: string;
  todoCount: number;
  inProgressCount: number;
  doneCount: number;
  totalTasks: number;
}

interface DashboardStats {
  employees: number;
  projectManagers: number;
  employeesWithActiveTasks: number;
  employeesWithoutActiveTasks: number;
  todo: number;
  inProgress: number;
  done: number;
  taskCounts: TaskCount[];
}



// Type for user roles
type Role = AuthenticatedUser['role'] | null;

@Component({
  selector: 'app-dashboard-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    MatToolbarModule,
    MatTooltipModule
  ],
  templateUrl: './dashboard-layout.html',
  styleUrls: ['./dashboard-layout.css']
})
export class DashboardLayout implements OnInit, OnDestroy {

  menuItems: MenuItem[] = [];
  currentUser: AuthenticatedUser | null = null;
  private currentRole: Role = null;

  userMessage: string = '';
  stats: DashboardStats = {
    employees: 0,
    projectManagers: 0,
    employeesWithActiveTasks: 0,
    employeesWithoutActiveTasks: 0,
    todo: 0,
    inProgress: 0,
    done: 0,
    taskCounts: []
  };
  private hubConnection!: signalR.HubConnection;
  welcomeMessage: string = '';
  private signalRStarted = false;





  private subscriptions = new Subscription();

  constructor(
    private authService: AuthService,
    private dashboardService: DashboardService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.subscriptions.add(
      this.authService.currentUser$.subscribe(user => {
        this.currentUser = user;
        this.currentRole = user?.role ?? null;
        this.menuItems = this.buildMenu(this.currentRole);

        this.dashboardService.loadStats();

        if (user && !this.signalRStarted) {
          this.startSignalRConnection();
          this.signalRStarted = true;
        }

        this.subscriptions.add(
          this.dashboardService.dashboardStats$.subscribe(stats => {
            this.stats = stats;
            this.updateWelcomeMessage();
          })
        );
      })
    );
  }


  private startSignalRConnection(): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5258/dashboardHub', { withCredentials: true })
      .withAutomaticReconnect([0, 2000, 10000, 30000]) // retry بعد 0ms, 2s, 10s, 30s
      .build();

    const startConnection = () => {
      this.hubConnection
        .start()
        .then(() => console.log('✅ SignalR Connected'))
        .catch(err => {
          console.error('❌ SignalR Error:', err);
        });
    };

    startConnection();

    this.hubConnection.onclose(error => {
      console.warn('⚠️ SignalR connection closed', error);
    });

    this.hubConnection.on('ReceiveWelcomeMessage', (message: string) => {
      console.log('📩 Message from server:', message);
      this.welcomeMessage = message;
      this.dashboardService.loadStats();
    });

    this.hubConnection.on('DashboardUpdated', () => {
      console.log('📩 Dashboard updated');
      this.dashboardService.loadStats();
    });
  }






  ngOnDestroy(): void {
    this.subscriptions.unsubscribe(); // Cleanup subscriptions
  }

  // Handle menu item click
  onNavItemClick(item: MenuItem, event: MouseEvent) {
    if (item.action) {
      event.preventDefault(); // Prevent default navigation
      item.action(); // Execute action
    }
  }

  // Permission checks
  canViewUsers(): boolean {
    return this.currentRole === 'admin';
  }

  canViewProjects(): boolean {
    return this.currentRole === 'admin' || this.currentRole === 'project-manager';
  }

  canViewTasks(): boolean {
    return !!this.currentRole;
  }

  // Build menu based on role
  private buildMenu(role: Role): MenuItem[] {
    if (!role) {
      return [];
    }

    const logoutAction = () => this.logout(); // Logout function

    const definitions: MenuItem[] = [
      { label: 'Users', icon: 'people', route: '/dashboard/users', roles: ['admin'] },
      { label: role === 'employee' ? 'My Tasks' : 'Tasks', icon: 'assignment', route: '/dashboard/tasks', roles: ['admin', 'project-manager', 'employee'] },
      { label: 'Logout', icon: 'logout', action: logoutAction, roles: ['admin', 'project-manager', 'employee'] }
    ];

    return definitions.filter(item => item.roles.includes(role)); // Return items allowed for role
  }

  // Logout function
  private logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }


  private updateWelcomeMessage(): void {
    if (!this.currentUser) return;

    switch (this.currentUser.role) {
      case 'admin':
        this.userMessage =
          `👑 Admin Dashboard — 👥 Employees: ${this.stats.employees} • 🧑‍💼 PMs: ${this.stats.projectManagers} • System running smoothly `;
        break;

      case 'project-manager':
        // this.userMessage =
        //   `🧑‍💼 Project Manager — 👨‍💻 Employees: ${this.stats.employees} • ⭕ ${this.stats.employeesWithoutActiveTasks} have no tasks • Manage your team 🔥`;
        break;

      case 'employee':
        const userId = Number(this.currentUser.id);
        const userTasks = this.stats.taskCounts.find(t => t.userId === userId);
        const todo = userTasks?.todoCount ?? 0;
        const inProgress = userTasks?.inProgressCount ?? 0;
        const done = userTasks?.doneCount ?? 0;

        this.userMessage =
          `You have: 📝 ${todo} Todo • ⚙️ ${inProgress} In Progress • ✅ ${done} Done`;
        break;
    }
  }

  // dashboard-layout.ts
  openIdleEmployees() {
    this.router.navigate(['/dashboard/idle-employees']);
  }



}
