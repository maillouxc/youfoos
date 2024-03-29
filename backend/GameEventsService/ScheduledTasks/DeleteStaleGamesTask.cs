﻿using FluentScheduler;
using Serilog;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.GameEventsService.ScheduledTasks
{
    public class DeleteStaleGamesTask : IJob
    {
        public const int StaleTimeMinutes = 30;

        private readonly IGamesRepository _gamesRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DeleteStaleGamesTask(IGamesRepository gamesRepository)
        {
            _gamesRepository = gamesRepository;
        }

        /// <summary>
        /// Deletes games in-progress that have gone on for beyond StaleTimeMinutes minutes.
        ///
        /// This situation can potentially occur if power or internet connection is lost
        /// during the course of the game, causing the table to never send a game-end message.
        /// If we don't delete these games from the database every now and then, we will have
        /// tons of "in-progress" games that just build up.
        ///
        /// Because we only support single tables at the moment, this will at most only delete
        /// the current game in progress if it is stale. If we later decide to add multi-table
        /// support, we will need to change this to check all games marked as in-progress.
        /// </summary>
        public async void Execute()
        {
            Log.Logger.Information("Executing scheduled task to delete stale games");

            var gameInProgress = await _gamesRepository.GetCurrentGame();

            if (gameInProgress == null)
            {
                Log.Logger.Information("No stale games found to delete");
                return;
            }

            if ((gameInProgress.GetDurationInSeconds() / 60) > StaleTimeMinutes)
            {
                Log.Logger.Debug("Deleting stale game {@Game}", gameInProgress);
                await _gamesRepository.DeleteGameByIdAsync(gameInProgress.Guid);
            }

            Log.Logger.Information("Done deleting stale games");
        }
    }
}
