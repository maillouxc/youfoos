using System;
using System.Collections.Generic;
using System.Linq;
using YouFoos.DataAccess.Entities.Enums;
using YouFoos.DataAccess.Entities.Tournaments;

namespace YouFoos.DataAccess.Entities
{
    public class Tournament
    {
        /// <summary>
        /// The unique ID of the tournament.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// The name of the tournament.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The text description of the tournament.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The number of players the tournament was created for.
        /// 
        /// This does not reflect the actual number of players who signed up - this is 
        /// simply the number of slots the tournament has for players when it was created.
        /// </summary>
        public int PlayerCount { get; set; }

        /// <summary>
        /// The gametype of games in the tournament - either Singles (1v1) or Doubles (2v2).
        /// </summary>
        public GameType GameType { get; set; }

        public List<TournamentRound> Rounds { get; set; }

        /// <summary>
        /// When the tournament was created.
        /// </summary>
        public DateTime CreateDate { get; set; }

        public DateTime StartDate { get; set; }

        /// <summary>
        /// The UTC DateTime when the tournament ended.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// The current status of the tournament.
        /// </summary>
        public TournamentState CurrentState { get; set; }

        /// <summary>
        /// The user ids of all players who have registered for the tournament.
        /// </summary>
        public HashSet<string> PlayerIds { get; set; } = new HashSet<string>();

        /// <summary>
        /// The user ID of the person who created the tournament.
        /// </summary>
        public string CreatorId { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Tournament(
            string name,
            GameType gameType, 
            int playerCount, 
            DateTime startDate, 
            DateTime endDate,
            string creatorId
        )
        {
            Name = name;
            GameType = gameType;
            PlayerCount = playerCount;
            CreateDate = new DateTime();
            StartDate = startDate;
            EndDate = endDate;
            CreatorId = creatorId;
            CurrentState = TournamentState.Registration;

            InitializeTournamentRounds();
        }

        private void InitializeTournamentRounds()
        {
            Rounds = new List<TournamentRound>();

            // TODO validate that playercount is a power of 2.

            int playersPerGame = GameType == GameType.Doubles ? 4 : 2;
            int round1GamesNeeded = PlayerCount / playersPerGame;

            int gamesPerRound = 1;
            while (gamesPerRound <= round1GamesNeeded)
            {
                if (gamesPerRound == 1)
                {
                    Rounds.Add(new TournamentRound("Championship", 1, GameType));
                }
                else
                {
                    // Every round except the championship is really like two half rounds (the left side and the right side)
                    Rounds.Insert(0, new TournamentRound($"Round of {gamesPerRound}", gamesPerRound / 2, GameType));
                    Rounds.Add(new TournamentRound($"Round of {gamesPerRound}", gamesPerRound / 2, GameType));
                }

                gamesPerRound <<= 1;
            }
        }

        public List<TournamentMatchup> GetAllUnplayedMatchups()
        {
            var unplayedMatchups = new List<TournamentMatchup>();

            foreach (TournamentRound round in Rounds)
            {
                foreach (TournamentMatchup matchup in round.Matchups)
                {
                    bool matchupHasBeenPlayed = matchup.GameId != null;
                    if (!matchupHasBeenPlayed)
                    {
                        unplayedMatchups.Add(matchup);
                    }
                }
            }

            return unplayedMatchups;
        }

        public void UpdateBracket(Game game)
        {
            TournamentMatchup matchup = GetMatchup(game.TournamentMatchupId);
            
            if (matchup == null)
            {
                throw new ArgumentException("Game is not a part of this tournament.");
            }
            
            TournamentRound currentRound = Rounds.Where(round => round.Id == matchup.RoundId).FirstOrDefault();

            if (currentRound == null)
            {
                throw new ArgumentException("Matchup is not a part of this tournament.");
            }

            TournamentRound nextRound = GetNextRound(currentRound);

            // If there is no next round, the tournament is over.
            if (nextRound == null)
            {
                EndDate = DateTime.UtcNow;
                CurrentState = TournamentState.Completed;
                return;
            }

            /* 
             * We next need to determine not only which match the winner should be in next, but which team color.
             *
             * To understand how we do this - consider the following bracket, where * denotes a team winning a game.
             * The matchup index within the round is also denoted between the two teams of the matchup.
             * 
             *     Team 1* ---
             * Matchups[0]    |--- Team 1 ---
             *     Team 2  ---               |
             *                 Matchups[0]   |--- Team 4--- ...
             *     Team 3  ---               |  
             * Matchups[1]    |--- Team 4*---
             *     Team 4* --- 
             */

            int currentMatchupIndexInRound = currentRound.Matchups.IndexOf(matchup);
            int nextMatchupIndexInRound = currentMatchupIndexInRound / 2;

            TeamColor nextTeam;

            // Next we determine which team they should be on in the next matchup.
            if (currentMatchupIndexInRound % 2 == 0)
            {
                nextTeam = TeamColor.BLACK;
            }
            else
            {
                nextTeam = TeamColor.GOLD;
            }

            TournamentMatchup nextMatchup = nextRound.Matchups[nextMatchupIndexInRound];
            
            if (game.GetWinningTeam() == TeamColor.GOLD)
            {
                if (nextTeam == TeamColor.BLACK)
                {
                    nextMatchup.BlackUser1Id = game.GoldOffenseUserId;
                    nextMatchup.BlackUser2Id = game.GoldDefenseUserId;
                }
                else
                {
                    nextMatchup.GoldUser1Id = game.GoldOffenseUserId;
                    nextMatchup.GoldUser2Id = game.GoldDefenseUserId;
                }
            }
            else
            {
                if (nextTeam == TeamColor.BLACK)
                {
                    nextMatchup.BlackUser1Id = game.BlackOffenseUserId;
                    nextMatchup.BlackUser2Id = game.BlackDefenseUserId;
                }
                else
                {
                    nextMatchup.GoldUser1Id = game.BlackOffenseUserId;
                    nextMatchup.GoldUser2Id = game.BlackDefenseUserId;
                }
            }
        }

        public TournamentRound GetNextRound(TournamentRound round)
        {
            int currentRoundNum = Rounds.IndexOf(round);

            // The idea here is that next round is the one that moves us closer to the championship round.
            // The rounds are in the collection from left to right, so the championship round is the middle element.
            // (There are always an odd number of rounds.)
            int championshipRoundNum = (Rounds.Count - 1) / 2;

            // If this is already the championship round, there is no next round.
            if (currentRoundNum == championshipRoundNum) return null;

            if (currentRoundNum < championshipRoundNum)
            {
                return Rounds[++currentRoundNum];
            }
            else // If the current round is on the right side of the bracket...
            {
                return Rounds[--currentRoundNum];
            }
        }

        public TournamentMatchup GetMatchup(Guid? id)
        {
            foreach (TournamentRound round in Rounds)
            {
                foreach (TournamentMatchup matchup in round.Matchups)
                {
                    if (matchup.Id == id) return matchup;
                }
            }

            return null;
        }
    }
}
