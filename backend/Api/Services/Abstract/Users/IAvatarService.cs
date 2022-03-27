using System.Threading.Tasks;
using YouFoos.DataAccess.Entities.Account;

namespace YouFoos.Api.Services.Users
{
    /// <summary>
    /// Business logic class for working with user avatars.
    /// </summary>
    public interface IAvatarService
    {
        /// <summary>
        /// Returns the avatar for the user with the given ID, or null if not found.
        /// </summary>
        Task<UserAvatar> GetAvatarByUserId(string id);

        /// <summary>
        /// Replaces avatar for the given user with the new avatar.
        /// </summary>
        Task ChangeUserAvatar(string userId, UserAvatar newAvatar);
    }
}
