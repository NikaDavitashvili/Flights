import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../auth/auth.service';
import { Router, ActivatedRoute } from '@angular/router';
import { PassengerService } from './../api/services/passenger.service';

@Component({
  selector: 'app-login-passenger',
  templateUrl: './login-passenger.component.html',
  styleUrls: ['./login-passenger.component.css']
})
export class LoginPassengerComponent implements OnInit {

  errorMessage: string | null = null;
  showPassword: boolean = false;
  notificationMessage: string = '';
  notificationType: 'success' | 'error' | 'info' | 'warning' = 'info';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private passengerService: PassengerService
  ) { }

  requestedUrl?: string = undefined;

  form = this.fb.group({
    email: ['', Validators.compose([Validators.required, Validators.email])],
    password: ['', Validators.compose([Validators.required, Validators.minLength(5), Validators.maxLength(25)])]
  });

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe(params => {
      if (params['message']) {
        this.notificationMessage = params['message']; // Set message if available
        this.notificationType = 'success';
        // Hide the message after 5 seconds
        setTimeout(() => {
          this.notificationMessage = ''; // Set to empty string instead of null
        }, 5000);
      }
    });

    sessionStorage.removeItem("SeasonName");

    this.activatedRoute.params.subscribe(p => this.requestedUrl = p['requestedUrl']);
  }

  login(): void {
    if (this.form.invalid) return;

    const { email, password } = this.form.value;

    this.passengerService.loginPassenger(email, password).subscribe(
      (user) => {
        this.errorMessage = null;
        this.loginUser(user);
      },
      error => {
        if (error.status === 404) {
          this.errorMessage = 'Invalid email or password.';
        } else if (error.status === 401) {
          // Handle the case where the user is not verified
          this.errorMessage = error.error.message || 'Please verify your email before logging in.';
        } else {
          this.errorMessage = 'An error occurred. Please try again later.';
        }
        console.error(error);
      }
    );
  }

  private loginUser(responseUser: any): void {
    const { email, password } = this.form.value;
    const user = { email, password, username: responseUser.userName, packetid: responseUser.packetId, purchasepercent: responseUser.purchasePercent, cancelpercent: responseUser.cancelPercent };
    this.authService.loginUser(user);
    this.router.navigate([this.requestedUrl ?? '/search-flights']);
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword; 
  }
}
