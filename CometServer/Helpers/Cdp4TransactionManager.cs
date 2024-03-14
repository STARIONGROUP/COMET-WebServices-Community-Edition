// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cdp4TransactionManager.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

namespace CometServer.Helpers
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Text;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.Authorization;

    using Configuration;

    using Npgsql;

    using NpgsqlTypes;

    using ServiceUtils = Services.Utils;

    /// <summary>
    /// The purpose of the <see cref="Cdp4TransactionManager"/> is provide a <see cref="NpgsqlTransaction"/> for
    /// read and write operations to the database while configuring the database to properly process
    /// any temporal database interactions
    /// </summary>
    public class Cdp4TransactionManager : ICdp4TransactionManager
    {
        /// <summary>
        /// The SiteDirectory partition
        /// </summary>
        public const string SITE_DIRECTORY_PARTITION = "SiteDirectory";

        /// <summary>
        /// The iteration partition prefix
        /// </summary>
        public const string ITERATION_PARTITION_PREFIX = "Iteration_";

        /// <summary>
        /// The engineering-model partition prefix
        /// </summary>
        public const string ENGINEERING_MODEL_PARTITION_PREFIX = "EngineeringModel_";

        /// <summary>
        /// The transaction info table name.
        /// </summary>
        private const string TransactionInfoTable = "transaction_info";

        /// <summary>
        /// The transaction info table user id column name.
        /// </summary>
        private const string UserIdColumn = "user_id";

        /// <summary>
        /// The transaction info table period start column name.
        /// </summary>
        private const string PeriodStartColumn = "period_start";

        /// <summary>
        /// The transaction info table period end column name.
        /// </summary>
        private const string PeriodEndColumn = "period_end";

        /// <summary>
        /// The transaction info table instant column name.
        /// </summary>
        private const string InstantColumn = "instant";

        /// <summary>
        /// The transaction info table transaction time column name.
        /// </summary>
        private const string TransactionTimeColumn = "transaction_time";

        /// <summary>
        /// The transaction audit enabled.
        /// </summary>
        private const string TransactionAuditEnabled = "audit_enabled";

        /// <summary>
        /// The value indicating whether to use cache tables for retrieving the data.
        /// </summary>
        private bool isCachedDtoReadEnabled;

        /// <summary>
        /// The value indicating whether the full access is granted to the current person.
        /// </summary>
        private bool isFullAccessGranted;

        /// <summary>
        /// Backing field for the cached rawSessionInstant value
        /// </summary>
        private object rawSessionInstant;

        /// <summary>
        /// Gets or sets the iteration setup dao.
        /// </summary>
        public IIterationSetupDao IterationSetupDao { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAppConfigService"/>
        /// </summary>
        public IAppConfigService AppConfigService { get; set; }

        /// <summary>
        /// Gets or sets the iteration setup.
        /// </summary>
        public IterationSetup IterationSetup { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Cdp4TransactionManager"/>
        /// </summary>
        public Cdp4TransactionManager()
        {
            this.rawSessionInstant = null;
        }

        /// <summary>
        /// The setup transaction.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="iterationIid">
        /// The iteration context to use.
        /// </param>
        /// <returns>
        /// The <see cref="NpgsqlTransaction"/>.
        /// </returns>
        public NpgsqlTransaction SetupTransaction(ref NpgsqlConnection connection, Credentials credentials, Guid iterationIid)
        {
            var transaction = this.GetTransaction(ref connection, credentials);

            if (iterationIid != Guid.Empty)
            {
                this.IterationSetup = this.GetIterationContext(transaction, iterationIid);
            }

            return transaction;
        }

        /// <summary>
        /// Setup a new transaction.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="credentials">
        /// The user credentials of the current request.
        /// </param>
        /// <returns>
        /// The <see cref="NpgsqlTransaction"/>.
        /// </returns>
        public NpgsqlTransaction SetupTransaction(ref NpgsqlConnection connection, Credentials credentials)
        {
            return this.GetTransaction(ref connection, credentials);
        }

        /// <summary>
        /// Get the current session time instant value from the database. In case the most current data is to be
        /// retrieved this value returns (+infinity) which translates to <see cref="DateTime.MaxValue"/>. In case
        /// a request is made related to time-travel the <see cref="DateTime"/> corresponding to the period_end
        /// is returned
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <returns>
        /// A <see cref="DateTime"/> that represents the session Instant (either <see cref="DateTime.MaxValue"/> or
        /// the <see cref="DateTime"/> corresponding to the period_end
        /// </returns>
        public DateTime GetSessionInstant(NpgsqlTransaction transaction)
        {
            return (DateTime)this.GetRawSessionInstant(transaction);
        }

        /// <summary>
        /// Get the raw current session time instant value from the database. In case the most current data is to be
        /// retrieved this value returns (+infinity) which translates to <see cref="DateTime.MaxValue"/>. In case
        /// a request is made related to time-travel the <see cref="DateTime"/> corresponding to the period_end
        /// is returned
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <returns>
        /// A <see cref="object"/> that represents the session Instant (either <see cref="DateTime.MaxValue"/> or
        /// the <see cref="DateTime"/> corresponding to the period_end
        /// </returns>
        public object GetRawSessionInstant(NpgsqlTransaction transaction)
        {
            if (this.rawSessionInstant == null)
            {
                using var command = new NpgsqlCommand(
                    "SELECT * FROM \"SiteDirectory\".\"get_session_instant\"();",
                    transaction.Connection,
                    transaction);

                this.rawSessionInstant = command.ExecuteScalar();
            }

            return this.rawSessionInstant;
        }

        /// <summary>
        /// Sets a value indicating whether to use cached Dto in form of Jsonb.
        /// </summary>
        /// <param name="value">
        /// The value indicating whether to use cached Dto in form of Jsonb.
        /// </param>
        public void SetCachedDtoReadEnabled(bool value)
        {
            this.isCachedDtoReadEnabled = value;
        }

        /// <summary>
        /// Sets a value indicating whether the current person has full access or not.
        /// </summary>
        /// <param name="value">
        /// The value indicating whether the current person has full access or not.
        /// </param>
        public void SetFullAccessState(bool value)
        {
            this.isFullAccessGranted = value;
        }

        /// <summary>
        /// Check session's instant time whether it is the latest state of things.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsCachedDtoReadEnabled(NpgsqlTransaction transaction)
        {
            if (!this.isCachedDtoReadEnabled)
            {
                return false;
            }

            var dateTime = this.GetSessionInstant(transaction);

            return dateTime == DateTime.MaxValue;
        }

        /// <summary>
        /// Indicate whether the full access was granted for the current person.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsFullAccessEnabled()
        {
            return this.isFullAccessGranted;
        }

        /// <summary>
        /// Get the timestamp in the form of a <see cref="DateTime"/> that the <see cref="NpgsqlTransaction"/> was
        /// created using the <see cref="SetupTransaction"/> method.
        /// </summary>
        /// <param name="transaction">
        /// The active <see cref="NpgsqlTransaction"/>
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public DateTime GetTransactionTime(NpgsqlTransaction transaction)
        {
            using var command = new NpgsqlCommand(
                $"SELECT * FROM \"SiteDirectory\".\"{"get_transaction_time"}\"();",
                transaction.Connection,
                transaction);

            return (DateTime)command.ExecuteScalar();
        }

        /// <summary>
        /// Sets the default temporal context (-infinity, +infinity)
        /// </summary>
        /// <param name="transaction">
        /// The active <see cref="NpgsqlTransaction"/>
        /// </param>
        public void SetDefaultContext(NpgsqlTransaction transaction)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.AppendFormat(
                "UPDATE {0} SET ({1}, {2}) = ('-infinity', 'infinity');", TransactionInfoTable, PeriodStartColumn, PeriodEndColumn);

            using var command = new NpgsqlCommand(sqlBuilder.ToString(), transaction.Connection, transaction);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Set the audit logging framework state for the current transaction.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="enabled">
        /// Set the audit logging framework state to off (false), or on (true).
        /// </param>
        public void SetAuditLoggingState(NpgsqlTransaction transaction, bool enabled)
        {
            var sql = $"UPDATE {TransactionInfoTable} SET {TransactionAuditEnabled} = :{TransactionAuditEnabled};";

            using var command = new NpgsqlCommand(sql, transaction.Connection, transaction);

            command.Parameters.Add($"{TransactionAuditEnabled}", NpgsqlDbType.Boolean).Value = enabled;

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Apply the iteration context as set from transaction setup method.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        public void SetIterationContext(NpgsqlTransaction transaction, string partition)
        {
            ApplyIterationContext(transaction, partition, this.IterationSetup);
        }

        /// <summary>
        /// Set the iteration context to a specific iteration.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="iterationId">
        /// The iteration id.
        /// </param>
        public void SetIterationContext(NpgsqlTransaction transaction, string partition, Guid iterationId)
        {
            // use default (non-iteration-tagged) temporal context to retrieve iterationsetup
            this.SetDefaultContext(transaction);
            this.IterationSetup = this.GetIterationContext(transaction, iterationId);
            this.SetIterationContext(transaction, partition);
        }

        /// <summary>
        /// The get transaction.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <returns>
        /// The <see cref="NpgsqlTransaction"/>.
        /// </returns>
        private NpgsqlTransaction GetTransaction(ref NpgsqlConnection connection, Credentials credentials)
        {
            this.rawSessionInstant = null;

            var transaction = this.SetupNewTransaction(ref connection);
            CreateTransactionInfoTable(transaction);
            CreateDefaultTransactionInfoEntry(transaction, credentials);

            return transaction;
        }

        /// <summary>
        /// Apply the iteration context to the transaction.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="iterationSetup">
        /// The iteration Setup.
        /// </param>
        private static void ApplyIterationContext(NpgsqlTransaction transaction, string partition, IterationSetup iterationSetup)
        {
            if (iterationSetup == null)
            {
                // skip function as the iteration was not set
                return;
            }
            
            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("UPDATE {0} SET ({1}, {2}) =", TransactionInfoTable, PeriodStartColumn, PeriodEndColumn);
            sqlBuilder.Append(" (SELECT \"ValidFrom\", \"ValidTo\"");
            sqlBuilder.Append($"  FROM (SELECT iteration_log.\"IterationIid\", revision_from.\"Revision\" AS \"FromRevision\", revision_from.\"Instant\" AS \"ValidFrom\", revision_to.\"Revision\" AS \"ToRevision\",");
            sqlBuilder.Append(" CASE WHEN iteration_log.\"ToRevision\" IS NULL THEN 'infinity' ELSE revision_to.\"Instant\" END AS \"ValidTo\" ");
            sqlBuilder.AppendFormat("FROM \"{0}\".\"IterationRevisionLog\" iteration_log LEFT JOIN \"{0}\".\"RevisionRegistry\" revision_from ON iteration_log.\"FromRevision\" = revision_from.\"Revision\" LEFT JOIN \"{0}\".\"RevisionRegistry\" revision_to ON iteration_log.\"ToRevision\" = revision_to.\"Revision\") IterationLogRevision", partition);
            sqlBuilder.Append("  WHERE \"IterationIid\" = :iterationIid);");

            using var command = new NpgsqlCommand(sqlBuilder.ToString(), transaction.Connection, transaction);

            command.Parameters.Add("iterationIid", NpgsqlDbType.Uuid).Value = iterationSetup.IterationIid;

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// The setup iteration context.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="iterationIid">
        /// The iteration id.
        /// </param>
        /// <returns>
        /// The <see cref="IterationSetup"/>.
        /// </returns>
        private IterationSetup GetIterationContext(NpgsqlTransaction transaction, Guid iterationIid)
        {
            return this.IterationSetupDao.ReadByIteration(transaction, "SiteDirectory", iterationIid, (DateTime)this.GetRawSessionInstant(transaction)).SingleOrDefault();
        }

        /// <summary>
        /// The setup new transaction.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="NpgsqlTransaction"/>.
        /// </returns>
        private NpgsqlTransaction SetupNewTransaction(ref NpgsqlConnection connection)
        {
            // setup connection if not supplied
            if (connection == null)
            {
                connection = new NpgsqlConnection(ServiceUtils.GetConnectionString(this.AppConfigService.AppConfig.Backtier, this.AppConfigService.AppConfig.Backtier.Database));
            }
            
            // ensure an open connection
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            // start transaction with rollback support
            var transaction = connection.BeginTransaction();

            return transaction;
        }

        /// <summary>
        /// Create a transaction info table with transaction scope lifetime.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        private static void CreateTransactionInfoTable(NpgsqlTransaction transaction)
        {
            // setup transaction_info table that is valid only for this transaction
            var sqlBuilder = new StringBuilder();

            sqlBuilder.AppendFormat("CREATE TEMPORARY TABLE {0} (", TransactionInfoTable);
            sqlBuilder.AppendFormat("{0} uuid, ", UserIdColumn);
            sqlBuilder.AppendFormat("{0} timestamp NOT NULL DEFAULT '-infinity', ", PeriodStartColumn);
            sqlBuilder.AppendFormat("{0} timestamp NOT NULL DEFAULT 'infinity', ", PeriodEndColumn);
            sqlBuilder.AppendFormat("{0} timestamp NOT NULL DEFAULT 'infinity', ", InstantColumn);
            sqlBuilder.AppendFormat("{0} timestamp NOT NULL DEFAULT statement_timestamp(), ", TransactionTimeColumn);
            sqlBuilder.AppendFormat("{0} boolean NOT NULL DEFAULT 'true'", TransactionAuditEnabled);
            sqlBuilder.Append(") ON COMMIT DROP;");

            using var command = new NpgsqlCommand(sqlBuilder.ToString(), transaction.Connection, transaction);

            command.ExecuteNonQuery();
        }

        /// <summary>
        /// The create default transaction info entry.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        private static void CreateDefaultTransactionInfoEntry(NpgsqlTransaction transaction, Credentials credentials)
        {
            // insert actor from the request credentials otherwise use default (null) user
            var sqlBuilder = new StringBuilder();
            var isCredentialSet = credentials != null;

            sqlBuilder.AppendFormat("INSERT INTO {0} ({1}, {2})", TransactionInfoTable, UserIdColumn, TransactionTimeColumn);
            sqlBuilder.AppendFormat(" VALUES({0}, statement_timestamp());", isCredentialSet ? $":{UserIdColumn}" : "null");

            using var command = new NpgsqlCommand(sqlBuilder.ToString(), transaction.Connection, transaction);

            if (isCredentialSet)
            {
                command.Parameters.Add(UserIdColumn, NpgsqlDbType.Uuid).Value = credentials.Person.Iid;
            }

            command.ExecuteNonQuery();
        }
    }
}
