using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using YouFoos.DataAccess.Entities.Account;

namespace YouFoos.DataAccess.Repositories
{
    /// <summary>
    /// Concrete implementation of <see cref="IAccountCredentialsRepository"/>.
    /// </summary>
    public class AccountCredentialsRepository : IAccountCredentialsRepository
    {
        private const string CollectionName = "account_credentials";

        private readonly IMongoCollection<AccountCredentials> _accountCredentials;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AccountCredentialsRepository(IMongoContext mongoContext)
        {
            _accountCredentials = mongoContext.GetCollection<AccountCredentials>(CollectionName);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IAccountCredentialsRepository.GetAccountCredentialsForUserWithEmail(string)"/>.
        /// </summary>
        public Task<AccountCredentials> GetAccountCredentialsForUserWithEmail(string email)
        {
            return _accountCredentials.AsQueryable().SingleOrDefaultAsync(
                creds => creds.Email.ToLower() == email.ToLower()
            );
        }

        /// <summary>
        /// Concrete implementation of <see cref="IAccountCredentialsRepository.InsertNewUserCredentials(AccountCredentials)"/>.
        /// </summary>
        public Task InsertNewUserCredentials(AccountCredentials credentials)
        {
            return _accountCredentials.InsertOneAsync(credentials);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IAccountCredentialsRepository.ReplaceCredentials(AccountCredentials)"/>.
        /// </summary>
        public Task ReplaceCredentials(AccountCredentials newCredentials)
        {
            return _accountCredentials.ReplaceOneAsync(
                existing => existing.Email.ToLower() == newCredentials.Email.ToLower(),
                newCredentials
            );
        }
    }
}
