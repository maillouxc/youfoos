using System;
using System.Collections.Generic;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Entities.Enums;

namespace YouFoos.DataAccess
{
    /// <summary>
    /// Represents a single round in a tournament.
    /// </summary>
    public class TournamentRound
    {
        /// <summary>
        /// The unique ID of the tournament round.
        /// </summary>
        public Guid Id;

        /// <summary>
        /// The name of the round.
        /// </summary>
        public string Name { get; set; }

        public GameType GameType { get; set; }

        public List<TournamentMatchup> Matchups { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TournamentRound(string name, int numGames, GameType gameType)
        {
            Id = Guid.NewGuid();
            Name = name;
            GameType = gameType;
            Matchups = new List<TournamentMatchup>(numGames);

            for (int i = 0; i < numGames; i++)
            {
                Matchups.Add(new TournamentMatchup(gameType));
            }
        }
    }
}
