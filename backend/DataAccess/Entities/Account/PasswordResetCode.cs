using System;
using MongoDB.Bson;

namespace YouFoos.DataAccess.Entities.Account
{
    /// <summary>
    /// A code used to reset a user's password. This is a short lived entity.
    /// </summary>
    public class PasswordResetCode
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PasswordResetCode(string userEmail, string code)
        {
            UserEmail = userEmail;
            Code = code;
            Created = DateTime.UtcNow;
        }

        /// <summary>
        /// Only used so we can stick these reset code objects into MongoDB without errors.
        /// </summary>
        public ObjectId Id { get; set; }

        /// <summary>
        /// The email address of the user that the code was generated for.
        /// </summary>
        public string UserEmail { get; set; }
        
        /// <summary>
        /// The text of the code itself.
        /// </summary>
        public string Code { get; set; }
        
        /// <summary>
        /// When the reset code was generated.
        /// </summary>
        public DateTime Created { get; set; }
    }
}
