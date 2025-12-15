import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import { of } from 'rxjs';
import { Tasks } from './tasks';
import { TasksService } from '../../core/tasks.service';
import { AuthService } from '../../core/auth.service';

describe('Tasks', () => {
  let component: Tasks;
  let fixture: ComponentFixture<Tasks>;
  const routerMock = { navigate: jasmine.createSpy('navigate') };
  const tasksServiceMock = {
    getTasks: () => of([]),
    deleteTask: () => of(void 0)
  };
  const authServiceMock = {
    currentUser$: of({ id: '1', name: 'Admin', email: 'admin@example.com', role: 'admin' })
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Tasks],
      providers: [
        { provide: Router, useValue: routerMock },
        { provide: TasksService, useValue: tasksServiceMock },
        { provide: AuthService, useValue: authServiceMock }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Tasks);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
