using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Serilog;
using YouFoos.Api.Dtos.Account;
using YouFoos.Api.Dtos;
using YouFoos.Exceptions;
using YouFoos.Api.Services.Users;
using YouFoos.DataAccess.Entities.Account;

namespace YouFoos.Api.Controllers
{
    /// <summary>
    /// This controller handles user profile and account related endpoints.
    /// </summary>
    [Authorize]
    [Route("api/users")]
    [Produces("application/json")]
    [ApiController]
    public class UsersController : CustomControllerBase
    {
        private readonly IAccountCreationService _accountCreationService;
        private readonly IRfidChangeService _rfidChangeService;
        private readonly IUsersService _usersService;
        private readonly IAvatarService _avatarService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public UsersController(
            IAccountCreationService accountCreationService,
            IRfidChangeService rfidChangeService,
            IUsersService usersService, 
            IAvatarService avatarService)
        {
            _accountCreationService = accountCreationService;
            _rfidChangeService = rfidChangeService;
            _usersService = usersService;
            _avatarService = avatarService;
        }

        /// <summary>
        /// Registers a new user account with the provided information.
        /// </summary>
        /// <param name="newUserInfo">The new account registration details.</param>
        /// <returns>The newly created user information, including their ID.</returns>
        /// <response code="409">If user with the given email already exists.</response>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<User>> CreateUser(CreateAccountDto newUserInfo)
        {
            try
            {
                var newlyCreatedUser = await _accountCreationService.RegisterNewUserAccount(newUserInfo);
                return Created("{id}/" + newlyCreatedUser.Id, newlyCreatedUser);
            }
            catch (ResourceAlreadyExistsException e)
            {
                return Conflict(new ApiError(e.Message));
            }
        }

        /// <summary>
        /// Returns a list of all app users. There are several query parameters for searching and filtering available.
        /// </summary>
        /// <param name="nameContains">The name (or partial name) of the user - used to search by user name.</param>
        /// <param name="email">The email of the user - used to search by email.</param>
        /// <param name="page">The page number to return - 0 indexed.</param>
        /// <param name="pageSize">The number of results per page to return - default is 20.</param>
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<User>>> GetAllUsers(
            [FromQuery] string nameContains,
            [FromQuery] string email,
            [FromQuery] int page = 0,
            [FromQuery] int pageSize = 20)
        {
            var pagingValidation = ValidatePagingParameters(pageSize, page);
            if (pagingValidation != null) return pagingValidation;

            var resultsPage = await _usersService.GetAllUsers(pageSize, page, email, nameContains);
            return Ok(resultsPage);
        }

        /// <summary>
        /// Returns the basic profile information for the user with a given id.
        /// </summary>
        /// <param name="id">The id of the user to get, such as `546c776b3e23f5f2ebdd3b03`</param>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _usersService.GetUserById(id);
            if (user == null)
            {
                return NotFound(new ApiError("User not found with the given ID"));
            }

            return Ok(user);
        }

        /// <summary>
        /// Returns the avatar for a user with the given id, or a default avatar if the user hasn't set one.
        /// </summary>
        /// <param name="id">The ID of the user to get the avatar for.</param>
        [HttpGet("{id}/avatar")]
        public async Task<ActionResult<UserAvatar>> GetUserAvatar(string id)
        {
            var userAvatar = await _avatarService.GetAvatarByUserId(id);
            return Ok(userAvatar);
        }

        /// <summary>
        /// Changes the users avatar to the new avatar provided. Supported image types are JPEG, GIF, and PNG.
        /// </summary>
        /// <param name="id">The id of the user.</param>
        /// <param name="avatar">The new avatar - must be less than 1MB, base 64 encoded.</param>
        [HttpPut("{id}/avatar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeUserAvatar(string id, [FromBody] UserAvatar avatar)
        {
            // Verify that the user is who they say they are - otherwise they could supply anyones avatar
            if (GetUserId() != id || avatar.UserId != id)
            {
                return Forbidden(new ApiError("You can't change another user's avatar - nice try!"));
            }

            try
            {
                await _avatarService.ChangeUserAvatar(id, avatar);
            }
            catch (ArgumentException e)
            {
                return BadRequest(new ApiError(e.Message));
            }

            return Ok();
        }

        /// <summary>
        /// Changes the user's RFID number to a new number.
        /// </summary>
        /// <remarks>
        /// This operation can only be performed on one's own account - not for other users.
        /// 
        /// Does not merge existing anonymous accounts. If the new RFID number is already in use by an anonymous
        /// account, the anonymous account will continue to exist but will no longer have an RFID associated with it.
        /// </remarks>
        /// <param name="id">The user id of the user to change the RFID number for. Must be the currently signed in user's ID.</param>
        /// <param name="rfidChangeRequest">The request DTO containing the new RFID number desired.</param>
        /// <response code="409">RFID number already in use by non-unclaimed user.</response>
        [HttpPatch("{id}/rfid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ChangeUserRfidNumber(string id, [FromBody] RfidChangeRequestDto rfidChangeRequest)
        {
            // Verify that the user is who they say they are - otherwise they could change anyone's RFID number.
            // Note that we don't actually need to check if the user exists, if their auth token is valid we know they do.
            if (GetUserId() != id)
            {
                return Forbidden(new ApiError("You can't change another user's RFID number - nice try!"));
            }

            // Attempt to change the user's RFID - but handle the case where the RFID is already in use
            try
            {
                await _rfidChangeService.ChangeRfidNumberForUser(id, rfidChangeRequest.NewRfidNumber);
                return Ok();
            }
            catch (ResourceAlreadyExistsException ex)
            {
                Log.Logger.Warning("Unable to change RFID number: {@e}", ex.ToString());
                return Conflict(new ApiError("RFID number already in use by someone else"));
            }
        }
    }
}
