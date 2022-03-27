using System;
using System.Threading.Tasks;

namespace YouFoos.StatisticsService.Services
{
    public interface IAchievementUnlockService
    {
        /// <summary>
        /// Given a game, triggers a progress update on all achievments for all players in the game.
        /// </summary>
        /// <param name="gameId">The ID of the game to check achievements for.</param>
        Task UpdateAchievementStatusesPostGame(Guid gameId);
    }
}
