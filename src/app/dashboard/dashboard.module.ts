import { NgModule } from '@angular/core';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardLayout } from './dashboard-layout/dashboard-layout';

@NgModule({
  imports: [
    DashboardRoutingModule,
    DashboardLayout
  ]
})
export class DashboardModule { }
