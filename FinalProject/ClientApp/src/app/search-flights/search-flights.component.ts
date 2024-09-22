import { Component, OnInit } from '@angular/core';
import { FlightService } from './../api/services/flight.service';
import { FlightRm } from '../api/models';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-search-flights',
  templateUrl: './search-flights.component.html',
  styleUrls: ['./search-flights.component.css']
})
export class SearchFlightsComponent implements OnInit {

  searchResult: FlightRm[] = [];
  currentUser: User | null = null;
  searchForm: FormGroup;
  seasonName: string | null = null;

  constructor(private flightService: FlightService, private fb: FormBuilder) {
    this.searchForm = this.fb.group({
      from: [''],
      destination: [''],
      fromDate: [''],
      toDate: [''],
      numberOfPassengers: [1]
    });
  }

  ngOnInit(): void {
    this.search();
    const userJson = sessionStorage.getItem('CurrentUser');
    if (userJson) {
      this.currentUser = JSON.parse(userJson);
    }
  }

  search(seasonName?: string): void {
    if (seasonName) {
      this.seasonName = seasonName;
    }
    const formValue = { ...this.searchForm.value, seasonName: this.seasonName };
    this.flightService.searchFlight(formValue).subscribe(
      response => this.searchResult = response,
      err => this.handleError(err)
    );
    this.seasonName = null;
  }

  private handleError(err: any): void {
    console.log("Response Error. Status: ", err.status);
    console.log("Response Error. Status Text: ", err.statusText);
    console.log(err);
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
