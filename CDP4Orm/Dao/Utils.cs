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

namespace CDP4Orm.Dao
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.Helpers;
    using CDP4Common.Types;

    using CDP4Orm.Dao.Resolve;

    using Npgsql;

    /// <summary>
    /// A utility class that supplies common functionalities to the ORM layer.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// The site directory partition.
        /// </summary>
        public const string SiteDirectoryPartition = "SiteDirectory";

        /// <summary>
        /// The EngineeringModel partition.
        /// </summary>
        public const string EngineeringModelPartition = "EngineeringModel";

        /// <summary>
        /// The iteration sub partition.
        /// </summary>
        public const string IterationSubPartition = "Iteration";

        /// <summary>
        /// The UTC (zulu) date time serialization format.
        /// </summary>
        public const string DateTimeUtcSerializationFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";

        /// <summary>
        /// The UTC (zulu) date time SQL format.
        /// </summary>
        public const string DateTimeUtcSqlFormat = "yyyy-MM-dd HH:mm:ss.fff";

        /// <summary>
        /// The class kind key.
        /// </summary>
        private const string ClassKindKey = "ClassKind";

        /// <summary>
        /// The id key.
        /// </summary>
        private const string IidKey = "Iid";

        /// <summary>
        /// The quotes.
        /// </summary>
        private const string Quotes = "\"";

        /// <summary>
        /// The key value pair entry separator.
        /// </summary>
        private static readonly string[] EntrySeparator = new[] { "," };

        /// <summary>
        /// The key value pair separator.
        /// </summary>
        private static readonly string[] KvpSeparator = new[] { "=>" };

        /// <summary>
        /// Parse a database HSTORE text representation to a Dictionary instance
        /// </summary>
        /// <param name="hstoreSource">
        /// The HSTORE source text representation.
        /// </param>
        /// <returns>
        /// A key value pair dictionary of type string, string.
        /// </returns>
        public static Dictionary<string, string> ParseHstoreString(string hstoreSource)
        {
            return hstoreSource.Split(EntrySeparator, StringSplitOptions.RemoveEmptyEntries)
                .Select(entry => entry.Split(KvpSeparator, StringSplitOptions.RemoveEmptyEntries))
                .ToDictionary(kvp => kvp[0].Replace(Quotes, string.Empty).Trim(), kvp => kvp[1].Replace(Quotes, string.Empty).Trim());            
        }

        /// <summary>
        /// Convert a string to a <see cref="ValueArray{T}"/>
        /// </summary>
        /// <typeparam name="T">The generic type of the <see cref="ValueArray{T}"/></typeparam>
        /// <param name="valueArrayString">The string to convert</param>
        /// <returns>The <see cref="ValueArray{T}"/></returns>
        public static ValueArray<T> FromHstoreToValueArray<T>(this string valueArrayString) =>
            ValueArrayUtils.FromHstoreToValueArray<T>(valueArrayString);

        /// <summary>
        /// Convert a <see cref="ValueArray{String}"/> to the JSON format
        /// </summary>
        /// <param name="valueArray">The <see cref="ValueArray{String}"/></param>
        /// <returns>The JSON string</returns>
        public static string ToHstoreString(this ValueArray<string> valueArray) =>
            ValueArrayUtils.ToHstoreString(valueArray);

        /// <summary>
        /// Parse a source string to it's equivalent enumeration representation.
        /// </summary>
        /// <param name="source">
        /// The source string.
        /// </param>
        /// <typeparam name="T">
        /// A passed in enumeration type
        /// </typeparam>
        /// <returns>
        /// The parsed enumeration entry as per the passed in type for <see cref="T"/>.
        /// </returns>
        public static T ParseEnum<T>(string source) where T : struct, IConvertible
        {
            return (T)Enum.Parse(typeof(T), source.Replace(" ", string.Empty), true);
        }

        /// <summary>
        /// Parse a source string to it's equivalent enumeration array representation.
        /// </summary>
        /// <param name="source">
        /// The source string.
        /// </param>
        /// <typeparam name="T">
        /// A passed in enumeration type
        /// </typeparam>
        /// <returns>
        /// The parsed enumeration entry as per the passed in type for <see cref="T"/>.
        /// </returns>
        public static IEnumerable<T> ParseEnumArray<T>(string source) where T : struct, IConvertible
        {
            var tempArray = source.Split(EntrySeparator, StringSplitOptions.RemoveEmptyEntries);
            return ParseEnumArray<T>(tempArray);
        }
        
        /// <summary>
        /// Parse a source string to it's equivalent enumeration array representation.
        /// </summary>
        /// <param name="source">
        /// The source string.
        /// </param>
        /// <typeparam name="T">
        /// A passed in enumeration type
        /// </typeparam>
        /// <returns>
        /// The parsed enumeration entry as per the passed in type for <see cref="T"/>.
        /// </returns>
        public static IEnumerable<T> ParseEnumArray<T>(string[] source) where T : struct, IConvertible
        {
            return source.Select(ParseEnum<T>);
        }

        /// <summary>
        /// Parse a bi-dimensional array to an ordered list of type T.
        /// </summary>
        /// <typeparam name="T">
        /// Value type T to which the value should be parsed
        /// </typeparam>
        /// <param name="orderedSource">
        /// The bi-dimensional source holding the ordered information.
        /// </param>
        /// <returns>
        /// An IEnumerable list of <see cref="OrderedItem"/>.
        /// </returns>
        public static IEnumerable<OrderedItem> ParseOrderedList<T>(string[,] orderedSource)
        {
            if (orderedSource == null || orderedSource.GetLength(1) == 0)
            {
                // cancel enumeration yield as there is no viable data present
                yield break;
            }

            // get type converter for type
            var converter = TypeDescriptor.GetConverter(typeof(T));
            for (var i = 0; i < orderedSource.GetLength(1); i++)
            {
                yield return new OrderedItem
                        {
                            K = long.Parse(orderedSource[0, i]),
                            V = (T)converter.ConvertFromString(orderedSource[1, i])
                        };
            }
        }

        /// <summary>
        /// Parse a string formatted as a UTC date.
        /// </summary>
        /// <param name="source">
        /// The source string representation of a UTC date.
        /// </param>
        /// <returns>
        /// A <see cref="DateTime"/> instance which corresponds to the supplied date in string format.
        /// </returns>
        public static DateTime ParseUtcDate(string source)
        {
            return DateTime.Parse(source, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal);
        }

        /// <summary>
        /// Convenience function to encapsulate null value determination when writing to the database.
        /// </summary>
        /// <param name="value">
        /// An object instance representing the a value to be written to the database.
        /// </param>
        /// <returns>
        /// The passed in value instance if not null, otherwise the DBNull.Value.
        /// </returns>
        public static object NullableValue(object value)
        {
            return value ?? DBNull.Value;
        }

        /// <summary>
        /// Extension method to escape string representation to allow database persistence.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/> value.
        /// </param>
        /// <returns>
        /// The escaped <see cref="string"/> if applicable.
        /// </returns>
        public static string Escape(this string value)
        {
            if (value == null)
            {
                return null;
            }

            return string.IsNullOrWhiteSpace(value) 
                ? string.Empty 
                : value.Replace("\"", "\\\"");
        }

        /// <summary>
        /// Extension method to un-escape a escaped string representation as retrieved from the database.
        /// </summary>
        /// <param name="value">
        /// The <see cref="string"/>  value.
        /// </param>
        /// <returns>
        /// The unescaped <see cref="string"/> if applicable.
        /// </returns>
        public static string UnEscape(this string value)
        {
            if (value == null)
            {
                return null;
            }

            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Replace("\\\"", "\"");
        }

        /// <summary>
        /// Extension method that reads the contents of the passed in SQL file to the NPGSQL command text.
        /// </summary>
        /// <param name="command">
        /// The command object to which the SQL file contents will be set.
        /// </param>
        /// <param name="resourceName">
        /// The embedded resource Name.
        /// </param>
        /// <param name="enc">
        /// Optional encoding to be used when reading in the file. By default UTF8 encoding is used.
        /// </param>
        /// <param name="replace">
        /// Optional replacement information expressed in the form of Tuples (item1 = text to replace, item2 = replacement text), these are executed in order.
        /// </param>
        public static void ReadSqlFromResource(this NpgsqlCommand command, string resourceName, System.Text.Encoding enc = null, IEnumerable<Tuple<string, string>> replace = null)
        {
            string strText;

            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName), enc ?? System.Text.Encoding.UTF8))
            {
                strText = reader.ReadToEnd();
            }

            AssignCommandText(command, strText, replace);
        }

        /// <summary>
        /// Extension method that reads the contents of the passed in SQL file to the NPGSQL command text.
        /// </summary>
        /// <param name="command">
        /// The command object to which the SQL file contents will be set.
        /// </param>
        /// <param name="filePath">
        /// The embedded resource Name.
        /// </param>
        /// <param name="enc">
        /// Optional encoding to be used when reading in the file. By default UTF8 encoding is used.
        /// </param>
        /// <param name="replace">
        /// Optional replacement information expressed in the form of Tuples (item1 = text to replace, item2 = replacement text), these are executed in order.
        /// </param>
        public static void ReadSqlFromFile(this NpgsqlCommand command, string filePath, System.Text.Encoding enc = null, IEnumerable<Tuple<string, string>> replace = null)
        {
            string strText;
            using (var reader = new StreamReader(filePath, enc ?? System.Text.Encoding.UTF8))
            {
                strText = reader.ReadToEnd();
            }

            AssignCommandText(command, strText, replace);
        }

        /// <summary>
        /// Assign the sql text to the supplied command.
        /// </summary>
        /// <param name="command">
        /// The command object to which the SQL file contents will be set.
        /// </param>
        /// <param name="strText">
        /// The SQL command text
        /// </param>
        /// <param name="replace">
        /// Optional replacement information expressed in the form of Tuples (item1 = text to replace, item2 = replacement text), these are executed in order.
        /// </param>
        public static void AssignCommandText(NpgsqlCommand command, string strText, IEnumerable<Tuple<string, string>> replace = null)
        {
            if (replace != null)
            {
                strText = replace.Aggregate(
                    strText,
                    (current, replacementInfo) => current.Replace(replacementInfo.Item1, replacementInfo.Item2));
            }

            command.CommandText = strText;
        }

        /// <summary>
        /// Get the formatted database schema name for the passed in engineering model id.
        /// </summary>
        /// <param name="engineeringModelIid">
        /// The engineering model id.
        /// </param>
        /// <returns>
        /// A formatted and quoted schema name
        /// </returns>
        public static string GetEngineeringModelSchemaName(Guid engineeringModelIid) => 
            $"{EngineeringModelPartition}_{engineeringModelIid.ToString().Replace("-", "_")}";

        /// <summary>
        /// Get the formatted database iteration schema name for the passed in engineering model id.
        /// </summary>
        /// <param name="engineeringModelIid">
        /// The engineering model id.
        /// </param>
        /// <returns>
        /// A formatted and quoted schema name
        /// </returns>
        public static string GetEngineeringModelIterationSchemaName(Guid engineeringModelIid) => 
            $"{IterationSubPartition}_{engineeringModelIid.ToString().Replace("-", "_")}";

        /// <summary>
        /// Extension method to get a tuple of Id and ClassKind from a <see cref="Thing"/> instance.
        /// </summary>
        /// <param name="thing">
        /// The thing.
        /// </param>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        public static DtoInfo GetInfoPlaceholder(this Thing thing)
        {
            return new DtoInfo(thing.ClassKind.ToString(), thing.Iid);
        }

        /// <summary>
        /// Extension method to get a tuple of Id and ClassKind from a <see cref="ClasslessDTO"/> instance.
        /// </summary>
        /// <param name="classlessDto">
        /// The <see cref="ClasslessDTO"/> instance.
        /// </param>
        /// <returns>
        /// The <see cref="Tuple"/>.
        /// </returns>
        public static DtoInfo GetInfoPlaceholder(this ClasslessDTO classlessDto)
        {
            return new DtoInfo(classlessDto[ClassKindKey].ToString(), (Guid)classlessDto[IidKey]);
        }

        public static int ExecuteAndLogNonQuery(this NpgsqlCommand command, ICommandLogger logger)
        {
            return logger.ExecuteAndLog(command);
        }
    }
}
