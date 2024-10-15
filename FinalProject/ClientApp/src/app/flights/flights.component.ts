import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup } from '@angular/forms';
import { FlightService } from './../api/services/flight.service';
import { FlightRm } from '../api/models';

@Component({
  selector: 'app-flights',
  templateUrl: './flights.component.html',
  styleUrls: ['./flights.component.css']
})
export class FlightsComponent implements OnInit {

  searchForm: FormGroup;
  searchResult: FlightRm[] = [];
  currentUser: User | null = null;
  departureDateType: string = 'date';  // 'date' or 'month'
  returnDateType: string = 'date';     // 'date' or 'month'

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder,
    private flightService: FlightService
  ) {
    // Initialize the search form
    this.searchForm = this.fb.group({
      from: [''],
      destination: [''],
      departureDateType: ['date'],
      fromDate: [''],
      fromMonth: [''],
      returnDateType: ['date'],
      toDate: [''],
      toMonth: [''],
      numberOfPassengers: [1]
    });
  }

  ngOnInit() {
    // Retrieve search parameters from the queryParams
    this.route.queryParams.subscribe(params => {
      // Set the form values from query parameters
      this.searchForm.patchValue({
        from: params['from'],
        destination: params['destination'],
        departureDateType: params['departureDateType'],
        fromDate: params['departureDateType'] === 'date' ? params['departureDate'] : '',
        fromMonth: params['departureDateType'] === 'month' ? params['departureDate'] : '',
        returnDateType: params['returnDateType'],
        toDate: params['returnDateType'] === 'date' ? params['returnDate'] : '',
        toMonth: params['returnDateType'] === 'month' ? params['returnDate'] : '',
        numberOfPassengers: params['numberOfPassengers']
      });

      const searchParams = {
        from: params['from'],
        destination: params['destination'],
        fromDate: params['departureDate'],
        toDate: params['returnDate'],
        numberOfPassengers: params['numberOfPassengers'],
        seasonName: params['seasonName']
      };

      // Call the service to fetch flight results based on the searchParams
      this.flightService.searchFlight(searchParams).subscribe(
        response => this.searchResult = response,
        err => console.error(err)
      );
    });
  }

  searchFlights(searchParams: any) {
    this.flightService.searchFlight(searchParams).subscribe(
      response => this.searchResult = response,
      err => console.error(err)
    );
  }

  // Same methods as in `search-flights.component.ts`
  onDepartureDateTypeChange(event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    this.departureDateType = selectElement.value;
    if (this.departureDateType === 'date') {
      this.searchForm.get('fromMonth')?.reset();
    } else {
      this.searchForm.get('fromDate')?.reset();
    }
  }

  onReturnDateTypeChange(event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    this.returnDateType = selectElement.value;
    if (this.returnDateType === 'date') {
      this.searchForm.get('toMonth')?.reset();
    } else {
      this.searchForm.get('toDate')?.reset();
    }
  }

  // Handle new search requests from the Flights page
  // Optionally, you can add a search method here to allow resubmission of the form
  search(): void {
    const formValues = this.searchForm.value;

    let departureDate = formValues.departureDateType === 'date' ? formValues.fromDate : formValues.fromMonth;
    let returnDate = formValues.returnDateType === 'date' ? formValues.toDate : formValues.toMonth;

    this.router.navigate(['/flights'], {
      queryParams: {
        from: formValues.from,
        destination: formValues.destination,
        departureDate: departureDate,
        returnDate: returnDate,
        numberOfPassengers: formValues.numberOfPassengers,
      }
    });
  }

  // Calculate discounted price based on user data
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
