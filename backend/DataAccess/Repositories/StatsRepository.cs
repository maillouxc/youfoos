using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using YouFoos.DataAccess.Entities.Enums;
using YouFoos.DataAccess.Entities.Stats;

namespace YouFoos.DataAccess.Repositories
{
    /// <summary>
    /// MongoDB implementation of <see cref="IStatsRepository"/>.
    /// </summary>
    public class StatsRepository : IStatsRepository
    {
        private const string CollectionName = "stats";
        
        private readonly IMongoCollection<UserStats> _stats;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public StatsRepository(IMongoContext mongoContext)
        {
            _stats = mongoContext.GetCollection<UserStats>(CollectionName);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IStatsRepository.InsertOne(UserStats)"/>.
        /// </summary>
        public Task InsertOne(UserStats stats)
        {
            return _stats.InsertOneAsync(stats);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IStatsRepository.CountUsersAboveNGamesInMode(int, StatsCategory)"/>.
        /// </summary>
        public async Task<int> CountUsersAboveNGamesInMode(int n, StatsCategory statsCategory)
        {
            PipelineDefinition<UserStats, BsonDocument> aggregationPipeline = new[]
            {
                AddGamesPlayedInModeField(statsCategory),
                FilterOutStatsWithLessThanNGamesPlayedInMode(n),
               
                // Group by user ID
                new BsonDocument("$group", new BsonDocument().Add("_id", "$UserId")),

                // Count the results
                new BsonDocument("$count", "n"), 
            };

            // And then execute the pipeline and return the results
            var resultsCursor = await _stats.AggregateAsync(aggregationPipeline);
            return (int) resultsCursor.First()["n"];
        }

        /// <summary>
        /// Concrete implementation of <see cref="IStatsRepository.GetStatsForUserWithId(string, DateTime?)"/>.
        /// </summary>
        public Task<UserStats> GetStatsForUserWithId(string userId, DateTime? cutoffDate = null)
        {
            var idFilter = Builders<UserStats>.Filter.Eq(s => s.UserId, userId);
            var cutoffDateFilter = Builders<UserStats>.Filter.Lt(s => s.Timestamp, cutoffDate ?? DateTime.MaxValue);

            return _stats
                .Find(idFilter & cutoffDateFilter)
                .SortByDescending(stats => stats.Timestamp)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Concrete implementation of <see cref="IStatsRepository.GetStatHistoryForUser(string, string, StatsCategory, DateTime, DateTime, StatSampleInterval)"/>.
        /// </summary>
        public async Task<List<StatHistoryDataPoint>> GetStatHistoryForUser(string userId, 
                                                                            string statName, StatsCategory statCategory,
                                                                            DateTime from, DateTime to, 
                                                                            StatSampleInterval sampleInterval)
        {
            var matchByUserIdAndDateRange = new BsonDocument("$match", new BsonDocument()
                .Add("UserId", userId)
                .Add("Timestamp", new BsonDocument()
                    .Add("$gt", new BsonDateTime(from))
                    .Add("$lt", new BsonDateTime(to))
                ));

            // MongoDB will optimize and not actually project fields we don't use later in the pipeline
            var projectNeededFieldsForGrouping = new BsonDocument("$project", new BsonDocument()
                .Add("Year", new BsonDocument().Add("$year", "$Timestamp"))
                .Add("Month", new BsonDocument().Add("$month", "$Timestamp"))
                .Add("Day", new BsonDocument().Add("$dayOfMonth", "$Timestamp"))
                .Add("Week", new BsonDocument().Add("$week", "$Timestamp"))
                .Add("Timestamp", "$Timestamp")
                .Add("Value", $"{statCategory}.{statName}"));

            // Get only the most recent value from each time group - for instance the last update from each month.
            // We also have to group by higher values like year, e.g. to separate Month 12, 2019 from Month 12, 2018.
            // We could DRY this up some using fallthrough but honestly this is easier to read and not much longer.
            var timeGroupings = new BsonDocument();
            switch (sampleInterval)
            {
                case StatSampleInterval.Daily:
                    timeGroupings.Add("Day", "$Day");
                    timeGroupings.Add("Month", "$Month");
                    timeGroupings.Add("Year", "$Year");
                    break;
                case StatSampleInterval.Monthly:
                    timeGroupings.Add("Month", "$Month");
                    timeGroupings.Add("Year", "$Year");
                    break;
                case StatSampleInterval.Yearly:
                    timeGroupings.Add("Year", "$Year");
                    break;
                case StatSampleInterval.Weekly:           
                    timeGroupings.Add("Week", "$Week");
                    timeGroupings.Add("Year", "$Year");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sampleInterval), sampleInterval, "Invalid sample interval");
            }

            var groupByNeededTimeFields = new BsonDocument("$group", new BsonDocument()
                .Add("_id", timeGroupings)
                .Add("Timestamp", new BsonDocument().Add("$last", "$Timestamp"))
                .Add("Value", new BsonDocument().Add("$last", "$Value")));

            var projectFinalResult = new BsonDocument("$project", new BsonDocument()
                .Add("_id", "$null") // We don't need the ID in the final result
                .Add("Timestamp", "$Timestamp")
                .Add("Value", "$Value"));

            // Build the aggregation pipeline from the stages we just created
            PipelineDefinition<UserStats, StatHistoryDataPoint> aggregationPipeline = new[]
            {
                matchByUserIdAndDateRange,
                SortByTimestamp(SortDirection.Ascending),
                projectNeededFieldsForGrouping,
                groupByNeededTimeFields,
                projectFinalResult
            };

            // Execute the pipeline and return the result
            var resultsCursor = await _stats.AggregateAsync(aggregationPipeline);
            return await resultsCursor.ToListAsync();
        }

        /// <summary>
        /// Concrete implementation of <see cref="IStatsRepository.GetStatsForLeaderboard(string, StatsCategory, int, int, int)"/>.
        /// </summary>
        public async Task<List<UserStats>> GetStatsForLeaderboard(string sortBy, StatsCategory statsCategory,
                                                                  int pageSize, int pageNumber,
                                                                  int minGamesToAppear = 10)
        {
            if (sortBy == "Rank") sortBy = "Skill.ConservativeRating";

            PipelineDefinition<UserStats, UserStats> aggregationPipeline = new[]
            {
                AddGamesPlayedInModeField(statsCategory),
                FilterOutStatsWithLessThanNGamesPlayedInMode(10),
                SortByTimestamp(SortDirection.Ascending),

                // Group by user ID
                new BsonDocument("$group", new BsonDocument()
                    .Add("_id", "$UserId")
                    .Add("UserId", new BsonDocument().Add("$last", "$UserId"))
                    .Add("StatsOverall", new BsonDocument().Add("$last", "$StatsOverall"))
                    .Add("Stats1V1", new BsonDocument().Add("$last", "$Stats1V1"))
                    .Add("Stats2V2", new BsonDocument().Add("$last", "$Stats2V2"))
                    .Add("Timestamp", new BsonDocument().Add("$last", "$Timestamp"))),

                // Sort by chosen stat
                new BsonDocument("$sort", new BsonDocument().Add($"{statsCategory}.{sortBy}", -1)),

                // Page the results
                new BsonDocument("$skip", pageSize * pageNumber),
                new BsonDocument("$limit", pageSize)
            };

            // And then execute the pipeline and return the results
            var resultsCursor = await _stats.AggregateAsync(aggregationPipeline);
            return await resultsCursor.ToListAsync();
        }

        /// <summary>
        /// Concrete implementation of <see cref="IStatsRepository.GetMaxOrMinStatValueForAccolades{T}(StatsCategory, string, bool)"/>.
        /// </summary>
        public async Task<UserStatValue<T>> GetMaxOrMinStatValueForAccolades<T>(
            StatsCategory statsCategory, string statName, bool min)
        {
            if (statName == "Rank") statName = "Skill.ConservativeRating";

            var sortDirection = min ? 1 : -1;

            PipelineDefinition<UserStats, UserStatValue<T>> aggregationPipeline = new[]
            {
                // Ensure desired stat is not null
                new BsonDocument("$match", new BsonDocument($"{statsCategory}.{statName}",
                    new BsonDocument("$exists", true))),

                AddGamesPlayedInModeField(statsCategory),
                FilterOutStatsWithLessThanNGamesPlayedInMode(10),
                SortByTimestamp(SortDirection.Ascending),

                // Group by user id and project only desired stat
                new BsonDocument("$group", new BsonDocument()
                    .Add("_id", "$UserId")
                    .Add("UserId", new BsonDocument("$last", "$UserId"))
                    .Add("StatValue", new BsonDocument("$last", $"${statsCategory}.{statName}"))),

                // Grab the max or min stat value
                new BsonDocument("$sort", new BsonDocument().Add("StatValue", sortDirection)),
                new BsonDocument("$limit", 1)
            };

            var resultsCursor = await _stats.AggregateAsync(aggregationPipeline);
            return await resultsCursor.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Concrete implementation of <see cref="IStatsRepository.GetMaxOrMinStatValuePerGameAvgForAccolades{T}(StatsCategory, string, bool)"/>.
        /// </summary>
        public async Task<UserStatValue<T>> GetMaxOrMinStatValuePerGameAvgForAccolades<T>(
            StatsCategory statsCategory, string statName, bool min)
        {
            var sortDirection = min ? 1 : -1;

            PipelineDefinition<UserStats, UserStatValue<T>> aggregationPipeline = new[]
            {
                AddGamesPlayedInModeField(statsCategory),
                FilterOutStatsWithLessThanNGamesPlayedInMode(10),
                SortByTimestamp(SortDirection.Ascending),

                // Group by user id and project only the needed data
                new BsonDocument("$group", new BsonDocument()
                    .Add("_id", "$UserId")
                    .Add("UserId", new BsonDocument("$last", "$UserId"))
                    .Add("StatValue", new BsonDocument("$last", $"${statsCategory}.{statName}"))
                    .Add("GamesPlayedInMode", new BsonDocument("$last", "$GamesPlayedInMode"))),

                // Project a calculated value for the desired per game average stat, factoring in game mode
                new BsonDocument("$project", new BsonDocument()
                    .Add("_id", "0")
                    .Add("UserId", "$UserId")
                    .Add("StatValue", new BsonDocument("$divide", 
                        new BsonArray().Add("$StatValue").Add("$GamesPlayedInMode")))),
                
                // Grab the max or min stat value
                new BsonDocument("$sort", new BsonDocument().Add("StatValue", sortDirection)),
                new BsonDocument("$limit", 1)
            };

            var resultsCursor = await _stats.AggregateAsync(aggregationPipeline);
            var result = await resultsCursor.FirstOrDefaultAsync();

            return result;
        }

        /// <summary>
        /// Concrete implementation of <see cref="IStatsRepository.CalcMaxOrMinWinrate(bool)"/>.
        /// </summary>
        public async Task<UserStatValue<double>> CalcMaxOrMinWinrate(bool min)
        {
            var sortDirection = min ? 1 : -1;

            PipelineDefinition<UserStats, UserStatValue<double>> aggregationPipeline = new[]
            {
                AddGamesPlayedInModeField(StatsCategory.StatsOverall),
                FilterOutStatsWithLessThanNGamesPlayedInMode(10),
                SortByTimestamp(SortDirection.Ascending),

                // Group by user id and project only the needed data
                new BsonDocument("$group", new BsonDocument()
                    .Add("_id", "$UserId")
                    .Add("UserId", new BsonDocument("$last", "$UserId"))
                    .Add("GamesWon", new BsonDocument("$last", "$StatsOverall.GamesWon"))
                    .Add("GamesPlayedInMode", new BsonDocument("$last", "$GamesPlayedInMode"))),

                // Project a calculated value for the desired per game average stat, factoring in game mode
                new BsonDocument("$project", new BsonDocument()
                    .Add("_id", "0")
                    .Add("UserId", "$UserId")
                    .Add("StatValue", new BsonDocument("$divide", 
                        new BsonArray().Add("$GamesWon").Add("$GamesPlayedInMode")))),

                // Grab the max or min stat value
                new BsonDocument("$sort", new BsonDocument().Add("StatValue", sortDirection)),
                new BsonDocument("$limit", 1)
            };

            var resultsCursor = await _stats.AggregateAsync(aggregationPipeline);
            return await resultsCursor.FirstOrDefaultAsync();
        }

        private static BsonDocument AddGamesPlayedInModeField(StatsCategory statsCategory)
        {
            return new BsonDocument("$addFields", new BsonDocument()
                .Add("GamesPlayedInMode", new BsonDocument()
                    .Add("$add", new BsonArray()
                        .Add($"${statsCategory}.GamesWon")
                        .Add($"${statsCategory}.GamesLost"))));
        }

        private static BsonDocument FilterOutStatsWithLessThanNGamesPlayedInMode(int n)
        {
            return new BsonDocument("$match", new BsonDocument()
                .Add("GamesPlayedInMode", new BsonDocument()
                    .Add("$gte", n)));
        }

        private static BsonDocument SortByTimestamp(SortDirection sortDirection)
        {
            switch (sortDirection)
            {
                case SortDirection.Ascending:
                    return new BsonDocument("$sort", new BsonDocument().Add("Timestamp", 1));
                case SortDirection.Descending:
                    return new BsonDocument("$sort", new BsonDocument().Add("Timestamp", -1));
                default:
                    throw new ArgumentException($"Unexpected value for sort direction given: {sortDirection}");
            }
        }
    }
}
