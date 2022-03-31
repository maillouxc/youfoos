using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using YouFoos.DataAccess.Entities;
using YouFoos.TestData;
using YouFoos.StatisticsService.StatCalculations;

namespace YouFoos.StatisticsService.Tests.Unit.Calculations
{
    [ExcludeFromCodeCoverage]
    public class WinLossStreakStatsTests
    {
        [Fact]
        public void GivenGamesList_CalculateLongestWinStreak_WorksCorrectly()
        {
            var testGames = new List<Game>()
            {
                TestGames.TestGame_1To2_Singles_AllOwnGoals,
                TestGames.TestGame_3To1_Doubles_2GoGoal_1GdGoal_1BoGoal,
                TestGames.TestGame_2To1_Singles_NoOwnGoals_SwappedPlayers,
                TestGames.TestGame_1To3_Singles_2OwnGoals_1UndoneOwnGoal
            };

            var actualWinStreak = WinLossStreak.CalculateLongestWinStreak(TestUsers.TestUser_2.Id, testGames);

            Assert.Equal(2, actualWinStreak);
        }

        [Fact]
        public void GivenGamesList_CalculateLongestLossStreak_WorksCorrectly()
        {
            var testGames = new List<Game>()
            {
                TestGames.TestGame_2to1_Singles_NoOwnGoals,
                TestGames.TestGame_3To1_Doubles_2GoGoal_1GdGoal_1BoGoal,
                TestGames.TestGame_1To2_Singles_AllOwnGoals,
                TestGames.TestGame_2To1_Singles_NoOwnGoals_SwappedPlayers
            };

            var actualLongestLossStreak = WinLossStreak.CalculateLongestLossStreak(TestUsers.TestUser_2.Id, testGames);

            Assert.Equal(1, actualLongestLossStreak);
        }

        [Fact]
        public void GivenGamesListWithNoWins_CalculateLongestWinStreak_Returns0()
        {
            var testGames = new List<Game>()
            {
                TestGames.TestGame_1To2_Singles_AllOwnGoals
            };

            var actualWins = WinLossStreak.CalculateLongestWinStreak(TestUsers.TestUser_1.Id, testGames);

            Assert.Equal(0, actualWins);
        }

        [Fact]
        public void GivenGamesListWithNoLosses_CalculateLongestLossStreak_Returns0()
        {
            var testGames = new List<Game>()
            {
                TestGames.TestGame_1To2_Singles_AllOwnGoals
            };

            var actualLosses = WinLossStreak.CalculateLongestLossStreak(TestUsers.TestUser_2.Id, testGames);

            Assert.Equal(0, actualLosses);
        }
    }
}
