using System.Threading.Tasks;
using YouFoos.Api.Dtos;
using YouFoos.DataAccess.Entities.Account;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.Api.Services.Users
{
    /// <summary>
    /// Concrete implementation of <see cref="IUsersService"/>.
    /// </summary>
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _usersRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        public UsersService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        /// <summary>
        /// Concrete implementation of <see cref="IUsersRepository.GetUserWithId(string)"/>.
        /// </summary>
        public async Task<User> GetUserById(string id)
        {
            return await _usersRepository.GetUserWithId(id);
        }

        /// <summary>
        /// Returns all users in the system.
        /// </summary>
        public async Task<PaginatedResult<User>> GetAllUsers(int pageSize, int page, string email, string nameContains)
        {
            // Doubt we'll ever have a problem with this cast, lol
            int totalCount = (int) await _usersRepository.CountUsers();

            var users = await _usersRepository.GetAllUsers(pageSize, page, email, nameContains);

            return new PaginatedResult<User>(page, totalCount, pageSize, users);
        }
    }
}
