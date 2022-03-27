using System.Collections.Generic;
using System.Linq;
using YouFoos.DataAccess.Entities;

namespace YouFoos.StatisticsService.StatCalculations
{
    /// <summary>
    /// This class contains methods for calculating the longest win and loss streaks given a set of games.
    /// </summary>
    public static class WinLossStreak
    {
        /// <summary>
        /// Given a list of games and a user's ID, returns the length of the longest unbroken
        /// winning streak among the games. This method sorts the list, so there is no need
        /// to sort it beforehand.
        /// </summary>
        public static int CalculateLongestWinStreak(string userId, IEnumerable<Game> userGames)
        {
            var currentWinStreak = 0;
            var longestWinStreak = 0;

            foreach (var game in userGames.OrderBy(g => g.StartTimeUtc))
            {
                if (game.GetPlayerTeam(userId) == game.GetWinningTeam())
                {
                    currentWinStreak++;
                }
                else
                {
                    if (currentWinStreak > longestWinStreak)
                    {
                        longestWinStreak = currentWinStreak;
                    }

                    currentWinStreak = 0;
                }
            }

            return (currentWinStreak > longestWinStreak) ? currentWinStreak : longestWinStreak;
        }

        /// <summary>
        /// Given a list of games and a user's ID, returns the length of the longest unbroken
        /// winning streak among the games. This method sorts the list, so there is no need
        /// to sort it beforehand.
        /// </summary>
        public static int CalculateLongestLossStreak(string userId, IEnumerable<Game> userGames)
        {
            var currentLossStreak = 0;
            var longestLossStreak = 0;

            foreach (var game in userGames.OrderBy(g => g.StartTimeUtc))
            {
                if (game.GetPlayerTeam(userId) != game.GetWinningTeam())
                {
                    currentLossStreak++;
                }
                else
                {
                    if (currentLossStreak > longestLossStreak)
                    {
                        longestLossStreak = currentLossStreak;
                    }

                    currentLossStreak = 0;
                }
            }

            return (currentLossStreak > longestLossStreak) ? currentLossStreak : longestLossStreak;
        }
    }
}
