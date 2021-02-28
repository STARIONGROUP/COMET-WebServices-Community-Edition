// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequirementsGroupSideEffect.cs" company="RHEA System S.A.">
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
    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using CometServer.Helpers;
    using CometServer.Services.Authorization;

    using Npgsql;

    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// The purpose of the <see cref="RequirementsGroupSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class RequirementsGroupSideEffect : OperationSideEffect<RequirementsGroup>
    {
        /// <summary>
        /// Gets or sets the <see cref="ISiteReferenceDataLibraryService"/>
        /// </summary>
        public ISiteReferenceDataLibraryService SiteReferenceDataLibraryService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IRequirementsSpecificationService"/>
        /// </summary>
        public IRequirementsSpecificationService RequirementsSpecificationService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="RequirementsGroup"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="CDP4Common.DTO.Thing"/> that is inspected.
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
            RequirementsGroup thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            if (rawUpdateInfo.ContainsKey("Group"))
            {
                var requirementsGroupIds = (List<Guid>)rawUpdateInfo["Group"];

                // Check for itself in the list
                if (requirementsGroupIds.Contains(thing.Iid))
                {
                    throw new AcyclicValidationException(
                        string.Format("RequirementsGroup {0} {1} cannot contain itself.", thing.Name, thing.Iid));
                }

                var requirementsGroups = this.RequirementsSpecificationService
                    .GetDeep(transaction, partition, null, securityContext)
                    .Where(x => x.ClassKind == ClassKind.RequirementsGroup)
                    .Cast<RequirementsGroup>()
                    .ToList();

                // Check every RequirementsGroup that it is acyclic
                foreach (var requirementsGroupId in requirementsGroupIds)
                {
                    var groupIdsToCheck = requirementsGroupIds.Where(x => x != requirementsGroupId).ToList();
                    groupIdsToCheck.Add(thing.Iid);

                    if (!this.IsRequirementsGroupAcyclic(requirementsGroups, groupIdsToCheck, requirementsGroupId))
                    {
                        throw new AcyclicValidationException(
                            string.Format(
                                "RequirementsGroup {0} {1} cannot contain RequirementsGroup {2} that leads to cyclic dependency",
                                thing.Name,
                                thing.Iid,
                                requirementsGroupId));
                    }
                }
            }
        }

        /// <summary>
        /// Is RequirementsGroup acyclic.
        /// </summary>
        /// <param name="requirementsGroups">
        /// The requirementsGroups from the specification.
        /// </param>
        /// <param name="groupIdsToCheck">
        /// The RequirementsGroup ids to check for being acyclic.
        /// </param>
        /// <param name="requirementsGroupId">
        /// The RequirementsGroup id to check whether it contains one of the supplied RequirementsGroup in its hierarchy.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> whether applied RequirementsGroup will not lead to cyclic dependency.
        /// </returns>
        private bool IsRequirementsGroupAcyclic(
            List<RequirementsGroup> requirementsGroups,
            List<Guid> groupIdsToCheck,
            Guid requirementsGroupId)
        {
            var containedGroupsList = new List<Guid>();
            this.SetContainedGroupsList(requirementsGroups, containedGroupsList, requirementsGroupId);
            foreach (var id in groupIdsToCheck)
            {
                if (containedGroupsList.Contains(id))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Set contained groups to the supplied list.
        /// </summary>
        /// <param name="requirementsGroups">
        /// The requirementsGroups from the specification.
        /// </param>
        /// <param name="containedGroupsList">
        /// The list to set found ids to.
        /// </param>
        /// <param name="requirementsGroupId">
        /// RequirementsGroup id to set list of ids for.
        /// </param>
        private void SetContainedGroupsList(
            List<RequirementsGroup> requirementsGroups,
            List<Guid> containedGroupsList,
            Guid requirementsGroupId)
        {
            var requirementsGroup = requirementsGroups.Find(x => x.Iid == requirementsGroupId);

            foreach (var id in requirementsGroup.Group)
            {
                this.SetContainedGroupsList(requirementsGroups, containedGroupsList, id);
            }

            containedGroupsList.Add(requirementsGroupId);
        }
    }
}