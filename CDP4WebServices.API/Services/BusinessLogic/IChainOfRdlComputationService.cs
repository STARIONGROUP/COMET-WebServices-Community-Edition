// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChainOfRdlComputationService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski
//
//    This file is part of COMET Web Services Community Edition. 
//    The COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

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
        /// Gets or sets the (injected) <see cref="IModelReferenceDataLibraryDao"/> used to query the <see cref="ModelReferenceDataLibrary"/> objects from the datastore
        /// </summary>
        public IModelReferenceDataLibraryDao ModelReferenceDataLibraryDao { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="ISiteReferenceDataLibraryDao"/> used to query the <see cref="SiteReferenceDataLibrary"/> objects from the datastore
        /// </summary>
        public ISiteReferenceDataLibraryDao SiteReferenceDataLibraryDao { get; set; }

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
        IEnumerable<Guid> QueryReferenceDataLibraryDependency(NpgsqlTransaction transaction, IEnumerable<EngineeringModelSetup> engineeringModelSetups);
    }
}
