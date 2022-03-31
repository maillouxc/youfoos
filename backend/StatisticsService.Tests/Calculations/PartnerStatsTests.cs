using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using YouFoos.TestData;
using YouFoos.StatisticsService.StatCalculations;

namespace YouFoos.StatisticsService.Tests.Unit.Calculations
{
    [ExcludeFromCodeCoverage]
    public class PartnerStatsTests
    {
        [Fact]
        public void GivenGames_CalculateMostFrequentTeamMate_ShouldWorkCorrectly()
        {
            PartnerStats.CalculateMostFrequentTeamMate(TestUsers.TestUser_1.Id, 
                TestGames.GetAllTestGames()
                         .FindAll(o => o.WasPlayerInGame(TestUsers.TestUser_1.Id)), out var teamMemberId, out var gamesTogether);

            Assert.Equal(2, gamesTogether);
            Assert.Equal(TestUsers.TestUser_3_Anon.Id, teamMemberId);
        }

        [Fact]
        public void GivenEmptyGames_CalculateMostFrequentTeammate_ShouldReturnNullTeamMate()
        {
            // Check with empty list
            PartnerStats.CalculateMostFrequentTeamMate(TestUsers.TestUser_1.Id, 
                TestGames.GetAllTestGames().FindAll(o => o.Guid == Guid.Empty), out var teamMemberId, out var gamesTogether);
            Assert.Equal(0, gamesTogether);
            Assert.Null(teamMemberId);
            
            // Check with null 
            PartnerStats.CalculateMostFrequentTeamMate(TestUsers.TestUser_1.Id, null, 
                out var nullListTeamMemberId, out var nullListGamesTogether);
            Assert.Equal(0, nullListGamesTogether);
            Assert.Null(nullListTeamMemberId);
        }

        [Fact]
        public void GivenEmptyGames_CalculateBestTeamMate_ShouldReturnNullTeamMate()
        {
            // Check with empty
            PartnerStats.CalculateBestTeamMate(TestUsers.TestUser_1.Id, 
                TestGames.GetAllTestGames().FindAll(o => o.Guid == Guid.Empty), out var teamMemberId, out var winRate);
            Assert.Equal(0, winRate);
            Assert.Null(teamMemberId);
            
            // Check with null
            PartnerStats.CalculateBestTeamMate(TestUsers.TestUser_1.Id, null, out var nullListTeamMemberId, out var nullListWinRate);
            Assert.Equal(0, nullListWinRate);
            Assert.Null(nullListTeamMemberId);
        }

        [Fact]
        public void GivenEmptyGames_CalculateWorstTeamMate_ShouldReturnNullTeamMate()
        {
            // Check with empty
            PartnerStats.CalculateWorstTeamMate(TestUsers.TestUser_1.Id, 
                TestGames.GetAllTestGames().FindAll(o => o.Guid == Guid.Empty), out var teamMemberId, out var winRate);
            Assert.Equal(0, winRate);
            Assert.Null(teamMemberId);
            
            // Check with null
            PartnerStats.CalculateWorstTeamMate(TestUsers.TestUser_1.Id, null, out var nullListTeamMemberId, out var nullListWinRate);
            Assert.Equal(0, nullListWinRate);
            Assert.Null(nullListTeamMemberId);
        }
    }
}
