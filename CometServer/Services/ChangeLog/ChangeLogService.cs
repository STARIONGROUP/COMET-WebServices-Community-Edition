﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeLogService.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
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

namespace CometServer.Services.ChangeLog
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using CDP4Common;
    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.Helpers;
    using CDP4Common.Polyfills;

    using CDP4Orm.Dao;
    using CDP4Orm.Dao.Resolve;

    using CometServer.Exceptions;
    using CometServer.Helpers;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations;

    using Microsoft.Extensions.Logging;

    using Npgsql;

    using INamedThing = CDP4Common.DTO.INamedThing;
    using IServiceProvider = CometServer.Services.IServiceProvider;
    using IShortNamedThing = CDP4Common.DTO.IShortNamedThing;
    using LogEntryChangelogItem = CDP4Common.DTO.LogEntryChangelogItem;
    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// Implements the injectable <see cref="IChangeLogService"/> interface used to append change log data to the database.
    /// </summary>
    public class ChangeLogService : IChangeLogService
    {
        /// <summary>
        /// Gets or sets the (injected) <see cref="ILogger"/>
        /// </summary>
        public ILogger<ChangeLogService> Logger { get; set; }

        /// <summary>
        /// Property names that are excluded from <see cref="CDP4Common.DTO.LogEntryChangelogItem.ChangeDescription"/> text.
        /// </summary>
        private readonly string[] excludedPropertyNames = [nameof(Thing.Iid), nameof(Thing.ClassKind), nameof(Thing.ModifiedOn)];

        /// <summary>
        /// The injected <see cref="Services.IServiceProvider"/>
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// The injected <see cref="IOperationProcessor"/>
        /// </summary>
        public IOperationProcessor OperationProcessor { get; set; }

        /// <summary>
        /// The injected <see cref="IRequestUtils"/>
        /// </summary>
        public IRequestUtils RequestUtils { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IMetaInfoProvider"/>
        /// </summary>
        public IMetaInfoProvider MetaInfoProvider { get; set; }

        /// <summary>
        /// The injected <see cref="IOptionService"/>
        /// </summary>
        public IOptionService OptionService { get; set; }

        /// <summary>
        /// The injected <see cref="IActualFiniteStateService"/>
        /// </summary>
        public IActualFiniteStateService ActualFiniteStateService { get; set; }

        /// <summary>
        /// The injected <see cref="IPossibleFiniteStateService"/>
        /// </summary>
        public IPossibleFiniteStateService PossibleFiniteStateService { get; set; }

        /// <summary>
        /// The injected <see cref="IServiceProvider"/>
        /// </summary>
        public IParameterTypeService ParameterTypeService { get; set; }

        /// <summary>
        /// The injected <see cref="IParameterService"/>
        /// </summary>
        public IParameterService ParameterService { get; set; }

        /// <summary>
        /// The injected <see cref="IParameterOverrideService"/>
        /// </summary>
        public IParameterOverrideService ParameterOverrideService { get; set; }

        /// <summary>
        /// The injected <see cref="IParameterSubscriptionService"/>
        /// </summary>
        public IParameterSubscriptionService ParameterSubscriptionService { get; set; }

        /// <summary>
        /// The injected <see cref="IIterationSetupService"/>
        /// </summary>
        public IIterationSetupService IterationSetupService { get; set; }

        /// <summary>
        /// The injected <see cref="IParameterOrOverrideBaseService"/>
        /// </summary>
        public IParameterOrOverrideBaseService ParameterOrOverrideBaseService { get; set; }

        /// <summary>
        /// The injected <see cref="IParameterValueSetService"/>
        /// </summary>
        public IParameterValueSetService ParameterValueSetService { get; set; }

        /// <summary>
        /// The injected <see cref="IParameterValueSetBaseService"/>
        /// </summary>
        public IParameterValueSetBaseService ParameterValueSetBaseService { get; set; }

        /// <summary>
        /// The injected <see cref="IDataModelUtils"/>
        /// </summary>
        public IDataModelUtils DataModelUtils { get; set; }

        /// <summary>
        /// The injected <see cref="IResolveService"/>
        /// </summary>
        public IResolveService ResolveService { get; set; }

        /// <summary>
        /// The injected <see cref="ICdp4TransactionManager"/>
        /// </summary>
        public ICdp4TransactionManager TransactionManager { get; set; }

        /// <summary>
        /// Tries to append changelog data based on the changes made to certain <see cref="Thing"/>s.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="actor">
        /// The <see cref="Person.Iid"/> of the person that made the changes.
        /// </param>
        /// <param name="transactionRevision">
        /// The revisionNumber of the <see cref="transaction"/>
        /// </param>
        /// <param name="operation">
        /// <see cref="CdpPostOperation"/> that resulted to all the changes.
        /// </param>
        /// <param name="things">
        /// The <see cref="IReadOnlyList{T}"/> of type <see cref="CDP4Common.DTO.Thing"/> that contains changed <see cref="CDP4Common.DTO.Thing"/>
        /// </param>
        /// <returns>
        /// True if change log data was added, otherwise false
        /// </returns>
        public async Task<bool> TryAppendModelChangeLogDataAsync(NpgsqlTransaction transaction, string partition, Guid actor, int transactionRevision, CdpPostOperation operation, IReadOnlyList<Thing> things)
        {
            var sw = Stopwatch.StartNew();
            this.Logger.LogInformation("Starting to append changelog data");

            var isCachedDtoReadEnabled = await this.TransactionManager.IsCachedDtoReadEnabledAsync(transaction);

            if (!isCachedDtoReadEnabled)
            {
                this.TransactionManager.SetCachedDtoReadEnabled(true);
            }

            var isFullAccessEnabled = this.TransactionManager.IsFullAccessEnabled();
            var result = false;

            if (!isFullAccessEnabled)
            {
                this.TransactionManager.SetFullAccessState(true);
            }

            try
            {
                var changedEngineeringModels =
                    things
                        .Where(x => x.ClassKind == ClassKind.EngineeringModel)
                        .Cast<EngineeringModel>()
                        .ToList();

                if (changedEngineeringModels.Count == 1)
                {
                    var addModelLogEntryToOperation = false;

                    var engineeringModel = changedEngineeringModels.First();
                    var changedThings = things.Where(x => x.RevisionNumber == transactionRevision).ToList();

                    var modelLogEntries =
                        operation
                            .Create
                            .Where(x => x.ClassKind == ClassKind.ModelLogEntry)
                            .Cast<ModelLogEntry>()
                            .ToList();

                    ModelLogEntry modelLogEntry;

                    if (modelLogEntries.Count != 1)
                    {
                        var newModelLogEntry = new ModelLogEntry(Guid.NewGuid(), 0)
                        {
                            Level = LogLevelKind.INFO,
                            Author = actor,
                            LanguageCode = "en-GB",
                            Content = "-"
                        };

                        addModelLogEntryToOperation = true;

                        engineeringModel.LogEntry.Add(newModelLogEntry.Iid);

                        modelLogEntry = newModelLogEntry;
                    }
                    else
                    {
                        modelLogEntry = modelLogEntries.First();
                    }

                    var newLogEntryChangelogItems = new List<LogEntryChangelogItem>();

                    foreach (var changedThing in changedThings.Where(x => x.ClassKind != ClassKind.ModelLogEntry))
                    {
                        var newLogEntryChangelogItem = await this.CreateAddOrUpdateLogEntryChangelogItemAsync(transaction, partition, changedThing, modelLogEntry, operation, changedThings);

                        if (newLogEntryChangelogItem == null)
                        {
                            continue;
                        }

                        newLogEntryChangelogItems.Add(newLogEntryChangelogItem);

                        if (changedThing is IOwnedThing ownedThing && !this.DataModelUtils.IsDerived(changedThing.ClassKind.ToString(), nameof(IOwnedThing.Owner)))
                        {
                            AddIfNotExists(modelLogEntry.AffectedDomainIid, ownedThing.Owner);
                            AddIfNotExists(newLogEntryChangelogItem.AffectedReferenceIid, ownedThing.Owner);
                        }

                        if (changedThing is ICategorizableThing categorizableThing)
                        {
                            foreach (var categoryIid in categorizableThing.Category)
                            {
                                AddIfNotExists(modelLogEntry.AffectedItemIid, categoryIid);
                                AddIfNotExists(newLogEntryChangelogItem.AffectedReferenceIid, categoryIid);
                            }
                        }
                    }

                    foreach (var deleteInfo in operation.Delete)
                    {
                        var newLogEntryChangelogItem =
                            await this.CreateDeleteLogEntryChangelogItemsAsync(transaction, partition, deleteInfo, modelLogEntry, changedThings);

                        if (newLogEntryChangelogItem != null)
                        {
                            newLogEntryChangelogItems.Add(newLogEntryChangelogItem);
                        }
                    }

                    if (newLogEntryChangelogItems.Count != 0)
                    {
                        modelLogEntry.LogEntryChangelogItem.AddRange(newLogEntryChangelogItems.Select(x => x.Iid));

                        var operationData = new CdpPostOperation();

                        operationData.Create.AddRange(newLogEntryChangelogItems);

                        if (addModelLogEntryToOperation)
                        {
                            var engineeringModelClasslessDTO = ClasslessDtoFactory
                                .FromThing(this.MetaInfoProvider,
                                    engineeringModel);

                            engineeringModelClasslessDTO.Add(nameof(EngineeringModel.LogEntry), new[] { modelLogEntry.Iid });

                            operationData.Update.Add(engineeringModelClasslessDTO);
                            operationData.Create.Add(modelLogEntry);
                        }
                        else
                        {
                            var modelLogEntryClasslessDTO =
                                ClasslessDtoFactory
                                    .FromThing(this.MetaInfoProvider,
                                        modelLogEntry,
                                        [nameof(ModelLogEntry.LogEntryChangelogItem), nameof(ModelLogEntry.AffectedItemIid), nameof(ModelLogEntry.AffectedDomainIid)]);

                            operationData.Update.Add(modelLogEntryClasslessDTO);
                        }

                        // New things that need to be read that are not yet in cache at this moment in time
                        this.TransactionManager.SetCachedDtoReadEnabled(false);
                        await this.OperationProcessor.ProcessAsync(operationData, transaction, partition);

                        result = true;
                    }
                }
            }
            catch (ResolveException ex)
            {
                this.Logger.LogDebug(ex, "{Message}", ex.Message);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "operation failed");
            }
            finally
            {
                if (!isFullAccessEnabled)
                {
                    this.TransactionManager.SetFullAccessState(false);
                }

                if (!isCachedDtoReadEnabled && await this.TransactionManager.IsCachedDtoReadEnabledAsync(transaction))
                {
                    this.TransactionManager.SetCachedDtoReadEnabled(false);
                }
            }

            sw.Stop();

            this.Logger.LogInformation("Finished appending to changelog data in {ElapsedMilliseconds} [ms]", sw.ElapsedMilliseconds);

            return result;
        }

        /// <summary>
        /// Adds a <see cref="CDP4Common.DTO.LogEntryChangelogItem"/> to a <see cref="ModelLogEntry"/> for update and create operations
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="changedThing">
        /// The <see cref="Thing"/> that was changed.</param>
        /// <param name="modelLogEntry">
        /// The <see cref="ModelLogEntry"/>
        /// </param>
        /// <param name="operation">
        /// The <see cref="CdpPostOperation"/>
        /// </param>
        /// <param name="changedThings">
        /// An <see cref="IReadOnlyList{T}"/> of type <see cref="Thing"/> that contains all changed things.
        /// </param>
        /// <returns>
        /// The created <see cref="CDP4Common.CommonData.LogEntryChangelogItem"/> if one was created, otherwise null.
        /// </returns>
        private async Task<LogEntryChangelogItem> CreateAddOrUpdateLogEntryChangelogItemAsync(NpgsqlTransaction transaction, string partition, Thing changedThing, ModelLogEntry modelLogEntry, CdpPostOperation operation, IReadOnlyList<Thing> changedThings)
        {
            if (!IsAddLogEntryChangeLogItemAllowed(changedThing.ClassKind))
            {
                return null;
            }

            var logEntryChangeLogItem = new LogEntryChangelogItem(Guid.NewGuid(), 0);
            AddIfNotExists(modelLogEntry.AffectedItemIid, changedThing.Iid);

            if (operation.Create.SingleOrDefault(x => x.Iid == changedThing.Iid) is not null)
            {
                await this.SetCreateLogEntryChangeLogItemAsync(transaction, partition, changedThing, modelLogEntry, logEntryChangeLogItem, changedThings);
                return logEntryChangeLogItem;
            }

            if (operation.Update.SingleOrDefault(x => x[nameof(Thing.Iid)] as Guid? == changedThing.Iid) is { } updateOperation)
            {
                await this.SetUpdateLogEntryChangeLogItemAsync(transaction, partition, changedThing, modelLogEntry, logEntryChangeLogItem, updateOperation);
                return logEntryChangeLogItem;
            }

            return null;
        }

        /// <summary>
        /// Adds one or more <see cref="LogEntryChangelogItem"/>s to a <see cref="ModelLogEntry"/> for delete operations
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="deleteInfo">
        /// The <see cref="ClasslessDTO"/>.
        /// </param>
        /// <param name="modelLogEntry">
        /// The <see cref="ModelLogEntry"/>
        /// </param>
        /// <param name="changedThings">
        /// An <see cref="IReadOnlyList{T}"/> of type <see cref="Thing"/> that contains all changed things.
        /// </param>
        /// <returns>
        /// The created <see cref="CDP4Common.CommonData.LogEntryChangelogItem"/>s.
        /// </returns>
        private async Task<LogEntryChangelogItem> CreateDeleteLogEntryChangelogItemsAsync(NpgsqlTransaction transaction, string partition, ClasslessDTO deleteInfo, ModelLogEntry modelLogEntry, IReadOnlyList<Thing> changedThings)
        {
            if (Enum.TryParse<ClassKind>(deleteInfo[nameof(Thing.ClassKind)].ToString(), out var classKind) && IsAddLogEntryChangeLogItemAllowed(classKind))
            {
                var newLogEntryChangelogItem = new LogEntryChangelogItem(Guid.NewGuid(), 0);
                AddIfNotExists(modelLogEntry.AffectedItemIid, (Guid) deleteInfo[nameof(Thing.Iid)]);

                var deletedThing = this.OperationProcessor.OperationOriginalThingCache.FirstOrDefault(x => x.Iid == (Guid) deleteInfo[nameof(Thing.Iid)]);

                if (deletedThing != null)
                {
                    await this.SetDeleteLogEntryChangeLogItemAsync(transaction, partition, deletedThing, modelLogEntry, newLogEntryChangelogItem, deleteInfo, changedThings);
                }

                return newLogEntryChangelogItem;
            }

            return null;
        }

        /// <summary>
        /// Fills a <see cref="LogEntryChangelogItem"/> based on a newly created <see cref="Thing"/> with appropriate data
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="createdThing">
        /// The newly created <see cref="Thing"/>
        /// </param>
        /// <param name="modelLogEntry">
        /// The <see cref="ModelLogEntry"/>
        /// </param>
        /// <param name="logEntryChangeLogItem">
        /// The <see cref="LogEntryChangelogItem"/>
        /// </param>
        /// <param name="changedThings">
        /// An <see cref="IReadOnlyList{T}"/> of type <see cref="Thing"/> that contains all changed things.
        /// </param>
        private async Task SetCreateLogEntryChangeLogItemAsync(NpgsqlTransaction transaction, string partition, Thing createdThing, ModelLogEntry modelLogEntry, LogEntryChangelogItem logEntryChangeLogItem, IReadOnlyList<Thing> changedThings)
        {
            logEntryChangeLogItem.ChangelogKind = LogEntryChangelogItemKind.ADD;
            logEntryChangeLogItem.AffectedItemIid = createdThing.Iid;

            if (createdThing is IOwnedThing ownedThing && !this.DataModelUtils.IsDerived(createdThing.ClassKind.ToString(), nameof(IOwnedThing.Owner)))
            {
                AddIfNotExists(modelLogEntry.AffectedDomainIid, ownedThing.Owner);
                AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, ownedThing.Owner);
            }

            if (createdThing is ICategorizableThing categorizableThing)
            {
                foreach (var categoryIid in categorizableThing.Category)
                {
                    AddIfNotExists(modelLogEntry.AffectedItemIid, categoryIid);
                    AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, categoryIid);
                }
            }

            var stringBuilder = new StringBuilder();

            var containerThing = GetContainerFromChangedThings(createdThing, changedThings);

            if (containerThing != null)
            {
                AddIfNotExists(modelLogEntry.AffectedItemIid, containerThing.Iid);
                AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, containerThing.Iid);

                stringBuilder.AppendLine($"* {await this.GetThingDescriptionAsync(transaction, partition, containerThing)}");
            }

            var affectedThingsData = await this.GetAffectedThingsDataAsync(transaction, partition, createdThing, null);

            foreach (var affectedItemId in affectedThingsData.AffectedItemIds)
            {
                AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, affectedItemId);
                AddIfNotExists(modelLogEntry.AffectedItemIid, affectedItemId);
            }

            foreach (var affectedDomainId in affectedThingsData.AffectedDomainIds)
            {
                AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, affectedDomainId);
                AddIfNotExists(modelLogEntry.AffectedDomainIid, affectedDomainId);
            }

            foreach (var extraChangeDescription in affectedThingsData.ExtraChangeDescriptions)
            {
                stringBuilder.AppendLine(extraChangeDescription);
            }

            stringBuilder.AppendLine($"* {await this.GetThingDescriptionAsync(transaction, partition, createdThing)}");

            var customDescriptions = await this.GetCustomDescriptionsAsync(transaction, partition, createdThing);

            if (!string.IsNullOrWhiteSpace(customDescriptions))
            {
                stringBuilder.AppendLine($"* {customDescriptions}");
            }

            logEntryChangeLogItem.ChangeDescription = stringBuilder.ToString();
        }

        /// <summary>
        /// Fills a <see cref="LogEntryChangelogItem"/> based on a modified <see cref="Thing"/> with appropriate data
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="updatedThing">
        /// The modified <see cref="Thing"/>
        /// </param>
        /// <param name="modelLogEntry">
        /// The <see cref="ModelLogEntry"/>
        /// </param>
        /// <param name="logEntryChangeLogItem">
        /// The <see cref="LogEntryChangelogItem"/>
        /// </param>
        /// <param name="updateOperation">
        /// The original update operation as a <see cref="ClasslessDTO"/>
        /// </param>
        private async Task SetUpdateLogEntryChangeLogItemAsync(NpgsqlTransaction transaction, string partition, Thing updatedThing, ModelLogEntry modelLogEntry, LogEntryChangelogItem logEntryChangeLogItem, ClasslessDTO updateOperation)
        {
            logEntryChangeLogItem.ChangelogKind = LogEntryChangelogItemKind.UPDATE;
            logEntryChangeLogItem.AffectedItemIid = updatedThing.Iid;

            if (updatedThing is IOwnedThing ownedThing && !this.DataModelUtils.IsDerived(updatedThing.ClassKind.ToString(), nameof(IOwnedThing.Owner)))
            {
                AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, ownedThing.Owner);
                AddIfNotExists(modelLogEntry.AffectedDomainIid, ownedThing.Owner);
            }

            if (updatedThing is ICategorizableThing categorizableThing)
            {
                foreach (var categoryIid in categorizableThing.Category)
                {
                    AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, categoryIid);
                    AddIfNotExists(modelLogEntry.AffectedItemIid, categoryIid);
                }
            }

            var affectedThingsData = await this.GetAffectedThingsDataAsync(transaction, partition, updatedThing, updateOperation);

            foreach (var affectedItemId in affectedThingsData.AffectedItemIds)
            {
                AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, affectedItemId);
                AddIfNotExists(modelLogEntry.AffectedItemIid, affectedItemId);
            }

            foreach (var affectedDomainId in affectedThingsData.AffectedDomainIds)
            {
                AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, affectedDomainId);
                AddIfNotExists(modelLogEntry.AffectedDomainIid, affectedDomainId);
            }

            var stringBuilder = new StringBuilder();

            foreach (var extraChangeDescription in affectedThingsData.ExtraChangeDescriptions)
            {
                stringBuilder.AppendLine(extraChangeDescription);
            }

            var containerThing = await this.GetContainerAsync(transaction, partition, updatedThing);

            if (containerThing != null)
            {
                AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, containerThing.Iid);
                AddIfNotExists(modelLogEntry.AffectedItemIid, containerThing.Iid);
                stringBuilder.AppendLine($"* {await this.GetThingDescriptionAsync(transaction, partition, containerThing)}");
            }

            stringBuilder.AppendLine($"* {await this.GetThingDescriptionAsync(transaction, partition, updatedThing)}");

            var extraDescriptions = await this.GetCustomDescriptionsAsync(transaction, partition, updatedThing);

            if (!string.IsNullOrEmpty(extraDescriptions))
            {
                stringBuilder.AppendLine(extraDescriptions);
            }

            var changedValues = await this.BuildUpdateOperationTextAsync(transaction, partition, updateOperation);

            stringBuilder.AppendLine(changedValues);

            logEntryChangeLogItem.ChangeDescription = stringBuilder.ToString();
        }

        /// <summary>
        /// Builds a textual representation of a changed property
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="updateOperation">
        /// The <see cref="ClasslessDTO"/>
        /// </param>
        /// <returns>T <see cref="string"/></returns>
        private async Task<string> BuildUpdateOperationTextAsync(NpgsqlTransaction transaction, string partition, ClasslessDTO updateOperation)
        {
            var relevantOperations = updateOperation.Where(x => !this.excludedPropertyNames.Contains(x.Key)).ToList();
            var stringBuilder = new StringBuilder();

            foreach (var operation in relevantOperations)
            {
                var originalThingIid = (Guid) updateOperation[nameof(Thing.Iid)];

                var originalThing = this.FindOriginalThing(originalThingIid);

                if (originalThing == null)
                {
                    stringBuilder.AppendLine($"  - {operation.Key} was changed");
                    continue;
                }

                if (operation.Value is Guid changedValue)
                {
                    await this.AddChangedThingIidLineAsync(transaction, partition, originalThing, operation.Key, changedValue, stringBuilder);
                }
                else if (operation.Value is IEnumerable<Guid> guids)
                {
                    foreach (var guid in guids)
                    {
                        await this.AddChangedThingIidLineAsync(transaction, partition, originalThing, operation.Key, guid, stringBuilder);
                    }
                }
                else
                {
                    var metaInfoProvider = this.MetaInfoProvider.GetMetaInfo(originalThing);
                    var orgValue = metaInfoProvider.GetValue(operation.Key, originalThing);
                    stringBuilder.AppendLine($"  - {operation.Key}: {orgValue} => {operation.Value}");
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Adds a line to a <see cref="StringBuilder"/> that holds the textual representation of a changed property that holds a <see cref="Thing.Iid"/>,
        /// or an <see cref="IEnumerable{T}"/> of type <see cref="Guid"/> that holds <see cref="Thing.Iid"/>s
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="originalThing">
        /// The original changed <see cref="Thing"/>
        /// </param>
        /// <param name="propertyName">
        /// The name of the changed property
        /// </param>
        /// <param name="changedValue">
        /// The <see cref="Thing.Iid"/> of the <see cref="Thing"/> that was changed in the <see cref="originalThing"/>'s property.
        /// </param>
        /// <param name="stringBuilder">
        /// The <see cref="StringBuilder"/>
        /// </param>
        private async Task AddChangedThingIidLineAsync(NpgsqlTransaction transaction, string partition, Thing originalThing, string propertyName, Guid changedValue, StringBuilder stringBuilder)
        {
            var metaInfoProvider = this.MetaInfoProvider.GetMetaInfo(originalThing);
            var metaInfo = metaInfoProvider.GetPropertyMetaInfo(propertyName);

            var securityContext = new RequestSecurityContext { ContainerReadAllowed = true };
            var service = this.ServiceProvider.MapToPersitableService(metaInfo.TypeName);

            var dtoInfo = new DtoInfo(metaInfo.TypeName, changedValue);
            var dtoResolverHelper = new DtoResolveHelper(dtoInfo);
            var resolverDictionary = new Dictionary<DtoInfo, DtoResolveHelper> { { dtoInfo, dtoResolverHelper } };

            if (this.DataModelUtils.GetSourcePartition(metaInfo.TypeName) != null)
            {
                await this.ResolveService.ResolveItemsAsync(transaction, partition, resolverDictionary);

                var changedValueThing = (await service.GetShallowAsync(transaction, dtoResolverHelper.Partition, [changedValue], securityContext)).FirstOrDefault();

                var orgValue = metaInfoProvider.GetValue(propertyName, originalThing);

                if (orgValue is IEnumerable)
                {
                    if (TryGetName(changedValueThing, out var changedThingName))
                    {
                        stringBuilder.AppendLine($"  - {propertyName}: Added => {changedThingName}");
                        return;
                    }
                    
                    if (changedValueThing != null)
                    {
                        stringBuilder.AppendLine($"  - {propertyName}: Added => {changedValueThing.ClassKind}");
                        return;
                    }
                }
                else
                {
                    if (orgValue != null)
                    {
                        var orgValueThing = (await service.GetShallowAsync(transaction, dtoResolverHelper.Partition, [(Guid) orgValue], securityContext)).FirstOrDefault();

                        if (TryGetName(changedValueThing, out var changedThingName) && TryGetName(orgValueThing, out var orgThingName))
                        {
                            stringBuilder.AppendLine($"  - {propertyName}: {orgThingName} => {changedThingName}");
                            return;
                        }
                    }
                }
            }

            stringBuilder.AppendLine($"  - {propertyName} was changed");
        }

        /// <summary>
        /// Finds a <see cref="Thing"/> in the <see cref="OperationProcessor"/>'s cache by its <see cref="Thing.Iid"/> 
        /// </summary>
        /// <param name="guid">
        /// The <see cref="Thing.Iid"/>
        /// </param>
        /// <returns>
        /// A <see cref="Thing"/> is found, otherwise null.
        /// </returns>
        private Thing FindOriginalThing(Guid guid)
        {
            foreach (var thing in this.OperationProcessor.OperationOriginalThingCache.ToArray())
            {
                if (thing.Iid.Equals(guid))
                {
                    return thing;
                }
            }

            return null;
        }

        /// <summary>
        /// Fills a <see cref="LogEntryChangelogItem"/> based on a deleted <see cref="Thing"/> with appropriate data
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="deletedThing">
        /// The modified <see cref="Thing"/>
        /// </param>
        /// <param name="modelLogEntry">
        /// The <see cref="ModelLogEntry"/>
        /// </param>
        /// <param name="logEntryChangeLogItem">
        /// The <see cref="LogEntryChangelogItem"/>
        /// </param>
        /// <param name="deleteOperation">
        /// The original update operation as a <see cref="ClasslessDTO"/>
        /// </param>
        /// <param name="changedThings">
        /// An <see cref="IReadOnlyList{T}"/> of type <see cref="Thing"/> that contains all changed things.
        /// </param>
        private async Task SetDeleteLogEntryChangeLogItemAsync(NpgsqlTransaction transaction, string partition, Thing deletedThing, ModelLogEntry modelLogEntry, LogEntryChangelogItem logEntryChangeLogItem, ClasslessDTO deleteOperation, IReadOnlyList<Thing> changedThings)
        {
            logEntryChangeLogItem.ChangelogKind = LogEntryChangelogItemKind.DELETE;
            logEntryChangeLogItem.AffectedItemIid = deletedThing.Iid;

            if (deletedThing is IOwnedThing ownedThing && !this.DataModelUtils.IsDerived(deletedThing.ClassKind.ToString(), nameof(IOwnedThing.Owner)))
            {
                AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, ownedThing.Owner);
                AddIfNotExists(modelLogEntry.AffectedDomainIid, ownedThing.Owner);
            }

            if (deletedThing is ICategorizableThing categorizableThing)
            {
                foreach (var categoryIid in categorizableThing.Category)
                {
                    AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, categoryIid);
                    AddIfNotExists(modelLogEntry.AffectedItemIid, categoryIid);
                }
            }

            var affectedThingsData = await this.GetAffectedThingsDataAsync(transaction, partition, deletedThing, deleteOperation, deletedThing);

            foreach (var affectedItemId in affectedThingsData.AffectedItemIds)
            {
                AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, affectedItemId);
                AddIfNotExists(modelLogEntry.AffectedItemIid, affectedItemId);
            }

            foreach (var affectedDomainId in affectedThingsData.AffectedDomainIds)
            {
                AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, affectedDomainId);
                AddIfNotExists(modelLogEntry.AffectedDomainIid, affectedDomainId);
            }

            var stringBuilder = new StringBuilder();

            foreach (var extraChangeDescription in affectedThingsData.ExtraChangeDescriptions)
            {
                stringBuilder.AppendLine(extraChangeDescription);
            }

            var containerThing = this.GetDeletedThingContainer(deletedThing, changedThings);

            if (containerThing != null)
            {
                AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, containerThing.Iid);
                AddIfNotExists(modelLogEntry.AffectedItemIid, containerThing.Iid);
                stringBuilder.AppendLine($"* {await this.GetThingDescriptionAsync(transaction, partition, containerThing)}");
            }

            stringBuilder.AppendLine($"* {await this.GetThingDescriptionAsync(transaction, partition, deletedThing)}");

            var extraDescriptions = await this.GetCustomDescriptionsAsync(transaction, partition, deletedThing);

            if (!string.IsNullOrEmpty(extraDescriptions))
            {
                stringBuilder.AppendLine(extraDescriptions);
            }

            logEntryChangeLogItem.ChangeDescription = stringBuilder.ToString();
        }

        /// <summary>
        /// Gets custom affected <see cref="Thing"/>s data as a <see cref="AffectedThingsData"/> object.
        /// <see cref="AffectedThingsData"/> contains AffectedItems, AffectedDomains and extra ChangeDescription text
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="rootThing">
        /// The original <see cref="Thing"/> that started a nested tree of calls to this method
        /// </param>
        /// <param name="updateOperation"></param>
        /// <param name="thing">
        /// The <see cref="Thing"/> we want to get custom data for.
        /// </param>
        /// <returns>
        /// The <see cref="AffectedThingsData"/>
        /// </returns>
        private async Task<AffectedThingsData> GetAffectedThingsDataAsync(NpgsqlTransaction transaction, string partition, Thing rootThing, ClasslessDTO updateOperation, Thing thing = null)
        {
            thing ??= rootThing;
            var affectedThingsData = new AffectedThingsData();
            var getContainerReferences = thing != rootThing;

            var basePartition = partition.Replace("EngineeringModel", "").Replace("Iteration", "");
            var engineeringModelPartition = $"EngineeringModel{basePartition}";
            var iterationPartition = $"Iteration{basePartition}";
            var siteDirectoryPartition = "SiteDirectory";

            var securityContext = new RequestSecurityContext { ContainerReadAllowed = true };

            switch (thing)
            {
                case Parameter parameter:
                {
                        AddIfNotExists(affectedThingsData.AffectedItemIds, parameter.ParameterType);

                    if ((await this.ParameterTypeService.GetShallowAsync(transaction, siteDirectoryPartition, [parameter.ParameterType], securityContext)).Single() is ParameterType parameterType)
                    {
                        foreach (var category in parameterType.Category)
                        {
                                AddIfNotExists(affectedThingsData.AffectedItemIds, category);
                        }
                    }

                    if (!(rootThing is ParameterSubscription) && !(rootThing is ParameterSubscriptionValueSet) && !(rootThing is ParameterOverride) && !(rootThing is ParameterOverrideValueSet))
                    {
                        if (updateOperation != null &&
                            (updateOperation.Keys.Count > 3 || !updateOperation.Keys.Contains(nameof(Parameter.ParameterSubscription))))
                        {
                            foreach (var parameterSubscription in parameter.ParameterSubscription)
                            {
                                    AddIfNotExists(affectedThingsData.AffectedItemIds, parameterSubscription);
                            }
                        }
                    }

                    if (await this.GetContainerAsync(transaction, engineeringModelPartition, parameter) is ElementDefinition elementDefinition)
                    {
                        var newAffectedItemsData = await this.GetAffectedThingsDataAsync(transaction, partition, rootThing, updateOperation, elementDefinition);
                        affectedThingsData.AddFrom(newAffectedItemsData);
                    }

                    if (getContainerReferences)
                    {
                            AddIfNotExists(affectedThingsData.AffectedDomainIds, parameter.Owner);
                            AddIfNotExists(affectedThingsData.AffectedItemIds, parameter.Iid);
                    }

                    break;
                }
                case ElementUsage elementUsage:
                {
                    if (await this.GetContainerAsync(transaction, engineeringModelPartition, elementUsage) is ElementDefinition elementDefinition)
                    {
                        var newAffectedItemsData = await this.GetAffectedThingsDataAsync(transaction, partition, rootThing, updateOperation, elementDefinition);
                        affectedThingsData.AddFrom(newAffectedItemsData);
                    }

                    if (getContainerReferences)
                    {
                        affectedThingsData.ExtraChangeDescriptions.Add($"* ElementUsage: {elementUsage.Name} ({elementUsage.ShortName})");

                        foreach (var category in elementUsage.Category)
                        {
                            AddIfNotExists(affectedThingsData.AffectedItemIds, category);
                        }

                        AddIfNotExists(affectedThingsData.AffectedDomainIds, elementUsage.Owner);
                        AddIfNotExists(affectedThingsData.AffectedItemIds, elementUsage.Iid);
                    }

                    break;
                }
                case ElementDefinition elementDefinition:
                {
                    if (getContainerReferences)
                    {
                        affectedThingsData.ExtraChangeDescriptions.Add($"* ElementDefinition: {elementDefinition.Name} ({elementDefinition.ShortName})");

                        foreach (var category in elementDefinition.Category)
                        {
                            AddIfNotExists(affectedThingsData.AffectedItemIds, category);
                        } 
                        
                        AddIfNotExists(affectedThingsData.AffectedDomainIds, elementDefinition.Owner);
                        AddIfNotExists(affectedThingsData.AffectedItemIds, elementDefinition.Iid);
                    }

                    break;
                }

                case ParameterOverride parameterOverride:
                {
                    if (!(rootThing is ParameterSubscription) && !(rootThing is ParameterSubscriptionValueSet))
                    {
                        if (updateOperation != null &&
                            (updateOperation.Keys.Count > 3 || !updateOperation.Keys.Contains(nameof(Parameter.ParameterSubscription))))
                        {
                            foreach (var parameterSubscription in parameterOverride.ParameterSubscription)
                            {
                                    AddIfNotExists(affectedThingsData.AffectedItemIds, parameterSubscription);
                            }
                        }
                    }

                    var parameterOverrideParameter = (await this.ParameterService.GetShallowAsync(transaction, iterationPartition, [parameterOverride.Parameter], securityContext))
                        .Cast<Parameter>()
                        .SingleOrDefault();

                    if (parameterOverrideParameter != null)
                    {
                        var newAffectedItemsData = await this.GetAffectedThingsDataAsync(transaction, partition, rootThing, updateOperation, parameterOverrideParameter);
                        affectedThingsData.AddFrom(newAffectedItemsData);
                    }

                    if (await this.GetContainerAsync(transaction, engineeringModelPartition, parameterOverride) is ElementUsage elementUsage)
                    {
                        var newAffectedItemsData = await this.GetAffectedThingsDataAsync(transaction, partition, rootThing, updateOperation, elementUsage);
                        affectedThingsData.AddFrom(newAffectedItemsData);
                    }

                    if (getContainerReferences)
                    {
                            AddIfNotExists(affectedThingsData.AffectedDomainIds, parameterOverride.Owner);
                            AddIfNotExists(affectedThingsData.AffectedItemIds, parameterOverride.Iid);
                    }

                    break;
                }
                case ParameterValueSet parameterValueSet:
                {
                    var parameterValueSetParameters = (await this.ParameterService.GetShallowAsync(transaction, iterationPartition, null, securityContext))
                        .Cast<Parameter>()
                        .ToList();

                    var parameterValueSetParameter = parameterValueSetParameters.SingleOrDefault(x => x.ValueSet.Contains(parameterValueSet.Iid));

                    if (parameterValueSetParameter != null)
                    {
                        var newAffectedItemsData = await this.GetAffectedThingsDataAsync(transaction, partition, rootThing, updateOperation, parameterValueSetParameter);
                        affectedThingsData.AddFrom(newAffectedItemsData);
                    }

                    break;
                }
                case ParameterOverrideValueSet parameterOverrideValueSet:
                {
                    var parameterOverrideValueSetParameterOverrides = (await this.ParameterOverrideService.GetShallowAsync(transaction, iterationPartition, null, securityContext))
                        .Cast<ParameterOverride>()
                        .ToList();

                    var parameterOverrideValueSetParameterOverride = parameterOverrideValueSetParameterOverrides.SingleOrDefault(x => x.ValueSet.Contains(parameterOverrideValueSet.Iid));

                    if (parameterOverrideValueSetParameterOverride != null)
                    {
                        var newAffectedItemsData = await this.GetAffectedThingsDataAsync(transaction, partition, rootThing, updateOperation, parameterOverrideValueSetParameterOverride);
                        affectedThingsData.AddFrom(newAffectedItemsData);
                    }

                    break;
                }
                case ParameterSubscription parameterSubscription:
                {
                    var parameterOrOverrideBases = (await this.ParameterOrOverrideBaseService.GetShallowAsync(transaction, iterationPartition, null, securityContext))
                        .Cast<ParameterOrOverrideBase>()
                        .ToList();

                    var parameterOrOverrideBase = parameterOrOverrideBases.SingleOrDefault(x => x.ParameterSubscription.Contains(parameterSubscription.Iid));

                    if (parameterOrOverrideBase != null)
                    {
                        var newAffectedItemsData = await this.GetAffectedThingsDataAsync(transaction, partition, rootThing, updateOperation, parameterOrOverrideBase);
                        affectedThingsData.AddFrom(newAffectedItemsData);
                    }

                    if (getContainerReferences)
                    {
                            AddIfNotExists(affectedThingsData.AffectedDomainIds, parameterSubscription.Owner);
                            AddIfNotExists(affectedThingsData.AffectedItemIds, parameterSubscription.Iid);
                    }

                    break;
                }
                case ParameterSubscriptionValueSet parameterSubscriptionValueSet:
                {
                    var parameterSubscriptionValueSetParameterSubscriptions = (await this.ParameterSubscriptionService.GetShallowAsync(transaction, iterationPartition, null, securityContext))
                        .Cast<ParameterSubscription>()
                        .ToList();

                    var parameterSubscriptionValueSetParameterSubscription = parameterSubscriptionValueSetParameterSubscriptions.SingleOrDefault(x => x.ValueSet.Contains(parameterSubscriptionValueSet.Iid));

                    if (parameterSubscriptionValueSetParameterSubscription != null)
                    {
                        var newAffectedItemsData = await this.GetAffectedThingsDataAsync(transaction, partition, rootThing, updateOperation, parameterSubscriptionValueSetParameterSubscription);
                        affectedThingsData.AddFrom(newAffectedItemsData);
                    }

                    break;
                }
            }

            return affectedThingsData;
        }

        /// <summary>
        /// Tries to get the <paramref name="updatedThing"/>'s container.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="updatedThing">
        /// The updated <see cref="Thing"/>
        /// </param>
        /// <returns>
        /// <paramref name="updatedThing"/>'s container <see cref="Thing"/> if found, otherwise null
        /// </returns>
        private async Task<Thing> GetContainerAsync(NpgsqlTransaction transaction, string partition, Thing updatedThing)
        {
            try
            {
                var containerClassType = ContainerPropertyHelper.ContainerClassName(updatedThing.ClassKind);

                if (containerClassType == updatedThing.ClassKind.ToString())
                {
                    return null;
                }

                var containerPropertyName = ContainerPropertyHelper.ContainerPropertyName(updatedThing.ClassKind);
                var securityContext = new RequestSecurityContext { ContainerReadAllowed = true };
                var service = this.ServiceProvider.MapToPersitableService(containerClassType);

                if (service == null)
                {
                    return null;
                }

                var containerPartition = partition;

                if (!EngineeringModelContainmentClassType.ClassKindArray.Contains((ClassKind) Enum.Parse(typeof(ClassKind), containerClassType)))
                {
                    containerPartition = partition.Replace("EngineeringModel", "Iteration");
                }

                var possibleContainers = (await service.GetShallowAsync(transaction, containerPartition, null, securityContext)).ToList();

                if (possibleContainers.Count == 0)
                {
                    return null;
                }

                var addContainer = GetContainerFromPossibleContainers(updatedThing, possibleContainers, containerPropertyName);

                return addContainer;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "operation failed");
            }

            return null;
        }

        /// <summary>
        /// Tries to get the <paramref name="deletedThing"/>'s container.
        /// </summary>
        /// <param name="deletedThing">
        /// The updated <see cref="Thing"/>
        /// </param>
        /// <param name="changedThings">
        /// the changed <see cref="Thing"/>s
        /// </param>
        /// <returns>
        /// <paramref name="deletedThing"/>'s container <see cref="Thing"/> if found, otherwise null
        /// </returns>
        private Thing GetDeletedThingContainer(Thing deletedThing, IReadOnlyList<Thing> changedThings)
        {
            try
            {
                var containerClassType = ContainerPropertyHelper.ContainerClassName(deletedThing.ClassKind);

                if (containerClassType == deletedThing.ClassKind.ToString())
                {
                    return null;
                }

                var containerPropertyName = ContainerPropertyHelper.ContainerPropertyName(deletedThing.ClassKind);

                var possibleContainers =
                    this.OperationProcessor.OperationOriginalThingCache.Where(x => x.ClassKind.ToString() == containerClassType).ToList();

                if (possibleContainers.Count == 0)
                {
                    foreach (var possibleContainerThing in changedThings)
                    {
                        if (possibleContainerThing.GetType().Name == containerClassType)
                        {
                            possibleContainers.Add(possibleContainerThing);
                        }
                        else if (possibleContainerThing.GetType().QueryBaseType()?.Name == containerClassType)
                        {
                            possibleContainers.Add(possibleContainerThing);
                        }
                    }
                }

                var addContainer = GetContainerFromPossibleContainers(deletedThing, possibleContainers, containerPropertyName);

                return addContainer;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "operation failed");
            }

            return null;
        }

        /// <summary>
        /// Tries to get the <paramref name="updatedThing"/>'s container from the <paramref name="possibleContainers"/> parameter.
        /// </summary>
        /// <param name="updatedThing">
        /// The updated <see cref="Thing"/>
        /// </param>
        /// <param name="possibleContainers">
        /// The <see cref="List{T}"/> of type <see cref="Thing"/> that holds all possible containers.
        /// </param>
        /// <param name="containerPropertyName">
        /// The name of the containers property where to search for a reference to <paramref name="updatedThing"/>
        /// </param>
        /// <returns>
        /// <paramref name="updatedThing"/>'s container <see cref="Thing"/> if found, otherwise null
        /// </returns>
        private static Thing GetContainerFromPossibleContainers(Thing updatedThing, List<Thing> possibleContainers, string containerPropertyName)
        {
            var propInfo =
                possibleContainers
                    .FirstOrDefault()?
                    .GetType()
                    .GetProperties()
                    .FirstOrDefault(x => string.Equals(x.Name, containerPropertyName, StringComparison.CurrentCultureIgnoreCase));

            if (propInfo == null)
            {
                return null;
            }

            foreach (var container in possibleContainers)
            {
                if (propInfo.PropertyType == typeof(List<Guid>))
                {
                    if (!((propInfo.GetValue(container) as List<Guid>)?.Contains(updatedThing.Iid) ?? false))
                    {
                        continue;
                    }

                    return container;
                }

                if (propInfo.PropertyType == typeof(Guid))
                {
                    if (!((propInfo.GetValue(container) as Guid?)?.Equals(updatedThing.Iid) ?? false))
                    {
                        continue;
                    }

                    return container;
                }
            }

            return null;
        }

        /// <summary>
        /// Searches a <see cref="IReadOnlyList{T}"/> of type <see cref="Thing"/> for containment of a <see cref="Thing"/>
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> to search for
        /// </param>
        /// <param name="containerThings">
        /// The <see cref="IReadOnlyList{T}"/> of type <see cref="Thing"/> to search in
        /// </param>
        /// <returns>
        /// The <paramref name="thing"/>'s container <see cref="Thing"/>if found, otherwise null.
        /// </returns>
        private static Thing GetContainerFromChangedThings(Thing thing, IReadOnlyList<Thing> containerThings)
        {
            var containerClassType = ContainerPropertyHelper.ContainerClassName(thing.ClassKind);
            var containerPropertyName = ContainerPropertyHelper.ContainerPropertyName(thing.ClassKind);

            if (containerClassType == thing.ClassKind.ToString())
            {
                return null;
            }

            var possibleContainers = containerThings.Where(x => x.ClassKind.ToString() == containerClassType).ToList();

            if (possibleContainers.Count == 0)
            {
                foreach (var possibleContainerThing in containerThings)
                {
                    if (possibleContainerThing.GetType().QueryBaseType()?.Name == containerClassType)
                    {
                        possibleContainers.Add(possibleContainerThing);
                    }
                }
            }

            return GetContainerFromPossibleContainers(thing, possibleContainers, containerPropertyName);
        }

        /// <summary>
        /// Gets the description for a specific <see cref="Thing"/> to be added to <see cref="LogEntryChangelogItem.ChangeDescription"/>
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="Thing"/>
        /// </param>
        /// <returns>
        /// The <see cref="Thing"/>s description.
        /// </returns>
        private async Task<string> GetThingDescriptionAsync(NpgsqlTransaction transaction, string partition, Thing thing)
        {
            var basePartition = partition.Replace("EngineeringModel", "").Replace("Iteration", "");
            var iterationPartition = $"Iteration{basePartition}";
            var siteDirectoryPartition = "SiteDirectory";

            var description = thing.ClassKind.ToString();

            if (TryGetName(thing, out var namedThingName))
            {
                description = $"{description} => {namedThingName}";
            }

            if (TryGetShortName(thing, out var shortNamedThingShortName))
            {
                description = $"{description} ({shortNamedThingShortName})";
            }

            var securityContext = new RequestSecurityContext { ContainerReadAllowed = true };

            if (thing is Iteration iteration
                && (await this.IterationSetupService.GetShallowAsync(transaction, siteDirectoryPartition, [iteration.IterationSetup], securityContext)).Single() is IterationSetup iterationSetup)
            {
                description = $"{description} {iterationSetup.IterationNumber}";
            }

            if (thing is Parameter parameter
                && (await this.ParameterTypeService.GetShallowAsync(transaction, siteDirectoryPartition, [parameter.ParameterType], securityContext)).Single() is ParameterType parameterType)
            {
                description = $"{description} {await this.GetThingDescriptionAsync(transaction, partition, parameterType)}";
            }

            if (thing is ParameterOverride parameterOverride
                && (await this.ParameterService.GetShallowAsync(transaction, iterationPartition, [parameterOverride.Parameter], securityContext)).Single() is Parameter parameterOverrideParameter)
            {
                description = $"ParameterOverride: {await this.GetThingDescriptionAsync(transaction, partition, parameterOverrideParameter)}";
            }

            if (thing is ParameterSubscription parameterSubscription
                && (await this.ParameterOrOverrideBaseService
                    .GetShallowAsync(transaction, iterationPartition, null, securityContext))
                    .Cast<ParameterOrOverrideBase>()
                    .SingleOrDefault(x => x.ParameterSubscription.Contains(parameterSubscription.Iid)) is { } parameterOrOverride)
            {
                description = $"ParameterSubscription => {await this.GetThingDescriptionAsync(transaction, partition, parameterOrOverride)}";
            }

            return description;
        }

        /// <summary>
        /// Gets custom descriptions for specific <see cref="Thing"/>s to be added to <see cref="LogEntryChangelogItem.ChangeDescription"/>
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="thing">The specific <see cref="Thing"/></param>
        /// <returns>A string containing the custom descriptions</returns>
        private async Task<string> GetCustomDescriptionsAsync(NpgsqlTransaction transaction, string partition, Thing thing)
        {
            var basePartition = partition.Replace("EngineeringModel", "").Replace("Iteration", "");
            var iterationPartition = $"Iteration{basePartition}";

            var securityContext = new RequestSecurityContext { ContainerReadAllowed = true };

            var stringBuilder = new StringBuilder();

            switch (thing)
            {
                case ParameterValueSet parameterValueSet:
                {
                    if (parameterValueSet.ActualOption.HasValue)
                    {
                        await this.TryAddOptionLineAsync(transaction, partition, parameterValueSet.ActualOption.Value, securityContext, stringBuilder);
                    }

                    if (parameterValueSet.ActualState.HasValue)
                    {
                        await this.TryAddStateLineAsync(transaction, partition, parameterValueSet.ActualState.Value, securityContext, stringBuilder);
                    }

                    break;
                }
                case ParameterOverrideValueSet parameterOverrideValueSet:
                {
                    if ((await this.ParameterValueSetService.GetShallowAsync(transaction, iterationPartition, [parameterOverrideValueSet.ParameterValueSet], securityContext)).Single() is ParameterValueSet parameterOverrideValueSetParameterValueSet)
                    {
                        var description = await this.GetCustomDescriptionsAsync(transaction, partition, parameterOverrideValueSetParameterValueSet);

                        if (!string.IsNullOrWhiteSpace(description))
                        {
                            stringBuilder.Append(description);
                        }
                    }

                    break;
                }
                case ParameterSubscriptionValueSet parameterSubscriptionValueSet:
                {
                    if ((await this.ParameterValueSetBaseService.GetShallowAsync(transaction, iterationPartition, [parameterSubscriptionValueSet.SubscribedValueSet], securityContext)).Single() is ParameterValueSetBase parameterSubscriptionValueSetParameterValueSetBase)
                    {
                        var description = await this.GetCustomDescriptionsAsync(transaction, partition, parameterSubscriptionValueSetParameterValueSetBase);

                        if (!string.IsNullOrWhiteSpace(description))
                        {
                            stringBuilder.Append(description);
                        }
                    }

                    break;
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Adds a <see cref="Guid"/> to an <see cref="ICollection{T}"/> of type <see cref="Guid"/> if it doesn't already exist.
        /// </summary>
        /// <param name="guids">The <see cref="ICollection{T}"/> of type <see cref="Guid"/></param>
        /// <param name="newGuid">The <see cref="Guid"/></param>
        private static void AddIfNotExists(ICollection<Guid> guids, Guid newGuid)
        {
            if (!guids.Contains(newGuid))
            {
                guids.Add(newGuid);
            }
        }

        /// <summary>
        /// Tries to add a descriptive line to a <see cref="StringBuilder"/> for a specific <see cref="ActualFiniteState"/>.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="stateIid">The <see cref="ActualFiniteState.Iid"/> of the <see cref="ActualFiniteState"/></param>
        /// <param name="securityContext">The <see cref="ISecurityContext"/></param>
        /// <param name="stringBuilder">The <see cref="StringBuilder"/></param>
        private async Task TryAddStateLineAsync(NpgsqlTransaction transaction, string partition, Guid stateIid, ISecurityContext securityContext, StringBuilder stringBuilder)
        {
            var basePartition = partition.Replace("EngineeringModel", "").Replace("Iteration", "");
            var iterationPartition = $"Iteration{basePartition}";

            if ((await this.ActualFiniteStateService.GetShallowAsync(transaction, iterationPartition, [stateIid], securityContext)).Single() is not ActualFiniteState actualState)
            {
                return;
            }

            var possibleFiniteStates = 
                (await this.PossibleFiniteStateService.GetShallowAsync(transaction, iterationPartition, actualState.PossibleState, securityContext))
                    .Cast<PossibleFiniteState>()
                    .ToList(); 

            if (possibleFiniteStates.Count == 0)
            {
                return;
            }

            var possibleFiniteStateString = string.Join("->", possibleFiniteStates.Select(x => x.ShortName));
            stringBuilder.AppendLine($"* ActualFiniteState: {possibleFiniteStateString}");
        }

        /// <summary>
        /// Tries to add a descriptive line to a <see cref="StringBuilder"/> for a specific <see cref="Option"/>.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="optionIid">The <see cref="Option.Iid"/> of the <see cref="Option"/></param>
        /// <param name="securityContext">The <see cref="ISecurityContext"/></param>
        /// <param name="stringBuilder">The <see cref="StringBuilder"/></param>
        private async Task TryAddOptionLineAsync(NpgsqlTransaction transaction, string partition, Guid optionIid, ISecurityContext securityContext, StringBuilder stringBuilder)
        {
            var basePartition = partition.Replace("EngineeringModel", "").Replace("Iteration", "");
            var iterationPartition = $"Iteration{basePartition}";

            if ((await this.OptionService.GetShallowAsync(transaction, iterationPartition, [optionIid], securityContext)).Single() is Option actualOption)
            {
                stringBuilder.AppendLine($"* Option: {actualOption.Name} ({actualOption.ShortName})");
            }
        }

        /// <summary>
        /// Checks if creating a <see cref="LogEntryChangelogItem"/> is allowed
        /// </summary>
        /// <param name="classKind">
        /// The <see cref="ClassKind"/> of the hanged <see cref="Thing"/>
        /// </param>
        /// <returns>
        /// True if creation is allowed, otherwise false.
        /// </returns>
        private static bool IsAddLogEntryChangeLogItemAllowed(ClassKind classKind)
        {
            var relevantClassKinds = new List<ClassKind>
            {
                ClassKind.ElementDefinition,
                ClassKind.ElementUsage,
                ClassKind.Parameter,
                ClassKind.ParameterOverride,
                ClassKind.ParameterSubscription,
                ClassKind.ParameterValueSet,
                ClassKind.ParameterOverrideValueSet,
                ClassKind.ParameterSubscriptionValueSet
            };

            return relevantClassKinds.Contains(classKind);
        }

        /// <summary>
        /// Try to get the name of a <see cref="Thing"/> and return if it has a readable <see cref="INamedThing.Name"/> property.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="name">The <see cref="INamedThing.Name"/></param>
        /// <returns>True, if the <see cref="Thing"/> has a readable <see cref="INamedThing.Name"/> property, otherwise false</returns>
        private static bool TryGetName(Thing thing, out string name)
        {
            name = null;

            if (thing is not INamedThing namedThing)
            {
                return false;
            }

            try
            {
                name = namedThing.Name;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Try to get the short name of a <see cref="Thing"/> and return if it has a readable <see cref="IShortNamedThing.ShortName"/> property.
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="shortName">The <see cref="IShortNamedThing.ShortName"/></param>
        /// <returns>True, if the <see cref="Thing"/> has a readable <see cref="IShortNamedThing.ShortName"/> property, otherwise false</returns>
        private static bool TryGetShortName(Thing thing, out string shortName)
        {
            shortName = null;

            if (thing is not IShortNamedThing shortNamedThing)
            {
                return false;
            }

            try
            {
                shortName = shortNamedThing.ShortName;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
