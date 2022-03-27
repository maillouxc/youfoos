#pragma warning disable 1591 // Disable XML comments being required

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using YouFoos.Api.Validators;

namespace YouFoos.Api.Dtos.Account
{
    [ExcludeFromCodeCoverage]
    public class ChangePasswordDto
    {
        [Required]
        [ValidPassword]
        public string NewPassword { get; set; }

        [Required]
        public string OldPassword { get; set; }
    }
}
