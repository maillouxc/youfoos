using System;
using System.Diagnostics.CodeAnalysis;
using Moq;
using Xunit;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Repositories;
using YouFoos.GameEventsService.ScheduledTasks;

namespace YouFoos.GameEventsService.Tests.Unit.ScheduledTasks
{
    [ExcludeFromCodeCoverage]
    public class DeleteStaleGamesTaskTests
    {
        [Fact]
        public void GivenStaleGames_Execute_ShouldDeleteThem()
        {
            // Arrange
            var staleTestGameGuid = Guid.NewGuid();
            var staleTestGameStartTime = DateTime.UtcNow.AddMinutes(-(DeleteStaleGamesTask.StaleTimeMinutes + 2));
            var staleTestGame = new Game(id: staleTestGameGuid, isInProgress: true)
            {
                StartTimeUtc = staleTestGameStartTime
            };

            var mockGamesRepository = new Mock<IGamesRepository>();
            mockGamesRepository.Setup(o => o.GetCurrentGame())
                               .ReturnsAsync(staleTestGame);

            var deleteStaleGamesTask = new DeleteStaleGamesTask(mockGamesRepository.Object);

            // Act
            deleteStaleGamesTask.Execute();

            // Assert
            mockGamesRepository.Verify(repo => repo.DeleteGameByIdAsync(
                It.Is<Guid>(fn => fn.Equals(staleTestGameGuid)))
            );        
        }

        [Fact]
        public void GivenNonStaleGames_Execute_ShouldNotDeleteThem()
        {
            // Arrange
            var testGameGuid = Guid.NewGuid();
            var testGameStartTime = DateTime.UtcNow.AddMinutes(-(DeleteStaleGamesTask.StaleTimeMinutes - 1));
            var testGame = new Game(testGameGuid)
            {
                StartTimeUtc = testGameStartTime
            };

            var mockGamesRepository = new Mock<IGamesRepository>();
            mockGamesRepository.Setup(o => o.GetCurrentGame())
                               .ReturnsAsync(testGame);

            var deleteStaleGamesTask = new DeleteStaleGamesTask(mockGamesRepository.Object);

            // Act
            deleteStaleGamesTask.Execute();

            // Assert
            mockGamesRepository.Verify(repo => repo.DeleteGameByIdAsync(It.IsAny<Guid>()), Times.Never());
        }

        [Fact]
        public void GivenNoGameInProgress_Execute_ShouldNotDeleteAnything()
        {
            // Arrange
            var mockGamesRepository = new Mock<IGamesRepository>();
            var deleteStaleGamesTask = new DeleteStaleGamesTask(mockGamesRepository.Object);

            // Act
            deleteStaleGamesTask.Execute();

            // Assert
            mockGamesRepository.Verify(repo => repo.DeleteGameByIdAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
}
