using System;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.Extensions.Options;
using Xunit;
using YouFoos.Api.Services.Authentication;
using YouFoos.Api.Config;
using YouFoos.DataAccess.SharedTestUtils.TestData;

namespace YouFoos.Api.Tests.Unit.Services.Authentication
{
    [ExcludeFromCodeCoverage]
    public class JwtMinterTests
    {
        private readonly IOptions<JwtSettings> _jwtSettings;
        private readonly JwtMinter _jwtMinter;

        public JwtMinterTests()
        {
            _jwtSettings = Options.Create<JwtSettings>(new JwtSettings()
            {
                Issuer = "youfoos.me/api",
                SigningSecret = "TestSecretThatIsActuallyLongEnough",
                TokenExpirationTimeMinutes = 60
            });

            _jwtMinter = new JwtMinter(_jwtSettings);
        }

        [Fact]
        public void JwtMinter_MintsTokenWithCorrectExpiration()
        {
            var jwt = new JwtSecurityToken(_jwtMinter.MintJwtForUser(TestUsers.TestUser_1));
            var expected = DateTime.UtcNow.AddMinutes((_jwtSettings.Value.TokenExpirationTimeMinutes)).Second;
            var actual = jwt.ValidTo.ToUniversalTime().Second;
            var difference = Math.Abs(actual - expected);
            const int oneMinute = 60;

            Assert.True(difference < oneMinute);
        }

        [Fact]
        public void MintJwtForUser_MintsTokenWithCorrectUserClaims()
        {
            var jwt = new JwtSecurityToken(_jwtMinter.MintJwtForUser(TestUsers.TestUser_1));

            var claimedId = jwt.Claims.Where(claim => claim.Type == "Id")
                                      .Select(claim => claim.Value)
                                      .SingleOrDefault();
            var claimedName = jwt.Claims.Where(claim => claim.Type == "Name")
                                        .Select(claim => claim.Value)
                                        .SingleOrDefault();
            var claimedEmail = jwt.Claims.Where(claim => claim.Type == "Email")
                                         .Select(claim => claim.Value)
                                         .SingleOrDefault();

            Assert.Equal(TestUsers.TestUser_1.Email, claimedEmail);
            Assert.Equal(TestUsers.TestUser_1.FirstAndLastName, claimedName);
            Assert.Equal(TestUsers.TestUser_1.Id, claimedId);
        }

        [Fact]
        public void GivenUserIsNull_MintJwtForUser_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => _jwtMinter.MintJwtForUser(null));
        }
    }
}
