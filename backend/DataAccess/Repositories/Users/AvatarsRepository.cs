using System.Threading.Tasks;
using MongoDB.Driver;
using YouFoos.DataAccess.Entities.Account;

namespace YouFoos.DataAccess.Repositories
{
    /// <summary>
    /// MongoDB implementation of <see cref="IAvatarsRepository"/>.
    /// </summary>
    public class AvatarsRepository : IAvatarsRepository
    {
        private const string CollectionName = "avatars";

        private readonly IMongoCollection<UserAvatar> _avatars;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AvatarsRepository(IMongoContext mongoContext)
        {
            _avatars = mongoContext.GetCollection<UserAvatar>(CollectionName);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IAvatarsRepository.InsertOneAsync(UserAvatar)"/>.
        /// </summary>
        public async Task InsertOneAsync(UserAvatar avatar)
        {
            await _avatars.InsertOneAsync(avatar);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IAvatarsRepository.GetAvatarByUserId(string)"/>.
        /// </summary>
        public Task<UserAvatar> GetAvatarByUserId(string id)
        {
            return _avatars.Find(avatar => avatar.UserId == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Concrete implementation of <see cref="IAvatarsRepository.ReplaceAvatar(UserAvatar)"/>.
        /// </summary>
        public async Task ReplaceAvatar(UserAvatar avatar)
        {
            await _avatars.ReplaceOneAsync(a => a.UserId == avatar.UserId, avatar);
        }
    }
}
