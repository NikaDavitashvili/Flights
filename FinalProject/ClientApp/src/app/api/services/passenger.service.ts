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

import { NewPassengerDto } from '../models/new-passenger-dto';

@Injectable({
  providedIn: 'root',
})
export class PassengerService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  /**
   * Path part for operation registerPassenger
   */
  static readonly RegisterPassengerPath = '/Passenger';

  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `registerPassenger()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */

  //registerPassenger$Response(params?: { body?: NewPassengerDto }): Observable<StrictHttpResponse<void>>
  //{
  //  const rb = new RequestBuilder(this.rootUrl, PassengerService.RegisterPassengerPath, 'post');
  //
  //  if (params) {
  //    rb.body(params.body, 'application/*+json');
  //  }
  //  return this.http.request(rb.build({
  //    responseType: 'json',  // Change to 'json' instead of 'text' to properly handle error messages
  //    accept: 'application/json'  // Ensure you're accepting JSON response
  //    //responseType: 'text',
  //    //accept: '*/*'
  //  })).pipe(
  //    filter((r: any) => r instanceof HttpResponse),
  //    map((r: HttpResponse<any>) => r as StrictHttpResponse<void>)  // Don't modify the body, keep the original response
  //    //map((r: HttpResponse<any>) => {
  //    //  return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
  //    //})
  //  );
  //}

  registerPassenger$Response(params?: {
    body?: NewPassengerDto
  }): Observable<StrictHttpResponse<void>> {

    const rb = new RequestBuilder(this.rootUrl, PassengerService.RegisterPassengerPath, 'post');

    if (params) {
      rb.body(params.body, 'application/*+json');
    }

    return this.http.request(rb.build({
      //responseType: 'json',  // Change to 'json' instead of 'text' to properly handle error messages
      //accept: 'application/json'  // Ensure you're accepting JSON response
      responseType: 'text',
      accept: '*/*'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      //map((r: HttpResponse<any>) => r as StrictHttpResponse<void>)  // Don't modify the body, keep the original response
      map((r: HttpResponse<any>) => {
        return (r as HttpResponse<any>).clone({ body: undefined }) as StrictHttpResponse<void>;
      })
    );
  }

  /**
   * This method provides access to only to the response body.
   * To access the full response (for headers, for example), `registerPassenger$Response()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  //registerPassenger(params?: { body?: NewPassengerDto }): Observable<void> {
  //
  //  return this.registerPassenger$Response(params).pipe(
  //    map(() => { }) 
  //    //map((r: StrictHttpResponse<void>) => r.body as void)
  //  );
  //}

  registerPassenger(params?: {
    body?: NewPassengerDto
  }): Observable<void> {

    return this.registerPassenger$Response(params).pipe(
      //map(() => { }) 
      map((r: StrictHttpResponse<void>) => r.body as void)
    );
  }


  /**
   * This method provides access to the full `HttpResponse`, allowing access to response headers.
   * To access only the response body, use `registerPassenger()` instead.
   *
   * This method sends `application/*+json` and handles request body of type `application/*+json`.
   */
  loginPassenger(email: string, password: string): Observable<User> {
    const url = `${this.rootUrl}/Passenger/${encodeURIComponent(email)}&${encodeURIComponent(password)}`;
    return this.http.post<User>(url, null).pipe(
      map(response => {
        return response;
      })
    );
  }

  logoutPassenger(): void {
    const url = `${this.rootUrl}/Passenger/Logout`;
    console.log("passenger.service.ts");
    console.log(url);
    this.http.post(url, null).subscribe(
      response => {
        console.log("Logout successful", response);
      },
      error => {
        console.error("Logout failed", error);
      }
    );
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
