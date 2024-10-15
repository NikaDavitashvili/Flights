import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FlightService } from './../api/services/flight.service';
import { FlightRm } from '../api/models';

@Component({
  selector: 'app-flights',
  templateUrl: './flights.component.html',
  styleUrls: ['./flights.component.css']
})
export class FlightsComponent implements OnInit {

  searchResult: FlightRm[] = [];

  constructor(private route: ActivatedRoute, private flightService: FlightService) { }

  ngOnInit() {
    // Retrieve search parameters from the queryParams
    this.route.queryParams.subscribe(params => {
      const searchParams = {
        from: params['from'],
        destination: params['destination'],
        departureDate: params['departureDate'],
        returnDate: params['returnDate'],
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
}
