export class StatsOverallDto {
  public gamesLost: number;
  public gamesWon: number;
  public winrate: number;
  public shutoutWins: number;
  public gamesAsGold: number;
  public gamesAsBlack: number;
  public averageGameLengthSecs: number;
  public shortestGameLengthSecs: number;
  public longestGameLengthSecs: number;
  public totalTimePlayedSecs: number;
  public longestWinStreak: number;
  public longestLossStreak: number;
  public goalsScored: number;
  public goalsAllowed: number;
  public goalsPerMinute: number;
  public ownGoals: number;
}
