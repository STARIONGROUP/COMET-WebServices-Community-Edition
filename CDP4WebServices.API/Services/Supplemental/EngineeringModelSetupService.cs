// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelSetupService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2019 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CDP4Common.DTO;
    using CDP4Orm.Dao;
    using Helpers;
    using Npgsql;

    /// <summary>
    /// Extension for the code-generated <see cref="EngineeringModelSetupService"/>
    /// </summary>
    public partial class EngineeringModelSetupService
    {
        /// <summary>
        /// Get the <see cref="EngineeringModelSetup"/> associated to the <paramref name="engineeringModelId"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="engineeringModelId">The identifier of the associated <see cref="EngineeringModel"/></param>
        /// <returns>The requested <see cref="EngineeringModelSetup"/></returns>
        public EngineeringModelSetup GetEngineeringModelSetup(NpgsqlTransaction transaction, Guid engineeringModelId)
        {
            return this.EngineeringModelSetupDao
                .Read(transaction, Cdp4TransactionManager.SITE_DIRECTORY_PARTITION)
                .FirstOrDefault(x => x.EngineeringModelIid == engineeringModelId);
        }
    }
}
