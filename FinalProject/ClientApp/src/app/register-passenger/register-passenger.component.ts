import { Component, OnInit } from '@angular/core';
import { PassengerService } from './../api/services/passenger.service';
import { FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../auth/auth.service';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-register-passenger',
  templateUrl: './register-passenger.component.html',
  styleUrls: ['./register-passenger.component.css']
})
export class RegisterPassengerComponent implements OnInit {

  constructor(private passengerService: PassengerService,
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private activatedRoute: ActivatedRoute) { }

  requestedUrl?: string = undefined;
  showPassword: boolean = false;

  form = this.fb.group({
    userName: ['', Validators.compose([Validators.required, Validators.minLength(3), Validators.maxLength(50)])],
    firstName: ['', Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(35)])],
    lastName: ['', Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(35)])],
    email: ['', Validators.email],
    password: ['', Validators.compose([Validators.required, Validators.minLength(5), Validators.maxLength(25)])],
    gender: ['', Validators.required]
  });

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(p => this.requestedUrl = p['requestedUrl']);
  }

  register(): void {
    if (this.form.invalid)
      return;

    this.passengerService.registerPassenger({ body: this.form.value })
      .subscribe(
        () => this.login(),
        error => console.error(error)
      );
  }

  private login = () => {
    const { email, password, userName } = this.form.value;
    const user = { email, password, username: userName };
    this.authService.loginUser(user)
    this.router.navigate([this.requestedUrl ?? '/search-flights'])
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

}
