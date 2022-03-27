import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../core/material.module';
import { CommonModule } from "@angular/common";
import { MomentModule } from "ngx-moment";

import { SecondsBasedStatPipe } from "./pipes/seconds-based-stat.pipe";
import { GamesProgressComponent } from "./components/progress-bar/progress.component";
import { OptionsScrollDirective } from "./directives/option-scroll.directive";
import { UserSearchComponent } from "./components/user-search/user-search.component";

/**
 * The Shared Module contains components, services, directives, pipes, etc. that
 * will be shared and reused throughout the entire application.
 */
@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MaterialModule,
    MomentModule
  ],
  exports: [
    ReactiveFormsModule,
    FormsModule,
    MaterialModule,
    SecondsBasedStatPipe,
    OptionsScrollDirective,
    GamesProgressComponent,
    UserSearchComponent
  ],
  declarations: [
    GamesProgressComponent,
    SecondsBasedStatPipe,
    OptionsScrollDirective,
    UserSearchComponent
  ]
})
export class SharedModule { }
