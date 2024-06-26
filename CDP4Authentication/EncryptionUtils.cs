﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptionUtils.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-COMET Webservices Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Webservices Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Authentication
{
    using System;
    using System.Security.Cryptography;

    /// <summary>
    /// Provides static helper methods to help with encryption
    /// </summary>
    public static class EncryptionUtils
    {
        /// <summary>
        /// The maximum number of bytes in a salt byte array.
        /// </summary>
        private const int SaltMaximumNumberOfBytes = 32;

        /// <summary>
        /// Generates a salted hash from an input string.
        /// </summary>
        /// <param name="inputString">
        /// The input string.
        /// </param>
        /// <param name="salt">
        /// The salt byte array.
        /// </param>
        /// <returns>
        /// A salted byte array.
        /// </returns>
        private static byte[] GenerateSaltedHash(string inputString, byte[] salt)
        {
            var plainText = GetBytesFromString(inputString);
            var plainTextWithSaltBytes = new byte[plainText.Length + salt.Length];

            for (var i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }

            for (var i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return SHA256.HashData(plainTextWithSaltBytes);
        }

        /// <summary>
        /// Generates a base 64 salted string from an input string and a byte array salt.
        /// </summary>
        /// <param name="inputString">
        /// The input string.
        /// </param>
        /// <param name="salt">
        /// The salt string.
        /// </param>
        /// <returns>
        /// Salted base 64 string.
        /// </returns>
        public static string GenerateSaltedString(string inputString, string salt)
        {
            var saltBytes = GetBytesFromBase64String(salt);
            return Convert.ToBase64String(GenerateSaltedHash(inputString, saltBytes));
        }

        /// <summary>
        /// Compares the input string with an encrypted string
        /// </summary>
        /// <param name="inputString">
        /// The input string.
        /// </param>
        /// <param name="encryptedString">
        /// The previously encoded string.
        /// </param>
        /// <param name="salt">
        /// The salt string.
        /// </param>
        /// <returns>
        /// True if the strings match.
        /// </returns>
        public static bool CompareSaltedStrings(string inputString, string encryptedString, string salt)
        {
            return string.Equals(GenerateSaltedString(inputString, salt), encryptedString);
        }

        /// <summary>
        /// Converts a string to a byte array.
        /// </summary>
        /// <remarks>
        /// source <see cref="http://stackoverflow.com/questions/472906/how-to-get-a-consistent-byte-representation-of-strings-in-c-sharp-without-manual"/>
        /// </remarks>
        /// <param name="inputString">
        /// The string to convert.
        /// </param>
        /// <returns>
        /// A byte array representation of the string
        /// </returns>
        public static byte[] GetBytesFromString(string inputString)
        {
            var bytes = new byte[inputString.Length * sizeof(char)];
            Buffer.BlockCopy(inputString.ToCharArray(), 0, bytes, 0, bytes.Length);

            return bytes;
        }

        /// <summary>
        /// Converts a byte array to a string.
        /// </summary>
        /// <remarks>
        /// source <see cref="http://stackoverflow.com/questions/472906/how-to-get-a-consistent-byte-representation-of-strings-in-c-sharp-without-manual"/>
        /// </remarks>
        /// <param name="bytes">
        /// The byte array to convert.
        /// </param>
        /// <returns>
        /// A string representation of the byte array.
        /// </returns>
        public static string GetStringFromBytes(byte[] bytes)
        {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);

            return new string(chars);
        }

        /// <summary>
        /// Generates a random 32 bytes non zero byte salt string
        /// </summary>
        /// <remarks>
        /// source <see cref="http://codereview.stackexchange.com/questions/93614/salt-generation-in-c"/>
        /// </remarks>
        /// <returns>
        /// A random 32 bytes non zero byte salt.
        /// </returns>
        private static byte[] GenerateRandomSaltBytes()
        {
            var salt = new byte[SaltMaximumNumberOfBytes];

            // Use RandomNumberGenerator.Create for a secure random number generator
            using var random = RandomNumberGenerator.Create();

            random.GetBytes(salt); // Use GetBytes instead of GetNonZeroBytes

            return salt;
        }

        /// <summary>
        /// Generates a random 32 bytes non zero byte salt string
        /// </summary>
        /// <remarks>
        /// source <see cref="http://codereview.stackexchange.com/questions/93614/salt-generation-in-c"/>
        /// </remarks>
        /// <returns>
        /// A random 32 bytes non zero byte salt string.
        /// </returns>
        public static string GenerateRandomSaltString()
        {
            return Convert.ToBase64String(GenerateRandomSaltBytes());
        }

        /// <summary>
        /// Converts a base 64 string to a byte array.
        /// </summary>
        /// <remarks>The input string really should be a Base64String to make any sense.</remarks>
        /// <param name="inputString">
        /// The string to convert.
        /// </param>
        /// <returns>
        /// A byte array representation of the string.
        /// </returns>
        private static byte[] GetBytesFromBase64String(string inputString)
        {
            return Convert.FromBase64String(inputString);
        }
    }
}
