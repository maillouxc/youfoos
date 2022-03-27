using MongoDB.Bson.Serialization.Attributes;

namespace YouFoos.DataAccess.Entities.Stats
{
    /// <summary>
    /// Represents a user's overall stats that apply across both 1v1 and 2v2 games.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class StatsOverall
    {
        /// <summary>
        /// The number of games won - both 1v1 and 2v2 combined.
        /// </summary>
        public int GamesWon { get; set; }

        /// <summary>
        /// The number of games lost - both 1v1 and 2v2 combined.
        /// </summary>
        public int GamesLost { get; set; }

        /// <summary>
        /// The percentage of games won by the player.
        /// </summary>
        public double Winrate { get; set; }

        /// <summary>
        /// The number of games won where the other team didn't score a single point.
        /// </summary>
        public int ShutoutWins { get; set; }

        /// <summary>
        /// The number of games played on the Gold side.
        /// </summary>
        public int GamesAsGold { get; set; }

        /// <summary>
        /// The number of games played on the Black side.
        /// </summary>
        public int GamesAsBlack { get; set; }

        /// <summary>
        /// The average length of games played by the player, in seconds.
        /// </summary>
        public double AverageGameLengthSecs { get; set; }

        /// <summary>
        /// The length of the shortest single game played by the player, in seconds.
        /// </summary>
        public double ShortestGameLengthSecs { get; set; }

        /// <summary>
        /// The length of the longest single game played by the player, in seconds.
        /// </summary>
        public double LongestGameLengthSecs { get; set; }

        /// <summary>
        /// The total time played by this player, in seconds.
        /// </summary>
        public double TotalTimePlayedSecs { get; set; }

        /// <summary>
        /// The longest number of games won in a row by this player.
        /// </summary>
        public int LongestWinStreak { get; set; }

        /// <summary>
        /// The longest number of games lost in a row by this player.
        /// </summary>
        public int LongestLossStreak { get; set; }

        /// <summary>
        /// The total number of goals scored by this player.
        /// </summary>
        public int GoalsScored { get; set; }

        /// <summary>
        /// The total number of goals scored on this player.
        /// </summary>
        public int GoalsAllowed { get; set; }

        /// <summary>
        /// The average number of goals scored per minute.
        /// </summary>
        public double GoalsPerMinute { get; set; }

        /// <summary>
        /// The number of goals the player has scored on themselves.
        /// </summary>
        public int OwnGoals { get; set; }
    }
}
