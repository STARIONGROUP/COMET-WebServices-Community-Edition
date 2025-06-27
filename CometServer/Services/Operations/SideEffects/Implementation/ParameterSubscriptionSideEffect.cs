// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterSubscriptionSideEffect.cs" company="Starion Group S.A.">
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

    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Exceptions;
    using CDP4Common.Types;

    using Microsoft.Extensions.Logging;

    using Npgsql;

    using ParameterOrOverrideBase = CDP4Common.DTO.ParameterOrOverrideBase;
    using ParameterSubscription = CDP4Common.DTO.ParameterSubscription;
    using ParameterSubscriptionValueSet = CDP4Common.DTO.ParameterSubscriptionValueSet;

    /// <summary>
    /// The purpose of the <see cref="ParameterSubscriptionSideEffect"/> Side-Effect class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class ParameterSubscriptionSideEffect : OperationSideEffect<ParameterSubscription>
    {
        /// <summary>
        /// Gets or sets the (injected) <see cref="ILogger"/>
        /// </summary>
        public ILogger<ParameterSubscriptionSideEffect> Logger { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionValueSetService"/>
        /// </summary>
        public IParameterSubscriptionValueSetService ParameterSubscriptionValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDefaultValueArrayFactory"/>
        /// </summary>
        public IDefaultValueArrayFactory DefaultValueArrayFactory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterValueSetService"/>
        /// </summary>
        public IParameterValueSetService ParameterValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterOverrideValueSetService"/>
        /// </summary>
        public IParameterOverrideValueSetService ParameterOverrideValueSetService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterSubscriptionService"/>
        /// </summary>
        public IParameterSubscriptionService ParameterSubscriptionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterService"/>
        /// </summary>
        public IParameterService ParameterService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterOverrideService"/>
        /// </summary>
        public IParameterOverrideService ParameterOverrideService { get; set; }

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
        public override async Task<bool> BeforeCreateAsync(
            ParameterSubscription thing, 
            Thing container, 
            NpgsqlTransaction transaction, 
            string partition, 
            ISecurityContext securityContext)
        {
            await this.OrganizationalParticipationResolverService.ValidateCreateOrganizationalParticipationAsync(thing, container, securityContext, transaction, partition);

            if (thing.Owner == Guid.Empty)
            {
                throw new InvalidOperationException("The owner cannot be empty.");
            }

            CheckOwnership(thing, container);

            return await this.IsUniqueSubscriptionAsync(transaction, partition, securityContext, thing, container);
        }

        /// <summary>
        /// Execute additional logic  a successful create operation.
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
        public override async Task AfterCreateAsync(
            ParameterSubscription thing,
            Thing container,
            ParameterSubscription originalThing,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            await this.CreateParameterSubscriptionValueSetsAsync(thing, container, transaction, partition, securityContext);
        }

        /// <summary>
        /// Creates ParameterSubscriptionValueSets for the supplied ParameterValueSets.
        /// </summary>
        /// <param name="thing">
        ///     The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        ///     The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        ///     The current transaction to the database.
        /// </param>
        /// <param name="partition">
        ///     The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        ///     The security-context
        /// </param>
        private async Task CreateParameterSubscriptionValueSetsAsync(
            ParameterSubscription thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            if (!(container is ParameterOrOverrideBase parameterOrOverrideBase))
            {
                throw new InvalidOperationException("The container of a ParameterSubscription can only be a ParameterOrOverrideBase.");
            }

            var paramContainer = container is CDP4Common.DTO.Parameter
                    ? (ParameterOrOverrideBase)(await this.ParameterService.GetShallowAsync(transaction, partition, [container.Iid], securityContext)).OfType<CDP4Common.DTO.Parameter>().SingleOrDefault()
                    : (await this.ParameterOverrideService.GetShallowAsync(transaction, partition, [container.Iid], securityContext)).OfType<CDP4Common.DTO.ParameterOverride>().SingleOrDefault();
            
            if (paramContainer == null || !paramContainer.ValueSets.Any())
            {
                throw new InvalidOperationException($"Could not determine the value-set to subscribe on for the parameter-subscription {thing.Iid} to create on parameter/override {container.Iid}");
            }

            var parameterValueSets = parameterOrOverrideBase is CDP4Common.DTO.Parameter
                ? (await this.ParameterValueSetService.GetShallowAsync(transaction, partition, paramContainer.ValueSets, securityContext)).OfType<CDP4Common.DTO.ParameterValueSetBase>().ToArray()
                : (await this.ParameterOverrideValueSetService.GetShallowAsync(transaction, partition, paramContainer.ValueSets, securityContext)).OfType<CDP4Common.DTO.ParameterValueSetBase>().ToArray();

            foreach (var parameterValueSet in parameterValueSets)
            {
                var parameterSubscriptionValueSet =
                    new ParameterSubscriptionValueSet(Guid.NewGuid(), 0)
                        {
                            SubscribedValueSet = parameterValueSet.Iid,
                            ValueSwitch = ParameterSwitchKind.COMPUTED
                        };

                var valueArray = new ValueArray<string>(this.DefaultValueArrayFactory.CreateDefaultValueArray(parameterValueSet.Manual.Count));
                parameterSubscriptionValueSet.Manual = valueArray;

                await this.ParameterSubscriptionValueSetService.CreateConceptAsync(
                    transaction,
                    partition,
                    parameterSubscriptionValueSet,
                    thing);
            }
        }

        /// <summary>
        /// Execute additional logic  before an update operation.
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
        /// <param name="rawUpdateInfo">
        /// The raw Update Info.
        /// </param>
        public override Task BeforeUpdateAsync(
            ParameterSubscription thing, 
            Thing container, 
            NpgsqlTransaction transaction, 
            string partition, 
            ISecurityContext securityContext, 
            ClasslessDTO rawUpdateInfo)
        {
            CheckOwnership(thing, container);

            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Checks whether a Thing and its container have different owners.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        private static void CheckOwnership(ParameterSubscription thing, Thing container)
        {
            if (thing.Owner == ((ParameterOrOverrideBase)container).Owner)
            {
                throw new Cdp4ModelValidationException(
                    "Parameter and ParameterSubscription cannot have the same owner.");
            }
        }

        /// <summary>
        /// Check whether the subscription to create on a <paramref name="container"/> is unique owner-wise
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security-context</param>
        /// <param name="newSubscription">The subscription that is being created</param>
        /// <param name="container">The container</param>
        /// <returns>
        /// Returns true if the create operation may continue, otherwise it shall be skipped.
        /// </returns>
        private async Task<bool> IsUniqueSubscriptionAsync(NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, ParameterSubscription newSubscription, Thing container)
        {
            var parameterOrOverride =
                (ParameterOrOverrideBase)(await this.ParameterService.GetShallowAsync(transaction, partition, [container.Iid], securityContext)).SingleOrDefault()
                ?? (ParameterOrOverrideBase)(await this.ParameterOverrideService.GetShallowAsync(transaction, partition, [container.Iid], securityContext)).SingleOrDefault();

            if (parameterOrOverride == null)
            {
                throw new InvalidOperationException("The container of a new parameter-subscription can only be a ParameterOrOverrideBase");
            }

            var existingSubscription = (await this.ParameterSubscriptionService.GetShallowAsync(transaction, partition, parameterOrOverride.ParameterSubscription, securityContext)).OfType<ParameterSubscription>().FirstOrDefault(x => x.Owner == newSubscription.Owner);

            if (existingSubscription != null)
            {
                this.Logger.LogWarning("A subscription already exist on parameter {Container}.", container.Iid);

                return false;
            }

            return true;
        }
    }
}
