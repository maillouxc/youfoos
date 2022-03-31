using YouFoos.SharedLibrary.Utilities;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace YouFoos.SharedLibrary.Tests.Unit.Utilities
{
    [ExcludeFromCodeCoverage]
    public class GaussianFunctionsTests
    {
        [Fact]
        public void CumulativeToTests()
        {
            Assert.Equal(0.691462, GaussianFunctions.CumulativeTo(0.5), precision: 4);
        }

        [Fact]
        public void AtTests()
        {
            Assert.Equal(0.352065, GaussianFunctions.At(0.5), precision: 4);
        }
    }
}
