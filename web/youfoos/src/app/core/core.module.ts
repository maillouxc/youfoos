import { NgModule, Optional, SkipSelf } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";

import { throwIfAlreadyLoaded } from './single-import-guard';
import { LoggerService } from "./logger.service";

/**
 * The core module contains services and other things that should only be used ONCE
 * throughout the entire application. For instance, this can include singleton services,
 * a logging service, the nav-bar, etc.
 */
@NgModule({
  imports: [
    CommonModule,
    BrowserAnimationsModule
  ],
  providers: [LoggerService]
})
export class CoreModule {

  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    throwIfAlreadyLoaded(parentModule, 'CoreModule')
  }

}
