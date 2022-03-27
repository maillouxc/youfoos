using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YouFoos.Api.Services;
using YouFoos.DataAccess.Entities;

namespace YouFoos.Api.Controllers
{
    /// <summary>
    /// This controller is responsible for handling all accolade related info.
    ///
    /// You can think of accolades as a sort of superlative stat.
    /// Some examples include the player with the highest rank, or the most time played.
    /// </summary>
    [Authorize]
    [Route("api/accolades")]
    [Produces("application/json")]
    [ApiController]
    public class AccoladesController : CustomControllerBase
    {
        private readonly IAccoladesService _accoladesService;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AccoladesController(IAccoladesService accoladesService)
        {
            _accoladesService = accoladesService;
        }

        /// <summary>
        /// Returns all accolades.
        /// </summary>
        /// <returns>The list of accolades</returns>
        [HttpGet]
        public async Task<ActionResult<List<Accolade>>> GetAllAccolades()
        {
            var accolades = await _accoladesService.GetAllAccolades();
            return Ok(accolades);
        }
    }
}
