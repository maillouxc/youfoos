using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using YouFoos.DataAccess.Entities.Account;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.DataAccess.SharedTestUtils.TestData
{
    [ExcludeFromCodeCoverage]
    public static class TestCredentials
    {
        private static readonly PasswordHasher<AccountCredentials> PasswordHasher = 
            new PasswordHasher<AccountCredentials>();

        public static async Task InsertIntoDatabase(IMongoContext mongoContext, 
                                                    List<AccountCredentials> credentials)
        {
            var credentialsRepository = new AccountCredentialsRepository(mongoContext);
            foreach (var credential in credentials)
            {
                await credentialsRepository.InsertNewUserCredentials(credential);
            }
        }

        public static List<AccountCredentials> GetAllTestCredentials()
        {
            return new List<AccountCredentials>()
            {
                TestCreds1
            };
        }

        public static readonly string TestCreds1Password = "Keylime123!";
        public static readonly AccountCredentials TestCreds1 = new AccountCredentials
        {
            Email = TestUsers.TestUser_1.Email,
            HashedPassword = PasswordHasher.HashPassword(null, TestCreds1Password)
        };
    }
}
