using System.Threading.Tasks;
using YouFoos.DataAccess.Entities.Account;

namespace YouFoos.GameEventsService.Services
{
    public interface IRfidToUserAccountResolverService
    {
        Task<User> GetUserWithRfid(string rfidBytes);

        Task<User> GetUserWithRfidOrCreateNewAnonymousUser(string rfidBytes);
    }
}
