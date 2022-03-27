using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Moq;
using Xunit;
using YouFoos.Api.Dtos.Account;
using YouFoos.Api.Exceptions;
using YouFoos.Api.Services;
using YouFoos.Api.Services.Users;
using YouFoos.DataAccess.Entities.Account;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.Api.Tests.Unit.Services
{
    [ExcludeFromCodeCoverage]
    public class AccountCreationServiceTests
    {
        [Fact]
        public async Task GivenEmailAlreadyInUse_RegisterNewAccount_ThrowsException()
        {
            // Arrange
            var signupInfo = new CreateAccountDto()
            {
                EmailAddress = "test@gmail.com",
                FirstAndLastName = "Testy McTestFace",
                Password = "P@ssword1@!",
                RfidNumber = "123456"
            };

            var existingAccount = new User("test@gmail.com", "Sir Testington", "654321");

            var mockUsersRepository = new Mock<IUsersRepository>();
            mockUsersRepository.Setup(r => r.GetUserWithEmail(
                It.Is<string>(s => s.Equals("test@gmail.com"))))
               .ReturnsAsync(existingAccount);

            var accountCreationService = new AccountCreationService(mockUsersRepository.Object, null, null);

            // Act and Assert
            await Assert.ThrowsAsync<ResourceAlreadyExistsException>(async () =>
            {
                await accountCreationService.RegisterNewUserAccount(signupInfo);
            });
        }

        [Fact]
        public async Task GivenRfidAlreadyInUseNotUnclaimedUser_RegisterNewUserAccount_ThrowsException()
        {
            // Arrange
            var signupInfo = new CreateAccountDto()
            {
                EmailAddress = "test@gmail.com",
                FirstAndLastName = "Testy McTestFace",
                Password = "P@ssword1@!",
                RfidNumber = "123456"
            };

            var existingAccount = new User("test123@gmail.com", "Sir Testington", "123456");

            var mockUsersRepository = new Mock<IUsersRepository>();
            mockUsersRepository.Setup(r => r.GetUserWithRfid(
                It.Is<string>(i => i.Equals("123456"))))
                .ReturnsAsync(existingAccount);

            var accountCreationService = new AccountCreationService(mockUsersRepository.Object, null, null);

            // Act and Assert
            await Assert.ThrowsAsync<ResourceAlreadyExistsException>(async () =>
            {
                await accountCreationService.RegisterNewUserAccount(signupInfo);
            });
        }

        [Fact]
        public async Task GivenRfidAlreadyInUseByUnclaimedUser_RegisterNewUserAccount_MergesAccounts()
        {
            // Arrange
            var signupInfo = new CreateAccountDto()
            {
                EmailAddress = "Testy.Testerson@test.test",
                FirstAndLastName = "Testy Testerson",
                Password = "Password123",
                RfidNumber = "654321"
            };
            
            var existingAccount = new User("", "", "0" ,true);
            
            var mockUsersRepository = new Mock<IUsersRepository>();
            mockUsersRepository.Setup(r => r.GetUserWithRfid(
                    It.Is<string>(i => i.Equals("654321"))))
                .ReturnsAsync(existingAccount);
            mockUsersRepository.Setup(o => o.UpsertUser(It.IsAny<User>()))
                .Returns(Task.CompletedTask).Verifiable();
            
            var mockAccountCredentialsRepository = new Mock<IAccountCredentialsRepository>();
            mockAccountCredentialsRepository.Setup(o => o.InsertNewUserCredentials(It.IsAny<AccountCredentials>()))
                .Returns(Task.CompletedTask);
            
            var mockEmailSender = new Mock<IEmailSender>();
            mockEmailSender.Setup(o => o.SendNewUserWelcomeEmail(It.IsAny<CreateAccountDto>()))
                .Returns(Task.CompletedTask);

            var accountCreationService = new AccountCreationService(
                mockUsersRepository.Object, 
                mockAccountCredentialsRepository.Object, 
                mockEmailSender.Object);

            // Act
            await accountCreationService.RegisterNewUserAccount(signupInfo);

            // Assert
            mockUsersRepository.Verify(o => o.UpsertUser(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task GivenValidRegistrationInfo_RegisterNewUserAccount_SendsWelcomeEmail()
        {
            // Arrange
            var signupInfo = new CreateAccountDto()
            {
                EmailAddress = "test@gmail.com",
                FirstAndLastName = "Testy McTestFace",
                Password = "P@ssword1@!",
                RfidNumber = "123456"
            };

            var mockUsersRepository = new Mock<IUsersRepository>();
            mockUsersRepository.Setup(r => r.GetUserWithRfid(
                    It.Is<string>(i => i.Equals("123456"))))
                    .ReturnsAsync(Task.FromResult<User>(null).Result);

            var mockAccountCredentialsRepository = new Mock<IAccountCredentialsRepository>();

            var mockEmailSender = new Mock<IEmailSender>();
            var accountCreationService = new AccountCreationService(mockUsersRepository.Object,
                                                                    mockAccountCredentialsRepository.Object,
                                                                    mockEmailSender.Object);

            // Act
            await accountCreationService.RegisterNewUserAccount(signupInfo);

            // Assert
            mockEmailSender.Verify(m => m.SendNewUserWelcomeEmail(It.IsAny<CreateAccountDto>()), Times.Once);
        }

        [Fact]
        public async Task GivenValidRegistrationInfo_RegisterNewUserAccount_HashesPassword()
        {
            // Arrange
            var signupInfo = new CreateAccountDto()
            {
                EmailAddress = "Testy.Testerson@test.test",
                FirstAndLastName = "Testy Testerson",
                Password = "Password123",
                RfidNumber = "654321"
            };

            var credentials = new AccountCredentials();
            
            var mockUsersRepository = new Mock<IUsersRepository>();
            mockUsersRepository.Setup(r => r.GetUserWithRfid(
                    It.Is<string>(i => i.Equals("654321"))))
                .ReturnsAsync(Task.FromResult<User>(null).Result);
            mockUsersRepository.Setup(o => o.InsertOne(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            var mockAccountCredentialsRepository = new Mock<IAccountCredentialsRepository>();
            mockAccountCredentialsRepository.Setup(o => o.InsertNewUserCredentials(It.IsAny<AccountCredentials>()))
                .Returns(Task.CompletedTask).Callback<AccountCredentials>(o => credentials = o);
            
            var mockEmailSender = new Mock<IEmailSender>();
            
            var accountCreationService = new AccountCreationService(
                mockUsersRepository.Object, 
                mockAccountCredentialsRepository.Object, 
                mockEmailSender.Object);
            
            // Act
            await accountCreationService.RegisterNewUserAccount(signupInfo);
            
            // Assert
            Assert.NotEqual(signupInfo.Password, credentials.HashedPassword);
        }

        [Fact]
        public async Task GivenValidRegistrationInfo_RegisterNewUserAccount_ReturnsNewlyCreatedUser()
        {
            // Arrange
            var signupInfo = new CreateAccountDto()
            {
                EmailAddress = "Testy.Testerson@test.test",
                FirstAndLastName = "Testy Testerson",
                Password = "Password123",
                RfidNumber = "654321"
            };
            
            var mockUsersRepository = new Mock<IUsersRepository>();
            mockUsersRepository.Setup(r => r.GetUserWithRfid(
                    It.Is<string>(i => i.Equals("654321"))))
                .ReturnsAsync(Task.FromResult<User>(null).Result);
            mockUsersRepository.Setup(o => o.InsertOne(It.IsAny<User>())).Returns(Task.CompletedTask).Verifiable();
            mockUsersRepository.SetupSequence(o => o.GetUserWithEmail(It.IsAny<string>()))
                .ReturnsAsync((User)null)
                .ReturnsAsync(new User(signupInfo.EmailAddress.ToLower(), 
                                       signupInfo.FirstAndLastName, 
                                       signupInfo.RfidNumber));
            
            var mockAccountCredentialsRepository = new Mock<IAccountCredentialsRepository>();
            var mockEmailSender = new Mock<IEmailSender>();
            
            var accountCreationService = new AccountCreationService(
                mockUsersRepository.Object, 
                mockAccountCredentialsRepository.Object, 
                mockEmailSender.Object);
            
            // Act and assert
            Assert.NotNull(await accountCreationService.RegisterNewUserAccount(signupInfo));

            // Ensure the user was created
            mockUsersRepository.Verify(o => o.InsertOne(It.IsAny<User>()), Times.Once);
        }
    }
}
