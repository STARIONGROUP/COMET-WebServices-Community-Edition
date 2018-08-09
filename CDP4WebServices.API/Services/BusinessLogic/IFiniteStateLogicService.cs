// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFiniteStateLogicService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using Authorization;
    using CDP4Common.DTO;
    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="IFiniteStateLogicService"/> is to provide services for <see cref="ActualFiniteState"/>  and <see cref="PossibleFiniteState"/> instances
    /// </summary>
    public interface IFiniteStateLogicService : IBusinessLogicService
    {
        // todo add get default (task T2815 CDP4WEBSERVICES)
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
        void UpdateAllRelevantActualFiniteStateList(PossibleFiniteStateList possibleFiniteStateList, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext);

        /// <summary>
        /// Update a <see cref="ActualFiniteStateList"/> by allocating new <see cref="ActualFiniteState"/>
        /// </summary>
        /// <param name="actualFiniteStateList">The <see cref="ActualFiniteStateList"/></param>
        /// <param name="iteration">The <see cref="Iteration"/></param>
        /// <param name="transaction">The current <see cref="NpgsqlTransaction"/></param>
        /// <param name="partition">The partition</param>
        /// <param name="securityContext">The <see cref="ISecurityContext"/></param>
        void UpdateActualFinisteStateList(ActualFiniteStateList actualFiniteStateList, Iteration iteration, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext);
    }
}
