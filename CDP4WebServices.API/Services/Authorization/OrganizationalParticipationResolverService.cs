// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationalParticipationResolverService.cs" company="RHEA System S.A.">
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

namespace CometServer.Services.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using Npgsql;

    using Definition = CDP4Common.DTO.Definition;
    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// Aids in resolving the applicable <see cref="OrganizationalParticipant" />
    /// </summary>
    public class OrganizationalParticipationResolverService : IOrganizationalParticipationResolverService
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
        /// Resolves the applicable <see cref="OrganizationalParticipant" />s needed to edit a particulat
        /// <see cref="CDP4Common.DTO.Thing" />
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="iteration">The <see cref="Iteration" /></param>
        /// <param name="thing">The <see cref="CDP4Common.DTO.Thing" /> to compute permissions for.</param>
        /// <param name="organizationalParticipantIid">The Iid of OrganizationalParticipant to validate</param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <returns>
        /// The list of the applicable <see cref="OrganizationalParticipant" />s needed to edit a particulat
        /// <see cref="CDP4Common.DTO.Thing" />
        /// </returns>
        public bool ResolveApplicableOrganizationalParticipations(NpgsqlTransaction transaction, string partition, Iteration iteration, Thing thing, Guid organizationalParticipantIid)
        {
            var securityContext = new RequestSecurityContext { ContainerReadAllowed = true };

            this.ResolveElementDefinitionTree(transaction, partition, iteration, organizationalParticipantIid, securityContext, out var elementDefinitions, out var relevantOpenDefinitions, out var fullTree);

            // only perform usage checks if certain classkinds are checked
            if (thing.ClassKind == ClassKind.ElementUsage ||
                thing.ClassKind == ClassKind.ParameterOverride ||
                thing.ClassKind == ClassKind.ParameterOverrideValueSet ||
                thing.ClassKind == ClassKind.Definition ||
                thing.ClassKind == ClassKind.Alias ||
                thing.ClassKind == ClassKind.HyperLink ||
                thing.ClassKind == ClassKind.Citation)
            {
                // add also element usages of allowed EDs and their contained items to list
                var elementUsages = this.ElementUsageService.Get(transaction, partition, elementDefinitions.SelectMany(ed => ed.ContainedElement), securityContext).Cast<ElementUsage>();

                // select relevant
                var relevatElementUsages = elementUsages.Where(eu => relevantOpenDefinitions.Select(ed => ed.Iid).Contains(eu.ElementDefinition));

                // get subtrees
                var relevantUsageSubtrees = this.ElementUsageService.GetDeep(transaction, partition, relevatElementUsages.Select(eu => eu.Iid), securityContext);

                //concat to ed trees
                fullTree = fullTree.Concat(relevantUsageSubtrees);
            }

            // if the thing we are checking exists in the trees of allowed EDs, allow it to pass
            var result = fullTree.FirstOrDefault(t => t.Iid.Equals(thing.Iid)) != null;

            return result;
        }

        /// <summary>
        /// Validates whether a create of some <see cref="Thing" />  is allowed based on Organizational Participation
        /// </summary>
        /// <param name="thing">The <see cref="Thing" /> being created.</param>
        /// <param name="container">The container of the new <see cref="Thing" /></param>
        /// <param name="securityContext">The security context</param>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <remarks>
        /// If the organizational setup prevents it an <see cref="InvalidOperationException" /> is thrown.
        /// </remarks>
        public void ValidateCreateOrganizationalParticipation(Thing thing, Thing container, ISecurityContext securityContext, NpgsqlTransaction transaction, string partition)
        {
            if (partition == "SiteDirectory")
            {
                return;
            }

            if (securityContext.Credentials != null && securityContext.Credentials.EngineeringModelSetup.OrganizationalParticipant.Any() && !securityContext.Credentials.IsDefaultOrganizationalParticipant)
            {
                if (securityContext.Credentials.OrganizationalParticipant == null)
                {
                    throw new InvalidOperationException("Not enough organizational participation priviliges.");
                }

                // check participational organization security and stop creation if containing definition is not alowed
                switch (thing.ClassKind)
                {
                    case ClassKind.ElementUsage:
                    case ClassKind.Parameter:
                    case ClassKind.ParameterGroup:
                    {
                        this.ValidateCreateElementDefinitionContainedThing(container, securityContext);
                        break;
                    }
                    case ClassKind.ParameterSubscription:
                    {
                        this.ValidateCreateParameterOrParameterOverrideContainedThing(container, securityContext, transaction, partition);
                        break;
                    }
                    case ClassKind.ParameterOverride:
                    {
                        this.ValidateCreateElementUsageContainedThing(container, securityContext, transaction, partition);
                        break;
                    }
                    case ClassKind.Alias:
                    case ClassKind.Definition:
                    case ClassKind.HyperLink:
                    {
                        this.ValidateCreateElementDefinitionOrUsageContainedThing(container, securityContext, transaction, partition);
                        break;
                    }
                    case ClassKind.Citation:
                    {
                        this.ValidateCreateDefinitionContainedThing(container, securityContext, transaction, partition);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Resolves the ElementDefinition tree
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="iteration">The <see cref="Iteration" /></param>
        /// =
        /// <param name="organizationalParticipantIid">The Iid of OrganizationalParticipant to validate</param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="securityContext">The security context</param>
        /// <param name="elementDefinitions">The list of all element definitions</param>
        /// <param name="relevantOpenDefinitions">
        /// The list of element definitions that are allowed to be seen by the organizational
        /// participant
        /// </param>
        /// <param name="fullTree">The full tree of allowed element definitions</param>
        private void ResolveElementDefinitionTree(NpgsqlTransaction transaction, string partition, Iteration iteration, Guid organizationalParticipantIid, RequestSecurityContext securityContext, out List<ElementDefinition> elementDefinitions, out List<ElementDefinition> relevantOpenDefinitions, out IEnumerable<Thing> fullTree)
        {
            // get all ED's shallow to determine relevant
            elementDefinitions = this.ElementDefinitionService.Get(transaction, partition, iteration.Element, securityContext).Cast<ElementDefinition>().ToList();

            // given a participation, select only allowed EDs
            relevantOpenDefinitions = elementDefinitions.Where(ed => ed.OrganizationalParticipant.Contains(organizationalParticipantIid)).ToList();

            // get deep expansions only of relevant Element Definitions
            fullTree = this.ElementDefinitionService.GetDeep(transaction, partition, relevantOpenDefinitions.Select(ed => ed.Iid), securityContext);
        }

        /// <summary>
        /// Validates whether a create of some <see cref="Thing" /> that is directly contained in a Parameter or ParameterOverride
        /// is allowed
        /// based on Organizational Participation
        /// </summary>
        /// <param name="container">The container of the new <see cref="Thing" /></param>
        /// <param name="securityContext">The security context</param>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        private void ValidateCreateParameterOrParameterOverrideContainedThing(Thing container, ISecurityContext securityContext, NpgsqlTransaction transaction, string partition)
        {
            switch (container.ClassKind)
            {
                case ClassKind.ParameterOverride:
                    this.ValidateCreateParameterOverrideContainedThing(container, securityContext, transaction, partition);
                    break;
                case ClassKind.Parameter:
                    this.ValidateCreateParameterContainedThing(container, securityContext, transaction, partition);
                    break;
            }
        }

        /// <summary>
        /// Validates whether a create of some <see cref="Thing" /> that is directly contained in a Definition is allowed
        /// based on Organizational Participation
        /// </summary>
        /// <param name="container">The container of the new <see cref="Thing" /></param>
        /// <param name="securityContext">The security context</param>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        private void ValidateCreateDefinitionContainedThing(Thing container, ISecurityContext securityContext, NpgsqlTransaction transaction, string partition)
        {
            if (container is Definition definition)
            {
                // the definition container must be inside the the allowed subtree. It has to pass check on being in the ED tree to begin with to prevent blocking unwanted definitions
                this.ResolveElementDefinitionTree(transaction, partition, securityContext.Credentials.Iteration, securityContext.Credentials.OrganizationalParticipant.Iid,
                    (RequestSecurityContext) securityContext, out var elementDefinitions, out var relevantOpenDefinitions, out var fullTree);

                var allElementDefinitionSubtree = this.ElementDefinitionService.GetDeep(transaction, partition, elementDefinitions.Select(ed => ed.Iid), securityContext);

                // if Definition IS in full try but NOT in allowed tree, throw an exception
                if (allElementDefinitionSubtree.FirstOrDefault(t => t.Iid.Equals(definition.Iid)) == null)
                {
                    // definition is somewhere else, ignore
                    return;
                }

                // else check in allowed tree
                if (fullTree.FirstOrDefault(t => t.Iid.Equals(definition.Iid)) == null)
                {
                    throw new InvalidOperationException("Not enough organizational participation priviliges.");
                }
            }
        }

        /// <summary>
        /// Validates whether a create of some <see cref="Thing" /> that is directly contained in an ElementUsage or
        /// ElementDefinition is allowed
        /// based on Organizational Participation
        /// </summary>
        /// <param name="container">The container of the new <see cref="Thing" /></param>
        /// <param name="securityContext">The security context</param>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        private void ValidateCreateElementDefinitionOrUsageContainedThing(Thing container, ISecurityContext securityContext, NpgsqlTransaction transaction, string partition)
        {
            switch (container.ClassKind)
            {
                case ClassKind.ElementDefinition:
                    this.ValidateCreateElementDefinitionContainedThing(container, securityContext);
                    break;
                case ClassKind.ElementUsage:
                    this.ValidateCreateElementUsageContainedThing(container, securityContext, transaction, partition);
                    break;
            }
        }

        /// <summary>
        /// Validates whether a create of some <see cref="Thing" /> that is directly contained in an ElementUsage is allowed
        /// based on Organizational Participation
        /// </summary>
        /// <param name="container">The container of the new <see cref="Thing" /></param>
        /// <param name="securityContext">The security context</param>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        private void ValidateCreateElementUsageContainedThing(Thing container, ISecurityContext securityContext, NpgsqlTransaction transaction, string partition)
        {
            if (container is ElementUsage usage)
            {
                // the usage container must ref a ED that is in the allowed tree
                this.ResolveElementDefinitionTree(transaction, partition, securityContext.Credentials.Iteration, securityContext.Credentials.OrganizationalParticipant.Iid,
                    (RequestSecurityContext) securityContext, out var elementDefinitions, out var relevantOpenDefinitions, out var fullTree);

                if (relevantOpenDefinitions.FirstOrDefault(t => t.Iid.Equals(usage.ElementDefinition)) == null)
                {
                    throw new InvalidOperationException("Not enough organizational participation priviliges.");
                }
            }
        }

        /// <summary>
        /// Validates whether a create of some <see cref="Thing" /> that is directly contained in a Parameter is allowed
        /// based on Organizational Participation
        /// </summary>
        /// <param name="container">The container of the new <see cref="Thing" /></param>
        /// <param name="securityContext">The security context</param>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        private void ValidateCreateParameterContainedThing(Thing container, ISecurityContext securityContext, NpgsqlTransaction transaction, string partition)
        {
            if (container is Parameter parameter)
            {
                // the parameter container must be in the allowed tree
                this.ResolveElementDefinitionTree(transaction, partition, securityContext.Credentials.Iteration, securityContext.Credentials.OrganizationalParticipant.Iid,
                    (RequestSecurityContext) securityContext, out var elementDefinitions, out var relevantOpenDefinitions, out var fullTree);

                if (fullTree.FirstOrDefault(t => t.Iid.Equals(parameter.Iid)) == null)
                {
                    throw new InvalidOperationException("Not enough organizational participation priviliges.");
                }
            }
        }

        /// <summary>
        /// Validates whether a create of some <see cref="Thing" /> that is directly contained in a ParameterOverride is allowed
        /// based on Organizational Participation
        /// </summary>
        /// <param name="container">The container of the new <see cref="Thing" /></param>
        /// <param name="securityContext">The security context</param>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        private void ValidateCreateParameterOverrideContainedThing(Thing container, ISecurityContext securityContext, NpgsqlTransaction transaction, string partition)
        {
            if (container is ParameterOverride parameterOverride)
            {
                // the parameter container must be in the allowed tree
                this.ResolveElementDefinitionTree(transaction, partition, securityContext.Credentials.Iteration, securityContext.Credentials.OrganizationalParticipant.Iid,
                    (RequestSecurityContext) securityContext, out var elementDefinitions, out var relevantOpenDefinitions, out var fullTree);

                // need to resolve element usage that contains it
                var allElementUsages = this.ElementDefinitionService.GetDeep(transaction, partition, elementDefinitions.Select(ed => ed.Iid), securityContext).OfType<ElementUsage>();

                var euContainer = allElementUsages.FirstOrDefault(eu => eu.ParameterOverride.Contains(parameterOverride.Iid));

                if (euContainer != null)
                {
                    this.ValidateCreateElementUsageContainedThing(euContainer, securityContext, transaction, partition);
                }
            }
        }

        /// <summary>
        /// Validates whether a create of some <see cref="Thing" /> that is directly contained in an ElementDefinition is allowed
        /// based on Organizational Participation
        /// </summary>
        /// <param name="container">The container of the new <see cref="Thing" /></param>
        /// <param name="securityContext">The security context</param>
        private void ValidateCreateElementDefinitionContainedThing(Thing container, ISecurityContext securityContext)
        {
            if (container is ElementDefinition elementDefinition)
            {
                if (!elementDefinition.OrganizationalParticipant.Contains(securityContext.Credentials.OrganizationalParticipant.Iid))
                {
                    throw new InvalidOperationException("Not enough organizational participation priviliges.");
                }
            }
        }
    }
}
