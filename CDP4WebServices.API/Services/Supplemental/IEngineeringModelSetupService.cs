// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEngineeringModelSetupService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2019 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using CDP4Common.DTO;
    using Npgsql;

    /// <summary>
    /// Extension for the code-generated <see cref="EngineeringModelSetupService"/>
    /// </summary>
    public partial interface IEngineeringModelSetupService
    {
        /// <summary>
        /// Get the <see cref="EngineeringModelSetup"/> associated to the <paramref name="engineeringModelId"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="engineeringModelId">The identifier of the associated <see cref="EngineeringModel"/></param>
        /// <returns>The requested <see cref="EngineeringModelSetup"/></returns>
        EngineeringModelSetup GetEngineeringModelSetup(NpgsqlTransaction transaction, Guid engineeringModelId);
    }
}
