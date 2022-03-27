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
    public class DataRemediator
    {
        private readonly IMongoContext _dbContext;
        private readonly IGamesRepository _gamesRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IStatsCalculator _statsCalculator;
        private readonly IAccoladesCalculator _accoladesCalculator;
        private readonly IAchievementUnlockService _achievementUnlockService;

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

        public async Task ResetAllPlayerRanks()
        {
            var allUsers = await _usersRepository.GetAllUsers(pageSize: int.MaxValue, page: 0);

            foreach (var user in allUsers)
            {
                await _usersRepository.Update1V1SkillForUser(user.Id, new Trueskill());
                await _usersRepository.Update2V2SkillForUser(user.Id, new Trueskill());
            }
        }

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
