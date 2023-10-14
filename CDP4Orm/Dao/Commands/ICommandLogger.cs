// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandLogger.cs" company="RHEA System S.A.">
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
