﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterOverrideSideEffect.cs" company="Starion Group S.A.">
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
    using System.Linq;
    using System.Threading.Tasks;

    using Authorization;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="ParameterOverrideSideEffect"/> Side-Effect class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class ParameterOverrideSideEffect : OperationSideEffect<ParameterOverride>
    {
        /// <summary>
        /// Gets or sets the <see cref="IParameterValueSetService"/>
        /// </summary>
        public IParameterValueSetService ParameterValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterOverrideValueSetService"/>
        /// </summary>
        public IParameterOverrideValueSetService ParameterOverrideValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterService"/>
        /// </summary>
        public IParameterService ParameterService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterOverrideValueSetFactory"/>
        /// </summary>
        public IParameterOverrideValueSetFactory ParameterOverrideValueSetFactory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionValueSetFactory"/>
        /// </summary>
        public IParameterSubscriptionValueSetFactory ParameterSubscriptionValueSetFactory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionService"/>
        /// </summary>
        public IParameterSubscriptionService ParameterSubscriptionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionValueSetService"/>
        /// </summary>
        public IParameterSubscriptionValueSetService ParameterSubscriptionValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDefaultValueArrayFactory"/>
        /// </summary>
        public IDefaultValueArrayFactory DefaultValueArrayFactory { get; set; }

        /// <summary>
        /// Gets the list of property names that are to be excluded from validation logic.
        /// </summary>
        public override IEnumerable<string> DeferPropertyValidation => ["ValueSet"];

        /// <summary>
        /// Execute additional logic  before a create operation.
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
        public override Task<bool> BeforeCreate(ParameterOverride thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            this.OrganizationalParticipationResolverService.ValidateCreateOrganizationalParticipation(thing, container, securityContext, transaction, partition);

            return Task.FromResult(true);
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
        public override async Task AfterCreate(ParameterOverride thing, Thing container, ParameterOverride originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            var newValueSet = (await this.ComputeValueSets(thing, transaction, partition, securityContext)).ToList();
            await this.WriteValueSet(transaction, partition, thing, newValueSet);
            await this.CreateSubscriptionFromParameter(transaction, partition, thing, securityContext, newValueSet);
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
        public override Task AfterUpdate(ParameterOverride thing, Thing container, ParameterOverride originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            var isOwnerChanged = thing.Owner != originalThing.Owner;

            // Remove the subscriptions owned by the new owner of the ParameterOverride
            var parameterSubscriptions = this.ParameterSubscriptionService.GetShallowAsync(transaction, partition, thing.ParameterSubscription, securityContext).OfType<ParameterSubscription>().ToArray();

            if (isOwnerChanged && parameterSubscriptions.Any(s => s.Owner == thing.Owner))
            {
                var parameterSubscriptionToRemove = parameterSubscriptions.SingleOrDefault(
                        s => s.Owner == thing.Owner && thing.ParameterSubscription.Contains(s.Iid));
            
                if (parameterSubscriptionToRemove != null)
                {
                    if (!this.ParameterSubscriptionService.DeleteConceptAsync(transaction, partition, parameterSubscriptionToRemove, thing))
                    {
                        throw new InvalidOperationException(
                            $"The update operation of the parameter override value set {parameterSubscriptionToRemove.Iid} failed.");
                    }
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Compute the <see cref="ParameterValueSet"/> for a <see cref="Parameter"/>
        /// </summary>
        /// <param name="parameterOverride">
        /// The <see cref="Parameter"/>
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
        /// <returns>
        /// The new <see cref="ParameterOverrideValueSet"/>s
        /// </returns>
        private Task<IEnumerable<ParameterOverrideValueSet>> ComputeValueSets(ParameterOverride parameterOverride, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            var parameters = this.ParameterService.GetShallowAsync(transaction, partition, [parameterOverride.Parameter], securityContext).OfType<Parameter>().ToArray();

            if (parameters.Length != 1)
            {
                throw new InvalidOperationException("None or more than one parameters were returned");
            }

            var parameter = parameters.Single();

            var parameterValueSets = this.ParameterValueSetService.GetShallowAsync(transaction, partition, parameter.ValueSet, securityContext).OfType<ParameterValueSet>().ToArray();

            if (parameterValueSets.Length != parameter.ValueSet.Count)
            {
                throw new InvalidOperationException("All the value sets could not be retrieved.");
            }

            var result = new List<ParameterOverrideValueSet>();

            foreach (var set in parameterValueSets)
            {
                result.Add(this.ParameterOverrideValueSetFactory.CreateParameterOverrideValueSet(set));
            }

            return Task.FromResult(result.AsEnumerable());
        }

        /// <summary>
        /// Write new <see cref="ParameterOverrideValueSet"/> into the database
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="parameterOverride">The current <see cref="ParameterOverride"/></param>
        /// <param name="newValueSet">The <see cref="ParameterOverrideValueSet"/> to write</param>
        private Task WriteValueSet(NpgsqlTransaction transaction, string partition, ParameterOverride parameterOverride, IEnumerable<ParameterOverrideValueSet> newValueSet)
        {
            foreach (var parameterValueSet in newValueSet)
            {
                this.ParameterOverrideValueSetService.CreateConceptAsync(transaction, partition, parameterValueSet, parameterOverride);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Create <see cref="ParameterSubscription"/> for this <paramref name="parameterOverride"/> from its <see cref="Parameter"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="parameterOverride">The <see cref="ParameterOverride"/></param>
        /// <param name="securityContext">The <see cref="ISecurityContext"/></param>
        /// <param name="overrideValueset">The <see cref="ParameterOverrideValueSet"/> to subscribe on</param>
        private Task CreateSubscriptionFromParameter(NpgsqlTransaction transaction, string partition, ParameterOverride parameterOverride, ISecurityContext securityContext, IReadOnlyList<ParameterOverrideValueSet> overrideValueset)
        {
            var parameters = this.ParameterService.GetShallowAsync(transaction, partition, [parameterOverride.Parameter], securityContext).OfType<Parameter>().ToArray();

            if (parameters.Length != 1)
            {
                throw new InvalidOperationException("None or more than one parameters were returned");
            }

            var parameter = parameters.Single();

            var existingSubscriptions = this.ParameterSubscriptionService
                                            .GetShallowAsync(transaction, partition, null, securityContext)
                                            .OfType<ParameterSubscription>()
                                            .ToList();

            var parameterSubscriptions = existingSubscriptions.Where(x => parameter.ParameterSubscription.Contains(x.Iid)).ToList();

            this.DefaultValueArrayFactory.Load(transaction, securityContext);
            var defaultArray = this.DefaultValueArrayFactory.CreateDefaultValueArray(parameter.ParameterType);

            var existingOverrideSubscriptions = existingSubscriptions.Where(x => parameterOverride.ParameterSubscription.Contains(x.Iid)).ToList();

            var subscriptionValueSets =
                this.ParameterSubscriptionValueSetService
                    .GetShallowAsync(transaction, partition, parameterSubscriptions.SelectMany(x => x.ValueSet), securityContext)
                    .OfType<ParameterSubscriptionValueSet>()
                    .ToList();

            foreach (var parameterSubscription in parameterSubscriptions)
            {
                if (parameterSubscription.Owner == parameterOverride.Owner || existingOverrideSubscriptions.Any(x => x.Owner == parameterSubscription.Owner))
                {
                    continue;
                }

                var newSubscription = new ParameterSubscription(Guid.NewGuid(), 0) { Owner = parameterSubscription.Owner };
                this.ParameterSubscriptionService.CreateConceptAsync(transaction, partition, newSubscription, parameterOverride);

                foreach (var parameterOverrideValueSet in overrideValueset)
                {
                    var subscriptionValueset =
                        this.ParameterSubscriptionValueSetFactory.CreateWithOldValues(
                            subscriptionValueSets.FirstOrDefault(x => x.SubscribedValueSet == parameterOverrideValueSet.ParameterValueSet), 
                            parameterOverrideValueSet.Iid, 
                            defaultArray);

                    this.ParameterSubscriptionValueSetService.CreateConceptAsync(transaction, partition, subscriptionValueset, newSubscription);
                }
            }

            return Task.CompletedTask;
        }
    }
}
