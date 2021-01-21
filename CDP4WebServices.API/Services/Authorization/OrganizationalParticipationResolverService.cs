// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationalParticipationResolverService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
// 
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski,
//                 Ahmed Abulwafa Ahmed
// 
//    This file is part of CDP4 Web Services Community Edition.
//    The CDP4 Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
// 
//    The CDP4 Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4 Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Authorization
{
    using System;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using Npgsql;

    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// Aids in resolving the applicable <see cref="OrganizationalParticipant"/>
    /// </summary>
    public class OrganizationalParticipationResolverService : IOrganizationalParticipationResolverService
    {
        /// <summary>
        /// Gets or sets the <see cref="IElementDefinitionService"/>
        /// </summary>
        public IElementDefinitionService ElementDefinitionService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IElementUsageService"/>
        /// </summary>
        public IElementUsageService ElementUsageService { get; set; }

        /// <summary>
        /// Resolves the applicable <see cref="OrganizationalParticipant"/>s needed to edit a particulat <see cref="CDP4Common.DTO.Thing"/>
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="iteration">The <see cref="Iteration"/></param>
        /// <param name="thing">The <see cref="CDP4Common.DTO.Thing"/> to compute permissions for.</param>
        /// <param name="organizationalParticipantIid">The Iid of OrganizationalParticipant to validate</param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <returns>The list of the applicable <see cref="OrganizationalParticipant"/>s needed to edit a particulat <see cref="CDP4Common.DTO.Thing"/></returns>
        public bool ResolveApplicableOrganizationalParticipations(NpgsqlTransaction transaction, string partition, Iteration iteration, Thing thing, Guid organizationalParticipantIid)
        {
            var securityContext = new RequestSecurityContext { ContainerReadAllowed = true };

            // get all ED's shallow to determine relevant
            var elementDefinitions = this.ElementDefinitionService.Get(transaction, partition, iteration.Element, securityContext).Cast<ElementDefinition>().ToList();

            // given a participation, select only allowed EDs
            var relevantOpenDefinitions = elementDefinitions.Where(ed => ed.OrganizationalParticipant.Contains(organizationalParticipantIid)).ToList();

            // get deep expansions only of relevant Element Definitions
            var fullTree = this.ElementDefinitionService.GetDeep(transaction, partition, relevantOpenDefinitions.Select(ed => ed.Iid), securityContext);

            // add also element usages of allowed EDs and their contained items to list
            var elementUsages = this.ElementUsageService.Get(transaction, partition, elementDefinitions.SelectMany(ed => ed.ContainedElement), securityContext).Cast<ElementUsage>();

            // only perform usage checks if certain classkinds are checked
            if (thing.ClassKind == ClassKind.ElementUsage ||
                thing.ClassKind == ClassKind.ParameterOverride ||
                thing.ClassKind == ClassKind.ParameterOverrideValueSet ||
                thing.ClassKind == ClassKind.Definition ||
                thing.ClassKind == ClassKind.Alias ||
                thing.ClassKind == ClassKind.HyperLink ||
                thing.ClassKind == ClassKind.Citation)
            {
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
    }
}
