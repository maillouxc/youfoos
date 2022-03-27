using System;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities.Account;

namespace YouFoos.StatisticsService.Services
{
    public interface IStatsCalculator
    {
        Task RecalculateStatsAfterGame(Guid gameId);

        Task RecalculateStatsForUserWithId(User user, DateTime timestamp);
    }
}
