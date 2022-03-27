using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Moq;
using Xunit;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Repositories;
using YouFoos.SharedLibrary.Exceptions;
using YouFoos.GameEventsService.Services;

namespace YouFoos.GameEventsService.Tests.Unit.Services
{
    [ExcludeFromCodeCoverage]
    public class GameplayMessageHandlerTests
    {
        private const string TestGameStartMessage = "{\"type\":\"gameStart\",\"game_guid\":\"9801fada-ae95-43b0-8fe8-52b1374da1be\",\"game_type\":\"2v2\",\"gold_offense_rfid\":\"17 3A D3 40 90 00\",\"gold_defense_rfid\":\"27 3A D3 40 90 00\",\"black_offense_rfid\":\"37 3A D3 40 90 00\",\"black_defense_rfid\":\"47 3A D3 40 90 00\",\"timestamp\":\"2019-04-23T20:22:16.1654867-04:00\"}";
        private const string TestGoalScoredMessage = "{\"type\":\"goalScored\",\"game_guid\":\"9801fada-ae95-43b0-8fe8-52b1374da1be\",\"scoring_player_rfid\":\"17 3A D3 40 90 00\",\"relative_timestamp\":720,\"team_scored_against\":1,\"timestamp\":\"2019-04-23T20:22:16.1654867-04:00\"}";
        private const string TestGoalUndoMessage = "{\"type\":\"goalUndo\",\"game_guid\":\"9801fada-ae95-43b0-8fe8-52b1374da1be\",\"timestamp\":\"2019-04-23T20:22:16.1654867-04:00\"}";
        private const string TestGameEndMessage = "{\"type\":\"gameEnd\",\"game_guid\":\"9801fada-ae95-43b0-8fe8-52b1374da1be\",\"final_gold_score\":5,\"final_black_score\":2,\"final_duration_millis\":42069,\"timestamp\":\"2019-04-23T20:22:16.1654867-04:00\"}";

        private readonly GameplayMessageHandler _gameplayMessageHandler;
        private readonly Mock<IGamesRepository> _mockGamesRepository;
        
        public GameplayMessageHandlerTests()
        {
            var mockRfidService = new Mock<IRfidToUserAccountResolverService>();
            _mockGamesRepository = new Mock<IGamesRepository>();
            
            _mockGamesRepository.Setup(o => o.InsertGoal(It.IsAny<Goal>())).Verifiable();
            _mockGamesRepository.Setup(o => o.UndoGoal(It.IsAny<Guid>(), It.IsAny<DateTime>())).Verifiable();
            _mockGamesRepository.Setup(o => o.EndGame(It.IsAny<Guid>(), It.IsAny<DateTime>())).Verifiable();

            // TODO finish
            //_gameplayMessageHandler = new GameplayMessageHandler(mockRfidService.Object, _mockGamesRepository.Object);
            throw new System.NotImplementedException("Not finished yet");
        }
        
        [Theory]
        [InlineData(TestGameStartMessage, "gameStart")]
        [InlineData(TestGoalScoredMessage, "goalScored")]
        [InlineData(TestGoalUndoMessage, "goalUndo")]
        [InlineData(TestGameEndMessage, "gameEnd")]
        public void GivenJsonMessage_Service_ShouldReturnCorrectMessageType(
            string message, string expectedMessageType)
        {
            var messageType = GameplayMessageHandler.GetMessageType(message);
            
            Assert.Equal(expectedMessageType, messageType);
        }
        
        [Theory]
        [InlineData(TestGameEndMessage)]
        [InlineData(TestGoalUndoMessage)]
        [InlineData(TestGoalScoredMessage)]
        public async Task GivenGameNotInDatabase_HandleMessageAsync_WillIgnoreMessagesBesidesGameStart(string message)
        {
            // Act and assert
            await Assert.ThrowsAsync<YouFoosException>(() => _gameplayMessageHandler.HandleMessageAsync(message));

            // Assert
            _mockGamesRepository.Verify(o => o.InsertGoal(It.IsAny<Goal>()), Times.Never());
            _mockGamesRepository.Verify(o => o.UndoGoal(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Never());
            _mockGamesRepository.Verify(o => o.EndGame(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Never());
        }

        [Fact]
        public async Task GivenInvalidMessage_HandleMessageAsync_WillIgnoreMessage()
        {
            // Arrange
            var invalidType = "{\"type\": \"invalid\"}";
            var messageWithoutType = "{\"message\": \"This message has no type.\"}";
            
            // Act and assert
            await Assert.ThrowsAsync<InvalidMessageTypeException>(
                () => _gameplayMessageHandler.HandleMessageAsync(invalidType));
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _gameplayMessageHandler.HandleMessageAsync(messageWithoutType));
        }
    }
}
