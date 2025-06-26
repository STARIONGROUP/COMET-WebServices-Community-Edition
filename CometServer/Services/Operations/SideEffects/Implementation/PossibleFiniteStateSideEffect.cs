// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PossibleFiniteStateSideEffect.cs" company="Starion Group S.A.">
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
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using CometServer.Services.Authorization;

    using Npgsql;

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
        public override async Task AfterCreate(PossibleFiniteState thing, Thing container, PossibleFiniteState originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            await base.AfterCreate(thing, container, originalThing, transaction, partition, securityContext);

            this.FiniteStateLogicService.UpdateAllRelevantActualFiniteStateListAsync((PossibleFiniteStateList)container, transaction, partition, securityContext);
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
        public override async Task BeforeDelete(PossibleFiniteState thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            await base.BeforeDelete(thing, container, transaction, partition, securityContext);

            var possibleList = (PossibleFiniteStateList)container;
            
            if (possibleList.PossibleState.Count == 1 &&
                possibleList.PossibleState.Single().V.ToString() == thing.Iid.ToString())
            {
                throw new InvalidOperationException($"It is not allowed to delete the last Possible Finite State {thing.Iid} from the list. Consider deleting the whole Possible Finite State List {possibleList.Iid}");
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
        public override async Task AfterDelete(PossibleFiniteState thing, Thing container, PossibleFiniteState originalThing, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            await base.AfterDelete(thing, container, originalThing, transaction, partition, securityContext);

            this.FiniteStateLogicService.UpdateAllRelevantActualFiniteStateListAsync((PossibleFiniteStateList)container, transaction, partition, securityContext);
        }
    }
}
