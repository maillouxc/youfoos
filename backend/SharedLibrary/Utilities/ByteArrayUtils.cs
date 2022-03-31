using System;

namespace YouFoos.SharedLibrary.Utilities
{
    /// <summary>
    /// Byte arrays are not classes/objects, so they cannot have extension methods, unfortunately.
    /// 
    /// This static class is used to define useful methods for operating on byte arrays, which
    /// would otherwise be defined as extension methods.
    /// </summary>
    public static class ByteArrayUtils
    {
        /// <summary>
        /// Rotates the provided byte array to the left by one bit.
        /// </summary>
        /// <exception cref="ArgumentNullException">If the provided byte array is null.</exception>
        public static void ShiftLeft(byte[] byteArray)
        {
            if (byteArray == null)
                throw new ArgumentNullException(nameof(byteArray));

            // Work from left to right.
            for (int i = 0; i < byteArray.Length; i++)
            {
                // If the leftmost bit of the current byte is 1 then we need to carry
                bool carryFlag = (byteArray[i] & 0x80) > 0;

                if (i > 0)
                {
                    if (carryFlag)
                    {
                        // Apply the carry to the rightmost bit of the current bytes neighbor to the left.
                        byteArray[i - 1] = (byte)(byteArray[i - 1] | 0x01);
                    }
                }

                byteArray[i] <<= 1;
            }
        }
    }
}
