// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationSetupDao.cs" company="RHEA System S.A.">
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
