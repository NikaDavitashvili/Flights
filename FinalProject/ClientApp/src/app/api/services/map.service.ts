import { Injectable } from '@angular/core';
import { Observable, from } from 'rxjs';
import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { filter, map, mergeMap, toArray } from 'rxjs/operators';
import { CitiesRm } from '../models/cities';
import { RequestBuilder } from '../request-builder';

@Injectable({
  providedIn: 'root',
})
export class MapService extends BaseService {

  private apiKey = '760ce55a5c7d40d7bc944901209025c7';
  static readonly MapPath = '/api/Map';
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

/*  getCities(): Observable<any[]> {
    return this.http.get<any[]>('/api/map').pipe(
      mergeMap(cities => from(cities)),
      mergeMap(city => this.getCoordinates(city.Departure).pipe(
        map(coords => ({
          name: city.Departure,
          lat: coords.lat,
          lng: coords.lng,
          description: 'Departure City'
        }))
      )),
      toArray()
    );
  }
*/

  getCoordinates(cityName: string): Observable<{ lat: number, lng: number }> {
    const url = `https://api.opencagedata.com/geocode/v1/json?q=${encodeURIComponent(cityName)}&key=${this.apiKey}`;
    return this.http.get<any>(url).pipe(
      map(response => {
        const results = response.results[0];
        return {
          lat: results.geometry.lat,
          lng: results.geometry.lng
        };
      })
    );
  }



  getCities(): Observable<CitiesRm[]> {
    const rb = new RequestBuilder(this.rootUrl, MapService.MapPath, 'get');
    rb.body(null, 'application/json');
    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r.body as CitiesRm[];
      })
    );
  } 
}
