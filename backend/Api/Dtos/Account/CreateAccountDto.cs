#pragma warning disable 1591 // Disable XML comments being required

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using YouFoos.Api.Validators;

namespace YouFoos.Api.Dtos.Account
{
    [ExcludeFromCodeCoverage]
    public class CreateAccountDto
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstAndLastName { get; set; }

        [Required]
        [ValidPassword]
        public string Password { get; set; }

        [Required]
        public string RfidNumber { get; set; }
    }
}
