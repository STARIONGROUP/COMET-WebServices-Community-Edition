// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MigrationEngine.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
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

namespace CometServer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Orm.MigrationEngine;

    using CometServer.Configuration;
    using CometServer.Services;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using Npgsql;

    /// <summary>
    /// The migration engine that handle database migration at start-up
    /// </summary>
    public class MigrationEngine : IMigrationEngine
    {
        /// <summary>
        /// Gets or sets the (injected) <see cref="ILogger"/>
        /// </summary>
        private readonly ILogger<MigrationEngine> logger;

        /// <summary>
        /// Gets or sets the (injected) <see cref="IAppConfigService"/>
        /// </summary>
        public IAppConfigService AppConfigService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IMigrationService"/>
        /// </summary>
        public IMigrationService MigrationService { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationEngine"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> us to create typed loggers
        /// </param>
        public MigrationEngine(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory == null ? NullLogger<MigrationEngine>.Instance : loggerFactory.CreateLogger<MigrationEngine>();
        }

        /// <summary>
        /// Apply migration scripts at start-up
        /// </summary>
        public async Task<bool> MigrateAllAtStartUpAsync()
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;
            var sw = Stopwatch.StartNew();

            this.logger.LogInformation("Start migration");

            try
            {
                connection = new NpgsqlConnection(Utils.GetConnectionString(this.AppConfigService.AppConfig.Backtier, this.AppConfigService.AppConfig.Backtier.Database));

                // ensure an open connection
                if (connection.State != ConnectionState.Open)
                {
                    try
                    {
                        await connection.OpenAsync();
                    }
                    catch (PostgresException e)
                    {
                        this.logger.LogWarning(e, "Could not connect to the database for migration, the database might not exist yet. Error message: {Message}", e.Message);
                        return false;
                    }
                }

                // start transaction with rollback support
                transaction = await connection.BeginTransactionAsync();

                // list all schema where the migration script shall be applied on
                var existingSchemas = new List<string>();

                await using (var schemaListCmd = new NpgsqlCommand("select nspname from pg_catalog.pg_namespace", transaction.Connection, transaction))
                {
                    await using (var reader = await schemaListCmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
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

                await using (var appliedMigrationCmd = new NpgsqlCommand("SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'SiteDirectory' AND table_name = 'MigrationManagement')", transaction.Connection, transaction))
                {
                    await using (var reader = await appliedMigrationCmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            doesMigrationTableExist = bool.Parse(reader[0].ToString());
                        }
                    }
                }

                if (doesMigrationTableExist)
                {
                    await using var appliedMigrationCmd = new NpgsqlCommand("SELECT \"version\" FROM \"SiteDirectory\".\"MigrationManagement\"", transaction.Connection, transaction);

                    await using var reader = await appliedMigrationCmd.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        existingMigrations.Add(new Version(reader[0].ToString()));
                    }
                }

                // exclude migration of already applied migrations at start up
                migrations.AddRange(this.MigrationService.GetMigrations(true).Where(x => !existingMigrations.Contains(x.MigrationMetaData.Version)));

                foreach (var migration in migrations.Where(x => x.MigrationMetaData.MigrationScriptApplicationKind == MigrationScriptApplicationKind.SiteDirectory || x.MigrationMetaData.MigrationScriptApplicationKind == MigrationScriptApplicationKind.All).OrderBy(x => x.MigrationMetaData.Version))
                {
                    await migration.ApplyMigration(transaction, existingSchemas.Where(x => x.StartsWith(MigrationScriptApplicationKind.SiteDirectory.ToString())).ToList());
                }

                foreach (var migration in migrations.Where(x => x.MigrationMetaData.MigrationScriptApplicationKind == MigrationScriptApplicationKind.EngineeringModel || x.MigrationMetaData.MigrationScriptApplicationKind == MigrationScriptApplicationKind.All).OrderBy(x => x.MigrationMetaData.Version))
                {
                    // apply migration on all EngineeringModel schema
                    await migration.ApplyMigration(transaction, existingSchemas.Where(x => x.StartsWith(MigrationScriptApplicationKind.EngineeringModel.ToString())).ToList());
                }

                foreach (var migration in migrations.Where(x => x.MigrationMetaData.MigrationScriptApplicationKind == MigrationScriptApplicationKind.Iteration || x.MigrationMetaData.MigrationScriptApplicationKind == MigrationScriptApplicationKind.All).OrderBy(x => x.MigrationMetaData.Version))
                {
                    // apply migration on all Iteration schema
                    await migration.ApplyMigration(transaction, existingSchemas.Where(x => x.StartsWith(MigrationScriptApplicationKind.Iteration.ToString())).ToList());
                }

                await transaction.CommitAsync();

                this.logger.LogInformation("Migration done in {ElapsedMilliseconds} ms.", sw.ElapsedMilliseconds);

                return true;
            }
            catch (Exception exception)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogError(exception, "An error occured during migration.");
                throw;
            }
            finally
            {
                if (transaction != null)
                {
                    await transaction.DisposeAsync();
                }

                if (connection != null)
                {
                    await connection.DisposeAsync();
                }
            }
        }
    }
}
