// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagramCanvasSideEffect.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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
    using System.Security;

    using CDP4Common;
    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    using CometServer.Services.Authorization;

    using Npgsql;

    using Thing = CDP4Common.DTO.Thing;

    public class DiagramCanvasSideEffect : OperationSideEffect<DiagramCanvas>
    {
        /// <summary>
        /// Gets or sets the <see cref="IDiagramCanvasService"/>.
        /// </summary>
        public IDiagramCanvasService DiagramCanvasService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="DiagramCanvas"/> instance that will be inspected.
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
        public override void BeforeUpdate(DiagramCanvas thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, ClasslessDTO rawUpdateInfo)
        {
            base.BeforeUpdate(thing, container, transaction, partition, securityContext, rawUpdateInfo);

            var isHidden = thing.IsHidden;
            var lockedBy = thing.LockedBy;

            // Pre check correctness of state after updates
            if (rawUpdateInfo.TryGetValue(nameof(thing.IsHidden), out var updatedIsHidden))
            {
                isHidden = (bool)updatedIsHidden;
            }

            if (rawUpdateInfo.TryGetValue(nameof(thing.LockedBy), out var updatedLockedBy))
            {
                lockedBy = (Guid?)updatedLockedBy;
            }

            this.CheckIsHiddenAndLockedBy(thing.ClassKind, isHidden, lockedBy);
            
            this.HasWriteAccess(thing, transaction, partition);
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before a create operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="DiagramCanvas"/> instance that will be inspected.
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
        public override bool BeforeCreate(DiagramCanvas thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            var result = base.BeforeCreate(thing, container, transaction, partition, securityContext);

            this.CheckIsHiddenAndLockedBy(thing.ClassKind, thing.IsHidden, thing.LockedBy);

            return result;
        }

        /// <summary>
        /// Checks business logic based on combination of IsHidden and LockedBy parameters
        /// </summary>
        /// <param name="classKind">The value of <see cref="DiagramCanvas.ClassKind"/></param>
        /// <param name="isHidden">The value of <see cref="DiagramCanvas.IsHidden"/></param>
        /// <param name="lockedBy">The <see cref="DiagramCanvas.LockedBy"/></param>
        /// <exception cref="Cdp4ModelValidationException">throws when business logic check fails</exception>
        private void CheckIsHiddenAndLockedBy(ClassKind classKind, bool isHidden, Guid? lockedBy)
        {
            if (classKind == ClassKind.DiagramCanvas)
            {
                // Not for sub types
                if (isHidden && lockedBy == null)
                {
                    throw new Cdp4ModelValidationException($"{nameof(DiagramCanvas)} cannot be set as hidden without also being locked by a specific {nameof(Person)}.");
                }
            }
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before a delete operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="DiagramCanvas"/> instance that will be inspected.
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
        public override void BeforeDelete(DiagramCanvas thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            base.BeforeDelete(thing, container, transaction, partition, securityContext);
            this.HasWriteAccess(thing, transaction, partition);
        }

        /// <summary>
        /// Checks the <see cref="DiagramCanvas"/> security
        /// </summary>
        /// <param name="thing">
        /// The instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        public void HasWriteAccess(DiagramCanvas thing, NpgsqlTransaction transaction, string partition)
        {
            if (!this.DiagramCanvasService.HasWriteAccess(
                    thing,
                    transaction,
                    partition))
            {
                throw new SecurityException($"User is not allowed to write to {nameof(DiagramCanvas)} '{thing.Name}'");
            }
        }
    }
}
