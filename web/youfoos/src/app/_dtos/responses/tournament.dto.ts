import { TournamentRoundDto } from "./tournamentRound.dto";
import { TournamentState } from "../enums/TournamentState";

export class TournamentDto {
  public id: string;
  public name: string;
  public startDate: string;
  public endDate: string;
  public playerCount: number;
  public playerIds: string[];
  public currentState: TournamentState;
  public rounds: TournamentRoundDto[];
}
