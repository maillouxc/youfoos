import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../environments/environment';
import { UserStatsDto } from '../_dtos/userStats.dto';
import { PagedUserStats } from '../_models/paged-user-stats';

/**
 * Business logic class responsible for handling stuff related to stats.
 */
@Injectable({ providedIn: 'root' })
export class StatsService {

  constructor(private http: HttpClient) {}

  /**
   * Returns the most recent stats for the given user.
   * @param id The ID of the user to get the stats for.
   */
  getUserStats(id: string): Observable<UserStatsDto> {
    const endpointUrl = `${environment.apiUrl}/stats/users/${id}`;
    return this.http.get<UserStatsDto>(endpointUrl);
  }

  getLeaderboardPage(
    sortBy: string,
    statCategory: string,
    pageSize: number,
    page: number,
    userId: string = null
  ): Observable<PagedUserStats> {
    const endpointUrl = `${environment.apiUrl}/stats/users`;

    statCategory = 'Stats' + statCategory;
    let queryParams = `?sortBy=${sortBy}&statCategory=${statCategory}&pageSize=${pageSize}&PageNumber=${page}`;

    if (userId) {
      queryParams += `&userId=${userId}`;
    }

    return this.http.get<PagedUserStats>(`${endpointUrl}/${queryParams}`);
  }

}
