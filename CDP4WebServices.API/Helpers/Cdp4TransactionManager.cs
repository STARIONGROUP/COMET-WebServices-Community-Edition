// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cdp4TransactionManager.cs" company="RHEA System S.A.">
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

namespace CometServer.Helpers
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Text;
    using CDP4Common.DTO;
    using CDP4Orm.Dao;
    using Configuration;
    using Npgsql;
    using NpgsqlTypes;
    using Services.Authentication;
    using ServiceUtils = Services.Utils;

    /// <summary>
    /// A wrapper class for the <see cref="NpgsqlTransaction"/> class, allowing temporal database interaction.
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
        /// Gets or sets the iteration setup dao.
        /// </summary>
        public IIterationSetupDao IterationSetupDao { get; set; }

        /// <summary>
        /// Gets or sets the Command logger.
        /// </summary>
        public ICommandLogger CommandLogger { get; set; }

        /// <summary>
        /// Gets or sets the iteration setup.
        /// </summary>
        public IterationSetup IterationSetup { get; private set; }

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
        /// Get the current session time instant.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public DateTime GetSessionInstant(NpgsqlTransaction transaction)
        {
            if (!this.CommandLogger.ExecuteCommands)
            {
                // if commands are not executed return now time
                return DateTime.UtcNow;
            }

            using (var command = new NpgsqlCommand(
                string.Format("SELECT * FROM \"SiteDirectory\".\"{0}\"();", "get_session_instant"),
                transaction.Connection,
                transaction))
            { 
                return (DateTime)command.ExecuteScalar();
            }
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
        /// Get the session timeframe start time.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public DateTime GetSessionTimeFrameStart(NpgsqlTransaction transaction)
        {
            if (!this.CommandLogger.ExecuteCommands)
            {
                // if commands are not executed return now time
                return DateTime.UtcNow;
            }

            using (var command = new NpgsqlCommand(
                string.Format("SELECT * FROM \"SiteDirectory\".\"{0}\"();", "get_session_timeframe_start"),
                transaction.Connection,
                transaction))
            { 
                return (DateTime)command.ExecuteScalar();
            }
        }

        /// <summary>
        /// Get the current transaction time.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public DateTime GetTransactionTime(NpgsqlTransaction transaction)
        {
            if (!this.CommandLogger.ExecuteCommands)
            {
                // if commands are not executed return now time
                return DateTime.UtcNow;
            }

            using (var command = new NpgsqlCommand(
                        string.Format("SELECT * FROM \"SiteDirectory\".\"{0}\"();", "get_transaction_time"),
                        transaction.Connection,
                        transaction))
            {
                return (DateTime)command.ExecuteScalar();
            }
        }

        /// <summary>
        /// The set default context.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        public void SetDefaultContext(NpgsqlTransaction transaction)
        {
            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat(
                "UPDATE {0} SET ({1}, {2}) = ('-infinity', 'infinity');", TransactionInfoTable, PeriodStartColumn, PeriodEndColumn);
            
            using (var command = new NpgsqlCommand(sqlBuilder.ToString(), transaction.Connection, transaction))
            {
                // log the sql command
                this.CommandLogger.ExecuteAndLog(command);
            }
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

            using (var command = new NpgsqlCommand(sql, transaction.Connection, transaction))
            {
                command.Parameters.Add($"{TransactionAuditEnabled}", NpgsqlDbType.Boolean).Value = enabled;

                // log the sql command
                this.CommandLogger.ExecuteAndLog(command);
            }
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
            this.ApplyIterationContext(transaction, partition, this.IterationSetup);
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
            var transaction = this.SetupNewTransaction(ref connection);
            this.CreateTransactionInfoTable(transaction);
            this.CreateDefaultTransactionInfoEntry(transaction, credentials);

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
        private void ApplyIterationContext(NpgsqlTransaction transaction, string partition, IterationSetup iterationSetup)
        {
            if (iterationSetup == null)
            {
                // skip function as the iteration was not set
                return;
            }
            
            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat(
                "UPDATE {0} SET ({1}, {2}) =", TransactionInfoTable, PeriodStartColumn, PeriodEndColumn).Append(
                " (SELECT \"ValidFrom\", \"ValidTo\"").AppendFormat(
                "  FROM \"{0}\".\"IterationRevisionLog_View\"", partition).Append(
                "  WHERE \"IterationIid\" = :iterationIid);");

            using (var command = new NpgsqlCommand(sqlBuilder.ToString(), transaction.Connection, transaction))
            {
                command.Parameters.Add("iterationIid", NpgsqlDbType.Uuid).Value = iterationSetup.IterationIid;

                // log the sql command
                this.CommandLogger.ExecuteAndLog(command);
            }
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
            return this.IterationSetupDao.ReadByIteration(transaction, "SiteDirectory", iterationIid).SingleOrDefault();
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
                connection = new NpgsqlConnection(ServiceUtils.GetConnectionString(AppConfig.Current.Backtier.Database));
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
        /// The create default transaction info entry.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        private void CreateDefaultTransactionInfoEntry(NpgsqlTransaction transaction, Credentials credentials)
        {
            // insert actor from the request credentials otherwise use default (null) user
            var sqlBuilder = new StringBuilder();
            var isCredentialSet = credentials != null;

            sqlBuilder.AppendFormat("INSERT INTO {0} ({1}, {2})", TransactionInfoTable, UserIdColumn, TransactionTimeColumn);
            sqlBuilder.AppendFormat(" VALUES({0}, statement_timestamp());", isCredentialSet ? string.Format(":{0}", UserIdColumn) : "null");

            using (var command = new NpgsqlCommand(sqlBuilder.ToString(), transaction.Connection, transaction))
            {
                if (isCredentialSet)
                {
                    command.Parameters.Add(UserIdColumn, NpgsqlDbType.Uuid).Value = credentials.Person.Iid;
                }

                // log the sql command
                this.CommandLogger.ExecuteAndLog(command);
            }
        }

        /// <summary>
        /// Create a transaction info table with transaction scope lifetime.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        private void CreateTransactionInfoTable(NpgsqlTransaction transaction)
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

            using (var command = new NpgsqlCommand(sqlBuilder.ToString(), transaction.Connection, transaction))
            {
                // log the sql command
                this.CommandLogger.ExecuteAndLog(command);
            }
        }
    }
}
