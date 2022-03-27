using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouFoos.Api.Dtos;
using YouFoos.DataAccess.Entities.Enums;
using YouFoos.DataAccess.Entities.Stats;

namespace YouFoos.Api.Services
{
    /// <summary>
    /// Business logic class for the retrieval of user game statistics.
    /// 
    /// This class is not used to calculate any new stats but is rather focused mainly on data retrieval.
    /// </summary>
    public interface IStatsService
    {
        /// <summary>
        /// Returns the UserStats object for the user with the given ID, that were captured before the given cutoff date.
        /// </summary>
        Task<UserStats> GetStatsForUserWithId(string id, DateTime? cutoffDate = null);

        /// <summary>
        /// Returns the history of a user's stats based on the provided parameters.
        /// </summary>
        Task<List<StatHistoryDataPoint>> GetStatHistoryForUser(
            string userId, string statName, StatsCategory statsCategory, 
            DateTime startDate, DateTime endDate, StatSampleInterval sampleInterval);

        /// <summary>
        /// Returns the leaderboard page data for the given parameters.
        /// </summary>
        Task<PaginatedResult<UserStats>> GetLeaderboardPage(
            string sortBy, StatsCategory statCategory, int pageSize = 10, int pageNumber = 0, string userId = null);
    }
}
