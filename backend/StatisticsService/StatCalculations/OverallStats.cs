using System;
using System.Collections.Generic;
using System.Linq;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Entities.Enums;

namespace YouFoos.StatisticsService.StatCalculations
{
    /// <summary>
    /// This class contains several methods for performing miscellaneous overall stats calculations,
    /// such as the number of games as gold and black, etc.
    /// </summary>
    public static class OverallStats
    {
        /// <summary>
        /// Given a list of games and a user ID, returns the number of games played as offense
        /// and the number of games played as defense by the given user.
        /// </summary>
        /// <remarks>
        /// If passed a singles game, an ArgumentException will be thrown.
        /// </remarks>
        public static void OffAndDefGamesCount(string userId, IReadOnlyCollection<Game> userGames,
                                               out int gamesAsDefense, out int gamesAsOffense)
        {
            if (userGames.Any(g => !g.IsDoubles))
            {
                throw new ArgumentException("This method does not support singles games.");
            }

            var totalGames = userGames.Count();
            gamesAsDefense = userGames.Count(g => (g.GetPlayerPosition(userId) == Position.Defense));
            gamesAsOffense = totalGames - gamesAsDefense;
        }

        /// <summary>
        /// Given a list of games and a user ID, returns the number of games the user has played
        /// for the black team, and the number of games on the gold team.
        /// </summary>
        public static void CalculateGamesAsGoldAndBlack(string userId, IEnumerable<Game> userGames,
                                                        out int gamesAsGold, out int gamesAsBlack)
        {
            gamesAsGold = 0;
            gamesAsBlack = 0;

            foreach (var game in userGames)
            {
                if (game.GetPlayerTeam(userId) == TeamColor.GOLD) gamesAsGold++;
                if (game.GetPlayerTeam(userId) == TeamColor.BLACK) gamesAsBlack++;
            }
        }
    }
}
