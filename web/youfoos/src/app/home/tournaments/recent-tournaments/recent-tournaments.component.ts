import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { Observable } from "rxjs";

import { TournamentService } from "../../../_services/tournament.service";
import { TournamentDto } from "../../../_dtos/responses/tournament.dto";
import { PaginatedResultDto } from "../../../_dtos/responses/paginatedResult.dto";

@Component({
  selector: 'yf-recent-tournaments',
  templateUrl: './recent-tournaments.component.html',
  styleUrls: ['./recent-tournaments.component.scss']
})
export class RecentTournamentsComponent implements OnInit {

  recentTournaments$: Observable<PaginatedResultDto<TournamentDto>>;

  constructor(
    private _router: Router,
    private tournamentService: TournamentService
  ) {}

  ngOnInit() {
    this.recentTournaments$ = this.tournamentService.getRecentTournaments(0, 10);
  }

  /**
   * Navigates to the detail page for the tournament with the given ID when it is clicked.
   */
  public onClickRecentTournament(id: string): void {
    this._router.navigate(['tournaments', id]);
  }

}
