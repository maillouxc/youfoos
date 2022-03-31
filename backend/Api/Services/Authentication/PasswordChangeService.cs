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

        private readonly IEmailSender _emailSender;
        private readonly IPasswordResetCodeRepository _passwordResetCodeRepository;
        private readonly IAccountCredentialsRepository _accountCredentialsRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PasswordChangeService(
            IEmailSender emailSender, 
            IPasswordResetCodeRepository passwordResetCodeRepository,
            IAccountCredentialsRepository accountCredentialsRepository)
        {
            _emailSender = emailSender;
            _passwordResetCodeRepository = passwordResetCodeRepository;
            _accountCredentialsRepository = accountCredentialsRepository;
        }

        /// <summary>
        /// Given a user's email address, emails them a password reset code.
        /// </summary>
        /// <remarks>
        /// If the user already has an existing valid reset code that is not expired, the same code is resent to them
        /// again. Otherwise, a new code is generated and sent.
        ///
        /// If the provided email is null or empty, a NullReferenceException is thrown.
        /// 
        /// If the user credentials set for the given email does not exist, no indication is given - the method simply
        /// returns like normal, but does not send an email or generate a code. We don't want people (or robots) to be
        /// able to easily probe our service for valid emails, as this would be a security risk.
        /// </remarks>
        public async Task SendResetCodeToUser(string email)
        {
            if (string.IsNullOrEmpty(email)) throw new NullReferenceException("Email cannot be null or empty.");

            // If the user with the given email does not exist, just return like normal - give no indication
            var foundCredentials = await _accountCredentialsRepository.GetAccountCredentialsForUserWithEmail(email);
            if (foundCredentials == null)
            {
                Log.Logger.Information("Password reset requested for email {@email} but user does not exist.");
                return;
            }

            var existingCode = await _passwordResetCodeRepository.GetResetCodeForUserWithEmail(email);
            if (existingCode == null)
            {
                // They didn't have an outstanding reset request yet, so we can generate a new code and send it
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
