// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FiniteStateLogicService.cs" company="RHEA System S.A.">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.EngineeringModelData;

    using CometServer.Services.Authorization;

    using Npgsql;

    using ActualFiniteState = CDP4Common.DTO.ActualFiniteState;
    using ActualFiniteStateList = CDP4Common.DTO.ActualFiniteStateList;
    using Iteration = CDP4Common.DTO.Iteration;
    using PossibleFiniteStateList = CDP4Common.DTO.PossibleFiniteStateList;

    /// <summary>
    /// The purpose of the <see cref="ParameterValueSetFactory"/> is to create <see cref="CDP4Common.DTO.ParameterValueSet"/> instances
    /// </summary>
    public class FiniteStateLogicService : IFiniteStateLogicService
    {
        /// <summary>
        /// Gets or sets the <see cref="IIterationService"/>
        /// </summary>
        public IIterationService IterationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IActualFiniteStateListService"/>
        /// </summary>
        public IActualFiniteStateListService ActualFiniteStateListService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IActualFiniteStateService"/>
        /// </summary>
        public IActualFiniteStateService ActualFiniteStateService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IStateDependentParameterUpdateService"/>
        /// </summary>
        public IStateDependentParameterUpdateService StateDependentParameterUpdateService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPossibleFiniteStateListService"/>
        /// </summary>
        public IPossibleFiniteStateListService PossibleFiniteStateListService { get; set; }

        /// <summary>
        /// Update All <see cref="CDP4Common.DTO.ActualFiniteStateList"/> related to a modified <see cref="CDP4Common.DTO.PossibleFiniteStateList"/>
        /// </summary>
        /// <param name="possibleFiniteStateList">
        /// The container instance of the <see cref="CDP4Common.DTO.PossibleFiniteStateList"/> that is inspected.
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
        public void UpdateAllRelevantActualFiniteStateList(PossibleFiniteStateList possibleFiniteStateList, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            // redirect out the iteration partition to the engineeringmodel one, as that contains the iteration information
            var engineeringModelPartition = partition.Replace(
                CDP4Orm.Dao.Utils.IterationSubPartition,
                CDP4Orm.Dao.Utils.EngineeringModelPartition);

            var iteration =
                this.IterationService.GetActiveIteration(transaction, engineeringModelPartition, securityContext);

            // get all actual finite state list
            var actualFiniteStateList = this.ActualFiniteStateListService.GetShallow(transaction, partition, null, securityContext).Cast<ActualFiniteStateList>();
            var actualFiniteStateListToUpdate = actualFiniteStateList.Where(x => x.PossibleFiniteStateList.Select(item => Guid.Parse(item.V.ToString())).Contains(possibleFiniteStateList.Iid));

            foreach (var finiteStateList in actualFiniteStateListToUpdate)
            {
                this.UpdateActualFinisteStateList(finiteStateList, iteration, transaction, partition, securityContext);
            }
        }

        /// <summary>
        /// Update a <see cref="ActualFiniteStateList"/> by allocating new <see cref="ActualFiniteState"/>
        /// </summary>
        /// <param name="actualFiniteStateList">The <see cref="ActualFiniteStateList"/></param>
        /// <param name="iteration">The <see cref="Iteration"/></param>
        /// <param name="transaction">The current <see cref="NpgsqlTransaction"/></param>
        /// <param name="partition">The partition</param>
        /// <param name="securityContext">The <see cref="ISecurityContext"/></param>
        public void UpdateActualFinisteStateList(ActualFiniteStateList actualFiniteStateList, Iteration iteration, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            // delete the old ActualFiniteState
            var oldActualStates = this.ActualFiniteStateService.GetShallow(transaction, partition, actualFiniteStateList.ActualState, securityContext)
                                                                .OfType<ActualFiniteState>().ToList();

            // Gets the possible finite state list of the current processed ActualFiniteStateList
            var pslCollection =
                this.PossibleFiniteStateListService
                    .GetShallow(transaction, partition, actualFiniteStateList.PossibleFiniteStateList.Select(item => Guid.Parse(item.V.ToString())), securityContext)
                    .OfType<PossibleFiniteStateList>()
                    .ToList();

            if (pslCollection.Count != actualFiniteStateList.PossibleFiniteStateList.Count)
            {
                throw new InvalidOperationException("All the Possible finite state lists could not be retrieved.");
            }

            // Build the ordered collection of PossibleFiniteStateList for the current ActualFiniteStateList
            var orderedPslCollection = new List<PossibleFiniteStateList>(pslCollection.Count);
            foreach (var item in actualFiniteStateList.PossibleFiniteStateList.OrderBy(x => x.K))
            {
                var psl = pslCollection.Single(x => x.Iid.ToString() == item.V.ToString());
                orderedPslCollection.Add(psl);
            }

            var newOldActualStateMap = new Dictionary<ActualFiniteState, ActualFiniteState>();
            this.CreateActualStates(orderedPslCollection, 0, null, actualFiniteStateList, transaction, partition, oldActualStates, ref newOldActualStateMap);

            this.StateDependentParameterUpdateService.UpdateAllStateDependentParameters(actualFiniteStateList, iteration, transaction, partition, securityContext, newOldActualStateMap);

            // This is where value-set are cleaned up
            // delete old actual states which will clean up all value sets that depend on it
            foreach (var actualState in oldActualStates)
            {
                if (!this.ActualFiniteStateService.DeleteConcept(transaction, partition, actualState, actualFiniteStateList))
                {
                    throw new InvalidOperationException(string.Format("The actual finite state {0} could not be deleted", actualState.Iid));
                }
            }
        }

        /// <summary>
        /// Create the <see cref="ActualFiniteState"/>s for a <see cref="ActualFiniteStateList"/>
        /// </summary>
        /// <param name="pslCollection">The ordered collection of <see cref="PossibleFiniteStateList"/></param>
        /// <param name="pslIndex">The index of the <see cref="PossibleFiniteStateList"/> in the collection to process</param>
        /// <param name="possibleStateIds">The current collection of <see cref="Guid"/> to use for the <see cref="ActualFiniteState"/> to create</param>
        /// <param name="container">The <see cref="ActualFiniteStateList"/></param>
        /// <param name="transaction">The current <see cref="NpgsqlTransaction"/></param>
        /// <param name="partition">The partition in the database</param>
        /// <param name="oldActualStates">The old <see cref="ActualFiniteState"/></param>
        /// <param name="newOldStateMap">The resulting map that links the new to old <see cref="ActualFiniteState"/></param>
        private void CreateActualStates(IReadOnlyList<PossibleFiniteStateList> pslCollection, int pslIndex, IEnumerable<Guid> possibleStateIds, ActualFiniteStateList container, NpgsqlTransaction transaction, string partition, IReadOnlyList<ActualFiniteState> oldActualStates, ref Dictionary<ActualFiniteState, ActualFiniteState> newOldStateMap)
        {
            var currentPossibleStateIds = possibleStateIds == null ? new List<Guid>() : possibleStateIds.ToList();

            // build the different PossibleStates combination taken from the PossibleFiniteStateLists and create an ActualState for each of these combinations
            foreach (var orderedItem in pslCollection[pslIndex].PossibleState.OrderBy(x => x.K))
            {
                var newPossibleStateIds = currentPossibleStateIds.ToList();
                newPossibleStateIds.Add(Guid.Parse(orderedItem.V.ToString()));

                // Last PossibleFiniteStateList in the ordered collection
                if (pslIndex == pslCollection.Count - 1)
                {
                    // create actual state
                    var newActualstate = new ActualFiniteState(Guid.NewGuid(), 1);
                    newActualstate.PossibleState.AddRange(newPossibleStateIds);

                    this.ActualFiniteStateService.CreateConcept(transaction, partition, newActualstate, container);

                    var oldActualState =
                        oldActualStates.FirstOrDefault(x => x.PossibleState.All(ps => newPossibleStateIds.Contains(ps)));

                    newActualstate.Kind = oldActualState?.Kind ?? ActualFiniteStateKind.MANDATORY;

                    newOldStateMap.Add(newActualstate, oldActualState);
                    continue;
                }

                this.CreateActualStates(pslCollection, pslIndex + 1, newPossibleStateIds, container, transaction, partition, oldActualStates, ref newOldStateMap);
            }
        }
    }
}
