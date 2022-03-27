using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities;

namespace YouFoos.Api.Services
{
    /// <summary>
    /// Business logic class for interacting with games.
    /// </summary>
    public interface IGamesService
    {
        /// <summary>
        /// Returns the game that is currently in progress, or null if there isn't one.
        /// </summary>
        Task<Game> GetCurrentGame();

        /// <summary>
        /// Returns the game with the given ID, or null if not found.
        /// </summary>
        Task<Game> GetGameById(Guid id);

        /// <summary>
        /// Returns the list of the n most recently played games.
        /// </summary>
        Task<List<Game>> GetRecentGames(int numGames);

        /// <summary>
        /// Returns the list of the n most recently played games for the given user.
        /// </summary>
        Task<List<Game>> GetRecentGamesByUserId(string userId, int numGames);
    }
}
