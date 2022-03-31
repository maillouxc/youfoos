using System;
using System.Collections.Generic;

namespace YouFoos.SharedLibrary.Extensions
{
    /// <summary>
    /// Contains useful extension methods for the <see cref="List{T}"/> class.
    /// </summary>
    public static class ListExtensions
    {
        private static readonly Random rng = new();

        /// <summary>
        /// Shuffles the provided list in place using the Fisher-Yates shuffle.
        /// 
        /// This method uses a non-secure random number generator and is not suitable for cryptographic purposes.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
    }
}
