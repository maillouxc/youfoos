import { Component, Inject } from "@angular/core";
import { MAT_DIALOG_DATA, MatDialogRef } from "@angular/material/dialog";

import { TournamentService } from "../../../_services/tournament.service";
import { TournamentDto } from "../../../_dtos/responses/tournament.dto";
import { AuthenticationService } from "../../../_services/authentication.service";

@Component({
  selector: 'yf-join-tournament',
  templateUrl: './join-tournament.component.html',
  styleUrls: ['./join-tournament.component.scss']
})
export class JoinTournamentComponent {

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: any,
    private dialogRef: MatDialogRef<JoinTournamentComponent>,
    private authService: AuthenticationService,
    private tournamentsService: TournamentService
  ) {}

  onClickJoinTournament() : void {
    this.tournamentsService.registerForTournament(
      this.data.tournament.id, this.authService.currentUserValue.id
    ).subscribe(
      data => {
        this.dialogRef.close();
      },
      error => {
        // TODO error handling
      }
    );
  }

}
