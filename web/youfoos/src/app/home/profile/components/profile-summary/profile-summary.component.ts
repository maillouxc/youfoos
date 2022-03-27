import { Component, Input, AfterViewInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { first, map, mergeMap } from 'rxjs/operators';
import { Observable } from 'rxjs';

import { UserService } from '../../../../_services/user.service';
import { AuthenticationService } from '../../../../_services/authentication.service';
import { DialogChangeRfidComponent } from '../change-rfid/change-rfid.component';
import { DialogChangePasswordComponent } from '../change-password/dialog-change-password.component';
import { DialogAvatarUploadComponent } from '../avatar-upload/dialog-avatar-upload.component';
import { Player } from 'src/app/_models/player.model';
import { UserDto } from 'src/app/_dtos/user.dto';
import { AvatarService } from 'src/app/_services/avatar.service';

export interface ProfileModel extends UserDto, Player {}

@Component({
  selector: 'yf-profile-summary',
  templateUrl: './profile-summary.component.html',
  styleUrls: ['./profile-summary.component.scss']
})
export class ProfileSummaryComponent implements AfterViewInit {

  @Input() userId: string;

  isCurrentUser: boolean;
  user$: Observable<Player>;

  constructor(
    private userService: UserService,
    private authenticator: AuthenticationService,
    public dialog: MatDialog,
    private avatarService: AvatarService,
  ) {}

  ngAfterViewInit(): void {
    this.user$ = this.getUserInfo(this.userId);
    this.isCurrentUser = this.checkIfUserIdMatchesCurrentUser();
  }

  private getUserInfo(userId: string): Observable<Player> {
    return this.userService
      .getPlayerInfoAndAvatar(userId)
      .pipe(
        mergeMap(player =>
          this.userService
            .getUser(player.id)
            .pipe(map(user => <ProfileModel>{ ...user, ...player }))
        )
      );
  }

  private checkIfUserIdMatchesCurrentUser(): boolean {
    return this.authenticator.currentUserValue.id === this.userId;
  }

  openChangeAvatarDialog(): void {
    const dialogRef = this.dialog.open(DialogAvatarUploadComponent, {
      width: '500px',
      height: '600px',
      data: { id: this.userId, userService: this.userService }
    });

    dialogRef
      .afterClosed()
      .pipe(first())
      .subscribe(() => {
        this.user$ = this.getUserInfo(this.userId);
        this.avatarService.setAvatarWithId(this.authenticator.currentUserValue.id);
      });
  }

  openChangeRfidDialog(): void {
    this.dialog.open(DialogChangeRfidComponent, {
      width: '360px',
      height: '230px',
      data: { id: this.userId, userService: this.userService }
    });
  }

  openChangePasswordDialog(): void {
    this.dialog.open(DialogChangePasswordComponent, {
      width: '360px',
      height: '355px',
      data: { id: this.userId, userService: this.userService }
    });
  }

}
