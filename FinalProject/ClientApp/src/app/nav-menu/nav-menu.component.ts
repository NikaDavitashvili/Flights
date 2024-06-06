import { Component, ElementRef, ViewChild } from '@angular/core';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  @ViewChild('loginModal') loginModal!: ElementRef;
  isExpanded = false;
  loginData = {
    email: '',
    password: ''
  };

  constructor(public authService: AuthService) { }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  onSubmit(event: Event) {
    event.preventDefault();
    this.authService.login(this.loginData.email, this.loginData.password).subscribe(response => {
      this.authService.loginUser(response.user); // Assuming the response contains user data
    }, error => {
      console.error('Login error', error);
    });
  }
}
