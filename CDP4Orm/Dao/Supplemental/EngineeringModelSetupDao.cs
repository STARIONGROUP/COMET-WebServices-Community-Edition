// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelSetupDao.cs" company="Starion Group S.A.">
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

namespace CDP4Orm.Dao
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using Microsoft.Extensions.Logging;

    using MigrationEngine;

    using Npgsql;

    using NpgsqlTypes;

    /// <summary>
    /// The engineering model setup dao supplemental code.
    /// </summary>
    public partial class EngineeringModelSetupDao
    {
        /// <summary>
        /// Gets or sets the <see cref="IMigrationService"/> (injected)
        /// </summary>
        public IMigrationService MigrationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ILogger{T}"/>
        /// </summary>
        public ILogger<EngineeringModelSetupDao> Logger { get; set; }

        /// <summary>
        /// Gets or sets the injected <see cref="IParticipantDao"/>
        /// </summary>
        public IParticipantDao ParticipantDao { get; set; }

        /// <summary>
        /// Read the data from the database based on <see cref="Person"/> id.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="personId">
        /// The <see cref="Person.Iid"/> to retrieve participant info for from the database.
        /// </param>
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>
        /// List of instances of <see cref="EngineeringModelSetup"/>.
        /// </returns>
        public async Task<IEnumerable<EngineeringModelSetup>> ReadByPersonAsync(NpgsqlTransaction transaction, string partition, Guid personId, DateTime? instant = null)
        {
            await using var command = new NpgsqlCommand();

            var sqlBuilder = new System.Text.StringBuilder();

            sqlBuilder.Append(this.BuildReadQuery(partition, null));

            if (!personId.Equals(Guid.Empty))
            {
                sqlBuilder.Append($" WHERE \"Participant\" && (SELECT array_agg(\"Iid\"::text) FROM ({this.ParticipantDao.BuildReadQuery(partition, instant)}) Participant WHERE \"Person\" = :personId AND \"ValueTypeSet\"->'IsActive' = 'True')");
                command.Parameters.Add("personId", NpgsqlDbType.Uuid).Value = personId;
            }

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                command.Parameters.Add("instant", NpgsqlDbType.Timestamp).Value = instant;
            }

            sqlBuilder.Append(';');

            command.Connection = transaction.Connection;
            command.Transaction = transaction;
            command.CommandText = sqlBuilder.ToString();

            await using var reader = await command.ExecuteReaderAsync();

            var result= new List<EngineeringModelSetup>();

            while (await reader.ReadAsync())
            {
                result.Add(this.MapToDto(reader));
            }

            return result;
        }

        /// <summary>
        /// Execute additional logic after each write function call.
        /// </summary>
        /// <param name="writeResult">
        /// The result of the write function Result.
        /// </param>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="partition">
        /// The partition.
        /// </param>
        /// <param name="thing">
        /// The thing.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        /// <remarks>
        /// THIS MUST BE "new" AND NOT OVERRIDE. Else this will be called by every abstract layer of the EngineeringModelSetupDao.
        /// </remarks>
        public new async Task<bool> AfterWriteAsync(bool writeResult, NpgsqlTransaction transaction, string partition, Thing thing, Thing container)
        {
            var stopwatch = Stopwatch.StartNew();

            var result = await base.AfterWriteAsync(writeResult, transaction, partition, thing, container);
            
            var engineeringModelSetup = (EngineeringModelSetup)thing;
            
            // insert the engineeringmodel schema
            var replacementInfo = new List<Tuple<string, string>>();
            var engineeringModelPartition = Utils.GetEngineeringModelSchemaName(engineeringModelSetup.EngineeringModelIid);
            var iterationPartition = Utils.GetEngineeringModelIterationSchemaName(engineeringModelSetup.EngineeringModelIid);

            replacementInfo.Add(new Tuple<string, string>(
                "EngineeringModel_REPLACE",
                $"{engineeringModelPartition}"));

            // support iteration sub partition (schema) which will have the same engineeringmodel identifier applied
            replacementInfo.Add(new Tuple<string, string>(
                "Iteration_REPLACE",
                $"{iterationPartition}"));

            await using (var command = new NpgsqlCommand())
            {
                command.Connection = transaction.Connection;
                command.Transaction = transaction;
                await ExecuteEngineeringModelSchemaScriptsAsync(command, replacementInfo);
            }

            stopwatch.Stop();
            this.Logger.LogDebug("EngineeringModelDefinition.sql scripts took {ElapsedMilliseconds}[ms]", stopwatch.ElapsedMilliseconds);

            stopwatch.Reset();
            stopwatch.Start();
            await this.MigrationService.ApplyMigrationsAsync(transaction, engineeringModelPartition, false);
            await this.MigrationService.ApplyMigrationsAsync(transaction, iterationPartition, false);

            stopwatch.Stop();
            this.Logger.LogDebug("Migration applied in {ElapsedMilliseconds}[ms]", stopwatch.ElapsedMilliseconds);
            return result;
        }

        /// <summary>
        /// Executes all SQL scripts to create EngineeringModel and Iteration schema
        /// </summary>
        /// <param name="sqlCommand">The <see cref="NpgsqlCommand"/></param>
        /// <param name="replacementInfo">The collection of replacement information</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation</returns>
        private static async Task ExecuteEngineeringModelSchemaScriptsAsync(NpgsqlCommand sqlCommand, IReadOnlyCollection<Tuple<string, string>> replacementInfo)
        {
            sqlCommand.ReadSqlFromResource("CDP4Orm.AutoGenStructure.04_EngineeringModel_setup.sql", replace: replacementInfo);
            await sqlCommand.ExecuteNonQueryAsync();
            sqlCommand.ReadSqlFromResource("CDP4Orm.AutoGenStructure.05_EngineeringModel_structure.sql", replace: replacementInfo);
            await sqlCommand.ExecuteNonQueryAsync();
            sqlCommand.ReadSqlFromResource("CDP4Orm.AutoGenStructure.06_EngineeringModel_triggers.sql", replace: replacementInfo);
            await sqlCommand.ExecuteNonQueryAsync();
        }
    }
}
