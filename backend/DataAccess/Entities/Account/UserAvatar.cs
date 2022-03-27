using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace YouFoos.DataAccess.Entities.Account
{
    /// <summary>
    /// Represents a user's avatar image (aka profile picture).
    /// </summary>
    public class UserAvatar
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public UserAvatar(string userId, string mimeType, string base64Image)
        {
            UserId = userId;
            MimeType = mimeType;
            Base64Image = base64Image;
        }

        /// <summary>
        /// This field is only needed to be able to stick this object in the database.
        /// </summary>
        [JsonIgnore]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// The ID of the user that the avatar is for.
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// The internet media type of the image.
        /// </summary>
        [Required]
        public string MimeType { get; set; }

        /// <summary>
        /// The image itself, as a base 64 encoded string.
        /// </summary>
        [Required]
        public string Base64Image { get; set; }
    }
}
