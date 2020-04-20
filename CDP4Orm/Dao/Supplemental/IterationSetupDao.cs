// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationSetupDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2020 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using CDP4Common.DTO;
    using Npgsql;
    using NpgsqlTypes;

    /// <summary>
    /// The iteration setup dao.
    /// </summary>
    public partial class IterationSetupDao
    {
        /// <summary>
        /// Read the data from the database.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="iterationId">
        /// The iteration Id.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="CDP4Common.DTO.IterationSetup"/>.
        /// </returns>
        public virtual IEnumerable<IterationSetup> ReadByIteration(NpgsqlTransaction transaction, string partition, Guid iterationId)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new StringBuilder();

                sqlBuilder.AppendFormat("SELECT * FROM \"{0}\".\"IterationSetup_View\"", partition);
                sqlBuilder.Append(" WHERE \"ValueTypeSet\" -> 'IterationIid' = :iterationIid");
                command.Parameters.Add("iterationIid", NpgsqlDbType.Text).Value = iterationId.ToString();

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
        /// Read the data from the database based on <see cref="EngineeringModelSetup.Iid"/>.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="engineeringModelSetupId">
        /// The <see cref="EngineeringModelSetup"/> Id.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="CDP4Common.DTO.IterationSetup"/>.
        /// </returns>
        public IEnumerable<IterationSetup> ReadByEngineeringModelSetup(NpgsqlTransaction transaction, string partition, Guid engineeringModelSetupId)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new StringBuilder();

                sqlBuilder.AppendFormat("SELECT * FROM \"{0}\".\"IterationSetup_View\"", partition);
                sqlBuilder.AppendFormat(" WHERE \"Iid\"::text = ANY(SELECT unnest(\"IterationSetup\") FROM \"{0}\".\"EngineeringModelSetup_View\" WHERE \"Iid\"::text = :engineeringModelSetupId)", partition);

                command.Parameters.Add("engineeringModelSetupId", NpgsqlDbType.Text).Value = engineeringModelSetupId.ToString();

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
        /// Execute additional logic before each delete function call.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be deleted.
        /// </param>
        /// <param name="iid">
        /// The thing DTO id that is to be deleted.
        /// </param>
        /// <param name="isHandled">
        /// Logic flag that can be set to true to skip the generated deleted logic
        /// </param>
        /// <returns>
        /// True if the concept was deleted.
        /// </returns>
        /// <remarks>
        /// IterationSetups cannot be deleted at all.
        /// </remarks>>
        public override bool BeforeDelete(NpgsqlTransaction transaction, string partition, Guid iid, out bool isHandled)
        {
            isHandled = true;
            return true;
        }
    }
}
