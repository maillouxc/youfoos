using System.Threading.Tasks;
using Serilog;
using YouFoos.Exceptions;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.Api.Services.Users
{
    /// <summary>
    /// This service is responsible for changing a user's RFID card number if they get a new card.
    /// </summary>
    public class RfidChangeService : IRfidChangeService
    {
        private readonly IUsersRepository _usersRepository;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public RfidChangeService(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        /// <summary>
        /// Changes the RFID card number for the user with the given ID.
        ///
        /// If the RFID is already in use by an unclaimed user who is NOT an unclaimed anonymous account.
        /// a ResourceAlreadyExistsException is thrown.
        ///
        /// If the RFID is already in use by an unclaimed user, the unclaimed user's RFID number is set to 0.
        /// </summary>
        public async Task ChangeRfidNumberForUser(string userId, string newRfidNumber)
        {   
            // We need to ensure there are no existing users with this RFID number who aren't unclaimed accounts
            var existingUserWithRfid = await _usersRepository.GetUserWithRfid(newRfidNumber);
            if (existingUserWithRfid != null && !existingUserWithRfid.IsUnclaimed)
            {
                throw new ResourceAlreadyExistsException("Rfid already in use by another player.");           
            }

            if (existingUserWithRfid != null && existingUserWithRfid.IsUnclaimed)
            {
                // Set the existing unclaimed user's RFID to 0
                Log.Logger.Information("Setting RFID number for unclaimed user {@uc} to 0", existingUserWithRfid);
                await _usersRepository.UpdateRfidForUser(existingUserWithRfid.Id, "0");
            }

            Log.Logger.Information("Changing user {@id} RFID number to {@new}", userId, newRfidNumber);
            await _usersRepository.UpdateRfidForUser(userId, newRfidNumber);
        }
    }
}
