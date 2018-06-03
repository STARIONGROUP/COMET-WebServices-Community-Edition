// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RevisionService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CDP4Common.DTO;
    using CDP4Orm.Dao.Revision;

    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// A service that allows revision based retrieval of concepts from the data store.
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
        /// <returns>
        /// List of instances of <see cref="Thing"/>.
        /// </returns>
        public IEnumerable<Thing> Get(NpgsqlTransaction transaction, string partition, int revision)
        {
            return this.InternalGet(transaction, partition, revision).Select(x => x.Thing);
        }

        /// <summary>
        /// Gets the revisions of the <see cref="Thing"/> with the given <paramref name="Guid"/>
        /// </summary>
        /// <param name="transaction">The current transaction to the database.</param>
        /// <param name="partition">The database partition (schema) where the requested resource is stored.</param>
        /// <param name="identifier">The identifier of the <see cref="Thing"/> to query</param>
        /// <param name="revisionFrom">The oldest revision to retrieve</param>
        /// <param name="revisionTo">The latest revision to retrieve</param>
        /// <returns>A collection of revised <see cref="Thing"/></returns>
        public IEnumerable<Thing> Get(NpgsqlTransaction transaction, string partition, Guid identifier, int revisionFrom, int revisionTo)
        {
            // Set the transaction to retrieve the latest database state
            this.TransactionManager.SetDefaultContext(transaction);
            return this.RevisionDao.ReadRevision(transaction, partition, identifier, revisionFrom, revisionTo);
        }

        /// <summary>
        /// Save The revision of a <see cref="Thing"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The database partition (schema) where the requested resource is stored.</param>
        /// <param name="actor">The identifier of the person who made this revision</param>
        /// <param name="revision">The base revision number from which the query is performed</param>
        /// <returns>A collection of saved <see cref="Thing"/></returns>
        public IEnumerable<Thing> SaveRevisions(NpgsqlTransaction transaction, string partition, Guid actor, int revision)
        {
            var thingRevisionInfos = this.InternalGet(transaction, partition, revision).ToArray();
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
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The partition</param>
        /// <param name="iteration">The iteration associated to the revision</param>
        /// <param name="fromRevision">The starting revision number for the iteration. If null the current revision is used.</param>
        /// <param name="toRevision">The ending revision number for the iteration. If null it means the iteration is the current one.</param>
        public void InsertIterationRevisionLog(NpgsqlTransaction transaction, string partition, Guid iteration, int? fromRevision, int? toRevision)
        {
            this.RevisionDao.InsertIterationRevisionLog(transaction, partition, iteration, fromRevision, toRevision);
        }

        /// <summary>
        /// Insert new data in the RevisionRegistry table
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The partition</param>
        public void InsertInitialRevision(NpgsqlTransaction transaction, string partition)
        {
            this.RevisionDao.InsertInitialRevision(transaction, partition);
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
        /// <returns>
        /// A collection of <see cref="Thing"/> instances.
        /// </returns>
        private IEnumerable<ThingRevisionInfo> InternalGet(NpgsqlTransaction transaction, string partition, int revision)
        {
            // Set the transaction to retrieve the latest database state
            this.TransactionManager.SetDefaultContext(transaction);

            var revisionInfoPerPartition =
                this.RevisionDao.Read(transaction, partition, revision).GroupBy(x => x.Partition);

            // Use the revision model data (grouped by partition)
            foreach (var partitionInfo in revisionInfoPerPartition)
            {
                // Use the revision model data (grouped by type) to retrieve all Things that are newer then the supplied revision number
                foreach (var revisionTypeInfo in partitionInfo.GroupBy(x => x.TypeInfo))
                {
                    var resolvedPartition = partitionInfo.Key;
                    var instanceType = revisionTypeInfo.Key;
                    var revisionedInstanceIids = revisionTypeInfo.Select(x => x.Iid);
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
