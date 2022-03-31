using System;
using System.Security.Cryptography;
using System.Text;

namespace YouFoos.Api.Utilities
{
    /// <summary>
    /// Generates alphanumeric strings - useful for things like password reset codes.
    /// </summary>
    public static class AlphanumericStringGenerator
    {
        private const string ALLOWED_CHARACTERS = "ABCDEFGHJKLMNPQRTWXY34689";

        /// <summary>
        /// This method is intended to be used for cryptographically secure random alphanumeric
        /// strings. These strings are useful for such tasks as generating password reset codes,
        /// for instance - though such strings should still be encrypted at rest for security.
        /// 
        /// Note that these strings are intended to be easy for humans to enter, so they will not
        /// contain characters that are easily confused, such as I and L, or 0 and O. This reduces
        /// their strength cryptographically somewhat, but not to an unacceptable level.
        /// </summary>
        /// <param name="length">The length of the string to return.</param>
        public static string GetSecureRandomAlphanumericString(int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Value must be greater than 0.");
            }   

            var chars = ALLOWED_CHARACTERS.ToCharArray();
            var data = new byte[length];
            RandomNumberGenerator.GetBytes(length);

            StringBuilder result = new(length);

            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }

            return result.ToString();
        }
    }
}
