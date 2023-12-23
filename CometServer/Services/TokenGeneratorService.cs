// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TokenGeneratorService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
// 
//    The CDP4-COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4-COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Services
{
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// The purpose of the <see cref="ITokenGeneratorService"/> is to generate
    /// request tracking tokens
    /// </summary>
    public class TokenGeneratorService : ITokenGeneratorService
    {
        /// <summary>
        /// Generates a random string that is used as a token in log statements to match log statements related to the 
        /// processing of one request
        /// </summary>
        /// <returns>
        /// random token
        /// </returns>
        public string GenerateRandomToken()
        {
            const int length = 12;
            const string validCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz123456789";

            var stringBuilder = new StringBuilder(length);
            var randomBytes = new byte[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                // Fill the array with a cryptographically strong sequence of random bytes
                rng.GetBytes(randomBytes);

                // Convert each byte into a character from the valid character set
                for (var i = 0; i < length; i++)
                {
                    // Convert the byte to an index into the valid character set
                    var index = randomBytes[i] % validCharacters.Length;
                    stringBuilder.Append(validCharacters[index]);
                }
            }

            return stringBuilder.ToString();
        }
    }
}
