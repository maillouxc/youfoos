import { Injectable } from '@angular/core';

/**
 * This class centralizes logging for our application.
 *
 * By using this class, we are able to later add more sophisticated logging without
 * changing the existing logging statements in our app. For instance, we can decide
 * to later log to a remote server for debugging.
 */
@Injectable()
export class LoggerService {

  log(msg: string) {
    console.log(msg);
  }

  error(msg: string) {
    console.error(msg);
  }

}
