import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { LeaderboardComponent } from './leaderboard.component';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  declarations: [LeaderboardComponent],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule.forChild([{ path: '', component: LeaderboardComponent }])
  ]
})
export class LeaderboardModule {}
