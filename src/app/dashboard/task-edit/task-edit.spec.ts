import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { of } from 'rxjs';
import { TaskEdit } from './task-edit';
import { TasksService } from '../../core/tasks.service';
import { UserService } from '../../core/user.service';
import { AuthService } from '../../core/auth.service';

describe('TaskEdit', () => {
  let component: TaskEdit;
  let fixture: ComponentFixture<TaskEdit>;
  const routerMock = { navigate: jasmine.createSpy('navigate') };
  const tasksServiceMock = {
    getTaskById: () => of({
      id: 1,
      title: 'Sample task',
      description: 'Desc',
      assignedTo: 'Omar',
      status: 'todo',
      createdAt: '2025-11-10'
    }),
    updateTask: () => of(null)
  };
  const usersServiceMock = {
    getUsers: () => of([])
  };
  const authServiceMock = {
    currentUser$: of({ id: '1', name: 'Admin', email: 'admin@example.com', role: 'admin' })
  };
  const activatedRouteMock = {
    snapshot: { paramMap: { get: () => '1' } }
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskEdit],
      providers: [
        { provide: Router, useValue: routerMock },
        { provide: TasksService, useValue: tasksServiceMock },
        { provide: UserService, useValue: usersServiceMock },
        { provide: AuthService, useValue: authServiceMock },
        { provide: ActivatedRoute, useValue: activatedRouteMock }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TaskEdit);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
