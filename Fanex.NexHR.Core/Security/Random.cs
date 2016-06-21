// --------------------------------------------------------------------------
// <copyright file="Random.cs" company=".">
// Copyright (c) .. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.6.14">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Core.Security
{
    using System.Security.Cryptography;

    /// <summary>
    /// Class Random.
    /// </summary>
    public static class Random
    {
        #region Numeric

        public static string Numberic(int size)
        {
            string numeric = "0123456789";
            return RandomString(numeric, size);
        }

        #endregion

        #region Alphabet

        public static string Alphabet(int size)
        {
            string alphabet = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM";
            return RandomString(alphabet, size);
        }

        public static string AnyAlphabet(int size)
        {
            string alphabet = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890~!@#$%&()<>?-+=\|/";
            return RandomString(alphabet, size);
        }

        #endregion

        #region RNGCrypRandom

        public static byte[] RNGCrypto(int size)
        {
            byte[] valueArray = new byte[size];
            new RNGCryptoServiceProvider().GetBytes(valueArray);

            return valueArray;
        }

        #endregion

        /// <summary>
        /// Function to random string.
        /// </summary>
        /// <param name="allowChars">Allow Chars.</param>
        /// <param name="size">Size of object.</param>
        /// <returns>String value.</returns>
        private static string RandomString(string allowChars, int size)
        {
            int length = allowChars.Length;
            char[] valueArray = new char[size];

            System.Random rd = new System.Random();
            for (int i = 0; i < size; i++)
            {
                valueArray[i] = allowChars[rd.Next(length)];
            }

            return new string(valueArray);
        }
    }
}
