// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelSetupDao.cs" company="RHEA System S.A.">
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

namespace CDP4Orm.Dao
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

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
        /// <returns>
        /// List of instances of <see cref="EngineeringModelSetup"/>.
        /// </returns>
        public IEnumerable<EngineeringModelSetup> ReadByPerson(NpgsqlTransaction transaction, string partition, Guid personId)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("SELECT * FROM \"{0}\".\"EngineeringModelSetup_View\"", partition);

                if (!personId.Equals(Guid.Empty))
                {
                    sqlBuilder.AppendFormat(" WHERE \"Participant\" && (SELECT array_agg(\"Iid\"::text) FROM \"{0}\".\"Participant_View\" WHERE \"Person\" = :personId AND \"ValueTypeSet\"->'IsActive' = 'True')", partition);
                    command.Parameters.Add("personId", NpgsqlDbType.Uuid).Value = personId;
                }

                sqlBuilder.Append(";");

                command.Connection = transaction.Connection;
                command.Transaction = transaction;
                command.CommandText = sqlBuilder.ToString();
                
                // log the sql command 
                this.LogCommand(command);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return this.MapToDto(reader);
                    }
                }
            }
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
        public new bool AfterWrite(bool writeResult, NpgsqlTransaction transaction, string partition, Thing thing, Thing container)
        {
            var result = base.AfterWrite(writeResult, transaction, partition, thing, container);

            var engineeringModelSetup = (EngineeringModelSetup)thing;
            
            // insert the engineeringmodel schema
            var replacementInfo = new List<Tuple<string, string>>();
            var engineeringModelPartition = Utils.GetEngineeringModelSchemaName(engineeringModelSetup.EngineeringModelIid);
            var iterationPartition = Utils.GetEngineeringModelIterationSchemaName(engineeringModelSetup.EngineeringModelIid);

            replacementInfo.Add(new Tuple<string, string>(
                "EngineeringModel_REPLACE",
                string.Format("{0}", engineeringModelPartition)));

            // support iteration sub partition (schema) which will have the same engineeringmodel identifier applied
            replacementInfo.Add(new Tuple<string, string>(
                "Iteration_REPLACE",
                string.Format("{0}", iterationPartition)));

            using (var command = new NpgsqlCommand())
            {
                command.ReadSqlFromResource(
                    "CDP4Orm.AutoGenStructure.EngineeringModelDefinition.sql",
                    replace: replacementInfo);

                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                this.ExecuteAndLogCommand(command);
            }

            this.MigrationService.ApplyMigrations(transaction, engineeringModelPartition, false);
            this.MigrationService.ApplyMigrations(transaction, iterationPartition, false);

            return result;
        }
    }
}
