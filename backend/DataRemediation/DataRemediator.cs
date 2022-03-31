using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using YouFoos.DataAccess;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Entities.Stats;
using YouFoos.DataAccess.Repositories;
using YouFoos.StatisticsService.Services;

namespace YouFoos.DataRemediation
{
    /// <summary>
    /// This is a simple command line utility that can be used to rebuild the YouFoos database from
    /// from the game events. It is useful if a bug is discovered in stats calculation code or elsewhere
    /// in the processing pipeline, or if we have added new stats to the system and wish to retroactively
    /// track them for past games played.
    /// </summary>
    public class DataRemediator
    {
        private readonly IMongoContext _dbContext;
        private readonly IGamesRepository _gamesRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IStatsCalculator _statsCalculator;
        private readonly IAccoladesCalculator _accoladesCalculator;
        private readonly IAchievementUnlockService _achievementUnlockService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DataRemediator(IMongoContext mongoContext,
                              IGamesRepository gamesRepository,
                              IUsersRepository usersRepository,
                              IStatsCalculator statsCalculator,
                              IAccoladesCalculator accoladesCalculator,
                              IAchievementUnlockService achievementUnlockService)
        {
            _dbContext = mongoContext;
            _gamesRepository = gamesRepository;
            _statsCalculator = statsCalculator;
            _usersRepository = usersRepository;
            _accoladesCalculator = accoladesCalculator;
            _achievementUnlockService = achievementUnlockService;
        }

        /// <summary>
        /// Runs the main logic of the utility - recalculates all user stats, accolades, and achievements.
        /// </summary>
        public async Task RemediateData()
        {
            var games = LoadAllGames().Result;

            foreach (var game in games)
            {
                Console.WriteLine($"Remediating game {game.GameNumber} of {games.Count}");
                await _statsCalculator.RecalculateStatsAfterGame(game.Guid);
                await _accoladesCalculator.RecalculateAllAccolades();
                await _achievementUnlockService.UpdateAchievementStatusesPostGame(game.Guid);
            }
        }

        /// <summary>
        /// Resets all player ranks to their intial values. This is necessary before recalculating stats.
        /// </summary>
        public async Task ResetAllPlayerRanks()
        {
            var allUsers = await _usersRepository.GetAllUsers(pageSize: int.MaxValue, page: 0);

            foreach (var user in allUsers)
            {
                await _usersRepository.Update1V1SkillForUser(user.Id, new Trueskill());
                await _usersRepository.Update2V2SkillForUser(user.Id, new Trueskill());
            }
        }

        /// <summary>
        /// Prepares the database for the remediation process by dropping collections that will be rebuilt.
        /// </summary>
        public async Task DropCollectionsThatWillBeRebuilt()
        {
            await _dbContext.GetCollection<UserStats>("stats").DeleteManyAsync(_ => true);
            await _dbContext.GetCollection<Accolade>("accolades").DeleteManyAsync(_ => true);
            await _dbContext.GetCollection<AchievementStatus>("achievements").DeleteManyAsync(_ => true);
        }

        private async Task<List<Game>> LoadAllGames()
        {
            return await _gamesRepository.GetAllGamesChronological();
        }
    }
}
