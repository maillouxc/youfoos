using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using Mongo2Go;
using MongoDB.Driver;
using YouFoos.DataAccess.Config;

namespace YouFoos.DataAccess.SharedTestUtils
{
    [ExcludeFromCodeCoverage]
    public class InMemoryMongoContext : IMongoContext, IDisposable
    {
        private static IMongoDatabase _database;

        public static MongoDbRunner Runner;

        /// <summary>
        /// Creates a connection to the in-memory Mongo2Go MongoDB server.
        /// The provided connection information in the mongoSettings param is ignored.
        /// </summary>
        // ReSharper disable once UnusedParameter.Local
        public InMemoryMongoContext(IOptions<MongoSettings> mongoSettings)
        {
            // Every time MongoDbRunner.Create() is called, a new in-memory MongoDB is created with empty data
            if (Runner == null || (Runner.State != State.Running && Runner.State != State.AlreadyRunning))
            {
                Runner = MongoDbRunner.Start();
                var mongoClient = new MongoClient(Runner.ConnectionString);
                _database = mongoClient.GetDatabase("IntegrationTestDB");
            }
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }

        public void Dispose()
        {
            Runner.Dispose();
        }
    }
}
