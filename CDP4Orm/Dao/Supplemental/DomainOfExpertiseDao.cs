// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainOfExpertiseDao.cs" company="Starion Group S.A.">
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
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using Npgsql;

    using NpgsqlTypes;

    /// <summary>
    /// The domain of expertise dao supplemental code.
    /// </summary>
    public partial class DomainOfExpertiseDao
    {
        /// <summary>
        /// Gets or sets the injected <see cref="IParticipantDao"/>
        /// </summary>
        public IParticipantDao ParticipantDao { get; set; }

        /// <summary>
        /// Gets or sets the injected <see cref="IEngineeringModelSetupDao"/>
        /// </summary>
        public IEngineeringModelSetupDao EngineeringModelSetupDao { get; set; }

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
        /// <param name="instant">
        /// The instant as a <see cref="DateTime"/>
        /// </param>
        /// <returns>
        /// List of instances of <see cref="DomainOfExpertise"/>.
        /// </returns>
        public async Task<IEnumerable<DomainOfExpertise>> ReadByPersonAndEngineeringModelSetupAsync(NpgsqlTransaction transaction, string partition, Guid personId, Guid engineeringModelSetupId, DateTime? instant = null)
        {
            await using var command = new NpgsqlCommand();

            var sqlBuilder = new System.Text.StringBuilder();

            sqlBuilder.Append(this.BuildReadQuery(partition, instant));

            if (!personId.Equals(Guid.Empty) && !engineeringModelSetupId.Equals(Guid.Empty))
            {
                sqlBuilder.Append($" WHERE \"Iid\"::text = ANY(SELECT unnest(\"Domain\") FROM ({this.ParticipantDao.BuildReadQuery(partition, instant)}) Participant WHERE \"Person\" = :personId AND \"Iid\"::text = ANY(SELECT unnest(\"Participant\") FROM ({this.EngineeringModelSetupDao.BuildReadQuery(partition, instant)}) EngineeringModelSetup WHERE \"Iid\" = :engineeringModelSetupId ))");
                command.Parameters.Add("personId", NpgsqlDbType.Uuid).Value = personId;
                command.Parameters.Add("engineeringModelSetupId", NpgsqlDbType.Uuid).Value = engineeringModelSetupId;
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

            var result = new List<DomainOfExpertise>();

            while (await reader.ReadAsync())
            {
                var domainOfExpertise = this.MapToDto(reader);
                result.Add(domainOfExpertise);
            }

            return result;
        }
    }
}
