using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using YouFoos.DataAccess.Entities.Stats;

namespace YouFoos.DataAccess.Entities.Account
{
    /// <summary>
    /// A YouFoos user account.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class User
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public User(string email, string firstAndLastName, string rfidNumber, bool isUnclaimed = false)
        {
            Email = email;
            FirstAndLastName = firstAndLastName;
            RfidNumber = rfidNumber;
            JoinedTimestamp = DateTime.UtcNow;
            IsUnclaimed = isUnclaimed;
            Skill1V1 = new Trueskill();
            Skill2V2 = new Trueskill();
        }

        /// <summary>
        /// The ID of the user.
        /// </summary>
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string Id { get; set; }

        /// <summary>
        /// The email address of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The user's name.
        /// </summary>
        public string FirstAndLastName { get; set; }

        /// <summary>
        /// The number of the user's RFID card.
        /// </summary>
        public string RfidNumber { get; set; }

        /// <summary>
        /// When the user's account was created.
        /// </summary>
        public DateTime JoinedTimestamp { get; set; }

        /// <summary>
        /// Whether the user is an anonymous account created automatically by the system when an unrecognized RFID card plays a game.
        /// </summary>
        public bool IsUnclaimed { get; set; }

        /// <summary>
        /// True, if the user is a YouFoos system administrator.
        /// </summary>
        public bool IsAdmin { get; set; } = false;

        /// <summary>
        /// The user's Trueskill for 1v1 mode.
        /// </summary>
        public Trueskill Skill1V1 { get; set; }
        
        /// <summary>
        /// The users's Trueskill for 2v2 mode.
        /// </summary>
        public Trueskill Skill2V2 { get; set; }
    }
}
