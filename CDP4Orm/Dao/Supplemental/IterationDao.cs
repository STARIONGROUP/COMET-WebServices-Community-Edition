﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationDao.cs" company="Starion Group S.A.">
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
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using Npgsql;

    using NpgsqlTypes;

    /// <summary>
    /// The Iteration Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class IterationDao
    {
        /// <summary>
        /// Set the end-validity of all iteration data
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The iteration partition</param>
        public async Task SetIterationValidityEndAsync(NpgsqlTransaction transaction, string partition)
        {
            await using var command = new NpgsqlCommand();

            const string sql = "SELECT \"SiteDirectory\".end_all_current_data_validity(:partitionname);";

            command.Parameters.Add("partitionname", NpgsqlDbType.Text).Value = partition;

            command.CommandText = sql;
            command.Connection = transaction.Connection;
            command.Transaction = transaction;
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Insert data in the "current" tables using the audit table data at a specific <paramref name="instant"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="instant">The instant that matches an iteration</param>
        public async Task InsertDataFromAuditAsync(NpgsqlTransaction transaction, string partition, DateTime instant)
        {
            await using var command = new NpgsqlCommand();

            const string sql = "SELECT \"SiteDirectory\".insert_data_from_audit(:partitionname, :instant);";

            command.Parameters.Add("partitionname", NpgsqlDbType.Text).Value = partition;
            command.Parameters.Add("instant", NpgsqlDbType.Timestamp).Value = instant;

            command.CommandText = sql;
            command.Connection = transaction.Connection;
            command.Transaction = transaction;
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Deletes all things of the "current" tables
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The iteration partition</param>
        public async Task DeleteAllIterationThingsAsync(NpgsqlTransaction transaction, string partition)
        {
            if (!Helper.StringExtensions.IsValidPartitionName(partition))
            {
                throw new ArgumentException("partition format is invalid. It must start with alphabetic characters and can be followed by segments of lowercase letters, numbers, and underscores.");
            }

            await using var command = new NpgsqlCommand();

            var sqlBuilder = new StringBuilder();

            sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"Thing\";", partition);

            command.CommandText = sqlBuilder.ToString();
            command.Connection = transaction.Connection;
            command.Transaction = transaction;
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Deletes all OrganizationalParticipant links
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The iteration partition</param>
        public async Task DeleteAllrganizationalParticipantThingsAsync(NpgsqlTransaction transaction, string partition)
        {
            if (!Helper.StringExtensions.IsValidPartitionName(partition))
            {
                throw new ArgumentException("partition format is invalid. It must start with alphabetic characters and can be followed by segments of lowercase letters, numbers, and underscores.");
            }

            await using var command = new NpgsqlCommand();

            var sqlBuilder = new StringBuilder();

            sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"ElementDefinition_OrganizationalParticipant\";", partition);

            command.CommandText = sqlBuilder.ToString();
            command.Connection = transaction.Connection;
            command.Transaction = transaction;
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Move the database to the next iteration.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The thing DTO that is to be persisted.
        /// </param>
        public async Task MoveToNextIterationFromLastAsync(NpgsqlTransaction transaction, string partition, Thing thing)
        {
            await UpdateContainment(transaction, partition, typeof(Option), thing);
            await UpdateContainment(transaction, partition, typeof(Publication), thing);
            await UpdateContainment(transaction, partition, typeof(PossibleFiniteStateList), thing);
            await UpdateContainment(transaction, partition, typeof(ElementDefinition), thing);
            await UpdateContainment(transaction, partition, typeof(Relationship), thing);
            await UpdateContainment(transaction, partition, typeof(ExternalIdentifierMap), thing);
            await UpdateContainment(transaction, partition, typeof(RequirementsSpecification), thing);
            await UpdateContainment(transaction, partition, typeof(DomainFileStore), thing);
            await UpdateContainment(transaction, partition, typeof(ActualFiniteStateList), thing);
            await UpdateContainment(transaction, partition, typeof(RuleVerificationList), thing);
        }

        /// <summary>
        /// update containment information
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="containedType">
        /// The contained Type.
        /// </param>
        /// <param name="thing">
        /// The thing DTO that is to be persisted.
        /// </param>
        private static async Task UpdateContainment(NpgsqlTransaction transaction, string partition, Type containedType, Thing thing)
        {
            if (string.IsNullOrWhiteSpace(partition) || string.IsNullOrWhiteSpace(containedType.Name))
            {
                throw new ArgumentException("Partition or contained type name cannot be null or whitespace.");
            }

            if (!Helper.StringExtensions.IsValidPartitionName(partition))
            {
                throw new ArgumentException("partition format is invalid. It must start with alphabetic characters and can be followed by segments of lowercase letters, numbers, and underscores.");
            }

            var containedTypePattern = @"^([A-Za-z]+(\.[A-Za-z]+)*)$";

            if (!Regex.IsMatch(containedType.Name, containedTypePattern))
            {
                throw new ArgumentException("partition format is invalid. It must start with alphabetic characters and can be followed by segments of lowercase letters, numbers, and underscores.");
            }

            await using var command = new NpgsqlCommand();

            command.CommandText = $"UPDATE \"{partition}\".\"{containedType.Name}\" SET \"Container\" = :container;";
            command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = thing.Iid;
            command.Connection = transaction.Connection;
            command.Transaction = transaction;

            await command.ExecuteNonQueryAsync();
        }
    }
}
