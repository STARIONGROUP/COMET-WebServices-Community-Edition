// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelReferenceDataLibraryDao.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2020 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Kamil Wojnowski, 
//            Nathanael Smiechowski
//
//    This file is part of CDP4 Web Services Community Edition. 
//    The CDP4 Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
//
//    The CDP4 Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4 Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// The ModelReferenceDataLibrary Service Interface which uses the ORM layer to interact with the data model.
    /// </summary>
    public partial interface IModelReferenceDataLibraryDao
    {
        /// <summary>
        /// GEts the identifiers of the chain of <see cref="SiteReferenceDataLibrary"/> dependencies for <see cref="EngineeringModelSetup"/>s
        /// </summary>
        /// <param name="modelSetups">The <see cref="EngineeringModelSetup"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <returns>The identifiers of the <see cref="SiteReferenceDataLibrary"/> dependency</returns>
        IEnumerable<Guid> GetSiteReferenceDataLibraryDependency(IEnumerable<EngineeringModelSetup> modelSetups, NpgsqlTransaction transaction);
    }
}
