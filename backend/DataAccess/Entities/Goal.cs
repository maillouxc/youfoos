using System;
using YouFoos.DataAccess.Entities.Enums;

namespace YouFoos.DataAccess.Entities
{
    /// <summary>
    /// Represents a single goal scored in a game of Foosball.
    /// </summary>
    public class Goal
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Goal(Guid gameGuid)
        {
            GameGuid = gameGuid;
            IsUndone = false;
        }

        /// <summary>
        /// The GUID of the game in which the goal was scored.
        /// </summary>
        public Guid GameGuid { get; set; }

        /// <summary>
        /// Whether the goal has been undone. Undone goals should not be used for any calculations.
        /// </summary>
        public bool IsUndone { get; set; }

        /// <summary>
        /// The user ID (not RFID number) of the player who scored the goal.
        /// </summary>
        public string ScoringUserId { get; set; }

        /// <summary>
        /// Whether or not the goal was scored by a player against their own team.
        /// </summary>
        public bool IsOwnGoal { get; set; }

        /// <summary>
        /// The team that the goal was scored against.
        /// </summary>
        public TeamColor TeamScoredAgainst { get; set; }

        /// <summary>
        /// A UTC timestamp of when the goal was scored.
        /// </summary>
        public DateTime TimeStampUtc { get; set; }

        /// <summary>
        /// The number of seconds into the game at when the goal was scored.
        /// </summary>
        public long TimeStampGameClock { get; set; }
    }
}
