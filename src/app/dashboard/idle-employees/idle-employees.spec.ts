import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IdleEmployeesDialog } from './idle-employees';

describe('IdleEmployeesDialog', () => {
  let component: IdleEmployeesDialog;
  let fixture: ComponentFixture<IdleEmployeesDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [IdleEmployeesDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(IdleEmployeesDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
