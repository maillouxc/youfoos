using System;
using System.Threading.Tasks;

namespace YouFoos.StatisticsService.Services
{
    public interface ITournamentGameHandler
    {
        Task HandleGameIfIsTournamentGame(Guid gameGuid);
    }
}
