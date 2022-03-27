using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using YouFoos.DataAccess.Entities;

namespace YouFoos.DataAccess.Repositories
{
    /// <summary>
    /// MongoDB implementation of <see cref="IAccoladesRepository"/>.
    /// </summary>
    public class AccoladesRepository : IAccoladesRepository
    {
        private const string CollectionName = "accolades";

        private readonly IMongoCollection<Accolade> _accolades;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AccoladesRepository(IMongoContext mongoContext)
        {
            _accolades = mongoContext.GetCollection<Accolade>(CollectionName);
        }

        /// <summary>
        /// Returns all accolades in the system.
        /// </summary>
        public async Task<List<Accolade>> GetAccolades()
        {
            return await _accolades.Find(_ => true).ToListAsync();
        }

        /// <summary>
        /// Upserts the given list of accolades into the database.
        /// </summary>
        public async Task InsertAccolades(List<Accolade> accolades)
        {
            var bulkUpsert = new List<WriteModel<Accolade>>();
            foreach (var accolade in accolades)
            {
                var filter = Builders<Accolade>.Filter.Where(o => o.Name == accolade.Name);
                bulkUpsert.Add(new ReplaceOneModel<Accolade>(filter, accolade) { IsUpsert = true });
            }

            await _accolades.BulkWriteAsync(bulkUpsert);
        }
    }
}
