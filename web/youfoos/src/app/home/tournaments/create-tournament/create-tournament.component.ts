import { Component } from "@angular/core";
import { BehaviorSubject, Subject } from "rxjs";
import * as moment from 'moment';

import { TournamentService } from "../../../_services/tournament.service";
import { CreateTournamentRequest } from "../../../_dtos/requests/createTournament.request";
import { GameType } from "../../../_dtos/enums/GameType";

@Component({
  selector: 'yf-create-tournament',
  templateUrl: './create-tournament.component.html',
  styleUrls: ['./create-tournament.component.scss']
})
export class CreateTournamentComponent {

  public currentlySelectedGameType: GameType = GameType.Singles;

  playerCountOptions1V1: string[] = ['8 (3 rounds)', '16 (4 rounds)', '32 (5 rounds)'];
  playerCountOptions2V2: string[] = ['16 (3 rounds)', '32 (4 rounds)', '64 (5 rounds)'];
  currentPlayerCountOptions$: Subject<string[]> = new BehaviorSubject(this.playerCountOptions1V1);

  public selectedPlayerCount: string;

  public minStartDate: moment.Moment;
  public maxStartDate: moment.Moment;
  public minEndDate: moment.Moment;
  public maxEndDate: moment.Moment;

  public startDate: moment.Moment;
  public endDate: moment.Moment;

  public tournamentName: string = "";
  public tournamentDescription: string = "";

  constructor(private tournamentService: TournamentService) {
    this.minStartDate = moment();
    this.maxStartDate = this.minStartDate.clone().add(30, 'days');
  }

  public onChangeGameType(gameType: string): void {
    if (gameType == '1V1') {
      this.currentPlayerCountOptions$.next(this.playerCountOptions1V1);
      this.currentlySelectedGameType = GameType.Singles
    } else if (gameType == '2V2') {
      this.currentPlayerCountOptions$.next(this.playerCountOptions2V2);
      this.currentlySelectedGameType = GameType.Doubles
    }
  }

  public onClickSubmit(): void {
    // TODO validate no empty fields

    let createTournamentRequest = new CreateTournamentRequest(
      this.tournamentName,
      this.tournamentDescription,
      this.startDate.toISOString(),
      this.endDate.toISOString(),
      this.getNumberFromPlayerCountOption(this.selectedPlayerCount),
      this.currentlySelectedGameType
    );

    this.tournamentService.createTournament(createTournamentRequest).subscribe();
  }

  private getNumberFromPlayerCountOption(option: string): number {
    return Number(option.split(" ")[0]);
  }

}
