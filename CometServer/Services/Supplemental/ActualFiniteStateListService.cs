﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActualFiniteStateListService.cs" company="Starion Group S.A.">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Authorization;
    
    using CDP4Common.DTO;
    
    using Npgsql;

    /// <summary>
    /// The ActualFiniteStateList Service which provides services on <see cref="ActualFiniteStateList"/>
    /// </summary>
    public sealed partial class ActualFiniteStateListService
    {
        /// <summary>
        /// Gets or sets the <see cref="IPossibleFiniteStateListService"/>
        /// </summary>
        public IPossibleFiniteStateListService PossibleFiniteStateListService { get; set; }

        /// <summary>
        /// Gets the default <see cref="ActualFiniteState"/> for <paramref name="actualList"/>
        /// </summary>
        /// <param name="actualList">The <see cref="ActualFiniteStateList"/> to get the default for</param>
        /// <param name="actualListStates">A list of <see cref="ActualFiniteState"/> defining the <paramref name="actualList"/></param>
        /// <param name="possibleLists">A list of <see cref="PossibleFiniteStateList"/> defining the <paramref name="actualList"/></param>
        /// <returns>The default <see cref="ActualFiniteState"/> if any, null otherwise</returns>
        public ActualFiniteState GetDefaultState(ActualFiniteStateList actualList, IReadOnlyList<ActualFiniteState> actualListStates, IReadOnlyList<PossibleFiniteStateList> possibleLists)
        {
            ArgumentNullException.ThrowIfNull(actualList);

            ArgumentNullException.ThrowIfNull(actualListStates);

            if (!actualList.ActualState.TrueForAll(x => actualListStates.Select(s => s.Iid).Contains(x)))
            {
                throw new ArgumentException("Some Actual Finite States are missing.", nameof(actualListStates));
            }

            ArgumentNullException.ThrowIfNull(possibleLists);

            if (!actualList.PossibleFiniteStateList.TrueForAll(x => possibleLists.Select(s => s.Iid).Contains(Guid.Parse(x.V.ToString()))))
            {
                throw new ArgumentException("Some Actual Finite States are missing.", nameof(possibleLists));
            }

            // get all default states from the possibleList combination making up the ActualList
            var actualPossibleLists = possibleLists.Where(p => actualList.PossibleFiniteStateList.Select(x => Guid.Parse(x.V.ToString())).Contains(p.Iid)).ToList();

            var defaultPossibleStates = new List<Guid>();

            foreach (var possibleFiniteStateList in actualPossibleLists)
            {
                if (!possibleFiniteStateList.DefaultState.HasValue)
                {
                    // no default ActualState if at least one has no default state
                    return null;
                }

                defaultPossibleStates.Add(possibleFiniteStateList.DefaultState.Value);
            }

            var actualActualListStates = actualListStates.Where(a => actualList.ActualState.Contains(a.Iid));
            return actualActualListStates.FirstOrDefault(a => a.PossibleState.TrueForAll(x => defaultPossibleStates.Contains(x)));
        }

        /// <summary>
        /// Gets the default <see cref="ActualFiniteState"/> for <paramref name="actualList"/>
        /// </summary>
        /// <param name="actualList">The <see cref="ActualFiniteStateList"/> to get the default for</param>
        /// <param name="actualListStates">A list of <see cref="ActualFiniteState"/> defining the <paramref name="actualList"/></param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security-context</param>
        /// <param name="transaction">The current transaction</param>
        /// <returns>The default <see cref="ActualFiniteState"/> if any, null otherwise</returns>
        public async Task<ActualFiniteState> GetDefaultState(ActualFiniteStateList actualList, IReadOnlyList<ActualFiniteState> actualListStates, string partition, ISecurityContext securityContext, NpgsqlTransaction transaction)
        {
            var possibleLists = (await this.PossibleFiniteStateListService.GetShallowAsync(transaction, partition, actualList.PossibleFiniteStateList.Select(x => Guid.Parse(x.V.ToString())), securityContext)).OfType<PossibleFiniteStateList>().ToList();
            return this.GetDefaultState(actualList, actualListStates, possibleLists);
        }
    }
}
