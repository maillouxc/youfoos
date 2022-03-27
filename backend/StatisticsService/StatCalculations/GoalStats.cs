using System;
using System.Collections.Generic;
using YouFoos.DataAccess.Entities;

namespace YouFoos.StatisticsService.StatCalculations
{
    /// <summary>
    /// This class contains methods used for calculating the various goal-related stats we provide.
    /// </summary>
    public static class GoalStats
    {
        /// <summary>
        /// Returns the number of goals scored, owngoals, and goals allowed for the user with the given
        /// ID in the provided list of games.
        /// </summary>
        /// <remarks>
        /// Owngoals scored by the other team don't count as goals scored by the player.
        /// </remarks>
        /// <param name="userId">The ID of the user to calculate the stats for.</param>
        /// <param name="userGames">The list of games to calculate the stats for.</param>
        /// <param name="goals">The number of goals scored by the player.</param>
        /// <param name="ownGoals">The number of goals the user has scored on themselves.</param>
        /// <param name="goalsAllowed">The number of goals scored against the player.</param>
        public static void CalculateGoals(string userId, IEnumerable<Game> userGames,
                                          out int goals, out int ownGoals, out int goalsAllowed)
        {
            goals = 0;
            ownGoals = 0;
            goalsAllowed = 0;

            foreach (var game in userGames)
            {
                foreach (var goal in game.Goals)
                {
                    if (goal.IsUndone) continue;

                    var playerTeam = game.GetPlayerTeam(userId);
                    var scoringPlayerTeam = game.GetPlayerTeam(goal.ScoringUserId);

                    if (goal.IsOwnGoal && goal.ScoringUserId == userId) ownGoals++;
                    if (!goal.IsOwnGoal && goal.ScoringUserId == userId) goals++;
                    if (!goal.IsOwnGoal && scoringPlayerTeam != playerTeam) goalsAllowed++;
                }
            }
        }

        /// <summary>
        /// Returns the number of goals scored as offense and the number of goals scored as defense,
        /// when provided a list of games the user with the given ID has played in.
        /// </summary>
        public static void CalculateOffenseDefenseGoals(string userId, IEnumerable<Game> userGames, 
                                                        out int goalsAsDefense, out int goalsAsOffense)
        {
            var defenseGames = new List<Game>();
            var offenseGames = new List<Game>();

            foreach (var game in userGames)
            {
                if (!game.IsDoubles)
                {
                    throw new ArgumentException("Should only be called with doubles games.");
                }

                if (userId == game.GoldDefenseUserId || userId == game.BlackDefenseUserId)
                {
                    defenseGames.Add(game);
                }
                else
                {
                    offenseGames.Add(game);
                }
            }

            CalculateGoals(userId, defenseGames, out goalsAsDefense, out _, out _);
            CalculateGoals(userId, offenseGames, out goalsAsOffense, out _, out _);
        }
    }
}
