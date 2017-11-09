// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelSetupDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    using System;
    using System.Collections.Generic;
    using CDP4Common.DTO;
    using Npgsql;
    using NpgsqlTypes;

    /// <summary>
    /// The engineering model setup dao supplemental code.
    /// </summary>
    public partial class EngineeringModelSetupDao
    {
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
                    sqlBuilder.AppendFormat(" WHERE \"Participant\" && (SELECT array_agg(\"Iid\"::text) FROM \"{0}\".\"Participant_View\" WHERE \"Person\" = :personId)", partition);
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
        public new bool AfterWrite(bool writeResult, NpgsqlTransaction transaction, string partition, Thing thing, Thing container)
        {
            var result = base.AfterWrite(writeResult, transaction, partition, thing, container);

            var engineeringModelSetup = (EngineeringModelSetup)thing;
            
            // insert the engineeringmodel schema
            var replacementInfo = new List<Tuple<string, string>>();

            replacementInfo.Add(new Tuple<string, string>(
                "EngineeringModel_REPLACE",
                string.Format("{0}", Utils.GetEngineeringModelSchemaName(engineeringModelSetup.EngineeringModelIid))));

            // support iteration sub partition (schema) which will have the same engineeringmodel identifier applied
            replacementInfo.Add(new Tuple<string, string>(
                "Iteration_REPLACE",
                string.Format("{0}", Utils.GetEngineeringModelIterationSchemaName(engineeringModelSetup.EngineeringModelIid))));

            using (var command = new NpgsqlCommand())
            {
                command.ReadSqlFromResource(
                    "CDP4Orm.AutoGenStructure.EngineeringModelDefinition.sql",
                    replace: replacementInfo);

                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                this.ExecuteAndLogCommand(command);
            }

            return result;
        }
    }
}
