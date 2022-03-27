import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { flatMap, startWith } from 'rxjs/operators';
import { interval, Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { GameDto } from '../_dtos/responses/game.dto';

/**
 * Business logic class responsible for handling management and retrieval of games.
 */
@Injectable({ providedIn: 'root' })
export class GameService {

  constructor(private http: HttpClient) {}

  /**
   * Returns the game that is currently in progress, if one exists.
   *
   * This method is setup specially to return a new observable value from the API every 2 seconds.
   */
  getCurrentGame(): Observable<GameDto> {
    const endpointUrl = `${environment.apiUrl}/games/current`;
    return interval(2000).pipe(
      flatMap(() => this.http.get<GameDto>(endpointUrl))
    );
  }

  /**
   * Returns the 10 most recent games that have been played.
   *
   * This method is setup specially to return a new observable value from the API every 5 seconds.
   */
  getRecentGames(): Observable<GameDto[]> {
    const endpointUrl = `${environment.apiUrl}/games/recent`;
    return interval(5000).pipe(
      startWith(0),
      flatMap( () => this.http.get<GameDto[]>(endpointUrl))
    );
  }

  /**
   * Returns the 10 most recent games the given user has played.
   * @param id The user ID of the user to get the recent games for.
   */
  getUserRecentGames(id: string): Observable<GameDto[]> {
    const endpointUrl = `${environment.apiUrl}/games/recent/user/${id}`;
    return this.http.get<GameDto[]>(endpointUrl);
  }

}
