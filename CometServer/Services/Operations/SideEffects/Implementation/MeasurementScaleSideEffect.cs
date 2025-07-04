// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeasurementScaleSideEffect.cs" company="Starion Group S.A.">
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
    /// The purpose of the <see cref="MeasurementScaleSideEffect"/> class is to execute additional logic before and
    /// after a specific operation is performed.
    /// </summary>
    public class MeasurementScaleSideEffect : OperationSideEffect<MeasurementScale>
    {
        /// <summary>
        /// Gets or sets the <see cref="ISiteReferenceDataLibraryService"/>
        /// </summary>
        public ISiteReferenceDataLibraryService SiteReferenceDataLibraryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IMappingToReferenceScaleService"/>
        /// </summary>
        public IMappingToReferenceScaleService MappingToReferenceScaleService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IScaleValueDefinitionService"/>
        /// </summary>
        public IScaleValueDefinitionService ScaleValueDefinitionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IMeasurementScaleService"/>
        /// </summary>
        public IMeasurementScaleService MeasurementScaleService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="MeasurementScale"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <paramref name="thing"/> that is inspected.
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
            MeasurementScale thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            if (rawUpdateInfo.TryGetValue("MappingToReferenceScale", out var value))
            {
                var mappingToReferenceScaleIids = (List<Guid>)value;

                var referenceDataLibrary = (ReferenceDataLibrary)container;

                // Check that all referenced MeasurementScales are from the same RDL chain
                var availableMeasurementScaleIids = await this.GetMeasurementScaleIidsFromRdlChainAsync(
                    transaction,
                    partition,
                    securityContext,
                    referenceDataLibrary.RequiredRdl);

                availableMeasurementScaleIids.AddRange(referenceDataLibrary.Scale);

                var allMeasurementScales = (await this.MeasurementScaleService
                    .GetAsync(transaction, partition, null, securityContext))
                    .Cast<MeasurementScale>()
                    .ToList();

                var scaleValueDefinitionContainerMeasurementScales = await this.GetScaleValueDefinitionContainerMeasurementScalesAsync(
                    transaction, partition, securityContext, allMeasurementScales, mappingToReferenceScaleIids);

                if (scaleValueDefinitionContainerMeasurementScales.Any(x => !availableMeasurementScaleIids.Contains(x.Iid)))
                {
                    throw new AcyclicValidationException($"MeasurementScale {thing.Name} {thing.Iid} cannot have " +
                                                         $"a MappingToReferenceScale referencing or depending upon " +
                                                         $"a ScaleValueDefinition contained by a MeasurementScale " +
                                                         $"from outside the current RDL chain.");
                }

                await this.CheckCycleDeepAsync(
                    transaction,
                    partition,
                    securityContext,
                    allMeasurementScales,
                    thing,
                    scaleValueDefinitionContainerMeasurementScales);
            }
        }

        /// <summary>
        /// Gets <see cref="MeasurementScale"/> iids from an <see cref="SiteReferenceDataLibrary"/> chain.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security context used for permission checking.
        /// </param>
        /// <param name="srdlIid">
        /// The iid of the <see cref="SiteReferenceDataLibrary"/> to start from.
        /// </param>
        /// <returns>
        /// The list of <see cref="MeasurementScale"/> iids.
        /// </returns>
        private async Task<List<Guid>> GetMeasurementScaleIidsFromRdlChainAsync(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            Guid? srdlIid)
        {
            var measurementScaleIids = new List<Guid>();

            if (srdlIid == null)
            {
                return measurementScaleIids;
            }

            var availableRdls = (await this.SiteReferenceDataLibraryService
                .GetAsync(transaction, partition, null, securityContext))
                .Cast<SiteReferenceDataLibrary>().ToList();

            var next = srdlIid;

            do
            {
                var rdl = availableRdls.First(x => x.Iid == next);
                measurementScaleIids.AddRange(rdl.Scale);
                next = rdl.RequiredRdl;
            } while (next != null);

            return measurementScaleIids;
        }

        /// <summary>
        /// Gets the list of <see cref="MeasurementScale"/> containers
        /// of the reference and dependent <see cref="ScaleValueDefinition"/>s
        /// of the given <see cref="MappingToReferenceScale"/> iids.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security context used for permission checking.
        /// </param>
        /// <param name="allMeasurementScales">
        /// All <see cref="MeasurementScale"/>s available.
        /// </param>
        /// <param name="mappingToReferenceScaleIids">
        /// The given <see cref="MappingToReferenceScale"/> iids.
        /// </param>
        /// <returns>
        /// The list of <see cref="MeasurementScale"/> containers.
        /// </returns>
        private async Task<List<MeasurementScale>> GetScaleValueDefinitionContainerMeasurementScalesAsync(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            List<MeasurementScale> allMeasurementScales,
            IEnumerable<Guid> mappingToReferenceScaleIids)
        {
            var mappingToReferenceScales = (await this.MappingToReferenceScaleService
                .GetAsync(transaction, partition, mappingToReferenceScaleIids, securityContext))
                .Cast<MappingToReferenceScale>()
                .ToList();

            var scaleValueDefinitions = new List<Thing>();

            scaleValueDefinitions.AddRange((await this.ScaleValueDefinitionService
                .GetAsync(transaction, partition, mappingToReferenceScales.Select(x => x.ReferenceScaleValue), securityContext))
                .ToList());

            scaleValueDefinitions.AddRange((await this.ScaleValueDefinitionService
                .GetAsync(transaction, partition, mappingToReferenceScales.Select(x => x.DependentScaleValue), securityContext))
                .ToList());

            return scaleValueDefinitions
                .Select(svd => allMeasurementScales.First(ms => ms.ValueDefinition.Contains(svd.Iid)))
                .ToList();
        }

        /// <summary>
        /// Checks if the given <paramref name="thing"/> is cyclic.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security context used for permission checking.
        /// </param>
        /// <param name="allMeasurementScales">
        /// All <see cref="MeasurementScale"/>s available.
        /// </param>
        /// <param name="thing">
        /// The <see cref="MeasurementScale"/> instance that must not be cyclic.
        /// </param>
        /// <param name="scaleValueDefinitionContainerMeasurementScales">
        /// The <see cref="MeasurementScale"/>s that will be inspected.
        /// </param>
        private async Task CheckCycleDeepAsync(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            List<MeasurementScale> allMeasurementScales,
            MeasurementScale thing,
            IEnumerable<MeasurementScale> scaleValueDefinitionContainerMeasurementScales)
        {
            foreach (var measurementScale in scaleValueDefinitionContainerMeasurementScales)
            {
                if (measurementScale.Iid == thing.Iid)
                {
                    throw new AcyclicValidationException($"MeasurementScale {thing.Name} {thing.Iid} cannot have " +
                                                         $"a MappingToReferenceScale referencing or depending upon " +
                                                         $"a ScaleValueDefinition contained by a MeasurementScale " +
                                                         $"that leads to cyclic dependency.");
                }

                if (measurementScale.MappingToReferenceScale.Count == 0)
                {
                    continue;
                }

                var next = await this.GetScaleValueDefinitionContainerMeasurementScalesAsync(
                    transaction, partition, securityContext, allMeasurementScales, measurementScale.MappingToReferenceScale);

                await this.CheckCycleDeepAsync(
                    transaction,
                    partition,
                    securityContext,
                    allMeasurementScales,
                    thing,
                    next);
            }
        }
    }
}
