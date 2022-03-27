import { Component, Input } from "@angular/core";
import { Router } from "@angular/router";

import { GameType } from "../../../_dtos/enums/GameType";
import { TournamentMatchupDto } from "../../../_dtos/responses/tournamentMatchup.dto";

@Component({
  selector: 'yf-tournament-matchup',
  templateUrl: './tournament-matchup.component.html',
  styleUrls: ['./tournament-matchup.component.scss']
})
export class TournamentMatchupComponent {

  @Input() matchup: TournamentMatchupDto;

  // Only needed so we can access the values of the enum in the template
  allGameTypes = GameType;

  constructor(private _router: Router) {}

  /**
   * Navigates to profile page for the user with the given ID when their name is clicked.
   */
  public goToProfile(id: string) : void {
    this._router.navigate(['user', id]);
  }

}
