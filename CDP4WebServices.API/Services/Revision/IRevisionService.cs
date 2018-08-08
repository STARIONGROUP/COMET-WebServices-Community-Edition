// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRevisionService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
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
        /// <returns>
        /// List of instances of <see cref="Thing"/>.
        /// </returns>
        IEnumerable<Thing> Get(NpgsqlTransaction transaction, string partition, int revision);

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
    }
}