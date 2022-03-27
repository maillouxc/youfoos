import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { Observable } from "rxjs";

import { TournamentDto } from "../../../_dtos/responses/tournament.dto";
import { TournamentService } from "../../../_services/tournament.service";

@Component({
  selector: 'yf-tournament-detail',
  templateUrl: './tournament-detail.component.html',
  styleUrls: ['./tournament-detail.component.scss']
})
export class TournamentDetailComponent implements OnInit {

  public tournament$: Observable<TournamentDto>;

  constructor(
    private tournamentService: TournamentService,
    private route: ActivatedRoute) {}

  ngOnInit() {
    const tournamentId = this.route.snapshot.paramMap.get('id');
    this.tournament$ = this.tournamentService.getTournamentById(tournamentId);
  }

}
