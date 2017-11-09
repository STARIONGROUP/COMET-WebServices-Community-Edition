// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainOfExpertiseDao.cs" company="RHEA System S.A.">
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
    /// The domain of expertise dao supplemental code.
    /// </summary>
    public partial class DomainOfExpertiseDao
    {
        /// <summary>
        /// Read the data from the database based on <see cref="Person"/> id and an <see cref="EngineeringModelSetup"/> id.
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
        /// <param name="engineeringModelSetupId">
        /// The <see cref="EngineeringModelSetup.Iid"/> to retrieve domain info for from the database.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="DomainOfExpertise"/>.
        /// </returns>
        public IEnumerable<DomainOfExpertise> ReadByPersonAndEngineeringModelSetup(NpgsqlTransaction transaction, string partition, Guid personId, Guid engineeringModelSetupId)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("SELECT * FROM \"{0}\".\"DomainOfExpertise_View\"", partition);

                if (!personId.Equals(Guid.Empty) && !engineeringModelSetupId.Equals(Guid.Empty))
                {
                    sqlBuilder.AppendFormat(" WHERE \"Iid\"::text = ANY(SELECT unnest(\"Domain\") FROM \"{0}\".\"Participant_View\" WHERE \"Person\" = :personId AND \"Iid\"::text = ANY(SELECT unnest(\"Participant\") FROM \"{0}\".\"EngineeringModelSetup_View\" WHERE \"Iid\" = :engineeringModelSetupId ))", partition);
                    command.Parameters.Add("personId", NpgsqlDbType.Uuid).Value = personId;
                    command.Parameters.Add("engineeringModelSetupId", NpgsqlDbType.Uuid).Value = engineeringModelSetupId;
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
    }
}
