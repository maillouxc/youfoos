using System.Threading.Tasks;
using Serilog;
using YouFoos.DataAccess.Entities.Account;
using YouFoos.DataAccess.Repositories;
using YouFoos.GameEventsService.Utilities;

namespace YouFoos.GameEventsService.Services
{
    /// <summary>
    /// This class is responsible for resolving an rfid number in the form of the full UID shifted
    /// and padded bytes as returned directly from the card reader, and then finding the user
    /// in the database associated with this particular RFID card, or creating a new anonymous
    /// user account if that user is not found.
    /// </summary>
    public class RfidToUserAccountResolverService : IRfidToUserAccountResolverService
    {
        private readonly IUsersRepository _usersRepository;

        public RfidToUserAccountResolverService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        /// <summary>
        /// Returns the user with the given RFID if found, else returns null.
        /// </summary>
        /// <remarks>
        /// We need this method in addition to the variant which creates the anonymous account if the
        /// user is not found because this one is called when goals are scored, for instance. There are
        /// sometimes cases when we want to retrieve the user without necessarily creating a new anonymous
        /// account. Rather than throwing an exception here, we just propagate the null and let the caller
        /// decide what to do with it.
        /// </remarks>
        /// <param name="rfidBytes">The RFID UID bytes as returned directly from the card reader.</param>
        public async Task<User> GetUserWithRfid(string rfidBytes)
        {
            string rfidNumber = RfidConverter.ConvertRfidUidToNumberOnCard(rfidBytes);
            return await _usersRepository.GetUserWithRfid(rfidNumber);
        }

        /// <summary>
        /// Returns the user with the given RFID if found, else creates a new anonymous user account
        /// with that RFID number.
        /// </summary>
        /// <param name="rfidBytes">The RFID UID bytes as returned directly from the card reader.</param>
        public async Task<User> GetUserWithRfidOrCreateNewAnonymousUser(string rfidBytes)
        {
            string rfidNumber = RfidConverter.ConvertRfidUidToNumberOnCard(rfidBytes);
            var userFoundWithRfidNumber = await _usersRepository.GetUserWithRfid(rfidNumber);
            if (userFoundWithRfidNumber == null)
            {
                return await CreateNewAnonymousUser(rfidNumber);
            }

            return userFoundWithRfidNumber;
        }

        private async Task<User> CreateNewAnonymousUser(string rfidNumber)
        {
            var anonUserName = $"Card {rfidNumber}";
            var anonymousUser = new User(null, anonUserName, rfidNumber, true);
 
            Log.Logger.Information("Creating new anonymous user account {@a}", anonymousUser);
            await _usersRepository.InsertOne(anonymousUser);
            var newlyCreatedAnonymousUser = await _usersRepository.GetUserWithRfid(rfidNumber);
            return newlyCreatedAnonymousUser;
        }
    }
}
