import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoginPassengerComponent } from './login-passenger.component';

describe('LoginPassengerComponent', () => {
  let component: LoginPassengerComponent;
  let fixture: ComponentFixture<LoginPassengerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LoginPassengerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginPassengerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
