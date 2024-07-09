import { Component, OnInit } from '@angular/core';
import { BookingRm, BookDto } from '../api/models';
import { BookingService } from '../api/services/booking.service';
import { AuthService } from '../auth/auth.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-my-bookings',
  templateUrl: './my-bookings.component.html',
  styleUrls: ['./my-bookings.component.css']
})
export class MyBookingsComponent implements OnInit {

  bookings: BookingRm[] = [];
  currentUser: User | null = null;
  form!: FormGroup;
  showReturnPopup: boolean = false;
  currentBooking: BookingRm | null = null;
  returnAmount: number = 0;
  cancelPercent: number = 0;

  constructor(
    private bookingService: BookingService,
    private authService: AuthService,
    private formBuilder: FormBuilder
  ) { }

  ngOnInit(): void {
    this.loadBookings();
    this.initializeForm();
    const userJson = sessionStorage.getItem('CurrentUser');
    if (userJson) {
      this.currentUser = JSON.parse(userJson);
    }
  }

  private loadBookings(): void {
    this.bookingService.listBooking({ email: this.authService.currentUser?.email ?? '' })
      .subscribe(
        (bookings: BookingRm[]) => {
          this.bookings = bookings;
        },
        (error) => {
          this.handleError(error);
        }
      );
  }

  private initializeForm(): void {
    this.form = this.formBuilder.group({
      number: ['', [Validators.required, Validators.min(1), Validators.max(10)]]
    });
  }

  private handleError(err: any): void {
    console.error('Response Error, Status:', err.status);
    console.error('Response Error, Status Text:', err.statusText);
    console.error(err);
  }

  openReturnPopup(booking: BookingRm): void {
    const numberOfTickets = this.form.get('number')?.value;
    if (numberOfTickets <= 0) {
      alert('Please enter a valid number of tickets to cancel.');
      return;
    }
    const priceAsInt = parseInt(booking.price!.toString(), 10);
    this.currentBooking = booking;

    const userJson = sessionStorage.getItem('CurrentUser');
    if (userJson) {
      const user: User = JSON.parse(userJson);
      this.cancelPercent = user.cancelpercent / 100;
    }


    this.returnAmount = (priceAsInt * numberOfTickets) * (this.cancelPercent ? this.cancelPercent : 1);
    console.log(this.returnAmount);
    this.showReturnPopup = true;
    document.body.classList.add('no-scroll');
  }

  closeReturnPopup(): void {
    this.showReturnPopup = false;
    this.currentBooking = null;
    document.body.classList.remove('no-scroll');
  }

  cancel(): void {
    const booking = this.currentBooking;
    const numberOfTickets = this.form.get('number')?.value;

    if (!booking) {
      alert('Invalid booking!');
      return;
    }

    if (numberOfTickets <= 0) {
      alert('Please enter a valid number of tickets to cancel.');
      return;
    }

    const dto: BookDto = {
      flightId: booking.flightId,
      numberOfSeats: numberOfTickets,
      passengerEmail: booking.passengerEmail
    };

    this.bookingService.cancelBooking({ body: dto })
      .subscribe(
        (_) => {
          // Remove the canceled booking from the local list
          this.bookings = this.bookings.filter(b => b !== booking);
          this.loadBookings();
          this.closeReturnPopup();
        },
        (error) => {
          this.handleError(error);
        }
      );
  }

  getDiscountedPrice(price: any): number | null {
    if (this.currentUser && this.currentUser.packetid !== 1) {
      var discountedPrice = (price * (1 - this.currentUser.purchasepercent / 100));
      return Math.round(discountedPrice);
    }
    return null;
  }

}
interface User {
  email: string;
  password: string;
  username: string;
  packetid: number;
  purchasepercent: number;
  cancelpercent: number;
}
