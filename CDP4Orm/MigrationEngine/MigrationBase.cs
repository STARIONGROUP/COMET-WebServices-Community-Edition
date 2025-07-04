﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationBase.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
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

namespace CDP4Orm.MigrationEngine
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;

    using Npgsql;

    using NpgsqlTypes;

    /// <summary>
    /// The SQL migration script base class.
    /// </summary>
    public abstract class MigrationBase
    {
        /// <summary>
        /// The string to replace in the script for schema partition
        /// </summary>
        protected const string SCHEMA_NAME_REPLACE = "SchemaName_Replace";

        /// <summary>
        /// The string to replace in the script for engineeringModel partition
        /// </summary>
        protected const string ENGINEERING_MODEL_REPLACE = "EngineeringModel_Replace";

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationBase"/> class
        /// </summary>
        /// <param name="migrationMetadata">The migration script metadata</param>
        protected MigrationBase(MigrationMetaData migrationMetadata)
        {
            this.MigrationMetaData = migrationMetadata;
        }

        /// <summary>
        /// Gets migration script metadata
        /// </summary>
        public MigrationMetaData MigrationMetaData { get; }

        /// <summary>
        /// Apply the current migration on the specified schema if applicable
        /// </summary>
        public virtual async Task ApplyMigration(NpgsqlTransaction transaction, IReadOnlyList<string> existingSchemas)
        {
            await this.SaveMigrationMetadata(transaction);
        }

        /// <summary>
        /// Save the migration in the migration management table which contains the list of migrations that have been applied in the current database
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        protected async Task SaveMigrationMetadata(NpgsqlTransaction transaction)
        {
            await using var sqlCommand = new NpgsqlCommand();

            const string cmdText = "INSERT INTO \"SiteDirectory\".\"MigrationManagement\" (\"version\", \"name\", \"date\", \"scope\", \"resource_name\") VALUES (:version, :name, :date, :scope, :resourceName) ON CONFLICT (\"version\") DO NOTHING;";

            sqlCommand.CommandText = cmdText;
            sqlCommand.Parameters.Add("version", NpgsqlDbType.Text).Value = this.MigrationMetaData.Version.ToString();
            sqlCommand.Parameters.Add("name", NpgsqlDbType.Text).Value = this.MigrationMetaData.Name;
            sqlCommand.Parameters.Add("date", NpgsqlDbType.Text).Value = this.MigrationMetaData.MigrationScriptDate.ToString(CultureInfo.InvariantCulture);
            sqlCommand.Parameters.Add("scope", NpgsqlDbType.Text).Value = this.MigrationMetaData.MigrationScriptApplicationKind.ToString();
            sqlCommand.Parameters.Add("resourceName", NpgsqlDbType.Text).Value = this.MigrationMetaData.ResourceName;

            sqlCommand.Connection = transaction.Connection;
            sqlCommand.Transaction = transaction;
            await sqlCommand.ExecuteNonQueryAsync();
        }
    }
}
