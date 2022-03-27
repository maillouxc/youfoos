using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using YouFoos.DataAccess.Entities.Enums;

namespace YouFoos.DataAccess.Entities
{
    /// <summary>
    /// A game of foosball.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// The number of goals needed for a game to be considered won.
        /// </summary>
        [BsonIgnore]
        public const int NumGoalsNeededToWin = 5;

        /// <summary>
        /// The unqiue ID for the game.
        /// </summary>
        [BsonId]
        public Guid Guid { get; set; }

        /// <summary>
        /// The sequential game number of the game - e.g. game 1, game 2, game 3.
        /// </summary>
        public int GameNumber { get; set; }

        /// <summary>
        /// The user ID of the person playing offense for the black team.
        /// </summary>
        public string BlackOffenseUserId { get; set; }
        
        /// <summary>
        /// The user ID of the person playing defense for the black team.
        /// </summary>
        public string BlackDefenseUserId { get; set; }
        
        /// <summary>
        /// The user ID of the person playing offense for the gold team.
        /// </summary>
        public string GoldOffenseUserId { get; set; }
        
        /// <summary>
        /// The user ID of the person playing defense for the gold team.
        /// </summary>
        public string GoldDefenseUserId { get; set; }

        /// <summary>
        /// The current score of the gold team.
        /// </summary>
        public int GoldTeamScore { get; set; }

        /// <summary>
        /// The current score of the black team.
        /// </summary>
        public int BlackTeamScore { get; set; }
        
        /// <summary>
        /// The list of goals that have been scored during the game, including undone goals.
        /// </summary>
        public List<Goal> Goals { get; set; }
        
        /// <summary>
        /// Whether the game is a 2v2 game. If false, the game is 1v1.
        /// </summary>
        public bool IsDoubles { get; set; }
        
        /// <summary>
        /// The UTC datetime at which the game began.
        /// </summary>
        public DateTime StartTimeUtc { get; set; }
        
        /// <summary>
        /// The UTC datetime at which the game ended, if it is not currently in progress.
        /// </summary>
        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        public DateTime EndTimeUtc { get; set; }
        
        /// <summary>
        /// Whether or not the game is currently ongoing.
        /// </summary>
        public bool IsInProgress { get; set; }

        /// <summary>
        /// The ID of the tournament that this game is a part of, if there is one.
        /// </summary>
        public Guid? TournamentId { get; set; }

        /// <summary>
        /// The unique ID of the matchup that this game is for in a tournament, if this is a tournament game.
        /// </summary>
        public Guid? TournamentMatchupId { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Game(Guid id, bool isInProgress = true)
        {
            Guid = id;
            GoldTeamScore = 0;
            BlackTeamScore = 0;
            Goals = new List<Goal>();
            IsInProgress = isInProgress;
        }

        /// <summary>
        /// Returns the team of a player who played in this game, when given the player id.
        /// </summary>
        /// <exception cref="ArgumentException">The player was not in the game.</exception>
        public TeamColor GetPlayerTeam(string playerId)
        {
            var isGold = (playerId == GoldOffenseUserId) || (playerId == GoldDefenseUserId);
            var isBlack = (playerId == BlackOffenseUserId) || (playerId == BlackDefenseUserId);

            if (isGold) return TeamColor.GOLD;
            if (isBlack) return TeamColor.BLACK;
            
            throw new ArgumentException("Player was not in this game. Player ID: " + playerId + " Game ID: " + Guid);
        }

        /// <summary>
        /// Returns the team a player was playing against.
        /// </summary>
        public TeamColor GetOpposingTeam(string playerId)
        {
            TeamColor playerTeam = GetPlayerTeam(playerId);
            return playerTeam == TeamColor.GOLD ? TeamColor.BLACK : TeamColor.GOLD;
        }

        /// <summary>
        /// Recalculates the score based on the goals.
        /// </summary>
        public void RecalculateScore()
        {
            int blackGoals = 0;
            int goldGoals = 0;
            foreach (var goal in Goals)
            {
                if (goal.IsUndone) continue;
                if (goal.TeamScoredAgainst == TeamColor.BLACK)
                {
                    goldGoals++;
                }
                else
                {
                    blackGoals++;
                }
            }

            GoldTeamScore = goldGoals;
            BlackTeamScore = blackGoals;
        }

        /// <summary>
        /// If the game was a doubles game, returns the ID of the player's teammate.
        /// </summary>
        /// <exception cref="ArgumentException">If the player was not in the game.</exception>
        public string GetPlayerTeammateId(string playerId)
        {
            switch (GetPlayerTeam(playerId))
            {
                case TeamColor.GOLD:
                    return playerId == GoldDefenseUserId ? GoldOffenseUserId : GoldDefenseUserId;
                case TeamColor.BLACK:
                    return playerId == BlackDefenseUserId ? BlackOffenseUserId : BlackDefenseUserId;
                default:
                    throw new ArgumentException("Player was not in this game.");
            }
        }

        /// <summary>
        /// Returns the position (either offense or defense) of a player in this game, given the player id.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the game was a 1v1 game.</exception>
        /// <exception cref="ArgumentException">If the player was not in the game.</exception>
        public Position GetPlayerPosition(string playerId)
        {
            if (!IsDoubles) 
            {
                throw new InvalidOperationException("Cannot get player position for a 1v1 game.");
            }

            var isOffense = (playerId == GoldOffenseUserId) || (playerId == BlackOffenseUserId);
            var isDefense = (playerId == GoldDefenseUserId) || (playerId == BlackDefenseUserId);

            if (isOffense) return Position.Offense;
            if (isDefense) return Position.Defense;

            throw new ArgumentException("Player was not in this game.");
        }

        /// <summary>
        /// Returns how long the game lasted, in seconds.
        /// </summary>
        /// <returns></returns>
        public double GetDurationInSeconds()
        {
            // TODO is this how we are getting those super long games showing up in the database sometimes?

            if (EndTimeUtc.Year == 1)
            {
                return (DateTime.UtcNow - StartTimeUtc).TotalSeconds;
            }

            return (EndTimeUtc - StartTimeUtc).TotalSeconds;
        }

        /// <summary>
        /// Returns true if the player with the given ID played in the game - false if they did not.
        /// </summary>
        public bool WasPlayerInGame(string playerId)
        {
            return BlackOffenseUserId == playerId 
                   || BlackDefenseUserId == playerId
                   || GoldDefenseUserId == playerId
                   || GoldOffenseUserId == playerId;
        }

        /// <summary>
        /// Returns the team who is currently leading, or the team that won - depending on whether the game is ongoing.
        /// </summary>
        public TeamColor GetWinningTeam()
        {
            return GoldTeamScore > BlackTeamScore ? TeamColor.GOLD : TeamColor.BLACK;
        }

        /// <summary>
        /// Returns the score of the team given.
        /// </summary>
        /// <param name="team">The team to get the score for.</param>
        /// <returns>The score of the given team.</returns>
        public int GetTeamScore(TeamColor team)
        {
            switch (team)
            {
                case TeamColor.GOLD:
                    return GoldTeamScore;
                case TeamColor.BLACK:
                    return BlackTeamScore;
                default:
                    throw new ArgumentException($"Unsupported team value: {team}", nameof(team));
            }
        }

        /// <summary>
        /// Returns a deduplicated set of user Ids for the players in the game.
        /// </summary>
        public HashSet<string> GetPlayerIds()
        {
            return new HashSet<string>()
            {
                BlackOffenseUserId,
                BlackDefenseUserId,
                GoldOffenseUserId,
                GoldDefenseUserId
            };
        }
    }
}
