using MongoDB.Bson;

namespace YouFoos.DataAccess.Entities.Account
{
    /// <summary>
    /// Represents the credentials that a user uses to login to YouFoos.
    /// </summary>
    public class AccountCredentials
    {
        /// <summary>
        /// Only used so we can stick these credentials into MongoDB without errors
        /// </summary>
        public ObjectId Id { get; set; }
  
        /// <summary>
        /// The email address of the user account.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The securly hashed password of the user.
        /// </summary>
        public string HashedPassword { get; set; }
    }
}
