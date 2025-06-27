// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChainOfRdlComputationService.cs" company="Starion Group S.A.">
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
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="IChainOfRdlComputationService"/> is to compute and return the chain of rdls for a
    /// provided list of <see cref="EngineeringModelSetup"/>s
    /// </summary>
    /// <remarks>
    /// This service is a per request service and is not thread safe
    /// </remarks>
    public interface IChainOfRdlComputationService : IBusinessLogicService
    {
        /// <summary>
        /// Gets the identifiers of the chain of <see cref="SiteReferenceDataLibrary"/> dependencies for a list of <see cref="EngineeringModelSetup"/>s
        /// </summary>
        /// <param name="engineeringModelSetups">
        /// The <see cref="EngineeringModelSetup"/> for which the unique identifiers of the chain of <see cref="SiteReferenceDataLibrary"/> needs to be determined
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the datastore
        /// </param>
        /// <returns>
        /// The unique identifiers of the <see cref="SiteReferenceDataLibrary"/> dependency of the provided <see cref="IEnumerable{EngineeringModelSetup}"/>
        /// </returns>
        Task<IEnumerable<Guid>> QueryReferenceDataLibraryDependencyAsync(NpgsqlTransaction transaction, IEnumerable<EngineeringModelSetup> engineeringModelSetups);
    }
}
