// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationalParticipationResolverService.cs" company="Starion Group S.A.">
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

namespace CometServer.Authorization
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using CometServer.Services;
    using CometServer.Services.Authorization;

    using Microsoft.Extensions.Logging;

    using Npgsql;

    using Definition = CDP4Common.DTO.Definition;
    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// Aids in resolving the applicable <see cref="OrganizationalParticipant" />
    /// </summary>
    public class OrganizationalParticipationResolverService : IOrganizationalParticipationResolverService
    {
        /// <summary>
        /// Gets or sets the (injected) <see cref="ILogger"/>
        /// </summary>
        public ILogger<OrganizationalParticipationResolverService> Logger { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IElementDefinitionService" />
        /// </summary>
        public IElementDefinitionService ElementDefinitionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IElementUsageService" />
        /// </summary>
        public IElementUsageService ElementUsageService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="ICredentialsService"/>
        /// </summary>
        public ICredentialsService CredentialsService { get; set; }

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
        public async Task<bool> ResolveApplicableOrganizationalParticipationsAsync(NpgsqlTransaction transaction, string partition, Iteration iteration, Thing thing, Guid organizationalParticipantIid)
        {
            this.Logger.LogTrace("Resolve OrganizationalParticipation {Iteration}.{Thing} for Participant {OrganizationalParticipantIid}", iteration, thing, organizationalParticipantIid);

            var securityContext = new RequestSecurityContext { ContainerReadAllowed = true };

            var resolvedElementDefinitionTree = await this.ResolveElementDefinitionTreeAsync(transaction, partition, iteration, organizationalParticipantIid, securityContext);

            var fullTree = resolvedElementDefinitionTree.FullTree;

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
                var elementUsages = (await this.ElementUsageService.GetAsync(transaction, partition, resolvedElementDefinitionTree.ElementDefinitions.SelectMany(ed => ed.ContainedElement), securityContext)).Cast<ElementUsage>();

                // select relevant
                var relevatElementUsages = elementUsages.Where(eu => resolvedElementDefinitionTree.RelevantOpenDefinitions.Select(ed => ed.Iid).Contains(eu.ElementDefinition));

                // get subtrees
                var relevantUsageSubtrees = await this.ElementUsageService.GetDeepAsync(transaction, partition, relevatElementUsages.Select(eu => eu.Iid), securityContext);

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
        public async Task ValidateCreateOrganizationalParticipationAsync(Thing thing, Thing container, ISecurityContext securityContext, NpgsqlTransaction transaction, string partition)
        {
            this.Logger.LogTrace("Validate Create OrganizationalParticipation {Container}.{Thing}", container, thing);

            if (partition == "SiteDirectory")
            {
                return;
            }

            if (this.CredentialsService.Credentials != null && this.CredentialsService.Credentials.EngineeringModelSetup.OrganizationalParticipant.Count != 0 && !this.CredentialsService.Credentials.IsDefaultOrganizationalParticipant)
            {
                if (this.CredentialsService.Credentials.OrganizationalParticipant == null)
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
                        this.ValidateCreateElementDefinitionContainedThing(container);
                        break;
                    }
                    case ClassKind.ParameterSubscription:
                    {
                        await this.ValidateCreateParameterOrParameterOverrideContainedThingAsync(container, securityContext, transaction, partition);
                        break;
                    }
                    case ClassKind.ParameterOverride:
                    {
                        await this.ValidateCreateElementUsageContainedThingAsync(container, securityContext, transaction, partition);
                        break;
                    }
                    case ClassKind.Alias:
                    case ClassKind.Definition:
                    case ClassKind.HyperLink:
                    {
                        await this.ValidateCreateElementDefinitionOrUsageContainedThingAsync(container, securityContext, transaction, partition);
                        break;
                    }
                    case ClassKind.Citation:
                    {
                        await this.ValidateCreateDefinitionContainedThingAsync(container, securityContext, transaction, partition);
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
        private async Task<ResolveElementDefinitionTreeResult> ResolveElementDefinitionTreeAsync(NpgsqlTransaction transaction, string partition, Iteration iteration, Guid organizationalParticipantIid, RequestSecurityContext securityContext)
        {
            // get all ED's shallow to determine relevant
            var elementDefinitions = (await this.ElementDefinitionService.GetAsync(transaction, partition, iteration.Element, securityContext)).Cast<ElementDefinition>().ToList();

            // given a participation, select only allowed EDs
            var relevantOpenDefinitions = elementDefinitions.Where(ed => ed.OrganizationalParticipant.Contains(organizationalParticipantIid)).ToList();

            // get deep expansions only of relevant Element Definitions
            var fullTree = await this.ElementDefinitionService.GetDeepAsync(transaction, partition, relevantOpenDefinitions.Select(ed => ed.Iid), securityContext);

            return new ResolveElementDefinitionTreeResult(elementDefinitions, relevantOpenDefinitions, fullTree);
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
        private async Task ValidateCreateParameterOrParameterOverrideContainedThingAsync(Thing container, ISecurityContext securityContext, NpgsqlTransaction transaction, string partition)
        {
            switch (container.ClassKind)
            {
                case ClassKind.ParameterOverride:
                    await this.ValidateCreateParameterOverrideContainedThingAsync(container, securityContext, transaction, partition);
                    break;
                case ClassKind.Parameter:
                    await this.ValidateCreateParameterContainedThingAsync(container, securityContext, transaction, partition);
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
        private async Task ValidateCreateDefinitionContainedThingAsync(Thing container, ISecurityContext securityContext, NpgsqlTransaction transaction, string partition)
        {
            if (container is Definition definition)
            {
                // the definition container must be inside the the allowed subtree. It has to pass check on being in the ED tree to begin with to prevent blocking unwanted definitions
                var resolvedElementDefinitionTree = await this.ResolveElementDefinitionTreeAsync(transaction, partition, this.CredentialsService.Credentials.Iteration, this.CredentialsService.Credentials.OrganizationalParticipant.Iid,
                    (RequestSecurityContext) securityContext);

                var allElementDefinitionSubtree = await this.ElementDefinitionService.GetDeepAsync(transaction, partition, resolvedElementDefinitionTree.ElementDefinitions.Select(ed => ed.Iid), securityContext);

                // if Definition IS in full try but NOT in allowed tree, throw an exception
                if (allElementDefinitionSubtree.FirstOrDefault(t => t.Iid.Equals(definition.Iid)) == null)
                {
                    // definition is somewhere else, ignore
                    return;
                }

                // else check in allowed tree
                if (resolvedElementDefinitionTree.FullTree.FirstOrDefault(t => t.Iid.Equals(definition.Iid)) == null)
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
        private async Task ValidateCreateElementDefinitionOrUsageContainedThingAsync(Thing container, ISecurityContext securityContext, NpgsqlTransaction transaction, string partition)
        {
            switch (container.ClassKind)
            {
                case ClassKind.ElementDefinition:
                    this.ValidateCreateElementDefinitionContainedThing(container);
                    break;
                case ClassKind.ElementUsage:
                    await this.ValidateCreateElementUsageContainedThingAsync(container, securityContext, transaction, partition);
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
        private async Task ValidateCreateElementUsageContainedThingAsync(Thing container, ISecurityContext securityContext, NpgsqlTransaction transaction, string partition)
        {
            if (container is ElementUsage usage)
            {
                // the usage container must ref a ED that is in the allowed tree
                var resolvedElementDefinitionTree = await this.ResolveElementDefinitionTreeAsync(transaction, partition, this.CredentialsService.Credentials.Iteration, this.CredentialsService.Credentials.OrganizationalParticipant.Iid,
                    (RequestSecurityContext) securityContext);

                if (resolvedElementDefinitionTree.RelevantOpenDefinitions.FirstOrDefault(t => t.Iid.Equals(usage.ElementDefinition)) == null)
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
        private async Task ValidateCreateParameterContainedThingAsync(Thing container, ISecurityContext securityContext, NpgsqlTransaction transaction, string partition)
        {
            if (container is Parameter parameter)
            {
                // the parameter container must be in the allowed tree
                var resolvedElementDefinitionTree = await this.ResolveElementDefinitionTreeAsync(transaction, partition, this.CredentialsService.Credentials.Iteration, this.CredentialsService.Credentials.OrganizationalParticipant.Iid,
                    (RequestSecurityContext) securityContext);

                if (resolvedElementDefinitionTree.FullTree.FirstOrDefault(t => t.Iid.Equals(parameter.Iid)) == null)
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
        private async Task ValidateCreateParameterOverrideContainedThingAsync(Thing container, ISecurityContext securityContext, NpgsqlTransaction transaction, string partition)
        {
            if (container is ParameterOverride parameterOverride)
            {
                // the parameter container must be in the allowed tree
                var resolvedElementDefinitionTree = await this.ResolveElementDefinitionTreeAsync(transaction, partition, this.CredentialsService.Credentials.Iteration, this.CredentialsService.Credentials.OrganizationalParticipant.Iid,
                    (RequestSecurityContext) securityContext);

                // need to resolve element usage that contains it
                var allElementUsages = (await this.ElementDefinitionService.GetDeepAsync(transaction, partition, resolvedElementDefinitionTree.ElementDefinitions.Select(ed => ed.Iid), securityContext)).OfType<ElementUsage>();

                var euContainer = allElementUsages.FirstOrDefault(eu => eu.ParameterOverride.Contains(parameterOverride.Iid));

                if (euContainer != null)
                {
                    await this.ValidateCreateElementUsageContainedThingAsync(euContainer, securityContext, transaction, partition);
                }
            }
        }

        /// <summary>
        /// Validates whether a create of some <see cref="Thing" /> that is directly contained in an ElementDefinition is allowed
        /// based on Organizational Participation
        /// </summary>
        /// <param name="container">The container of the new <see cref="Thing" /></param>
        private void ValidateCreateElementDefinitionContainedThing(Thing container)
        {
            if (container is ElementDefinition elementDefinition)
            {
                if (!elementDefinition.OrganizationalParticipant.Contains(this.CredentialsService.Credentials.OrganizationalParticipant.Iid))
                {
                    throw new InvalidOperationException("Not enough organizational participation priviliges.");
                }
            }
        }
    }
}
