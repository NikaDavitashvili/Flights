/* tslint:disable */
/* eslint-disable */
import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { StrictHttpResponse } from '../strict-http-response';
import { RequestBuilder } from '../request-builder';
import { Observable } from 'rxjs';
import { map, filter } from 'rxjs/operators';

import { BookDto } from '../models/book-dto';
import { FlightRm } from '../models/flight-rm';

@Injectable({
  providedIn: 'root',
})
export class FlightService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }


  static readonly SearchFlightPath = '/Flight';


  searchFlight$Plain$Response(params?: {
    fromDate?: string;
    toDate?: string;
    from?: string;
    destination?: string;
    numberOfPassengers?: number;
  }): Observable<StrictHttpResponse<Array<FlightRm>>> {

    const rb = new RequestBuilder(this.rootUrl, FlightService.SearchFlightPath, 'get');
    if (params) {
      rb.query('fromDate', params.fromDate, {});
      rb.query('toDate', params.toDate, {});
      rb.query('from', params.from, {});
      rb.query('destination', params.destination, {});
      rb.query('numberOfPassengers', params.numberOfPassengers, {});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<FlightRm>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `searchFlight$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  searchFlight$Plain(params?: {
    fromDate?: string;
    toDate?: string;
    from?: string;
    destination?: string;
    numberOfPassengers?: number;
  }): Observable<Array<FlightRm>> {

    return this.searchFlight$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<Array<FlightRm>>) => r.body as Array<FlightRm>)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `searchFlight()` instead.
   *
   * This method doesn't expect any request body.
   */
  searchFlight$Response(params?: {
    fromDate?: string;
    toDate?: string;
    from?: string;
    destination?: string;
    numberOfPassengers?: number;
  }): Observable<StrictHttpResponse<Array<FlightRm>>> {

    const rb = new RequestBuilder(this.rootUrl, FlightService.SearchFlightPath, 'get');
    if (params) {
      rb.query('fromDate', params.fromDate, {});
      rb.query('toDate', params.toDate, {});
      rb.query('from', params.from, {});
      rb.query('destination', params.destination, {});
      rb.query('numberOfPassengers', params.numberOfPassengers, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<Array<FlightRm>>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `searchFlight$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  searchFlight(params?: {
    fromDate?: string;
    toDate?: string;
    from?: string;
    destination?: string;
    numberOfPassengers?: number;
    seasonName?: string;
  }): Observable<Array<FlightRm>> {

    const rb = new RequestBuilder(this.rootUrl, FlightService.SearchFlightPath, 'get');
    if (params) {
      rb.query('fromDate', params.fromDate, {});
      rb.query('toDate', params.toDate, {});
      rb.query('from', params.from, {});
      rb.query('destination', params.destination, {});
      rb.query('numberOfPassengers', params.numberOfPassengers, {});
      rb.query('seasonName', params.seasonName, {}); // Ensure seasonName is included
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => r.body as Array<FlightRm>)
    );
  }

  /**
   * Path part for operation bookFlight
   */
  static readonly BookFlightPath = '/Flight';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `bookFlight()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  bookFlight$Response(params?: {
    body?: FlightRm
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, FlightService.BookFlightPath, 'post');
    if (params) {
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: '*/*'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `bookFlight$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  bookFlight(params?: {
    body?: FlightRm,
  }): Observable<void> {

    return this.bookFlight$Response(params).pipe(
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }

  /**
   * Path part for operation findFlight
   */
  static readonly FindFlightPath = '/Flight/{id}';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `findFlight$Plain()` instead.
   *
   * This method doesn't expect any request body.
   */
  findFlight$Plain$Response(params: {
    id: string;
  }): Observable<StrictHttpResponse<FlightRm>> {

    const rb = new RequestBuilder(this.rootUrl, FlightService.FindFlightPath, 'get');
    if (params) {
      rb.path('id', params.id, {});
    }

    return this.http.request(rb.build({
      responseType: 'text',
      accept: 'text/plain'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<FlightRm>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `findFlight$Plain$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  findFlight$Plain(params: {
    id: string;
  }): Observable<FlightRm> {

    return this.findFlight$Plain$Response(params).pipe(
      map((r: StrictHttpResponse<FlightRm>) => r.body as FlightRm)
    );
  }

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `findFlight()` instead.
   *
   * This method doesn't expect any request body.
   */
  findFlight$Response(params: {
    id: string;
  }): Observable<StrictHttpResponse<FlightRm>> {

    const rb = new RequestBuilder(this.rootUrl, FlightService.FindFlightPath, 'get');
    if (params) {
      rb.path('id', params.id, {});
    }

    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'text/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r as StrictHttpResponse<FlightRm>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `findFlight$Response()` instead.
   *
   * This method doesn't expect any request body.
   */
  findFlight(params: {
    id: string;
  }): Observable<FlightRm> {

    return this.findFlight$Response(params).pipe(
      map((r: StrictHttpResponse<FlightRm>) => r.body as FlightRm)
    );
  }

}
