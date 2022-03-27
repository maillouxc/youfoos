using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouFoos.Api.Dtos;
using YouFoos.Api.Services;
using YouFoos.DataAccess.Entities;

namespace YouFoos.Api.Controllers
{
    /// <summary>
    /// This controller is responsible for handling all achievement related API endpoints.
    /// </summary>
    [Authorize]
    [Route("api/achievements")]
    [Produces("application/json")]
    [ApiController]
    public class AchievementsController : CustomControllerBase
    {
        private readonly IAchievementsService _achievementsService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AchievementsController(IAchievementsService achievementsService)
        {
            _achievementsService = achievementsService;
        }

        /// <summary>
        /// Returns the status of all achievements for the given user.
        /// </summary>
        [HttpGet("users/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Accolade>>> GetUserAchievements(string id)
        {
            var userAchievements = await _achievementsService.GetAllAchievementsForUser(id);
            if (userAchievements == null)
            {
                return NotFound(new ApiError("No achievements found for user with the given Id."));
            }

            return Ok(userAchievements);
        }
    }
}
