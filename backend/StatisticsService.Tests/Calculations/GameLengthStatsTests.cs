using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using YouFoos.DataAccess.Entities;
using YouFoos.TestData;
using YouFoos.StatisticsService.StatCalculations;

namespace YouFoos.StatisticsService.Tests.Unit.Calculations
{
    [ExcludeFromCodeCoverage]
    public class GameLengthStatsTests
    {
        [Fact]
        public void ShouldCalculateShortestGameCorrectly()
        {
            var testGames = new List<Game>()
            {
                TestGames.TestGame_2to1_Singles_NoOwnGoals,
                TestGames.TestGame_1To2_Singles_AllOwnGoals
            };

            const int expectedSecs = 4 * 60;
            var actualSecs = GameLength.GetShortestGame(testGames).GetDurationInSeconds();
            var difference = Math.Abs(actualSecs - expectedSecs);
            const double allowableDeltaSecs = 0.01;

            Assert.True(difference < allowableDeltaSecs);
        }

        [Fact]
        public void ShouldCalculateLongestGameCorrectly()
        {
            var testGames = new List<Game>()
            {
                TestGames.TestGame_2to1_Singles_NoOwnGoals,
                TestGames.TestGame_1To2_Singles_AllOwnGoals
            };

            const int expectedSecs = 6 * 60;
            var actualSecs = GameLength.GetLongestGame(testGames).GetDurationInSeconds();
            var difference = Math.Abs(actualSecs - expectedSecs);
            const double allowableDeltaSecs = 0.01;

            Assert.True(difference < allowableDeltaSecs);
        }

        [Fact]
        public void ShouldCalculateAverageGameLengthCorrectly()
        {
            var testGames = new List<Game>()
            {
                TestGames.TestGame_2to1_Singles_NoOwnGoals,
                TestGames.TestGame_1To2_Singles_AllOwnGoals
            };

            const int expectedAvgTimeSecs = (10 * 60) / 2;
            var actualAvgTimeSecs = GameLength.GetAverageGameLengthSecs(testGames);
            var difference = Math.Abs(actualAvgTimeSecs - expectedAvgTimeSecs);
            const double allowableDeltaSecs = 0.1;

            Assert.True(difference < allowableDeltaSecs);
        }

        [Fact]
        public void ShouldCalculateTotalTimePlayedCorrectly()
        {
            var testGames = new List<Game>()
            {
                TestGames.TestGame_2to1_Singles_NoOwnGoals,
                TestGames.TestGame_1To2_Singles_AllOwnGoals
            };

            const int expectedTotalTimeSecs = 10 * 60;
            var actualTotalTimeSecs = GameLength.GetTotalTimePlayedSecs(testGames);
            var difference = Math.Abs(actualTotalTimeSecs - expectedTotalTimeSecs);
            const double allowableDeltaSecs = 0.01;

            Assert.True(difference < allowableDeltaSecs);
        }
    }
}
