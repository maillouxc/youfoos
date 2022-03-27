using MongoDB.Bson.Serialization.Attributes;

namespace YouFoos.DataAccess.Entities.Stats
{
    /// <summary>
    /// Represents a single stat value for a single user at a given point in time.
    /// </summary>
    /// <typeparam name="T">The type of data the stat represents.</typeparam>
    [BsonIgnoreExtraElements]
    public class UserStatValue<T>
    {
        /// <summary>
        /// The ID of the user who that stat value is for.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The value of the statistic at the given time.
        /// </summary>
        public T StatValue { get; set; }
    }
}
