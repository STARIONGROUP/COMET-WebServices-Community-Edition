// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICdp4TransactionManager.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Helpers
{
    using System;

    using CDP4Orm.Dao;

    using CDP4WebServices.API.Services.Authentication;

    using Npgsql;

    /// <summary>
    /// The TransactionManager interface.
    /// </summary>
    public interface ICdp4TransactionManager
    {
        /// <summary>
        /// Gets or sets the Command logger.
        /// </summary>
        ICommandLogger CommandLogger { get; set; }

        /// <summary>
        /// Setup a new transaction instance. 
        /// </summary>
        /// <param name="connection">
        /// The connection to the database
        /// </param>
        /// <param name="credentials">
        /// The user credential information for this request
        /// </param>
        /// <returns>
        /// The <see cref="NpgsqlTransaction"/>.
        /// </returns>
        NpgsqlTransaction SetupTransaction(ref NpgsqlConnection connection, Credentials credentials);

        /// <summary>
        /// Setup a new transaction instance bound to an iteration context. 
        /// </summary>
        /// <param name="connection">
        /// The connection to the database
        /// </param>
        /// <param name="credentials">
        /// The user credential information for this request
        /// </param>
        /// <param name="iterationIid">
        /// An optional iteration id that can be supplied to set the transaction context to the iteration
        /// </param>
        /// <returns>
        /// The <see cref="NpgsqlTransaction"/>.
        /// </returns>
        NpgsqlTransaction SetupTransaction(ref NpgsqlConnection connection, Credentials credentials, Guid iterationIid);

        /// <summary>
        /// Set the transaction statement time used when creating items.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <returns>
        /// The newly set transaction time.
        /// </returns>
        DateTime UpdateTransactionStatementTime(NpgsqlTransaction transaction);

        /// <summary>
        /// Get the current transaction time.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        DateTime GetTransactionTime(NpgsqlTransaction transaction);
        
        /// <summary>
        /// Get the session timeframe start time.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        DateTime GetSessionTimeFrameStart(NpgsqlTransaction transaction);

        /// <summary>
        /// Get the current session time instant.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        DateTime GetSessionInstant(NpgsqlTransaction transaction);

        /// <summary>
        /// Indicate whether the full access was granted for the current person.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsFullAccessEnabled();

        /// <summary>
        /// Check session's instant time whether it is the latest state of things.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsCachedDtoReadEnabled(NpgsqlTransaction transaction);

        /// <summary>
        /// Set the audit logging framework state for the current transaction.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="enabled">
        /// Set the audit logging framework state to off (false), or on (true).
        /// </param>
        void SetAuditLoggingState(NpgsqlTransaction transaction, bool enabled);

        /// <summary>
        /// The set default context.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        void SetDefaultContext(NpgsqlTransaction transaction);

        /// <summary>
        /// Apply the iteration context as set from transaction setup method.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        void SetIterationContext(NpgsqlTransaction transaction, string partition);

        /// <summary>
        /// Set the iteration context to a specific iteration.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="iterationId">
        /// The iteration id.
        /// </param>
        void SetIterationContext(NpgsqlTransaction transaction, string partition, Guid iterationId);

        /// <summary>
        /// Sets a value indicating whether to use a cached Dto in form of Jsonb.
        /// </summary>
        /// <param name="value">
        /// The value indicating whether to use a cached Dto in form of Jsonb.
        /// </param>
        void SetCachedDtoReadEnabled(bool value);

        /// <summary>
        /// Sets a value indicating whether the current person has full access or not.
        /// </summary>
        /// <param name="value">
        /// The value indicating whether the current person has full access or not.
        /// </param>
        void SetFullAccessState(bool value);
    }
}