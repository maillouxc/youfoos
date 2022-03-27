using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace YouFoos.DataAccess.Entities.Stats
{
    /// <summary>
    /// Represents a single user's stats at a single point in time.
    /// 
    /// These objects are created after every game and can be compared for determining stat value changes.
    /// </summary>
    public class UserStats
    {
        /// <summary>
        /// This field is only required to store this in MongoDb.
        ///
        /// There is probably a better way to do this, and we should investigate that at some point.
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// The user's ID who these stats are for.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The 1v1 stats of the user.
        /// </summary>
        public Stats1V1 Stats1V1 { get; set; }
        
        /// <summary>
        /// The 2v2 stats of the user.
        /// </summary>
        public Stats2V2 Stats2V2 { get; set; }

        /// <summary>
        /// The overall stats of the user.
        /// </summary>
        public StatsOverall StatsOverall { get; set; }

        /// <summary>
        /// A timestamp of when the stats were calculated.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
