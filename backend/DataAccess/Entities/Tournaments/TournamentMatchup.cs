using System;
using YouFoos.DataAccess.Entities.Enums;

namespace YouFoos.DataAccess.Entities
{
    /// <summary>
    /// Represents a single matchup in a tournament.
    /// 
    /// Note that this concept does not necessarily correspond one-to-one with a game.
    /// For instance, some matchups might represent a game that has not yet taken place.
    /// </summary>
    public class TournamentMatchup
    {
        /// <summary>
        /// The unique ID of the matchup.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The ID of the round that the matchup is a part of.
        /// </summary>
        public Guid RoundId { get; set; }

        /// <summary>
        /// The type of game that the matchup is - Singles (1v1) or Doubles (2v2).
        /// </summary>
        public GameType GameType { get; set; }

        /// <summary>
        /// The ID of the game that was played for this matchup, if applicable.
        /// </summary>
        public Guid? GameId { get; set; }

        /// <summary>
        /// The user ID of the 1st gold team user in the game.
        /// </summary>
        public string GoldUser1Id { get; set; }
        
        /// <summary>
        /// The user ID of the 2nd gold team user in the game - or null if a 1v1 game.
        /// </summary>
        public string GoldUser2Id { get; set; }
        
        /// <summary>
        /// The user ID of the 1st black team user in the game.
        /// </summary>
        public string BlackUser1Id { get; set; }

        /// <summary>
        /// The userId of the 2nd black team user in the game - or null if a 1v1 game.
        /// </summary>
        public string BlackUser2Id { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TournamentMatchup(GameType gameType)
        {
            Id = Guid.NewGuid();
            GameType = gameType;
        }

        /// <summary>
        /// Returns true if the matchup has the two given players on the same side.
        /// </summary>
        public bool HasTeam(string player1Id, string player2Id)
        {
            if (GameType == GameType.Singles)
            {
                throw new InvalidOperationException("This method is not valid for 1v1 games.");
            }

            bool isGoldTeam = (player1Id == GoldUser1Id && player2Id == GoldUser2Id)
                || (player1Id == GoldUser2Id && player2Id == GoldUser1Id);
            bool isBlackTeam = (player1Id == BlackUser1Id && player2Id == BlackUser2Id)
                || (player1Id == BlackUser2Id && player2Id == BlackUser1Id);

            return isGoldTeam || isBlackTeam;
        }
    }
}
