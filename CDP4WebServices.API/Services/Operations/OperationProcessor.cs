// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationProcessor.cs" company="RHEA System S.A.">
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

namespace CometServer.Services.Operations
{
    using CDP4Common;
    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.Exceptions;
    using CDP4Common.MetaInfo;
    using CDP4Common.Types;

    using CDP4Orm.Dao;
    using CDP4Orm.Dao.Resolve;

    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using NLog;

    using Npgsql;

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security;

    using IServiceProvider = CometServer.Services.IServiceProvider;
    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// The operation processor class that provides logic for CUD logic on the data source.
    /// </summary>
    public class OperationProcessor : IOperationProcessor
    {
        /// <summary>
        /// The JSON formatted class kind property key.
        /// </summary>
        private const string ClasskindKey = "ClassKind";

        /// <summary>
        /// The JSON formatted id property key.
        /// </summary>
        private const string IidKey = "Iid";

        /// <summary>
        /// The JSON formatted revision number property key.
        /// </summary>
        private const string RevisionNumberKey = "RevisionNumber";

        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The base properties of a DTO which can not be updated directly.
        /// </summary>
        private readonly string[] baseProperties = { IidKey, RevisionNumberKey, ClasskindKey };

        /// <summary>
        /// The top container types.
        /// </summary>
        private readonly string[] topContainerTypes = { "SiteDirectory", "EngineeringModel" };

        /// <summary>
        /// Gets the operation <see cref="Thing"/> instance cache.
        /// In this cache you can find <see cref="DtoInfo"/>s, or <see cref="ContainerInfo"/>s and their <see cref="DtoResolveHelper"/>s
        /// from <see cref="Thing"/>s that were resolved during the execution of the <see cref="Process"/> method.
        /// </summary>
        private readonly Dictionary<DtoInfo, DtoResolveHelper> operationThingCache = new Dictionary<DtoInfo, DtoResolveHelper>();

        /// <summary>
        /// Backing field for <see cref="OperationOriginalThingCache"/>
        /// </summary>
        private readonly List<Thing> operationOriginalThingCache = new List<Thing>();

        /// <summary>
        /// Gets the operation original <see cref="Thing"/> instance cache.
        /// </summary>
        /// <remarks>
        /// Do NOT use this cache for things that influence database concurrency,
        /// because that could lead to unexpected results.
        /// </remarks>
        public IReadOnlyList<Thing> OperationOriginalThingCache => this.operationOriginalThingCache;

        /// <summary>
        /// Gets or sets the service registry.
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Gets or sets  the transaction.
        /// </summary>
        public IRequestUtils RequestUtils { get; set; }

        /// <summary>
        /// Gets or sets the revision service.
        /// </summary>
        public IRevisionService RevisionService { get; set; }

        /// <summary>
        /// Gets or sets the resolve service.
        /// </summary>
        public IResolveService ResolveService { get; set; }

        /// <summary>
        /// Gets or sets the operation side effect processor.
        /// </summary>
        public IOperationSideEffectProcessor OperationSideEffectProcessor { get; set; }

        /// <summary>
        /// Gets or sets the file binary service.
        /// </summary>
        public IFileBinaryService FileBinaryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICopySourceService"/>
        /// </summary>
        public ICopySourceService CopySourceService { get; set; }

        /// <summary>
        /// Process the posted operation message.
        /// </summary>
        /// <param name="operation">
        /// The <see cref="CdpPostOperation"/> that is to be processed
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="fileStore">
        /// The optional file binaries that were included in the request.
        /// </param>
        public void Process(
            CdpPostOperation operation,
            NpgsqlTransaction transaction,
            string partition,
            Dictionary<string, Stream> fileStore = null)
        {
            this.ValidatePostMessage(operation, fileStore ?? new Dictionary<string, Stream>(), transaction, partition);
            this.ApplyOperation(operation, transaction, partition, fileStore ?? new Dictionary<string, Stream>());
        }

        /// <summary>
        /// Validate the integrity of the delete operations.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="partition">
        /// The current partition
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If validation failed
        /// </exception>
        internal void ValidateDeleteOperations(CdpPostOperation operation, NpgsqlTransaction transaction, string partition)
        {
            // verify presence of classkind and iid
            if (operation.Delete.Any(x => !x.ContainsKey(ClasskindKey) || !x.ContainsKey(IidKey)))
            {
                throw new InvalidOperationException("Incomplete delete items encountered in operation");
            }

            // verify no scalar properties in operation delete section (exluding Classkind and IidKey
            if (
                operation.Delete.Any(
                    x =>
                        x.Where(kvp => kvp.Key != IidKey && kvp.Key != ClasskindKey)
                            .Any(kvp => this.GetTypeMetaInfo(x[ClasskindKey].ToString()).IsScalar(kvp.Key))))
            {
                throw new InvalidOperationException("Scalar properties are not allowed on delete items");
            }

            // only using the delete pattern with id/classkind
            foreach (var thingInfo in operation.Delete.Where(x => x.Count == 2).Select(x => x.GetInfoPlaceholder()))
            {
                this.operationThingCache.Add(thingInfo, new DtoResolveHelper(thingInfo));
            }

            // for the other delete pattern using containment property of an object
            foreach (var deleteInfo in operation.Delete.Where(x => x.Count > 2))
            {
                // get all deleted properties
                // check if the delete info has any properties set other than Iid, revision and classkind properties
                var operationProperties = deleteInfo.Where(x => !this.baseProperties.Contains(x.Key)).ToList();
                var typeName = deleteInfo[ClasskindKey].ToString();
                var iid = (Guid)deleteInfo[IidKey];
                var metaInfo = this.RequestUtils.MetaInfoProvider.GetMetaInfo(typeName);

                foreach (var kvp in operationProperties)
                {
                    var propertyName = kvp.Key;
                    var propInfo = metaInfo.GetPropertyMetaInfo(propertyName);
                    if (propInfo.Aggregation != AggregationKind.Composite)
                    {
                        // reference delete
                        var deletedDtoInfo = deleteInfo.GetInfoPlaceholder();
                        if (!this.operationThingCache.ContainsKey(deletedDtoInfo))
                        {
                            this.operationThingCache.Add(deletedDtoInfo, new DtoResolveHelper(deletedDtoInfo));
                        }

                        continue;
                    }

                    // object delete via containing property
                    var containerInfo = new ContainerInfo(typeName, iid);
                    if (!this.operationThingCache.ContainsKey(containerInfo))
                    {
                        this.operationThingCache.Add(containerInfo, new DtoResolveHelper(containerInfo));
                    }

                    if (propInfo.PropertyKind == PropertyKind.List)
                    {
                        var deletedCollectionItems = (IEnumerable)kvp.Value;

                        foreach (var deletedValue in deletedCollectionItems)
                        {
                            var deletedValueIid = (Guid)deletedValue;
                            var childTypeName = this.ResolveService.ResolveTypeNameByGuid(transaction, partition, deletedValueIid);
                            var dtoInfo = new DtoInfo(childTypeName, deletedValueIid);

                            if (!this.operationThingCache.ContainsKey(dtoInfo))
                            {
                                this.operationThingCache.Add(dtoInfo, new DtoResolveHelper(dtoInfo));
                            }
                        }
                    }
                    else if (propInfo.PropertyKind == PropertyKind.OrderedList)
                    {
                        var deletedOrderedCollectionItems = (IEnumerable<OrderedItem>)kvp.Value;
                        foreach (var deletedOrderedItem in deletedOrderedCollectionItems)
                        {
                            var deletedValueIid = Guid.Parse(deletedOrderedItem.V.ToString());
                            var childTypeName = this.ResolveService.ResolveTypeNameByGuid(transaction, partition, Guid.Parse(deletedOrderedItem.V.ToString()));
                            var dtoInfo = new DtoInfo(childTypeName, deletedValueIid);

                            if (!this.operationThingCache.ContainsKey(dtoInfo))
                            {
                                this.operationThingCache.Add(dtoInfo, new DtoResolveHelper(dtoInfo));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validate the integrity of the create operations.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="fileStore">
        /// The file Store.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If validation failed
        /// </exception>
        internal void ValidateCreateOperations(CdpPostOperation operation, Dictionary<string, Stream> fileStore)
        {
            // verify all mandatory properties of the thing supplied (throw), 
            // defer property validation as per the operationsideeffect
            operation.Create.ForEach(
                x =>
                    this.RequestUtils.MetaInfoProvider.GetMetaInfo(x)
                        .Validate(
                            x,
                            propertyName => this.OperationSideEffectProcessor.ValidateProperty(x, propertyName)));

            // register items for resolvement
            foreach (var thing in operation.Create)
            {
                var thingInfo = thing.GetInfoPlaceholder();
                if (!this.operationThingCache.ContainsKey(thingInfo))
                {
                    this.operationThingCache.Add(thingInfo, new DtoResolveHelper(thing));
                }
            }

            // verify thing iid is included in (ANY) composite collection property of a container entry in _create or _update section
            foreach (var thing in operation.Create)
            {
                var thingType = thing.GetType().Name;
                if (this.topContainerTypes.Contains(thingType))
                {
                    throw new InvalidOperationException($"Topcontainer item:'{thingType}' creation is not supported");
                }

                if (!this.IsContainerUpdateIncluded(operation, thing))
                {
                    throw new InvalidOperationException($"Container update of item:'{thingType}' with iid:'{thing.Iid}' is missing from the operation");
                }
            }

            var newFileRevisions = operation.Create.OfType<FileRevision>().ToList();

            // validate that each uploaded file has a resepective fileRevision part
            if (fileStore.Keys.Any(hashKey => newFileRevisions.All(x => x.ContentHash != hashKey)))
            {
                throw new InvalidOperationException("All uploaded files must be referenced by their respective (SHA1) content hash in a new 'FileRevision' object.");
            }

            // validate the FileRevision items in operation
            foreach (var fileRevision in newFileRevisions)
            {
                // validate that ContentHash is supplied
                if (string.IsNullOrWhiteSpace(fileRevision.ContentHash))
                {
                    throw new Cdp4ModelValidationException(
                              string.Format(
                                  "The 'ContentHash' property of 'FileRevision' with iid '{0}' is mandatory and cannot be an empty string or null.",
                                  fileRevision.Iid));
                }

                if (!fileStore.ContainsKey(fileRevision.ContentHash))
                {
                    // try if file content is already on disk
                    if (!this.FileBinaryService.IsFilePersisted(fileRevision.ContentHash))
                    {
                        throw new InvalidOperationException(
                                  string.Format(
                                      "FileRevision with iid:'{0}' with content Hash [{1}] does not exist",
                                      fileRevision.Iid,
                                      fileRevision.ContentHash));
                    }
                }
            }
        }

        /// <summary>
        /// Validate the integrity of the update operations.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If validation failed
        /// </exception>
        internal void ValidateCopyOperations(CdpPostOperation operation)
        {
            // verify presence of classkind and iid (throw)
            if (operation.Copy.Any(x => x.Source.Thing.Iid == Guid.Empty))
            {
                throw new InvalidOperationException("Incomplete copy items encountered in operation");
            }

            // verify presence of classkind and iid (throw)
            if (operation.Copy.Any(x => x.Source.TopContainer.Iid == Guid.Empty))
            {
                throw new InvalidOperationException("Incomplete copy items encountered in operation");
            }

            // verify presence of classkind and iid (throw)
            if (operation.Copy.Any(x => x.Target.Container.Iid == Guid.Empty))
            {
                throw new InvalidOperationException("Incomplete copy items encountered in operation");
            }

            // verify presence of classkind and iid (throw)
            if (operation.Copy.Any(x => x.Target.TopContainer.Iid == Guid.Empty))
            {
                throw new InvalidOperationException("Incomplete copy items encountered in operation");
            }

            // verify owner has value if used
            if (operation.Copy.Any(x => x.Options.KeepOwner.HasValue && !x.Options.KeepOwner.Value && x.ActiveOwner == Guid.Empty))
            {
                throw new InvalidOperationException("Incomplete copy items encountered in operation");
            }

            if (operation.Copy.Any(x => x.Source.Thing.ClassKind != ClassKind.ElementDefinition))
            {
                throw new InvalidOperationException("Only Element-Definition may be copied");
            }

            if (operation.Copy.Any(x => x.Source.TopContainer.ClassKind != ClassKind.EngineeringModel || x.Target.TopContainer.ClassKind != ClassKind.EngineeringModel))
            {
                throw new InvalidOperationException("Copy operations may only be applied on Engineering-Model");
            }
        }

        /// <summary>
        /// Validate the integrity of the update operations.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If validation failed
        /// </exception>
        internal void ValidateUpdateOperations(CdpPostOperation operation)
        {
            // verify presence of classkind and iid (throw)
            if (operation.Update.Any(x => !x.ContainsKey(ClasskindKey) || !x.ContainsKey(IidKey)))
            {
                throw new InvalidOperationException("Incomplete update items encountered in operation");
            }

            // verify no updates on FileRevision
            if (operation.Update.Any(x => x[ClasskindKey].ToString() == typeof(FileRevision).Name))
            {
                throw new InvalidOperationException("FileRevisions updates are not allowed");
            }
        }

        /// <summary>
        /// Register the update things for container resolving.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        private void RegisterUpdateContainersForResolvement(CdpPostOperation operation)
        {
            // register items for resolvement
            foreach (var thingInfo in operation.Update.Select(x => x.GetInfoPlaceholder()))
            {
                if (!this.operationThingCache.ContainsKey(thingInfo))
                {
                    var metaInfo = this.RequestUtils.MetaInfoProvider.GetMetaInfo(thingInfo.TypeName);
                    ContainerInfo containerInfo = null;

                    // if not topcontainer set containment resolve info to default, forcing data store resolvement
                    if (!metaInfo.IsTopContainer)
                    {
                        containerInfo = new ContainerInfo();
                    }

                    // register container info for later resolvement
                    var resolveHelper = new DtoResolveHelper(thingInfo) { ContainerInfo = containerInfo };
                    this.operationThingCache.Add(thingInfo, resolveHelper);
                }
            }
        }

        /// <summary>
        /// Convenience method to retrieve Meta Info by type name.
        /// </summary>
        /// <param name="typeName">
        /// The type name.
        /// </param>
        /// <returns>
        /// The <see cref="IMetaInfo"/>.
        /// </returns>
        private IMetaInfo GetTypeMetaInfo(string typeName)
        {
            return this.RequestUtils.MetaInfoProvider.GetMetaInfo(typeName);
        }

        /// <summary>
        /// Validate the received post message integrity.
        /// </summary>
        /// <param name="operation">
        /// The received operation.
        /// </param>
        /// <param name="fileStore">
        /// The file Store.
        /// </param>
        private void ValidatePostMessage(CdpPostOperation operation, Dictionary<string, Stream> fileStore, NpgsqlTransaction transaction, string partition)
        {
            this.ValidateDeleteOperations(operation, transaction, partition);
            this.ValidateCreateOperations(operation, fileStore);
            this.ValidateUpdateOperations(operation);
            this.ValidateCopyOperations(operation);

            this.RegisterUpdateContainersForResolvement(operation);
        }

        /// <summary>
        /// Check if container information is supplied in the operation update or create section.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="thing">
        /// The thing to validate containment for.
        /// </param>
        /// <returns>
        /// True if the thing type is contained.
        /// </returns>
        private bool IsContainerUpdateIncluded(CdpPostOperation operation, Thing thing)
        {
            var thingType = thing.GetType().Name;
            var metaInfo = this.RequestUtils.MetaInfoProvider.GetMetaInfo(thingType);

            return this.TryFindContainerInUpdates(operation, thing, metaInfo)
                   || this.TryFindContainerInCreateSection(operation, thing, metaInfo);
        }

        /// <summary>
        /// Try to find the container in the updates section of the operation message.
        /// </summary>
        /// <param name="operation">
        /// The operation message to search in.
        /// </param>
        /// <param name="thing">
        /// The thing for which to find the container.
        /// </param>
        /// <param name="metaInfo">
        /// The meta meta info of the thing.
        /// </param>
        /// <returns>
        /// True if found, which also registers the container in the local operationContainmentCache
        /// </returns>
        private bool TryFindContainerInUpdates(CdpPostOperation operation, Thing thing, IMetaInfo metaInfo)
        {
            // get the thing info as cachekey
            var thingInfo = thing.GetInfoPlaceholder();
            ContainerInfo containerInfo = null;

            // inspect the operation update section to find containment info for the supplied thing type.
            foreach (var updateInfo in operation.Update)
            {
                // As this is a classlessDto JSON types are used to inspect the object.
                var typeInfo = updateInfo[ClasskindKey].ToString();
                var iidInfo = (Guid)updateInfo[IidKey];

                PropertyMetaInfo containerPropertyInfo;
                if (!metaInfo.TryGetContainerProperty(typeInfo, out containerPropertyInfo))
                {
                    continue;
                }

                var containerPropertyKey = containerPropertyInfo.Name;
                if (!updateInfo.ContainsKey(containerPropertyKey))
                {
                    continue;
                }

                if (containerPropertyInfo.PropertyKind == PropertyKind.List)
                {
                    // property is an unordered list
                    // check if the found container property includes the supplied id.
                    if (((IEnumerable<Guid>)updateInfo[containerPropertyKey]).Any(y => y == thing.Iid))
                    {
                        containerInfo = new ContainerInfo(typeInfo, iidInfo);
                        break;
                    }
                }
                else
                {
                    // property is an ordered list
                    foreach (var orderedItem in (IEnumerable<OrderedItem>)updateInfo[containerPropertyKey])
                    {
                        // check if the found container property includes the supplied id.
                        if (Guid.Parse(orderedItem.V.ToString()) != thing.Iid)
                        {
                            continue;
                        }

                        containerInfo = new ContainerInfo(typeInfo, iidInfo, orderedItem.K);
                        break;
                    }
                }
            }

            // container not found
            if (containerInfo == null)
            {
                return false;
            }

            var containerMetaInfo = this.RequestUtils.MetaInfoProvider.GetMetaInfo(containerInfo.TypeName);

            // container found
            var containerResolvable = new DtoResolveHelper(containerInfo)
                                          {
                                              ContainerInfo = containerMetaInfo.IsTopContainer ? null : new ContainerInfo()
                                          };

            if (this.operationThingCache.ContainsKey(thingInfo))
            {
                // register the container reference
                this.operationThingCache[thingInfo].ContainerInfo = containerInfo;
            }

            // add the container as resolvable
            if (!this.operationThingCache.ContainsKey(containerInfo))
            {
                this.operationThingCache.Add(containerInfo, containerResolvable);
            }

            return true;
        }

        /// <summary>
        /// Try to find the container in the create section of the operation message.
        /// </summary>
        /// <param name="operation">
        /// The operation message to search in.
        /// </param>
        /// <param name="thing">
        /// The thing for which to find the container.
        /// </param>
        /// <param name="metaInfo">
        /// The meta meta info of the thing.
        /// </param>
        /// <returns>
        /// True if found, which also registers the container in the local operationContainmentCache
        /// </returns>
        private bool TryFindContainerInCreateSection(CdpPostOperation operation, Thing thing, IMetaInfo metaInfo)
        {
            // get the thing info as cachekey
            var thingInfo = thing.GetInfoPlaceholder();

            // inspect the operation create section to find containment info for the supplied thing type.
            foreach (var createInfo in operation.Create)
            {
                var typeInfo = createInfo.GetType().Name;
                PropertyMetaInfo containerPropertyInfo;
                ContainerInfo containerInfo;

                if (!metaInfo.TryGetContainerProperty(typeInfo, out containerPropertyInfo))
                {
                    continue;
                }

                if (containerPropertyInfo.PropertyKind == PropertyKind.OrderedList)
                {
                    // check if thing is contained by ordered containment property 
                    var orderedItem =
                        this.RequestUtils.MetaInfoProvider.GetMetaInfo(createInfo)
                            .GetOrderedContainmentIds(createInfo, containerPropertyInfo.Name)
                            .SingleOrDefault(x => Guid.Parse(x.V.ToString()) == thing.Iid);

                    if (orderedItem != null)
                    {
                        // container found
                        containerInfo = new ContainerInfo(
                                            createInfo.ClassKind.ToString(),
                                            createInfo.Iid,
                                            orderedItem.K);

                        if (this.operationThingCache.ContainsKey(thingInfo))
                        {
                            // register the container reference
                            this.operationThingCache[thingInfo].ContainerInfo = containerInfo;
                        }

                        // add the container as resolvable
                        if (!this.operationThingCache.ContainsKey(containerInfo))
                        {
                            this.operationThingCache.Add(containerInfo, new DtoResolveHelper(createInfo));
                        }

                        return true;
                    }

                    continue;
                }

                // Check if the found container property includes the supplied id.
                var containermetaInfo = this.RequestUtils.MetaInfoProvider.GetMetaInfo(typeInfo);
                if (!containermetaInfo.GetContainmentIds(createInfo, containerPropertyInfo.Name).Contains(thing.Iid))
                {
                    continue;
                }

                // container found
                containerInfo = new ContainerInfo(createInfo.ClassKind.ToString(), createInfo.Iid);

                if (this.operationThingCache.ContainsKey(thingInfo))
                {
                    // register the container reference
                    this.operationThingCache[thingInfo].ContainerInfo = containerInfo;
                }

                // add the container as resolvable
                if (!this.operationThingCache.ContainsKey(containerInfo))
                {
                    this.operationThingCache.Add(containerInfo, new DtoResolveHelper(createInfo));
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// A convenience method to get the container information of the passed in <see cref="Thing"/>.
        /// </summary>
        /// <param name="thing">
        /// The thing for which to get the container information.
        /// </param>
        /// <returns>
        /// The <see cref="Thing"/>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// If the container was not found (because of invoked validation logic this should never happen at this point)
        /// </exception>
        private DtoResolveHelper GetContainerInfo(Thing thing)
        {
            DtoResolveHelper resolvedThing;
            if (!this.operationThingCache.TryGetValue(thing.GetInfoPlaceholder(), out resolvedThing))
            {
                throw new InvalidOperationException(
                          string.Format(
                              "The resolved information fo '{0}' with iid: '{1}' was not found.",
                              thing.GetType().Name,
                              thing.Iid));
            }

            if (resolvedThing.ContainerInfo == null)
            {
                throw new InvalidOperationException(
                          string.Format(
                              "The container information for '{0}' with iid: '{1}' was not found.",
                              thing.GetType().Name,
                              thing.Iid));
            }

            DtoResolveHelper resolvedThingContainer;
            if (!this.operationThingCache.TryGetValue(resolvedThing.ContainerInfo, out resolvedThingContainer))
            {
                throw new InvalidOperationException(
                          string.Format(
                              "Expected container for '{0}' with iid: '{1}' not found in operation message.",
                              thing.GetType().Name,
                              thing.Iid));
            }

            return resolvedThingContainer;
        }

        /// <summary>
        /// Apply the operation to the model.
        /// </summary>
        /// <param name="operation">
        /// The validated operation.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The partition.
        /// </param>
        /// <param name="fileStore">
        /// The file Store.
        /// </param>
        private void ApplyOperation(
            CdpPostOperation operation,
            NpgsqlTransaction transaction,
            string partition,
            Dictionary<string, Stream> fileStore)
        {
            // resolve any meta data from the persitence store
            this.ResolveService.ResolveItems(transaction, partition, this.operationThingCache);

            // apply the operations
            this.ApplyDeleteOperations(operation, transaction, partition);
            this.ApplyCreateOperations(operation, transaction);
            this.ApplyUpdateOperations(operation, transaction);
            
            // all operations passed successfully: store the validated uploaded files to disk
            foreach (var kvp in fileStore)
            {
                this.FileBinaryService.StoreBinaryData(kvp.Key, kvp.Value);
            }

            this.ApplyCopyOperations(operation, transaction, partition);
        }

        /// <summary>
        /// Delete the persisted item as indicated by its id.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="persistedThing">
        /// The persisted thing to delete.
        /// </param>
        private void DeletePersistedItem(NpgsqlTransaction transaction, string partition, IPersistService service, Thing persistedThing)
        {
            // get the persisted thing (full) so that   permission can be checked against potential owner
            var securityContext = new RequestSecurityContext { ContainerReadAllowed = true };
            var thing = service.GetShallow(transaction, partition, new[] {persistedThing.Iid}, securityContext).FirstOrDefault();
            if (thing == null)
            {
                return;
            }

            service.DeleteConcept(transaction, partition, thing);
        }

        /// <summary>
        /// Try and get persisted item from the data store.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="service">
        /// The service instance for the requested type.
        /// </param>
        /// <param name="iid">
        /// The id of the item to retrieve.
        /// </param>
        /// <returns>
        /// The retrieved <see cref="Thing"/> instance or null if not found.
        /// </returns>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        private Thing GetPersistedItem(NpgsqlTransaction transaction, string partition, IPersistService service, Guid iid, ISecurityContext securityContext)
        {
            return service.GetShallow(
                    transaction, partition, new[] { iid }, securityContext)
                       .SingleOrDefault();
        }

        /// <summary>
        /// The apply delete operations.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        private void ApplyDeleteOperations(CdpPostOperation operation, NpgsqlTransaction transaction, string requestPartition)
        {
            foreach (var deleteInfo in operation.Delete)
            {
                var typeName = deleteInfo[ClasskindKey].ToString();
                var iid = (Guid)deleteInfo[IidKey];
                var metaInfo = this.RequestUtils.MetaInfoProvider.GetMetaInfo(typeName);
                var service = this.ServiceProvider.MapToPersitableService(typeName);

                // check if the delete info has any properties set other than Iid, revision and classkind properties
                var operationProperties = deleteInfo.Where(x => !this.baseProperties.Contains(x.Key)).ToList();

                if (!operationProperties.Any())
                {
                    var dtoInfo = deleteInfo.GetInfoPlaceholder();
                    this.ExecuteDeleteOperation(dtoInfo, transaction, metaInfo);
                }
                else
                {
                    // do not delete item => process the collection properties
                    foreach (var kvp in operationProperties)
                    {
                        var propertyName = kvp.Key;
                        var propInfo = metaInfo.GetPropertyMetaInfo(propertyName);
                        var resolvedInfo = this.operationThingCache[deleteInfo.GetInfoPlaceholder()];

                        if (propInfo.PropertyKind == PropertyKind.List)
                        {
                            var deletedCollectionItems = (IEnumerable)kvp.Value;

                            // unordered list
                            foreach (var deletedValue in deletedCollectionItems)
                            {
                                if (propInfo.Aggregation == AggregationKind.Composite)
                                {
                                    var deletedValueId = (Guid)deletedValue;

                                    var childTypeName = this.ResolveService.ResolveTypeNameByGuid(transaction, requestPartition, deletedValueId);
                                    var dtoInfo = new DtoInfo(childTypeName, deletedValueId);

                                    this.ExecuteDeleteOperation(dtoInfo, transaction, metaInfo);
                                    continue;
                                }
                            
                                // remove link information
                                if (!service.DeleteFromCollectionProperty(transaction, resolvedInfo.Partition, propertyName, iid, deletedValue))
                                {
                                    Logger.Info(
                                        "The item '{0}' with iid: '{1}' in '{2}.{3}' was already deleted: continue processing.",
                                        propInfo.TypeName,
                                        deletedValue,
                                        typeName,
                                        propInfo.Name);
                                }
                            }
                        }
                        else if (propInfo.PropertyKind == PropertyKind.OrderedList)
                        {
                            var deletedOrderedCollectionItems = (IEnumerable<OrderedItem>)kvp.Value;

                            foreach (var deletedOrderedItem in deletedOrderedCollectionItems)
                            {
                                if (propInfo.Aggregation == AggregationKind.Composite)
                                {
                                    var deletedValueId = Guid.Parse(deletedOrderedItem.V.ToString());
                                    var childTypeName = this.ResolveService.ResolveTypeNameByGuid(transaction, requestPartition, deletedValueId);
                                    var dtoInfo = new DtoInfo(childTypeName, deletedValueId);

                                    this.ExecuteDeleteOperation(dtoInfo, transaction, metaInfo);
                                    continue;
                                }

                                // remove link information
                                if (!service.DeleteFromCollectionProperty(transaction, resolvedInfo.Partition, propertyName, iid, deletedOrderedItem))
                                { 
                                        Logger.Info(
                                            "The ordered item '{0}' with value: '{1}' in '{2}.{3}' was already deleted: continue processing.",
                                            propInfo.TypeName,
                                            deletedOrderedItem.V,
                                            typeName,
                                            propInfo.Name);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The apply create operations.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If the item already exists
        /// </exception>
        private void ApplyCreateOperations(CdpPostOperation operation, NpgsqlTransaction transaction)
        {
            // re-order create
            this.ReorderCreateOrder(operation);
            foreach (var createInfo in operation.Create.Select(x => x.GetInfoPlaceholder()))
            {
                var service = this.ServiceProvider.MapToPersitableService(createInfo.TypeName);
                var securityContext = new RequestSecurityContext { ContainerReadAllowed = true };

                securityContext.Credentials = this.RequestUtils.Context.AuthenticatedCredentials;

                var resolvedInfo = this.operationThingCache[createInfo];

                // check that item doen not exist:
                var persistedItem = this.GetPersistedItem(transaction, resolvedInfo.Partition, service, createInfo.Iid, securityContext);

                if (persistedItem != null)
                {
                    throw new InvalidOperationException(
                        string.Format("Item '{0}' with Iid: '{1}' already exists", createInfo.TypeName, createInfo.Iid));
                }

                // get the (cached) containment information for this create request
                var resolvedContainerInfo = this.GetContainerInfo(resolvedInfo.Thing);

                // keep a copy of the orginal thing to pass to the after create hook
                var originalThing = resolvedInfo.Thing.DeepClone<Thing>();

                if (this.operationOriginalThingCache.All(x => x.Iid != originalThing.Iid))
                {
                    this.operationOriginalThingCache.Add(originalThing);
                }

                // call before create hook
                if (!this.OperationSideEffectProcessor.BeforeCreate(resolvedInfo.Thing, resolvedContainerInfo.Thing, transaction, resolvedInfo.Partition, securityContext))
                {
                    Logger.Warn("Skipping create operation of thing {0} with id {1} as a consequence of the side-effect.", createInfo.TypeName, createInfo.Iid);
                    continue;
                }

                if (resolvedInfo.ContainerInfo.ContainmentSequence != -1)
                {
                    service.CreateConcept(transaction, resolvedInfo.Partition, resolvedInfo.Thing, resolvedContainerInfo.Thing, resolvedInfo.ContainerInfo.ContainmentSequence);
                }
                else
                {
                    service.CreateConcept(transaction, resolvedInfo.Partition, resolvedInfo.Thing, resolvedContainerInfo.Thing);
                }

                var createdItem = this.GetPersistedItem(transaction, resolvedInfo.Partition, service, createInfo.Iid, securityContext);

                // call after create hook
                this.OperationSideEffectProcessor.AfterCreate(createdItem, resolvedContainerInfo.Thing, originalThing, transaction, resolvedInfo.Partition, securityContext);
            }
        }

        /// <summary>
        /// Executes the copy-operations
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="requestPartition">
        /// The current contexttual partition
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// If mandatory resources cannot be found to perform the operation
        /// </exception>
        private void ApplyCopyOperations(CdpPostOperation operation, NpgsqlTransaction transaction, string requestPartition)
        {
            // re-order create
            var targetModelPartition = requestPartition.Contains(typeof(EngineeringModel).Name) ? requestPartition : requestPartition.Replace(typeof(Iteration).Name, typeof(EngineeringModel).Name);
            var targetIterationPartition = requestPartition.Contains(typeof(EngineeringModel).Name) ? requestPartition.Replace(typeof(EngineeringModel).Name, typeof(Iteration).Name) : requestPartition;

            var modelSetupService = (IEngineeringModelSetupService)this.ServiceProvider.MapToReadService(ClassKind.EngineeringModelSetup.ToString());

            foreach (var copyinfo in operation.Copy)
            {
                var targetModelSetup = modelSetupService.GetEngineeringModelSetup(transaction, copyinfo.Target.TopContainer.Iid);
                if (targetModelSetup == null)
                {
                    throw new InvalidOperationException("The target engineering-model-setup could not be found");
                }

                var sourceThings = this.CopySourceService.GetCopySourceData(transaction, copyinfo, requestPartition);

                var service = this.ServiceProvider.MapToPersitableService(copyinfo.Source.Thing.ClassKind.ToString());
                var securityContext = new RequestSecurityContext { ContainerReadAllowed = true, ContainerWriteAllowed = true };

                var containerService = this.ServiceProvider.MapToReadService(copyinfo.Target.Container.ClassKind.ToString());
                var container = containerService.GetShallow(transaction, requestPartition, new[] {copyinfo.Target.Container.Iid}, securityContext).SingleOrDefault();
                if (container == null)
                {
                    throw new InvalidOperationException("The container for the copy operation cannot be found.");
                }

                var mrdlService = (IModelReferenceDataLibraryService)this.ServiceProvider.MapToReadService(typeof(ModelReferenceDataLibrary).Name);
                var iterationservice = this.ServiceProvider.MapToReadService(typeof(Iteration).Name);

                var targetIteration = (Iteration)iterationservice.GetShallow(transaction, targetModelPartition, new[] {copyinfo.Target.IterationId.Value}, securityContext).SingleOrDefault();
                if (targetIteration == null)
                {
                    throw new InvalidOperationException("The target iteration could not be found");
                }

                var rdls = mrdlService.QueryReferenceDataLibrary(transaction, targetIteration).ToList();

                // copy all sourceDtos from copySource
                var topCopy = sourceThings.Single(x => x.Iid == copyinfo.Source.Thing.Iid);

                var idmap = this.CopySourceService.GenerateCopyReference(sourceThings);

                var elementDefs = sourceThings.OfType<ElementDefinition>().ToList();
                if (elementDefs.Count == 1)
                {
                    ((ServiceBase)service).Copy(transaction, targetIterationPartition, topCopy, container, sourceThings, copyinfo, idmap, rdls, targetModelSetup, securityContext);
                }
                else
                {
                    // Copy across different models
                    // copy usages after all definitions
                    foreach (var elementDefinition in elementDefs)
                    {
                        ((ServiceBase)service).Copy(transaction, targetIterationPartition, elementDefinition, container, sourceThings, copyinfo, idmap, rdls, targetModelSetup, securityContext);
                    }

                    var sourceElementDefIds = elementDefs.Select(x => x.Iid).ToArray();
                    var elementDefCopyIds = idmap.Where(x => sourceElementDefIds.Contains(x.Key)).Select(x => x.Value);
                    var elementDefCopies = service.GetShallow(transaction, targetIterationPartition, elementDefCopyIds, securityContext).ToList();

                    var sourceUsages = sourceThings.OfType<ElementUsage>().ToList();
                    var usageService = this.ServiceProvider.MapToPersitableService(ClassKind.ElementUsage.ToString());
                    foreach (var elementUsage in sourceUsages)
                    {
                        var sourceElementDefContainer = elementDefs.Single(x => x.ContainedElement.Contains(elementUsage.Iid));
                        var elementDefContainer = elementDefCopies.SingleOrDefault(x => x.Iid == idmap[sourceElementDefContainer.Iid]);
                        if (elementDefContainer == null)
                        {
                            throw new InvalidOperationException($"The target element definition container could not be found for the usage to copy.");
                        }

                        ((ServiceBase)usageService).Copy(transaction, targetIterationPartition, elementUsage, elementDefContainer, sourceThings, copyinfo, idmap, rdls, targetModelSetup, securityContext);
                    }
                }

            }
        }

        /// <summary>
        /// The apply update operations.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        private void ApplyUpdateOperations(CdpPostOperation operation, NpgsqlTransaction transaction)
        {
            foreach (var updateInfo in operation.Update)
            {
                var updateInfoKey = updateInfo.GetInfoPlaceholder();

                var metaInfo = this.RequestUtils.MetaInfoProvider.GetMetaInfo(updateInfoKey.TypeName);
                var service = this.ServiceProvider.MapToPersitableService(updateInfoKey.TypeName);
                var securityContext = new RequestSecurityContext { ContainerReadAllowed = true, ContainerWriteAllowed = true };
                securityContext.Credentials = this.RequestUtils.Context.AuthenticatedCredentials;

                var resolvedInfo = this.operationThingCache[updateInfoKey];

                // get persisted thing
                var updatableThing = resolvedInfo.Thing;

                if (updatableThing == null)
                {
                    Logger.Info(
                        "The requested update resource '{0}' with iid: '{1}' could not be retrieved.",
                        updateInfoKey.TypeName,
                        updateInfoKey.Iid);

                    // skip further processing for this update item as it is deleted from the database
                    continue;
                }

                Thing containerInfo = null;
                if (!metaInfo.IsTopContainer)
                {
                    containerInfo = this.GetContainerInfo(updatableThing).Thing;
                }

                // keep a copy of the orginal thing to pass to the after update hook
                var originalThing = updatableThing.DeepClone<Thing>();

                if (this.operationOriginalThingCache.All(x => x.Iid != originalThing.Iid))
                {
                    this.operationOriginalThingCache.Add(originalThing);
                }

                // call before update hook
                this.OperationSideEffectProcessor.BeforeUpdate(updatableThing, containerInfo, transaction, resolvedInfo.Partition, securityContext, updateInfo);

                // track if updates occured
                var isUpdated = false;

                // iterate the update info properties other than Iid, revision and classkind
                foreach (var update in updateInfo.Where(x => !this.baseProperties.Contains(x.Key)))
                {
                    var propertyName = update.Key;
                    var propInfo = metaInfo.GetPropertyMetaInfo(propertyName);
                    
                    switch (propInfo.PropertyKind)
                    {
                        case PropertyKind.Scalar:
                        case PropertyKind.ValueArray:
                        {
                            // apply scalar or valuarray value update
                            if (metaInfo.ApplyPropertyUpdate(updatableThing, propertyName, update.Value))
                            {
                                isUpdated = true;
                            }

                            break;
                        }

                        case PropertyKind.List:
                        { 
                            var collectionItems = (IEnumerable)update.Value;
                            if (propInfo.Aggregation != AggregationKind.Composite)
                            {
                                // add new collection items to unordered non-composite list property
                                foreach (var newValue in collectionItems)
                                {
                                    service.AddToCollectionProperty(transaction, resolvedInfo.Partition, propertyName, resolvedInfo.InstanceInfo.Iid, newValue);
                                }
                            }
                            else if (propInfo.Aggregation == AggregationKind.Composite)
                            {
                                // move containment of collection items defined in unordered composite list property
                                foreach (Guid containedIid in collectionItems)
                                {
                                    if (operation.Create.Any(x => x.Iid == containedIid))
                                    {
                                        continue;
                                    }

                                    // change containment of the indicated item
                                    var containedThingService = this.ServiceProvider.MapToPersitableService(propInfo.TypeName);
                                    var containedThing = containedThingService.GetShallow(
                                        transaction,
                                        resolvedInfo.Partition,
                                        new[] { containedIid },
                                        new RequestSecurityContext { ContainerReadAllowed = true }).SingleOrDefault();

                                    if (containedThing == null)
                                    {
                                        Logger.Info(
                                            "The containment change of item '{0}' with iid: '{1}' was not completed as the item could not be retrieved.",
                                            propInfo.TypeName,
                                            containedIid);

                                        continue;
                                    }

                                    // try apply containment change
                                    if (!containedThingService.UpdateConcept(transaction, resolvedInfo.Partition, containedThing, updatableThing))
                                    {
                                        Logger.Info(
                                            "The containment change of item '{0}' with iid: '{1}' to container '{2}' with '{3}' could not be performed.",
                                            propInfo.TypeName,
                                            containedIid,
                                            resolvedInfo.InstanceInfo.TypeName,
                                            resolvedInfo.InstanceInfo.Iid);
                                    }
                                }
                            }

                            break;
                        }

                        case PropertyKind.OrderedList:
                        {
                            var orderedCollectionItems = ((IEnumerable<OrderedItem>)update.Value).ToList();
                            if (propInfo.Aggregation != AggregationKind.Composite)
                            {
                                foreach (var newOrderedItem in orderedCollectionItems.Where(x => !x.M.HasValue))
                                {
                                    service.DeleteFromCollectionProperty(transaction, resolvedInfo.Partition, propertyName, resolvedInfo.InstanceInfo.Iid, newOrderedItem);

                                    // add ordered item to collection property
                                    isUpdated = service.AddToCollectionProperty(
                                        transaction,
                                        resolvedInfo.Partition,
                                        propertyName,
                                        resolvedInfo.InstanceInfo.Iid,
                                        newOrderedItem);
                                }

                                foreach (var orderedItemUpdate in orderedCollectionItems.Where(x => x.M.HasValue))
                                {
                                    orderedItemUpdate.MoveItem(orderedItemUpdate.K, orderedItemUpdate.M.Value);

                                    // try apply collection property reorder
                                    isUpdated = service.ReorderCollectionProperty(transaction, resolvedInfo.Partition, propertyName, resolvedInfo.InstanceInfo.Iid, orderedItemUpdate);

                                    if (!isUpdated)
                                    { 
                                        Logger.Info(
                                            "The item '{0}' order update from sequence {1} to {2} of {3}.{4} with iid: '{5}' could not be performed.",
                                            orderedItemUpdate.V,
                                            orderedItemUpdate.K,
                                            orderedItemUpdate.M,
                                            resolvedInfo.InstanceInfo.TypeName,
                                            propertyName,
                                            updatableThing.Iid);
                                    }
                                }
                            }
                            else if (propInfo.Aggregation == AggregationKind.Composite)
                            {
                                // the create section will have handled new composite ordered items; only handle reordering of contained items here
                                foreach (var orderUpdateItemInfo in orderedCollectionItems.Where(x => x.M.HasValue))
                                {
                                    var containedThingService = this.ServiceProvider.MapToPersitableService(propInfo.TypeName);
                                    var containedItemIid = Guid.Parse(orderUpdateItemInfo.V.ToString());

                                    var containedThing = containedThingService.GetShallow(
                                            transaction,
                                            resolvedInfo.Partition,
                                            new[] { containedItemIid },
                                            new RequestSecurityContext { ContainerReadAllowed = true })
                                            .SingleOrDefault();

                                    if (containedThing == null)
                                    {
                                        Logger.Info(
                                            "The contained item '{0}' with iid: '{1}' could not be retrieved.",
                                            propInfo.TypeName,
                                            containedItemIid);

                                        continue;
                                    }

                                    // update containment order
                                    var orderedItemUpdate = new OrderedItem
                                    {
                                        V = containedItemIid
                                    };
                                    orderedItemUpdate.MoveItem(orderUpdateItemInfo.K, orderUpdateItemInfo.M.Value);

                                    isUpdated = containedThingService.ReorderContainment(transaction, resolvedInfo.Partition, orderedItemUpdate);

                                    if (!isUpdated)
                                    {
                                            Logger.Info(
                                                "The contained item '{0}' with iid: '{1}' could not be reordered.",
                                                propInfo.TypeName,
                                                containedItemIid);
                                    }
                                }
                            }

                            break;
                        }
                    }
                }

                if (isUpdated)
                {
                    // PreCheck CanWrite
                    if (service is ServiceBase serviceBase)
                    {
                        if (!serviceBase.TransactionManager.IsFullAccessEnabled()
                            && !serviceBase.PermissionService.CanWrite(transaction, originalThing, updateInfoKey.TypeName,
                            resolvedInfo.Partition, ServiceBase.UpdateOperation, securityContext))
                        {
                            throw new SecurityException("The person " + serviceBase.PermissionService.Credentials.Person.UserName + " does not have an appropriate update permission for " + originalThing.GetType().Name + ".");
                        }
                    }

                    // apply scalar updates to the thing
                    service.UpdateConcept(transaction, resolvedInfo.Partition, updatableThing, containerInfo);
                    
                    // get persisted thing
                    var updatedThing = this.GetPersistedItem(transaction, resolvedInfo.Partition, service, resolvedInfo.InstanceInfo.Iid, securityContext);

                    // call after update hook
                    this.OperationSideEffectProcessor.AfterUpdate(updatedThing, containerInfo, originalThing, transaction, resolvedInfo.Partition, securityContext);
                }
            }
        }

        /// <summary>
        /// Execute the delete operation
        /// </summary>
        /// <param name="dtoInfo">The <see cref="DtoInfo"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="metaInfo">The <see cref="IMetaInfo"/> for the deleted object</param>
        private void ExecuteDeleteOperation(DtoInfo dtoInfo, NpgsqlTransaction transaction, IMetaInfo metaInfo)
        {
            if (!this.operationThingCache.TryGetValue(dtoInfo, out var resolvedInfo))
            {
                Logger.Info("The item '{0}' with iid: '{1}' was already deleted: continue processing.", dtoInfo.TypeName, dtoInfo.Iid);
                return;
            }

            var persistedThing = resolvedInfo.Thing;

            var container = 
                metaInfo.IsTopContainer 
                    ? null 
                    : this.GetContainerInfo(persistedThing).Thing;

            if (container != null)
            {
                var originalContainer = container.DeepClone<Thing>();
                this.operationOriginalThingCache.Add(originalContainer);
            }

            // keep a copy of the orginal thing to pass to the after delete hook
            var originalThing = persistedThing.DeepClone<Thing>();

            if (this.operationOriginalThingCache.All(x => x.Iid != originalThing.Iid))
            {
                this.operationOriginalThingCache.Add(originalThing);
            }

            var securityContext = new RequestSecurityContext { ContainerReadAllowed = true };

            // call before delete hook
            this.OperationSideEffectProcessor.BeforeDelete(persistedThing, container, transaction, resolvedInfo.Partition, securityContext);

            // delete the item
            var propertyService = this.ServiceProvider.MapToPersitableService(dtoInfo.TypeName);
            this.DeletePersistedItem(transaction, resolvedInfo.Partition, propertyService, persistedThing);

            // call after delete hook
            this.OperationSideEffectProcessor.AfterDelete(persistedThing, container, originalThing, transaction, resolvedInfo.Partition, securityContext);
        }

        /// <summary>
        /// Reorder the create list of a <see cref="CdpPostOperation"/>
        /// </summary>
        /// <param name="postOperation">The <see cref="CdpPostOperation"/></param>
        /// <remarks>
        /// This is done to make sure that some things that depend on other are created last
        /// </remarks>
        private void ReorderCreateOrder(CdpPostOperation postOperation)
        {
            var subscriptions = postOperation.Create.OfType<ParameterSubscription>().ToArray();

            foreach (var subscription in subscriptions)
            {
                postOperation.Create.Remove(subscription);
            }

            postOperation.Create.AddRange(subscriptions);
        }
    }
}
