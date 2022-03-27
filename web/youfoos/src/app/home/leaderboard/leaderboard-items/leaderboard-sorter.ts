/**
 * Represents the data inside of the dropdown used to sort the leaderboard.
 */
export class LeaderboardSorter {

  static sortOptionsOverall: string[] = [
    'Winrate',
    'Goals Per Minute',
    'Average Game Length',
    'Shutout Wins',
    'Goals Scored',
    'Goals Allowed',
    'Owngoals',
    'Time Played',
  ];

  static sortOptions1V1: string[] = [
    'Rank',
    'Winrate',
    'Games Won',
    'Games Lost',
    'Goals Scored',
    'Goals Allowed'
  ];

  static sortOptions2V2: string[] = [
    'Rank',
    'Winrate',
    'Offense Winrate',
    'Defense Winrate',
    'Games Won',
    'Games Lost',
    'Games as Offense',
    'Games as Defense',
    'Goals Scored as Offense',
    'Goals Scored as Defense'
  ];

  constructor() {}

  static formatSortOptionsAsStatNameForBackend(sortOption: string): string {
    switch (sortOption) {
      case 'Rank':
        return 'Rank';
      case 'Games Won':
        return 'GamesWon';
      case 'Games Lost':
        return 'GamesLost';
      case 'Winrate':
        return 'Winrate';
      case 'Offense Winrate':
        return 'OffenseWinrate';
      case 'Defense Winrate':
        return 'DefenseWinrate';
      case 'Shutout Wins':
        return 'ShutoutWins';
      case 'Goals Scored':
        return 'GoalsScored';
      case 'Goals Allowed':
        return 'GoalsAllowed';
      case 'Games as Offense':
        return 'GamesAsOffense';
      case 'Games as Defense':
        return 'GamesAsDefense';
      case 'Goals Scored as Offense':
        return 'GoalsScoredAsOffense';
      case 'Goals Scored as Defense':
        return 'GoalsScoredAsDefense';
      case 'Goals Per Minute':
        return 'GoalsPerMinute';
      case 'Average Game Length':
        return 'AverageGameLengthSecs';
      case 'Longest Win Streak':
        return 'LongestWinStreak';
      case 'Longest Loss Streak':
        return 'LongestLossStreak';
      case 'Time Played':
        return 'TotalTimePlayedSecs';
      case 'Owngoals':
        return 'OwnGoals';
    }
  }

}
