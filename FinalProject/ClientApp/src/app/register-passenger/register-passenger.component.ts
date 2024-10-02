import { Component, OnInit } from '@angular/core';
import { PassengerService } from './../api/services/passenger.service';
import { FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../auth/auth.service';
import { Router, ActivatedRoute } from '@angular/router';
import { passwordStrengthValidator } from './password-strength.validator'; // Import custom validator

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
  errorMessage: string | null = null;

  form = this.fb.group({
    userName: ['', Validators.compose([Validators.required, Validators.minLength(3), Validators.maxLength(50)])],
    firstName: ['', Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(35)])],
    lastName: ['', Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(35)])],
    email: ['', Validators.email],
    password: ['', Validators.compose([
      Validators.required,
      Validators.minLength(5),
      Validators.maxLength(25),
      passwordStrengthValidator  // Apply the custom validator here
    ])],
    gender: ['', Validators.required],
    packetid: [1],
    purchasePercent: [0],
    cancelPercent: [0],
  });

  ngOnInit(): void {
    this.activatedRoute.params.subscribe(p => this.requestedUrl = p['requestedUrl']);
  }

  register(): void {
    if (this.form.invalid) return;

    this.passengerService.registerPassenger({ body: this.form.value })
      .subscribe(
        () => this.login(),
        error => {
          if (error.status === 409) {
            this.errorMessage = error.error?.message || error.message || "Unknown error";
          } else {
            console.error(error);
            this.errorMessage = "Registration failed. Please try again.";
          }
        }
      );
  }

  private login = () => {
    const { email, password, userName, packetId, purchasePercent, cancelPercent } = this.form.value;
    const user = { email, password, username: userName, packetid: packetId, purchasepercent: purchasePercent, cancelpercent: cancelPercent };
    this.authService.loginUser(user)
    this.router.navigate([this.requestedUrl ?? '/search-flights']);
  }

  togglePasswordVisibility(): void {
    this.showPassword = !this.showPassword;
  }

}
