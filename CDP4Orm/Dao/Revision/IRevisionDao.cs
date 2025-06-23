// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRevisionDao.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2023 Starion Group S.A.
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

namespace CDP4Orm.Dao.Revision
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// A data access class that allows revision based retrieval of concepts from the data store.
    /// </summary>
    public interface IRevisionDao
    {
        /// <summary>
        /// Retrieves the data that was changed after the indicated revision.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="revision">
        /// The revision number from which to determine the delta response up to the current revision.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="RevisionInfo"/>.
        /// </returns>
        Task<ReadOnlyCollection<RevisionInfo>> Read(NpgsqlTransaction transaction, string partition, int revision);

        /// <summary>
        /// Retrieves the data that was changed in the indicated revision.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="revision">
        /// The revision number from which to return a delta response.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="RevisionInfo"/>.
        /// </returns>
        Task<ReadOnlyCollection<RevisionInfo>> ReadCurrentRevisionChanges(NpgsqlTransaction transaction, string partition, int revision);

        /// <summary>
        /// Retrieves data from the RevisionRegistry table in the specific partition.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="RevisionRegistryInfo"/>.
        /// </returns>
        Task<ReadOnlyCollection<RevisionRegistryInfo>> ReadRevisionRegistry(NpgsqlTransaction transaction, string partition);

        /// <summary>
        /// Read the revisions of a <see cref="Thing"/>
        /// </summary>
        /// <param name="transaction">The current transaction to the database.</param>
        /// <param name="partition">The database partition (schema) where the requested resource is stored.</param>
        /// <param name="identifier">The identifier of the <see cref="Thing"/></param>
        /// <param name="revisionFrom">The oldest revision to query</param>
        /// <param name="revisionTo">The latest revision to query</param>
        /// <returns>The collection of revised <see cref="Thing"/></returns>
        Task<ReadOnlyCollection<Thing>> ReadRevision(NpgsqlTransaction transaction, string partition, Guid identifier, int revisionFrom, int revisionTo);

        /// <summary>
        /// Save The revision of a <see cref="Thing"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The database partition (schema) where the requested resource is stored.</param>
        /// <param name="thing">The revised <see cref="Thing"/></param>
        /// <param name="actor">The identifier of the person who made this revision</param>
        Task WriteRevision(NpgsqlTransaction transaction, string partition, Thing thing, Guid actor);

        /// <summary>
        /// Insert new data in the RevisionRegistry table if it does not exist for this transaction
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
        Task<int> GetRevisionForTransaction(NpgsqlTransaction transaction, string partition);

        /// <summary>
        /// Insert new values into the IterationRevisionLog table
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The partition</param>
        /// <param name="iteration">The iteration associated to the revision</param>
        /// <param name="fromRevision">The starting revision number for the iteration. If null the current revision is used.</param>
        /// <param name="toRevision">The ending revision number for the iteration. If null it means the iteration is the current one.</param>
        Task InsertIterationRevisionLog(NpgsqlTransaction transaction, string partition, Guid iteration, int? fromRevision, int? toRevision);

        /// <summary>
        /// Save revisions for a collection of <see cref="Thing"/>s
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The database partition (schema) where the requested resource is stored.</param>
        /// <param name="things">The revised <see cref="Thing"/></param>
        /// <param name="actor">The identifier of the person who made this revision</param>
        Task BulkWriteRevision(NpgsqlTransaction transaction, string partition, IReadOnlyCollection<Thing> things, Guid actor);
    }
}