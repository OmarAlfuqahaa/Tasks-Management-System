import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { Users } from './users';
import { UserService } from '../../core/user.service';
import { AuthService } from '../../core/auth.service';

describe('Users', () => {
  let component: Users;
  let fixture: ComponentFixture<Users>;
  let usersServiceSpy: jasmine.SpyObj<UserService>;
  const authServiceMock = {
    currentUser$: of({ id: '1', name: 'Admin', email: 'admin@example.com', role: 'admin' })
  };

  beforeEach(async () => {
    usersServiceSpy = jasmine.createSpyObj('UsersService', ['getUsers', 'deleteUser']);
    usersServiceSpy.getUsers.and.returnValue(of([]));
    usersServiceSpy.deleteUser.and.returnValue(of(void 0));

    await TestBed.configureTestingModule({
      imports: [Users],
      providers: [
        { provide: UserService, useValue: usersServiceSpy },
        { provide: AuthService, useValue: authServiceMock }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Users);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should request users on init', () => {
    expect(usersServiceSpy.getUsers).toHaveBeenCalled();
    expect(component.filteredUsers).toEqual([]);
  });
});
