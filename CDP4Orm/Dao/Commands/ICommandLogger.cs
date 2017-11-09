// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandLogger.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    using System.Collections.Generic;

    using Npgsql;

    /// <summary>
    /// The command logger interface.
    /// </summary>
    public interface ICommandLogger
    {
        /// <summary>
        /// Retrieve the Sql commands as an ordered collection
        /// </summary>
        IEnumerable<string> LoggedSqlCommands { get; }

        /// <summary>
        /// Retrieve the logged SQL command text
        /// </summary>
        string LoggedSqlCommandText { get; }

        /// <summary>
        /// Enable or disable the SQL command logger
        /// </summary>
        /// <remarks>
        /// Disabled by default
        /// </remarks>
        bool LoggingEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether to execute issued SQL commands or only log (for database seeding purposes)
        /// </summary>
        /// <remarks>
        /// Enabled by default
        /// </remarks>
        bool ExecuteCommands { get; set; }

        /// <summary>
        /// Clear the log
        /// </summary>
        void ClearLog();

        /// <summary>
        /// Execute and Log the prepared SQL command
        /// </summary>
        /// <param name="command">
        /// The NpgsqlCommand to log from
        /// </param>
        /// <returns>
        /// The number of rows affected. 
        /// </returns>
        int ExecuteAndLog(NpgsqlCommand command);

        /// <summary>
        /// Log the SQL from the command
        /// </summary>
        /// <param name="command">
        /// The NpgsqlCommand to log from
        /// </param>
        void Log(NpgsqlCommand command);
    }
}
