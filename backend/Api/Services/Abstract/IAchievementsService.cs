using System.Collections.Generic;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities;

namespace YouFoos.Api.Services
{
    /// <summary>
    /// Business logic class for interacting with achievments.
    /// </summary>
    public interface IAchievementsService
    {
        /// <summary>
        /// Returns the progress of all achievements for the given user.
        /// </summary>
        Task<List<AchievementStatus>> GetAllAchievementsForUser(string id);
    }
}
