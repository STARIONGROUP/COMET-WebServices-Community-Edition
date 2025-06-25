﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterSideEffect.cs" company="Starion Group S.A.">
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

namespace CometServer.Services.Operations.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.Exceptions;
    using CDP4Common.Types;

    using CDP4Orm.Dao;

    using CometServer.Exceptions;
    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="ParameterSideEffect"/> Side-Effect class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class ParameterSideEffect : OperationSideEffect<Parameter>
    {
        /// <summary>
        /// Gets or sets the (injected) <see cref="IParameterValueSetService"/>
        /// </summary>
        public IParameterValueSetService ParameterValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IActualFiniteStateListService"/>
        /// </summary>
        public IActualFiniteStateListService ActualFiniteStateListService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IOptionService"/>
        /// </summary>
        public IOptionService OptionService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IIterationService"/>
        /// </summary>
        public IIterationService IterationService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IDefaultValueArrayFactory"/>
        /// </summary>
        public IDefaultValueArrayFactory DefaultValueArrayFactory { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="ICachedReferenceDataService"/>
        /// </summary>
        public ICachedReferenceDataService CachedReferenceDataService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterService"/>
        /// </summary>
        public IParameterService ParameterService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterValueSetFactory"/>
        /// </summary>
        public IParameterValueSetFactory ParameterValueSetFactory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterOverrideValueSetFactory"/>
        /// </summary>
        public IParameterOverrideValueSetFactory ParameterOverrideValueSetFactory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionValueSetFactory"/>
        /// </summary>
        public IParameterSubscriptionValueSetFactory ParameterSubscriptionValueSetFactory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterOverrideService"/>
        /// </summary>
        public IParameterOverrideService ParameterOverrideService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionService"/>
        /// </summary>
        public IParameterSubscriptionService ParameterSubscriptionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionValueSetService"/>
        /// </summary>
        public IParameterSubscriptionValueSetService ParameterSubscriptionValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterOverrideValueSetService"/>
        /// </summary>
        public IParameterOverrideValueSetService ParameterOverrideValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IElementUsageService"/>
        /// </summary>
        public IElementUsageService ElementUsageService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IElementDefinitionService"/>
        /// </summary>
        public IElementDefinitionService ElementDefinitionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IOldParameterContextProvider"/>
        /// </summary>
        public IOldParameterContextProvider OldParameterContextProvider { get; set; }

        /// <summary>
        /// Gets the list of property names that are to be excluded from validation logic.
        /// </summary>
        public override IEnumerable<string> DeferPropertyValidation => ["ValueSet"];

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Parameter"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        /// <param name="rawUpdateInfo">
        /// The raw update info that was serialized from the user posted request.
        /// The <see cref="ClasslessDTO"/> instance only contains values for properties that are to be updated.
        /// It is important to note that this variable is not to be changed likely as it can/will change the operation processor outcome.
        /// </param>
        public override async Task BeforeUpdate(
            Parameter thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            if (rawUpdateInfo.ContainsKey("ParameterType"))
            {
                await this.ValidateParameterTypeUpdate(thing, transaction, securityContext, rawUpdateInfo);
            }
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before a create operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public override async Task<bool> BeforeCreate(
            Parameter thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            this.OrganizationalParticipationResolverService.ValidateCreateOrganizationalParticipation(thing, container, securityContext, transaction, partition);

            await this.ValidateParameterTypeAndScale(thing, transaction, securityContext);

            return true;
        }

        /// <summary>
        /// Execute additional logic after a successful create operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public override async Task AfterCreate(
            Parameter thing,
            Thing container,
            Parameter originalThing,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            await this.CheckDuplicateParameterTypes(container, thing, transaction, partition, securityContext);

            // creates default value-arrays for the parameter
            this.DefaultValueArrayFactory.Load(transaction, securityContext);
            var defaultValueArray = this.DefaultValueArrayFactory.CreateDefaultValueArray(thing.ParameterType);

            var newValueSet = this.ComputeValueSets(thing, null, transaction, partition, securityContext, defaultValueArray);
            await this.WriteValueSet(transaction, partition, thing, newValueSet);
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic after a successful update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public override async Task AfterUpdate(
            Parameter thing,
            Thing container,
            Parameter originalThing,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            var isParameterTypeChanged = thing.ParameterType != originalThing.ParameterType;

            if (isParameterTypeChanged)
            {
                await this.CheckDuplicateParameterTypes(container, thing, transaction, partition, securityContext, true);
            }

            var isOptionDependencyChanged = thing.IsOptionDependent != originalThing.IsOptionDependent;
            var isStateDependencyChanged = thing.StateDependence != originalThing.StateDependence;
            var isOwnerChanged = thing.Owner != originalThing.Owner;

            // do nothing if the option and state dependency has not changed
            if (!isOwnerChanged && !isOptionDependencyChanged && !isStateDependencyChanged)
            {
                return;
            }

            // creates default value-arrays for the parameter
            this.DefaultValueArrayFactory.Load(transaction, securityContext);
            var defaultValueArray = this.DefaultValueArrayFactory.CreateDefaultValueArray(thing.ParameterType);

            var newValueSet = this.ComputeValueSets(thing, originalThing, transaction, partition, securityContext, defaultValueArray)
                .ToList();

            await this.WriteValueSet(transaction, partition, thing, newValueSet);

            var newOldValueSetMap = new Dictionary<ParameterValueSet, ParameterValueSet>();

            foreach (var parameterValueSet in newValueSet)
            {
                var oldValueSet = this.OldParameterContextProvider.GetsourceValueSet(parameterValueSet.ActualOption, parameterValueSet.ActualState);
                newOldValueSetMap.Add(parameterValueSet, oldValueSet);
            }

            var parameterOverrides = this.ParameterOverrideService
                .GetShallow(transaction, partition, null, securityContext)
                .OfType<ParameterOverride>()
                .Where(x => x.Parameter == thing.Iid).ToList();

            var elementUsages = this.ElementUsageService.GetShallow(transaction, partition, null, securityContext)
                .Cast<ElementUsage>().Where(x => x.ElementDefinition == container.Iid).ToList();

            var parameterSubscriptionIds = new List<Guid>(thing.ParameterSubscription);
            parameterSubscriptionIds.AddRange(parameterOverrides.SelectMany(x => x.ParameterSubscription));

            var parameterSubscriptions = this.ParameterSubscriptionService
                .GetShallow(transaction, partition, parameterSubscriptionIds, securityContext)
                .OfType<ParameterSubscription>().ToList();

            foreach (var parameterOverride in parameterOverrides)
            {
                if (!thing.AllowDifferentOwnerOfOverride)
                {
                    // Set the owner of the ParameterOverride to the new owner of the Parameter
                    parameterOverride.Owner = thing.Owner;
                    var elementUsage = elementUsages.SingleOrDefault(x => x.ParameterOverride.Contains(parameterOverride.Iid));

                    if (elementUsage == null)
                    {
                        throw new InvalidOperationException("The ElementUsage could not be retrieved.");
                    }

                    this.ParameterOverrideService.UpdateConcept(
                        transaction,
                        partition,
                        parameterOverride,
                        elementUsage);
                }

                this.UpdateParameterOverrideAndSubscription(
                    parameterOverride,
                    newOldValueSetMap,
                    parameterSubscriptions,
                    defaultValueArray,
                    transaction,
                    partition,
                    securityContext);
            }

            // Remove the subscriptions owned by the new owner of the Parameter
            if (isOwnerChanged && parameterSubscriptions.Any(s => s.Owner == thing.Owner))
            {
                var parameterSubscriptionToRemove = parameterSubscriptions.SingleOrDefault(s => s.Owner == thing.Owner && thing.ParameterSubscription.Contains(s.Iid));

                if (parameterSubscriptionToRemove != null)
                {
                    if (this.ParameterSubscriptionService.DeleteConcept(
                            transaction,
                            partition,
                            parameterSubscriptionToRemove,
                            thing))
                    {
                        parameterSubscriptions.Remove(parameterSubscriptionToRemove);
                    }
                }

                // Remove the subscriptions owned by the new owner of the Parameter on all parameterOverrides
                foreach (var parameterOverride in parameterOverrides.Where(o => o.ParameterSubscription.Count != 0))
                {
                    var subscriptionToRemove = parameterSubscriptions.SingleOrDefault(
                        s => parameterOverride.ParameterSubscription.Contains(s.Iid)
                             && s.Owner == parameterOverride.Owner);

                    if (subscriptionToRemove != null)
                    {
                        if (this.ParameterSubscriptionService.DeleteConcept(
                                transaction,
                                partition,
                                subscriptionToRemove,
                                thing))
                        {
                            parameterSubscriptions.Remove(subscriptionToRemove);
                        }
                    }
                }
            }

            this.UpdateParameterSubscriptions(
                thing,
                newOldValueSetMap,
                parameterSubscriptions,
                defaultValueArray,
                transaction,
                partition,
                securityContext);

            // clean up old values
            await this.DeleteOldValueSet(transaction, partition, originalThing);
        }

        /// <summary>
        /// Checks if a <see cref="Parameter"/> having a <see cref="ParameterType"/> that was already present in the <see cref="ElementDefinition"/>.
        /// If so, throws a <see cref="BadRequestException"/>
        /// </summary>
        /// <param name="container">
        /// The container <see cref="Thing"/>
        /// </param>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        /// <param name="IsUpdateExisting">
        /// Are we performing an update , or an insert?
        /// </param>
        /// <exception cref="BadRequestException">
        /// Throws a <see cref="BadRequestException"/> adding a <see cref="Parameter"/> results in a duplicate <see cref="ParameterType"/>
        /// for the <see cref="ElementDefinition"/>
        /// </exception>
        /// <remarks>
        /// Already existing duplicate ParameterTypes in the database are left untouched in this method and therefore will not throw an error.
        /// </remarks>
        private Task CheckDuplicateParameterTypes(Thing container, Parameter thing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, bool IsUpdateExisting = false)
        {
            var elementDefinition = container as ElementDefinition;

            if (elementDefinition == null)
            {
                throw new Cdp4ModelValidationException($"{nameof(ElementDefinition)} not found for {nameof(Parameter)} having id {thing.Iid}");
            }

            var updatedElementDefinition = this.ElementDefinitionService.GetShallow(transaction, partition, new[] { elementDefinition.Iid }, securityContext).SingleOrDefault() as ElementDefinition;

            if (updatedElementDefinition == null)
            {
                throw new Cdp4ModelValidationException($"{nameof(ElementDefinition)} not found for {nameof(Parameter)} having id {thing.Iid}");
            }

            var originalParameterGuids = elementDefinition.Parameter;
            var updatedParameterGuids = updatedElementDefinition.Parameter.Except(elementDefinition.Parameter).ToList();
            var existingNonRemovedParameterGuids = originalParameterGuids.Intersect(updatedElementDefinition.Parameter).ToArray();

            if (IsUpdateExisting && !updatedParameterGuids.Contains(thing.Iid))
            {
                updatedParameterGuids.Add(thing.Iid);
            }

            // Get all updated Parameters from the database
            var updatedParameters = this.ParameterService.GetShallow(transaction, partition, updatedParameterGuids, securityContext);
            var updatedParameterTypes = updatedParameters.OfType<Parameter>().Select(x => x.ParameterType).ToArray();

            // Find new/updated parameters that cause a duplication in the current Transaction/Operation Container
            var duplicateUpdatedParameterTypes = updatedParameterTypes.GroupBy(x => x).Where(x => x.Count() > 1).Select(x => x.Key).ToArray();

            if (duplicateUpdatedParameterTypes.Length != 0)
            {
                var duplicateParameterTypes =
                    this.CachedReferenceDataService.QueryParameterTypes(transaction, securityContext)
                        .Where(x => duplicateUpdatedParameterTypes.Contains(x.Key))
                        .Select(x => x.Value);

                throw new BadRequestException(
                    $"Cannot add the same {nameof(ParameterType)} to an {nameof(ElementDefinition)} multiple times: '{string.Join("', and '", duplicateParameterTypes.Select(x => x.Name))}'.");
            }

            // Get all non-removed existing Parameters from the database
            var existingNonRemovedParameters =
                existingNonRemovedParameterGuids.Length != 0
                    ? this.ParameterService.GetShallow(transaction, partition, existingNonRemovedParameterGuids, securityContext)
                    : Array.Empty<Thing>();

            var existingParameterTypes = existingNonRemovedParameters.OfType<Parameter>().Select(x => x.ParameterType).Distinct().ToArray();

            //find all updated ParameterTypes that cause a duplication based on existing ParameterTypes
            var duplicateParameterTypeGuids = existingParameterTypes.Intersect(updatedParameterTypes).ToArray();

            if (duplicateParameterTypeGuids.Length != 0)
            {
                var duplicateParameterTypes =
                    this.CachedReferenceDataService.QueryParameterTypes(transaction, securityContext)
                        .Where(x => duplicateParameterTypeGuids.Contains(x.Key))
                        .Select(x => x.Value);

                throw new BadRequestException(
                    $"{nameof(ElementDefinition)} '{elementDefinition.Name}' already contains {nameof(Parameter)}(s) of type(s) '{string.Join("', and '", duplicateParameterTypes.Select(x => x.Name))}'.");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Update the associated <see cref="ParameterOverride"/> and their <see cref="ParameterSubscription"/>
        /// </summary>
        /// <param name="parameterOverride">The <see cref="ParameterOverride"/></param>
        /// <param name="newOldValueSet">The new <see cref="ParameterValueSet"/></param>
        /// <param name="parameterSubscriptions">The <see cref="ParameterSubscription"/></param>
        /// <param name="defaultValueArray">The default value array</param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        private void UpdateParameterOverrideAndSubscription(
            ParameterOverride parameterOverride,
            IReadOnlyDictionary<ParameterValueSet, ParameterValueSet> newOldValueSet,
            IEnumerable<ParameterSubscription> parameterSubscriptions,
            ValueArray<string> defaultValueArray,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            var overrideSubscription = parameterSubscriptions
                .Where(x => parameterOverride.ParameterSubscription.Contains(x.Iid)).ToList();

            var overrideSubscriptionValueSets =
                this.ParameterSubscriptionValueSetService.GetShallow(transaction, partition, overrideSubscription.SelectMany(x => x.ValueSet), securityContext)
                    .Cast<ParameterSubscriptionValueSet>()
                    .ToList();

            var oldOverrideSets = this.ParameterOverrideValueSetService.GetShallow(transaction, partition, parameterOverride.ValueSet, securityContext).Cast<ParameterOverrideValueSet>().ToList();

            foreach (var parameterValueSet in newOldValueSet)
            {
                var oldOverrideValueSet = parameterValueSet.Value != null ? oldOverrideSets.FirstOrDefault(x => x.ParameterValueSet == parameterValueSet.Value.Iid) : null;

                var newValueSetOverride =
                    this.ParameterOverrideValueSetFactory.CreateWithOldValues(
                        oldOverrideValueSet,
                        parameterValueSet.Key);

                this.ParameterOverrideValueSetService.CreateConcept(
                    transaction,
                    partition,
                    newValueSetOverride,
                    parameterOverride);

                foreach (var parameterSubscription in overrideSubscription)
                {
                    var oldSubscriptionValueSet = oldOverrideValueSet != null ? overrideSubscriptionValueSets.FirstOrDefault(x => x.SubscribedValueSet == oldOverrideValueSet.Iid) : null;

                    var newSubscriptionValueSet =
                        this.ParameterSubscriptionValueSetFactory.CreateWithOldValues(
                            oldSubscriptionValueSet,
                            newValueSetOverride.Iid,
                            defaultValueArray);

                    this.ParameterSubscriptionValueSetService.CreateConcept(
                        transaction,
                        partition,
                        newSubscriptionValueSet,
                        parameterSubscription);
                }
            }
        }

        /// <summary>
        /// Update the <see cref="ParameterSubscription"/> associated to a <see cref="Parameter"/>
        /// </summary>
        /// <param name="parameter">The <see cref="Parameter"/></param>
        /// <param name="newOldValueSet">The new <see cref="ParameterValueSet"/></param>
        /// <param name="parameterSubscriptions">The <see cref="ParameterSubscription"/></param>
        /// <param name="defaultValueArray">The default value array</param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        private void UpdateParameterSubscriptions(
            Parameter parameter,
            IReadOnlyDictionary<ParameterValueSet, ParameterValueSet> newOldValueSet,
            IEnumerable<ParameterSubscription> parameterSubscriptions,
            ValueArray<string> defaultValueArray,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            var subscriptions = parameterSubscriptions.Where(x => parameter.ParameterSubscription.Contains(x.Iid))
                .ToList();

            var subscriptionValueSets = this.ParameterSubscriptionValueSetService.GetShallow(transaction, partition, subscriptions.SelectMany(x => x.ValueSet), securityContext).Cast<ParameterSubscriptionValueSet>().ToList();

            foreach (var parameterValueSet in newOldValueSet)
            {
                foreach (var parameterSubscription in subscriptions)
                {
                    var oldSubscriptionValueSet = parameterValueSet.Value != null ? subscriptionValueSets.FirstOrDefault(x => x.SubscribedValueSet == parameterValueSet.Value.Iid) : null;

                    var newSubscriptionValueSet =
                        this.ParameterSubscriptionValueSetFactory.CreateWithOldValues(
                            oldSubscriptionValueSet,
                            parameterValueSet.Key.Iid,
                            defaultValueArray);

                    this.ParameterSubscriptionValueSetService.CreateConcept(
                        transaction,
                        partition,
                        newSubscriptionValueSet,
                        parameterSubscription);
                }
            }
        }

        /// <summary>
        /// Compute the <see cref="ParameterValueSet"/> for a <see cref="Parameter"/>
        /// </summary>
        /// <param name="parameter">
        /// The <see cref="Parameter"/>
        /// </param>
        /// <param name="oldParameter">
        /// The old parameter
        /// </param>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="partition">
        /// The current partition
        /// </param>
        /// <param name="securityContext">
        /// The security context
        /// </param>
        /// <param name="defaultValueArray">
        /// The default value-array
        /// </param>
        /// <returns>
        /// The new <see cref="ParameterValueSet"/>s
        /// </returns>
        private ReadOnlyCollection<ParameterValueSet> ComputeValueSets(
            Parameter parameter,
            Parameter oldParameter,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ValueArray<string> defaultValueArray)
        {
            // redirect out the iteration partition to the engineeringmodel one, as that contains the iteration information
            var engineeringModelPartition = partition.Replace(
                Utils.IterationSubPartition,
                Utils.EngineeringModelPartition);

            var iteration = this.IterationService.GetActiveIteration(transaction, engineeringModelPartition, securityContext);

            if (oldParameter != null)
            {
                this.OldParameterContextProvider.Initialize(oldParameter, transaction, partition, securityContext, iteration);
            }

            var newValueSet = new List<ParameterValueSet>();

            if (parameter.IsOptionDependent)
            {
                newValueSet.AddRange(
                    this.CreateDefaultOptionDependentValueSetCollection(
                        parameter,
                        iteration,
                        transaction,
                        partition,
                        securityContext,
                        defaultValueArray));
            }
            else if (parameter.StateDependence != null)
            {
                var actualList = this.ActualFiniteStateListService
                    .GetShallow(transaction, partition, [parameter.StateDependence.Value], securityContext)
                    .Cast<ActualFiniteStateList>()
                    .First();

                newValueSet.AddRange(
                    this.CreateDefaultStateDependentValueSetCollection(actualList, null, defaultValueArray));
            }
            else
            {
                var sourceValueSet = this.OldParameterContextProvider.GetsourceValueSet(null, null);
                newValueSet.Add(this.ParameterValueSetFactory.CreateNewParameterValueSetFromSource(null, null, sourceValueSet, defaultValueArray));
            }

            return newValueSet.AsReadOnly();
        }

        /// <summary>
        /// Create default option-dependent <see cref="ParameterValueSet"/>
        /// </summary>
        /// <param name="parameter">
        /// The <see cref="Parameter"/>
        /// </param>
        /// <param name="iteration">
        /// The <see cref="Iteration"/>
        /// </param>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="partition">
        /// The partition
        /// </param>
        /// <param name="securityContext">
        /// The security context
        /// </param>
        /// <param name="defaultValueArray">
        /// The default Value Array.
        /// </param>
        /// <returns>
        /// The new <see cref="ParameterValueSet"/>
        /// </returns>
        private ReadOnlyCollection<ParameterValueSet> CreateDefaultOptionDependentValueSetCollection(
            Parameter parameter,
            Iteration iteration,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ValueArray<string> defaultValueArray)
        {
            var newValueSet = new List<ParameterValueSet>();
            var optionIds = iteration.Option.OrderBy(x => x.K).Select(x => Guid.Parse(x.V.ToString())).ToList();

            if (parameter.StateDependence != null)
            {
                var actualFiniteStateList = this.ActualFiniteStateListService
                    .GetShallow(
                        transaction,
                        partition,
                        new List<Guid> { parameter.StateDependence.Value },
                        securityContext)
                    .OfType<ActualFiniteStateList>().First();

                foreach (var option in optionIds)
                {
                    newValueSet.AddRange(
                        this.CreateDefaultStateDependentValueSetCollection(
                            actualFiniteStateList,
                            option,
                            defaultValueArray));
                }
            }
            else
            {
                foreach (var option in optionIds)
                {
                    var sourceValueSet = this.OldParameterContextProvider.GetsourceValueSet(option, null);
                    newValueSet.Add(this.ParameterValueSetFactory.CreateNewParameterValueSetFromSource(option, null, sourceValueSet, defaultValueArray));
                }
            }

            return newValueSet.AsReadOnly();
        }

        /// <summary>
        /// Create default state-dependent <see cref="ParameterValueSet"/>
        /// </summary>
        /// <param name="actualFiniteStateList">
        /// The <see cref="ActualFiniteStateList"/>
        /// </param>
        /// <param name="actualOption">
        /// The actual option
        /// </param>
        /// <param name="defaultValueArray">
        /// The default Value Array.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{ParameterValueSet}"/>.
        /// </returns>
        private IEnumerable<ParameterValueSet> CreateDefaultStateDependentValueSetCollection(
            ActualFiniteStateList actualFiniteStateList,
            Guid? actualOption,
            ValueArray<string> defaultValueArray)
        {
            foreach (var state in actualFiniteStateList.ActualState)
            {
                // try to find a value-set in the old sets that matches the specified state, else just take the fi
                var sourceValueSet = this.OldParameterContextProvider.GetsourceValueSet(actualOption, state);
                yield return this.ParameterValueSetFactory.CreateNewParameterValueSetFromSource(actualOption, state, sourceValueSet, defaultValueArray);
            }
        }

        /// <summary>
        /// Delete the old <see cref="ParameterValueSet"/> that are obsolete
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="partition">
        /// The current partition
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
        /// </param>
        private Task DeleteOldValueSet(NpgsqlTransaction transaction, string partition, Parameter originalThing)
        {
            foreach (var valueset in originalThing.ValueSet)
            {
                this.ParameterValueSetService.DeleteConcept(transaction, partition, new ParameterValueSet(valueset, 0), originalThing);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Write new <see cref="ParameterValueSet"/> into the database
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="parameter">The current <see cref="Parameter"/></param>
        /// <param name="newValueSet">The <see cref="ParameterValueSet"/> to write</param>
        private Task WriteValueSet(
            NpgsqlTransaction transaction,
            string partition,
            Parameter parameter,
            IEnumerable<ParameterValueSet> newValueSet)
        {
            foreach (var parameterValueSet in newValueSet)
            {
                this.ParameterValueSetService.CreateConcept(transaction, partition, parameterValueSet, parameter);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Validates ParameterType update on a Parameter.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Parameter"/> instance that will be inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        /// <param name="rawUpdateInfo">
        /// The raw update info that was serialized from the user posted request.
        /// The <see cref="ClasslessDTO"/> instance only contains values for properties that are to be updated.
        /// It is important to note that this variable is not to be changed likely as it can/will change the operation processor outcome.
        /// </param>
        private Task ValidateParameterTypeUpdate(
            Parameter thing,
            NpgsqlTransaction transaction,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            var parameterTypeId = (Guid)rawUpdateInfo["ParameterType"];

            var parameterTypes = this.CachedReferenceDataService.QueryParameterTypes(transaction, securityContext);

            if (!parameterTypes.TryGetValue(parameterTypeId, out var parameterType))
            {
                throw new ArgumentException($"ParameterType with iid {parameterTypeId} cannot be found.");
            }

            if (parameterType is QuantityKind)
            {
                // Check that a parameter contains scale and it is not changed to null
                if (thing.Scale != null)
                {
                    if (rawUpdateInfo.TryGetValue("Scale", out var value))
                    {
                        var scaleId = (Guid?)value;

                        if (scaleId == null || scaleId == Guid.Empty)
                        {
                            throw new ArgumentNullException(nameof(thing),
                                "Parameter with a parameterType of QuantityKind cannot have scale set to null.");
                        }
                    }
                }

                if (thing.Scale == null)
                {
                    if (rawUpdateInfo.TryGetValue("Scale", out var value))
                    {
                        var scaleId = (Guid?)value;

                        if (scaleId == null || scaleId == Guid.Empty)
                        {
                            throw new ArgumentNullException(nameof(rawUpdateInfo),
                                "Parameter with a parameterType of QuantityKind cannot have scale set to null.");
                        }
                    }
                    else
                    {
                        throw new ArgumentNullException(nameof(thing),
                            "Parameter with a parameterType of QuantityKind cannot have scale set to null.");
                    }
                }
            }
            else
            {
                // Check that a parameter does not contain scale
                if (thing.Scale != null)
                {
                    if (rawUpdateInfo.TryGetValue("Scale", out var value))
                    {
                        var scaleId = (Guid?)value;

                        if (scaleId != null)
                        {
                            throw new ArgumentException(
                                "Parameter with a parameterType of type different from QuantityKind must have scale set to null.");
                        }
                    }
                    else
                    {
                        throw new ArgumentException(
                            "Parameter with a parameterType of type different from QuantityKind must have scale set to null.");
                    }
                }

                if (thing.Scale == null)
                {
                    if (rawUpdateInfo.TryGetValue("Scale", out var value))
                    {
                        var scaleId = (Guid?)value;

                        if (scaleId != null)
                        {
                            throw new ArgumentException(
                                "Parameter with a parameterType of type different from QuantityKind must have scale set to null.");
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Validates ParameterType update on a Parameter.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Parameter"/> instance that will be inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        private Task ValidateParameterTypeAndScale(
            Parameter thing,
            NpgsqlTransaction transaction,
            ISecurityContext securityContext)
        {
            var parameterTypes = this.CachedReferenceDataService.QueryParameterTypes(transaction, securityContext);

            if (!parameterTypes.TryGetValue(thing.ParameterType, out var parameterType))
            {
                throw new ArgumentException($"ParameterType with iid {thing.ParameterType} cannot be found.");
            }

            if (parameterType is QuantityKind)
            {
                // Check that a parameter contains scale
                if (thing.Scale == null)
                {
                    throw new ArgumentNullException(nameof(thing),
                        "Parameter with a parameterType of QuantityKind cannot have scale set to null.");
                }
            }
            else
            {
                // Check that a parameter does not contain scale
                if (thing.Scale != null)
                {
                    throw new ArgumentException(
                        "Parameter with a parameterType of type different from QuantityKind must have scale set to null.");
                }
            }

            return Task.CompletedTask;
        }
    }
}
