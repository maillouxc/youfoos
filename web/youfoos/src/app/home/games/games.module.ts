import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MomentModule } from "ngx-moment";

import { GamesComponent } from './containers/games.component';
import { RecentGamesComponent } from './containers/recent-games/recent-games.component';
import { GameInProgressComponent } from './components/game-in-progress/game-in-progress.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { GameEventsComponent } from './components/game-events/game-events.component';
import { RecentGameComponent } from './components/recent-game/recent-game.component';

@NgModule({
  declarations: [
    GamesComponent,
    GameInProgressComponent,
    RecentGamesComponent,
    GameEventsComponent,
    RecentGameComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    RouterModule.forChild([{path: '', component: GamesComponent}]),
    MomentModule
  ]
})
export class GamesModule {}
