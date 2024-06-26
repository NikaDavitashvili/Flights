import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AuthGuard } from './auth/auth.guard';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { SearchFlightsComponent } from './search-flights/search-flights.component';
import { BookFlightComponent } from './book-flight/book-flight.component';
import { RegisterPassengerComponent } from './register-passenger/register-passenger.component';
import { MyBookingsComponent } from './my-bookings/my-bookings.component';
import { AuthService } from './auth/auth.service';
import { LoginPassengerComponent } from './login-passenger/login-passenger.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    SearchFlightsComponent,
    BookFlightComponent,
    RegisterPassengerComponent,
    LoginPassengerComponent,
    MyBookingsComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: '', component: SearchFlightsComponent, pathMatch: 'full' },
      { path: 'search-flights', component: SearchFlightsComponent },
      { path: 'book-flight/:flightId', component: BookFlightComponent, canActivate: [AuthGuard] },
      { path: 'login-passenger', component: LoginPassengerComponent },
      { path: 'register-passenger', component: RegisterPassengerComponent },
      { path: 'my-booking', component: MyBookingsComponent, canActivate: [AuthGuard]  }

    ])
  ],
  providers: [AuthService],
  bootstrap: [AppComponent]
})
export class AppModule { }
