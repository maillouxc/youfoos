using System.Threading.Tasks;
using YouFoos.Api.Dtos.Account;
using YouFoos.DataAccess.Entities.Account;

namespace YouFoos.Api.Services.Users
{
    /// <summary>
    /// Business logic class for handling new user account creation.
    /// </summary>
    public interface IAccountCreationService
    {
        /// <summary>
        /// Creates a new user account with the provided info.
        /// </summary>
        Task<User> RegisterNewUserAccount(CreateAccountDto userInfo);
    }
}
