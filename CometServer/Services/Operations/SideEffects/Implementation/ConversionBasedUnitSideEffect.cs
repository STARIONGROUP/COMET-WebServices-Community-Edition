﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConversionBasedUnitSideEffect.cs" company="Starion Group S.A.">
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

    using CDP4Common;
    using CDP4Common.DTO;

    using CometServer.Exceptions;
    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// Abstract super class from which all ConversionBasedUnit SideEffect sub classes derive.
    /// </summary>
    /// <typeparam name="T">
    /// Generic type T that must be of type <see cref="Thing"/>
    /// </typeparam>
    public abstract class ConversionBasedUnitSideEffect<T> : OperationSideEffect<T> where T : ConversionBasedUnit
    {
        /// <summary>
        /// Gets or sets the <see cref="ISiteReferenceDataLibraryService"/>
        /// </summary>
        public ISiteReferenceDataLibraryService SiteReferenceDataLibraryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ConversionBasedUnitService"/>
        /// </summary>
        public IConversionBasedUnitService ConversionBasedUnitService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="T"/> instance that will be inspected.
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
        public override async Task BeforeUpdateAsync(
            T thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            if (rawUpdateInfo.TryGetValue("ReferenceUnit", out var value))
            {
                var referenceUnitId = (Guid)value;

                // Check for itself
                if (referenceUnitId == thing.Iid)
                {
                    throw new AcyclicValidationException(
                        $"ConversionBasedUnit {thing.Iid} cannot have itself as a RefernceUnit");
                }

                // Get RDL chain and collect units' ids
                var unitIdsFromChain = await this.GetUnitIdsFromRdlChainAsync(
                    transaction,
                    partition,
                    securityContext,
                    ((ReferenceDataLibrary)container).RequiredRdl);

                unitIdsFromChain.AddRange(((ReferenceDataLibrary)container).Unit);

                // Check that reference unit is present in the chain
                if (!unitIdsFromChain.Contains(referenceUnitId))
                {
                    throw new AcyclicValidationException(
                        $"ConversionBasedUnit {thing.Iid} cannot have a RefernceUnit from outside the RDL chain");
                }

                // Get all ConversionBasedUnits
                var units = (await this.ConversionBasedUnitService
                    .GetAsync(transaction, partition, unitIdsFromChain, securityContext)).Cast<ConversionBasedUnit>()
                    .ToList();

                // Check reference unit that it is acyclic
                if (!IsReferenceUnitAcyclic(units, referenceUnitId, thing.Iid))
                {
                    throw new AcyclicValidationException(
                        $"ConversionBasedUnit {thing.Iid} cannot have a RefernceUnit {referenceUnitId} that leads to cyclic dependency");
                }
            }
        }

        /// <summary>
        /// Get unit ids from an rdl chain.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        /// <param name="rdlId">
        /// The Iid of RDL to start from.
        /// </param>
        /// <returns>
        /// The list of unit ids.
        /// </returns>
        private async Task<List<Guid>> GetUnitIdsFromRdlChainAsync(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            Guid? rdlId)
        {
            var availableRdls = (await this.SiteReferenceDataLibraryService.GetAsync(transaction, partition, null, securityContext))
                .Cast<SiteReferenceDataLibrary>().ToList();

            var unitIds = new List<Guid>();

            var requiredRdl = rdlId;

            while (requiredRdl != null)
            {
                var rdl = availableRdls.Find(x => x.Iid == requiredRdl);
                unitIds.AddRange(rdl.Unit);
                requiredRdl = rdl.RequiredRdl;
            }

            return unitIds;
        }

        /// <summary>
        /// Is reference unit acyclic.
        /// </summary>
        /// <param name="units">
        /// The units from the rdl chain.
        /// </param>
        /// <param name="referenceUnitId">
        /// The reference unit to check for being acyclic.
        /// </param>
        /// <param name="conversionBasedUnitId">
        /// The based unit id to set a reference unit to.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> whether a reference unit will not lead to cyclic dependency.
        /// </returns>
        private static bool IsReferenceUnitAcyclic(List<ConversionBasedUnit> units, Guid referenceUnitId, Guid conversionBasedUnitId)
        {
            Guid? nextContainingGroupId = referenceUnitId;

            while (nextContainingGroupId != null)
            {
                if (nextContainingGroupId == conversionBasedUnitId)
                {
                    return false;
                }

                nextContainingGroupId = units.FirstOrDefault(x => x.Iid == nextContainingGroupId)?.ReferenceUnit;
            }

            return true;
        }
    }
}