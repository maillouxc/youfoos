using System.Threading.Tasks;
using YouFoos.DataAccess.Entities.Account;

namespace YouFoos.DataAccess.Repositories
{
    /// <summary>
    /// Data accesss class for working with user account credentials.
    /// </summary>
    public interface IAccountCredentialsRepository
    {
        /// <summary>
        /// Returns the account credentials for the user with the given email, case insensitive,
        /// or null if no user has been found with that email.
        /// </summary>
        Task<AccountCredentials> GetAccountCredentialsForUserWithEmail(string email);

        /// <summary>
        /// Inserts a new set of account credentials into the database.
        /// </summary>
        Task InsertNewUserCredentials(AccountCredentials credentials);

        /// <summary>
        /// Replaces the given account credentials in teh database.
        /// </summary>
        Task ReplaceCredentials(AccountCredentials newCredentials);
    }
}
