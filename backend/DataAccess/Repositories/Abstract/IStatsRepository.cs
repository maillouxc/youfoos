using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities.Enums;
using YouFoos.DataAccess.Entities.Stats;

namespace YouFoos.DataAccess.Repositories
{
    /// <summary>
    /// Repository class for managing user stats data.
    /// </summary>
    public interface IStatsRepository
    {
        /// <summary>
        /// Inserts a new stats object into the database.
        /// </summary>
        Task InsertOne(UserStats stats);

        /// <summary>
        /// Counts the number of users with more than n games played in the given game mode.
        /// </summary>
        Task<int> CountUsersAboveNGamesInMode(int n, StatsCategory statsCategory);

        /// <summary>
        /// Returns the most recent stats object for a user with the given ID, or none if no user stats for the given
        /// user ID were found. Can also be supplied a cutoff date - if given, stats objects recorded after the cutoff
        /// will not be considered.
        /// </summary>
        Task<UserStats> GetStatsForUserWithId(string userId, DateTime? cutoffDate = null);

        /// <summary>
        /// Returns the history of a stat's value over time with the given parameters.
        /// </summary>
        /// <returns></returns>
        Task<List<StatHistoryDataPoint>> GetStatHistoryForUser(string userId, 
                                                               string statName, StatsCategory statCategory, 
                                                               DateTime from, DateTime to, 
                                                               StatSampleInterval sampleInterval);

        /// <summary>
        /// Returns the stats needed to construct a single page of the leaderboard.
        /// </summary>
        Task<List<UserStats>> GetStatsForLeaderboard(string sortBy, StatsCategory statCategory,
                                                     int pageSize, int pageNumber,
                                                     int minGamesToAppear = 10);

        /// <summary>
        /// Returns the maximum or minimum value of the given stat across the entire system.
        /// </summary>
        Task<UserStatValue<T>> GetMaxOrMinStatValueForAccolades<T>(StatsCategory statsCategory, 
                                                                   string statName, 
                                                                   bool min = false);

        /// <summary>
        /// Returns the maximum or minimum value of the per-game average given stat across the entire system.
        /// </summary>
        Task<UserStatValue<T>> GetMaxOrMinStatValuePerGameAvgForAccolades<T>(StatsCategory statsCategory,
                                                                             string statName,
                                                                             bool min = false);

        /// <summary>
        /// Returns the highest or lowest (based on argument value) value of winrate across the entire system.
        /// </summary>
        Task<UserStatValue<double>> CalcMaxOrMinWinrate(bool min = false);
    }
}
