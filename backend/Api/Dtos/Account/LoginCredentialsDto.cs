#pragma warning disable 1591 // Disable XML comments being required

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace YouFoos.Api.Dtos.Account
{
    [ExcludeFromCodeCoverage]
    public class LoginCredentialsDto
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
