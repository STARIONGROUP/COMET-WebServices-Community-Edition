// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStateDependentParameterUpdateService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System.Collections.Generic;
    using CDP4Common.DTO;
    using CDP4WebServices.API.Services.Authorization;
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