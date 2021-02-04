// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeLogService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft.
//
//    This file is part of CDP4 Web Services Community Edition. 
//    The CDP4 Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
//
//    The CDP4 Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4 Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.ChangeLog
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using CDP4Common;
    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.Helpers;
    using CDP4Common.Polyfills;

    using CDP4Orm.Dao;
    using CDP4Orm.Dao.Resolve;

    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Operations;

    using NLog;

    using Npgsql;

    using INamedThing = CDP4Common.DTO.INamedThing;
    using IServiceProvider = CDP4WebServices.API.Services.IServiceProvider;
    using IShortNamedThing = CDP4Common.DTO.IShortNamedThing;
    using LogEntryChangelogItem = CDP4Common.DTO.LogEntryChangelogItem;
    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// Implements the injectable <see cref="IChangeLogService"/> interface used to append change log data to the database.
    /// </summary>
    public class ChangeLogService : IChangeLogService
    {
        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Property names that are excluded from <see cref="CDP4Common.DTO.LogEntryChangelogItem.ChangeDescription"/> text.
        /// </summary>
        private readonly string[] excludedPropertyNames = { nameof(Thing.Iid), nameof(Thing.ClassKind), nameof(Thing.ModifiedOn) };

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
        /// Appends changelog data based on the changed <see cref="CDP4Common.DTO.Thing"/>s
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
        public bool TryAppendModelChangeLogData(NpgsqlTransaction transaction, string partition, Guid actor, int transactionRevision, CdpPostOperation operation, IReadOnlyList<Thing> things)
        {
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
                        var modelLogOperationData = new CdpPostOperation();

                        var newModelLogEntry = new ModelLogEntry(Guid.NewGuid(), 0)
                        {
                            Level = LogLevelKind.USER,
                            Author = actor,
                            LanguageCode = "en-GB",
                            Content = "-"
                        };

                        engineeringModel.LogEntry.Add(newModelLogEntry.Iid);

                        var engineeringModelClasslessDTO = ClasslessDtoFactory
                            .FromThing(this.RequestUtils.MetaInfoProvider,
                                engineeringModel);

                        engineeringModelClasslessDTO.Add(nameof(EngineeringModel.LogEntry), new[] { newModelLogEntry.Iid });

                        modelLogOperationData.Update.Add(engineeringModelClasslessDTO);
                        modelLogOperationData.Create.Add(newModelLogEntry);
                        this.OperationProcessor.Process(modelLogOperationData, transaction, partition, null);

                        modelLogEntry = newModelLogEntry;
                    }
                    else
                    {
                        modelLogEntry = modelLogEntries.First();
                    }

                    var newLogEntryChangelogItems = new List<LogEntryChangelogItem>();

                    foreach (var changedThing in changedThings.Where(x => x.ClassKind != ClassKind.ModelLogEntry))
                    {
                        var newLogEntryChangelogItem = this.CreateAddOrUpdateLogEntryChangelogItem(transaction, partition, changedThing, modelLogEntry, operation, changedThings);

                        if (newLogEntryChangelogItem == null)
                        {
                            continue;
                        }

                        newLogEntryChangelogItems.Add(newLogEntryChangelogItem);

                        if (changedThing is IOwnedThing ownedThing && !this.DataModelUtils.IsDerived(changedThing.ClassKind.ToString(), nameof(IOwnedThing.Owner)))
                        {
                            this.AddIfNotExists(modelLogEntry.AffectedDomainIid, ownedThing.Owner);
                            this.AddIfNotExists(newLogEntryChangelogItem.AffectedReferenceIid, ownedThing.Owner);
                        }

                        if (changedThing is ICategorizableThing categorizableThing)
                        {
                            foreach (var categoryIid in categorizableThing.Category)
                            {
                                this.AddIfNotExists(modelLogEntry.AffectedItemIid, categoryIid);
                                this.AddIfNotExists(newLogEntryChangelogItem.AffectedReferenceIid, categoryIid);
                            }
                        }
                    }

                    foreach (var deleteInfo in operation.Delete)
                    {
                        var newLogEntryChangelogItem =
                            this.CreateDeleteLogEntryChangelogItems(transaction, partition, operation, deleteInfo, modelLogEntry, changedThings);

                        if (newLogEntryChangelogItem != null)
                        {
                            newLogEntryChangelogItems.Add(newLogEntryChangelogItem);
                        }
                    }

                    var operationData = new CdpPostOperation();

                    if (newLogEntryChangelogItems.Any())
                    {
                        operationData.Create.AddRange(newLogEntryChangelogItems);

                        modelLogEntry.LogEntryChangelogItem.AddRange(newLogEntryChangelogItems.Select(x => x.Iid));

                        var modelLogEntryClasslessDTO =
                            ClasslessDtoFactory
                                .FromThing(this.RequestUtils.MetaInfoProvider,
                                    modelLogEntry,
                                    new[] { nameof(ModelLogEntry.LogEntryChangelogItem), nameof(ModelLogEntry.AffectedItemIid), nameof(ModelLogEntry.AffectedDomainIid) });

                        operationData.Update.Add(modelLogEntryClasslessDTO);

                        this.OperationProcessor.Process(operationData, transaction, partition, null);

                        result = true;
                    }
                }
            }
            finally
            {
                if (!isFullAccessEnabled)
                {
                    this.TransactionManager.SetFullAccessState(false);
                }
            }

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
        private LogEntryChangelogItem CreateAddOrUpdateLogEntryChangelogItem(NpgsqlTransaction transaction, string partition, Thing changedThing, ModelLogEntry modelLogEntry, CdpPostOperation operation, IReadOnlyList<Thing> changedThings)
        {
            if (!this.IsAddLogEntryChangeLogItemAllowed(changedThing.ClassKind))
            {
                return null;
            }

            var logEntryChangeLogItem = new LogEntryChangelogItem(Guid.NewGuid(), 0);
            this.AddIfNotExists(modelLogEntry.AffectedItemIid, changedThing.Iid);

            if (operation.Create.SingleOrDefault(x => x.Iid == changedThing.Iid) is { })
            {
                this.SetCreateLogEntryChangeLogItem(transaction, partition, changedThing, modelLogEntry, logEntryChangeLogItem, changedThings);
                return logEntryChangeLogItem;
            }
            else if (operation.Update.SingleOrDefault(x => x[nameof(Thing.Iid)] as Guid? == changedThing.Iid) is { } updateOperation)
            {
                this.SetUpdateLogEntryChangeLogItem(transaction, partition, changedThing, modelLogEntry, logEntryChangeLogItem, updateOperation);
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
        /// <param name="operation">
        /// The <see cref="CdpPostOperation"/>
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
        private LogEntryChangelogItem CreateDeleteLogEntryChangelogItems(NpgsqlTransaction transaction, string partition, CdpPostOperation operation, ClasslessDTO deleteInfo, ModelLogEntry modelLogEntry, IReadOnlyList<Thing> changedThings)
        {
            if (Enum.TryParse<ClassKind>(deleteInfo[nameof(Thing.ClassKind)].ToString(), out var classKind) && this.IsAddLogEntryChangeLogItemAllowed(classKind))
            {
                var newLogEntryChangelogItem = new LogEntryChangelogItem(Guid.NewGuid(), 0);
                this.AddIfNotExists(modelLogEntry.AffectedItemIid, (Guid) deleteInfo[nameof(Thing.Iid)]);

                var dtoInfo = deleteInfo.GetInfoPlaceholder();

                if (this.OperationProcessor.OperationThingCache.TryGetValue(dtoInfo, out var resolvedInfo))
                {
                    this.SetDeleteLogEntryChangeLogItem(transaction, partition, resolvedInfo.Thing, modelLogEntry, newLogEntryChangelogItem, deleteInfo, changedThings);
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
        private void SetCreateLogEntryChangeLogItem(NpgsqlTransaction transaction, string partition, Thing createdThing, ModelLogEntry modelLogEntry, LogEntryChangelogItem logEntryChangeLogItem, IReadOnlyList<Thing> changedThings)
        {
            logEntryChangeLogItem.ChangelogKind = LogEntryChangelogItemKind.ADD;
            logEntryChangeLogItem.AffectedItemIid = createdThing.Iid;

            if (createdThing is IOwnedThing ownedThing && !this.DataModelUtils.IsDerived(createdThing.ClassKind.ToString(), nameof(IOwnedThing.Owner)))
            {
                this.AddIfNotExists(modelLogEntry.AffectedDomainIid, ownedThing.Owner);
                this.AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, ownedThing.Owner);
            }

            if (createdThing is ICategorizableThing categorizableThing)
            {
                foreach (var categoryIid in categorizableThing.Category)
                {
                    this.AddIfNotExists(modelLogEntry.AffectedItemIid, categoryIid);
                    this.AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, categoryIid);
                }
            }

            var stringBuilder = new StringBuilder();

            var containerThing = this.GetContainerFromChangedThings(createdThing, changedThings);

            if (containerThing != null)
            {
                this.AddIfNotExists(modelLogEntry.AffectedItemIid, containerThing.Iid);
                this.AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, containerThing.Iid);

                stringBuilder.AppendLine($"* {this.GetThingDescription(transaction, partition, containerThing)}");
            }

            var customData = this.GetCustomAffectedThingsData(transaction, partition, createdThing, null);

            foreach (var customAffectedItemId in customData.AffectedItemIds)
            {
                this.AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, customAffectedItemId);
                this.AddIfNotExists(modelLogEntry.AffectedItemIid, customAffectedItemId);
            }

            foreach (var customAffectedDomainId in customData.AffectedDomainIds)
            {
                this.AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, customAffectedDomainId);
                this.AddIfNotExists(modelLogEntry.AffectedDomainIid, customAffectedDomainId);
            }

            foreach (var customHeaderRow in customData.ExtraChangeDescriptions)
            {
                stringBuilder.AppendLine(customHeaderRow);
            }

            stringBuilder.AppendLine($"* {this.GetThingDescription(transaction, partition, createdThing)}");

            var customDescriptions = this.GetCustomDescriptions(transaction, partition, createdThing);

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
        private void SetUpdateLogEntryChangeLogItem(NpgsqlTransaction transaction, string partition, Thing updatedThing, ModelLogEntry modelLogEntry, LogEntryChangelogItem logEntryChangeLogItem, ClasslessDTO updateOperation)
        {
            logEntryChangeLogItem.ChangelogKind = LogEntryChangelogItemKind.UPDATE;
            logEntryChangeLogItem.AffectedItemIid = updatedThing.Iid;

            if (updatedThing is IOwnedThing ownedThing && !this.DataModelUtils.IsDerived(updatedThing.ClassKind.ToString(), nameof(IOwnedThing.Owner)))
            {
                this.AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, ownedThing.Owner);
                this.AddIfNotExists(modelLogEntry.AffectedDomainIid, ownedThing.Owner);
            }

            if (updatedThing is ICategorizableThing categorizableThing)
            {
                foreach (var categoryIid in categorizableThing.Category)
                {
                    this.AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, categoryIid);
                    this.AddIfNotExists(modelLogEntry.AffectedItemIid, categoryIid);
                }
            }

            var customData = this.GetCustomAffectedThingsData(transaction, partition, updatedThing, updateOperation);

            foreach (var customAffectedItemId in customData.AffectedItemIds)
            {
                this.AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, customAffectedItemId);
                this.AddIfNotExists(modelLogEntry.AffectedItemIid, customAffectedItemId);
            }

            foreach (var customAffectedDomainId in customData.AffectedDomainIds)
            {
                this.AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, customAffectedDomainId);
                this.AddIfNotExists(modelLogEntry.AffectedDomainIid, customAffectedDomainId);
            }

            var stringBuilder = new StringBuilder();

            foreach (var customHeaderRow in customData.ExtraChangeDescriptions)
            {
                stringBuilder.AppendLine(customHeaderRow);
            }

            var containerThing = this.GetContainer(transaction, partition, updatedThing);

            if (containerThing != null)
            {
                this.AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, containerThing.Iid);
                this.AddIfNotExists(modelLogEntry.AffectedItemIid, containerThing.Iid);
                stringBuilder.AppendLine($"* {this.GetThingDescription(transaction, partition, containerThing)}");
            }

            stringBuilder.AppendLine($"* {this.GetThingDescription(transaction, partition, updatedThing)}");

            var extraDescriptions = this.GetCustomDescriptions(transaction, partition, updatedThing);

            if (!string.IsNullOrEmpty(extraDescriptions))
            {
                stringBuilder.AppendLine(extraDescriptions);
            }

            var changedValues =
                updateOperation
                    .Where(x => !this.excludedPropertyNames.Contains(x.Key))
                    .Select(x => $" - {x.Key} = {(x.Value is IEnumerable<Guid> enumerable ? $"Added: {string.Join(",", enumerable)}" : x.Value)}");

            stringBuilder.AppendLine(string.Join("\n", changedValues));

            logEntryChangeLogItem.ChangeDescription = stringBuilder.ToString();
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
        private void SetDeleteLogEntryChangeLogItem(NpgsqlTransaction transaction, string partition, Thing deletedThing, ModelLogEntry modelLogEntry, LogEntryChangelogItem logEntryChangeLogItem, ClasslessDTO deleteOperation, IReadOnlyList<Thing> changedThings)
        {
            logEntryChangeLogItem.ChangelogKind = LogEntryChangelogItemKind.DELETE;
            logEntryChangeLogItem.AffectedItemIid = deletedThing.Iid;

            if (deletedThing is IOwnedThing ownedThing && !this.DataModelUtils.IsDerived(deletedThing.ClassKind.ToString(), nameof(IOwnedThing.Owner)))
            {
                this.AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, ownedThing.Owner);
                this.AddIfNotExists(modelLogEntry.AffectedDomainIid, ownedThing.Owner);
            }

            if (deletedThing is ICategorizableThing categorizableThing)
            {
                foreach (var categoryIid in categorizableThing.Category)
                {
                    this.AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, categoryIid);
                    this.AddIfNotExists(modelLogEntry.AffectedItemIid, categoryIid);
                }
            }

            var customData = this.GetCustomAffectedThingsData(transaction, partition, deletedThing, deleteOperation, deletedThing);

            foreach (var customAffectedItemId in customData.AffectedItemIds)
            {
                this.AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, customAffectedItemId);
                this.AddIfNotExists(modelLogEntry.AffectedItemIid, customAffectedItemId);
            }

            foreach (var customAffectedDomainId in customData.AffectedDomainIds)
            {
                this.AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, customAffectedDomainId);
                this.AddIfNotExists(modelLogEntry.AffectedDomainIid, customAffectedDomainId);
            }

            var stringBuilder = new StringBuilder();

            foreach (var customHeaderRow in customData.ExtraChangeDescriptions)
            {
                stringBuilder.AppendLine(customHeaderRow);
            }

            var containerThing = this.GetDeletedThingContainer(deletedThing, changedThings);

            if (containerThing != null)
            {
                this.AddIfNotExists(logEntryChangeLogItem.AffectedReferenceIid, containerThing.Iid);
                this.AddIfNotExists(modelLogEntry.AffectedItemIid, containerThing.Iid);
                stringBuilder.AppendLine($"* {this.GetThingDescription(transaction, partition, containerThing)}");
            }

            stringBuilder.AppendLine($"* {this.GetThingDescription(transaction, partition, deletedThing)}");

            var extraDescriptions = this.GetCustomDescriptions(transaction, partition, deletedThing);

            if (!string.IsNullOrEmpty(extraDescriptions))
            {
                stringBuilder.AppendLine(extraDescriptions);
            }

            logEntryChangeLogItem.ChangeDescription = stringBuilder.ToString();
        }

        /// <summary>
        /// Gets custom affected <see cref="Thing"/>s data as a <see cref="CustomAffectedThingsData"/> object.
        /// <see cref="CustomAffectedThingsData"/> containes AffectedItems, AffectedDomains and extra ChangeDescription text
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
        /// The <see cref="CustomAffectedThingsData"/>
        /// </returns>
        private CustomAffectedThingsData GetCustomAffectedThingsData(NpgsqlTransaction transaction, string partition, Thing rootThing, ClasslessDTO updateOperation, Thing thing = null)
        {
            thing ??= rootThing;
            var customData = new CustomAffectedThingsData();
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
                    this.AddIfNotExists(customData.AffectedItemIds, parameter.ParameterType);

                    if (this.ParameterTypeService.GetShallow(transaction, siteDirectoryPartition, new[] { parameter.ParameterType }, securityContext).Single() is ParameterType parameterType)
                    {
                        foreach (var category in parameterType.Category)
                        {
                            this.AddIfNotExists(customData.AffectedItemIds, category);
                        }
                    }

                    if (!(rootThing is ParameterSubscription) && !(rootThing is ParameterSubscriptionValueSet) &&!(rootThing is ParameterOverride) && !(rootThing is ParameterOverrideValueSet))
                    {
                        if (updateOperation != null &&
                            (updateOperation.Keys.Count > 3 || !updateOperation.Keys.Contains(nameof(Parameter.ParameterSubscription))))
                        {
                            foreach (var parameterSubscription in parameter.ParameterSubscription)
                            {
                                this.AddIfNotExists(customData.AffectedItemIds, parameterSubscription);
                            }
                        }
                    }

                    if (this.GetContainer(transaction, engineeringModelPartition, parameter) is ElementDefinition elementDefinition)
                    {
                        var newCustomData = this.GetCustomAffectedThingsData(transaction, partition, rootThing, updateOperation, elementDefinition);
                        customData.AddFrom(newCustomData);
                    }

                    if (getContainerReferences)
                    {
                        this.AddIfNotExists(customData.AffectedDomainIds, parameter.Owner);
                        this.AddIfNotExists(customData.AffectedItemIds, parameter.Iid);
                    }

                    break;
                }
                case ElementUsage elementUsage:
                {
                    if (this.GetContainer(transaction, engineeringModelPartition, elementUsage) is ElementDefinition elementDefinition)
                    {
                        var newCustomData = this.GetCustomAffectedThingsData(transaction, partition, rootThing, updateOperation, elementDefinition);
                        customData.AddFrom(newCustomData);
                    }

                    if (getContainerReferences)
                    {
                        foreach (var category in elementUsage.Category)
                        {
                            this.AddIfNotExists(customData.AffectedItemIds, category);
                        }

                        this.AddIfNotExists(customData.AffectedDomainIds, elementUsage.Owner);
                        this.AddIfNotExists(customData.AffectedItemIds, elementUsage.Iid);
                    }

                    break;
                }
                case ElementDefinition elementDefinition:
                {
                    if (getContainerReferences)
                    {
                        foreach (var category in elementDefinition.Category)
                        {
                            this.AddIfNotExists(customData.AffectedItemIds, category);
                        }

                        this.AddIfNotExists(customData.AffectedDomainIds, elementDefinition.Owner);
                        this.AddIfNotExists(customData.AffectedItemIds, elementDefinition.Iid);
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
                                this.AddIfNotExists(customData.AffectedItemIds, parameterSubscription);
                            }
                        }
                    }

                    var parameterOverrideParameter = this.ParameterService.GetShallow(transaction, iterationPartition, new[] { parameterOverride.Parameter }, securityContext)
                        .Cast<Parameter>()
                        .SingleOrDefault();

                    if (parameterOverrideParameter != null)
                    {
                        var newCustomData = this.GetCustomAffectedThingsData(transaction, partition, rootThing, updateOperation, parameterOverrideParameter);
                        customData.AddFrom(newCustomData);
                    }

                    if (this.GetContainer(transaction, engineeringModelPartition, parameterOverride) is ElementUsage elementUsage)
                    {
                        var newCustomData = this.GetCustomAffectedThingsData(transaction, partition, rootThing, updateOperation, elementUsage);
                        customData.AddFrom(newCustomData);
                    }

                    if (getContainerReferences)
                    {
                        this.AddIfNotExists(customData.AffectedDomainIds, parameterOverride.Owner);
                        this.AddIfNotExists(customData.AffectedItemIds, parameterOverride.Iid);
                    }

                    break;
                }
                case ParameterValueSet parameterValueSet:
                {
                    var parameterValueSetParameters = this.ParameterService.GetShallow(transaction, iterationPartition, null, securityContext)
                        .Cast<Parameter>()
                        .ToList();

                    var parameterValueSetParameter = parameterValueSetParameters.SingleOrDefault(x => x.ValueSet.Contains(parameterValueSet.Iid));

                    if (parameterValueSetParameter != null)
                    {
                        var newCustomData = this.GetCustomAffectedThingsData(transaction, partition, rootThing, updateOperation, parameterValueSetParameter);
                        customData.AddFrom(newCustomData);
                    }

                    break;
                }
                case ParameterOverrideValueSet parameterOverrideValueSet:
                {
                    var parameterOverrideValueSetParameterOverrides = this.ParameterOverrideService.GetShallow(transaction, iterationPartition, null, securityContext)
                        .Cast<ParameterOverride>()
                        .ToList();

                    var parameterOverrideValueSetParameterOverride = parameterOverrideValueSetParameterOverrides.SingleOrDefault(x => x.ValueSet.Contains(parameterOverrideValueSet.Iid));

                    if (parameterOverrideValueSetParameterOverride != null)
                    {
                        var newCustomData = this.GetCustomAffectedThingsData(transaction, partition, rootThing, updateOperation, parameterOverrideValueSetParameterOverride);
                        customData.AddFrom(newCustomData);
                    }

                    break;
                }
                case ParameterSubscription parameterSubscription:
                {
                    var parameterOrOverrideBases = this.ParameterOrOverrideBaseService.GetShallow(transaction, iterationPartition, null, securityContext)
                        .Cast<ParameterOrOverrideBase>()
                        .ToList();

                    var parameterOrOverrideBase = parameterOrOverrideBases.SingleOrDefault(x => x.ParameterSubscription.Contains(parameterSubscription.Iid));

                    if (parameterOrOverrideBase != null)
                    {
                        var newCustomData = this.GetCustomAffectedThingsData(transaction, partition, rootThing, updateOperation, parameterOrOverrideBase);
                        customData.AddFrom(newCustomData);
                    }

                    if (getContainerReferences)
                    {
                        this.AddIfNotExists(customData.AffectedDomainIds, parameterSubscription.Owner);
                        this.AddIfNotExists(customData.AffectedItemIds, parameterSubscription.Iid);
                    }

                    break;
                }
                case ParameterSubscriptionValueSet parameterSubscriptionValueSet:
                {
                    var parameterSubscriptionValueSetParameterSubscriptions = this.ParameterSubscriptionService.GetShallow(transaction, iterationPartition, null, securityContext)
                        .Cast<ParameterSubscription>()
                        .ToList();

                    var parameterSubscriptionValueSetParameterSubscription = parameterSubscriptionValueSetParameterSubscriptions.SingleOrDefault(x => x.ValueSet.Contains(parameterSubscriptionValueSet.Iid));

                    if (parameterSubscriptionValueSetParameterSubscription != null)
                    {
                        var newCustomData = this.GetCustomAffectedThingsData(transaction, partition, rootThing, updateOperation, parameterSubscriptionValueSetParameterSubscription);
                        customData.AddFrom(newCustomData);
                    }

                    break;
                }
            }

            return customData;
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
        private Thing GetContainer(NpgsqlTransaction transaction, string partition, Thing updatedThing)
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

                var possibleContainers = service.GetShallow(transaction, containerPartition, null, securityContext).ToList();

                if (!possibleContainers.Any())
                {
                    return null;
                }

                var addContainer = this.GetContainerFromPossibleContainers(updatedThing, possibleContainers, containerPropertyName);

                return addContainer;
            }
            catch (Exception ex)
            {
                // Ignore
                Logger.Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Tries to get the <paramref name="deletedThing"/>'s container.
        /// </summary>
        /// <param name="deletedThing">
        /// The updated <see cref="Thing"/>
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
                    this.OperationProcessor.OperationThingCache.Where(x => x.Key is ContainerInfo && x.Key.TypeName == containerClassType)
                        .Select(x => x.Value.Thing)
                        .ToList();

                if (!possibleContainers.Any())
                {
                    foreach (var possibleContainerThing in changedThings)
                    {
                        if (possibleContainerThing.GetType().QueryBaseType()?.Name == containerClassType)
                        {
                            possibleContainers.Add(possibleContainerThing);
                        }
                    }
                }

                var addContainer = this.GetContainerFromPossibleContainers(deletedThing, possibleContainers, containerPropertyName);

                return addContainer;
            }
            catch (Exception ex)
            {
                // Ignore
                Logger.Error(ex);
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
        private Thing GetContainerFromPossibleContainers(Thing updatedThing, List<Thing> possibleContainers, string containerPropertyName)
        {
            var propInfo =
                possibleContainers
                    .FirstOrDefault()?
                    .GetType()
                    .GetProperties()
                    .FirstOrDefault(x => string.Equals(x.Name, containerPropertyName, StringComparison.CurrentCultureIgnoreCase));

            if (propInfo == null)
            {
                {
                    return null;
                }
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
        private Thing GetContainerFromChangedThings(Thing thing, IReadOnlyList<Thing> containerThings)
        {
            var containerClassType = ContainerPropertyHelper.ContainerClassName(thing.ClassKind);
            var containerPropertyName = ContainerPropertyHelper.ContainerPropertyName(thing.ClassKind);

            if (containerClassType == thing.ClassKind.ToString())
            {
                return null;
            }

            var possibleContainers = containerThings.Where(x => x.ClassKind.ToString() == containerClassType).ToList();

            if (!possibleContainers.Any())
            {
                foreach (var possibleContainerThing in containerThings)
                {
                    if (possibleContainerThing.GetType().QueryBaseType()?.Name == containerClassType)
                    {
                        possibleContainers.Add(possibleContainerThing);
                    }
                }
            }

            return this.GetContainerFromPossibleContainers(thing, possibleContainers, containerPropertyName);
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
        private string GetThingDescription(NpgsqlTransaction transaction, string partition, Thing thing)
        {
            var basePartition = partition.Replace("EngineeringModel", "").Replace("Iteration", "");
            var iterationPartition = $"Iteration{basePartition}";
            var siteDirectoryPartition = "SiteDirectory";

            var description = thing.ClassKind.ToString();

            if (thing is INamedThing namedThing)
            {
                description = $"{description} => {namedThing.Name}";
            }

            if (thing is IShortNamedThing shortNamedThing)
            {
                description = $"{description} ({shortNamedThing.ShortName})";
            }

            var securityContext = new RequestSecurityContext { ContainerReadAllowed = true };

            if (thing is Iteration iteration
                && this.IterationSetupService.GetShallow(transaction, siteDirectoryPartition, new[] { iteration.IterationSetup }, securityContext).Single() is IterationSetup iterationSetup)
            {
                description = $"{description} {iterationSetup.IterationNumber}";
            }

            if (thing is Parameter parameter
                && this.ParameterTypeService.GetShallow(transaction, siteDirectoryPartition, new[] { parameter.ParameterType }, securityContext).Single() is ParameterType parameterType)
            {
                description = $"{description} {this.GetThingDescription(transaction, partition, parameterType)}";
            }

            if (thing is ParameterOverride parameterOverride
                && this.ParameterService.GetShallow(transaction, iterationPartition, new[] { parameterOverride.Parameter }, securityContext).Single() is Parameter parameterOverrideParameter)
            {
                description = $"ParameterOverride: {this.GetThingDescription(transaction, partition, parameterOverrideParameter)}";
            }

            if (thing is ParameterSubscription parameterSubscription
                && this.ParameterOrOverrideBaseService
                    .GetShallow(transaction, iterationPartition, null, securityContext)
                    .Cast<ParameterOrOverrideBase>()
                    .SingleOrDefault(x => x.ParameterSubscription.Contains(parameterSubscription.Iid)) is { } parameterOrOverride)
            {
                description = $"ParameterSubscription => {this.GetThingDescription(transaction, partition, parameterOrOverride)}";
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
        private string GetCustomDescriptions(NpgsqlTransaction transaction, string partition, Thing thing)
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
                        this.TryAddOptionLine(transaction, partition, parameterValueSet.ActualOption.Value, securityContext, stringBuilder);
                    }

                    if (parameterValueSet.ActualState.HasValue)
                    {
                        this.TryAddStateLine(transaction, partition, parameterValueSet.ActualState.Value, securityContext, stringBuilder);
                    }

                    break;
                }
                case ParameterOverrideValueSet parameterOverrideValueSet:
                {
                    if (this.ParameterValueSetService.GetShallow(transaction, iterationPartition, new[] { parameterOverrideValueSet.ParameterValueSet }, securityContext).Single() is ParameterValueSet parameterOverrideValueSetParameterValueSet)
                    {
                        var description = this.GetCustomDescriptions(transaction, partition, parameterOverrideValueSetParameterValueSet);

                        if (!string.IsNullOrWhiteSpace(description))
                        {
                            stringBuilder.Append(description);
                        }
                    }

                    break;
                }
                case ParameterSubscriptionValueSet parameterSubscriptionValueSet:
                {
                    if (this.ParameterValueSetBaseService.GetShallow(transaction, iterationPartition, new[] { parameterSubscriptionValueSet.SubscribedValueSet }, securityContext).Single() is ParameterValueSetBase parameterSubscriptionValueSetParameterValueSetBase)
                    {
                        var description = this.GetCustomDescriptions(transaction, partition, parameterSubscriptionValueSetParameterValueSetBase);

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
        private void AddIfNotExists(ICollection<Guid> guids, Guid newGuid)
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
        private void TryAddStateLine(NpgsqlTransaction transaction, string partition, Guid stateIid, ISecurityContext securityContext, StringBuilder stringBuilder)
        {
            var basePartition = partition.Replace("EngineeringModel", "").Replace("Iteration", "");
            var iterationPartition = $"Iteration{basePartition}";

            if (this.ActualFiniteStateService.GetShallow(transaction, iterationPartition, new[] { stateIid }, securityContext).Single() is not ActualFiniteState actualState)
            {
                return;
            }

            if (this.PossibleFiniteStateService.GetShallow(transaction, iterationPartition, actualState.PossibleState, securityContext).Single() is not PossibleFiniteState possibleFiniteState)
            {
                return;
            }

            var possibleFiniteStateString = string.Join(" ", possibleFiniteState.ShortName);

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
        private void TryAddOptionLine(NpgsqlTransaction transaction, string partition, Guid optionIid, ISecurityContext securityContext, StringBuilder stringBuilder)
        {
            var basePartition = partition.Replace("EngineeringModel", "").Replace("Iteration", "");
            var iterationPartition = $"Iteration{basePartition}";

            if (this.OptionService.GetShallow(transaction, iterationPartition, new[] { optionIid }, securityContext).Single() is Option actualOption)
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
        private bool IsAddLogEntryChangeLogItemAllowed(ClassKind classKind)
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
    }
}
