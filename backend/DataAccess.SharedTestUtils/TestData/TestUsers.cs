using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using YouFoos.DataAccess;
using YouFoos.DataAccess.Entities.Account;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.TestData
{
    [ExcludeFromCodeCoverage]
    public class TestUsers
    {   
        public static async Task InsertIntoDatabase(IMongoContext mongoContext, List<User> users)
        {
            var usersRepository = new UsersRepository(mongoContext);

            foreach (var user in users)
            {
                await usersRepository.InsertOne(user);
            }
        }

        public static List<User> GetAllTestUsers()
        {
            return new List<User>()
            {
                TestUser_1,
                TestUser_2,
                TestUser_3_Anon,
                TestUser_4
            };
        }

        public static readonly User TestUser_1 = new("test@gmail.com", "Testy McTestFace", "4630118")
        {
            Id = "546c776b3e23f5f2ebdd3b01",
            JoinedTimestamp = DateTime.UtcNow.AddDays(-1)
        };

        public static readonly User TestUser_2 = new("test2@gmail.com", "Testy TheSecond", "2345678")
        {
            Id = "446c776b3e23f5f2ebdd3b02",
            JoinedTimestamp = DateTime.UtcNow.AddDays(-2)
        };

        public static readonly User TestUser_3_Anon = new(null, "Card 56789", "3456789", isUnclaimed: true)
        {
            Id = "446c776b3e23f562ebdd3b03",
            JoinedTimestamp = DateTime.UtcNow.AddDays(-3)
        };

        public static readonly User TestUser_4 = new("test4@gmail.com", "Johnny Test", "4567890")
        {
            Id = "446c776b3e23f5f2ebdd3b04",
            JoinedTimestamp = DateTime.UtcNow.AddDays(-2)
        };
    }
}
