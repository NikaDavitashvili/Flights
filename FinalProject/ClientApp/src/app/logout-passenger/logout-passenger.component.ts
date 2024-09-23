import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { PassengerService } from './../api/services/passenger.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-logout-passenger',
  templateUrl: './logout-passenger.component.html',
  styleUrls: ['./logout-passenger.component.css']
})
export class LogoutPassengerComponent implements OnInit {


  constructor(
    private fb: FormBuilder,
    private router: Router,
    private passengerService: PassengerService
  ) { }

  requestedUrl?: string = undefined;

  form = this.fb.group({
    email: ['', Validators.compose([Validators.required, Validators.email])],
    password: ['', Validators.compose([Validators.required, Validators.minLength(5), Validators.maxLength(25)])]
  });

  ngOnInit(): void
  {
    this.logout();
  }

  logout(): void {
    this.passengerService.logoutPassenger();

    this.router.navigate([this.requestedUrl ?? '/search-flights']);
  }
}
