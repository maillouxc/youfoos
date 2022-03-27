import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { UserService } from '../../../_services/user.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { UserDto } from '../../../_dtos/user.dto';
import { Observable } from 'rxjs';

export interface DialogData {
  id: string;
  userService: UserService;
}

@Component({
  selector: 'yf-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {

  user$: Observable<UserDto>;

  constructor(
    private userService: UserService,
    private authenticator: AuthenticationService,
    private router: Router,
    private route: ActivatedRoute
  ) {
    // Without this, clicking a profile link would change the URL but not reload the page
    this.router.routeReuseStrategy.shouldReuseRoute = function() {
      return false;
    };
  }

  ngOnInit() {
    if (this.route.snapshot.params['id']) {
      const userId = this.route.snapshot.params['id'];
      this.user$ = this.userService.getUser(userId);
    } else {
      this.user$ = this.loadUserInfoFromAuthenticatorService();
    }
  }

  loadUserInfoFromAuthenticatorService() {
        return this.userService.getUser(this.authenticator.currentUserValue.id);
  }

}
