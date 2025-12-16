import {ComponentFixture, TestBed} from '@angular/core/testing';
import {of} from 'rxjs';
import {Router} from '@angular/router';
import {TaskCreate} from './task-create';
import {TasksService} from '../../core/tasks.service';
import {AuthService} from '../../core/auth.service';

describe('TaskCreate', () => {
  let component: TaskCreate;
  let fixture: ComponentFixture<TaskCreate>;
  const routerMock = {navigate: jasmine.createSpy('navigate')};
  const tasksServiceMock = {createTask: () => of(null)};
  const usersServiceMock = jasmine.createSpyObj('UsersService', ['getUsers']);
  usersServiceMock.getUsers.and.returnValue(of([]));
  const authServiceMock = {
    currentUser$: of({id: '1', name: 'Admin', email: 'admin@example.com', role: 'admin'})
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskCreate],
      providers: [
        {provide: Router, useValue: routerMock},
        {provide: TasksService, useValue: tasksServiceMock},
        {provide: AuthService, useValue: authServiceMock}
      ]
    })
      .compileComponents();

    fixture = TestBed.createComponent(TaskCreate);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load assignees on init', () => {
    expect(usersServiceMock.getUsers).toHaveBeenCalled();
    expect(component.assignees).toEqual([]);
  });
});
