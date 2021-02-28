// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLogger.cs" company="RHEA System S.A.">
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
    using System.Linq;
    using System.Text;

    using Npgsql;

    using NpgsqlTypes;

    /// <summary>
    /// A utility class that retrieves and stores the respective SQL for each command send from the ORM layer.
    /// </summary>
    public class CommandLogger : ICommandLogger
    {
        /// <summary>
        /// SQL insert command declaration start signature
        /// </summary>
        private const string InsertCommandDeclaration = "INSERT INTO \"";

        /// <summary>
        /// SQL insert Values declaration signature
        /// </summary>
        private const string InsertValuesDeclaration = "VALUES (";

        /// <summary>
        /// Keep the parenthesis when selecting the SQL insert command substring
        /// </summary>
        private const int ParenthesisOffset = 1;

        /// <summary>
        /// Apply format indent for readability sake
        /// </summary>
        private const string FormatIndent = "    ";

        /// <summary>
        /// The SQL insert statement terminator string
        /// </summary>
        private const string SqlStatementTerminator = ");";

        /// <summary>
        /// The SQL insert statement continuation string
        /// </summary>
        private const string SqlStatementContinuation = "),";

        /// <summary>
        /// The stored SQL commands 
        /// </summary>
        private readonly List<string> storedSqlCommands = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLogger"/> class.
        /// </summary>
        public CommandLogger()
        {
            // disable logging by default
            this.LoggingEnabled = false;

            // enable command execution by default
            this.ExecuteCommands = true;
        }

        /// <summary>
        /// Gets the SQL commands as an ordered collection
        /// </summary>
        public IEnumerable<string> LoggedSqlCommands
        {
            get
            {
                return this.storedSqlCommands;
            }
        }

        /// <summary>
        /// Gets the logged SQL command text
        /// </summary>
        public string LoggedSqlCommandText
        {
            get
            {
                var builder = new StringBuilder();

                // keep track of handled typed insert commands
                var handledTypeInserts = new HashSet<string>();

                foreach (var sqlCommand in this.LoggedSqlCommands)
                {
                    if (!handledTypeInserts.Any(x => sqlCommand.StartsWith(x)))
                    {
                        if (sqlCommand.StartsWith(InsertCommandDeclaration, StringComparison.InvariantCultureIgnoreCase))
                        {
                            // select the insert command part up untill the VALUE declaration
                            var typedInsertInfo = sqlCommand.Substring(
                                0,
                                sqlCommand.IndexOf(
                                    InsertValuesDeclaration,
                                    0,
                                    StringComparison.InvariantCultureIgnoreCase) + 
                                    (InsertValuesDeclaration.Length - ParenthesisOffset));

                            // find and collect all sqlcommands that have the have the same insert signature
                            var insertCommands = this.LoggedSqlCommands.Where(x => x.StartsWith(typedInsertInfo)).ToList();

                            // write out entire type def
                            builder.AppendLine(typedInsertInfo);
                            for (var index= 0; index < insertCommands.Count - 1; index++)
                            {
                                builder.AppendLine(
                                    insertCommands[index].Replace(typedInsertInfo, FormatIndent)
                                        .Replace(SqlStatementTerminator, SqlStatementContinuation));
                            }

                            // add last sql statement keeping the SqlStatementTerminator to close the buildup statement
                            builder.AppendLine(insertCommands.Last().Replace(typedInsertInfo, FormatIndent));

                            // empty line
                            builder.AppendLine();

                            // register this typed insert as handled
                            handledTypeInserts.Add(typedInsertInfo);
                        }
                        else
                        {
                            // regular statement commands
                            builder.AppendLine(sqlCommand);

                            // empty line
                            builder.AppendLine();
                        }
                    }
                    
                    // skip line as it has already been handled
                }

                return builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to log issued SQL commands
        /// </summary>
        /// <remarks>
        /// Disabled by default
        /// </remarks>
        public bool LoggingEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to execute issued SQL commands or only log (for database seeding purposes)
        /// </summary>
        /// <remarks>
        /// Enabled by default
        /// </remarks>
        public bool ExecuteCommands { get; set; }

        /// <summary>
        /// Clear the log
        /// </summary>
        public void ClearLog()
        {
            this.storedSqlCommands.Clear();
        }

        /// <summary>
        /// Execute and Log the prepared SQL command
        /// </summary>
        /// <param name="command">
        /// The NpgsqlCommand to log from
        /// </param>
        /// <returns>
        /// The number of rows affected. 
        /// </returns>
        public int ExecuteAndLog(NpgsqlCommand command)
        {
            this.Log(command);

            if (!this.ExecuteCommands)
            {
                // as the commands are not actually executed, mock the affected rows as if changed
                return 1;
            }

            // execute the prepared command
            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// Log the SQL from the command
        /// </summary>
        /// <param name="command">
        /// The <see cref="NpgsqlCommand"/> to log from
        /// </param>
        public void Log(NpgsqlCommand command)
        {
            if (!this.LoggingEnabled)
            {
                return;
            }

            if (command == null || string.IsNullOrEmpty(command.CommandText))
            {
                // no sql to log, return
                return;
            }

            this.ExtractAndLogSql(command);
        }

        /// <summary>
        /// Convert .net HSTORE type <see cref="Dictionary{TKey,TValue}"/>
        /// </summary>
        /// <param name="hstore">
        /// An instance of an HSTORE store dictionary
        /// </param>
        /// <returns>
        /// Serialized string representation
        /// </returns>
        /// <remarks>
        /// Based on NPGSQL source, added string value char escaping
        /// </remarks>
        public string ReadHstore(Dictionary<string, string> hstore)
        {
            var sb = new StringBuilder();
            var i = hstore.Count;
            foreach (var kv in hstore)
            {
                sb.Append('"');
                sb.Append(kv.Key);
                sb.Append(@"""=>");

                if (kv.Value == null)
                {
                    sb.Append("NULL");
                }
                else
                {
                    sb.Append('"');

                    // make sure to escape single and double quotes so that already escaped double quotes are not escaped again
                    sb.Append(kv.Value.Replace("'", "''").Replace("\"", "\\\"").Replace("\\\\\"", "\\\""));
                    sb.Append('"');
                }

                if (--i > 0)
                {
                    sb.Append(',');
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Serialize the passed in value to be written out as valid SQL
        /// </summary>
        /// <typeparam name="TDotNetType">
        /// The mapped .net type
        /// </typeparam>
        /// <param name="dbType">
        /// The <see cref="NpgsqlDbType"/> datatype
        /// </param>
        /// <param name="isArray">
        /// Indicates if this is an array parameter
        /// </param>
        /// <param name="value">
        /// The parameter value to serialize to SQL string
        /// </param>
        /// <returns>
        /// A serialized string that is a valid SQL representation of the value
        /// </returns>
        internal static string RetrieveValueString<TDotNetType>(NpgsqlDbType dbType, bool isArray, object value)
        {
            if (value == DBNull.Value)
            {
                return "null";
            }

            return isArray
                ? string.Format("'{{{0}}}'::{1}[]", string.Join(", ", (IEnumerable<TDotNetType>)value), dbType.ToString().ToLower())
                : string.Format("'{0}'::{1}", value, dbType.ToString().ToLower());
        }

        /// <summary>
        /// Construct and log the SQL command as string
        /// </summary>
        /// <param name="command">
        /// The <see cref="NpgsqlCommand"/> to log from
        /// </param>
        internal void ExtractAndLogSql(NpgsqlCommand command)
        {
            var hasSqlParameters = command.Parameters != null && command.Parameters.Count > 0;
            
            // log sql before execution
            var sql = command.CommandText;
            
            if (hasSqlParameters)
            {
                foreach (var param in command.Parameters.ToList())
                {
                    var isArrayType = param.NpgsqlDbType < 0;

                    var dbType = isArrayType
                        ? Utils.ParseEnum<NpgsqlDbType>((param.NpgsqlDbType - NpgsqlDbType.Array).ToString())
                        : param.NpgsqlDbType;

                    string stringValue;
                    switch (dbType)
                    {
                        case NpgsqlDbType.Uuid:
                        {
                            stringValue = isArrayType ? RetrieveValueString<Guid>(dbType, true, param.NpgsqlValue) : RetrieveValueString<string>(dbType, false, param.NpgsqlValue);
                            break;
                        }
                        case NpgsqlDbType.Hstore:
                        {
                            stringValue = RetrieveValueString<string>(dbType, false, this.ReadHstore((Dictionary<string, string>)param.NpgsqlValue));
                            break;
                        }
                        case NpgsqlDbType.Bigint:
                        {
                            stringValue = RetrieveValueString<float>(dbType, isArrayType, param.NpgsqlValue);
                            break;
                        }
                        case NpgsqlDbType.Integer:
                        {
                            stringValue = RetrieveValueString<int>(dbType, isArrayType, param.NpgsqlValue);
                            break;
                        }
                        case NpgsqlDbType.Real:
                        {
                            stringValue = RetrieveValueString<double>(dbType, isArrayType, param.NpgsqlValue);
                            break;
                        }
                        case NpgsqlDbType.Text:
                        {
                            stringValue = RetrieveValueString<string>(dbType, isArrayType, param.NpgsqlValue.ToString().Replace("'", "''"));
                            break;
                        }
                        case NpgsqlDbType.Boolean:
                        {
                            stringValue = RetrieveValueString<bool>(dbType, isArrayType, param.NpgsqlValue);
                            break;
                        }
                        default:
                        {
                            stringValue = RetrieveValueString<string>(
                                NpgsqlDbType.Varchar,
                                isArrayType,
                                param.NpgsqlValue.ToString().Replace("'", "''"));
                            break;
                        }
                    }

                    sql = sql.Replace(string.Format(":{0}", param.ParameterName), stringValue);
                }
            }

            this.storedSqlCommands.Add(sql);
        }
    }
}
