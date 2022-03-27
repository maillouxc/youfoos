using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YouFoos.Api.Dtos;
using YouFoos.Api.Services;
using YouFoos.DataAccess.Entities.Enums;
using YouFoos.DataAccess.Entities.Stats;

namespace YouFoos.Api.Controllers
{
    /// <summary>
    /// This controller is responsible for retrieving and returning stats related data.
    ///
    /// This includes data needed to construct leaderboards, historic values of particular stats,
    /// stats for particular users, etc.
    /// </summary>
    [Authorize]
    [Route("api/stats")]
    [Produces("application/json")]
    [ApiController]
    public class StatsController : CustomControllerBase
    {
        private readonly IStatsService _statsService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        /// <summary>
        /// Returns the most recent stats for the user with the given ID.
        /// </summary>
        /// <param name="id">The user's ID</param>
        /// <param name="cutoffDate">The cutoff date for when to return the stats from. If provided, the most recent stats for the user
        /// that were recorded before this date will be returned, or null if there aren't any</param>
        /// <returns>A UserStatsDto with the user's statistics.</returns>
        [HttpGet("users/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserStats>> GetByIdUserStats(string id, [FromQuery] DateTime? cutoffDate = null)
        {
            var stats = await _statsService.GetStatsForUserWithId(id, cutoffDate);
            if (stats == null)
            {
                return NotFound(new ApiError("No stats found for user with the given Id."));
            }

            return Ok(stats);
        }

        /// <summary>
        /// Returns a list of user stats, sorted by a chosen stat. Generally used to implement a leaderboard.
        /// </summary>
        /// <remarks>
        /// The stats are passed as a string based on their name in the user stats object, with the mode appended to the name.
        /// For example, to sort by 1V1 games won, the endpoint URL should be <code>/api/stats/users?sortBy=GamesWon&amp;statCategory=1V1</code>
        ///
        /// Not all combinations of `sortBy` and `statCategory` are valid.
        ///
        /// Only combinations that can be found within the stats objects themselves are valid.
        /// For instance, Stats1V1 doesn't contain a field `Rank`, so passing the endpoint values of 
        /// `statCategory=1V1&amp;sortBy=Rank` is invalid.
        /// 
        /// Importantly, this endpoint does NOT yet validate these combinations.
        /// If you try to use an unsupported combination, results are undefined.
        /// </remarks>
        /// <returns>A list of `UserStats` objects, sorted in the specified order.</returns>
        /// <param name="sortBy">The name of the stat to sort the results by, descending. Default is by rank.
        /// These are enum values - you should pass the name of the enum value.
        /// </param>
        /// <param name="statCategory">For sorting stats - Must be either "Overall", "1V1", or "2V2"</param>
        /// <param name="pageSize">The number of results to return</param>
        /// <param name="pageNumber">The 0-indexed page number of results to return</param>
        /// <param name="userId">If a valid user ID is specified, returns only the leaderboard page containing the user with the given ID</param>
        [HttpGet("users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaginatedResult<UserStats>>> GetStatsForLeaderboard(
                [FromQuery] string sortBy = "Rank",
                [FromQuery] StatsCategory statCategory = StatsCategory.StatsOverall,
                [FromQuery] int pageSize = 10,
                [FromQuery] int pageNumber = 0,
                [FromQuery] string userId = null)
        {
            try
            {
                var pagingValidation = ValidatePagingParameters(pageSize, pageNumber);
                if (pagingValidation != null) return pagingValidation;

                var leaderboardPage = await _statsService.GetLeaderboardPage(sortBy, statCategory, pageSize, pageNumber, userId);

                if (leaderboardPage == null)
                {
                    if (!string.IsNullOrEmpty(userId))
                    {
                        return NotFound(new ApiError("Leaderboard entry for user with given ID not found."));
                    }

                    return NotFound(new ApiError("No stats for leaderboard available."));
                }

                return Ok(leaderboardPage);
 
            }
            catch (ArgumentException e)
            {
                return BadRequest(new ApiError(e.Message));
            }
        }

        /// <summary>
        /// Returns the history of a specified stat's value, for the purposes of graphing it.
        /// </summary>
        /// <remarks>
        /// Not all combinations of `statName` and `statCategory` are valid.
        ///
        /// Only combinations that can be found within the stats objects themselves are valid.
        /// For instance, Stats1V1 doesn't contain a field `Rank`, so passing the endpoint values of
        /// `statCategory=1V1&amp;statName=Rank` is invalid.
        /// 
        /// Importantly, this endpoint does NOT yet validate these combinations.
        /// If you try to use an unsupported combination, results are undefined.
        /// </remarks>
        /// <param name="id">The ID of the user to retrieve the stat history for</param>
        /// <param name="statName">The name of the statistic to retrieve the history for</param>
        /// <param name="statCategory">Must be either "Overall", "1V1", or "2V2"</param>
        /// <param name="sampleInterval">Either "Daily" (default), "Weekly", "Monthly", or "Yearly"</param>
        /// <param name="startDate">The date to begin the history range for</param>
        /// <param name="endDate">The date to end the history range for</param>
        /// <returns>A `StatHistoryDto` containing the a list of historical data points for the stat</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("users/{id}/history")]
        public async Task<ActionResult<List<StatHistoryDataPoint>>> GetStatHistory(
                string id, 
                [FromQuery] string statName,
                [FromQuery] StatsCategory statCategory,
                [FromQuery] DateTime startDate,
                [FromQuery] DateTime endDate,
                [FromQuery] StatSampleInterval sampleInterval = StatSampleInterval.Daily)
        {
            try
            {
                var statHistory = await _statsService.GetStatHistoryForUser(
                    id, statName, statCategory, startDate, endDate, sampleInterval);

                return Ok(statHistory);
            }
            catch (ArgumentException e)
            {
                return BadRequest(new ApiError(e.Message));
            }
        }
    }
}
