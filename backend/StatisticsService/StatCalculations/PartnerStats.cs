using System;
using System.Collections.Generic;
using System.Linq;
using YouFoos.DataAccess.Entities;

namespace YouFoos.StatisticsService.StatCalculations
{
    public static class PartnerStats
    {
        public static void CalculateMostFrequentTeamMate(string userId, IReadOnlyCollection<Game> userGames, 
                                                         out string teamMemberId, out int gamesTogether)
        {
            if (userGames == null)
            {
                teamMemberId = null;
                gamesTogether = 0;
                return;
            }
            
            var teamMates = CalculateTotalGamesWithTeamMates(userId, userGames);
            if (teamMates.Count == 0)
            {
                teamMemberId = null;
                gamesTogether = 0;
                return;
            }

            // Used variable here because out variables cannot be used in lambda expressions.
            var maxGamesTogether = teamMates.Values.Max();
            teamMemberId = teamMates.FirstOrDefault(o => o.Value == maxGamesTogether).Key;
            gamesTogether = maxGamesTogether;
        }

        public static void CalculateBestTeamMate(string userId, IReadOnlyCollection<Game> userGames, 
                                                 out string teamMemberId, out double winRate)
        {
            if (userGames == null)
            {
                teamMemberId = null;
                winRate = 0.0;
                return;
            }
            
            var teamMateWinRates = CalculateTeammateWinRates(userId, userGames);
            if (teamMateWinRates.Count == 0)
            {
                teamMemberId = null;
                winRate = 0.0;
                return;
            }
            
            // Used variable here because out variables cannot be used in lambda expressions.
            var maxWinRate = teamMateWinRates.Values.Max(); // TODO fix floating point comparison
            var teamMemberIds = new List<string>();
            foreach (var individualId in teamMateWinRates.Keys)
            {
                if (Math.Abs(teamMateWinRates[individualId] - maxWinRate) > .1) continue;
                teamMemberIds.Add(individualId);
            }

            // This algorithm sets best team mate consistently based on number of games.
            if (teamMemberIds.Count > 1)
            {
                teamMemberId = teamMemberIds[0];
                var gameCounts = CalculateTotalGamesWithTeamMates(userId, userGames);
                foreach (var id in teamMemberIds)
                {
                    // Higher is better for this stat.
                    if (gameCounts[id] <= gameCounts[teamMemberId]) continue;
                    teamMemberId = id;
                }
            }
            else
            {
                teamMemberId = teamMemberIds[0];
            }
            
            winRate = maxWinRate;
        }
        
        public static void CalculateWorstTeamMate(string userId, IReadOnlyCollection<Game> userGames, 
                                                  out string teamMemberId, out double winRate)
        {
            if (userGames == null)
            {
                teamMemberId = null;
                winRate = 0.0;
                return;
            }
            
            var teamMateWinRates = CalculateTeammateWinRates(userId, userGames);
            if (teamMateWinRates.Count == 0)
            {
                teamMemberId = null;
                winRate = 0.0;
                return;
            }
            
            // Used variable here because out variables cannot be used in lambda expressions.
            var minWinRate = teamMateWinRates.Values.Min(); // TODO fix floating point comparison
            var teamMemberIds = new List<string>();
            foreach (var individualId in teamMateWinRates.Keys)
            {
                if (Math.Abs(teamMateWinRates[individualId] - minWinRate) > .1) continue;
                teamMemberIds.Add(individualId);
            }

            // This algorithm sets best team mate consistently based on number of games.
            if (teamMemberIds.Count > 1)
            {
                teamMemberId = teamMemberIds[0];
                var gameCounts = CalculateTotalGamesWithTeamMates(userId, userGames);
                foreach (var id in teamMemberIds)
                {
                    // Lower is better for this stat.
                    if (gameCounts[id] >= gameCounts[teamMemberId]) continue;
                    teamMemberId = id;
                }
            }
            else
            {
                teamMemberId = teamMemberIds[0];
            }
            
            winRate = minWinRate;
        }

        private static Dictionary<string, int> CalculateTotalGamesWithTeamMates(string userId, IEnumerable<Game> userGames)
        {
            var teamMates = new Dictionary<string, int>();
            foreach (var game in userGames)
            {
                if (!game.IsDoubles) continue;
                var currentTeamMate = game.GetPlayerTeammateId(userId);
                if (teamMates.ContainsKey(currentTeamMate))
                {
                    teamMates[currentTeamMate] = teamMates[currentTeamMate] + 1;
                }
                else
                {
                    teamMates[currentTeamMate] = 1;
                }
            }

            return teamMates;
        }

        private static Dictionary<string, int> CalculateGamesWonWithTeamMates(string userId, IEnumerable<Game> userGames)
        {
            var teamMateWins = new Dictionary<string, int>();
            foreach (var game in userGames)
            {
                if (!game.IsDoubles) continue;
                if (game.GetPlayerTeam(userId) == game.GetWinningTeam())
                {
                    var teamMate = game.GetPlayerTeammateId(userId);
                    if (teamMateWins.ContainsKey(teamMate))
                    {
                        teamMateWins[teamMate] = teamMateWins[teamMate] + 1;
                    }
                    else
                    {
                        teamMateWins.Add(teamMate, 1);
                    }
                }
            }

            return teamMateWins;
        }

        private static Dictionary<string, double> CalculateTeammateWinRates(string userId, IReadOnlyCollection<Game> userGames)
        {
            var teamMates = CalculateTotalGamesWithTeamMates(userId, userGames);
            var teamMateWins = CalculateGamesWonWithTeamMates(userId, userGames);
            var teamMateWinRates = new Dictionary<string, double>();

            foreach (var teamMate in teamMates.Keys)
            {
                // Don't include teammates who have played less than 5 games with the person
                if (teamMates[teamMate] < 5)
                {
                    continue;
                }

                teamMateWinRates.Add(teamMate, teamMateWins.ContainsKey(teamMate) ? ((double) teamMateWins[teamMate]) / teamMates[teamMate] : 0.0);
            }
            
            return teamMateWinRates;
        }
    }
}
