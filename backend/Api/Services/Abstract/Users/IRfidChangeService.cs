using System.Threading.Tasks;

namespace YouFoos.Api.Services.Users
{
    /// <summary>
    /// Business logic class for handling user RFID card number changes.
    /// </summary>
    public interface IRfidChangeService
    {
        /// <summary>
        /// Changes the RFID card number for the given user.
        /// </summary>
        Task ChangeRfidNumberForUser(string userId, string newRfidNumber);
    }
}
