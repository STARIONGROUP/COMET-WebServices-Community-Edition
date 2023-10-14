// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringHelper.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

namespace CometServer.Helpers
{
    using System;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// A static helper class that provides extensions to the <see cref="String"/> data-type
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Parse the identifier input as GUID.
        /// </summary>
        /// <param name="input">
        /// The input string which is to be parsed.
        /// </param>
        /// <returns>
        /// The parsed GUID identifier
        /// </returns>
        /// <exception cref="Exception">
        /// If the supplied input is not a valid GUID string representation
        /// </exception>
        public static Guid ParseIdentifier(this string input)
        {
            if (!Guid.TryParse(input, out var identifier))
            {
                throw new FormatException($"An invalid Identifier was supplied: {input}");
            }

            return identifier;
        }

        /// <summary>
        /// Coverts the first characther of a string to lowercase
        /// </summary>
        /// <param name="input">
        /// The input instring
        /// </param>
        /// <returns>
        /// The updated <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If supplied input is null or empty
        /// </exception>
        public static string FirstLetterToLower(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("string can't be empty!");
            }

            return $"{input.First().ToString(CultureInfo.InvariantCulture).ToLower()}{input.Substring(1)}";
        }

        /// <summary>
        /// Coverts the first characther of a string to uppercase
        /// </summary>
        /// <param name="input">
        /// The input instring
        /// </param>
        /// <returns>
        /// The updated <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If supplied input is null or empty
        /// </exception>
        public static string CapitalizeFirstLetter(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("string can't be empty!");
            }

            return $"{input.First().ToString(CultureInfo.InvariantCulture).ToUpper()}{input.Substring(1)}";
        }
    }
}
