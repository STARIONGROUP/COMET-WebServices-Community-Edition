// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterValueSetSideEffect.cs" company="Starion Group S.A.">
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
    using CDP4Common.Validation;

    using CometServer.Exceptions;

    using Npgsql;

    using Parameter = CDP4Common.DTO.Parameter;
    using ParameterValueSet = CDP4Common.DTO.ParameterValueSet;

    /// <summary>
    /// The purpose of the <see cref="ParameterValueSetSideEffect"/> Side-Effect class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class ParameterValueSetSideEffect : OperationSideEffect<ParameterValueSet>
    {
        /// <summary>
        /// Gets or sets the injected <see cref="IParameterService"/> used to retrieve linked <see cref="Thing"/>
        /// </summary>
        public IParameterService ParameterService { get; set; }

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
        public override Task<bool> BeforeCreateAsync(
            ParameterValueSet thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Execute additional logic before a delete operation
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/></param>
        /// <param name="container">The container of the <see cref="Thing"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        public override Task BeforeDeleteAsync(ParameterValueSet thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            throw new InvalidOperationException("ParameterValueSet Cannot be deleted");
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
        public override async Task BeforeUpdateAsync(ParameterValueSet thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, ClasslessDTO rawUpdateInfo)
        {
            await base.BeforeUpdateAsync(thing, container, transaction, partition, securityContext, rawUpdateInfo);

            if (rawUpdateInfo.Keys.All(key => !Array.Exists(Enum.GetNames(typeof(ParameterSwitchKind)), 
                        x => key.Equals(x, StringComparison.InvariantCultureIgnoreCase))))
            {
                return;
            }

            if (container is not Parameter parameter)
            {
                throw new ArgumentException("The container of the ParameterValueSet is not a Parameter", nameof(container));
            }

            var things = new List<Thing>();

            things.AddRange(await this.ParameterService.QueryReferencedSiteDirectoryThingsAsync(parameter, transaction, securityContext));

            var validationResult = parameter.ValidateAndCleanup(rawUpdateInfo, things, CultureInfo.InvariantCulture);

            if (validationResult.ResultKind != ValidationResultKind.Valid)
            {
                throw new BadRequestException(validationResult.Message);
            }
        }
    }
}
