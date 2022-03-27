using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Xunit;
using YouFoos.Api.Config;
using YouFoos.Api.Services;

namespace YouFoos.Api.Tests.Unit.Services
{
    [ExcludeFromCodeCoverage]
    public class EmailSenderTests
    {
        [Theory]
        [InlineData("", "test", "test")]
        [InlineData(null, "test", "test")]
        [InlineData("test", "test", "test")]
        [InlineData("test@test", "test", "test")]
        [InlineData("test@test.com", "", "test")]
        [InlineData("test@test.com", null, "")]
        [InlineData("test@test.com", "test", "")]
        public async Task GivenInvalidArguments_SendEmailAsync_ThrowsArgumentException(string to, string subj, string body)
        {
            var someOptions = Options.Create(new EmailSettings());
            var emailSender = new EmailSender(someOptions);

            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await emailSender.SendEmailAsync(to, subj, body);
            });
        }
    }
}
