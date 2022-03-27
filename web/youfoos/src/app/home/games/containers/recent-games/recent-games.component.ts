import { Component, OnInit } from '@angular/core';
import { of, Observable } from 'rxjs';
import { switchMap, distinctUntilChanged } from 'rxjs/operators';

import { GameService } from '../../../../_services/game.service';
import { UserService } from '../../../../_services/user.service';
import { GameWithPlayers } from '../../game-items/gameWithPlayers';
import { GameDto } from 'src/app/_dtos/responses/game.dto';

@Component({
  selector: 'yf-recent-games',
  templateUrl: './recent-games.component.html',
  styleUrls: ['./recent-games.component.scss']
})
export class RecentGamesComponent implements OnInit {

  public recentGames$: Observable<Observable<GameWithPlayers>[]>;

  constructor(
    private gameService: GameService,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    this.recentGames$ = this.getRecentGames();
  }

  getRecentGames(): Observable<Observable<GameWithPlayers>[]> {
    return this.gameService
      .getRecentGames()
      .pipe(
        distinctUntilChanged(
          (x: GameDto[], y: GameDto[]) => x[0].guid === y[0].guid
        )
      )
      .pipe(
        switchMap(games => {
          return of(
            games.map(game =>
              this.userService.getGameWithPlayers(
                game,
                this.getGameWinner(game.blackTeamScore, game.goldTeamScore)
              )
            )
          );
        })
      );
  }

  public getGameWinner(blackTeamScore: number, goldTeamScore: number): string {
    if (blackTeamScore > goldTeamScore) {
      return 'Black';
    } else {
      return 'Gold';
    }
  }

}
