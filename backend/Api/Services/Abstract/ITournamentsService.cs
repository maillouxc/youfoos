using System;
using System.Threading.Tasks;
using YouFoos.Api.Dtos;
using YouFoos.DataAccess.Entities;

namespace YouFoos.Api.Services
{
    /// <summary>
    /// Business logic class for working with tournaments from the API.
    /// </summary>
    public interface ITournamentsService
    {
        /// <summary>
        /// Returns the tournament that is currently in progress, or null if there isn't one.
        /// </summary>
        Task<Tournament> GetCurrentTournament();

        /// <summary>
        /// Returns the tournament with the given ID, or null if not found.
        /// </summary>
        Task<Tournament> GetTournamentById(Guid id);

        /// <summary>
        /// Returns the most recently played tournaments, paginated.
        /// </summary>
        Task<PaginatedResult<Tournament>> GetRecentTournaments(int pageSize, int pageNumber);

        Task<Tournament> CreateTournament(CreateTournamentRequest request, string creatorId);

        Task RegisterForTournament(Guid tournamentId, string userId);

        Task UpdateTournament();
    }
}
