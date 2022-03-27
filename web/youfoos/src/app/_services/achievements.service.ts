import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";

import { environment } from "../../environments/environment";
import { AchievementStatusDto } from "../_dtos/responses/achievementStatus.dto";

/**
 * Business logic class responsible for handling management of achievements.
 */
@Injectable({ providedIn: 'root' })
export class AchievementsService {

  constructor(private http: HttpClient) {}

  /**
   * Returns the status of all achievements for a given user.
   * @param userId The user ID of the person to get the statuses for.
   */
  public getUserAchievements(userId: string): Observable<AchievementStatusDto[]> {
    const endpointUrl = `${environment.apiUrl}/achievements/users/${userId}`;
    return this.http.get<AchievementStatusDto[]>(endpointUrl);
  }

}
