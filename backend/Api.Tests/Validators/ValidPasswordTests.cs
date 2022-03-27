using System.Diagnostics.CodeAnalysis;
using Xunit;
using YouFoos.Api.Validators;

namespace YouFoos.Api.Tests.Unit.Validators
{
    [ExcludeFromCodeCoverage]
    public class ValidPasswordTests
    {
        [Theory]
        [InlineData("", false)]
        [InlineData("1", false)]
        [InlineData("a", false)]
        [InlineData("abc123", false)]
        [InlineData("abC1@", false)]
        [InlineData("123456768980AB", false)]
        [InlineData("Password123", true)]
        [InlineData("KeyLimePi1", true)]
        public void GivenPassword_ValidatePassword_ShouldReturnCorrectResult(string password, bool isValid)
        {
            var validPassword = new ValidPassword();
            var validationResult = validPassword.IsValid(password);

            Assert.Equal(validationResult, isValid);
        }
    }
}
