// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementDefinitionSideEffect.cs" company="RHEA System S.A.">
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

    using CDP4Orm.Dao;

    using CometServer.Helpers;
    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="ElementDefinitionSideEffect" /> class is to execute additional logic before and after
    /// a specific operation is performed.
    /// </summary>
    public sealed class ElementDefinitionSideEffect : OperationSideEffect<ElementDefinition>
    {
        /// <summary>
        /// Gets or sets the <see cref="IElementDefinitionService" />
        /// </summary>
        public IElementDefinitionService ElementDefinitionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IElementUsageService" />
        /// </summary>
        public IElementUsageService ElementUsageService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IIterationService" />
        /// </summary>
        public IIterationService IterationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IIterationSetupService" />
        /// </summary>
        public IIterationSetupService IterationSetupService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IEngineeringModelService" />
        /// </summary>
        public IEngineeringModelService EngineeringModelService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IEngineeringModelSetupService" />
        /// </summary>
        public IEngineeringModelSetupService EngineeringModelSetupService { get; set; }

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
        /// <returns>
        /// Returns true if the create operation may continue, otherwise it shall be skipped.
        /// </returns>
        public override bool BeforeCreate(ElementDefinition thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            // inject the organizational participant of the creator to the ED before creating
            if (securityContext.Credentials != null && securityContext.Credentials.EngineeringModelSetup.OrganizationalParticipant.Any() && !securityContext.Credentials.IsDefaultOrganizationalParticipant)
            {
                if (!thing.OrganizationalParticipant.Contains(securityContext.Credentials.OrganizationalParticipant.Iid))
                {
                    thing.OrganizationalParticipant.Add(securityContext.Credentials.OrganizationalParticipant.Iid);
                }
            }

            return true;
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before a delete operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="ElementDefinition" /> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing" /> that is inspected.
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
        public override void BeforeDelete(ElementDefinition thing, Thing container, NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            if (container is Iteration iteration)
            {
                if (!(iteration.TopElement?.Equals(thing.Iid) ?? false))
                {
                    return;
                }

                var baseErrorString =
                    $"Could not set {nameof(Iteration)}.{nameof(Iteration.TopElement)} to null.";

                var iterationSetup = this.IterationSetupService.GetShallow(transaction,
                    Utils.SiteDirectoryPartition,
                    new[] { iteration.IterationSetup }, securityContext).Cast<IterationSetup>().SingleOrDefault();

                if (iterationSetup == null)
                {
                    throw new KeyNotFoundException(
                        $"{baseErrorString}\n{nameof(IterationSetup)} with iid {iteration.IterationSetup} could not be found.");
                }

                var engineeringModelSetup = this.EngineeringModelSetupService
                    .GetShallow(transaction, Utils.SiteDirectoryPartition, null, securityContext)
                    .Cast<EngineeringModelSetup>()
                    .SingleOrDefault(ms => ms.IterationSetup.Contains(iterationSetup.Iid));

                if (engineeringModelSetup == null)
                {
                    throw new KeyNotFoundException(
                        $"{baseErrorString}\n{nameof(IterationSetup)} with iid {iteration.IterationSetup}) could not be found in any {nameof(EngineeringModelSetup)}");
                }

                var engineeringModelPartition =
                    this.RequestUtils.GetEngineeringModelPartitionString(engineeringModelSetup.EngineeringModelIid);

                var updatedIteration = this.IterationService
                    .GetShallow(transaction, engineeringModelPartition, new[] { iteration.Iid }, securityContext)
                    .Cast<Iteration>()
                    .SingleOrDefault();

                if (updatedIteration == null)
                {
                    throw new KeyNotFoundException(
                        $"{baseErrorString}\n{nameof(Iteration)} with iid {iteration.Iid}) could not be found.");
                }

                if (!(updatedIteration.TopElement?.Equals(thing.Iid) ?? false))
                {
                    return;
                }

                updatedIteration.TopElement = null;

                var engineeringModel = this.EngineeringModelService
                    .GetShallow(transaction, engineeringModelPartition,
                        new[] { engineeringModelSetup.EngineeringModelIid }, securityContext).Cast<EngineeringModel>()
                    .SingleOrDefault();

                if (engineeringModel == null)
                {
                    throw new KeyNotFoundException(
                        $"{baseErrorString}\n{nameof(EngineeringModelSetup)} with iid {engineeringModelSetup.EngineeringModelIid}) could not be found in any {nameof(EngineeringModel)}");
                }

                this.IterationService.UpdateConcept(transaction, engineeringModelPartition, updatedIteration,
                    engineeringModel);
            }
            else
            {
                if (container == null)
                {
                    throw new ArgumentNullException(nameof(container));
                }

                throw new ArgumentException($"(Type:{container.GetType().Name}) should be of type {nameof(Iteration)}.",
                    nameof(container));
            }
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="ElementDefinition" /> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing" /> that is inspected.
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
        /// The <see cref="ClasslessDTO" /> instance only contains values for properties that are to be updated.
        /// It is important to note that this variable is not to be changed likely as it can/will change the operation
        /// processor outcome.
        /// </param>
        public override void BeforeUpdate(
            ElementDefinition thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            if (rawUpdateInfo.ContainsKey("ContainedElement"))
            {
                var containedElementsId = (IEnumerable<Guid>) rawUpdateInfo["ContainedElement"];

                var elementDefinitions = this.ElementDefinitionService
                    .Get(transaction, partition, null, securityContext).Cast<ElementDefinition>().ToList();

                var elementUsages = this.ElementUsageService.Get(transaction, partition, null, securityContext)
                    .Cast<ElementUsage>().ToList();

                // Check every contained element that it is acyclic
                foreach (var containedElementId in containedElementsId)
                {
                    if (!this.IsElementDefinitionAcyclic(
                        elementDefinitions,
                        elementUsages,
                        containedElementId,
                        thing.Iid))
                    {
                        throw new AcyclicValidationException(
                            $"{nameof(ElementDefinition)} {thing.Name} {thing.Iid} cannot have an {nameof(ElementUsage)} {containedElementId} that leads to cyclic dependency");
                    }
                }
            }
        }

        /// <summary>
        /// Is element definition acyclic.
        /// </summary>
        /// <param name="elementDefinitions">
        /// Available element definitions from iteration.
        /// </param>
        /// <param name="elementUsages">
        /// Available element usages from iteration.
        /// </param>
        /// <param name="containedElementId">
        /// The element usage id to check for being acyclic.
        /// </param>
        /// <param name="elementDefinitionId">
        /// The element definition id to set element usage to.
        /// </param>
        /// <returns>
        /// The <see cref="bool" /> whether applied element usage will not lead to cyclic dependency.
        /// </returns>
        private bool IsElementDefinitionAcyclic(
            List<ElementDefinition> elementDefinitions,
            List<ElementUsage> elementUsages,
            Guid containedElementId,
            Guid elementDefinitionId)
        {
            var elementUsage = elementUsages.Find(x => x.Iid == containedElementId);

            return !(elementUsage.ElementDefinition == elementDefinitionId ||
                     !this.IsReferencedElementDefinitionAcyclic(
                         elementDefinitions,
                         elementUsages,
                         elementUsage.ElementDefinition,
                         elementDefinitionId));
        }

        /// <summary>
        /// Is referenced element definition acyclic.
        /// </summary>
        /// <param name="elementDefinitions">
        /// Available element definitions from iteration.
        /// </param>
        /// <param name="elementUsages">
        /// Available element usages from iteration.
        /// </param>
        /// <param name="referencedElementDefinitionId">
        /// The referenced element definition id to check for cyclic dependency.
        /// </param>
        /// <param name="elementDefinitionId">
        /// The root element definition id.
        /// </param>
        /// <returns>
        /// The <see cref="bool" /> whether referenced element definition will not lead to cyclic dependency.
        /// </returns>
        private bool IsReferencedElementDefinitionAcyclic(
            List<ElementDefinition> elementDefinitions,
            List<ElementUsage> elementUsages,
            Guid referencedElementDefinitionId,
            Guid elementDefinitionId)
        {
            var elementDefinition = elementDefinitions.Find(x => x.Iid == referencedElementDefinitionId);

            foreach (var elementUsageId in elementDefinition.ContainedElement)
            {
                var elementUsage = elementUsages.Find(x => x.Iid == elementUsageId);
                if (elementUsage.ElementDefinition != elementDefinitionId)
                {
                    if (!this.IsReferencedElementDefinitionAcyclic(
                        elementDefinitions,
                        elementUsages,
                        elementUsage.ElementDefinition,
                        elementDefinitionId))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}