// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterGroupSideEffect.cs" company="RHEA System S.A.">
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
    /// The purpose of the <see cref="ParameterGroupSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class ParameterGroupSideEffect : OperationSideEffect<ParameterGroup>
    {
        /// <summary>
        /// Gets or sets the <see cref="IParameterService"/>
        /// </summary>
        public IParameterService ParameterService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IParameterGroupService"/>
        /// </summary>
        public IParameterGroupService ParameterGroupService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before a delete operation.
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
        public override void BeforeDelete(
            ParameterGroup thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            // Get all Parameters that reference the given ParameterGroup and set references to null
            var parameters = this.ParameterService.Get(transaction, partition, null, securityContext)
                .OfType<Parameter>().Where(x => x.Group == thing.Iid).ToList();
            foreach (var parameter in parameters)
            {
                parameter.Group = null;
                this.ParameterService.UpdateConcept(transaction, partition, parameter, container);
            }
        }

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
        public override bool BeforeCreate(
            ParameterGroup thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            this.OrganizationalParticipationResolverService.ValidateCreateOrganizationalParticipation(thing, container, securityContext, transaction, partition);

            if (thing.ContainingGroup != null)
            {
                this.ValidateContainingGroup(
                    thing,
                    container,
                    transaction,
                    partition,
                    securityContext,
                    (Guid)thing.ContainingGroup);
            }

            return true;
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="ParameterGroup"/> instance that will be inspected.
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
        public override void BeforeUpdate(
            ParameterGroup thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            if (rawUpdateInfo.ContainsKey("ContainingGroup"))
            {
                var containingGroupId = rawUpdateInfo["ContainingGroup"];
                if (containingGroupId != null && Guid.TryParse(containingGroupId.ToString(), out var containingGroupIdGuid))
                {
                    this.ValidateContainingGroup(
                        thing,
                        container,
                        transaction,
                        partition,
                        securityContext,
                        containingGroupIdGuid);
                }
            }
        }

        /// <summary>
        /// Validate containing Group for being acyclic.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="ParameterGroup"/> instance that will be inspected.
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
        /// <param name="containingGroupId">
        /// The containing group id to check for being acyclic
        /// </param>
        public void ValidateContainingGroup(
            ParameterGroup thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            Guid containingGroupId)
        {
            // Check for itself
            if (containingGroupId == thing.Iid)
            {
                throw new AcyclicValidationException(
                    string.Format(
                        "ParameterGroup {0} {1} cannot have itself as a containing ParameterGroup.",
                        thing.Name,
                        thing.Iid));
            }

            // Check that containing group is from the same element definition
            if (!((ElementDefinition)container).ParameterGroup.Contains(containingGroupId))
            {
                throw new AcyclicValidationException(
                    string.Format(
                        "ParameterGroup {0} {1} cannot have a ParameterGroup from outside the current elementDefinition.",
                        thing.Name,
                        thing.Iid));
            }

            // Get all parameter groups from the container
            var parameterGroups = this.ParameterGroupService.Get(
                transaction,
                partition,
                ((ElementDefinition)container).ParameterGroup,
                securityContext).Cast<ParameterGroup>().ToList();

            // Check whether containing parameter group is acyclic
            if (!this.IsParameterGroupAcyclic(parameterGroups, containingGroupId, thing.Iid))
            {
                throw new AcyclicValidationException(
                    string.Format(
                        "ParameterGroup {0} {1} cannot have a containing ParameterGroup {2} that leads to cyclic dependency",
                        thing.Name,
                        thing.Iid,
                        containingGroupId));
            }
        }

        /// <summary>
        /// Is containing parameter group acyclic.
        /// </summary>
        /// <param name="parameterGroups">
        /// The parameterGroups from the element defenition.
        /// </param>
        /// <param name="containingGroupId">
        /// The containing parameter group to check for being acyclic.
        /// </param>
        /// <param name="parameterGroupId">
        /// The parameter group id to set a containing parameter group to.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> whether a containing parameter group will not lead to cyclic dependency.
        /// </returns>
        private bool IsParameterGroupAcyclic(
            List<ParameterGroup> parameterGroups,
            Guid containingGroupId,
            Guid parameterGroupId)
        {
            Guid? nextContainingGroupId = containingGroupId;

            while (nextContainingGroupId != null)
            {
                if (nextContainingGroupId == parameterGroupId)
                {
                    return false;
                }

                nextContainingGroupId =
                    parameterGroups.FirstOrDefault(x => x.Iid == nextContainingGroupId)?.ContainingGroup;
            }

            return true;
        }
    }
}
