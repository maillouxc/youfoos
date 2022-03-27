using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.Api.Services
{
    /// <summary>
    /// Concrete implementation of <see cref="IGamesService"/>.
    /// </summary>
    public class GamesService : IGamesService
    {
        private readonly IGamesRepository _gamesRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GamesService(IGamesRepository gamesRepository)
        {
            _gamesRepository = gamesRepository;
        }

        /// <summary>
        /// Concrete implementation of <see cref="IGamesService.GetCurrentGame"/>.
        /// </summary>
        public async Task<Game> GetCurrentGame()
        {
            return await _gamesRepository.GetCurrentGame();
        }

        /// <summary>
        /// Concrete implementation of <see cref="IGamesService.GetGameById(Guid)"/>.
        /// </summary>
        public async Task<Game> GetGameById(Guid id)
        {
            return await _gamesRepository.GetGameById(id);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IGamesService.GetRecentGames(int)"/>.
        /// </summary>
        public async Task<List<Game>> GetRecentGames(int numGames)
        {
            return await _gamesRepository.GetListOfRecentGames(numGames);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IGamesService.GetRecentGamesByUserId(string, int)"/>.
        /// </summary>
        public async Task<List<Game>> GetRecentGamesByUserId(string userId, int numGames)
        {
            return await _gamesRepository.GetListOfRecentGamesByUserId(userId, DateTime.UtcNow, numGames);
        }
    }
}
