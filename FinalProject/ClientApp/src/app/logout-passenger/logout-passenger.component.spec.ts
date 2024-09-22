import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LogoutPassengerComponent } from './logout-passenger.component';

describe('LogoutPassengerComponent', () => {
  let component: LogoutPassengerComponent;
  let fixture: ComponentFixture<LogoutPassengerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LogoutPassengerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LogoutPassengerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
