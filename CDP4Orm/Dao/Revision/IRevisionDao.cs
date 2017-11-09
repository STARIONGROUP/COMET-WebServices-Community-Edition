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
    /// The RevisionDao interface.
    /// </summary>
    public interface IRevisionDao
    {
        /// <summary>
        /// Read the data from the database.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="revision">
        /// revision to retrieve from the database.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="RevisionInfo"/>.
        /// </returns>
        IEnumerable<RevisionInfo> Read(NpgsqlTransaction transaction, string partition, int revision);

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
        /// Insert new data in the RevisionRegistry table
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The partition</param>
        /// <param name="revisionNumber">The new revision to insert</param>
        /// <param name="actor">The person who created the new revision</param>
        void InsertRevisionRegistry(NpgsqlTransaction transaction, string partition, int revisionNumber, Guid actor);

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