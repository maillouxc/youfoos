using MongoDB.Driver;
using System;
using System.Threading.Tasks;
using YouFoos.Api.Dtos;
using YouFoos.DataAccess.Entities;

namespace YouFoos.DataAccess.Repositories
{
    /// <summary>
    /// Concrete implementation of <see cref="ITournamentsRepository"/>.
    /// </summary>
    public class TournamentsRepository : ITournamentsRepository
    {
        private const string CollectionName = "tournaments";

        private readonly IMongoCollection<Tournament> _tournaments;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TournamentsRepository(IMongoContext mongoContext)
        {
            _tournaments = mongoContext.GetCollection<Tournament>(CollectionName);
        }

        /// <summary>
        /// Concrete implementation of <see cref="ITournamentsRepository.GetCurrentTournament"/>.
        /// </summary>
        public async Task<Tournament> GetCurrentTournament()
        {
            var isInProgressFilter = Builders<Tournament>.Filter
                .Where(tournament => tournament.CurrentState == Entities.Tournaments.TournamentState.InProgress);

            var isUpcomingFilter = Builders<Tournament>.Filter
                .Where(tournament => tournament.StartDate > DateTime.UtcNow);

            return await _tournaments.Find(isUpcomingFilter | isInProgressFilter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Concrete implementation of <see cref="ITournamentsRepository.GetTournamentById(Guid?)"/>.
        /// </summary>
        public Task<Tournament> GetTournamentById(Guid? id)
        {
            return  _tournaments.Find(tournament => tournament.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Concrete implementation of <see cref="ITournamentsRepository.GetRecentTournaments(int, int)"/>.
        /// </summary>
        public async Task<PaginatedResult<Tournament>> GetRecentTournaments(int pageSize, int pageNumber)
        {
            long totalCount = await _tournaments.CountDocumentsAsync(tournament => tournament.EndDate != null);

            var tournaments = await _tournaments
                .Find(tournament => tournament.EndDate != null)
                .SortByDescending(tournament => tournament.EndDate)
                .Skip(pageSize * pageNumber)
                .Limit(pageSize)
                .ToListAsync();

            return new PaginatedResult<Tournament>(pageNumber, (int)totalCount, pageSize, tournaments);
        }

        /// <summary>
        /// Concrete implementation of <see cref="ITournamentsRepository.InsertOne(Tournament)"/>.
        /// </summary>
        public Task InsertOne(Tournament tournament)
        {
            return _tournaments.InsertOneAsync(tournament);
        }

        /// <summary>
        /// Concrete implementation of <see cref="ITournamentsRepository.UpdateTournament(Tournament)"/>.
        /// </summary>
        public Task UpdateTournament(Tournament tournament)
        {
            var idFilter = Builders<Tournament>.Filter.Eq("Id", tournament.Id);
            return _tournaments.ReplaceOneAsync(idFilter, tournament);
        }
    }
}
