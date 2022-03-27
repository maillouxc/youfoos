using System.Collections.Generic;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities.Account;
using YouFoos.DataAccess.Entities.Stats;

namespace YouFoos.DataAccess.Repositories
{
    /// <summary>
    /// Repository class for managing user account data.
    /// </summary>
    public interface IUsersRepository
    {
        /// <summary>
        /// Inserts the given user into the database.
        /// </summary>
        Task InsertOne(User user);

        /// <summary>
        /// Returns the total number of user accounts.
        /// 
        /// If a nameContains param is specified, searches only for users whose name contains the given substring.
        /// </summary>
        Task<long> CountUsers(string nameContains = null);

        /// <summary>
        /// Returns all users.
        /// </summary>
        Task<List<User>> GetAllUsers(int pageSize = 20, int page = 0, string email = null, string nameContains = null);

        /// <summary>
        /// Returns the user with the given ID or null if not found.
        /// </summary>
        Task<User> GetUserWithId(string id);

        /// <summary>
        /// Returns the user with the given email address (case-insensitive) or null if not found.
        /// </summary>
        Task<User> GetUserWithEmail(string email);

        /// <summary>
        /// Returns the user with the given RFID card number, or null if not found.
        /// </summary>
        Task<User> GetUserWithRfid(string rfidNumber);

        /// <summary>
        /// Returns the email address of every user in the system.
        /// </summary>
        Task<List<string>> GetAllUserEmails();

        /// <summary>
        /// Upserts the given user into the database.
        /// </summary>
        Task UpsertUser(User user);

        Task Update1V1SkillForUser(string id, Trueskill new1V1Skill);

        Task Update2V2SkillForUser(string id, Trueskill new2V2Skill);

        /// <summary>
        /// Updates the RFID card number for the given user.
        /// </summary>
        Task UpdateRfidForUser(string id, string newRfidNumber);
    }
}
