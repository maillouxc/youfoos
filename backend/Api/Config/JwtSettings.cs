namespace YouFoos.Api.Config
{
    /// <summary>
    /// The settings that YouFoos uses to configure JWT authentication.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// A secret key that all tokens are signed with.
        /// </summary>
        public string SigningSecret { get; set; }

        /// <summary>
        /// The number of minutes from creation that tokens should be considered expired.
        /// </summary>
        public int TokenExpirationTimeMinutes { get; set; }

        /// <summary>
        /// The token issuer - in the form of a URL.
        /// </summary>
        public string Issuer { get; set; }
    }
}
