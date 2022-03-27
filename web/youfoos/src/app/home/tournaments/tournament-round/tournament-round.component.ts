import { Component, Input } from "@angular/core";

import { TournamentRoundDto } from "../../../_dtos/responses/tournamentRound.dto";

/**
 * Component responsible for displaying a single round of a tournament.
 */
@Component({
  selector: 'yf-tournament-round',
  templateUrl: './tournament-round.component.html',
  styleUrls: ['./tournament-round.component.scss']
})
export class TournamentRoundComponent {

  @Input() tournamentRound: TournamentRoundDto

}
