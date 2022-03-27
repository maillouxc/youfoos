using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities.Stats;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.DataAccess.SharedTestUtils.TestData
{
    [ExcludeFromCodeCoverage]
    public class TestStats
    {
        public static async Task InsertIntoDatabase(IMongoContext mongoContext, List<UserStats> stats)
        {
            var statsRepo = new StatsRepository(mongoContext);
            foreach (var s in stats)
            {
                await statsRepo.InsertOne(s);
            }
        }

        public static List<UserStats> GetAllTestStats()
        {
            return new List<UserStats>()
            {
                TestStats1
            };
        }

        public static UserStats TestStats1 = new UserStats()
        {
            Stats2V2 = new Stats2V2()
            {
                BestPartnerId = TestUsers.TestUser_2.Id,
                BestPartnerWinrate = .50,
                GamesAsDefense = 30,
                GamesAsOffense = 30,
                GamesLost = 30,
                GamesWon = 30,
                GoalsScoredAsDefense = 80,
                GoalsScoredAsOffense = 80,
                MostFrequentPartnerId = TestUsers.TestUser_2.Id,
                MostFrequentPartnerNumGamesPlayed = 40,
            },
            StatsOverall = new StatsOverall
            {
                GamesLost = 70,
                GamesWon = 120,
                GamesAsGold = 97,
                GamesAsBlack = 93,
                AverageGameLengthSecs = 1280,
                ShortestGameLengthSecs = 1190,
                LongestGameLengthSecs = 1350,
                TotalTimePlayedSecs = 243200,
                LongestWinStreak = 12,
                LongestLossStreak = 9,
                GoalsScored = 325,
                GoalsAllowed = 135,
                GoalsPerMinute = 0.08018092105263157894736842105263,
                OwnGoals = 75
            },
            Stats1V1 = new Stats1V1
            {
                GamesWon = 80,
                GamesLost = 50,
                GoalsScored = 135,
                GoalsAllowed = 67
            },
            Timestamp = DateTime.Now,
            UserId = TestUsers.TestUser_1.Id
        };
    }
}
