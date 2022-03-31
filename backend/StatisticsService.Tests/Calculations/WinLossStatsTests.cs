using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using YouFoos.DataAccess.Entities;
using YouFoos.TestData;
using YouFoos.StatisticsService.StatCalculations;

namespace YouFoos.StatisticsService.Tests.Unit.Calculations
{
    [ExcludeFromCodeCoverage]
    public class WinLossStatsTests
    {
        [Fact]
        public void GivenGames_CalculateOverallWinsAndLosses_CalculatesCorrectly()
        {
            var testGames = new List<Game>()
            {
                TestGames.TestGame_2to1_Singles_NoOwnGoals,
                TestGames.TestGame_1To2_Singles_AllOwnGoals,
                TestGames.TestGame_2To1_Singles_NoOwnGoals_SwappedPlayers,
                TestGames.TestGame_3To1_Doubles_2GoGoal_1GdGoal_1BoGoal
            };

            WinLossStats.CalculateWinsAndLosses(TestUsers.TestUser_1.Id, testGames, out var wins, out var losses);

            Assert.Equal(2, wins);
            Assert.Equal(2, losses);
        }
    }
}
