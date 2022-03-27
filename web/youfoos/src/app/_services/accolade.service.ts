import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { AccoladeDto } from '../_dtos/responses/accolade.dto';

/**
 * Business logic class responsible for handling management of accolades.
 */
@Injectable({ providedIn: 'root' })
export class AccoladeService {

  constructor(private http: HttpClient) { }

  /**
   * Returns all accolades for the system.
   */
  public getAccolades(): Observable<AccoladeDto[]> {
    const endpointUrl = `${environment.apiUrl}/accolades`;
    return this.http.get<AccoladeDto[]>(endpointUrl);
  }

}
