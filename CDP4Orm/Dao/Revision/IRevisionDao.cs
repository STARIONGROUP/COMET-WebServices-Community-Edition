// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRevisionDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao.Revision
{
    using System;
    using System.Collections.Generic;
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
        IEnumerable<RevisionInfo> Read(NpgsqlTransaction transaction, string partition, int revision);

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
        IEnumerable<RevisionInfo> ReadCurrentRevisionChanges(NpgsqlTransaction transaction, string partition, int revision);

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
        IEnumerable<RevisionRegistryInfo> ReadRevisionRegistry(NpgsqlTransaction transaction, string partition);

        /// <summary>
        /// Read the revisions of a <see cref="Thing"/>
        /// </summary>
        /// <param name="transaction">The current transaction to the database.</param>
        /// <param name="partition">The database partition (schema) where the requested resource is stored.</param>
        /// <param name="identifier">The identifier of the <see cref="Thing"/></param>
        /// <param name="revisionFrom">The oldest revision to query</param>
        /// <param name="revisionTo">The latest revision to query</param>
        /// <returns>The collection of revised <see cref="Thing"/></returns>
        IEnumerable<Thing> ReadRevision(NpgsqlTransaction transaction, string partition, Guid identifier, int revisionFrom, int revisionTo);

        /// <summary>
        /// Save The revision of a <see cref="Thing"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The database partition (schema) where the requested resource is stored.</param>
        /// <param name="thing">The revised <see cref="Thing"/></param>
        /// <param name="actor">The identifier of the person who made this revision</param>
        void WriteRevision(NpgsqlTransaction transaction, string partition, Thing thing, Guid actor);

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
        int GetRevisionForTransaction(NpgsqlTransaction transaction, string partition);

        /// <summary>
        /// Insert new values into the IterationRevisionLog table
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The partition</param>
        /// <param name="iteration">The iteration associated to the revision</param>
        /// <param name="fromRevision">The starting revision number for the iteration. If null the current revision is used.</param>
        /// <param name="toRevision">The ending revision number for the iteration. If null it means the iteration is the current one.</param>
        void InsertIterationRevisionLog(NpgsqlTransaction transaction, string partition, Guid iteration, int? fromRevision, int? toRevision);
    }
}