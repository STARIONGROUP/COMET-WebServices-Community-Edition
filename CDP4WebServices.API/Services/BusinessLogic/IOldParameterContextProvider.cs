// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOldParameterContextProvider.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using Authorization;
    using CDP4Common.DTO;
    using Npgsql;

    /// <summary>
    /// A service interface that provides context on the original version of an updated <see cref="Parameter"/>
    /// </summary>
    public interface IOldParameterContextProvider : IBusinessLogicService
    {
        /// <summary>
        /// Initializes the service with a previous version of an updated <see cref="Parameter"/>
        /// </summary>
        /// <param name="oldParameter">The old <see cref="Parameter"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        /// <param name="iteration">The current <see cref="Iteration"/> (nullable)</param>
        void Initialize(Parameter oldParameter, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext, Iteration iteration = null);

        /// <summary>
        /// Gets the source <see cref="ParameterValueSet"/> for the new one to be created for a specified option and state
        /// </summary>
        /// <param name="option">The identifier of the option</param>
        /// <param name="state">The identifier of the state</param>
        /// <returns>The source <see cref="ParameterValueSet"/></returns>
        ParameterValueSet GetsourceValueSet(Guid? option, Guid? state);
    }
}
