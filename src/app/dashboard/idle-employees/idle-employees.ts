import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DashboardService } from '../dashboard.service';
import {
  MatCell, MatCellDef,
  MatColumnDef,
  MatHeaderCell,
  MatHeaderCellDef,
  MatHeaderRow, MatHeaderRowDef,
  MatRow, MatRowDef,
  MatTable
} from '@angular/material/table';
import {MatProgressBar} from '@angular/material/progress-bar';
import { MatIconModule } from '@angular/material/icon';
import {NgClass, NgIf} from '@angular/common';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatButtonModule, MatIconButton} from '@angular/material/button';

export interface IdleEmployee {
  id: number;
  name: string;
  email: string;
  totalTasks: number;
  status: 'IDLE' | 'DONE';
}

@Component({
  selector: 'app-idle-employees',
  templateUrl: './idle-employees.html',
  standalone: true,
  imports: [
    MatIconModule,
    MatTable,
    MatProgressBar,
    NgIf,
    MatRow,
    MatHeaderRow,
    MatTooltipModule,
    MatCell,
    MatHeaderCell,
    MatColumnDef,
    MatHeaderCellDef,
    MatCellDef,
    MatRowDef,
    MatHeaderRowDef,
    MatButtonModule,
    NgClass,
    MatIconButton
  ],
  styleUrls: ['./idle-employees.css']
})
export class IdleEmployeesComponent implements OnInit {

  displayedColumns: string[] = [
    'name',
    'email',
    'totalTasks',
    'status',
    'action'
  ];

  employees: IdleEmployee[] = [];
  loading = true;

  constructor(
    private dashboardService: DashboardService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadIdleEmployees();
  }

  loadIdleEmployees(): void {
    this.loading = true;

    this.dashboardService.getIdleEmployees().subscribe({
      next: (res) => {
        this.employees = res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  assignTask(employee: IdleEmployee): void {
    this.router.navigate(['/dashboard/tasks/create'], {
      queryParams: { employeeId: employee.id }
    });
  }

  goToTasks(employeeId: number): void {
    this.router.navigate(['/dashboard/tasks'], {
      queryParams: { assignedUserIds: [employeeId] }
    });
  }





}
