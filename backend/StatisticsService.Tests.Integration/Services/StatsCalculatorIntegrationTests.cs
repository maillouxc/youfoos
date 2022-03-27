using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Moq;
using Xunit;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Repositories;
using YouFoos.DataAccess.SharedTestUtils.TestData;
using YouFoos.StatisticsService.Services;

namespace YouFoos.StatisticsService.Tests.Integration.Services
{
    [ExcludeFromCodeCoverage]
    public class StatsCalculatorIntegrationTests
    {
        private static readonly StatsCalculator StatsCalculatorWithMockedDependencies;
        private static readonly List<Game> AllGames;
        private static readonly List<Game> User1Games;
        private static readonly List<Game> User11V1Games;
        private static readonly List<Game> User12V2Games;
        private static readonly List<Game> User2Games;
        private static readonly List<Game> User21V1Games;
        private static readonly List<Game> User22V2Games;
        
        static StatsCalculatorIntegrationTests()
        {
            var trueSkillCalculator = new Mock<ITrueskillCalculator>();
            var gamesRepository = new Mock<IGamesRepository>();
            var statsRepository = new Mock<IStatsRepository>();
            var accoladesRepository = new Mock<IAccoladesRepository>();

            var usersRepository = new Mock<IUsersRepository>();
            usersRepository.Setup(o => o.GetUserWithId(TestUsers.TestUser_1.Id)).ReturnsAsync(TestUsers.TestUser_1);

            StatsCalculatorWithMockedDependencies = new StatsCalculator(
                trueSkillCalculator.Object, usersRepository.Object, 
                gamesRepository.Object, statsRepository.Object);
            
            AllGames = TestGames.GetAllTestGames();
            User1Games = AllGames.FindAll(o => o.WasPlayerInGame(TestUsers.TestUser_1.Id));
            User11V1Games = User1Games.FindAll(o => !o.IsDoubles);
            User12V2Games = User1Games.FindAll(o => o.IsDoubles);

            User2Games = AllGames.FindAll(o => o.WasPlayerInGame(TestUsers.TestUser_2.Id));
            User21V1Games = User2Games.FindAll(o => !o.IsDoubles);
            User22V2Games = User2Games.FindAll(o => o.IsDoubles);
        }

        [Fact]
        public static void Calculate1V1StatsForUserWithIdShouldCorrectlyCalculateAllStats()
        {
            var user1Stats1V1 = StatsCalculatorWithMockedDependencies
                    .Calculate1V1StatsForUserWithId(TestUsers.TestUser_1, User11V1Games);
            Assert.Equal(1, user1Stats1V1.GamesWon);
            Assert.Equal(3, user1Stats1V1.GamesLost);
            Assert.Equal(5, user1Stats1V1.GoalsScored);
            Assert.Equal(3, user1Stats1V1.GoalsAllowed);

            var user2Stats1V1 = StatsCalculatorWithMockedDependencies
                    .Calculate1V1StatsForUserWithId(TestUsers.TestUser_2, User21V1Games);
            Assert.Equal(3, user2Stats1V1.GamesWon);
            Assert.Equal(1, user2Stats1V1.GamesLost);
            Assert.Equal(3, user2Stats1V1.GoalsScored);
            Assert.Equal(5, user2Stats1V1.GoalsAllowed);
        }

        [Fact]
        public static void Calculate2V2StatsForUserWithIdShouldCorrectlyCalculateAllStats()
        {
            var user1Stats2V2 = StatsCalculatorWithMockedDependencies
                .Calculate2V2StatsForUserWithId(TestUsers.TestUser_1, User12V2Games);

            Assert.Equal(2, user1Stats2V2.GamesWon);
            Assert.Equal(3, user1Stats2V2.GamesLost);
            Assert.Equal(0, user1Stats2V2.GamesAsDefense);
            Assert.Equal(5, user1Stats2V2.GamesAsOffense);
            Assert.Equal(0, user1Stats2V2.GoalsScoredAsDefense);
            Assert.Equal(6, user1Stats2V2.GoalsScoredAsOffense);
            Assert.Equal(TestUsers.TestUser_3_Anon.Id, user1Stats2V2.MostFrequentPartnerId);
            Assert.Equal(2, user1Stats2V2.MostFrequentPartnerNumGamesPlayed);

            var user2Stats2V2 = StatsCalculatorWithMockedDependencies
                .Calculate2V2StatsForUserWithId(TestUsers.TestUser_2, User22V2Games);

            Assert.Equal(2, user2Stats2V2.GamesWon);
            Assert.Equal(3, user2Stats2V2.GamesLost);
            Assert.Equal(3, user2Stats2V2.GamesAsDefense);
            Assert.Equal(2, user2Stats2V2.GamesAsOffense);
            Assert.Equal(1, user2Stats2V2.GoalsScoredAsDefense);
            Assert.Equal(3, user2Stats2V2.GoalsScoredAsOffense);
            Assert.True(user2Stats2V2.MostFrequentPartnerId == TestUsers.TestUser_4.Id 
                || user2Stats2V2.MostFrequentPartnerId == TestUsers.TestUser_3_Anon.Id);
            Assert.Equal(2, user2Stats2V2.MostFrequentPartnerNumGamesPlayed);
        }

        [Fact]
        public static void CalculateStatsOverallShouldCorrectlyCalculateAllStats()
        {
            var user1StatsOverall = StatsCalculatorWithMockedDependencies
                .CalculateOverallStatsForUserWithId(TestUsers.TestUser_1, User1Games);

            Assert.Equal(3, user1StatsOverall.GamesWon);
            Assert.Equal(6, user1StatsOverall.GamesLost);
            Assert.Equal(4, user1StatsOverall.OwnGoals);
            Assert.Equal(11, user1StatsOverall.GoalsScored);
            Assert.Equal(11, user1StatsOverall.GoalsAllowed);
            Assert.Equal(8, user1StatsOverall.GamesAsGold);
            Assert.Equal(1, user1StatsOverall.GamesAsBlack);
            Assert.Equal((11/42.5), user1StatsOverall.GoalsPerMinute, 3);
            Assert.Equal(2550, user1StatsOverall.TotalTimePlayedSecs, 3);
            Assert.Equal(283.333333, user1StatsOverall.AverageGameLengthSecs, 3);
            Assert.Equal(600, user1StatsOverall.LongestGameLengthSecs, 3);
            Assert.Equal(180, user1StatsOverall.ShortestGameLengthSecs, 3);
        }
    }
}
