namespace YouFoos.DataAccess.Entities.Stats
{
    /// <summary>
    /// Represents a user's 1V1 stats for a single point in time.
    /// </summary>
    public class Stats1V1
    {
        /// <summary>
        /// The player's TrueSkill rating for 1v1 matches.
        /// </summary>
        public Trueskill Skill { get; set; }

        /// <summary>
        /// The number of games won in 1v1.
        /// </summary>
        public int GamesWon { get; set; }

        /// <summary>
        /// The number of games lost in 1v1.
        /// </summary>
        public int GamesLost { get; set; }

        /// <summary>
        /// The percentage of games won by the player in 1v1.
        /// </summary>
        public double Winrate { get; set; }

        /// <summary>
        /// The number of goals scored when playing 1v1.
        /// </summary>
        public int GoalsScored { get; set; }

        /// <summary>
        /// The number of goals scored when playing 2v2.
        /// </summary>
        public int GoalsAllowed { get; set; }
    }
}