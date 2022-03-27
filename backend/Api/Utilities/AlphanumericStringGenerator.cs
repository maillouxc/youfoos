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
        /// <summary>
        /// This method is intended to be used for cryptographically secure random alphanumeric
        /// strings. These strings are useful for such tasks as generating password reset codes,
        /// for instance.
        /// 
        /// Note that these strings are intended to be easy for humans to enter, so they will not
        /// contain characters that are easily confused, such as I and L, or 0 and O. This reduces
        /// their strength cryptographically somewhat, but not to an unacceptable level.
        /// </summary>
        /// <param name="length">The length of the string to return.</param>
        /// <returns></returns>
        public static string GetSecureRandomAlphanumericString(int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException();

            var chars = "ABCDEFGHJKLMNPQRTWXY34689".ToCharArray();
            var data = new byte[length];
            using (RNGCryptoServiceProvider srng = new RNGCryptoServiceProvider())
            {
                srng.GetBytes(data);
            };

            StringBuilder result = new StringBuilder(length);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }

            return result.ToString();
        }
    }
}
