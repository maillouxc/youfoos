using System.Collections.Generic;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.Api.Services
{
    /// <summary>
    /// Concrete implementation of <see cref="IAccoladesService"/>.
    /// </summary>
    public class AccoladesService : IAccoladesService
    {
        private readonly IAccoladesRepository _accoladesRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AccoladesService(IAccoladesRepository accoladesRepository)
        {
            _accoladesRepository = accoladesRepository;
        }

        /// <summary>
        /// Concrete implementation of <see cref="IAccoladesService.GetAllAccolades"/>.
        /// </summary>
        public async Task<List<Accolade>> GetAllAccolades()
        {
            return await _accoladesRepository.GetAccolades();
        }
    }
}
