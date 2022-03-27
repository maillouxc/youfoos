import { Injectable,  } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { catchError, map, mergeMap, toArray, concatMap, switchMap } from 'rxjs/operators';
import { Observable, throwError, from } from 'rxjs';

import { environment } from '../../environments/environment';
import { UserDto } from '../_dtos/user.dto';
import { CreateAccountRequest } from '../_dtos/requests/createAccount.request';
import { UserAvatarDto } from '../_dtos/useravatar.dto';
import { ResetPasswordRequest } from '../_dtos/requests/resetPassword.request';
import { ChangePasswordRequest } from '../_dtos/requests/changePassword.request';
import { ChangeRfidRequest } from '../_dtos/requests/changeRfid.request';
import { AllUser } from '../_models/all-user.model';
import { Player } from '../_models/player.model';
import { GameWithPlayers } from '../home/games/game-items/gameWithPlayers';
import { GameDto } from '../_dtos/responses/game.dto';

@Injectable({ providedIn: 'root' })
export class UserService {

  constructor(private http: HttpClient) {}

  /**
   * Gets the list of users from the API.
   */
  getAllUsers(page, pageSize, name): Observable<Player[]> {
    const endpointUrl = `${environment.apiUrl}/users`;

    return this.http
      .get<AllUser>(
        `${endpointUrl}/?page=${page}&pageSize=${pageSize}&nameContains=${name}`
      )
      .pipe(
        switchMap(data => {
          return from(data.results).pipe(
            concatMap(user => {
              return this.getPlayerInfoAndAvatar(user.id);
            })
          );
        }),toArray()
      )
  }

  /**
   * Gets the user with the given ID.
   */
  getUser(id: string): Observable<UserDto> {
    const endpointUrl = `${environment.apiUrl}/users/${id}`;
    return this.http.get<UserDto>(endpointUrl);
  }

  /**
   * This get for onClickForgotPassword endpoint using email as a string.
   * This function will cause the api to send an email with a code to be used on this page
   */
  // Can't seem to type. probably really easy to type. Fix error handling.
  getForgotPassword(email: string) {
    const endpointUrl = `${environment.apiUrl}/users/${email}/reset-password`;
    return this.http.get(endpointUrl).pipe(catchError(this.handleError));
  }

  /**
   * Fetches a user and their avatar at the same time and returns a single observable
   */
  getPlayerInfoAndAvatar(id: string): Observable<Player> {
    return this.getUser(id).pipe(
      mergeMap(user =>
        this.getAvatar(id).pipe(
          map(avatar => {
            return <Player>{
              name: user.firstAndLastName,
              id: user.id,
              avatar: avatar,
              joinedTimestamp: user.joinedTimestamp,
              isAdmin: user.isAdmin
            };
          })
        )
      )
    );
  }

  getPlayers(ids: string[]): Observable<Player[]> {
    return from(ids).pipe(
      concatMap(id => this.getPlayerInfoAndAvatar(id)),
      toArray()
    );
  }

  getGameWithPlayers(game: GameDto, winner: string) : Observable<GameWithPlayers> {
    return this.getPlayers([ game.goldDefenseUserId, game.goldOffenseUserId, game.blackOffenseUserId, game.blackDefenseUserId]).pipe(
      map(players => {
        return new GameWithPlayers(
          game,
          players[0],
          players[1],
          players[2],
          players[3],
          winner
        );
      })
    );
  }

  /**
   * This Post the Reset password to the api using a resetPasswordDto
   * This is used on the Forgot Password Page
   */
  postResetPassword(email: string, newPassword: ResetPasswordRequest): Observable<ResetPasswordRequest> {
    const endpointUrl = `${environment.apiUrl}/users/${email}/reset-password`;
    return this.http
      .post<ResetPasswordRequest>(endpointUrl, newPassword)
      .pipe(catchError(this.handleError));
  }

  /**
   * This Post a changePasswordDto to the api.
   * This is used on the Change Password dialog on Profile page
   */
  postChangePassword(id: string, newPassword: ChangePasswordRequest): Observable<ChangePasswordRequest> {
    const endpointUrl = `${environment.apiUrl}/users/${id}/change-password`;
    return this.http
      .post<ChangePasswordRequest>(endpointUrl, newPassword)
      .pipe(catchError(this.handleError));
  }

  /**
   * Returns the avatar for the user with the given ID.
   */
  getAvatar(id: string): Observable<string> {
    const endpointUrl = `${environment.apiUrl}/users/${id}/avatar`;
    return this.http
      .get<UserAvatarDto>(endpointUrl).pipe( map((avatarData: UserAvatarDto) =>
      'data:' + avatarData.mimeType + ';base64,' + avatarData.base64Image))
      .pipe(catchError(this.handleError));
  }

  /**
   * Updates the avatar for the user with the given ID.
   */
  putNewAvatar(id: string, newAvatar: UserAvatarDto): Observable<UserAvatarDto> {
    const endpointUrl = `${environment.apiUrl}/users/${id}/avatar`;
    return this.http
      .put<UserAvatarDto>(endpointUrl, newAvatar)
      .pipe(catchError(this.handleError));
  }

  /**
   * Registers a new user account - if one doesn't already exist with the given email.
   */
  registerNewUser(newAccountInfo: CreateAccountRequest): Observable<UserDto> {
    const endpointUrl = `${environment.apiUrl}/users`;
    return this.http
      .post<UserDto>(endpointUrl, newAccountInfo)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      // A client side or network error occurred - handle it accordingly
    } else {
      // The backend returned an unsuccessful response code.
      // The response body should be in the form of an ApiErrorDto containing information about the error.
    }
    // Return an Observable with a user-facing error message
    return throwError(error.error.message);
  }

  patchChangeRfid(id: string, newRfid: ChangeRfidRequest): Observable<ChangeRfidRequest> {
    const endpointUrl = `${environment.apiUrl}/users/${id}/rfid`;
    return this.http
      .patch<ChangeRfidRequest>(endpointUrl, newRfid)
      .pipe(catchError(this.handleError));
  }
}
