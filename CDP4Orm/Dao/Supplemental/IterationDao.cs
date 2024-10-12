// --------------------------------------------------------------------------------------------------------------------
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
        public void SetIterationValidityEnd(NpgsqlTransaction transaction, string partition)
        {
            using var command = new NpgsqlCommand();

            const string sql = "SELECT \"SiteDirectory\".end_all_current_data_validity(:partitionname);";

            command.Parameters.Add("partitionname", NpgsqlDbType.Text).Value = partition;

            command.CommandText = sql;
            command.Connection = transaction.Connection;
            command.Transaction = transaction;
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Insert data in the "current" tables using the audit table data at a specific <paramref name="instant"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="instant">The instant that matches an iteration</param>
        public void InsertDataFromAudit(NpgsqlTransaction transaction, string partition, DateTime instant)
        {
            using var command = new NpgsqlCommand();

            const string sql = "SELECT \"SiteDirectory\".insert_data_from_audit(:partitionname, :instant);";

            command.Parameters.Add("partitionname", NpgsqlDbType.Text).Value = partition;
            command.Parameters.Add("instant", NpgsqlDbType.Timestamp).Value = instant;

            command.CommandText = sql;
            command.Connection = transaction.Connection;
            command.Transaction = transaction;
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes all things of the "current" tables
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The iteration partition</param>
        public void DeleteAllIterationThings(NpgsqlTransaction transaction, string partition)
        {
            if (!CDP4Orm.Helper.StringExtensions.IsValidPartitionName(partition))
            {
                throw new ArgumentException("partition format is invalid. It must start with alphabetic characters and can be followed by segments of lowercase letters, numbers, and underscores.");
            }

            using var command = new NpgsqlCommand();

            var sqlBuilder = new StringBuilder();

            sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"Thing\";", partition);

            command.CommandText = sqlBuilder.ToString();
            command.Connection = transaction.Connection;
            command.Transaction = transaction;
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes all OrganizationalParticipant links
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The iteration partition</param>
        public void DeleteAllrganizationalParticipantThings(NpgsqlTransaction transaction, string partition)
        {
            if (!CDP4Orm.Helper.StringExtensions.IsValidPartitionName(partition))
            {
                throw new ArgumentException("partition format is invalid. It must start with alphabetic characters and can be followed by segments of lowercase letters, numbers, and underscores.");
            }

            using var command = new NpgsqlCommand();

            var sqlBuilder = new StringBuilder();

            sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"ElementDefinition_OrganizationalParticipant\";", partition);

            command.CommandText = sqlBuilder.ToString();
            command.Connection = transaction.Connection;
            command.Transaction = transaction;
            command.ExecuteNonQuery();
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
        public void MoveToNextIterationFromLast(NpgsqlTransaction transaction, string partition, Thing thing)
        {
            UpdateContainment(transaction, partition, typeof(Option), thing);
            UpdateContainment(transaction, partition, typeof(Publication), thing);
            UpdateContainment(transaction, partition, typeof(PossibleFiniteStateList), thing);
            UpdateContainment(transaction, partition, typeof(ElementDefinition), thing);
            UpdateContainment(transaction, partition, typeof(Relationship), thing);
            UpdateContainment(transaction, partition, typeof(ExternalIdentifierMap), thing);
            UpdateContainment(transaction, partition, typeof(RequirementsSpecification), thing);
            UpdateContainment(transaction, partition, typeof(DomainFileStore), thing);
            UpdateContainment(transaction, partition, typeof(ActualFiniteStateList), thing);
            UpdateContainment(transaction, partition, typeof(RuleVerificationList), thing);
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
        private static void UpdateContainment(NpgsqlTransaction transaction, string partition, Type containedType, Thing thing)
        {
            if (string.IsNullOrWhiteSpace(partition) || string.IsNullOrWhiteSpace(containedType.Name))
            {
                throw new ArgumentException("Partition or contained type name cannot be null or whitespace.");
            }

            if (!CDP4Orm.Helper.StringExtensions.IsValidPartitionName(partition))
            {
                throw new ArgumentException("partition format is invalid. It must start with alphabetic characters and can be followed by segments of lowercase letters, numbers, and underscores.");
            }

            var containedTypePattern = @"^([A-Za-z]+(\.[A-Za-z]+)*)$";

            if (!Regex.IsMatch(containedType.Name, containedTypePattern))
            {
                throw new ArgumentException("partition format is invalid. It must start with alphabetic characters and can be followed by segments of lowercase letters, numbers, and underscores.");
            }

            using var command = new NpgsqlCommand();

            command.CommandText = $"UPDATE \"{partition}\".\"{containedType.Name}\" SET \"Container\" = :container;";
            command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = thing.Iid;
            command.Connection = transaction.Connection;
            command.Transaction = transaction;

            command.ExecuteNonQuery();
        }
    }
}
