// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActualFiniteStateListService.cs" company="RHEA System S.A.">
//   Copyright (c) 2017-2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
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