import { AfterViewInit, Component, Input } from "@angular/core";
import { Observable, of } from "rxjs";
import { switchMap } from "rxjs/operators";

import { GameEvent } from "./game-event";
import { GameEventType } from "./game-event-type";
import { Player } from "../../../../_models/player.model";
import { GameWithPlayers } from "../../game-items/gameWithPlayers";

@Component({
  selector: 'yf-game-events',
  templateUrl: './game-events.component.html',
  styleUrls: ['./game-events.component.scss']
})
export class GameEventsComponent implements AfterViewInit {

  @Input() gameData: Observable<GameWithPlayers>;

  gameEvents: Observable<GameEvent[]>;

  constructor() {}

  ngAfterViewInit(): void {
    this.gameEvents = this.gameData.pipe(switchMap(game => this.buildGameEventsLog(game)));
  }

  public buildGameEventsLog(game: GameWithPlayers) : Observable<GameEvent[]> {
    let gameEvents: GameEvent[] = [];

    // Add the game start event
    gameEvents.push(
      new GameEvent(GameEventType.GameStart, "Game started", null, null, 0)
    );

    // Iterate through the goals collection on the game and calculate events
    game.game.goals.forEach(goal => {
      let scoringPlayer: Player;
      let scoringPlayerTeam: string;

      // Figure out who scored the goal
      switch (goal.scoringUserId) {
        case game.goldOffensePlayer.id:
          scoringPlayer = game.goldOffensePlayer;
          scoringPlayerTeam = 'gold';
          break;
        case game.goldDefensePlayer.id:
          scoringPlayer = game.goldDefensePlayer;
          scoringPlayerTeam = 'gold';
          break;
        case game.blackOffensePlayer.id:
          scoringPlayer = game.blackOffensePlayer;
          scoringPlayerTeam = 'black';
          break;
        case game.blackDefensePlayer.id:
          scoringPlayer = game.blackDefensePlayer;
          scoringPlayerTeam = 'black';
          break;
      }
      
      if (goal.isUndone) {
        gameEvents.push(new GameEvent(
          GameEventType.GoalUndo,
          `Goal Undone!`,
          null,
          null,
          goal.timeStampGameClock
        ));
      }
      else if (goal.isOwnGoal) {
        // Handle owngoals
        gameEvents.push(new GameEvent(
          GameEventType.OwnGoal,
          `Oof! Owngoal by `,
          scoringPlayer.name,
          scoringPlayerTeam,
          goal.timeStampGameClock
        ));
      }
      else {
        // Handle normal goals
        gameEvents.push(new GameEvent(
          GameEventType.GoalScored,
          'Goal scored by ',
          scoringPlayer.name,
          scoringPlayerTeam,
          goal.timeStampGameClock
        ));
      }
    });

    // If the game is over, add the game end message
    if (!game.game.isInProgress) {
      gameEvents.push(new GameEvent(
        GameEventType.GameEnd,
        "Game ended",
        null,
        null,
        gameEvents[gameEvents.length - 1].timestampSeconds
      ));
    }

    // Reverse the list so that we see them in reverse chronological order
    gameEvents.reverse();

    return of(gameEvents);
  }

}
