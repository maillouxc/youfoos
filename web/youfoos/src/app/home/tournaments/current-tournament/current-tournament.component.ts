import { Component, OnInit } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";

import { AuthenticationService } from "../../../_services/authentication.service";
import { TournamentService } from "../../../_services/tournament.service";
import { TournamentDto } from "../../../_dtos/responses/tournament.dto";
import { JoinTournamentComponent } from "../join-tournament/join-tournament.component";

@Component({
  selector: 'yf-current-tournament',
  templateUrl: './current-tournament.component.html',
  styleUrls: ['./current-tournament.component.scss']
})
export class CurrentTournamentComponent implements OnInit {

  currentTournament$: Observable<TournamentDto>;

  public isAdmin: boolean = false;
  public isPlayerInCurrentTournament: boolean = false;

  private currentTournament: TournamentDto;

  constructor(
    public dialog: MatDialog,
    private authService: AuthenticationService,
    private tournamentsService: TournamentService
  ) {}

  ngOnInit() {
    this.isAdmin = this.authService.currentUserValue.isAdmin;

    this.currentTournament$ = this.tournamentsService.getCurrentTournament()
      .pipe(tap(tournament => {
        this.currentTournament = tournament;

        let currentUserId = this.authService.currentUserValue.id;
        this.isPlayerInCurrentTournament = tournament.playerIds.includes(currentUserId);
      }));
  }

  public onClickJoinTournament(): void {
    let dialogRef = this.dialog.open(JoinTournamentComponent, {
      data: { tournament: this.currentTournament }
    })
  }

}
