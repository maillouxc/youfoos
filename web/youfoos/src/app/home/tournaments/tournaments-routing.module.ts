import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { TournamentsComponent } from "./tournaments.component";
import { CurrentTournamentComponent}  from "./current-tournament/current-tournament.component";
import { RecentTournamentsComponent } from "./recent-tournaments/recent-tournaments.component";
import { CreateTournamentComponent } from "./create-tournament/create-tournament.component";
import { TournamentDetailComponent } from "./tournament-detail/tournament-detail.component";

const routes: Routes = [{
  path: '',
  component: TournamentsComponent,
  children: [
    { path: 'current', component: CurrentTournamentComponent },
    { path: 'recent', component: RecentTournamentsComponent },
    { path: 'create', component: CreateTournamentComponent },
    { path: ':id', component: TournamentDetailComponent }
  ]
}];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TournamentsRoutingModule {}
