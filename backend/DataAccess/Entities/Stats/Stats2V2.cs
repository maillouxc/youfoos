namespace YouFoos.DataAccess.Entities.Stats
{
    /// <summary>
    /// Represents a user's 2v2 stats from a single point in time.
    /// </summary>
    public class Stats2V2
    {
        /// <summary>
        /// The user's TrueSkill ranking for 2v2 games.
        /// </summary>
        public Trueskill Skill { get; set; }

        /// <summary>
        /// The number of games won in 2v2.
        /// </summary>
        public int GamesWon { get; set; }

        /// <summary>
        /// The number of games lost in 2v2.
        /// </summary>
        public int GamesLost { get; set; }

        /// <summary>
        /// The percentage of games won by the player in 2v2.
        /// </summary>
        public double Winrate { get; set; }

        /// <summary>
        /// The percentage of games won by the player as offense.
        /// </summary>
        public double OffenseWinrate { get; set; }

        /// <summary>
        /// The percentage of games won by the player as defense.
        /// </summary>
        public double DefenseWinrate { get; set; }

        /// <summary>
        /// The number of games played as offense.
        /// </summary>
        public int GamesAsOffense { get; set; }

        /// <summary>
        /// The number of games played as defense.
        /// </summary>
        public int GamesAsDefense { get; set; }

        /// <summary>
        /// The number of games won while playing as offense. 
        /// </summary>
        public int OffenseWins { get; set; }

        /// <summary>
        /// The number of games won while playing as defense.
        /// </summary>
        public int DefenseWins { get; set; }

        /// <summary>
        /// The number of goals scored while playing as offense.
        /// </summary>
        public int GoalsScoredAsOffense { get; set; }

        /// <summary>
        /// The number of goals scored while playing as defense.
        /// </summary>
        public int GoalsScoredAsDefense { get; set; }

        /// <summary>
        /// The User ID of the player's partner with the highest winrate.
        /// Must have played at least 5 games with the person.
        /// </summary>
        public string BestPartnerId { get; set; }

        /// <summary>
        /// The winrate achieved with the best partner.
        /// </summary>
        public double BestPartnerWinrate { get; set; }

        /// <summary>
        /// The User ID of the player's partner with the lowest winrate.
        /// Must have played at least 5 games with the person.
        /// </summary>
        public string WorstPartnerId { get; set; }

        /// <summary>
        /// The winrate achieved with the worst partner.
        /// </summary>
        public double WorstPartnerWinrate { get; set; }

        /// <summary>
        /// The User ID of the person who this player has played with the most.
        /// </summary>
        public string MostFrequentPartnerId { get; set; }
        
        /// <summary>
        /// The number of games played with the most frequent partner as a teammate.
        /// </summary>
        public int MostFrequentPartnerNumGamesPlayed { get; set; }
    }
}
