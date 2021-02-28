// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActualFiniteStateListService.cs" company="RHEA System S.A.">
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

    using Authorization;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// The ActualFiniteStateListService Service Interface which uses the ORM layer to interact with the data model.
    /// </summary>
    public partial interface IActualFiniteStateListService
    {
        /// <summary>
        /// Gets the default <see cref="ActualFiniteState"/> for <paramref name="actualList"/>
        /// </summary>
        /// <param name="actualList">The <see cref="ActualFiniteStateList"/> to get the default for</param>
        /// <param name="actualListStates">A list of <see cref="ActualFiniteState"/> defining the <paramref name="actualList"/></param>
        /// <param name="possibleLists">A list of <see cref="PossibleFiniteStateList"/> defining the <paramref name="actualList"/></param>
        /// <returns>The default <see cref="ActualFiniteState"/> if any, null otherwise</returns>
        ActualFiniteState GetDefaultState(ActualFiniteStateList actualList, IReadOnlyList<ActualFiniteState> actualListStates, IReadOnlyList<PossibleFiniteStateList> possibleLists);

        /// <summary>
        /// Gets the default <see cref="ActualFiniteState"/> for <paramref name="actualList"/>
        /// </summary>
        /// <param name="actualList">The <see cref="ActualFiniteStateList"/> to get the default for</param>
        /// <param name="actualListStates">A list of <see cref="ActualFiniteState"/> defining the <paramref name="actualList"/></param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security-context</param>
        /// <param name="transaction">The current transaction</param>
        /// <returns>The default <see cref="ActualFiniteState"/> if any, null otherwise</returns>
        ActualFiniteState GetDefaultState(ActualFiniteStateList actualList, IReadOnlyList<ActualFiniteState> actualListStates, string partition, ISecurityContext securityContext, NpgsqlTransaction transaction);
    }
}