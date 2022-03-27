using Moq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Xunit;
using YouFoos.Api.Services;
using YouFoos.Api.Services.Authentication;
using YouFoos.DataAccess.Entities.Account;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.Api.Tests.Unit.Services.Authentication
{
    [ExcludeFromCodeCoverage]
    public class PasswordChangeServiceTests
    {
        private readonly PasswordChangeService _service;

        private readonly Mock<IPasswordResetCodeRepository> _mockResetCodeRepo = new Mock<IPasswordResetCodeRepository>();
        private readonly Mock<IAccountCredentialsRepository> _mockCredentialsRepo = new Mock<IAccountCredentialsRepository>();
        private readonly Mock<IEmailSender> _mockEmailSender = new Mock<IEmailSender>();

        private readonly PasswordResetCode _validResetCode = new PasswordResetCode("t1@gmail.com", "1234567890");
        private readonly PasswordResetCode _expiredResetCode = new PasswordResetCode("t2@gmail.com", "12345567890")
        {
            Created = DateTime.UtcNow.Subtract(new TimeSpan(hours: 0, minutes: 30, seconds: 0)),
        };

        private readonly AccountCredentials _testCredentials = new AccountCredentials()
        {
            Email = "t1@gmail.com",
            HashedPassword = "doesntmatterwhatweusehere"
        };
        private readonly AccountCredentials _testCredentials2 = new AccountCredentials()
        {
            Email = "t2@gmail.com",
            HashedPassword = "doesntmatterwhatweusehere"
        };
        private readonly AccountCredentials _testCredentials3 = new AccountCredentials()
        {
            Email = "t3@gmail.com",
            HashedPassword = "doesntmatterwhatweusehere"
        };

        public PasswordChangeServiceTests()
        {
            // Setup mocks
            _mockResetCodeRepo.Setup(mock => mock.GetResetCodeForUserWithEmail("t1@gmail.com"))
                                                 .ReturnsAsync(_validResetCode);
            _mockResetCodeRepo.Setup(mock => mock.GetResetCodeForUserWithEmail("t2@gmail.com"))
                                                 .ReturnsAsync(_expiredResetCode);
            
            _mockCredentialsRepo.Setup(mock => mock.GetAccountCredentialsForUserWithEmail("t1@gmail.com"))
                                                   .ReturnsAsync(_testCredentials);
            _mockCredentialsRepo.Setup(mock => mock.GetAccountCredentialsForUserWithEmail("t2@gmail.com"))
                                                   .ReturnsAsync(_testCredentials2);
            _mockCredentialsRepo.Setup(mock => mock.GetAccountCredentialsForUserWithEmail("t3@gmail.com"))
                                                   .ReturnsAsync(_testCredentials3);

            // Init the service with the mock objects
            _service = new PasswordChangeService(_mockEmailSender.Object,
                                                 _mockResetCodeRepo.Object,
                                                 _mockCredentialsRepo.Object);
        }

        [Fact]
        public async Task GivenExistingNonExpiredResetCode_CodeRequests_ShouldEmailSameCode()
        {
            const string existingCode = "1234567890";

            await _service.SendResetCodeToUser("t1@gmail.com");

            _mockEmailSender.Verify(a => a.SendPasswordResetEmail(
                It.Is<PasswordResetCode>(c => !c.Code.IsNullOrEmpty() && c.Code.Equals(existingCode)),
                It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GivenExpiredResetCode_CodeRequest_EmailsNewResetCode()
        {
            const string existingCode = "1234567890";

            await _service.SendResetCodeToUser("t2@gmail.com");

            _mockEmailSender.Verify(a => a.SendPasswordResetEmail(
                It.Is<PasswordResetCode>(c => !c.Code.IsNullOrEmpty() && !c.Code.Equals(existingCode)),
                It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GivenNoExistingCode_RequestResetCode_EmailsNewResetCode()
        {
            await _service.SendResetCodeToUser("t3@gmail.com");

            _mockEmailSender.Verify(a => a.SendPasswordResetEmail(
                It.Is<PasswordResetCode>(c => !c.Code.IsNullOrEmpty()), 
                It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GivenNonExistentUserCredentials_RequestResetCode_DoesNotThrowException()
        {
            await _service.SendResetCodeToUser("IDontExist@gmail.com");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task GivenNullOrEmptyEmail_RequestResetCode_ThrowsException(string email)
        {
            await Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                await _service.SendResetCodeToUser(email);
            });
        }

        [Fact]
        public async Task GivenNewPassword_ChangingPassword_StoresHashedPasswordNotPlaintext()
        {
            await _service.ChangePasswordForUser("t1@gmail.com", "testPassword");

            _mockCredentialsRepo.Verify(mock => mock.ReplaceCredentials(
                It.Is<AccountCredentials>(c => c.HashedPassword != "testPassword" 
                                           && !c.HashedPassword.IsNullOrEmpty())), Times.Once);
        }

        [Fact]
        public async Task GivenNewPasswordAndOldPassword_ChangePassword_ChangesPassword()
        {
            // Arrange
            var oldPassword = _testCredentials.HashedPassword;

            // Act
            await _service.ChangePasswordForUser("t1@gmail.com", "testPassword_unique");
            var storedPassword = _testCredentials.HashedPassword;

            Assert.True(oldPassword != storedPassword);
        }

        [Fact]
        public async Task GivenCredentialsDoNotExist_ChangePassword_ThrowsException()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() => 
                _service.ChangePasswordForUser("IDontExist@gmail.com", "Password123!")); ;
        }
    }
}
