using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YouFoos.Api.Dtos;
using YouFoos.Api.Services;
using YouFoos.Api.Services.Users;
using YouFoos.DataAccess.Entities;

namespace YouFoos.Api.Controllers
{
    /// <summary>
    /// This controller is responsible for handling info related to games.
    ///
    /// This includes returning information on the current game, recent games, or any game in particular.
    /// </summary>
    [Authorize]
    [Route("api/games")]
    [Produces("application/json")]
    [ApiController]
    public class GamesController : CustomControllerBase
    {
        private readonly IGamesService _gamesService;
        private readonly IUsersService _usersService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GamesController(IGamesService gamesService, IUsersService usersService)
        {
            _gamesService = gamesService;
            _usersService = usersService;
        }

        /// <summary>
        /// Returns the game with a given ID.
        /// </summary>
        /// <param name="id">The GUID of the game to get data from.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Game>> GetGame(string id)
        {            
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new ApiError("The ID cannot be null or empty."));
            }
            if (!Guid.TryParse(id, out var guid))
            {
                return BadRequest(new ApiError("The ID provided doesn't follow a GUID format."));
            }

            var game = await _gamesService.GetGameById(guid);
            if (game == null)
            {
                return NotFound(new ApiError("The game with the given ID was not found."));
            }

            return Ok(game);
        }

        /// <summary>
        /// Returns the currently ongoing game if there is one, or null if not.
        /// </summary>
        [HttpGet("current")]
        public async Task<ActionResult<Game>> GetCurrentGame()
        {
            var currentGame = await _gamesService.GetCurrentGame();
            return Ok(currentGame);
        }

        /// <summary>
        /// Returns the list of the 10 most recent games.
        /// </summary>
        [HttpGet("recent")]
        public async Task<ActionResult<List<Game>>> GetRecentGames()
        {
            int numGames = 10;
            var recentGamesList = await _gamesService.GetRecentGames(numGames);
            return Ok(recentGamesList);
        }

        /// <summary>
        /// Returns a list of the 10 most recent games by user ID.
        /// </summary>
        /// <param name="id">The user's ID.</param>
        [HttpGet("recent/user/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Game>>> GetRecentGamesForUser(string id)
        {
            int numGames = 10;

            if (string.IsNullOrEmpty(id))
            {
                return BadRequest(new ApiError("The ID cannot be null or empty."));
            }

            var user = await _usersService.GetUserById(id);
            if (user == null)
            {
                return NotFound(new ApiError("The user ID was not found in the database."));
            }

            var recentGamesList = await _gamesService.GetRecentGamesByUserId(id, numGames);

            return Ok(recentGamesList);
        }
    }
}
