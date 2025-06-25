// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterSubscriptionValueSetSideEffect.cs" company="Starion Group S.A.">
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
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using Authorization;

    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Exceptions;
    using CDP4Common.Validation;

    using CometServer.Exceptions;

    using Npgsql;

    using Parameter = CDP4Common.DTO.Parameter;
    using ParameterOrOverrideBase = CDP4Common.DTO.ParameterOrOverrideBase;
    using ParameterOverride = CDP4Common.DTO.ParameterOverride;
    using ParameterSubscription = CDP4Common.DTO.ParameterSubscription;
    using ParameterSubscriptionValueSet = CDP4Common.DTO.ParameterSubscriptionValueSet;

    /// <summary>
    /// The purpose of the <see cref="ParameterSubscriptionValueSetSideEffect"/> Side-Effect class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class ParameterSubscriptionValueSetSideEffect : OperationSideEffect<ParameterSubscriptionValueSet>
    {
        /// <summary>
        /// Gets or sets the injected <see cref="IParameterService"/> used to retrieve linked <see cref="Thing"/>
        /// </summary>
        public IParameterService ParameterService { get; set; }

        /// <summary>
        /// Gets or sets the injected <see cref="IParameterOverrideService"/> used to retrieve linked <see cref="Thing"/>
        /// </summary>
        public IParameterOverrideService ParameterOverrideService { get; set; }

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
        public override Task<bool> BeforeCreate(
            ParameterSubscriptionValueSet thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
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
        /// The raw update info that was serialized from the user posted request.
        /// The <see cref="ClasslessDTO"/> instance only contains values for properties that are to be updated.
        /// It is important to note that this variable is not to be changed likely as it can/will change the operation processor outcome.
        /// </param>
        public override Task BeforeUpdate(ParameterSubscriptionValueSet thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, ClasslessDTO rawUpdateInfo)
        {
            base.BeforeUpdate(thing, container, transaction, partition, securityContext, rawUpdateInfo);

            if (rawUpdateInfo.Keys.All(key => !Array.Exists(Enum.GetNames(typeof(ParameterSwitchKind)), 
                    x => key.Equals(x, StringComparison.InvariantCultureIgnoreCase))))
            {
                return Task.CompletedTask;
            }

            if (container is not ParameterSubscription parameterSubscription)
            {
                throw new ArgumentException("The container of the ParameterSubscriptionValueSet is not a ParameterSubscription", nameof(container));
            }

            var parameters = this.ParameterService.Get(transaction, partition, null, securityContext).ToList();

            var parameterOrOverrideBase =
                parameters
                    .OfType<ParameterOrOverrideBase>()
                    .SingleOrDefault(
                        x => x.ParameterSubscription.Contains(parameterSubscription.Iid)) 
                ?? 
                this.ParameterOverrideService.Get(transaction, partition, null, securityContext)
                    .OfType<ParameterOrOverrideBase>()
                    .SingleOrDefault(
                        x => x.ParameterSubscription.Contains(parameterSubscription.Iid));

            var things = new List<Thing>();

            if (parameterOrOverrideBase == null)
            {
                throw new ThingNotFoundException("The ParameterSubscription's container Parameter, or ParameterOverride was not found");
            }

            ValidationResult validationResult = default;
            validationResult.Message = "Validation failed";

            if (parameterOrOverrideBase is Parameter parameter)
            {
                things.AddRange(this.ParameterService.QueryReferencedSiteDirectoryThings(parameter, transaction, securityContext));

                validationResult = parameter.ValidateAndCleanup(rawUpdateInfo, things, CultureInfo.InvariantCulture);
            }

            if (parameterOrOverrideBase is ParameterOverride parameterOverride)
            {
                var parameterOverrideParameter = parameters.SingleOrDefault(x => x.Iid == parameterOverride.Parameter) as Parameter;

                if (parameterOrOverrideBase == null)
                {
                    throw new ThingNotFoundException("The ParameterOverride's container Parameter was not found");
                }

                things.Add(parameterOverrideParameter);
                things.AddRange(this.ParameterService.QueryReferencedSiteDirectoryThings(parameterOverrideParameter, transaction, securityContext));

                validationResult = parameterOverride.ValidateAndCleanup(rawUpdateInfo, things, CultureInfo.InvariantCulture);
            }

            if (validationResult.ResultKind != ValidationResultKind.Valid)
            {
                throw new BadRequestException(validationResult.Message);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Execute additional logic before a delete operation
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="container">The container of the <see cref="Thing"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        public override Task BeforeDelete(ParameterSubscriptionValueSet thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            throw new InvalidOperationException("ParameterOverrideValueSet Cannot be deleted");
        }
    }
}
