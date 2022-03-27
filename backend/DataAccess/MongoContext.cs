using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using YouFoos.DataAccess.Config;

namespace YouFoos.DataAccess
{
    /// <summary>
    /// Concrete implementation of <see cref="IMongoContext"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MongoContext : IMongoContext
    {
        private readonly IMongoDatabase _database;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MongoContext(IOptions<MongoSettings> mongoSettings)
        {
            var connectionString = mongoSettings.Value.ConnectionString;
            var databaseName = mongoSettings.Value.Database;

            IMongoClient mongoClient = new MongoClient(connectionString);
            _database = mongoClient.GetDatabase(databaseName);
        }

        /// <summary>
        /// Returns the MongoDB collection with the given name.
        /// </summary>
        /// <typeparam name="T">The type of object that is stored in the Mongo collection.</typeparam>
        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }
}
