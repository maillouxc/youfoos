using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Xunit;
using Moq;
using YouFoos.Api.Dtos.Account;
using YouFoos.Api.Services.Authentication;
using YouFoos.DataAccess.Entities.Account;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.Api.Tests.Unit.Services.Authentication
{
    [ExcludeFromCodeCoverage]
    public class AuthenticatorTests
    {
        private readonly Authenticator _authenticator;

        public AuthenticatorTests()
        {
            // Create a mocked user with credentials
            var passwordHasher = new PasswordHasher<AccountCredentials>();
            var realHashedPassword = passwordHasher.HashPassword(null, "Bob12345@");
            var bobCredentials = new AccountCredentials()
            {
                Email = "bob@gmail.com",
                HashedPassword = realHashedPassword
            };
            
            var mockCredentialsRepository = new Mock<IAccountCredentialsRepository>();
            // Return null for a user not existing in our mock database
            mockCredentialsRepository
                .Setup(mock => mock.GetAccountCredentialsForUserWithEmail("john@gmail.com"))
                .ReturnsAsync((AccountCredentials) null);
            // Return the users credentials for a user that we will pretend exists
            mockCredentialsRepository
                .Setup(mock => mock.GetAccountCredentialsForUserWithEmail("bob@gmail.com"))
                .ReturnsAsync(bobCredentials);
            
            _authenticator = new Authenticator(mockCredentialsRepository.Object);
        }

        [Theory]
        [InlineData("bob@gmail.com", "iForgot", false)]
        [InlineData("john@gmail.com", "somePassword", false)]
        [InlineData("bob@gmail.com", "Bob12345@", true)]
        [InlineData("  ", "", false)]
        public async Task GivenCredentials_Authenticate_OnlyAllowsValidCredentials(
            string email, string password, bool shouldSucceed)
        {
            // Arrange - create a credentials object to login with
            var credentials = new LoginCredentialsDto()
            {
                EmailAddress = email,
                Password = password
            };

            // Act - attempt to authenticate
            var result = await _authenticator.AuthenticateUser(credentials);

            // Assert
            Assert.Equal(shouldSucceed, result);
        }
    }
}
