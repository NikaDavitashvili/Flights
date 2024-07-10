import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MapService } from '../api/services/map.service';
import * as L from 'leaflet';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css']
})
export class MapComponent implements OnInit, OnDestroy {

  cities: any[] = [];

  constructor(private http: HttpClient, private mapService: MapService) { }

  ngOnInit(): void {
    document.body.classList.add('map-active');
    this.getCities();
  }

  ngOnDestroy(): void {
    document.body.classList.remove('map-active');
  }

  getCities(): void {
    this.mapService.getCities().subscribe(
      data => {
        this.cities = data;
        this.initializeMap();
      },
      error => {
        console.error('Error fetching cities:', error);
      }
    );
  }

  initializeMap(): void {
    const map = L.map('map').setView([0, 0], 2);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png').addTo(map);
    
    this.cities.forEach(city => {
      L.marker([city.lat, city.lng]).addTo(map)
        .bindPopup(`<b>${city.name}</b><br>${city.description}`);
    });
  }
}
