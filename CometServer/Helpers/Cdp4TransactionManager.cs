// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Cdp4TransactionManager.cs" company="Starion Group S.A.">
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

namespace CometServer.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.Authorization;

    using Configuration;

    using Microsoft.Extensions.Logging;

    using Npgsql;

    using NpgsqlTypes;

    using ServiceUtils = Services.Utils;

    /// <summary>
    /// A wrapper class for the <see cref="NpgsqlTransaction"/> class, allowing temporal database interaction.
    /// </summary>
    public class Cdp4TransactionManager : ICdp4TransactionManager, IAsyncDisposable
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
        /// The list of connections to the PostgreSQL database.
        /// </summary>
        private readonly List<NpgsqlConnection> connections = [];

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
        /// Gets or sets the <see cref="IAppConfigService"/>
        /// </summary>
        public IAppConfigService AppConfigService { get; set; }

        /// <summary>
        /// Gets the injected <see cref="ILogger{T}"/> of type <see cref="Cdp4TransactionManager"/>
        /// </summary>
        public ILogger<Cdp4TransactionManager> Logger { get; set; }

        /// <summary>
        /// Gets or sets the iteration setup.
        /// </summary>
        public IterationSetup IterationSetup { get; private set; }

        /// <summary>
        /// Checks if an <see cref="NpgsqlTransaction"/> is already disposed
        /// </summary>
        /// <param name="transaction">The <see cref="NpgsqlTransaction"/></param>
        /// <returns>A value indicating if the transaction is disposed, or not</returns>
        public bool IsTransactionDisposed(NpgsqlTransaction transaction)
        {
            try
            {
                // Accessing the Connection property will throw if disposed
                var _ = transaction.Connection;
                return false;
            }
            catch (ObjectDisposedException)
            {
                return true;
            }
        }

        /// <summary>
        /// Asynchronously tries to rollback the transaction, if it is not disposed.
        /// </summary>
        /// <param name="transaction">The <see cref="NpgsqlTransaction"/></param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        public async Task TryRollbackTransaction(NpgsqlTransaction transaction)
        {
            try
            {
                if (transaction != null && !this.IsTransactionDisposed(transaction))
                {
                    await transaction.RollbackAsync();
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning(ex, "Rollback transaction failed.");
            }
        }

        /// <summary>
        /// Asynchronously tries to dispose the transaction, if it is not disposed.
        /// </summary>
        /// <param name="transaction">The <see cref="NpgsqlTransaction"/></param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        public async Task TryDisposeTransaction(NpgsqlTransaction transaction)
        {
            try
            {
                if (transaction != null && !this.IsTransactionDisposed(transaction))
                {
                    await transaction.DisposeAsync();
                }
            }
            catch (ObjectDisposedException)
            {
                // empty catch on purpose, I don't care if it is already disposed, when I want to dispose.
            }
            catch (Exception ex)
            {
                this.Logger.LogWarning(ex, "Dispose transaction failed.");
            }
        }

        /// <summary>
        /// Asynchronously setup a new Transaction
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        /// <param name="iterationIid">
        /// The iteration context to use.
        /// </param>
        /// <returns>
        /// The <see cref="NpgsqlTransaction"/>.
        /// </returns>
        public async Task<NpgsqlTransaction> SetupTransactionAsync(Credentials credentials, Guid iterationIid)
        {
            var transaction = await this.GetTransactionAsync(credentials);

            if (iterationIid != Guid.Empty)
            {
                this.IterationSetup = await this.GetIterationContextAsync(transaction, iterationIid);
            }

            return transaction;
        }

        /// <summary>
        /// Asynchronously setup a new Transaction
        /// </summary>
        /// <param name="credentials">
        /// The user credentials of the current request.
        /// </param>
        /// <returns>
        /// The <see cref="NpgsqlTransaction"/>.
        /// </returns>
        public Task<NpgsqlTransaction> SetupTransactionAsync(Credentials credentials)
        {
            return this.GetTransactionAsync(credentials);
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
        public async Task<DateTime> GetSessionInstantAsync(NpgsqlTransaction transaction)
        {
            return (DateTime) await this.GetRawSessionInstantAsync(transaction);
        }

        /// <summary>
        /// Get the raw current session time instant value from the database.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public async Task<object> GetRawSessionInstantAsync(NpgsqlTransaction transaction)
        {
            await using var command = new NpgsqlCommand(
                "SELECT * FROM \"SiteDirectory\".\"get_session_instant\"();",
                transaction.Connection,
                transaction);

            return await command.ExecuteScalarAsync();
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
        public async Task<bool> IsCachedDtoReadEnabledAsync(NpgsqlTransaction transaction)
        {
            if (!this.isCachedDtoReadEnabled)
            {
                return false;
            }

            var dateTime = await this.GetSessionInstantAsync(transaction);

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
        /// Get the current transaction time.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public async Task<DateTime> GetTransactionTimeAsync(NpgsqlTransaction transaction)
        {
            await using var command = new NpgsqlCommand("SELECT * FROM \"SiteDirectory\".get_transaction_time();",
                transaction.Connection,
                transaction);

            return  (DateTime) await command.ExecuteScalarAsync();
        }

        /// <summary>
        /// The set default context.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        public async Task SetDefaultContextAsync(NpgsqlTransaction transaction)
        {
            var sql = $"UPDATE {TransactionInfoTable} SET ({PeriodStartColumn}, {PeriodEndColumn}) = ('-infinity', 'infinity');";

            await using var command = new NpgsqlCommand(sql, transaction.Connection, transaction);

            await command.ExecuteNonQueryAsync();
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
        public async Task SetAuditLoggingStateAsync(NpgsqlTransaction transaction, bool enabled)
        {
            var sql = $"UPDATE {TransactionInfoTable} SET {TransactionAuditEnabled} = :{TransactionAuditEnabled};";

            await using var command = new NpgsqlCommand(sql, transaction.Connection, transaction);

            command.Parameters.Add($"{TransactionAuditEnabled}", NpgsqlDbType.Boolean).Value = enabled;

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Asyncrhonoulsy Apply the iteration context as set from transaction setup method.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        public Task SetIterationContextAsync(NpgsqlTransaction transaction, string partition)
        {
            return ApplyIterationContextAsync(transaction, partition, this.IterationSetup);
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
        public async Task SetIterationContextAsync(NpgsqlTransaction transaction, string partition, Guid iterationId)
        {
            // use default (non-iteration-tagged) temporal context to retrieve iterationsetup
            await this.SetDefaultContextAsync(transaction);
            this.IterationSetup = await this.GetIterationContextAsync(transaction, iterationId);
            await this.SetIterationContextAsync(transaction, partition);
        }

        /// <summary>
        /// Asyncrhonously gets a new transaction and prepares the PostgreSQL database
        /// with CDP4-COMET transaction data
        /// </summary>
        /// <param name="credentials">
        /// The <see cref="Credentials"/> used to perform authentication and authorization
        /// </param>
        /// <returns>
        /// The new <see cref="NpgsqlTransaction"/>.
        /// </returns>
        private async Task<NpgsqlTransaction> GetTransactionAsync(Credentials credentials)
        {
            var transaction = await this.SetupNewTransactionAsync();
            await CreateTransactionInfoTable(transaction);
            await CreateDefaultTransactionInfoEntry(transaction, credentials);

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
        private static async Task ApplyIterationContextAsync(NpgsqlTransaction transaction, string partition, IterationSetup iterationSetup)
        {
            if (iterationSetup == null)
            {
                // skip function as the iteration was not set
                return;
            }
            
            var sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("UPDATE {0} SET ({1}, {2}) =", TransactionInfoTable, PeriodStartColumn, PeriodEndColumn);
            sqlBuilder.Append(" (SELECT \"ValidFrom\", \"ValidTo\"");
            sqlBuilder.Append(" FROM (SELECT iteration_log.\"IterationIid\", revision_from.\"Revision\" AS \"FromRevision\", revision_from.\"Instant\" AS \"ValidFrom\", revision_to.\"Revision\" AS \"ToRevision\",");
            sqlBuilder.Append(" CASE WHEN iteration_log.\"ToRevision\" IS NULL THEN 'infinity' ELSE revision_to.\"Instant\" END AS \"ValidTo\" ");
            sqlBuilder.AppendFormat("FROM \"{0}\".\"IterationRevisionLog\" iteration_log LEFT JOIN \"{0}\".\"RevisionRegistry\" revision_from ON iteration_log.\"FromRevision\" = revision_from.\"Revision\" LEFT JOIN \"{0}\".\"RevisionRegistry\" revision_to ON iteration_log.\"ToRevision\" = revision_to.\"Revision\") IterationLogRevision", partition);
            sqlBuilder.Append(" WHERE \"IterationIid\" = :iterationIid);");

            await using var command = new NpgsqlCommand(sqlBuilder.ToString(), transaction.Connection, transaction);

            command.Parameters.Add("iterationIid", NpgsqlDbType.Uuid).Value = iterationSetup.IterationIid;

            await command.ExecuteNonQueryAsync();
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
        private async Task<IterationSetup> GetIterationContextAsync(NpgsqlTransaction transaction, Guid iterationIid)
        {
            var iterationSetups  = await this.IterationSetupDao.ReadByIterationAsync(transaction, "SiteDirectory", iterationIid, (DateTime) await this.GetRawSessionInstantAsync(transaction));

            return iterationSetups.SingleOrDefault();
        }

        /// <summary>
        /// The setup new transaction.
        /// </summary>
        /// <returns>
        /// The <see cref="NpgsqlTransaction"/>.
        /// </returns>
        private async Task<NpgsqlTransaction> SetupNewTransactionAsync()
        {
            var connection = new NpgsqlConnection(ServiceUtils.GetConnectionString(this.AppConfigService.AppConfig.Backtier, this.AppConfigService.AppConfig.Backtier.Database));
            
            this.connections.Add(connection);
            await connection.OpenAsync();

            // start transaction with rollback support
            var transaction = await connection.BeginTransactionAsync();

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
        private static async Task CreateDefaultTransactionInfoEntry(NpgsqlTransaction transaction, Credentials credentials)
        {
            // insert actor from the request credentials otherwise use default (null) user
            var sqlBuilder = new StringBuilder();
            var isCredentialSet = credentials != null;

            sqlBuilder.AppendFormat("INSERT INTO {0} ({1}, {2})", TransactionInfoTable, UserIdColumn, TransactionTimeColumn);
            sqlBuilder.AppendFormat(" VALUES({0}, statement_timestamp());", isCredentialSet ? $":{UserIdColumn}" : "null");

            await using var command = new NpgsqlCommand(sqlBuilder.ToString(), transaction.Connection, transaction);

            if (isCredentialSet)
            {
                command.Parameters.Add(UserIdColumn, NpgsqlDbType.Uuid).Value = credentials.Person.Iid;
            }

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Create a transaction info table with transaction scope lifetime.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        private static async Task CreateTransactionInfoTable(NpgsqlTransaction transaction)
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

            await using var command = new NpgsqlCommand(sqlBuilder.ToString(), transaction.Connection, transaction);

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// handles the disposal of the transaction manager and all connections.
        /// </summary>
        /// <returns>An awaitable <see cref="ValueTask"/></returns>
        public async ValueTask DisposeAsync()
        {
            try
            {
                foreach (var connection in this.connections)
                {
                    if (connection != null)
                    {
                        await connection.DisposeAsync();
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                //empty on purpose, already disposed!
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Error was thrown during disposal of the {nameof(Cdp4TransactionManager)}");
            }
        }
    }
}
