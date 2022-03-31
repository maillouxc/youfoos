using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace YouFoos.Api.Validators
{
    /// <summary>
    /// A <see cref="ValidationAttribute"/> attribute that can be used to determine whether a password meets YouFoos requirements.
    /// </summary>
    public class ValidPassword : ValidationAttribute
    {
        private const int MIN_PASSWORD_LENGTH = 8;

        /// <summary>
        /// Determines whether the given password is valid according to YouFoos password security requirements.
        /// </summary>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;

            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Password cannot be empty.");
            }

            if (password.Any(char.IsUpper) &&
                password.Any(char.IsLower) &&
                password.Any(char.IsDigit) &&
                password.Length >= MIN_PASSWORD_LENGTH)
            {
                return ValidationResult.Success;
            }
            else
            {
                var errors = new List<string>();
          
                if (!password.Any(char.IsUpper))
                    errors.Add("Password must contain a upper case letter.");
                if (!password.Any(char.IsLower))
                    errors.Add("Password must contain a lower case letter.");
                if (!password.Any(char.IsDigit))
                    errors.Add("Password must contain a number.");
                if (password.Length < MIN_PASSWORD_LENGTH)
                    errors.Add("Password must contain at least 8 characters.");

                var errorString = string.Join(", ", errors.ToArray());

                return new ValidationResult(errorString);
            }
        }
    }
}
