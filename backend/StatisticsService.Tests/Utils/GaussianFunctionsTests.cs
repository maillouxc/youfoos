using YouFoos.StatisticsService.Utils;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace YouFoos.StatisticsService.Tests.Unit.Utils
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
