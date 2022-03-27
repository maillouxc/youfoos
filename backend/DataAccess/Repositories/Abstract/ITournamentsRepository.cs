using System;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities;
using YouFoos.Api.Dtos;

namespace YouFoos.DataAccess.Repositories
{
    /// <summary>
    /// Data access class for working with tournament data.
    /// </summary>
    public interface ITournamentsRepository
    {
        /// <summary>
        /// Returns the tournament that is currently progress, or null if there isn't one.
        /// </summary>
        Task<Tournament> GetCurrentTournament();

        /// <summary>
        /// Returns the tournament with the given ID, or null if not found.
        /// </summary>
        Task<Tournament> GetTournamentById(Guid? id);

        /// <summary>
        /// Returns the most recently played tournaments, paginated.
        /// </summary>
        Task<PaginatedResult<Tournament>> GetRecentTournaments(int pageSize, int pageNumber);

        /// <summary>
        /// Inserts a new tournament into the database.
        /// </summary>
        Task InsertOne(Tournament tournament);

        /// <summary>
        /// Updates the given tournament in the database.
        /// </summary>
        Task UpdateTournament(Tournament tournament);
    }
}
