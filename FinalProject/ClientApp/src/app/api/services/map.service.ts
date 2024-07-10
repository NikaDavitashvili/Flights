import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseService } from '../base-service';
import { ApiConfiguration } from '../api-configuration';
import { RequestBuilder } from '../request-builder';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { map, filter } from 'rxjs/operators';
import { CitiesRm } from '../models/cities';

@Injectable({
  providedIn: 'root',
})
export class MapService extends BaseService {
  constructor(
    config: ApiConfiguration,
    http: HttpClient
  ) {
    super(config, http);
  }

  static readonly PacketPath = '/api/Map';

  getCities(UserEmail: string): Observable<CitiesRm> {
    const rb = new RequestBuilder(this.rootUrl, MapService.PacketPath, 'get');
    rb.body({ UserEmail }, 'application/json');
    return this.http.request(rb.build({
      responseType: 'json',
      accept: 'application/json',
    })).pipe(
      filter((r: any) => r instanceof HttpResponse),
      map((r: HttpResponse<any>) => {
        return r.body as CitiesRm;
      })
    );
  }
}
