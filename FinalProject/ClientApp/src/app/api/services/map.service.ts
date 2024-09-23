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

    private apiKey = '0d0d0ae1d3a74fdf98612af8f99f231c';
    static readonly MapPath = '/api/Map';
    constructor(
      config: ApiConfiguration,
      http: HttpClient
    ) {
      super(config, http);
    }

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

    getCities(departureCity: string | undefined, arrivalCity: string | undefined): Observable<CitiesRm[]> {
      const rb = new RequestBuilder(this.rootUrl, MapService.MapPath + '/' + departureCity + '&' + arrivalCity, 'get');
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

    getOptimalRoute(departureCity: string | undefined, arrivalCity: string | undefined): Observable<CitiesRm[]> {
      const rb = new RequestBuilder(this.rootUrl, MapService.MapPath + '/' + departureCity + '&' + arrivalCity, 'get');
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
