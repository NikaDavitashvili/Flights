import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FlightService } from './../api/services/flight.service';
import { BookingService } from './../api/services/booking.service';
import { BookDto, FlightRm } from '../api/models';
import { AuthService } from '../auth/auth.service';
import { FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-book-flight',
  templateUrl: './book-flight.component.html',
  styleUrls: ['./book-flight.component.css']
})
export class BookFlightComponent implements OnInit {
  currentFlight: FlightRm | null = null;
  currentUser: User | null = null;
  purchasePercent: number = 0;
  paymentAmount: number = 0;
  showCardInsertionPopup = false;
  cardNumber = '';
  expiryDate = '';
  cvv = '';
  showErrorMessage = false;
  seasonName: string | null = null; 

  constructor(private route: ActivatedRoute,
    private router: Router,
    private flightService: FlightService,
    private bookService: BookingService,
    private authService: AuthService,
    private fb: FormBuilder) { }

  flightId: string = 'not loaded';
  flight: FlightRm = {};

  form = this.fb.group({
    number: [1, Validators.compose([Validators.required, Validators.min(1), Validators.max(10)])]
  });

  ngOnInit(): void {
    const flightData = this.bookService.getFlight();

    // Use type assertion to avoid the error
    this.flight = flightData as FlightRm;

    // Safely check if flightData is not null
    if (flightData) {
      console.log('Flight:', this.flight);  // Logs the flight object
    } else {
      console.log('No flight data available');
    }

    console.log("BOOKFLIGHT");
    this.route.paramMap.subscribe(p => this.findFlight(p.get("flightId")));
    const userJson = sessionStorage.getItem('CurrentUser');
    if (userJson) {
      this.currentUser = JSON.parse(userJson);
    }
  }

  private findFlight = (flightId: string | null) => {
    this.flightId = flightId ?? 'not passed';

    this.flightService.findFlight({ id: this.flightId })
      .subscribe(flight => this.flight = flight, this.handleError);
  }

  private handleError = (err: any) => {
    if (err.status === 404) {
      alert("Flight not found!");
      this.router.navigate(['/search-flights']);
    }

    if (err.status === 409) {
      console.log("err: " + err);
      alert(JSON.parse(err.error).message);
    }

    console.log("Response Error. Status: ", err.status);
    console.log("Response Error. Status Text: ", err.statusText);
    console.log(err);
  }

  purchase(flight: FlightRm): void {
    if (this.form.invalid) {
      return;
    }

    const numberOfTickets = this.form.get('number')?.value;
    if (numberOfTickets <= 0) {
      alert('Please enter a valid number of tickets to cancel.');
      return;
    }
    const priceAsInt = parseInt(flight.price!.toString(), 10);
    this.currentFlight = flight;

    const userJson = sessionStorage.getItem('CurrentUser');
    if (userJson) {
      const user: User = JSON.parse(userJson);
      this.purchasePercent = 1 - user.purchasepercent / 100;
    }

    this.paymentAmount = priceAsInt * numberOfTickets * this.purchasePercent;

    // Open the card insertion popup here
    this.showCardInsertionPopup = true;

    // Disable scroll
    document.body.classList.add('no-scroll');
  }

  /*buyTickets() {
    if (!this.cardNumber || !this.expiryDate || !this.cvv) {
      alert('Please enter all card details.');
      return;
    }

    //const booking: BookDto = {
    //  flightId: this.flight.id,
    //  passengerEmail: this.authService.currentUser?.email || '',
    //  numberOfSeats: this.form.get('number')?.value
    //};
    const numberOfSeats = this.form.get('number')?.value;
    console.log(numberOfSeats);
    this.flight.seatsToBuy = numberOfSeats;

    this.flightService.bookFlight({ body: this.flight })
      .subscribe(_ => this.router.navigate(['/my-booking']), this.handleError);
    this.closePopup();
  }*/

  buyTickets() {
    if (!this.cardNumber || !this.expiryDate || !this.cvv) {
      alert('Please enter all card details.');
      return;
    }

    const numberOfSeats = this.form.get('number')?.value;
    console.log(numberOfSeats);
    this.flight.seatsToBuy = numberOfSeats;
    const booking: BookDto = {
      flightId: this.flight.id,
      passengerEmail: this.authService.currentUser?.email || '',
      numberOfSeats: this.form.get('number')?.value
    };

    // Simulate a fake delay of 3 seconds
    setTimeout(() => {
      this.showErrorMessage = true;
    this.flightService.bookFlight({ body: booking })
      .subscribe(_ => this.router.navigate(['/my-booking']), this.handleError);

      // Optionally auto-hide the message after 5 seconds
      setTimeout(() => {
        this.showErrorMessage = false;
        this.closePopup();
      }, 5000);  // Hide after 5 seconds
    }, 1000);  // Show error after 3 seconds
    this.closePopup();
  }

  closePopup() {
    this.showCardInsertionPopup = false;

    // Enable scroll
    document.body.classList.remove('no-scroll');
  }

  get number() {
    return this.form.get('number');
  }

  get cardNumberInvalid() {
    // Basic validation for card number
    return this.cardNumber.length < 16 || isNaN(Number(this.cardNumber));
  }

  getDiscountedPrice(price: any): number | null {
    this.seasonName = sessionStorage.getItem('SeasonName');

    var seasonDiscountedPrice = 0;
    if (this.currentUser && this.currentUser.packetid !== 1) {
      var discountedPrice = (price * (1 - this.currentUser.purchasepercent / 100));

      if (this.seasonName === 'Summer')
        seasonDiscountedPrice = discountedPrice * 0.85;
      else seasonDiscountedPrice = discountedPrice * 0.90;
    }
    else {
      if (this.seasonName === 'Summer')
        seasonDiscountedPrice = price * 0.85;
      else seasonDiscountedPrice = price * 0.90;
    }
    return Math.round(seasonDiscountedPrice);
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
