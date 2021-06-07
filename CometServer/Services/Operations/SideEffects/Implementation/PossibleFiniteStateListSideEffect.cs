// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PossibleFiniteStateListSideEffect.cs" company="RHEA System S.A.">
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

    using CDP4Common.DTO;

    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="PossibleFiniteStateListSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class PossibleFiniteStateListSideEffect : OperationSideEffect<PossibleFiniteStateList>
    {
        /// <summary>
        /// The List of <see cref="ActualFiniteStateList"/> to update after a <see
        /// </summary>
        private List<Guid> actualFiniteStateListIdsToUpdate;

        /// <summary>
        /// Gets or sets the <see cref="IActualFiniteStateListService"/>
        /// </summary>
        public IActualFiniteStateListService ActualFiniteStateListService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IStateDependentParameterUpdateService"/>
        /// </summary>
        public IStateDependentParameterUpdateService StateDependentParameterUpdateService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IFiniteStateLogicService"/>
        /// </summary>
        public IFiniteStateLogicService FiniteStateLogicService { get; set; }

        /// <summary>
        /// Execute the supplemental logic related to the update of a <see cref="PossibleFiniteStateList"/>
        /// </summary>
        /// <param name="thing">The <see cref="PossibleFiniteStateList"/></param>
        /// <param name="container">The container</param>
        /// <param name="originalThing">The original <see cref="PossibleFiniteStateList"/></param>
        /// <param name="transaction">the current transaction</param>
        /// <param name="partition">the current partition</param>
        /// <param name="securityContext">The security context</param>
        public override void AfterUpdate(PossibleFiniteStateList thing, Thing container, PossibleFiniteStateList originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            base.AfterUpdate(thing, container, originalThing, transaction, partition, securityContext);
            if (!thing.PossibleState.All(x => originalThing.PossibleState.Any(y => y.K == x.K && y.V == x.V))
                || thing.PossibleState.Count != originalThing.PossibleState.Count)
            {
                // Update all actualFiniteStateLists
                this.FiniteStateLogicService.UpdateAllRelevantActualFiniteStateList(thing, transaction, partition, securityContext);
            }
        }

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
        public override void BeforeDelete(PossibleFiniteStateList thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            var actualFiniteStateListCollectionToUpdate =
                this.ActualFiniteStateListService.GetShallow(transaction, partition, null, securityContext)
                    .OfType<ActualFiniteStateList>()
                    .Where(x => x.PossibleFiniteStateList.Select(oi => Guid.Parse(oi.V.ToString())).Contains(thing.Iid))
                    .ToList();

            this.actualFiniteStateListIdsToUpdate = new List<Guid>();
            foreach (var actualFiniteStateList in actualFiniteStateListCollectionToUpdate)
            {
                // delete all actual lists that only have the deleted PossibleList as PossibleList and update all parameters that depend on them
                // do it before as otherwise the ActualStateList would not contain the PossibleFiniteStateList anymore
                if (actualFiniteStateList.PossibleFiniteStateList.Count == 1)
                {
                    this.StateDependentParameterUpdateService.UpdateAllStateDependentParameters(actualFiniteStateList, (Iteration)container, transaction, partition, securityContext, null);
                    if (!this.ActualFiniteStateListService.DeleteConcept(transaction, partition, actualFiniteStateList, container))
                    {
                        throw new InvalidOperationException($"The actual finite state list {actualFiniteStateList.Iid} could not be deleted");
                    }
                }
                else
                {
                    // this cannot be parallelized or needs to be refactored
                    this.actualFiniteStateListIdsToUpdate.Add(actualFiniteStateList.Iid);
                }
            }
        }

        /// <summary>
        /// Executes post update logic after <see cref="PossibleFiniteStateList"/> was updated
        /// </summary>
        /// <param name="thing">The updated <see cref="PossibleFiniteStateList"/></param>
        /// <param name="container">The container</param>
        /// <param name="originalThing">The original <see cref="PossibleFiniteStateList"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        public override void AfterDelete(PossibleFiniteStateList thing, Thing container, PossibleFiniteStateList originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            base.AfterDelete(thing, container, originalThing, transaction, partition, securityContext);

            var actualFiniteStateLists = this.ActualFiniteStateListService
                .GetShallow(transaction, partition, this.actualFiniteStateListIdsToUpdate, securityContext)
                .OfType<ActualFiniteStateList>()
                .ToList(); 

            foreach (var actualFiniteStateList in actualFiniteStateLists)
            {
                this.FiniteStateLogicService.UpdateActualFinisteStateList(actualFiniteStateList, (Iteration)container, transaction, partition, securityContext);
            }
        }
    }
}
