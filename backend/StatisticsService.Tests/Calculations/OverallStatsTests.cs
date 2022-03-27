using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.SharedTestUtils.TestData;
using YouFoos.StatisticsService.StatCalculations;

namespace YouFoos.StatisticsService.Tests.Unit.Calculations
{
    [ExcludeFromCodeCoverage]
    public class OverallStatsTests
    {
        [Fact]
        public void GivenGames_CalculateOffenseAndDefenseGamesCount_CalculatesCorrectly()
        {
            var testGames = new List<Game>()
            {
                TestGames.TestGame_3To1_Doubles_2GoGoal_1GdGoal_1BoGoal
            };

            OverallStats.OffAndDefGamesCount(TestUsers.TestUser_3_Anon.Id, testGames, 
                                             out var actualDefenseGames, 
                                             out var actualOffenseGames);

            Assert.Equal(0, actualOffenseGames);
            Assert.Equal(1, actualDefenseGames);
        }

        [Fact]
        public void GivenSinglesGames_OffDefCount_ShouldThrowException()
        {
            var testGames = new List<Game>()
            {
                TestGames.TestGame_3To1_Doubles_2GoGoal_1GdGoal_1BoGoal,
                TestGames.TestGame_2to1_Singles_NoOwnGoals
            };

            Assert.Throws<ArgumentException>(() =>
            {
                OverallStats.OffAndDefGamesCount("", testGames, out var i, out var j);
            });
        }

        [Fact]
        public void GivenGames_CalculateOverallGamesAsGoldAndBlack_CalculatesCorrectly()
        {
            var testGames = new List<Game>()
            {
                TestGames.TestGame_2to1_Singles_NoOwnGoals,
                TestGames.TestGame_1To2_Singles_AllOwnGoals,
                TestGames.TestGame_2To1_Singles_NoOwnGoals_SwappedPlayers,
            };

            OverallStats.CalculateGamesAsGoldAndBlack(TestUsers.TestUser_1.Id, testGames, out var gold, out var black);

            Assert.Equal(2, gold);
            Assert.Equal(1, black);
        }

        [Fact]
        public void GivenNullListOfGames_CalculateGamesAsGoldAndBlack_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() =>
            {
                OverallStats.CalculateGamesAsGoldAndBlack("", null, out var i, out var j);
            });
        }

        [Fact]
        public void GivenEmptyListOfGames_CalculateGamesAsGoldAndBlack_Returns0()
        {
            OverallStats.CalculateGamesAsGoldAndBlack("", new List<Game>(), out var i, out var j);

            Assert.Equal(0, i);
            Assert.Equal(0, j);
        }
    }
}
