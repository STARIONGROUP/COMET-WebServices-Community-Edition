// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ThingTableMigrationBase.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2023 Starion Group S.A.
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using CDP4Common.CommonData;

    using Microsoft.Extensions.Logging;

    using Npgsql;

    using NpgsqlTypes;

    /// <summary>
    /// A migration script handler class that is used to handle migration script template that are applied on every table
    /// </summary>
    public abstract class ThingTableMigrationBase : MigrationBase
    {
        /// <summary>
        /// The sequence to modify by actual table name in the script
        /// </summary>
        private const string TABLE_REPLACE = "TABLE_REPLACE";

        /// <summary>
        /// The Logger
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThingTableMigrationBase"/> class
        /// </summary>
        /// <param name="migrationFileMetadata">The migration metadata</param>
        /// <param name="logger">
        /// The <see cref="ILogger"/>
        /// </param>
        internal ThingTableMigrationBase(MigrationMetaData migrationFileMetadata, ILogger logger) : base(migrationFileMetadata)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Apply the current migration on the specified schema if applicable
        /// </summary>
        public override void ApplyMigration(NpgsqlTransaction transaction, IReadOnlyList<string> existingSchemas)
        {
            this.logger.LogInformation("Start migration script {ResourceName}", this.MigrationMetaData.ResourceName);

            var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(this.MigrationMetaData.ResourceName);

            if (resource == null)
            {
                throw new FileNotFoundException($"The resource {this.MigrationMetaData.ResourceName} could not be found.");
            }

            var applicableSchemas = this.MigrationMetaData.MigrationScriptApplicationKind == MigrationScriptApplicationKind.All
                ? existingSchemas
                : existingSchemas.Where(x => x.StartsWith(this.MigrationMetaData.MigrationScriptApplicationKind.ToString())).ToList();

            if (applicableSchemas.Count == 0)
            {
                // no schema to apply the migration on
                return;
            }

            var thingTypes = Enum.GetValues(typeof(ClassKind)).Cast<ClassKind>().Select(x => x.ToString()).ToList();

            foreach (var applicableSchema in applicableSchemas)
            {
                var thingTableList = new List<string>();

                using (var tableListCmd = new NpgsqlCommand())
                {
                    this.logger.LogInformation("Getting all {tableNameTemplate} tables for schema {applicableSchema}", this.GetTableNameTemplate(), applicableSchema);

                    var cmdTxt = "SELECT table_name FROM information_schema.tables WHERE table_schema = :schemaName AND table_type = 'BASE TABLE';";

                    tableListCmd.CommandText = cmdTxt;
                    tableListCmd.Parameters.Add("schemaName", NpgsqlDbType.Text).Value = applicableSchema;
                    tableListCmd.Connection = transaction.Connection;
                    tableListCmd.Transaction = transaction;

                    using (var reader = tableListCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tableName = reader[0].ToString();

                            if (this.ApplyFilter(thingTypes, tableName))
                            {
                                thingTableList.Add(tableName);
                            }
                        }
                    }

                    this.logger.LogInformation("{tableNameTemplate} table fetched: {Count}", this.GetTableNameTemplate(), thingTableList.Count);
                }

                using (var sqlCommand = new NpgsqlCommand())
                {
                    var cmdList = new List<string>();

                    using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(this.MigrationMetaData.ResourceName)))
                    {
                        var strText = reader.ReadToEnd();
                        cmdList.AddRange(thingTableList.Select(x => strText.Replace(TABLE_REPLACE, x).Replace(SCHEMA_NAME_REPLACE, applicableSchema)));
                    }

                    sqlCommand.CommandText = string.Join(Environment.NewLine, cmdList);
                    sqlCommand.Connection = transaction.Connection;
                    sqlCommand.Transaction = transaction;
                    sqlCommand.ExecuteNonQuery();
                    this.logger.LogInformation("End migration script {ResourceName}", this.MigrationMetaData.ResourceName);
                }

                base.ApplyMigration(transaction, existingSchemas);
            }
        }

        /// <summary>
        /// Gets the template string that defines the type of ThingTableMigration to be used in for example; logging statements
        /// </summary>
        /// <returns></returns>
        protected abstract string GetTableNameTemplate();

        /// <summary>
        /// Aplies a specific filter on the TableName
        /// </summary>
        /// <param name="thingTypes">The type of things that are allowed to be used for this migration</param>
        /// <param name="tableName">The name of the table that was found while searching the database for tables</param>
        /// <returns>True is the entry is allowed to be used in the Migration</returns>
        protected abstract bool ApplyFilter(List<string> thingTypes, string tableName);
    }
}
