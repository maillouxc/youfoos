using System.Diagnostics.CodeAnalysis;
using Moq;
using Xunit;
using YouFoos.DataAccess.Entities.Account;
using YouFoos.DataAccess.Repositories;
using YouFoos.GameEventsService.Services;

namespace YouFoos.GameEventsService.Tests.Unit.Services
{
    [ExcludeFromCodeCoverage]
    public class RfidToUserAccountResolverServiceTests
    {
        private readonly RfidToUserAccountResolverService _sut;

        private readonly Mock<IUsersRepository> _mockUsersRepository;

        public RfidToUserAccountResolverServiceTests()
        {
            var existingUser = new User("t@gmail.com", "Testy Puller", "4630117");

            _mockUsersRepository = new Mock<IUsersRepository>();
            _mockUsersRepository.Setup(mock => mock.GetUserWithRfid(
                    It.Is<string>(i => i.Equals("4630117"))))
                    .ReturnsAsync(existingUser);

            _sut = new RfidToUserAccountResolverService(_mockUsersRepository.Object);
        }

        [Fact]
        public async void GivenUnusedRfid_GetUserWithRfidOrCreateNewAnonymousUser_ShouldCreateNewAnonymousUser()
        {
            const string rfidBytes = "97 3A D3 40 90 00"; // Equates to 4630118 on the printed card
            await _sut.GetUserWithRfidOrCreateNewAnonymousUser(rfidBytes);
            
            _mockUsersRepository.Verify(mock => mock.InsertOne(
                It.Is<User>(value => value.IsUnclaimed.Equals(true) 
                                  && value.RfidNumber.Equals("4630118"))
            ), Times.Once);
        }

        [Fact]
        public async void GivenRfidAlreadyInUse_GetUserWithRfidOrCreateNewAnonymousUser_ShouldReturnUser()
        {
            const string testRfid = "97 3A D2 C0 90 00"; // Equates to 4630117 on the printed card
            var user = await _sut.GetUserWithRfidOrCreateNewAnonymousUser(testRfid);
            
            Assert.Equal("4630117", user.RfidNumber);
        }

        [Fact]
        public async void GivenUnusedRfid_GetUserWithRfid_ShouldNotCreateNewAnonUser()
        {
            const string rfidBytes = "97 3A D3 40 90 00"; // Equates to 4630118 on the printed card
            await _sut.GetUserWithRfid(rfidBytes);
            _mockUsersRepository.Verify(mock => mock.InsertOne(It.IsAny<User>()), Times.Never);
        }
    }
}
