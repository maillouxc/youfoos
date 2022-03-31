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
    public class GoalStatsTests
    {
        [Theory]
        [MemberData(nameof(GetGoalsCalcTestData))]
        public void ShouldCalculateGoalsCorrectly(string userId,
                                                  IEnumerable<Game> userGames,
                                                  int expectedGoals,
                                                  int expectedOwngoals,
                                                  int expectedGoalsAllowed)
        {
            GoalStats.CalculateGoals(userId, userGames, 
                                     out int actualGoals, out int actualOwngoals, 
                                     out int actualGoalsAllowed);

            Assert.Equal(expectedGoals, actualGoals);
            Assert.Equal(expectedOwngoals, actualOwngoals);
            Assert.Equal(expectedGoalsAllowed, actualGoalsAllowed);
        }

        [Fact]
        public void OwnGoalsShouldNotCountAsGoalsScored()
        {
            var testGames = new List<Game> { TestGames.TestGame_1To2_Singles_AllOwnGoals};

            GoalStats.CalculateGoals(TestUsers.TestUser_1.Id, testGames, out int actualGoals, out int _, out int _);

            Assert.Equal(0, actualGoals);
        }

        [Fact]
        public void OffenseDefenseGoalsShouldThrowExceptionWithSinglesGames()
        {
            var testGames = new List<Game>()
            {
                TestGames.TestGame_3To1_Doubles_2GoGoal_1GdGoal_1BoGoal,
                TestGames.TestGame_2to1_Singles_NoOwnGoals
            };

            Assert.Throws<ArgumentException>(() =>
            {
                GoalStats.CalculateOffenseDefenseGoals(TestUsers.TestUser_1.Id, testGames, out int _, out int _);
            });
        }

        [Theory]
        [MemberData(nameof(GetOffenseDefenseGoalsCalcTestData))]
        public void ShouldCalculateOffenseDefenseGoalsCorrectly(string userId, 
                                                                List<Game> userGames,
                                                                int expectedOffenseGoals,
                                                                int expectedDefenseGoals)
        {
            GoalStats.CalculateOffenseDefenseGoals(userId, userGames, 
                                                   out int actualDefenseGoals, 
                                                   out int actualOffenseGoals);

            Assert.Equal(expectedOffenseGoals, actualOffenseGoals);
            Assert.Equal(expectedDefenseGoals, actualDefenseGoals);
        }

        public static IEnumerable<object[]> GetGoalsCalcTestData()
        {
            return new TheoryData<string, List<Game>, int, int, int>
            {
                { TestUsers.TestUser_1.Id, new List<Game>()
                    { TestGames.TestGame_2to1_Singles_NoOwnGoals }, 2, 0, 1},
                { TestUsers.TestUser_1.Id, new List<Game>()
                    { TestGames.TestGame_1To2_Singles_AllOwnGoals }, 0, 2, 0},
                { TestUsers.TestUser_1.Id, new List<Game>()
                    { TestGames.TestGame_1To3_Singles_2OwnGoals_1UndoneOwnGoal }, 1, 2, 1},
                { TestUsers.TestUser_1.Id, new List<Game>()
                    { TestGames.TestGame_3To1_Doubles_2GoGoal_1GdGoal_1BoGoal }, 2, 0, 1 }
            };
        }

        public static IEnumerable<object[]> GetOffenseDefenseGoalsCalcTestData()
        {
            return new TheoryData<string, List<Game>, int, int>
            {
                { TestUsers.TestUser_1.Id, new List<Game>()
                    { TestGames.TestGame_3To1_Doubles_2GoGoal_1GdGoal_1BoGoal }, 2, 0 },
                { TestUsers.TestUser_3_Anon.Id, new List<Game>()
                    { TestGames.TestGame_3To1_Doubles_2GoGoal_1GdGoal_1BoGoal }, 0, 1 },
            };
        }
    }
}
