using System.Threading.Tasks;
using YouFoos.DataAccess.Entities.Account;

namespace YouFoos.DataAccess.Repositories
{
    /// <summary>
    /// Data access class for working with account password reset codes.
    /// </summary>
    public interface IPasswordResetCodeRepository
    {
        /// <summary>
        /// Inserts a new password reset code into the database.
        /// </summary>
        Task InsertOne(PasswordResetCode resetCode);

        /// <summary>
        /// Returns the password reset code object for the user with the given email address.
        /// </summary>
        Task<PasswordResetCode> GetResetCodeForUserWithEmail(string email);

        /// <summary>
        /// Replaces the given password reset code with this updated version.
        /// </summary>
        Task ReplaceOne(PasswordResetCode resetCode);

        /// <summary>
        /// Deletes the password reset code for the user with the given email address.
        /// </summary>
        Task DeleteResetCodeForUserWithEmail(string email);
    }
}
