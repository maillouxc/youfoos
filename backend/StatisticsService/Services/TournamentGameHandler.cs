using System;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.StatisticsService.Services
{
    /// <summary>
    /// Concrete implementation of <see cref="ITournamentGameHandler"/>.
    /// </summary>
    public class TournamentGameHandler : ITournamentGameHandler
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly ITournamentsRepository _tournamentsRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TournamentGameHandler(ITournamentsRepository tournamentsRepository)
        {
            _tournamentsRepository = tournamentsRepository;
        }

        /// <summary>
        /// Concrete implementation of <see cref="ITournamentGameHandler.HandleGameIfIsTournamentGame"/>.
        /// </summary>
        public async Task HandleGameIfIsTournamentGame(Guid gameGuid)
        {
            Game game = await _gamesRepository.GetGameById(gameGuid);

            if (game.TournamentId != null)
            {
                Tournament tournament = await _tournamentsRepository.GetTournamentById(game.TournamentId);
                tournament.UpdateBracket(game);
                await _tournamentsRepository.UpdateTournament(tournament);
            }
        }
    }
}
