using System.Collections.Generic;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities;

namespace YouFoos.DataAccess.Repositories
{
    /// <summary>
    /// Repository class for storing data related to user achievements.
    /// </summary>
    public interface IAchievementsRepository
    {
        /// <summary>
        /// Returns the progress for all achievements for the given user.
        /// </summary>
        Task<List<AchievementStatus>> GetAllAchievementsForUser(string userId);

        /// <summary>
        /// Replaces the achievement stats for the given achievements in the database.
        /// </summary>
        Task UpdateAchievementsProgressForUser(List<AchievementStatus> newStatuses);
    }
}
