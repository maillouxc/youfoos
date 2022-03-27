using YouFoos.DataAccess.Entities.Account;

namespace YouFoos.Api.Services.Authentication
{
    /// <summary>
    /// This class is responsible for signing and minting authentication tokens used for user authentication.
    /// </summary>
    public interface IJwtMinter
    {        
        /// <summary>
        /// Returns a signed JWT auth token for the provided user.
        /// </summary>
        string MintJwtForUser(User userDto);
    }
}
