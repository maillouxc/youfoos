using System;
using System.Linq;

namespace YouFoos.GameEventsService.Utilities
{
    public static class RfidConverter
    {
        /// <summary>
        /// This function takes the UID outputted by the card reader and converts it to a string
        /// representing the number printed on the front of the card.
        /// </summary>
        /// 
        /// <remarks>
        /// Hopefully it goes without saying that this only applies to the type of RFID cards
        /// that CallMiner is currently using. Other cards may or may not be compatible,
        /// depending on the specific UID formatting they use - there are thousands of different
        /// formats in use, but their current IntellID HID cards use the highly standardized
        /// 26-bit Weigand format, H10301.
        /// </remarks>
        /// 
        /// <param name="rfidCardUidHexString">The hex bytes of the card's UID in string form.</param>
        /// 
        /// <returns>An string value corresponding to the number printed on the physical card.</returns>
        public static string ConvertRfidUidToNumberOnCard(string rfidCardUidHexString)
        {
            // Because we take in the data as a string of hex bytes, we first have to convert it to real bytes.
            byte[] uidBytes = rfidCardUidHexString
                .Split(' ')
                .Select(hexString => Convert.ToByte(hexString, fromBase: 16))
                .ToArray();

            // Now, we can do the actual conversion to the card's printed number...
            // We remove the first bit and the 26th bit, since they are parity bits.
            // We could actually check the parity here, but it's probably overkill for us.
            // We don't care about any of the bits after the 26th bit.
            // I'm not even sure what the remaining bits are, all that matters is that they aren't important here.

            ByteArrayUtils.ShiftLeft(uidBytes); // Shift left to cut out the first bit

            // Convert each part of the UID to an actual number
            int facilityCode = uidBytes[0];

            // The bytes returned from the reader are big endian...
            // If the machine we are running on uses little endian, we need to flip them
            var cardNumberBytes = new byte[2] { uidBytes[1], uidBytes[2] };
            if (BitConverter.IsLittleEndian)
            {
                // Luckily, this is very easy for us because the number is only 2 bytes
                // We just have to swap the bytes position
                var temp = cardNumberBytes[0];
                cardNumberBytes[0] = cardNumberBytes[1];
                cardNumberBytes[1] = temp;
            }
           
            int cardNumber = BitConverter.ToUInt16(cardNumberBytes, startIndex: 0);

            // Handle leading zeros on the card number
            // This is a temporary hack
            string cardNumberString = cardNumber.ToString();
            while (cardNumberString.Length < 5)
            {
                cardNumberString = "0" + cardNumberString;
            }


            // Now concatenate the results into a single string and convert that string to a number
            return facilityCode + cardNumberString;
        }
    }
}
