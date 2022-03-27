import { GameType } from "../enums/GameType";

export class CreateTournamentRequest {

  constructor(
    public name: string,
    public description: string,
    public startDate: string,
    public endDate: string,
    public playerCount: number,
    public gameType: GameType
  ) {}

}
