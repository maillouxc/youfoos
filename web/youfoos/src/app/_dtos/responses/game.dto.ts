import { GoalDto } from './goal.dto';

export class GameDto {
  public guid: string;
  public gameNumber: number;
  public blackOffenseUserId: string;
  public blackDefenseUserId: string;
  public goldOffenseUserId: string;
  public goldDefenseUserId: string;
  public goldTeamScore: number;
  public blackTeamScore: number;
  public goals: GoalDto[];
  public isDoubles: boolean;
  public isInProgress: boolean;
  public tournamentId: string;
}
