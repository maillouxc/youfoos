using System.Threading.Tasks;
using YouFoos.Api.Dtos.Account;

namespace YouFoos.Api.Services.Authentication
{
    /// <summary>
    /// Business logic class resposible for the authentication of users.
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// Returns true if provided valid credentials, otherwise returns false.
        /// </summary>
        Task<bool> AuthenticateUser(LoginCredentialsDto credentials);
    }
}
