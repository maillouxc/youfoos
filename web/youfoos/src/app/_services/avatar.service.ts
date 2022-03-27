import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { take } from 'rxjs/operators';

import { UserService } from './user.service';

/**
 * Business logic class responsible for handling management of user avatars.
 */
@Injectable({ providedIn: 'root' })
export class AvatarService {

  private userAvatar = new BehaviorSubject<string>('');

  constructor(private userService: UserService) {}

  /**
   * Returns the avatar of a given user as a base64 encoded string observable.
   *
   * If the user doesn't have a custom avatar set, the API returns a default one,
   * so this method should always return an image.
   */
  getAvatar(): Observable<string> {
    return this.userAvatar.asObservable();
  }

  setAvatarWithId(id: string) {
    this.userService
      .getAvatar(id)
      .pipe(take(1))
      .subscribe((avatar: string) => this.userAvatar.next(avatar));
  }

}
