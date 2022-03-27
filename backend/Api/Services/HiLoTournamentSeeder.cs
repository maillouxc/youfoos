using System;
using System.Collections.Generic;
using System.Linq;
using YouFoos.DataAccess;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Entities.Account;
using YouFoos.DataAccess.Entities.Enums;
using YouFoos.SharedLibrary.Extensions;

namespace YouFoos.Api.Services
{
    public class HiLoTournamentSeeder
    {
        private static readonly Random rng = new Random();

        public void SeedTournament(Tournament tournament, List<User> players)
        {
            List<Team> unseededTeams = CreateTeams(players);
            unseededTeams.OrderByDescending(team => team.GetSummedRatings());

            TournamentRound firstRoundLeftSide = tournament.Rounds.First();
            TournamentRound firstRoundRightSide = tournament.Rounds.Last();

            var unseededLeftSideMatchups = firstRoundLeftSide.Matchups;
            var unseededRightSideMatchups = firstRoundRightSide.Matchups;

            // This will hold the list of matchups in the order we want to seed them.
            var unseededMatchups = new List<TournamentMatchup>();

            while (unseededLeftSideMatchups.Any())
            {
                unseededMatchups.Add(unseededLeftSideMatchups.First());
                unseededMatchups.Add(unseededLeftSideMatchups.Last());
                unseededMatchups.Add(unseededRightSideMatchups.First());
                unseededMatchups.Add(unseededRightSideMatchups.Last());

                unseededLeftSideMatchups.Remove(unseededLeftSideMatchups[0]);
                unseededLeftSideMatchups.Remove(unseededLeftSideMatchups[^1]);
                unseededRightSideMatchups.Remove(unseededRightSideMatchups[0]);
                unseededRightSideMatchups.Remove(unseededRightSideMatchups[^1]);
            }

            foreach (TournamentMatchup matchup in unseededMatchups)
            {
                SeedMatchup(matchup, unseededTeams);
            }
        }

        private void SeedMatchup(TournamentMatchup matchup, List<Team> unseededTeams)
        {
            // We don't want the better of the two teams to always be one one color, so we randomize it.
            TeamColor teamToSeedFirst = rng.Next(1) == 1 ? TeamColor.BLACK : TeamColor.GOLD;

            if (teamToSeedFirst == TeamColor.GOLD)
            {
                matchup.GoldUser1Id = unseededTeams.FirstOrDefault()?.Player1?.Id;
                matchup.GoldUser2Id = unseededTeams.FirstOrDefault()?.Player2?.Id;
                unseededTeams.RemoveAt(0);

                matchup.BlackUser1Id = unseededTeams.LastOrDefault()?.Player1?.Id;
                matchup.BlackUser2Id = unseededTeams.LastOrDefault()?.Player2?.Id;
                unseededTeams.RemoveAt(unseededTeams.Count - 1);
            }
            else
            {
                matchup.BlackUser1Id = unseededTeams.FirstOrDefault()?.Player1?.Id;
                matchup.BlackUser2Id = unseededTeams.FirstOrDefault()?.Player2?.Id;
                unseededTeams.RemoveAt(0);

                matchup.GoldUser1Id = unseededTeams.LastOrDefault()?.Player1?.Id;
                matchup.GoldUser2Id = unseededTeams.LastOrDefault()?.Player2?.Id;
                unseededTeams.RemoveAt(unseededTeams.Count - 1);
            }
        }

        private List<Team> CreateTeams(List<User> players)
        {
            var teams = new List<Team>();

            players.Shuffle();

            for (int i = 0; i < players.Count; i += 2)
            {
                User player1 = players[i];
                User player2 = null;

                if (i + 1 < players.Count)
                {
                    player2 = players[i + 2];
                }

                teams.Add(new Team(player1, player2));
            }

            return teams;
        }
    }
}
