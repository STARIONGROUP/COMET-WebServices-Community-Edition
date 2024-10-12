// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Starion Group S.A.">
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

namespace CometServer.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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
            if (inputValues.StartsWith('[') && inputValues.EndsWith(']'))
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
            if (!shortGuids.StartsWith('['))
            {
                throw new ArgumentException("Invalid ShortGuid Array, must start with [", nameof(shortGuids));
            }

            if (!shortGuids.EndsWith(']'))
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
        /// Attempts to parse a string containing comma-separated short GUIDs into a list of Guid.
        /// </summary>
        /// <param name="values">The string containing comma-separated short GUIDs.</param>
        /// <param name="identifiers">When this method returns, contains the list of parsed Guids, or an empty list if parsing fails.</param>
        /// <returns>
        /// True if the parsing is successful and the list of Guids is not empty; otherwise, false.
        /// </returns>
        public static bool TryParseEnumerableOfGuid(this string values, out List<Guid> identifiers)
        {
            ArgumentNullException.ThrowIfNull(values, nameof(values));

            identifiers = new List<Guid>();

            try
            {
                identifiers = values.FromShortGuidArray()?.ToList() ?? new List<Guid>();
                return identifiers.Any();
            }
            catch (Exception)
            {
                return false;
            }
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

        /// <summary>
        /// Sanitizes the string to prevent malicious injection such as log injection
        /// The method removes newline, and returns
        /// </summary>
        /// <param name="input">
        /// the string to sanitize
        /// </param>
        /// <returns>
        /// the sanitized string
        /// </returns>
        public static string Sanitize(this string input)
        {
            return input.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
        }
    }
}
