import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { PaginatedResultDto } from "../_dtos/responses/paginatedResult.dto";
import { TournamentDto } from "../_dtos/responses/tournament.dto";
import { CreateTournamentRequest } from "../_dtos/requests/createTournament.request";

/**
 * Business logic class responsible for handling the management of tournaments.
 */
@Injectable({ providedIn: 'root' })
export class TournamentService {

  constructor(private http: HttpClient) {}

  /**
   * Gets the current tournament in progress from the API.
   */
  public getCurrentTournament(): Observable<TournamentDto> {
    const endpointUrl = `${environment.apiUrl}/tournaments/current`;
    return this.http.get<TournamentDto>(endpointUrl);
  }

  /**
   * Gets the tournament with the given ID from the API.
   */
  public getTournamentById(id: string): Observable<TournamentDto> {
    const endpointUrl = `${environment.apiUrl}/tournaments/${id}`;
    return this.http.get<TournamentDto>(endpointUrl);
  }

  /**
   * Returns the most recently played tournaments from the API, paginated.
   */
  public getRecentTournaments(page: Number, pageSize: Number): Observable<PaginatedResultDto<TournamentDto>> {
    const endpointUrl = `${environment.apiUrl}/tournaments`;
    const queryParams = `page=${page}&pageSize=${pageSize}`;
    const requestUrl = `${endpointUrl}?${queryParams}`;

    return this.http.get<PaginatedResultDto<TournamentDto>>(requestUrl);
  }

  /**
   * Sends a request to the API to create a new tournament.
   */
  public createTournament(request: CreateTournamentRequest) {
    const endpointUrl = `${environment.apiUrl}/tournaments`;

    return this.http.post<CreateTournamentRequest>(endpointUrl, request);

    // TODO error handling
  }

  /**
   * Registers the user to participate in the current tournament.
   */
  public registerForTournament(tournamentId: string, userId: string) {
    const endpointUrl = `${environment.apiUrl}/tournaments/${tournamentId}/users/${userId}`;

    return this.http.post(endpointUrl, null);

    // TODO error handling
  }

}
