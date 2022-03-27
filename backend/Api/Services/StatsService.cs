using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouFoos.Api.Dtos;
using YouFoos.DataAccess.Entities.Enums;
using YouFoos.DataAccess.Entities.Stats;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.Api.Services
{
    /// <summary>
    /// Concrete implementation of <see cref="IStatsService"/>.
    /// </summary>
    public class StatsService : IStatsService
    {
        private readonly IStatsRepository _statsRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        public StatsService(IStatsRepository statsRepository)
        {
            _statsRepository = statsRepository;
        }
        
        /// <summary>
        /// Concrete implementation of <see cref="IStatsService.GetStatsForUserWithId(string, DateTime?)"/>.
        /// </summary>
        public async Task<UserStats> GetStatsForUserWithId(string id, DateTime? cutoffDate = null)
        {
            return await _statsRepository.GetStatsForUserWithId(id, cutoffDate);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IStatsService.GetStatHistoryForUser(string, string, StatsCategory, DateTime, DateTime, StatSampleInterval)"/>.
        /// </summary>
        public async Task<List<StatHistoryDataPoint>> GetStatHistoryForUser(
            string userId, 
            string statName, StatsCategory statsCategory, 
            DateTime startDate, DateTime endDate, 
            StatSampleInterval sampleInterval)
        {
            return await _statsRepository.GetStatHistoryForUser(
                userId, statName, statsCategory, startDate, endDate, sampleInterval);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IStatsService.GetLeaderboardPage(string, StatsCategory, int, int, string)"/>.
        /// </summary>
        public async Task<PaginatedResult<UserStats>> GetLeaderboardPage(
            string sortBy, StatsCategory statCategory, 
            int pageSize = 10, int pageNumber = 0, 
            string userId = null)
        {
            int totalCount = await _statsRepository.CountUsersAboveNGamesInMode(10, statCategory);

            // If the user is searching for a particular user...
            if (userId != null)
            {
                // TODO this is a crappy solution, but it works. It's just awful for performance. Fix it later.

                // Check each page of the leaderboard for the user
                int p = 0;
                while (true)
                {
                    // If there are no more pages and we still haven't found the user, return null
                    var pageContents = await _statsRepository.GetStatsForLeaderboard(sortBy, statCategory, pageSize, p);
                    if (pageContents.Count == 0)
                    {
                        return null;
                    }

                    // If we find the user on this page, return the page
                    var foundUser = pageContents.Find(user => user.Id == userId) != null;
                    if (foundUser)
                    {
                        return new PaginatedResult<UserStats>(p, totalCount, pageSize, pageContents);
                    }
                    
                    p++;
                }
            }

            // If this isn't a search for a particular user, just return the requested leaderboard page
            var leaderboardStats = await _statsRepository.GetStatsForLeaderboard(sortBy, statCategory, pageSize, pageNumber);

            return leaderboardStats == null ? null : new PaginatedResult<UserStats>(pageNumber, totalCount, pageSize, leaderboardStats);
        }
    }
}
