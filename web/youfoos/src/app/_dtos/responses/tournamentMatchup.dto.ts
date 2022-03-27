import { GameType } from "../enums/GameType";

export class TournamentMatchupDto {
  public gameType: GameType;
  public goldUser1Id: string;
  public goldUser2Id: string;
  public blackUser1Id: string;
  public blackUser2Id: string;
  public goldTeamScore: number;
  public blackTeamScore: number;
}
