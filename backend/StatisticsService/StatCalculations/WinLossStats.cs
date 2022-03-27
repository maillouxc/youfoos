using System.Collections.Generic;
using System.Linq;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Entities.Enums;

namespace YouFoos.StatisticsService.StatCalculations
{
    public class WinLossStats
    {
        /// <summary>
        /// Given a list of games, returns the number of wins and losses for the player with the
        /// given ID. Our system does not accept tie games, so no need to worry about that.
        /// </summary>
        public static void CalculateWinsAndLosses(string userId, IEnumerable<Game> userGames, 
                                                  out int wins, out int losses)
        {
            var totalGames = userGames.Count();
            wins = userGames.Count(g => g.GetPlayerTeam(userId) == g.GetWinningTeam());
            losses = totalGames - wins;
        }

        /// <summary>
        /// Given a list of games, returns the number of games where the player beat the other team
        /// without the other team scoring a single point.
        /// </summary>
        public static int CalculateShutoutWins(string userId, IEnumerable<Game> userGames)
        {
            int result = 0;
            foreach (var game in userGames)
            {
                if (game.GetPlayerTeam(userId) == TeamColor.BLACK)
                {
                    if (game.GoldTeamScore == 0 && game.BlackTeamScore >= game.GoldTeamScore)
                    {
                        result++;
                    }
                }
                else
                {
                    if (game.BlackTeamScore == 0 && game.GoldTeamScore >= game.BlackTeamScore)
                    {
                        result++;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Calculates the winrate (expressed as a number between 0 and 1) give the number of games won and lost.
        /// </summary>
        public static double CalculateWinrate(int gamesWon, int gamesLost)
        {
            if (gamesWon == 0) return 0;

            return (double)gamesWon / (gamesWon + gamesLost);
        }
    }
}
