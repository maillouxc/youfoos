using System.Threading.Tasks;
using YouFoos.Api.Dtos;
using YouFoos.Api.Dtos.Account;
using YouFoos.DataAccess.Entities.Account;

namespace YouFoos.Api.Services
{
    /// <summary>
    /// Business logic class for handling the sending of YouFoos emails to users.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends an email based on the provided parameters.
        /// </summary>
        Task SendEmailAsync(string toAddress, string subject, string body);

        /// <summary>
        /// Sends the new user welcoming email that should be done when a new account is created.
        /// </summary>
        Task SendNewUserWelcomeEmail(CreateAccountDto newUser);
        
        /// <summary>
        /// Sends the password reset email for the provided password reset code.
        /// </summary>
        Task SendPasswordResetEmail(PasswordResetCode resetCode, int codeDurationMinutes);

        /// <summary>
        /// Sends an email to a user notifying them that a new tournament was created.
        /// </summary>
        Task SendTournamentCreationEmail(CreateTournamentRequest tournamentInfo, string toEmail);
    }
}
