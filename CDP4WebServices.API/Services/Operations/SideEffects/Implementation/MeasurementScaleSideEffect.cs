// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeasurementScaleSideEffect.cs" company="RHEA System S.A.">
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

namespace CometServer.Services.Operations.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common;
    using CDP4Common.DTO;

    using CometServer.Helpers;
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
        public override void BeforeUpdate(
            MeasurementScale thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            if (!rawUpdateInfo.ContainsKey("MappingToReferenceScale"))
            {
                return;
            }

            var mappingToReferenceScaleIids = (List<Guid>)rawUpdateInfo["MappingToReferenceScale"];
            var referenceDataLibrary = (ReferenceDataLibrary)container;

            // Check that all referenced MeasurementScales are from the same RDL chain
            var availableMeasurementScaleIids = this.GetMeasurementScaleIidsFromRdlChain(
                transaction,
                partition,
                securityContext,
                referenceDataLibrary.RequiredRdl);

            availableMeasurementScaleIids.AddRange(referenceDataLibrary.Scale);

            var allMeasurementScales = this.MeasurementScaleService
                .Get(transaction, partition, null, securityContext)
                .Cast<MeasurementScale>()
                .ToList();

            var scaleValueDefinitionContainerMeasurementScales = this.GetScaleValueDefinitionContainerMeasurementScales(
                transaction, partition, securityContext, allMeasurementScales, mappingToReferenceScaleIids);

            if (scaleValueDefinitionContainerMeasurementScales.Any(x => !availableMeasurementScaleIids.Contains(x.Iid)))
            {
                throw new AcyclicValidationException($"MeasurementScale {thing.Name} {thing.Iid} cannot have " +
                                                     $"a MappingToReferenceScale referencing or depending upon " +
                                                     $"a ScaleValueDefinition contained by a MeasurementScale " +
                                                     $"from outside the current RDL chain.");
            }
            
            this.CheckCycleDeep(
                transaction,
                partition,
                securityContext,
                allMeasurementScales,
                thing,
                scaleValueDefinitionContainerMeasurementScales);
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
        private List<Guid> GetMeasurementScaleIidsFromRdlChain(
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

            var availableRdls = this.SiteReferenceDataLibraryService
                .Get(transaction, partition, null, securityContext)
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
        private List<MeasurementScale> GetScaleValueDefinitionContainerMeasurementScales(
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            List<MeasurementScale> allMeasurementScales,
            IEnumerable<Guid> mappingToReferenceScaleIids)
        {
            var mappingToReferenceScales = this.MappingToReferenceScaleService
                .Get(transaction, partition, mappingToReferenceScaleIids, securityContext)
                .Cast<MappingToReferenceScale>()
                .ToList();

            var scaleValueDefinitions = new List<Thing>();

            scaleValueDefinitions.AddRange(this.ScaleValueDefinitionService
                .Get(transaction, partition, mappingToReferenceScales.Select(x => x.ReferenceScaleValue), securityContext)
                .ToList());

            scaleValueDefinitions.AddRange(this.ScaleValueDefinitionService
                .Get(transaction, partition, mappingToReferenceScales.Select(x => x.DependentScaleValue), securityContext)
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
        private void CheckCycleDeep(
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

                var next = this.GetScaleValueDefinitionContainerMeasurementScales(
                    transaction, partition, securityContext, allMeasurementScales, measurementScale.MappingToReferenceScale);

                this.CheckCycleDeep(
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
