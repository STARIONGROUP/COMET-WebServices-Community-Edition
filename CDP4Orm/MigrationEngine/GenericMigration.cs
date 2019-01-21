// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericMigration.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2018 RHEA System S.A.
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
                var replace = new Tuple<string, string>(SCHEMA_NAME_REPLACE, applicableSchema);
                using (var sqlCommand = new NpgsqlCommand())
                {
                    sqlCommand.ReadSqlFromResource(this.MigrationMetaData.ResourceName, null, new [] { replace });

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
