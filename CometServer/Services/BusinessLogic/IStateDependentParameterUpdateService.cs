// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStateDependentParameterUpdateService.cs" company="RHEA System S.A.">
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

namespace CometServer.Services
{
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The interface for a service class that is responsible for updating state-dependent parameters
    /// </summary>
    public interface IStateDependentParameterUpdateService : IBusinessLogicService
    {
        /// <summary>
        /// Update all the relevant <see cref="ParameterBase"/>
        /// </summary>
        /// <param name="actualFiniteStateList">The updated <see cref="ActualFiniteStateList"/></param>
        /// <param name="iteration">The <see cref="Iteration"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        /// <param name="newOldActualStateMap">The map that links the new to old <see cref="ActualFiniteState"/></param>
        void UpdateAllStateDependentParameters(ActualFiniteStateList actualFiniteStateList, Iteration iteration, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, IReadOnlyDictionary<ActualFiniteState, ActualFiniteState> newOldActualStateMap);
    }
}
