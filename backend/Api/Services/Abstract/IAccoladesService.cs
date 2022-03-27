using System.Collections.Generic;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities;

namespace YouFoos.Api.Services
{
    /// <summary>
    /// Business logic class for interacting with user accolades, which are displayed in the hall of fame.
    /// </summary>
    public interface IAccoladesService
    {
        /// <summary>
        /// Returns all accolades for the system.
        /// </summary>
        Task<List<Accolade>> GetAllAccolades();
    }
}
