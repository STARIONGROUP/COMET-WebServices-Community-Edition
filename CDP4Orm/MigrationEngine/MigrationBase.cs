// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationBase.cs" company="RHEA System S.A.">
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

namespace CDP4Orm.MigrationEngine
{
    using System.Collections.Generic;
    using System.Globalization;

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
        public virtual void ApplyMigration(NpgsqlTransaction transaction, IReadOnlyList<string> existingSchemas)
        {
            this.SaveMigrationMetadata(transaction);
        }

        /// <summary>
        /// Save the migration in the migration management table which contains the list of migrations that have been applied in the current database
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        protected void SaveMigrationMetadata(NpgsqlTransaction transaction)
        {
            using (var sqlCommand = new NpgsqlCommand())
            {
                var cmdText = "INSERT INTO \"SiteDirectory\".\"MigrationManagement\" (\"version\", \"name\", \"date\", \"scope\", \"resource_name\") VALUES (:version, :name, :date, :scope, :resourceName) ON CONFLICT (\"version\") DO NOTHING;";

                sqlCommand.CommandText = cmdText;
                sqlCommand.Parameters.Add("version", NpgsqlDbType.Text).Value = this.MigrationMetaData.Version.ToString();
                sqlCommand.Parameters.Add("name", NpgsqlDbType.Text).Value = this.MigrationMetaData.Name;
                sqlCommand.Parameters.Add("date", NpgsqlDbType.Text).Value = this.MigrationMetaData.MigrationScriptDate.ToString(CultureInfo.InvariantCulture);
                sqlCommand.Parameters.Add("scope", NpgsqlDbType.Text).Value = this.MigrationMetaData.MigrationScriptApplicationKind.ToString();
                sqlCommand.Parameters.Add("resourceName", NpgsqlDbType.Text).Value = this.MigrationMetaData.ResourceName;

                sqlCommand.Connection = transaction.Connection;
                sqlCommand.Transaction = transaction;
                sqlCommand.ExecuteNonQuery();
            }
        }
    }
}
