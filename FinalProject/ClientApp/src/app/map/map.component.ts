import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { MapService } from '../api/services/map.service';
import * as L from 'leaflet';
import { CitiesRm } from '../api/models/cities';
const DefaultIcon = L.icon({
  iconUrl: 'http://localhost:44492/marker-icon.png',
  iconSize: [25, 41],
  iconAnchor: [12, 41],
  popupAnchor: [1, -34],
  tooltipAnchor: [16, -28],
  shadowSize: [41, 41]
});
@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css']
})
export class MapComponent implements OnInit, OnDestroy, AfterViewInit {

  cities: CitiesRm[] = [];
  map: L.Map | undefined;
  planes: L.FeatureGroup = L.featureGroup();
  markers: L.Marker[] = [];
  polylines: L.Polyline[] = [];
  departureCity: string | undefined;
  arrivalCity: string | undefined;
  totalPrice: number = 0;

  constructor(private mapService: MapService) { }

  ngOnInit(): void {
    document.body.classList.add('map-active');
    this.getCities(this.departureCity, this.arrivalCity);
    this.initializeMap();
  }

  ngAfterViewInit(): void { }

  ngOnDestroy(): void {
    document.body.classList.remove('map-active');
  }

  getCities(departureCity: string | undefined, arrivalCity: string | undefined): void {
    this.mapService.getCities(departureCity, arrivalCity).subscribe(
      data => {
        this.cities = data;
        this.totalPrice = 0;

        if (this.map) {
          this.addMarkers();
        }
      },
      error => {
        console.error('Error fetching cities:', error);
      }
    );
  }

  getOptimalRoute(departureCity: string, arrivalCity: string): void {
    this.mapService.getOptimalRoute(departureCity, arrivalCity).subscribe(
      data => {
        this.cities = data;
        this.calculateTotalPrice();

        if (this.map) {
          this.addMarkers();
        }
      },
      error => {
        console.error('Error fetching optimal route:', error);
      }
    );
  }

  onCleanSearchClick(): void {
    this.departureCity = undefined;
    this.arrivalCity = undefined;
    this.getCities(this.departureCity, this.arrivalCity);
  }

  onSearchClick(): void {
    this.departureCity = (document.getElementById('departureCity') as HTMLInputElement).value;
    this.arrivalCity = (document.getElementById('arrivalCity') as HTMLInputElement).value;
    this.getOptimalRoute(this.departureCity, this.arrivalCity);
  }

  calculateTotalPrice(): void {
    this.totalPrice = this.cities.reduce((acc, city) => acc + city.price, 0);
  }

  initializeMap(): void {
    this.map = L.map('map').setView([45, 20], 4.5);
    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      maxZoom: 18,
      attribution: '© SkyConnect Map'
    }).addTo(this.map);

    L.Marker.prototype.options.icon = DefaultIcon;
    this.planes.addTo(this.map);

    if (this.cities.length > 0) {
      this.addMarkers();
    }
  }

  addMarkers(): void {
    if (!this.map) return;

    this.markers.forEach(marker => marker.remove());
    this.polylines.forEach(polyline => polyline.remove());
    this.planes.clearLayers();

    this.markers = [];
    this.polylines = [];
    this.cities.forEach(city => {
      this.mapService.getCoordinates(city.departure).subscribe(
        departureCoords => {
          const departureMarker = L.marker([departureCoords.lat, departureCoords.lng]).addTo(this.map!)
            .bindPopup(`<b>${city.departure}</b><br>Departure City`);
          this.markers.push(departureMarker);
          this.mapService.getCoordinates(city.arrival).subscribe(
            arrivalCoords => {
              const arrivalMarker = L.marker([arrivalCoords.lat, arrivalCoords.lng]).addTo(this.map!)
                .bindPopup(`<b>${city.arrival}</b><br>Arrival City`);
              this.markers.push(arrivalMarker);

              const polyline = L.polyline([
                [departureCoords.lat, departureCoords.lng],
                [arrivalCoords.lat, arrivalCoords.lng]
              ], {
                color: '#2980b9',
                weight: 3,
                opacity: 0.7,
              }).addTo(this.map!);
              this.polylines.push(polyline);

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
