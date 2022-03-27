import { TournamentMatchupDto } from "./tournamentMatchup.dto";

export class TournamentRoundDto {
  public name: string;
  public matchups: TournamentMatchupDto[];
}
