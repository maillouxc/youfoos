using System.ComponentModel.DataAnnotations;

namespace YouFoos.Api.Dtos
{
    /// <summary>
    /// Represents a generic error returned by the YouFoos REST API.
    /// </summary>
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
