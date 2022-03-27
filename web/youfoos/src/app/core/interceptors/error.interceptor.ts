import { Injectable } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { AuthenticationService } from '../../_services/authentication.service';

/**
 * Centralized interceptor for handling HTTP request errors.
 *
 * This allows us to perform logging, redirects, etc.
 */
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private activatedRoute: ActivatedRoute,
              private authenticationService: AuthenticationService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(catchError(err => {
      // When a 401 Unauthorized is returned, log the user out and reload, which returns them to the login page
      if (err.status === 401) {
        var currentUrl = this.activatedRoute.snapshot['_routerState'].url;

        // Only redirect if not currently on this login page
        if (!(currentUrl.endsWith('/login') || currentUrl.endsWith('/profile'))) {
          this.authenticationService.logout();
          location.reload();
        }
      }

      // Once we're done handling, rethrow the error for others to potentially handle
      return throwError(err);
    }));
  }

}
