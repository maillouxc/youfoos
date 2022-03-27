using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using YouFoos.Api.Utilities;

namespace YouFoos.Api.Tests.Unit.Utilities
{
    [ExcludeFromCodeCoverage]
    public class AlphanumericStringGeneratorTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(200)]
        public void GivenLength_Generator_ReturnsStringsOfCorrectLength(int length)
        {
            var result = AlphanumericStringGenerator.GetSecureRandomAlphanumericString(length);
            Assert.Equal(length, result.Length);
        }

        [Fact]
        public void GivenNegativeLength_Generator_ThrowsException()
        {
            int length = -1;
            Assert.Throws<ArgumentOutOfRangeException>
            (
                () => AlphanumericStringGenerator.GetSecureRandomAlphanumericString(length)
            );
        }
    }
}
