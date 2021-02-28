// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptionUtils.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WspDatabaseAuthentication
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Provides static helper methods to help with encryption
    /// </summary>
    public static class EncryptionUtils
    {
        /// <summary>
        /// Compares the input string with an encrypted string. (WSP specific)
        /// </summary>
        /// <param name="password">
        /// The input password string.
        /// </param>
        /// <param name="encryptedPassword">
        /// The previously password encoded string.
        /// </param>
        /// <param name="salt">
        /// The salt string.
        /// </param>
        /// <param name="serverSalt">
        /// The WSP server salt string.
        /// </param>
        /// <returns>
        /// True if the strings match.
        /// </returns>
        public static bool CompareWspSaltedString(string password, string encryptedPassword, string salt, string serverSalt)
        {
            return BuildWspSaltedString(password, salt, serverSalt).ToLower().Equals(encryptedPassword.ToLower());
        }

        /// <summary>
        /// Generates a base 64 salted string from an input string and a byte array salt. (WSP specific)
        /// </summary>
        /// <param name="password">
        /// The input password string.
        /// </param>
        /// <param name="salt">
        /// The salt string.
        /// </param>
        /// <param name="serverSalt">
        /// The WSP server salt string.
        /// </param>
        /// <returns>
        /// Salted base 64 string.
        /// </returns>
        public static string BuildWspSaltedString(string password, string salt, string serverSalt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltBytes = Encoding.UTF8.GetBytes(salt);
            var serverSaltBytes = Encoding.UTF8.GetBytes(serverSalt);

            string hashedPassword;

            using (var hasher = SHA256.Create())
            {
                hasher.Initialize();
                hasher.TransformBlock(serverSaltBytes, 0, serverSaltBytes.Length, null, 0);
                hasher.TransformBlock(passwordBytes, 0, passwordBytes.Length, null, 0);
                hasher.TransformFinalBlock(saltBytes, 0, saltBytes.Length);

                hashedPassword = BitConverter.ToString(hasher.Hash).Replace("-", "");
            }

            return hashedPassword;
        }
    }
}
