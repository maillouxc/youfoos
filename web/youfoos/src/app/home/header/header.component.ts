import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BreakpointObserver } from '@angular/cdk/layout';
import { Observable } from 'rxjs';

import { AuthenticationService } from '../../_services/authentication.service';
import { AvatarService } from 'src/app/_services/avatar.service';

@Component({
  selector: 'yf-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {

  userAvatar$: Observable<string>;
  userName: string;

  constructor(
    private authService: AuthenticationService,
    private router: Router,
    private avatarService: AvatarService,
    private breakpointObserver: BreakpointObserver
  ) {}

  ngOnInit() {
    this.userAvatar$ = this.avatarService.getAvatar();
    this.userName = this.authService.currentUserValue.firstAndLastName;
  }

  get isMobile() {
    return this.breakpointObserver.isMatched('(max-width: 1155px)');
  }

  onLogout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

}
