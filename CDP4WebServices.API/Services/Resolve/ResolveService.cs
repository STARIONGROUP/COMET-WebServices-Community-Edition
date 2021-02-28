// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolveService.cs" company="RHEA System S.A.">
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

    using CDP4Orm.Dao;
    using CDP4Orm.Dao.Resolve;

    using CometServer.Helpers;
    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// A service that resolves the supplied <see cref="CDP4Common.DTO.Thing"/> information from the data store.
    /// </summary>
    public class ResolveService : IResolveService
    {
        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Gets or sets the resolve dao.
        /// </summary>
        public IResolveDao ResolveDao { get; set; }
        
        /// <summary>
        /// Gets or sets the container dao.
        /// </summary>
        public IContainerDao ContainerDao { get; set; }

        /// <summary>
        /// Gets or sets the transaction manager.
        /// </summary>
        public ICdp4TransactionManager TransactionManager { get; set; }

        /// <summary>
        /// Gets or sets the DataModel Utils helper.
        /// </summary>
        public IDataModelUtils DataModelUtils { get; set; }

        /// <summary>
        /// Gets or sets  the transaction.
        /// </summary>
        public IRequestUtils RequestUtils { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IMetaInfoProvider"/>
        /// </summary>
        public IMetaInfoProvider MetaInfoProvider { get; set; }

        /// <summary>
        /// Resolve the information in the supplied collection of <see cref="DtoResolveHelper"/>.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="resolvableInfo">
        /// The resolvable info placeholders.
        /// </param>
        public void ResolveItems(NpgsqlTransaction transaction, string partition, Dictionary<DtoInfo, DtoResolveHelper> resolvableInfo)
        {
            // 1. try to statically resolve partition info (without access to datastore) 
            this.ResolvePartitionStatically(partition, resolvableInfo.Values);

            // 1.1 resolve partition info and retrieve container info for types
            this.ResolvePartitionFromDataStore(transaction, partition, resolvableInfo.Values);

            // 2. collect missing container info from the datastore
            var resolvableContainer = new ContainerInfo();
            var partionedResolveInfoGroup = resolvableInfo.Values.Where(x => !this.MetaInfoProvider.GetMetaInfo(x.InstanceInfo.TypeName).IsTopContainer && (x.ContainerInfo == null || x.ContainerInfo.Equals(resolvableContainer)))
                                            .GroupBy(x => string.Format("{0}|{1}", x.Partition, x.InstanceInfo.TypeName));

            this.ResolveContainersFromDataStore(transaction, resolvableInfo, partionedResolveInfoGroup);

            // 2.1 resolve any remaining unresolved partition info (e.g. for newly created items) by using the partition from the instance's nearest (partition-resolved) container
            this.ResolvePartitionFromContainmentTree(partition, resolvableInfo.Values.Where(x => !x.IsPartitionResolved), resolvableInfo);

            // At this point the partition info should be fully resolved, break if not
            if (resolvableInfo.Values.Any(x => !x.IsPartitionResolved))
            {
                throw new Exception("Not all partition info could be resolved!");
            }

            // 3. resolve instance info for any remaining un-resolved items
            // group the items to limit database calls
            var unresolvedPerPartition = resolvableInfo.Values.Where(x => !x.IsInstanceResolved).GroupBy(x => x.Partition);
            this.ResolveInstances(transaction, unresolvedPerPartition);

            // All items should be resolved, break if not
            if (resolvableInfo.Values.Any(x => !x.IsResolved))
            {
                throw new Exception("Not all items could be resolved!");
            }
        }

        /// <summary>
        /// Resolve missing containers from data store.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="iid">
        /// The guid of the item to resolve type name for.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> type name of the supplied item.
        /// </returns> 
        public string ResolveTypeNameByGuid(
            NpgsqlTransaction transaction,
            string partition,
            Guid iid)
        {
            var resolveInfo = this.ResolveDao.Read(
                transaction,
                partition,
                new List<Guid>() { iid }).ToList();

            if (resolveInfo.Count == 0)
            {
                throw new Exception("TypeName could not be resolved for iid " + iid + ".");
            }

            return resolveInfo[0].InstanceInfo.TypeName;
        }

        /// <summary>
        /// Use the resolved meta data to resolve the actual <see cref="CDP4Common.DTO.Thing"/> instances resolve instances.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>>
        /// <param name="unresolvedPerPartition">
        /// The unresolved per partition.
        /// </param>
        /// <exception cref="Exception">
        /// Exception if unresolved
        /// </exception>
        internal void ResolveInstances(NpgsqlTransaction transaction, IEnumerable<IGrouping<string, DtoResolveHelper>> unresolvedPerPartition)
        {
            foreach (var partitionedInfo in unresolvedPerPartition)
            {
                foreach (var unresolvedTypeInfo in partitionedInfo.GroupBy(x => x.InstanceInfo.TypeName))
                {
                    // use the resolved partition
                    var resolvedPartition = partitionedInfo.Key;
                    var instanceType = unresolvedTypeInfo.Key;
                    var resolveIds = unresolvedTypeInfo.Select(x => x.InstanceInfo.Iid).Distinct();
                    var service = this.ServiceProvider.MapToReadService(instanceType);

                    // resolve the items from the datastore
                    var resolvedItems =
                        service.GetShallow(
                            transaction,
                            resolvedPartition,
                            resolveIds,
                            new RequestSecurityContext { ContainerReadAllowed = true }).ToList();

                    foreach (var unresolved in unresolvedTypeInfo)
                    {
                        var resolvedItem = resolvedItems.SingleOrDefault(x => x.Iid == unresolved.InstanceInfo.Iid);
                        if (resolvedItem == null)
                        {
                            throw new Exception("Could not resolve item!");
                        }

                        // set resolved item
                        unresolved.Thing = resolvedItem;
                    }
                }
            }
        }

        /// <summary>
        /// Resolve partition from containment tree.
        /// </summary>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="unresolvedItems">
        /// The unresolved items.
        /// </param>
        /// <param name="resolvableInfo">
        /// The resolvable info.
        /// </param>
        internal void ResolvePartitionFromContainmentTree(string partition, IEnumerable<DtoResolveHelper> unresolvedItems, Dictionary<DtoInfo, DtoResolveHelper> resolvableInfo)
        {
            foreach (var unresolvedItem in unresolvedItems)
            {
                unresolvedItem.Partition = this.TryRetrievePartitionFromContainmentTree(
                    partition,
                    unresolvedItem.ContainerInfo,
                    resolvableInfo);
            }
        }

        /// <summary>
        /// The try retrieve partition from containment tree.
        /// </summary>
        /// <param name="partition">
        /// The partition.
        /// </param>
        /// <param name="containerInfo">
        /// The container info.
        /// </param>
        /// <param name="resolvableInfo">
        /// The resolvable info.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal string TryRetrievePartitionFromContainmentTree(string partition, ContainerInfo containerInfo, Dictionary<DtoInfo, DtoResolveHelper> resolvableInfo)
        {
            if (containerInfo == null)
            {
                // top container encountered, use the passed in partition
                return partition;
            }

            var container = resolvableInfo[containerInfo];
            
            // try to get the container recursively
            return container.IsPartitionResolved 
                ? container.Partition 
                : this.TryRetrievePartitionFromContainmentTree(partition, container.ContainerInfo, resolvableInfo);
        }

        /// <summary>
        /// Try to statically resolve the partition info for any unresolved Item.
        /// </summary>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="unresolvedItems">
        /// A collection of unresolved items.
        /// </param>
        internal void ResolvePartitionStatically(string partition, IEnumerable<DtoResolveHelper> unresolvedItems)
        {
            foreach (var unresolved in unresolvedItems)
            {
                var resolvedPartition = this.ResolvePartition(partition, unresolved.InstanceInfo.TypeName);
                if (resolvedPartition != null)
                {
                    unresolved.Partition = resolvedPartition;
                }
            }
        }

        /// <summary>
        /// Try to resolve partition info from the data store.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="partition">
        /// The partition.
        /// </param>
        /// <param name="unresolvedItems">
        /// The unresolved items.
        /// </param>
        internal void ResolvePartitionFromDataStore(NpgsqlTransaction transaction, string partition, IEnumerable<DtoResolveHelper> unresolvedItems)
        {
            var unresolved = unresolvedItems.Where(x => !x.IsPartitionResolved).ToList();

            // Use the passed in 'default' partition when it is the SiteDirectoryPartition
            if (partition == CDP4Orm.Dao.Utils.SiteDirectoryPartition)
            {
                unresolved.ForEach(x => x.Partition = partition);
                return;
            }

            // resolve the partition of the instance data (engineeringmodel or iteration bound)
            var resolvedInfo = this.ResolveDao.Read(transaction, partition, unresolved.Select(x => x.InstanceInfo.Iid)).ToList();

            foreach (var unresolvedItem in unresolved)
            {
                var resolvedItem = resolvedInfo.SingleOrDefault(x => x.InstanceInfo.Iid == unresolvedItem.InstanceInfo.Iid);
                if (resolvedItem == null)
                {
                    continue;
                }

                unresolvedItem.Partition = resolvedItem.Partition;
            }
        }

        /// <summary>
        /// The static resolve partition.
        /// </summary>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string ResolvePartition(string partition, string typeName)
        {
            if (partition == CDP4Orm.Dao.Utils.SiteDirectoryPartition)
            {
                return partition;
            }

            var staticTypeInfo = this.DataModelUtils.GetSourcePartition(typeName);

            if (staticTypeInfo == "SiteDirectory")
            {
                return staticTypeInfo;
            }

            if (staticTypeInfo != null)
            {
                // source partition info found
                return partition.StartsWith(staticTypeInfo)
                           ? partition
                           : partition.Replace(CDP4Orm.Dao.Utils.EngineeringModelPartition, staticTypeInfo);
            }

            return null;
        }

        /// <summary>
        /// Resolve missing containers from data store.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="resolvableInfo">
        /// The resolvable info.
        /// </param>
        /// <param name="partionedResolveInfoGroup">
        /// The resolve info grouped per partition/type.
        /// </param>
        private void ResolveContainersFromDataStore(
            NpgsqlTransaction transaction,
            IDictionary<DtoInfo, DtoResolveHelper> resolvableInfo,
            IEnumerable<IGrouping<string, DtoResolveHelper>> partionedResolveInfoGroup)
        {
            foreach (var partionedResolveInfo in partionedResolveInfoGroup)
            {
                var resolvedPartition = partionedResolveInfo.First().Partition;
                var resolvableType = partionedResolveInfo.First().InstanceInfo.TypeName;

                // resolve the container info per type
                var resolvedContainerInfos = this.ContainerDao.Read(
                    transaction,
                    resolvedPartition,
                    resolvableType,
                    partionedResolveInfo.Select(x => x.InstanceInfo.Iid));

                // register retrieved information for later instance resolvement
                foreach (var resolvedContainerInfo in resolvedContainerInfos)
                {
                    var containedItem = resolvableInfo.SingleOrDefault(x => x.Key.Iid == resolvedContainerInfo.Item1);
                    var containerInfo = resolvedContainerInfo.Item2;

                    containedItem.Value.ContainerInfo = containerInfo;

                    if (!resolvableInfo.ContainsKey(containerInfo))
                    {
                        resolvableInfo.Add(
                            containerInfo,
                            new DtoResolveHelper(containerInfo) { Partition = containerInfo.Partition });
                    }
                }
            }
        }
    }
}
