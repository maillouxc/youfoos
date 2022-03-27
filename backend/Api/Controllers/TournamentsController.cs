using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YouFoos.Api.Dtos;
using YouFoos.Api.Exceptions;
using YouFoos.Api.Services;
using YouFoos.DataAccess.Entities;
using YouFoos.SharedLibrary.Exceptions;

namespace YouFoos.Api.Controllers
{
    /// <summary>
    /// This controller is responsible for managing tournaments.
    /// </summary>
    [Authorize]
    [Route("api/tournaments")]
    [Produces("application/json")]
    [ApiController]
    public class TournamentsController : CustomControllerBase
    {
        private readonly ITournamentsService _tournamentsService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TournamentsController(ITournamentsService tournamentsService)
        {
            _tournamentsService = tournamentsService;
        }

        /// <summary>
        /// Creates a new tournament.
        /// </summary>
        /// <remarks>
        /// Only admins can create tournaments - anyone else who tries will receive a 403 Forbidden response.<br/>
        /// 
        /// Only one tournament may be scheduled or in progress at a given time - a 400 response will be returned if 
        /// there is already another one. <br/>
        /// 
        /// The newly created tournament is returned.
        /// </remarks>
        /// <param name="request">The request object containing the configuration for the new tournament.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Tournament>> CreateTournament(CreateTournamentRequest request)
        {
            if (!IsAdmin())
            {
                return Forbidden(new ApiError("Only Admins can create tournaments."));
            }

            string userId = GetUserId();

            try
            {
                var newTournament = await _tournamentsService.CreateTournament(request, userId);
                return Created("/{id}/" + newTournament.Id, newTournament);
            }
            catch (YouFoosException ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Signs a user up to play in a tournament.
        /// </summary>
        /// <remarks>
        /// Players can only sign up themselves - anyone else trying will receive a 403 Forbidden response.
        /// </remarks>
        /// <param name="tournamentId">The ID of the tournament to sign the player up for.</param>
        /// <param name="userId">The ID of the user to sign up.</param>
        [HttpPost("{tournamentId}/users/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RegisterForTournament(string tournamentId, string userId)
        {
            // TODO ensure a user can only register themselves, unless they are an admin (they can register other users)

            Guid tournamentGuid = Guid.Parse(tournamentId);

            try
            {
                await _tournamentsService.RegisterForTournament(tournamentGuid, userId);
                return Ok();
            }
            catch (YouFoosException ex)
            {
                return BadRequest(new ApiError(ex.Message));
            }
            catch (ResourceNotFoundException ex)
            {
                return NotFound(new ApiError(ex.Message));
            }
        }

        /// <summary>
        /// Returns the most recently played tournaments.
        /// </summary>
        /// <param name="pageNumber">The 0-indexed page number of results to return.</param>
        /// <param name="pageSize">The number of tournaments per page to return - up to 100.</param>
        /// <returns>The most recently played tournaments, as a PaginatedResult object.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaginatedResult<Tournament>>> GetRecentTournaments(
            [FromQuery] int pageNumber = 0,
            [FromQuery] int pageSize = 10
        )
        {
            var pagingValidation = ValidatePagingParameters(pageSize, pageNumber);
            if (pagingValidation != null) return pagingValidation;

            var tournamentsPage = await _tournamentsService.GetRecentTournaments(pageSize, pageNumber);

            return Ok(tournamentsPage);
        }

        /// <summary>
        /// Gets the tournament with the given ID.
        /// </summary>
        /// <param name="id">The ID of the tournament to get.</param>
        /// <returns>The Tournament with the given ID.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Tournament>> GetTournament(Guid id)
        {
            var tournament = await _tournamentsService.GetTournamentById(id);

            if (tournament == null)
            {
                return NotFound(new ApiError("The tournament with the given ID was not found."));
            }

            return Ok(tournament);
        }

        /// <summary>
        /// Returns the tournament currently in progress, if there is one, or null if not.
        /// </summary>
        /// <returns>The currently in progress tournament.</returns>
        [HttpGet("current")]
        public async Task<ActionResult<Tournament>> GetCurrentTournament()
        {
            var currentTournament = await _tournamentsService.GetCurrentTournament();

            if (currentTournament == null)
            {
                return NotFound(new ApiError("There is no tournament currently in progress."));
            }

            return Ok(currentTournament);
        }
    }
}
