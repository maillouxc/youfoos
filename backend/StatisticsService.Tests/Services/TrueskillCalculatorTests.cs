using YouFoos.DataAccess.Entities.Stats;
using YouFoos.DataAccess.SharedTestUtils.TestData;
using YouFoos.StatisticsService.Services;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace YouFoos.StatisticsService.Tests.Unit.Services
{
    [ExcludeFromCodeCoverage]
    public class TrueskillCalculatorTests
    {
        private readonly TrueskillCalculator _calculator;

        public TrueskillCalculatorTests()
        {
            _calculator = new TrueskillCalculator();
        }

        [Fact]
        public void TwoPlayerTestNotDrawn()
        {
            // Arrange
            var goldPlayer = TestUsers.TestUser_1;
            var blackPlayer = TestUsers.TestUser_2;

            // Act
            var matchQuality = _calculator.CalculateMatchQuality1V1(goldPlayer, blackPlayer);
            _calculator.CalculateNewRatings1V1(goldPlayer, blackPlayer);

            // Assert
            AssertRating(29.39583201999924, 7.171475587326186, goldPlayer.Skill1V1);
            AssertRating(20.60416798000076, 7.171475587326186, blackPlayer.Skill1V1);
            AssertMatchQuality(0.447, matchQuality);
        }

        [Fact]
        public void TwoOnTwoSimpleTest()
        {
            // Arrange
            var goUser = TestUsers.TestUser_1;
            var gdUser = TestUsers.TestUser_2;
            var boUser = TestUsers.TestUser_3_Anon;
            var bdUser = TestUsers.TestUser_4;

            // Act
            _calculator.CalculateNewRatings2V2(goUser, gdUser, boUser, bdUser);

            // Assert
            AssertRating(28.108, 7.774, goUser.Skill2V2);
            AssertRating(28.108, 7.774, gdUser.Skill2V2);
            AssertRating(21.892, 7.774, boUser.Skill2V2);
            AssertRating(21.892, 7.774, bdUser.Skill2V2);

            AssertMatchQuality(0.447, _calculator.CalculateMatchQuality2V2(goUser, gdUser, boUser, bdUser));
        }

        [Fact]
        public void TwoOnTwoUpsetTest()
        {
            // Arrange
            var goUser = TestUsers.TestUser_1;
            var gdUser = TestUsers.TestUser_2;
            var boUser = TestUsers.TestUser_3_Anon;
            var bdUser = TestUsers.TestUser_4;

            goUser.Skill2V2 = new Trueskill(20, 8);
            gdUser.Skill2V2 = new Trueskill(25, 6);
            boUser.Skill2V2 = new Trueskill(35, 7);
            bdUser.Skill2V2 = new Trueskill(40, 5);

            // Act
            var matchQuality = _calculator.CalculateMatchQuality2V2(goUser, gdUser, boUser, bdUser);
            _calculator.CalculateNewRatings2V2(goUser, gdUser, boUser, bdUser);

            // Assert
            AssertRating(29.455, 7.008, goUser.Skill2V2);
            AssertRating(30.455, 5.594, gdUser.Skill2V2);
            AssertRating(27.575, 6.346, boUser.Skill2V2);
            AssertRating(36.211, 4.768, bdUser.Skill2V2);
            AssertMatchQuality(0.084, matchQuality);
        }

        #region Helpers

        private static void AssertRating(double expectedMean, double expectedStdDev, Trueskill actual)
        {
            Assert.Equal(expectedMean, actual.Mean, precision: 0);
            Assert.Equal(expectedStdDev, actual.StdDev, precision: 1);
        }

        private static void AssertMatchQuality(double expectedQuality, double actualQuality)
        {
            Assert.Equal(expectedQuality, actualQuality, precision: 1);
        }

        #endregion
    }
}
