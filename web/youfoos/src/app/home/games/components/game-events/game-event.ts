import { GameEventType } from "./game-event-type";

export class GameEvent {

  constructor(public eventType: GameEventType,
              public message: string,
              public scoringPlayerName: string,
              public scoringPlayerTeam: string,
              public timestampSeconds: number) {}

}
