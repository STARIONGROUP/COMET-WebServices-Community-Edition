// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICdp4TransactionManager.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
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
    using System.Threading.Tasks;

    using CDP4Common.DTO;
    
    using CometServer.Authorization;

    using Npgsql;

    /// <summary>
    /// A wrapper interface for the <see cref="NpgsqlTransaction"/> class, allowing temporal database interaction.
    /// </summary>
    public interface ICdp4TransactionManager
    {
        /// <summary>
        /// Gets the iteration setup context.
        /// </summary>
        IterationSetup IterationSetup { get;}

        /// <summary>
        /// Setup a new transaction instance. 
        /// </summary>
        /// <param name="credentials">
        /// The user credential information for this request
        /// </param>
        /// <returns>
        /// The <see cref="NpgsqlTransaction"/>.
        /// </returns>
        Task<NpgsqlTransaction> SetupTransactionAsync(Credentials credentials);

        /// <summary>
        /// Setup a new transaction instance bound to an iteration context. 
        /// </summary>
        /// <param name="credentials">
        /// The user credential information for this request
        /// </param>
        /// <param name="iterationIid">
        /// An optional iteration id that can be supplied to set the transaction context to the iteration
        /// </param>
        /// <returns>
        /// The <see cref="NpgsqlTransaction"/>.
        /// </returns>
        Task<NpgsqlTransaction> SetupTransactionAsync(Credentials credentials, Guid iterationIid);

        /// <summary>
        /// Get the current transaction time.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        Task<DateTime> GetTransactionTimeAsync(NpgsqlTransaction transaction);

        /// <summary>
        /// Get the current session time instant.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        Task<DateTime> GetSessionInstantAsync(NpgsqlTransaction transaction);

        /// <summary>
        /// Get the raw current session time instant value from the database.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <returns>
        /// The current session instant as an <see cref="object"/>.
        /// </returns>
        Task<object> GetRawSessionInstantAsync(NpgsqlTransaction transaction);

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
        Task<bool> IsCachedDtoReadEnabledAsync(NpgsqlTransaction transaction);

        /// <summary>
        /// Set the audit logging framework state for the current transaction.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="enabled">
        /// Set the audit logging framework state to off (false), or on (true).
        /// </param>
        Task SetAuditLoggingStateAsync(NpgsqlTransaction transaction, bool enabled);

        /// <summary>
        /// The set default context.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        Task SetDefaultContextAsync(NpgsqlTransaction transaction);

        /// <summary>
        /// Apply the iteration context as set from transaction setup method.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        Task SetIterationContextAsync(NpgsqlTransaction transaction, string partition);

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
        Task SetIterationContextAsync(NpgsqlTransaction transaction, string partition, Guid iterationId);

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
