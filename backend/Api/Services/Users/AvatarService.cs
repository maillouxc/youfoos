using System;
using System.Linq;
using System.Threading.Tasks;
using YouFoos.Api.Utilities;
using YouFoos.DataAccess.Entities.Account;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.Api.Services.Users
{
    /// <summary>
    /// Concrete implementation of <see cref="IAvatarService"/>.
    /// </summary>
    public class AvatarService : IAvatarService
    {
        private readonly IAvatarsRepository _avatarsRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AvatarService(IAvatarsRepository avatarsRepository)
        {
            _avatarsRepository = avatarsRepository;
        }

        /// <summary>
        /// Concrete implementation of <see cref="IAvatarService.GetAvatarByUserId(string)"/>.
        /// </summary>
        public async Task<UserAvatar> GetAvatarByUserId(string id)
        {
            var userAvatar = await _avatarsRepository.GetAvatarByUserId(id);
            
            if (userAvatar == null)
            {
                var defaultAvatar = DefaultAvatarLoader.GetDefaultAvatar();
                return new UserAvatar(id, "image/png", defaultAvatar);
            }

            return userAvatar;
        }

        /// <summary>
        /// Concrete implementation of <see cref="IAvatarService.ChangeUserAvatar(string, UserAvatar)"/>.
        /// </summary>
        public async Task ChangeUserAvatar(string userId, UserAvatar avatar)
        {
            // Validate the image's MIME type to ensure it is an allowed type
            var validMimeTypes = new[] { "image/jpeg", "image/gif", "image/png" };
            if (!validMimeTypes.Contains(avatar.MimeType))
            {
                throw new ArgumentException("Invalid avatar image mime type.");
            }

            // Ensure that the file size is <= 1 MB - we have to decode it into binary first
            var fileSizeBytes = Convert.FromBase64String(avatar.Base64Image).Length;
            if (fileSizeBytes > 1_048_576)
            {
                throw new ArgumentException($"Avatar too large - must be less than 1MB. Actual size was {fileSizeBytes}");
            }

            // If the user doesn't have an existing avatar yet, create this as a new one
            var currentAvatar = await _avatarsRepository.GetAvatarByUserId(avatar.UserId);
            if (currentAvatar == null)
            {
                var newAvatar = new UserAvatar(userId, avatar.MimeType, avatar.Base64Image);
                await _avatarsRepository.InsertOneAsync(newAvatar);
            }
            else
            {
                // Otherwise, replace their existing avatar
                currentAvatar.MimeType = avatar.MimeType;
                currentAvatar.Base64Image = avatar.Base64Image;
                await _avatarsRepository.ReplaceAvatar(currentAvatar);
            }
        }
    }
}
