using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace YouFoos.Api.Dtos
{
    /// <summary>
    /// Represents a generic error returned by the YouFoos REST API.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ApiError
    {
        /// <summary>
        /// The error message, which should be human friendly.
        /// </summary>
        [Required]
        public string Message { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ApiError(string message)
        {
            Message = message;
        }
    }
}
