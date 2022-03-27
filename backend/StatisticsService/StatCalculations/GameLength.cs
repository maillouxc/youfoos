using System.Collections.Generic;
using System.Linq;
using YouFoos.DataAccess.Entities;

namespace YouFoos.StatisticsService.StatCalculations
{
    /// <summary>
    /// This class contains methods useful for calculating stats related to game length, such as longest
    /// and shortest games played within a list of games.
    /// </summary>
    public static class GameLength
    {
        /// <summary>
        /// Returns the game with the shortest overall duration from a supplied list of games.
        /// If there are two games of equal length, the first is returned.
        /// </summary>
        /// <remarks>
        /// We choose to return the whole game since we may in the future want to know specific stats related to the game.
        /// For instance, a user may want to know not only how long was the shortest game, but also which game was it, and
        /// when was it played? What was the result? Etc.
        /// </remarks>
        public static Game GetShortestGame(IReadOnlyList<Game> games)
        {
            var minLength = double.MaxValue;
            var shortestGame = games[0];

            foreach (var game in games)
            {
                if (game.GetDurationInSeconds() < minLength)
                {
                    minLength = game.GetDurationInSeconds();
                    shortestGame = game;
                }
            }

            return shortestGame;
        }

        /// <summary>
        /// Returns the game with the shortest overall duration from a supplied list of games.
        /// If there are two games of equal length, the first is returned.
        /// </summary>
        /// <remarks>
        /// We choose to return the whole game since we may in the future want to know specific stats related to the game.
        /// For instance, a user may want to know not only how long was the shortest game, but also which game was it, and
        /// when was it played? What was the result? Etc.
        /// </remarks>
        public static Game GetLongestGame(IReadOnlyList<Game> games)
        {
            var maxLength = 0.0;
            var longestGame = games[0];

            foreach (var game in games)
            {
                if (game.GetDurationInSeconds() > maxLength)
                {
                    maxLength = game.GetDurationInSeconds();
                    longestGame = game;
                }
            }

            return longestGame;
        }

        /// <summary>
        /// Returns the average game length of the games list supplied, in seconds.
        /// </summary>
        public static double GetAverageGameLengthSecs(IReadOnlyCollection<Game> games)
        {
            return games.Average(game => game.GetDurationInSeconds());
        }

        /// <summary>
        /// Returns the total duration of the provided list of games, in seconds.
        /// </summary>
        public static double GetTotalTimePlayedSecs(IReadOnlyCollection<Game> games)
        {
            return games.Sum(game => game.GetDurationInSeconds());
        }
    }
}
