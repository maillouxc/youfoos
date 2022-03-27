using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Moq;
using Xunit;
using YouFoos.Api.Exceptions;
using YouFoos.Api.Services.Users;
using YouFoos.DataAccess.Repositories;
using YouFoos.DataAccess.SharedTestUtils.TestData;

namespace YouFoos.Api.Tests.Unit.Services
{
    [ExcludeFromCodeCoverage]
    public class RfidChangeServiceTests
    {
        private readonly RfidChangeService _service;
        private readonly Mock<IUsersRepository> _mockUsersRepo;

        public RfidChangeServiceTests()
        {
            _mockUsersRepo = new Mock<IUsersRepository>();
            
            // Configure mocks
            _mockUsersRepo.Setup(m => m.GetUserWithRfid(TestUsers.TestUser_1.RfidNumber))
                .ReturnsAsync(TestUsers.TestUser_1);
            _mockUsersRepo.Setup(m => m.GetUserWithRfid(TestUsers.TestUser_3_Anon.RfidNumber))
                .ReturnsAsync(TestUsers.TestUser_3_Anon);
            _mockUsersRepo.Setup(m => m.GetUserWithRfid(TestUsers.TestUser_2.RfidNumber))
                .ReturnsAsync(TestUsers.TestUser_2);
            _mockUsersRepo.Setup(m => m.GetUserWithId(TestUsers.TestUser_2.Id))
                .ReturnsAsync(TestUsers.TestUser_2);
            _mockUsersRepo.Setup(m => m.UpdateRfidForUser(TestUsers.TestUser_3_Anon.Id, "6666666"))
                .Returns(Task.CompletedTask).Verifiable();
            _mockUsersRepo.Setup(m => m.UpdateRfidForUser(TestUsers.TestUser_1.Id, "6666666"))
                .Returns(Task.CompletedTask).Verifiable();

            _service = new RfidChangeService(_mockUsersRepo.Object);
        }

        [Fact]
        public async Task GivenExistingUnclaimedUserWithRfid_ChangeRfid_CorrectlyChangesRfid()
        {
            // Act
            await _service.ChangeRfidNumberForUser(TestUsers.TestUser_3_Anon.Id, "6666666");
            
            // Assert
            _mockUsersRepo.Verify(m => m.UpdateRfidForUser(TestUsers.TestUser_3_Anon.Id, "6666666"), Times.Once);
        }

        [Fact]
        public async Task GivenExistingUserAndInUseRfid_ChangeRfid_ThrowsException()
        {
            await Assert.ThrowsAsync<ResourceAlreadyExistsException>(async () =>
            {
                await _service.ChangeRfidNumberForUser(
                    TestUsers.TestUser_1.Id, TestUsers.TestUser_2.RfidNumber);
            });
        }

        [Fact]
        public async Task GivenExistingUserAndUnusedRfid_ChangeRfid_UpdatesRfid()
        {
            // Act
            await _service.ChangeRfidNumberForUser(TestUsers.TestUser_1.Id, "6666666");
            
            // Assert
            _mockUsersRepo.Verify(m => m.UpdateRfidForUser(TestUsers.TestUser_1.Id, "6666666"), Times.Once);
        }
    }
}
