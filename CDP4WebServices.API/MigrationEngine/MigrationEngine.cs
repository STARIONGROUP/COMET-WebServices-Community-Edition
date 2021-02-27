// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationEngine.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2020 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;

    using CDP4Orm.MigrationEngine;

    using CDP4WebServices.API.Configuration;
    using CDP4WebServices.API.Services;

    using NLog;

    using Npgsql;

    /// <summary>
    /// The migration engine that handle database migration at start-up
    /// </summary>
    public static class MigrationEngine
    {
        /// <summary>
        /// The Logger
        /// </summary>
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Apply migration scripts at start-up
        /// </summary>
        public static void MigrateAllAtStartUp()
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;
            var sw = Stopwatch.StartNew();

            Logger.Info("Start migration.");

            try
            {
                // setup connection if not supplied
                connection = new NpgsqlConnection(Utils.GetConnectionString(AppConfig.Current.Backtier.Database));

                // ensure an open connection
                if (connection.State != ConnectionState.Open)
                {
                    try
                    {
                        connection.Open();
                    }
                    catch (PostgresException e)
                    {
                        Logger.Warn("Could not connect to the database for migration, the database might not exist yet. Skipping migration. Error message: {0}", e.Message);
                        return;
                    }
                }

                // start transaction with rollback support
                transaction = connection.BeginTransaction();

                // list all schema where the migration script shall be applied on
                var existingSchemas = new List<string>();
                using (var schemaListCmd = new NpgsqlCommand("select nspname from pg_catalog.pg_namespace", transaction.Connection, transaction))
                {
                    using (var reader = schemaListCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var schemaName = reader[0].ToString();
                            if (schemaName.StartsWith(MigrationScriptApplicationKind.SiteDirectory.ToString())
                                || schemaName.StartsWith(MigrationScriptApplicationKind.EngineeringModel.ToString())
                                || schemaName.StartsWith(MigrationScriptApplicationKind.Iteration.ToString()))
                            {
                                existingSchemas.Add(schemaName);
                            }
                        }
                    }
                }

                // only apply migrations that have not been applied before (not present in the migration table)
                var migrations = new List<MigrationBase>();
                var existingMigrations = new List<Version>();
                var doesMigrationTableExist = false;

                using (var appliedMigrationCmd = new NpgsqlCommand("SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'SiteDirectory' AND table_name = 'MigrationManagement')", transaction.Connection, transaction))
                {
                    using (var reader = appliedMigrationCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            doesMigrationTableExist = bool.Parse(reader[0].ToString());
                        }
                    }
                }

                if (doesMigrationTableExist)
                {
                    using (var appliedMigrationCmd = new NpgsqlCommand("SELECT \"version\" FROM \"SiteDirectory\".\"MigrationManagement\"", transaction.Connection, transaction))
                    {
                        using (var reader = appliedMigrationCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                existingMigrations.Add(new Version(reader[0].ToString()));
                            }
                        }
                    }
                }

                // exclude migration of already applied migrations at start up
                migrations.AddRange(MigrationService.GetMigrations(true).Where(x => !existingMigrations.Contains(x.MigrationMetaData.Version)));
                foreach (var migration in migrations.Where(x => x.MigrationMetaData.MigrationScriptApplicationKind == MigrationScriptApplicationKind.SiteDirectory || x.MigrationMetaData.MigrationScriptApplicationKind == MigrationScriptApplicationKind.All).OrderBy(x => x.MigrationMetaData.Version))
                {
                    migration.ApplyMigration(transaction, existingSchemas.Where(x => x.StartsWith(MigrationScriptApplicationKind.SiteDirectory.ToString())).ToList());
                }

                foreach (var migration in migrations.Where(x => x.MigrationMetaData.MigrationScriptApplicationKind == MigrationScriptApplicationKind.EngineeringModel || x.MigrationMetaData.MigrationScriptApplicationKind == MigrationScriptApplicationKind.All).OrderBy(x => x.MigrationMetaData.Version))
                {
                    // apply migration on all EngineeringModel schema
                    migration.ApplyMigration(transaction, existingSchemas.Where(x => x.StartsWith(MigrationScriptApplicationKind.EngineeringModel.ToString())).ToList());
                }

                foreach (var migration in migrations.Where(x => x.MigrationMetaData.MigrationScriptApplicationKind == MigrationScriptApplicationKind.Iteration || x.MigrationMetaData.MigrationScriptApplicationKind == MigrationScriptApplicationKind.All).OrderBy(x => x.MigrationMetaData.Version))
                {
                    // apply migration on all Iteration schema
                    migration.ApplyMigration(transaction, existingSchemas.Where(x => x.StartsWith(MigrationScriptApplicationKind.Iteration.ToString())).ToList());
                }

                transaction.Commit();
            }
            catch (Exception e)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                Logger.Error("An error occured during migration.");
                Logger.Error(e.Message);
                throw;
            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }

                if (connection != null)
                {
                    connection.Dispose();
                }

                Logger.Info("Migration done in {0} ms.", sw.ElapsedMilliseconds);
            }
        }
    }
}
