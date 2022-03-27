using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities;

namespace YouFoos.DataAccess.Repositories
{
    /// <summary>
    /// MongoDB implementation of <see cref="IAccoladesRepository"/>.
    /// </summary>
    public class AchievementsRepository : IAchievementsRepository
    {
        private const string CollectionName = "achievements";

        private readonly IMongoCollection<AchievementStatus> _achievements;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AchievementsRepository(IMongoContext mongoContext)
        {
            _achievements = mongoContext.GetCollection<AchievementStatus>(CollectionName);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IAchievementsRepository.GetAllAchievementsForUser(string)"/>.
        /// </summary>
        public Task<List<AchievementStatus>> GetAllAchievementsForUser(string userId)
        {
            var achievementStatuses = new List<AchievementStatus>();

            return _achievements.Find(achievementStatus => achievementStatus.UserId == userId).ToListAsync();
        }

        /// <summary>
        /// Concrete implementation of <see cref="IAchievementsRepository.UpdateAchievementsProgressForUser(List{AchievementStatus})"/>.
        /// </summary>
        public async Task UpdateAchievementsProgressForUser(List<AchievementStatus> updatedStatuses)
        {
            var bulkUpsert = new List<WriteModel<AchievementStatus>>();

            foreach (var achievementStatus in updatedStatuses)
            {
                var userIdFilter = Builders<AchievementStatus>.Filter.Where(obj => obj.UserId == achievementStatus.UserId);
                var achievementNameFilter = Builders<AchievementStatus>.Filter.Where(obj => obj.Name == achievementStatus.Name);

                var combinedFilter = userIdFilter & achievementNameFilter;

                bulkUpsert.Add(new ReplaceOneModel<AchievementStatus>(combinedFilter, achievementStatus) { IsUpsert = true });
            }

            await _achievements.BulkWriteAsync(bulkUpsert);
        }
    }
}
