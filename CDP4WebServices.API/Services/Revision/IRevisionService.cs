// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRevisionService.cs" company="RHEA System S.A.">
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

namespace CometServer.Services
{
    using System;
    using System.Collections.Generic;

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
        /// List of instances of <see cref="Thing"/>.
        /// </returns>
        IEnumerable<Thing> Get(NpgsqlTransaction transaction, string partition, int revision, bool useDefaultContext);

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
        /// A collection of revised <see cref="Thing"/>
        /// </returns>
        IEnumerable<Thing> Get(NpgsqlTransaction transaction, string partition, Guid identifier, int revisionFrom, int revisionTo);

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
        /// A collection of saved <see cref="Thing"/>
        /// </returns>
        IEnumerable<Thing> SaveRevisions(NpgsqlTransaction transaction, string partition, Guid actor, int revision);

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
        void InsertIterationRevisionLog(NpgsqlTransaction transaction, string partition, Guid iteration, int? fromRevision, int? toRevision);

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
        /// The current or next available revision number
        /// </returns>
        int GetRevisionForTransaction(NpgsqlTransaction transaction, string partition);

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
        /// List of instances of <see cref="Thing"/>.
        /// </returns>
        IEnumerable<Thing> GetCurrentChanges(NpgsqlTransaction transaction, string partition, int revision, bool useDefaultContext);
    }
}
