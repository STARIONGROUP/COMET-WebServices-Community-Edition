// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utils.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
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
    using System.Text.RegularExpressions;

    using CDP4Common;
    using CDP4Common.DTO;
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
        /// Parse a string to a typed <see cref="ValueArray{T}"/>
        /// </summary>
        /// <param name="source">
        /// The source string.
        /// </param>
        /// <typeparam name="T">
        /// Value type T to which the value should be parsed
        /// </typeparam>
        /// <returns>
        /// An instantiated <see cref="ValueArray{T}"/>.
        /// </returns>
        public static ValueArray<T> ParseValueArray<T>(string source)
        {
            const string ValueArrayPattern = @"\{([^)]*)\}";
            const string Delimiter = ";";

            var matches = Regex.Matches(source, ValueArrayPattern);
            var content = matches[0].Groups[1].Value;
            
            // unescape the Hstore escaped string before further processing
            var unescapedContent = content.Replace("\"", string.Empty);
            var stringValues = unescapedContent.Split(Delimiter.ToCharArray());
            
            // get type converter for type
            var converter = TypeDescriptor.GetConverter(typeof(T));
            
            // cast the values to the supplied type T, ensure Trim is called
            var castValues = stringValues.Select(x => (T)converter.ConvertFromString(x.Trim()));
            return new ValueArray<T>(castValues);
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
        public static string GetEngineeringModelSchemaName(Guid engineeringModelIid)
        {
            return string.Format("{0}_{1}", "EngineeringModel", engineeringModelIid.ToString().Replace("-", "_"));
        }

        /// <summary>
        /// Get the formatted database iteration schema name for the passed in engineering model id.
        /// </summary>
        /// <param name="engineeringModelIid">
        /// The engineering model id.
        /// </param>
        /// <returns>
        /// A formatted and quoted schema name
        /// </returns>
        public static string GetEngineeringModelIterationSchemaName(Guid engineeringModelIid)
        {
            return string.Format("{0}_{1}", "Iteration", engineeringModelIid.ToString().Replace("-", "_"));
        }

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
