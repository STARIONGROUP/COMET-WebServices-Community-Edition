// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utils.cs" company="Starion Group S.A.">
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

namespace CometServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.Types;
    
    using CometServer.Configuration;

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
        /// Construct a POSTGRESQL connection string based on the passed in database name.
        /// </summary>
        /// <param name="backtierConfig">
        /// The <see cref="BacktierConfig"/> that provides the connection string settings
        /// </param>
        /// <param name="database">
        /// The name of the database to connect to.
        /// </param>
        /// <returns>
        /// The constructed connection string
        /// </returns>
        public static string GetConnectionString(BacktierConfig backtierConfig, string database)
        {
            return $"Server={backtierConfig.HostName};Port={backtierConfig.Port};User Id={backtierConfig.UserName};Password={backtierConfig.Password};Database={database};CommandTimeout={backtierConfig.StatementTimeout};";
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
    }
}
