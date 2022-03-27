﻿using System.Collections.Generic;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities;

 namespace YouFoos.DataAccess.Repositories
{
    /// <summary>
    /// Data access class for working with accolades.
    /// </summary>
    public interface IAccoladesRepository
    {
        /// <summary>
        /// Returns all accolades for the system.
        /// </summary>
        Task<List<Accolade>> GetAccolades();

        /// <summary>
        /// Upserts all accolades for the system.
        /// </summary>
        Task InsertAccolades(List<Accolade> accolades);
    }
}
