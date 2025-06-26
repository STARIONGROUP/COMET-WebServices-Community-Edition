﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelSetupService.cs" company="Starion Group S.A.">
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

namespace CometServer.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

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
        /// <remarks>
        /// The <see cref="EngineeringModelSetup"/> objects are read from the Cache Table and not from the View table
        /// </remarks>
        public async Task<EngineeringModelSetup> GetEngineeringModelSetupFromDataBaseCache(NpgsqlTransaction transaction, Guid engineeringModelId)
        {
            return (await this.EngineeringModelSetupDao
                .ReadAsync(transaction, Cdp4TransactionManager.SITE_DIRECTORY_PARTITION, null, true))
                .FirstOrDefault(x => x.EngineeringModelIid == engineeringModelId);
        }
    }
}
