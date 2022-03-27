import { Component } from "@angular/core";
import { Router } from "@angular/router";
import { of, Observable, BehaviorSubject } from "rxjs";
import { map, distinctUntilChanged, switchMap } from "rxjs/operators";

import { GameService } from "../../../../_services/game.service";
import { UserService } from "../../../../_services/user.service";
import { GameWithPlayers } from "../../game-items/gameWithPlayers";
import { Player } from "src/app/_models/player.model";

@Component({
  selector: "yf-game-in-progress",
  templateUrl: "./game-in-progress.component.html",
  styleUrls: ["./game-in-progress.component.scss"]
})
export class GameInProgressComponent {

  public gameInProgressObservable = this.getCurrentGame();
  public players = new BehaviorSubject<Player[]>(null);

  constructor(
    private gameService: GameService,
    private userService: UserService,
    private router: Router
  ) {}

  getCurrentGame(): Observable<GameWithPlayers> {
    return this.gameService
      .getCurrentGame()
      .pipe(distinctUntilChanged())
      .pipe(
        switchMap(game => {
          if (game) {
            if (this.players.value) {
              return of(
                new GameWithPlayers(
                  game,
                  this.players.value[0],
                  this.players.value[1],
                  this.players.value[2],
                  this.players.value[3],
                  "none"
                )
              );
            }
            return this.userService.getGameWithPlayers(game, "none").pipe(
              map(game => {
                this.players.next([
                  game.goldDefensePlayer,
                  game.goldOffensePlayer,
                  game.blackOffensePlayer,
                  game.blackDefensePlayer
                ]);
                return game;
              })
            );
          }
          this.players.next(null);
          return of(null);
        })
      );
  }

  public goToUserProfile(id: string) {
    this.router.navigate(["user", id]);
  }

}
