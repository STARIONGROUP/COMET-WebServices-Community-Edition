﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrganizationalParticipationResolverService.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
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
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// Aids in resolving the applicable <see cref="OrganizationalParticipant" />
    /// </summary>
    public interface IOrganizationalParticipationResolverService
    {
        /// <summary>
        /// Resolves the applicable <see cref="OrganizationalParticipant" />s needed to edit a particulat <see cref="Thing" />
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="iteration">The <see cref="Iteration" /></param>
        /// <param name="thing">The <see cref="Thing" /> to compute permissions for.</param>
        /// <param name="organizationalParticipantIid">The Iid of OrganizationalParticipant to validate</param>
        /// <returns>
        /// The list of the applicable <see cref="OrganizationalParticipant" />s needed to edit a particulat
        /// <see cref="Thing" />
        /// </returns>
        Task<bool> ResolveApplicableOrganizationalParticipationsAsync(NpgsqlTransaction transaction, string partition, Iteration iteration, Thing thing, Guid organizationalParticipantIid);

        /// <summary>
        /// Validates whether a create of some <see cref="Thing"/>  is allowed based on Organizational Participation
        /// </summary>
        /// <param name="thing">The <see cref="Thing"/> being created.</param>
        /// <param name="container">The container of the new <see cref="Thing"/></param>
        /// <param name="securityContext">The security context</param>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        Task ValidateCreateOrganizationalParticipationAsync(Thing thing, Thing container, ISecurityContext securityContext, NpgsqlTransaction transaction, string partition);
    }
}
