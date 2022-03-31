using Microsoft.AspNetCore.Identity;
using Serilog;
using System;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities.Account;
using YouFoos.DataAccess.Repositories;
using YouFoos.Api.Utilities;

namespace YouFoos.Api.Services.Authentication
{
    /// <summary>
    /// Concrete implementation of <see cref="IPasswordChangeService"/>.
    /// </summary>
    public class PasswordChangeService : IPasswordChangeService
    {
        private const int PasswordResetCodeExpirationMinutes = 10;
        private const int PasswordResetCodeLength = 10;

        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;
        private readonly IPasswordResetCodeRepository _passwordResetCodeRepository;
        private readonly IAccountCredentialsRepository _accountCredentialsRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PasswordChangeService(
            ILogger logger,
            IEmailSender emailSender,
            IPasswordResetCodeRepository passwordResetCodeRepository,
            IAccountCredentialsRepository accountCredentialsRepository)
        {
            _logger = logger;
            _emailSender = emailSender;
            _passwordResetCodeRepository = passwordResetCodeRepository;
            _accountCredentialsRepository = accountCredentialsRepository;
        }

        /// <summary>
        /// Concrete implementation of <see cref="IPasswordChangeService.SendResetCodeToUser(string)"/>.
        /// 
        /// This implementation delivers reset codes via email.
        /// If a user with the given email address does not exist, this method will still return normally,
        /// which prevents users from fishing the system for valid email addresses by passing emails until it works.
        /// </summary>
        public async Task SendResetCodeToUser(string email)
        {
            if (string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email), "Value cannot be null or empty.");

            // If the user with the given email does not exist, just return like normal - give no indication
            var foundCredentials = await _accountCredentialsRepository.GetAccountCredentialsForUserWithEmail(email);
            if (foundCredentials == null)
            {
                _logger.Information("Password reset requested for user with email {@email} but user does not exist.", email);
                return;
            }

            var existingCode = await _passwordResetCodeRepository.GetResetCodeForUserWithEmail(email);

            if (existingCode == null)
            {
                // They didn't have an outstanding reset request yet, so we can generate a new code and send it:
                string code = AlphanumericStringGenerator.GetSecureRandomAlphanumericString(PasswordResetCodeLength);
                var resetCode = new PasswordResetCode(email, code);
                await _passwordResetCodeRepository.InsertOne(resetCode);
                await _emailSender.SendPasswordResetEmail(resetCode, PasswordResetCodeExpirationMinutes);
                return;
            }

            // Else, they already had a reset code in the database. Is it still valid?
            var expirationDate = existingCode.Created.AddMinutes(PasswordResetCodeExpirationMinutes);
            if (expirationDate > DateTime.UtcNow) // Yes, the code is still valid - resend it to them
            {
                await _emailSender.SendPasswordResetEmail(existingCode, PasswordResetCodeExpirationMinutes);
                return;
            }

            // Else, their existing code is expired, so generate a new one and send it to them
            existingCode.Code = AlphanumericStringGenerator.GetSecureRandomAlphanumericString(PasswordResetCodeLength);
            existingCode.Created = DateTime.UtcNow;
            await _passwordResetCodeRepository.ReplaceOne(existingCode);
            await _emailSender.SendPasswordResetEmail(existingCode, PasswordResetCodeExpirationMinutes);
        }
        
        /// <summary>
        /// Concrete implementation of <see cref="IPasswordChangeService.ResetPasswordForUser(string, string, string)"/>.
        /// </summary>
        public async Task<bool> ResetPasswordForUser(string email, string newPassword, string resetCode)
        {
            var isResetCodeValidForUser = await IsResetCodeValidForUser(resetCode, email);

            if (isResetCodeValidForUser)
            {
                await ChangePasswordForUser(email, newPassword);
                await _passwordResetCodeRepository.DeleteResetCodeForUserWithEmail(email);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Concrete implementation of <see cref="IPasswordChangeService.ChangePasswordForUser(string, string)"/>.
        /// </summary>
        public async Task ChangePasswordForUser(string email, string newPassword)
        {
            var passwordHasher = new PasswordHasher<AccountCredentials>();
            var credentials = await _accountCredentialsRepository.GetAccountCredentialsForUserWithEmail(email);
            credentials.HashedPassword = passwordHasher.HashPassword(null, newPassword);
            await _accountCredentialsRepository.ReplaceCredentials(credentials);
        }

        private async Task<bool> IsResetCodeValidForUser(string code, string email)
        {
            var resetCode = await _passwordResetCodeRepository.GetResetCodeForUserWithEmail(email);
            if (resetCode == null || string.IsNullOrEmpty(code)) return false;
            var expirationDate = resetCode.Created.AddMinutes(PasswordResetCodeExpirationMinutes);
            return (resetCode.Code == code && expirationDate > DateTime.Now);
        }
    }
}
