using System.Diagnostics.CodeAnalysis;

namespace YouFoos.Api.Config
{
    /// <summary>
    /// Contains strongly typed configuration values used to configure JWT authentication.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class JwtSettings
    {
        /// <summary>
        /// A secret key with all tokens are signed with.
        /// </summary>
        public string SigningSecret { get; set; }

        /// <summary>
        /// The number of minutes from creation that tokens should be considered expired.
        /// </summary>
        public int TokenExpirationTimeMinutes { get; set; }

        /// <summary>
        /// The token issuer.
        /// </summary>
        public string Issuer { get; set; }
    }
}
