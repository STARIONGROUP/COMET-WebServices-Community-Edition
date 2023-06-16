// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
// 
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski,
//                 Ahmed Abulwafa Ahmed
// 
//    This file is part of CDP4 Web Services Community Edition.
//    The CDP4 Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
// 
//    The CDP4 Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4 Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension class for <see cref="string"/> object
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Tries to parse a string that should contains a collection of values to a <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="inputValues">The string input</param>
        /// <param name="retrievedValues">The <see cref="IEnumerable{T}"/> of retrieved values</param>
        /// <param name="separator">The separator used to split values</param>
        /// <returns>Assert if the parsing of the <paramref name="inputValues"/> could be done</returns>
        public static bool TryParseCollectionOfValues(this string inputValues, out IEnumerable<string> retrievedValues, char separator = ';')
        {
            if (inputValues.StartsWith("[") && inputValues.EndsWith("]"))
            {
                retrievedValues = inputValues.Substring(1, inputValues.Length - 2).Split(separator);
                return true;
            }

            retrievedValues = Enumerable.Empty<string>();
            return false;
        }

        /// <summary>
        /// Creates a <see cref="IEnumerable{Guid}" /> based the ShortGuid Array representation ->
        /// </summary>
        /// <param name="shortGuids">
        /// an <see cref="IEnumerable{String}" /> shortGuid
        /// </param>
        /// <returns>
        /// an <see cref="IEnumerable{Guid}" />
        /// </returns>
        /// <remarks>
        /// A ShortGuid is a base64 encoded guid-string representation where any "/" has been replaced with a "_"
        /// and any "+" has been replaced with a "-" (to make the string representation <see cref="Uri" /> friendly)
        /// A ShortGuid Array is a string that starts with "[", ends with "]" and contains a number of ShortGuid separated by a ";"
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// Thrown when the <paramref name="shortGuids" /> does not start with '[' or ends with ']'
        /// </exception>
        public static IEnumerable<Guid> FromShortGuidArray(this string shortGuids)
        {
            if (!shortGuids.StartsWith("["))
            {
                throw new ArgumentException("Invalid ShortGuid Array, must start with [", nameof(shortGuids));
            }

            if (!shortGuids.EndsWith("]"))
            {
                throw new ArgumentException("Invalid ShortGuid Array, must end with ]", nameof(shortGuids));
            }

            var listOfShortGuids = shortGuids.TrimStart('[').TrimEnd(']').Split(';');

            foreach (var shortGuid in listOfShortGuids)
            {
                yield return shortGuid.FromShortGuid();
            }
        }

        /// <summary>
        /// Creates a <see cref="Guid" /> based the ShortGuid representation
        /// </summary>
        /// <param name="shortGuid">
        /// a shortGuid string
        /// </param>
        /// <returns>
        /// an instance of <see cref="Guid" />
        /// </returns>
        /// <remarks>
        /// A ShortGuid is a base64 encoded guid-string representation where any "/" has been replaced with a "_"
        /// and any "+" has been replaced with a "-" (to make the string representation <see cref="Uri" /> friendly)
        /// </remarks>
        public static Guid FromShortGuid(this string shortGuid)
        {
            var buffer = Convert.FromBase64String(shortGuid.Replace("_", "/").Replace("-", "+") + "==");
            return new Guid(buffer);
        }

    }
}
