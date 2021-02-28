// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utils.cs" company="RHEA System S.A.">
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

namespace CometServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Autofac;

    using CDP4Common.Types;

    using CometServer.Configuration;
    using CometServer.Modules;

    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// A utility class that supplies common functionalities to the Service layer.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// The UTC (zulu) date time serialization format.
        /// </summary>
        public const string DateTimeUtcSerializationFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffff'Z'";

        /// <summary>
        /// The capitalize first letter.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If supplied input is null or empty
        /// </exception>
        public static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("string can't be empty!");
            }

            return $"{input.First().ToString(CultureInfo.InvariantCulture).ToUpper()}{input.Substring(1)}";
        }

        /// <summary>
        /// Get a string and lower the first letter
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string FirstLetterToLower(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("string can't be empty!");
            }

            return string.Format(
                "{0}{1}", input.First().ToString(CultureInfo.InvariantCulture).ToLower(), input.Substring(1)); 
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
        public static Guid ParseIdentifier(string input)
        {
            Guid identifier;
            if (!Guid.TryParse(input, out identifier))
            {
                throw new Exception("Invalid identifier supplied");
            }

            return identifier;
        }

        /// <summary>
        /// Construct a POSTGRESQL connection string based on the passed in database name.
        /// </summary>
        /// <param name="database">
        /// The name of the database to connect to.
        /// </param>
        /// <returns>
        /// The constructed connection string
        /// </returns>
        public static string GetConnectionString(string database)
        {
            return $"Server={AppConfig.Current.Backtier.HostName};Port={AppConfig.Current.Backtier.Port};User Id={AppConfig.Current.Backtier.UserName};Password={AppConfig.Current.Backtier.Password};Database={database};Convert Infinity DateTime=true;CommandTimeout={AppConfig.Current.Backtier.StatementTimeout};";
        }

        /// <summary>
        /// Extension method that extract the ordered Ids as a list of GUID from the passed in IEnumerable of <see cref="OrderedItem"/>.
        /// </summary>
        /// <param name="orderedList">
        /// The ordered list of Guids.
        /// </param>
        /// <returns>
        /// A list instance with the extracted Guids.
        /// </returns>
        public static IEnumerable<Guid> ToIdList(this IEnumerable<OrderedItem> orderedList)
        {
            // TODO leave only (Guid)x.V in the Select when json deserialisation problem is solved (task T2780 CDP4WEBSERVICES)
            return orderedList.Select(x => x.V.GetType() == typeof(Guid) ? (Guid)x.V : Guid.Parse(x.V.ToString()));
        }

        /// <summary>
        /// Extension method to check that the DTO thing instance is equal to, or a descendant of the supplied generic type T.
        /// </summary>
        /// <typeparam name="T">
        /// Generic type parameter to check against
        /// </typeparam>
        /// <param name="thing">
        /// The Thing instance on which the type check is performed
        /// </param>
        /// <returns>
        /// True if determined equal or descendant type.
        /// </returns>
        public static bool IsSameOrDerivedClass<T>(this Thing thing)
        {
            var potentialBase = typeof(T);
            var potentialDescendant = thing.GetType();
            return potentialDescendant.IsSubclassOf(potentialBase) || potentialDescendant == potentialBase;
        }

        /// <summary>
        /// Extension method to set authenticated credential information to permission service.
        /// </summary>
        /// <param name="module">
        /// The module.
        /// </param>
        internal static void CdpAuthorization(this ApiBase module)
        {
            module.PermissionService.Credentials = module.RequestUtils.Context.AuthenticatedCredentials;
        }
    }
}
