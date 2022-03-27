using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using YouFoos.DataAccess.Entities.Account;
using YouFoos.DataAccess.Entities.Stats;

namespace YouFoos.DataAccess.Repositories
{
    /// <summary>
    /// MongoDB implementation of <see cref="IUsersRepository"/>.
    /// </summary>
    public class UsersRepository : IUsersRepository
    {
        private const string CollectionName = "users";

        private readonly IMongoCollection<User> _users;

        /// <summary>
        /// Constructor.
        /// </summary>
        public UsersRepository(IMongoContext mongoContext)
        {
            _users = mongoContext.GetCollection<User>(CollectionName);
        }

        /// <summary>
        /// Inserts the given user into the database.
        /// </summary>
        public Task InsertOne(User user)
        {
            return _users.InsertOneAsync(user);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IUsersRepository.CountUsers(string)"/>.
        /// </summary>
        public async Task<long> CountUsers(string nameContains)
        {
            if (!string.IsNullOrEmpty(nameContains))
            {
                return await _users.CountDocumentsAsync(user => user.FirstAndLastName.Contains(nameContains));
            }

            return await _users.CountDocumentsAsync(_ => true);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IUsersRepository.GetAllUsers(int, int, string, string)"/>.
        /// </summary>
        public Task<List<User>> GetAllUsers(int pageSize = 20, int page = 0, string email = null, string nameContains = null)
        {
            var nameFilter = Builders<User>.Filter.Where(user => user.FirstAndLastName.ToLower().Contains(nameContains.ToLower()));
            var emailFilter = Builders<User>.Filter.Where(user => user.Email.ToLower() == email.ToLower());

            var combinedFilter = Builders<User>.Filter.Empty;
            if (nameContains != null) combinedFilter &= nameFilter;
            if (email != null) combinedFilter &= emailFilter;

            return _users
                .Find(combinedFilter)
                .SortByDescending(user => user.JoinedTimestamp)
                .Skip(pageSize * page)
                .Limit(pageSize)
                .ToListAsync();
        }

        /// <summary>
        /// Concrete implementation of <see cref="IUsersRepository.GetUserWithId(string)"/>.
        /// </summary>
        public Task<User> GetUserWithId(string id)
        {
            var idFilter = Builders<User>.Filter.Eq("_id", ObjectId.Parse(id));
            return _users.Find(idFilter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Concrete implementation of <see cref="IUsersRepository.GetUserWithEmail(string)"/>.
        /// </summary>
        public Task<User> GetUserWithEmail(string email)
        {
            return _users.Find(u => u.Email.ToLower() == email.ToLower()).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Concrete implementation of <see cref="IUsersRepository.GetUserWithRfid(string)"/>.
        /// </summary>
        public Task<User> GetUserWithRfid(string rfidNumber)
        {
            return _users.Find(user => user.RfidNumber == rfidNumber).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Concrete implementation of <see cref="IUsersRepository.GetAllUserEmails"/>.
        /// </summary>
        public Task<List<string>> GetAllUserEmails()
        {
            // TODO

            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Concrete implementation of <see cref="IUsersRepository.UpsertUser(User)"/>.
        /// </summary>
        public Task UpsertUser(User user)
        {
            return _users.FindOneAndReplaceAsync(u => u.Id == user.Id, user);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IUsersRepository.Update1V1SkillForUser(string, Trueskill)"/>.
        /// </summary>
        public Task Update1V1SkillForUser(string id, Trueskill new1V1Skill)
        {
            var update = Builders<User>.Update.Set(user => user.Skill1V1, new1V1Skill);

            return _users.UpdateOneAsync(user => user.Id == id, update);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IUsersRepository.Update2V2SkillForUser(string, Trueskill)"/>.
        /// </summary>
        public Task Update2V2SkillForUser(string id, Trueskill new2V2Skill)
        {
            var update = Builders<User>.Update.Set(user => user.Skill2V2, new2V2Skill);

            return _users.UpdateOneAsync(user => user.Id == id, update);
        }

        /// <summary>
        /// Concrete implementation of <see cref="IUsersRepository.UpdateRfidForUser(string, string)"/>.
        /// </summary>
        public Task UpdateRfidForUser(string id, string newRfidNumber)
        {
            var update = Builders<User>.Update.Set(user => user.RfidNumber, newRfidNumber);
            return _users.UpdateOneAsync(user => user.Id == id, update);
        }
    }
}
