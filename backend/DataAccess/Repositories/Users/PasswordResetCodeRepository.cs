using System.Threading.Tasks;
using MongoDB.Driver;
using YouFoos.DataAccess.Entities.Account;

namespace YouFoos.DataAccess.Repositories
{
    /// <summary>
    /// MongoDB implementation of <see cref="IPasswordResetCodeRepository"/>.
    /// </summary>
    public class PasswordResetCodeRepository : IPasswordResetCodeRepository
    {
        private const string CollectionName = "password_reset_codes";

        private readonly IMongoCollection<PasswordResetCode> _resetCodes;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PasswordResetCodeRepository(IMongoContext mongoContext)
        {
            _resetCodes = mongoContext.GetCollection<PasswordResetCode>(CollectionName);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IPasswordResetCodeRepository.InsertOne(PasswordResetCode)"/>.
        /// </summary>
        public Task InsertOne(PasswordResetCode resetCode)
        {
            return _resetCodes.InsertOneAsync(resetCode);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IPasswordResetCodeRepository.GetResetCodeForUserWithEmail(string)"/>.
        /// </summary>
        public Task<PasswordResetCode> GetResetCodeForUserWithEmail(string email)
        {
            return _resetCodes.Find(code => code.UserEmail.ToLower() == email.ToLower())
                              .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Concrete implementation of <see cref="IPasswordResetCodeRepository.ReplaceOne(PasswordResetCode)"/>.
        /// </summary>
        public Task ReplaceOne(PasswordResetCode resetCode)
        {
            return _resetCodes.ReplaceOneAsync(
                existing => existing.UserEmail.ToLower() == resetCode.UserEmail.ToLower(),
                resetCode
            );
        }

        /// <summary>
        /// Concrete implementation of <see cref="IPasswordResetCodeRepository.DeleteResetCodeForUserWithEmail(string)"/>.
        /// </summary>
        public Task DeleteResetCodeForUserWithEmail(string email)
        {
            return _resetCodes.DeleteManyAsync(code => code.UserEmail.ToLower() == email.ToLower());
        }
    }
}
