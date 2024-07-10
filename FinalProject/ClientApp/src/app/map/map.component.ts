import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css']
})
export class MapComponent implements OnInit {

  cities: any[] = []; // Assuming city data structure

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.fetchCities();
  }

  fetchCities(): void {
    // Replace with your actual API endpoint to fetch city data
    this.http.get<any[]>('http://your-api-url/GetCities').subscribe(
      data => {
        this.cities = data;
        //this.initializeMap();
      },
      error => {
        console.error('Error fetching cities:', error);
      }
    );
  }

  //initializeMap(): void {
  //  // Initialize your map here, e.g., using Leaflet or Angular Google Maps
  //  // Example: Leaflet integration
  //  const map = L.map('map').setView([0, 0], 2); // Set initial view to center of the world
  //  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png').addTo(map);

  //  // Add markers for each city
  //  this.cities.forEach(city => {
  //    L.marker([city.lat, city.lng]).addTo(map)
  //      .bindPopup(`<b>${city.name}</b><br>${city.description}`); // Example popup with city name and description
  //  });
  //}
}
