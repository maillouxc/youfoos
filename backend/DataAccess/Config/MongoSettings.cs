using System.Diagnostics.CodeAnalysis;

namespace YouFoos.DataAccess.Config
{
    /// <summary>
    /// Configuration object that holds settings that are used to configure MongoDB related code.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class MongoSettings
    {
        /// <summary>
        /// The connection string used to connect to the database.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// The name of the MongoDB database to connect to.
        /// </summary>
        public string Database { get; set; }
    }
}
