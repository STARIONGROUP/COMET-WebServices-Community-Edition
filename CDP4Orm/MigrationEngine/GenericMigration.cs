// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericMigration.cs" company="RHEA System S.A.">
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Dao;

    using NLog;

    using Npgsql;

    /// <summary>
    /// A class that handles a plain migration script that just needs to be executed fully
    /// </summary>
    public class GenericMigration : MigrationBase
    {
        /// <summary>
        /// The Logger
        /// </summary>
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericMigration"/> class
        /// </summary>
        /// <param name="migrationMetadata">Themigration metadata</param>
        internal GenericMigration(MigrationMetaData migrationMetadata) : base(migrationMetadata)
        {
        }

        /// <summary>
        /// Apply the current migration on the specified schema if applicable
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="existingSchemas">The schema on which the migration shall be applied on</param>
        public override void ApplyMigration(NpgsqlTransaction transaction, IReadOnlyList<string> existingSchemas)
        {
            Logger.Info("Start migration script {0}", this.MigrationMetaData.ResourceName);
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
                Logger.Info(" no schema to apply the migration on");
                return;
            }

            foreach (var applicableSchema in applicableSchemas)
            {
                var replaceList = new List<Tuple<string, string>>();

                // using the actual schema name in the generic script to execute
                var replace = new Tuple<string, string>(SCHEMA_NAME_REPLACE, applicableSchema);
                replaceList.Add(replace);

                // allow possibility of replacing engineeringmodel tokens in iterations
                if (this.MigrationMetaData.MigrationScriptApplicationKind == MigrationScriptApplicationKind.Iteration)
                {
                    if (applicableSchema.Contains("Iteration_"))
                    {
                        var applicableEngineeringModelSchema = applicableSchema.Replace("Iteration_", "EngineeringModel_");
                        replaceList.Add(new Tuple<string, string>(ENGINEERING_MODEL_REPLACE, applicableEngineeringModelSchema));
                    }
                }

                using (var sqlCommand = new NpgsqlCommand())
                {
                    sqlCommand.ReadSqlFromResource(this.MigrationMetaData.ResourceName, null, replaceList);

                    sqlCommand.Connection = transaction.Connection;
                    sqlCommand.Transaction = transaction;
                    sqlCommand.ExecuteNonQuery();
                    Logger.Info("End migration script {0}", this.MigrationMetaData.ResourceName);
                }
            }

            base.ApplyMigration(transaction, existingSchemas);
        }
    }
}
