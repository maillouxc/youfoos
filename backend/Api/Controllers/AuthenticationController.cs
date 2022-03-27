using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YouFoos.Api.Dtos;
using YouFoos.Api.Dtos.Account;
using YouFoos.Api.Services.Authentication;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.Api.Controllers
{
    /// <summary>
    /// Although this class serves endpoints under the same base URL as Users controller, it exists
    /// to support the authentication related endpoints of the app, rather than the general user
    /// info related endpoints that are supported by the users controller.
    /// </summary>
    [Authorize]
    [Route("api/users")]
    [Produces("application/json")]
    [ApiController]
    public class AuthenticationController : CustomControllerBase
    {
        private readonly IAuthenticator _authenticator;
        private readonly IJwtMinter _jwtMinter;
        private readonly IPasswordChangeService _passwordChangeService;
        private readonly IUsersRepository _usersRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AuthenticationController(
            IAuthenticator authenticator,
            IJwtMinter jwtMinter,
            IPasswordChangeService passwordChangeService,
            IUsersRepository usersRepository)
        {
            _authenticator = authenticator;
            _jwtMinter = jwtMinter;
            _passwordChangeService = passwordChangeService;
            _usersRepository = usersRepository;
        }

        /// <summary>
        /// Returns a JWT for login purposes when supplied with valid user credentials.
        /// </summary>
        /// <remarks>
        /// This token expires after a certain time period, and a new one must be requested.
        /// Currently, this time period is set to 60 minutes.
        /// </remarks>
        /// <response code="401">Invalid login information provided.</response>
        /// <returns>A JWT Bearer token that should be sent on all subsequent requests.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<string>> Login([FromBody] LoginCredentialsDto userCredentials)
        {
            bool successfullyAuthenticated = await _authenticator.AuthenticateUser(userCredentials);
            if (successfullyAuthenticated)
            {
                var user = await _usersRepository.GetUserWithEmail(userCredentials.EmailAddress);
                var signedToken = _jwtMinter.MintJwtForUser(user);

                return Ok(signedToken);
            }

            return Unauthorized(new ApiError("Authentication failed."));
        }

        /// <summary>
        /// If a user exists with the given email, sends a password reset code via email to them.
        /// </summary>
        /// <remarks>
        /// The reset code has a duration of 10 minutes. If this endpoint is called again before
        /// the first code expires, the same code is sent again. A new code will not be generated
        /// until the first one expires.
        /// </remarks>
        /// <response code="200">Regardless of whether a user with the given email exists.</response>
        [AllowAnonymous]
        [HttpGet("{email}/reset-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> RequestPasswordResetCode([EmailAddress] string email)
        {
            await _passwordChangeService.SendResetCodeToUser(email);
            return Ok();
        }

        /// <summary>
        /// Changes the user's password, if the attached reset code is valid.
        /// </summary>
        /// <response code="401">Incorrect or expired reset code.</response>
        [AllowAnonymous]
        [HttpPost("{email}/reset-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ResetPassword(string email, ResetPasswordDto resetPasswordRequest)
        {
            var isSuccess = await _passwordChangeService.ResetPasswordForUser(email, resetPasswordRequest.NewPassword, resetPasswordRequest.PasswordResetToken);

            if (!isSuccess)
            {
                return Unauthorized(new ApiError("Password reset code invalid or expired."));
            }

            return Ok();
        }

        /// <summary>
        /// Changes the user's password, if the user is already logged in.
        /// </summary>
        /// <response code="401">Old password is incorrect.</response>
        [HttpPost("{id}/change-password")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ChangePassword(string id, ChangePasswordDto changePasswordRequest)
        {
            // Verify user ID matches expected email address - otherwise you could change another user's password
            string userEmail = GetUserEmail();
            if (userEmail == null) throw new ArgumentException("User email claim was null unexpectedly.");

            // Authenticate the user
            var loginCredentials = new LoginCredentialsDto()
            {
                EmailAddress = userEmail,
                Password = changePasswordRequest.OldPassword
            };

            // Change their password if authentication successful
            bool successfulAuthentication = await _authenticator.AuthenticateUser(loginCredentials);
            if (successfulAuthentication)
            {
                await _passwordChangeService.ChangePasswordForUser(userEmail, changePasswordRequest.NewPassword);
                return Ok();
            }

            return Unauthorized(new ApiError("Incorrect current password."));
        }
    }
}
