using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Serilog;
using YouFoos.Api.Dtos.Account;
using YouFoos.Exceptions;
using YouFoos.DataAccess.Entities.Account;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.Api.Services.Users
{
    /// <summary>
    /// This service is responsible for the creation of new YouFoos user accounts.
    /// </summary>
    public class AccountCreationService : IAccountCreationService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IAccountCredentialsRepository _accountCredentialsRepository;
        private readonly IEmailSender _emailSender;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AccountCreationService(
            IUsersRepository usersRepository,
            IAccountCredentialsRepository accountCredentialsRepository,
            IEmailSender emailSender)
        {
            _usersRepository = usersRepository;
            _accountCredentialsRepository = accountCredentialsRepository;
            _emailSender = emailSender;
        }

        /// <summary>
        /// Creates a new YouFoos account for the user with the provided information.
        /// </summary>
        /// <remarks>
        /// If a user with the given email address already exists, a ResourceAlreadyInUseException is thrown.
        /// 
        /// If a user with the given RFID number is found in use by an unclaimed account (which is generate
        /// by playing games with an RFID card before registering an account for the site), the unclaimed
        /// account will be merged with the users information and assigned to the user as their account.
        ///
        /// If a user with the given RFID number is found in use by a claimed account, a
        /// ResourceAlreadyExistsException is thrown.
        /// </remarks>
        public async Task<User> RegisterNewUserAccount(CreateAccountDto userInfo)
        {
            // Throw an exception if a user with the given email already exists
            if (await _usersRepository.GetUserWithEmail(userInfo.EmailAddress) != null)
            {
                throw new ResourceAlreadyExistsException("User already exists with given email.");
            }

            User newUser;

            // If there is already an existing user with the given RFID card number that is not anonymous, throw exception
            // Otherwise, transition the anonymous account into the user's new account
            var existingAccountWithRfidNumber = await _usersRepository.GetUserWithRfid(userInfo.RfidNumber);
            if (existingAccountWithRfidNumber != null)
            {
                if (existingAccountWithRfidNumber.IsUnclaimed)
                {
                    Log.Logger.Information("Merging anonymous account {@a1} for new user account request {@a2}",
                                           existingAccountWithRfidNumber, userInfo);

                    newUser = existingAccountWithRfidNumber;
                    newUser.IsUnclaimed = false;
                    newUser.FirstAndLastName = userInfo.FirstAndLastName;
                    newUser.Email = userInfo.EmailAddress;
                    await _usersRepository.UpsertUser(newUser);
                }
                else
                {
                    throw new ResourceAlreadyExistsException("RFID card already in use by another user.");
                }
            }
            else
            {
                // Initialize a new User object from scratch
                newUser = new User(userInfo.EmailAddress, userInfo.FirstAndLastName, userInfo.RfidNumber);
                await _usersRepository.InsertOne(newUser);
            }

            // Create and store the new user's credentials
            var passwordHasher = new PasswordHasher<AccountCredentials>();
            var credentials = new AccountCredentials()
            {
                Email = userInfo.EmailAddress,
                HashedPassword = passwordHasher.HashPassword(null, userInfo.Password)
            };
            await _accountCredentialsRepository.InsertNewUserCredentials(credentials);

            // Retrieve the newly created user back from mongo
            var newlyCreatedUser = await _usersRepository.GetUserWithEmail(newUser.Email);

            // Send their welcome email
            await _emailSender.SendNewUserWelcomeEmail(userInfo);

            return newlyCreatedUser;
        }
    }
}
