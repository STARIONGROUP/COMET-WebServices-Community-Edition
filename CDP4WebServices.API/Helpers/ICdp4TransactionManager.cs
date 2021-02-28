// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICdp4TransactionManager.cs" company="RHEA System S.A.">
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

    using CDP4Common.DTO;
    
    using CDP4Orm.Dao;

    using CometServer.Services.Authentication;

    using Npgsql;

    /// <summary>
    /// The TransactionManager interface.
    /// </summary>
    public interface ICdp4TransactionManager
    {
        /// <summary>
        /// Gets the iteration setup context.
        /// </summary>
        IterationSetup IterationSetup { get;}

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