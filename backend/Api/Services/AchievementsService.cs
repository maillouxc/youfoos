using System.Collections.Generic;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.Api.Services
{
    /// <summary>
    /// Concrete implementation of <see cref="IAchievementsService"/>.
    /// </summary>
    public class AchievementsService : IAchievementsService
    {
        private readonly IAchievementsRepository _achievementsRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AchievementsService(IAchievementsRepository achievementsRepository)
        {
            _achievementsRepository = achievementsRepository;
        }

        /// <summary>
        /// Concrete implementation of <see cref="IAchievementsService.GetAllAchievementsForUser(string)"/>.
        /// </summary>
        public async Task<List<AchievementStatus>> GetAllAchievementsForUser(string id)
        {
            return await _achievementsRepository.GetAllAchievementsForUser(id);
        }
    }
}
