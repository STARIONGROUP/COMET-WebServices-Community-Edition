// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationBase.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.MigrationEngine
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using NLog;
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
