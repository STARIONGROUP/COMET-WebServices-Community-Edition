﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantPermissionSideEffect.cs" company="Starion Group S.A.">
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
    using System.Threading.Tasks;

    using Authorization;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="ParticipantPermissionSideEffect"/> Side-Effect class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class ParticipantPermissionSideEffect : OperationSideEffect<ParticipantPermission>
    {
        /// <summary>
        /// Gets or sets the <see cref="IAccessRightKindValidationService"/>
        /// </summary>
        public IAccessRightKindValidationService AccessRightKindValidationService { get; set; }

        /// <summary>
        /// Gets the list of property names that are to be excluded from validation logic.
        /// </summary>
        public override IEnumerable<string> DeferPropertyValidation => ["accessRight"];

        /// <summary>
        /// Execute additional logic  before a create operation.
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
        public override async Task<bool> BeforeCreate(
            ParticipantPermission thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            await this.ValidateAccessRightKindAsync(thing);

            return true;
        }
        
        public override async Task AfterUpdate(ParticipantPermission thing, Thing container, ParticipantPermission originalThing,
            NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            await this.ValidateAccessRightKindAsync(thing);
        }

        /// <summary>
        /// Checks whether a set access right is valid for the supplied <see cref="ParticipantPermission"/>.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        private Task ValidateAccessRightKindAsync(ParticipantPermission thing)
        {
            if (!this.AccessRightKindValidationService.IsParticipantPermissionValid(thing))
            {
                throw new InvalidOperationException(
                    "The accessRight " + thing.AccessRight + " cannot be set for the class " + thing.ObjectClass + " .");
            }

            return Task.CompletedTask;
        }
    }
}
