using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Entities.Enums;

namespace YouFoos.DataAccess.Repositories
{
    /// <summary>
    /// MongoDB Implementation of <see cref="IGamesRepository"/>.
    /// </summary>
    public class GamesRepository : IGamesRepository
    {
        private const string CollectionName = "games";

        private readonly IMongoCollection<Game> _games;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GamesRepository(IMongoContext mongoContext)
        {
            _games = mongoContext.GetCollection<Game>(CollectionName);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IGamesRepository.InsertGame(Game)"/>.
        /// </summary>
        public async Task InsertGame(Game game)
        {
            // TODO Don't know why we need this but too afraid to touch it right now
            FilterDefinition<Game> filter = "{ 'GUID': '" + game.Guid + "'}";

            // If game does not already exist
            if (_games.CountDocuments(filter) == 0)
            {
                // MongoDB doesn't have any kind of built-in auto-increment functionality
                game.GameNumber = (int) (await CountGames() + 1);

                await _games.InsertOneAsync(game);
                return;
            } 

            throw new InvalidOperationException("Cannot create new game - game already exists in database.");
        }
        
        /// <summary>
        /// Concrete implementation of <see cref="IGamesRepository.GetGameById(Guid)"/>.
        /// </summary>
        public Task<Game> GetGameById(Guid id)
        {
            return _games.Find(game => game.Guid == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Concrete implementation of <see cref="IGamesRepository.GetCurrentGame"/>.
        /// </summary>
        public Task<Game> GetCurrentGame()
        {
            return _games.Find(game => game.IsInProgress).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Concrete implementation of <see cref="IGamesRepository.GetAllGamesChronological"/>.
        /// </summary>
        public Task<List<Game>> GetAllGamesChronological()
        {
            return _games.Find(_ => true).SortBy(g => g.StartTimeUtc).ToListAsync();
        }

        /// <summary>
        /// Concrete implementation of <see cref="IGamesRepository.GetListOfRecentGames(int)"/>.
        /// </summary>
        public async Task<List<Game>> GetListOfRecentGames(int numberOfGames = 100)
        {
            return await _games
                .Find(game => game.IsInProgress == false)
                .SortByDescending(game => game.StartTimeUtc)
                .Limit(numberOfGames)
                .ToListAsync();
        }

        /// <summary>
        /// Concrete implementation of <see cref="IGamesRepository.GetListOfRecentGamesByUserId(string, DateTime, int)"/>.
        /// </summary>
        public Task<List<Game>> GetListOfRecentGamesByUserId(string id, DateTime cutoff, int numberOfGames = int.MaxValue)
        {
            if (string.IsNullOrEmpty(id)) return null;

            var userId = ObjectId.Parse(id);
            var userIdFilter =
                Builders<Game>.Filter.Eq("BlackOffenseUserId", userId)
                | Builders<Game>.Filter.Eq("BlackDefenseUserId", userId)
                | Builders<Game>.Filter.Eq("GoldOffenseUserId", userId)
                | Builders<Game>.Filter.Eq("GoldDefenseUserId", userId);

            var isNotInProgressFilter = Builders<Game>.Filter.Eq("IsInProgress", false);
            var cutoffDateFilter = Builders<Game>.Filter.Lte("EndTimeUtc", cutoff);
            
            return _games
                .Find(userIdFilter & isNotInProgressFilter & cutoffDateFilter)
                .SortByDescending(game => game.EndTimeUtc)
                .Limit(numberOfGames)
                .ToListAsync();
        }

        /// <summary>
        /// Concrete implementation of <see cref="IGamesRepository.CountGames"/>.
        /// </summary>
        public Task<long> CountGames()
        {
            return _games.CountDocumentsAsync(_ => true);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IGamesRepository.CountGoalsScored"/>.
        /// </summary>
        public Task<int> CountGoalsScored()
        {
            return _games.AsQueryable().SumAsync(g => g.BlackTeamScore + g.GoldTeamScore);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IGamesRepository.CountTimePlayedSecs"/>.
        /// </summary>
        public Task<long> CountTimePlayedSecs()
        {
            return _games.AsQueryable().SumAsync(g => (g.Goals.Last().TimeStampGameClock));
        }

        /// <summary>
        /// Concrete implementation of <see cref="IGamesRepository.CountTeamWins(TeamColor)"/>.
        /// </summary>
        public async Task<int> CountTeamWins(TeamColor team)
        {
            var teamWins = 0;

            if (team == TeamColor.BLACK)
            {
                await _games.AsQueryable().ForEachAsync(c =>
                {
                    if (c.BlackTeamScore > c.GoldTeamScore)
                    {
                        teamWins++;
                    }
                });
            }
            else
            {
                await _games.AsQueryable().ForEachAsync(c =>
                {
                    if (c.GoldTeamScore > c.BlackTeamScore)
                    {
                        teamWins++;
                    }
                });
            }

            return teamWins;
        }

        /// <summary>
        /// Concrete implementation of <see cref="IGamesRepository.InsertGoal(Goal)"/>.
        /// </summary>
        public async Task InsertGoal(Goal goal)
        {
            var game = await _games.Find(o => o.Guid.Equals(goal.GameGuid)).SingleAsync();
            game.Goals.Add(goal);
            game.RecalculateScore();
            await _games.FindOneAndReplaceAsync(o => o.Guid.Equals(game.Guid), game);
        }
        
        /// <summary>
        /// Concrete implementation of <see cref="IGamesRepository.UndoGoal"/>.
        /// </summary>
        public async Task UndoGoal(Guid gameGuid, DateTime timestamp)
        {
            var game = await _games.Find(o => o.Guid.Equals(gameGuid)).SingleAsync();

            var goalList = game.Goals.OrderByDescending(o => o.TimeStampUtc).ToList();

            // If the game has no goals yet, just return without doing anything
            if (goalList.Count == 0) return;

            foreach (var goal in goalList)
            {
                if (goal.IsUndone)
                {
                    continue; // Skip goals already undone
                }

                goal.IsUndone = true;
                break;
            }
            game.Goals = goalList;
            game.RecalculateScore();

            await _games.FindOneAndReplaceAsync(g => g.Guid == game.Guid, game);
        }
        
        /// <summary>
        /// Concrete implementation of <see cref="IGamesRepository.EndGame(Guid, DateTime)"/>.
        /// </summary>
        public Task EndGame(Guid gameGuid, DateTime endTimeUtc)
        {
            var updateDef = Builders<Game>.Update.Set(game => game.IsInProgress, false)
                                                 .Set(game => game.EndTimeUtc, endTimeUtc);

            return _games.UpdateOneAsync(game => game.Guid.Equals(gameGuid), updateDef);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IGamesRepository.DeleteGameByIdAsync(Guid)"/>.
        /// </summary>
        public Task DeleteGameByIdAsync(Guid gameGuid)
        {
            return _games.DeleteOneAsync(game => game.Guid == gameGuid);
        }
    }
}
