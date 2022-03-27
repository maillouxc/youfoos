import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, Route, UrlSegment } from '@angular/router';

import { AuthenticationService } from '../../_services/authentication.service';

/**
 * Guard used to disable certain routes based on the user's login state.
 */
@Injectable()
export class AuthGuard implements CanActivate {

  constructor(
    private authService: AuthenticationService,
    private router: Router
  ) {}

  /**
   * Determines whether the user can access a particular route, based on whether they are logged in.
   *
   * This is not a method for any sort of security, but merely to ensure that the user sees the correct
   * UI elements based on their login state. Because the web app runs in the client browser, we must assume
   * that the user will try to fiddle with the code to get places they shouldn't be.
   */
  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    if (this.authService.currentUserValue) {
      // If current user is truthy, the user is currently logged in, so return true
      return true;
    }
    // Else, they are not logged in... send them to the login page if not already there
    if (!state.url.endsWith('/login')) {
      this.router.navigate(['/login'], {
        queryParams: { returnUrl: state.url }
      });
    }
    return false;
  }
  canLoad(route: Route, segments: UrlSegment[]) {
    const fullPath = segments.reduce((path, currentSegment) => {
      return `${path}/${currentSegment.path}`;
    }, '');
    if (localStorage.length > 0) {
      this.authService.loadLoginInfoFromSessionStorage();
    }
    if (this.authService.currentUserValue) {
      return true;
    }
    this.router.navigate(['/login'], { queryParams: { returnUrl: fullPath } });
    return false;
  }

}
