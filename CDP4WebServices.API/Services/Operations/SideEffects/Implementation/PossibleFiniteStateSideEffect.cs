// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PossibleFiniteStateSideEffect.cs" company="RHEA System S.A.">
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
    using System.Linq;

    using CDP4Common.DTO;

    using CometServer.Services.Authorization;

    using Npgsql;

    using Iteration = CDP4Common.DTO.Iteration;
    using PossibleFiniteState = CDP4Common.DTO.PossibleFiniteState;
    using PossibleFiniteStateList = CDP4Common.DTO.PossibleFiniteStateList;

    /// <summary>
    /// The purpose of the <see cref="PossibleFiniteStateSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class PossibleFiniteStateSideEffect : OperationSideEffect<PossibleFiniteState>
    {
        /// <summary>
        /// Gets or sets the <see cref="IFiniteStateLogicService"/>
        /// </summary>
        public IFiniteStateLogicService FiniteStateLogicService { get; set; }

        /// <summary>
        /// Execute additional logic after a successful create operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="originalThing">
        /// The original Thing.
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
        public override void AfterCreate(PossibleFiniteState thing, Thing container, PossibleFiniteState originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            this.FiniteStateLogicService.UpdateAllRelevantActualFiniteStateList((PossibleFiniteStateList)container, transaction, partition, securityContext);
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
        public override void BeforeDelete(PossibleFiniteState thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            var possibleList = (PossibleFiniteStateList)container;
            if (possibleList.PossibleState.Count == 1 &&
                possibleList.PossibleState.Single().V.ToString() == thing.Iid.ToString())
            {
                throw new InvalidOperationException(string.Format("It is not allowed to delete the last Possible Finite State {0} from the list. Consider deleting the whole Possible Finite State List {1}", thing.Iid, possibleList.Iid));
            }
        }

        /// <summary>
        /// After a <see cref="PossibleFiniteState"/> is deleted, update related <see cref="ActualFiniteStateList"/> and related <see cref="ParameterBase"/>s
        /// </summary>
        /// <param name="thing">The deleted <see cref="PossibleFiniteState"/></param>
        /// <param name="container">The container</param>
        /// <param name="originalThing">The original <see cref="PossibleFiniteState"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        public override void AfterDelete(PossibleFiniteState thing, Thing container, PossibleFiniteState originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            base.AfterDelete(thing, container, originalThing, transaction, partition, securityContext);
            this.FiniteStateLogicService.UpdateAllRelevantActualFiniteStateList((PossibleFiniteStateList)container, transaction, partition, securityContext);
        }
    }
}
