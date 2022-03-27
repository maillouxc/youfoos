using System;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities;

namespace YouFoos.GameEventsService.Services
{
    public interface IGameToTournamentResolverService
    {
        Task<Guid?> CheckIsTournamentGame(Game game);
    }
}
