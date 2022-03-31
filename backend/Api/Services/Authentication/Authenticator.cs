using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using YouFoos.Api.Dtos.Account;
using YouFoos.DataAccess.Entities.Account;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.Api.Services.Authentication
{
    /// <summary>
    /// Concrete implementation of <see cref="IAuthenticator"/>.
    /// </summary>
    public class Authenticator : IAuthenticator
    {
        private readonly IAccountCredentialsRepository _credentialsRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Authenticator(IAccountCredentialsRepository credentialsRepository)
        {
            _credentialsRepository = credentialsRepository;
        }

        /// <summary>
        /// Concrete implementation of <see cref="IAuthenticator.AuthenticateUser(LoginCredentialsDto)"/>.
        /// </summary>
        public async Task<bool> AuthenticateUser(LoginCredentialsDto credentials)
        {
            if (credentials == null) return false;

            var expectedCredentials = await _credentialsRepository.GetAccountCredentialsForUserWithEmail(credentials.EmailAddress);
            if (expectedCredentials == null) return false;

            var passwordHasher = new PasswordHasher<AccountCredentials>();
            var hashVerificationResult = passwordHasher.VerifyHashedPassword(
                expectedCredentials, expectedCredentials.HashedPassword, credentials.Password);

            return hashVerificationResult == PasswordVerificationResult.Success;
        }
    }
}
