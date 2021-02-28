// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RevisionService.cs" company="RHEA System S.A.">
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
    using System.Linq;

    using CDP4Common.DTO;

    using CDP4Orm.Dao.Revision;

    using CometServer.Helpers;
    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// A service that allows revision based retrieval of concepts from the data store.
    /// For data retrieval the revision number of the partition's top container Thing is used
    /// For data manipulations a new revision is created based on the transaction timestamp, 
    /// this revision is reused throughout the active transaction
    /// </summary>
    public class RevisionService : IRevisionService
    {
        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Gets or sets the cache service.
        /// </summary>
        public ICacheService CacheService { get; set; }

        /// <summary>
        /// Gets or sets the revision dao.
        /// </summary>
        public IRevisionDao RevisionDao { get; set; }

        /// <summary>
        /// Gets or sets the transaction manager.
        /// </summary>
        public ICdp4TransactionManager TransactionManager { get; set; }

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
        public IEnumerable<Thing> Get(NpgsqlTransaction transaction, string partition, int revision, bool useDefaultContext)
        {
            if (partition == Cdp4TransactionManager.SITE_DIRECTORY_PARTITION && !useDefaultContext)
            {
                throw new ArgumentException("the parameter shall be true for Sitedirectory data", nameof(useDefaultContext));
            }

            return this.InternalGet(transaction, partition, revision, true, useDefaultContext).Select(x => x.Thing);
        }

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
        public IEnumerable<Thing> GetCurrentChanges(NpgsqlTransaction transaction, string partition, int revision, bool useDefaultContext)
        {
            if (partition == Cdp4TransactionManager.SITE_DIRECTORY_PARTITION && !useDefaultContext)
            {
                throw new ArgumentException("the parameter shall be true for Sitedirectory data", nameof(useDefaultContext));
            }

            return this.InternalGet(transaction, partition, revision, false, useDefaultContext).Select(x => x.Thing);
        }

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
        public IEnumerable<Thing> Get(NpgsqlTransaction transaction, string partition, Guid identifier, int revisionFrom, int revisionTo)
        {
            // Set the transaction to retrieve the latest database state
            // TODO This will cause issue with older iterations
            this.TransactionManager.SetDefaultContext(transaction);
            return this.RevisionDao.ReadRevision(transaction, partition, identifier, revisionFrom, revisionTo);
        }

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
        public IEnumerable<Thing> SaveRevisions(NpgsqlTransaction transaction, string partition, Guid actor, int revision)
        {
            var thingRevisionInfos = this.InternalGet(transaction, partition, revision, false, true).ToArray();
            foreach (var thingRevInfo in thingRevisionInfos)
            {
                this.RevisionDao.WriteRevision(transaction, thingRevInfo.RevisionInfo.Partition, thingRevInfo.Thing, actor);
                this.CacheService.WriteToCache(transaction, thingRevInfo.RevisionInfo.Partition, thingRevInfo.Thing);
            }

            return thingRevisionInfos.Select(x => x.Thing);
        }

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
        public void InsertIterationRevisionLog(NpgsqlTransaction transaction, string partition, Guid iteration, int? fromRevision, int? toRevision)
        {
            this.RevisionDao.InsertIterationRevisionLog(transaction, partition, iteration, fromRevision, toRevision);
        }

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
        public int GetRevisionForTransaction(NpgsqlTransaction transaction, string partition)
        {
            return this.RevisionDao.GetRevisionForTransaction(transaction, partition);
        }

        /// <summary>
        /// The internal get.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="partition">
        /// The partition.
        /// </param>
        /// <param name="revision">
        /// The revision.
        /// </param>
        /// <param name="deltaResponse">
        /// The delta Response.
        /// </param>
        /// <param name="useDefaultContext">
        /// Indicates whether the default context shall be used. Else use the request context (set at module-level).
        /// </param>
        /// <returns>
        /// A collection of <see cref="Thing"/> instances.
        /// </returns>
        private IEnumerable<ThingRevisionInfo> InternalGet(NpgsqlTransaction transaction, string partition, int revision, bool deltaResponse, bool useDefaultContext)
        {
            if (useDefaultContext)
            {
                // Set the transaction to retrieve the latest database state
                this.TransactionManager.SetDefaultContext(transaction);
            }
            else
            {
                this.TransactionManager.SetIterationContext(transaction, partition);
            }
             
            var revisionInfoPerPartition = deltaResponse
                                               ? this.RevisionDao.Read(transaction, partition, revision).GroupBy(x => x.Partition)
                                               : this.RevisionDao.ReadCurrentRevisionChanges(transaction, partition, revision).GroupBy(x => x.Partition);

            // Use the revision model data (grouped by partition)
            foreach (var partitionInfo in revisionInfoPerPartition)
            {
                // Use the revision model data (grouped by type) to retrieve all Things that are newer then the supplied revision number
                foreach (var revisionTypeInfo in partitionInfo.GroupBy(x => x.TypeInfo))
                {
                    var resolvedPartition = partitionInfo.Key;
                    var instanceType = revisionTypeInfo.Key;

                    // filter out iteration objects not within the current context
                    var revisionedInstanceIids = instanceType == typeof(Iteration).Name && !useDefaultContext
                        ? revisionTypeInfo.Select(x => x.Iid).Where(x => x == this.TransactionManager.IterationSetup.IterationIid)
                        : revisionTypeInfo.Select(x => x.Iid);

                    var service = this.ServiceProvider.MapToReadService(instanceType);
                    
                    // Retrieve typed information filtered by the revisioned iids
                    var versionedThings = service.GetShallow(
                        transaction,
                        resolvedPartition,
                        revisionedInstanceIids,
                        new RequestSecurityContext { ContainerReadAllowed = true });

                    // Aggregate the retrieved versioned data
                    foreach (var delta in versionedThings)
                    {
                        yield return new ThingRevisionInfo(
                            delta,
                            new RevisionInfo
                                {
                                    Iid = delta.Iid,
                                    Partition = resolvedPartition,
                                    TypeInfo = instanceType
                                });
                    }
                }
            }
        }
    }
}
