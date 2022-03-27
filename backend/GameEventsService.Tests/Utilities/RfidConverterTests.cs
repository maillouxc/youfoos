using System.Diagnostics.CodeAnalysis;
using Xunit;
using YouFoos.GameEventsService.Utilities;

namespace YouFoos.GameEventsService.Tests.Unit.Utilities
{
    [ExcludeFromCodeCoverage]
    public class RfidConverterTests
    {
        [Theory]
        [InlineData("97 3A D3 40 90 00", "4630118")]
        [InlineData("97 3A D2 C0 90 00", "4630117")]
        [InlineData("97 3A D1 B0 90 00", "4630115")]
        [InlineData("97 3A D0 A0 90 00", "4630113")]
        [InlineData("97 0A D0 A0 90 00", "4605537")]
        public void ShouldReturnCorrectCardNumberWhenGivenUid(string uid, string expectedCardNumber)
        {
            string cardNumber = RfidConverter.ConvertRfidUidToNumberOnCard(uid);

            Assert.Equal(expectedCardNumber, cardNumber);
        }
    }
}
