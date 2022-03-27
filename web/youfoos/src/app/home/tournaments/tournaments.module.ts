import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

import { SharedModule } from "../../shared/shared.module";
import { TournamentsRoutingModule } from "./tournaments-routing.module";
import { TournamentsComponent } from "./tournaments.component";
import { TournamentBracketComponent } from "./tournament-bracket/tournament-bracket.component";
import { TournamentMatchupComponent } from "./tournament-matchup/tournament-matchup.component";
import { TournamentRoundComponent } from "./tournament-round/tournament-round.component";
import { RecentTournamentsComponent } from "./recent-tournaments/recent-tournaments.component";
import { CreateTournamentComponent } from "./create-tournament/create-tournament.component";
import { CurrentTournamentComponent } from "./current-tournament/current-tournament.component";
import { TournamentDetailComponent } from "./tournament-detail/tournament-detail.component";
import { JoinTournamentComponent } from "./join-tournament/join-tournament.component";

@NgModule({
  declarations: [
    TournamentsComponent,
    TournamentBracketComponent,
    TournamentMatchupComponent,
    TournamentRoundComponent,
    CurrentTournamentComponent,
    RecentTournamentsComponent,
    CreateTournamentComponent,
    TournamentDetailComponent,
    JoinTournamentComponent
  ],
  imports: [
    CommonModule,
    SharedModule,
    TournamentsRoutingModule
  ]
})
export class TournamentsModule { }
