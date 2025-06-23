// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationSetupDao.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2023 Starion Group S.A.
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
    using System.Text;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using Npgsql;

    using NpgsqlTypes;

    /// <summary>
    /// The iteration setup dao.
    /// </summary>
    public partial class IterationSetupDao
    {
        /// <summary>
        /// Gets or sets the injected <see cref="IEngineeringModelSetupDao"/>
        /// </summary>
        public IEngineeringModelSetupDao EngineeringModelSetupDao { get; set; }

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
        /// <param name="instant">
        /// The instant as a nullable <see cref="DateTime"/>
        /// </param>
        /// <returns>
        /// List of instances of <see cref="CDP4Common.DTO.IterationSetup"/>.
        /// </returns>
        public virtual async Task<IEnumerable<IterationSetup>> ReadByIterationAsync(NpgsqlTransaction transaction, string partition, Guid iterationId, DateTime? instant = null)
        {
            await using var command = new NpgsqlCommand();

            var sqlBuilder = new StringBuilder();

            sqlBuilder.Append($"{this.BuildReadQuery(partition, instant)}");
            sqlBuilder.Append($" WHERE {this.GetValueTypeSet()} -> 'IterationIid' = :iterationIid");
            command.Parameters.Add("iterationIid", NpgsqlDbType.Text).Value = iterationId.ToString();

            if (instant.HasValue && instant.Value != DateTime.MaxValue)
            {
                command.Parameters.Add("instant", NpgsqlDbType.Timestamp).Value = instant;
            }

            sqlBuilder.Append(';');

            command.Connection = transaction.Connection;
            command.Transaction = transaction;
            command.CommandText = sqlBuilder.ToString();

            await using var reader = await command.ExecuteReaderAsync();

            var result = new List<IterationSetup>();

            while (await reader.ReadAsync())
            {
                var dto = this.MapToDto(reader);
                result.Add(dto);
            }

            return result;
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
