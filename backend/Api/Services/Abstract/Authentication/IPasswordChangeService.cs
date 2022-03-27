using System.Threading.Tasks;

namespace YouFoos.Api.Services.Authentication
{
    /// <summary>
    /// The service responsible for handling password changes and resets.
    /// </summary>
    public interface IPasswordChangeService
    {
        /// <summary>
        /// Sends a password reset code to the user with the given email address.
        /// </summary>
        /// <remarks>
        /// This contract does not guarantee that the code will be sent via email - the email address is used merely
        /// to look up the user. The reset code could theoretically be sent via some other means, such as SMS.
        /// </remarks>
        /// <param name="email">The email address of the user to send the reset code to.</param>
        Task SendResetCodeToUser(string email);

        /// <summary>
        /// When provided a valid reset code, changes the user's password to the new password - else does nothing.
        /// After resetting the user's password, the reset code is deleted so that it cannot be reused.
        /// 
        /// This method assumes that the new password has already been validated to meet any password requirements.
        /// </summary>
        /// <param name="email">The email address of the user to reset the password for.</param>
        /// <param name="newPassword">The new password to use.</param>
        /// <param name="resetCode">The reset code provided by the user.</param>
        /// <returns>True if the reset code was valid and the password was changed.</returns>
        Task<bool> ResetPasswordForUser(string email, string newPassword, string resetCode);

        /// <summary>
        /// Changes the password for the user with the given email address.
        ///
        /// This method assumes that the new password has already been validated to meet any password requirements.
        /// </summary>
        /// <remarks>
        /// Throws a NullReferenceException if the given user's credentials could not be found.
        /// </remarks>
        /// <param name="email">The email address of the user to change the password for.</param>
        /// <param name="newPassword">The new password to use.</param>
        Task ChangePasswordForUser(string email, string newPassword);
    }
}
