// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2019 RHEA System S.A.
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
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("SELECT \"SiteDirectory\".end_all_current_data_validity(:partitionname);");
                command.Parameters.Add("partitionname", NpgsqlDbType.Text).Value = partition;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;
                this.ExecuteAndLogCommand(command);
            }
        }

        /// <summary>
        /// Insert data in the "current" tables using the audit table data at a specific <paramref name="instant"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="instant">The instant that matches an iteration</param>
        public void InsertDataFromAudit(NpgsqlTransaction transaction, string partition, DateTime instant)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();
                sqlBuilder.AppendFormat("SELECT \"SiteDirectory\".insert_data_from_audit(:partitionname, :instant);");
                command.Parameters.Add("partitionname", NpgsqlDbType.Text).Value = partition;
                command.Parameters.Add("instant", NpgsqlDbType.Timestamp).Value = instant;

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;
                this.ExecuteAndLogCommand(command);
            }
        }

        /// <summary>
        /// Deletes all things of the "current" tables
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The iteration partition</param>
        public void DeleteAllIterationThings(NpgsqlTransaction transaction, string partition)
        {
            using (var command = new NpgsqlCommand())
            {
                var sqlBuilder = new System.Text.StringBuilder();

                sqlBuilder.AppendFormat("DELETE FROM \"{0}\".\"Thing\";", partition);

                command.CommandText = sqlBuilder.ToString();
                command.Connection = transaction.Connection;
                command.Transaction = transaction;
                this.ExecuteAndLogCommand(command);
            }
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
            this.UpdateContainment(transaction, partition, typeof(Option), thing);
            this.UpdateContainment(transaction, partition, typeof(Publication), thing);
            this.UpdateContainment(transaction, partition, typeof(PossibleFiniteStateList), thing);
            this.UpdateContainment(transaction, partition, typeof(ElementDefinition), thing);
            this.UpdateContainment(transaction, partition, typeof(Relationship), thing);
            this.UpdateContainment(transaction, partition, typeof(ExternalIdentifierMap), thing);
            this.UpdateContainment(transaction, partition, typeof(RequirementsSpecification), thing);
            this.UpdateContainment(transaction, partition, typeof(DomainFileStore), thing);
            this.UpdateContainment(transaction, partition, typeof(ActualFiniteStateList), thing);
            this.UpdateContainment(transaction, partition, typeof(RuleVerificationList), thing);
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
        private void UpdateContainment(NpgsqlTransaction transaction, string partition, Type containedType, Thing thing)
        {
            using (var command = new NpgsqlCommand())
            {
                command.CommandText = string.Format("UPDATE \"{0}\".\"{1}\" SET \"Container\" = :container;", partition, containedType.Name);
                command.Parameters.Add("container", NpgsqlDbType.Uuid).Value = thing.Iid;
                command.Connection = transaction.Connection;
                command.Transaction = transaction;

                this.ExecuteAndLogCommand(command);
            }
        }
    }
}
