using System.Threading.Tasks;
using YouFoos.DataAccess.Entities.Account;

namespace YouFoos.DataAccess.Repositories
{
    /// <summary>
    /// Data access class for working with user avatars images.
    /// </summary>
    public interface IAvatarsRepository
    {
        /// <summary>
        /// Inserts the given avatar into the database.
        /// </summary>
        Task InsertOneAsync(UserAvatar avatar);

        /// <summary>
        /// Returns the avatar for the user with the given ID.
        /// </summary>
        Task<UserAvatar> GetAvatarByUserId(string id);

        /// <summary>
        /// Replaces the given avatar in the database with the provided updated version.
        /// </summary>
        Task ReplaceAvatar(UserAvatar avatar);
    }
}
