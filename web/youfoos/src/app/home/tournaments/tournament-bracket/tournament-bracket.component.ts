import { Component, Input, OnInit } from "@angular/core";

import { TournamentDto } from "../../../_dtos/responses/tournament.dto";
import { TournamentState } from "../../../_dtos/enums/TournamentState";

@Component({
  selector: 'yf-tournament-bracket',
  templateUrl: './tournament-bracket.component.html',
  styleUrls: ['./tournament-bracket.component.scss']
})
export class TournamentBracketComponent implements OnInit {

  @Input() tournament: TournamentDto

  public tournamentStatus: string;
  public tournamentStatusClass: string;

  ngOnInit() {
    this.setTournamentStatus();
  }

  private setTournamentStatus(): void {
    switch (this.tournament.currentState) {
      case TournamentState.Registration:
        const slotsRemaining = this.tournament.playerCount - this.tournament.playerIds.length;
        this.tournamentStatusClass = "registration";
        this.tournamentStatus = `Open for registration - ${slotsRemaining} out of ${this.tournament.playerCount} slots available`;
        break;
      case TournamentState.Seeding:
        this.tournamentStatusClass = "seeding";
        this.tournamentStatus = "This tournament is currently being seeded"
        break;
      case TournamentState.WaitingForStartTime:
        this.tournamentStatusClass = "waiting-for-start-time";
        this.tournamentStatus = "Waiting for start time";
        break;
      case TournamentState.InProgress:
        this.tournamentStatusClass = "in-progress"
        this.tournamentStatus = "Currently in progress"
        break;
      case TournamentState.Completed:
        this.tournamentStatusClass = "completed"
        this.tournamentStatus = "Completed";
        break;
      case TournamentState.ErrorNotEnoughPlayers:
        this.tournamentStatusClass = "error";
        this.tournamentStatus = "Error - Not Enough Players to Start";
        break;
      case TournamentState.ErrorUnfinishedPastDeadline:
        this.tournamentStatusClass = "error";
        this.tournamentStatus = "Error - Unfinished Games but Deadline has Passed";
        break;
    }
  }

}
