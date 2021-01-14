using System;
using System.Linq;

namespace TrophyParser
{
    /// <summary>
    /// An extension class for byte witchcraft...
    /// </summary>
    public static class Misc
    {

        /// <summary>
        /// Convert an numerical value to byte array of size 4, this probably won't work with a float
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns></returns>
        public static byte[] ToBytes(this object value)
        {
            byte[] temp = BitConverter.GetBytes((uint)value);
            if (BitConverter.IsLittleEndian) Array.Reverse(temp);
            return temp;
        }

        /// <summary>
        /// Convert  byte array to uint.
        /// </summary>
        /// <param name="data">byte array to convert</param>
        /// <returns></returns>
        /// <exception cref="Exception">If is not possible to convert</exception>
        public static uint ToUint(this byte[] data)
        {
            if (BitConverter.IsLittleEndian) Array.Reverse(data);
            return data.Length switch
            {
                1 => data[0],
                2 => BitConverter.ToUInt16(data, 0),
                4 => BitConverter.ToUInt32(data, 0),
                _ => throw new Exception("Size is to big for the value")
            };
        }

        /// <summary>
        /// Convert a HexString to UInt,
        /// </summary>
        /// <param name="hex">string representation of hex value </param>
        public static uint ToUInt(this string hex)
            => Convert.ToUInt32(hex, 16);

        public static uint ToUInt(this object value)
            => Convert.ToUInt32(value);

        /// <summary>
        /// Convert a string to UInt
        /// </summary>
        /// <param name="data">string that represent a data</param>
        /// <param name="b">Base that represent the data string</param>
        public static uint ToUInt(this string data, int b)
            => Convert.ToUInt32(data, b);


        /// <summary>
        /// Convert a HexString to Int,
        /// </summary>
        /// <param name="hex">string representation of hex value </param>
        public static int ToInt(this string hex)
            => Convert.ToInt32(hex, 16);


        /// <summary>
        /// Convert a string to Int
        /// </summary>
        /// <param name="data">string that represent a data</param>
        /// <param name="b">Base that represent the data string</param>
        public static int ToInt(this string data, int b)
            => Convert.ToInt32(data, b);

        /// <summary>
        /// Convert a string in hex format to byte array
        /// </summary>
        /// <param name="hex">string with hex representation</param>
        /// <returns></returns>
        public static byte[] ToBytes(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        /// <summary>
        /// Convert byte array to a string with hex representation.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string ToHexString(this byte[] buffer)
        {
            string str = "";
            for (int i = 0; i < buffer.Length; i++)
                str = str + buffer[i].ToString("X2");
            return str;
        }



        /// <summary>
        /// Search a byte array inside other byte array
        /// </summary>
        /// <param name="src">Universe where to search</param>
        /// <param name="pattern">Subset of the universe to find</param>
        /// <param name="nMatch">How many times must be found</param>
        /// <returns>Position where it was found</returns>
        /// <exception cref="Exception">If the subset was not found</exception>
        public static long Search(this byte[] src, byte[] pattern, int nMatch)
        {
            int found = 0;
            for (long i = 0; i < src.Length - pattern.Length + 1; ++i)
            {
                long j;
                if (src[i] != pattern[0]) continue;
                for (j = pattern.Length - 1; j >= 1 && src[i + j] == pattern[j]; j--) ;
                if (j == 0 && ++found == nMatch) return i;
            }
            return -1;
        }

    }
}