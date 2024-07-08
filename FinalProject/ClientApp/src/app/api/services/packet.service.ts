import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { RequestBuilder } from '../request-builder';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { map, filter } from 'rxjs/operators';
import { PacketRm } from '../models/packet';

@Injectable({
  providedIn: 'root',
})
export class PacketService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  static readonly PacketPath = '/api/Packet';

  getPackets(): Observable<PacketRm[]> {
    const rb = new RequestBuilder(this.rootUrl, PacketService.PacketPath, 'get');
    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r.body as PacketRm[];
      })
    );
  }

  buyPacket(PassengerEmail: string, Name: string, ValidityMonths: number): Observable<PacketRm[]> {
    const rb = new RequestBuilder(this.rootUrl, PacketService.PacketPath, 'post');
    rb.body({ PassengerEmail, Name, ValidityMonths }, 'application/json'); // Set request body here
    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json'
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r.body as PacketRm[];
      })
    );
  }
}
