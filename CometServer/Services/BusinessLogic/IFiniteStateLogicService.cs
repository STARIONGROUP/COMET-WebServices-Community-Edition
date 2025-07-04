﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFiniteStateLogicService.cs" company="Starion Group S.A.">
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

namespace CometServer.Services
{
    using System.Threading.Tasks;

    using Authorization;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="IFiniteStateLogicService"/> is to provide services for <see cref="ActualFiniteState"/>  and <see cref="PossibleFiniteState"/> instances
    /// </summary>
    public interface IFiniteStateLogicService : IBusinessLogicService
    {
        /// <summary>
        /// Get and update all relevant <see cref="CDP4Common.DTO.ActualFiniteStateList"/>
        /// </summary>
        /// <param name="possibleFiniteStateList">
        /// The container instance of the <see cref="PossibleFiniteStateList"/> that is inspected.
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
        Task UpdateAllRelevantActualFiniteStateListAsync(PossibleFiniteStateList possibleFiniteStateList, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext);

        /// <summary>
        /// Update a <see cref="ActualFiniteStateList"/> by allocating new <see cref="ActualFiniteState"/>
        /// </summary>
        /// <param name="actualFiniteStateList">The <see cref="ActualFiniteStateList"/></param>
        /// <param name="iteration">The <see cref="Iteration"/></param>
        /// <param name="transaction">The current <see cref="NpgsqlTransaction"/></param>
        /// <param name="partition">The partition</param>
        /// <param name="securityContext">The <see cref="ISecurityContext"/></param>
        Task UpdateActualFinisteStateListAsync(ActualFiniteStateList actualFiniteStateList, Iteration iteration, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext);
    }
}
