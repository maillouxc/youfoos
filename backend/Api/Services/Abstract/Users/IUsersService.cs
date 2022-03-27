using System.Threading.Tasks;
using YouFoos.Api.Dtos;
using YouFoos.DataAccess.Entities.Account;

namespace YouFoos.Api.Services.Users
{
    /// <summary>
    /// Business logic class for working with users.
    /// </summary>
    public interface IUsersService
    {
        /// <summary>
        /// Returns the user with the given ID, or null if not found.
        /// </summary>
        Task<User> GetUserById(string id);

        /// <summary>
        /// Returns all users meeting the given criteria.
        /// </summary>
        Task<PaginatedResult<User>> GetAllUsers(int pageSize, int page, string email, string nameContains);
    }
}
