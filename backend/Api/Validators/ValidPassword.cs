using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace YouFoos.Api.Validators
{
    /// <summary>
    /// A validation attribute that can be used to determine whether a password meets YouFoos requirements.
    /// </summary>
    public class ValidPassword : ValidationAttribute
    {
        /// <summary>
        /// Determines whether the given password is valid according to YouFoos requirements.
        /// </summary>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = (string) value;

            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Password cannot be empty.");
            }

            if (password.Any(char.IsUpper) &&
                password.Any(char.IsLower) &&
                password.Any(char.IsDigit) &&
                password.Length >= 8)
            {
                return ValidationResult.Success;
            }
            else
            {
                var errors = new List<string>();
          
                if (!password.Any(char.IsUpper))
                    errors.Add("Password must contain a upper case letter");
                if (!password.Any(char.IsLower))
                    errors.Add("Password must contain a lower case letter");
                if (!password.Any(char.IsDigit))
                    errors.Add("Password must contain a number");
                if (password.Length < 8)
                    errors.Add("Password must contain at least 8 characters");

                var errorString = string.Join(", ", errors.ToArray());
                return new ValidationResult(errorString);
            }
        }
    }
}
