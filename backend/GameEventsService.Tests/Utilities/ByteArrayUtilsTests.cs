using Xunit;
using System.Diagnostics.CodeAnalysis;
using YouFoos.GameEventsService.Utilities;

namespace YouFoos.GameEventsService.Tests.Unit.Utilities
{
    [ExcludeFromCodeCoverage]
    public class ByteArrayUtilsTests
    {
        // This test data makes more sense in Binary form, trust me - hex is just more compact
        [Theory]
        [InlineData(new byte[] { 0x00, 0x00 }, new byte[] { 0x00, 0x00 })]
        [InlineData(new byte[] { 0xFF, 0xFF }, new byte[] { 0xFF, 0xFE })]
        [InlineData(new byte[] { 0x00, 0xFF }, new byte[] { 0x01, 0xFE })]
        [InlineData(new byte[] { 0xAA, 0xAA }, new byte[] { 0x55, 0x54 })]
        [InlineData(new byte[] { 0xFF }, new byte[] { 0xFE })]
        [InlineData(new byte[] { 0x09, 0x73, 0xAD, 0x34}, new byte[] { 0x12, 0xE7, 0x5A, 0x68 })]
        public void ShouldShiftByteArraysLeftByOne(byte[] givenBytes, byte[] expectedBytes)
        {
            ByteArrayUtils.ShiftLeft(givenBytes);
            // Compare actual and expected byte by byte
            for (int i = 0; i < givenBytes.Length; i++)
            {
                Assert.Equal(expectedBytes[i], givenBytes[i]);
            }
        }
    }
}
