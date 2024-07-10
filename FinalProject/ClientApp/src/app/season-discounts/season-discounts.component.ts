import { Component, OnInit } from '@angular/core';
import { FlightService } from './../api/services/flight.service';
import { FlightRm } from '../api/models';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-season-discounts',
  templateUrl: './season-discounts.component.html',
  styleUrls: ['./season-discounts.component.css']
})
export class SeasonDiscountsComponent implements OnInit {

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
    const userJson = sessionStorage.getItem('CurrentUser');
    if (userJson) {
      this.currentUser = JSON.parse(userJson);
    }
  }

  search(seasonName?: string): void {
    if (seasonName) {
      this.seasonName = seasonName;
      sessionStorage.setItem('SeasonName', seasonName);
    }
    const formValue = { ...this.searchForm.value, seasonName: this.seasonName };
    this.flightService.searchFlight(formValue).subscribe(
      response => this.searchResult = response,
      err => this.handleError(err)
    );
  }

  private handleError(err: any): void {
    console.log("Response Error. Status: ", err.status);
    console.log("Response Error. Status Text: ", err.statusText);
    console.log(err);
  }

  getDiscountedPrice(price: any): number | null {
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
