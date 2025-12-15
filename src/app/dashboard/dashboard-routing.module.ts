import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardLayout } from './dashboard-layout/dashboard-layout';
import { TaskDetailsComponent } from './tasks/task-details/task-details';


const routes: Routes = [
  {
    path: '',
    component: DashboardLayout,
    children: [
      {
        path: 'tasks',
        loadComponent: () => import('./tasks/tasks').then(m => m.Tasks)
      },
      {
        path: 'tasks/create',
        loadComponent: () => import('./task-create/task-create').then(m => m.TaskCreate)
      },
      {
        path: 'idle-employees',
        loadComponent: () => import('./idle-employees/idle-employees').then(m => m.IdleEmployeesComponent)
      },
      {
        path: 'tasks/edit/:id',
        loadComponent: () => import('./task-edit/task-edit').then(m => m.TaskEdit)
      },
      {
        path: 'tasks/:id',
        component: TaskDetailsComponent,
        runGuardsAndResolvers: 'paramsOrQueryParamsChange'
      },
      {
        path: 'users',
        loadComponent: () => import('./users/users').then(m => m.Users)
      },
      {
        path: '',
        redirectTo: 'tasks',
        pathMatch: 'full'
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }
