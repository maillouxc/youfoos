import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import * as jwt_decode from 'jwt-decode';

import { environment } from '../../environments/environment';
import { User } from '../_models/user';
import { LoginCredentialsRequest } from '../_dtos/requests/loginCredentials.request';
import { AvatarService } from './avatar.service';

/**
 * Business logic class containing code relating to management of user authentication.
 */
@Injectable({ providedIn: 'root' })
export class AuthenticationService {

  private currentUserSubject: BehaviorSubject<User>;

  // Allows access to current user info and sends reactive updates to listeners on their state of login
  public currentUser: Observable<User>;

  constructor(private http: HttpClient, private avatarService: AvatarService) {
    this.currentUserSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem('CURRENT_USER')));
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): User {
    return this.currentUserSubject.value;
  }
  // TODO: Clean up login function and make it easier to read.
  public login(email: string, password: string): Observable<string> {
    const endpointUrl = `${environment.apiUrl}/users/login`;
    const loginCredentials = new LoginCredentialsRequest(email, password);

    return this.http.post<string>(endpointUrl, loginCredentials)
      .pipe(map(response => {
        // Login successful if there is a response containing a JWT
        if (response) {
          // When we get our login request response back from the API, extract the needed info from it
          const decodedToken = JSON.parse(JSON.stringify(jwt_decode(response)));
          // We can't update the values of a subject easily, so we instead create a shallow copy
          const updatedUser = { ...this.currentUserValue };
          // noinspection TypeScriptUnresolvedVariable
          updatedUser.email = decodedToken.Email;
          // noinspection TypeScriptUnresolvedVariable
          updatedUser.firstAndLastName = decodedToken.Name;
          updatedUser.id = decodedToken.Id;
          updatedUser.token = response; // We want the non-decoded token here

          this.currentUserSubject.next(updatedUser);
          this.avatarService.setAvatarWithId(updatedUser.id);

          this.storeLoginInfoInLocalStorage(response, updatedUser);

          return response;
        }
      }));
  }

  private storeLoginInfoInLocalStorage(jwt: string, currentUser: User): void {
    localStorage.setItem('CURRENT_USER', JSON.stringify(currentUser));
  }

  public loadLoginInfoFromSessionStorage(): void {
    let user: User = JSON.parse(localStorage.getItem('CURRENT_USER'));
    this.currentUserSubject.next(user);
    this.avatarService.setAvatarWithId(user.id);
  }

  public logout(): void {
    localStorage.removeItem('CURRENT_USER');
    this.currentUserSubject.next(null);
  }

}
