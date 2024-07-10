import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { MapService } from '../api/services/map.service';
import * as L from 'leaflet';
import { CitiesRm } from '../api/models/cities';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css']
})
export class MapComponent implements OnInit, OnDestroy, AfterViewInit {

  cities: CitiesRm[] = [];
  map: L.Map | undefined;
  planes: L.FeatureGroup = L.featureGroup();

  constructor(private mapService: MapService) { }

  ngOnInit(): void {
    document.body.classList.add('map-active');
    this.getCities();
    this.initializeMap();
  }

  ngAfterViewInit(): void {
  }

  ngOnDestroy(): void {
    document.body.classList.remove('map-active');
  }

  getCities(): void {
    this.mapService.getCities().subscribe(
      data => {
        this.cities = data;
        if (this.map) {
          this.addMarkers();
        }
      },
      error => {
        console.error('Error fetching cities:', error);
      }
    );
  }

  initializeMap(): void {
    this.map = L.map('map').setView([0, 0], 2);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 18,
      attribution: '© OpenStreetMap'
    }).addTo(this.map);

    this.planes.addTo(this.map);

    if (this.cities.length > 0) {
      this.addMarkers();
    }
  }

  addMarkers(): void {
    if (!this.map) return;

    this.cities.forEach(city => {
      this.mapService.getCoordinates(city.departure).subscribe(
        departureCoords => {
          L.marker([departureCoords.lat, departureCoords.lng]).addTo(this.map!)
            .bindPopup(`<b>${city.departure}</b><br>Departure City`);

          this.mapService.getCoordinates(city.arrival).subscribe(
            arrivalCoords => {
              L.marker([arrivalCoords.lat, arrivalCoords.lng]).addTo(this.map!)
                .bindPopup(`<b>${city.arrival}</b><br>Arrival City`);

              const polyline = L.polyline([
                [departureCoords.lat, departureCoords.lng],
                [arrivalCoords.lat, arrivalCoords.lng]
              ], {
                color: '#2980b9',
                weight: 3,
                opacity: 0.7,
              }).addTo(this.map!);

              const planeIcon = L.divIcon({
                className: 'plane-icon',
                html: '<i class="fa-solid fa-circle"></i>',
                iconSize: [30, 30],
                iconAnchor: [6, 36]
              });
              const middleLatLng = polyline.getLatLngs()[Math.floor(polyline.getLatLngs().length / 2)];

              const latLngString = middleLatLng.toString();
              const latLngArray = latLngString.replace(/[^\d.,-]/g, '').split(',');

              const Lat = parseFloat(latLngArray[0]);
              const Lng = parseFloat(latLngArray[1]);

              L.marker({ lat: Lat, lng: Lng }, { icon: planeIcon }).addTo(this.planes);
            },
            error => {
              console.error(`Error fetching coordinates for ${city.arrival}:`, error);
            }
          );
        },
        error => {
          console.error(`Error fetching coordinates for ${city.departure}:`, error);
        }
      );
    });
  }
}
