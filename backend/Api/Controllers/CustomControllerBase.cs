using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using YouFoos.Api.Dtos;

namespace YouFoos.Api.Controllers
{
    /// <summary>
    /// A base class containing code that should be shared by all YouFoos API controllers.
    /// </summary>
    public class CustomControllerBase : ControllerBase
    {
        /// <summary>
        /// Returns a 403 forbidden response which includes the provided object.
        /// </summary>
        [NonAction]
        protected ObjectResult Forbidden(object value)
        {
            return StatusCode(403, value);
        }

        /// <summary>
        /// Returns a 400 bad request response if the pagination parameters provided are outside of the allowable range.
        /// </summary>
        [NonAction]
        protected ObjectResult ValidatePagingParameters(int pageSize, int pageNumber)
        {
            if (pageSize < 0 || pageSize > 100)
            {
                return BadRequest(new ApiError("pageSize must be between 0 and 100."));
            }

            if (pageNumber < 0)
            {
                return BadRequest(new ApiError("pageNumber must be greater than 0."));
            }

            return null;
        }

        /// <summary>
        /// Returns true if the currently authenticated user is a system administrator.
        /// </summary>
        [NonAction]
        protected bool IsAdmin()
        {
            var userClaims = ((ClaimsIdentity)User.Identity).Claims;
            string isAdminBool = userClaims.Where(claim => claim.Type == "IsAdmin")
                                           .Select(claim => claim.Value)
                                           .SingleOrDefault();

            return (!string.IsNullOrEmpty(isAdminBool) && isAdminBool == "true");
        }

        /// <summary>
        /// Returns the ID of the currently authenticated user.
        /// </summary>
        [NonAction]
        protected string GetUserId()
        {
            var userClaims = ((ClaimsIdentity)User.Identity).Claims;
            return userClaims.Where(claim => claim.Type == "Id")
                             .Select(claim => claim.Value)
                             .SingleOrDefault();
        }

        /// <summary>
        /// Returns the email address of the currently authenticated user.
        /// </summary>
        [NonAction]
        protected string GetUserEmail()
        {
            var userClaims = ((ClaimsIdentity)User.Identity).Claims;
            return userClaims.Where(claim => claim.Type == "Email")
                             .Select(claim => claim.Value)
                             .SingleOrDefault();
        }
    }
}
