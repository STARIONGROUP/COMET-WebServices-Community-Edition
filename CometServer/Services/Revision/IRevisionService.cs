// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRevisionService.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
//
//    Author: Sam Geren�, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Th�ate
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

namespace CometServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// A service that allows revision based retrieval of concepts from the data store.
    /// For data retrieval the revision number of the partition's top container Thing is used
    /// For data manipulations a new revision is created based on the transaction timestamp, 
    /// this revision is reused throughout the active transaction
    /// </summary>
    public interface IRevisionService
    {
        /// <summary>
        /// Get the requested revision data from the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="revision">
        /// The revision number used to retrieve data from the database
        /// </param>
        /// <param name="useDefaultContext">
        /// Indicates whether the default context shall be used. Else use the request context (set at module-level).
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having list of instances of <see cref="Thing"/> as a result.
        /// </returns>
        Task<IEnumerable<Thing>> GetAsync(NpgsqlTransaction transaction, string partition, int revision, bool useDefaultContext);

        /// <summary>
        /// Gets the revisions of the <see cref="Thing"/> with the given <paramref name="{Guid}"/>
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="identifier">
        /// The identifier of the <see cref="Thing"/> to query
        /// </param>
        /// <param name="revisionFrom">
        /// The oldest revision to retrieve
        /// </param>
        /// <param name="revisionTo">
        /// The latest revision to retrieve
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having a collection of revised <see cref="Thing"/> as a result
        /// </returns>
        Task<IEnumerable<Thing>> GetAsync(NpgsqlTransaction transaction, string partition, Guid identifier, int revisionFrom, int revisionTo);

        /// <summary>
        /// Save The revision of a <see cref="Thing"/>
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="actor">
        /// The identifier of the person who made this revision
        /// </param>
        /// <param name="revision">
        /// The base revision number from which the query is performed
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having a collection of saved <see cref="Thing"/> as a result
        /// </returns>
        Task<IEnumerable<Thing>> SaveRevisionsAsync(NpgsqlTransaction transaction, string partition, Guid actor, int revision);

        /// <summary>
        /// Insert new values into the IterationRevisionLog table
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="partition">
        /// The partition
        /// </param>
        /// <param name="iteration">
        /// The iteration associated to the revision
        /// </param>
        /// <param name="fromRevision">
        /// The starting revision number for the iteration. If null the current revision is used.
        /// </param>
        /// <param name="toRevision">
        /// The to Revision.
        /// </param>
        /// <returns>
        /// An awaitable <see ="Task"/> that completes when the operation is done
        /// </returns>
        Task InsertIterationRevisionLogAsync(NpgsqlTransaction transaction, string partition, Guid iteration, int? fromRevision, int? toRevision);

        /// <summary>
        /// Gets a unique revision number for this transaction by reading it from the RevisionRegistry table, or adding it there if it does not exist yet
        /// This ensures that there is only 
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="partition">
        /// The partition
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having the current or next available revision number as a result
        /// </returns>
        Task<int> GetRevisionForTransactionAsync(NpgsqlTransaction transaction, string partition);

        /// <summary>
        /// Get the requested revision data from the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="revision">
        /// The revision number used to retrieve data from the database
        /// </param>
        /// <param name="useDefaultContext">
        /// Indicates whether the default context shall be used. Else use the request context (set at module-level).
        /// should only be false for engineering-model data
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/> having a list of instances of <see cref="Thing"/> as a result.
        /// </returns>
        Task<IEnumerable<Thing>> GetCurrentChangesAsync(NpgsqlTransaction transaction, string partition, int revision, bool useDefaultContext);
    }
}
