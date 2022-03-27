#pragma warning disable 1591 // Disable XML comments being required

using MongoDB.Driver;

namespace YouFoos.DataAccess
{    
    /// <summary>
    /// Database context object that encapsulates the needed code for setting up MongoDB connections and obtaining handles to collections.
    /// </summary>
    public interface IMongoContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
    }
}
