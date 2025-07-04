// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelReferenceDataLibraryService.cs" company="Starion Group S.A.">
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
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// Extension for the code-generated <see cref="IModelReferenceDataLibraryService"/>
    /// </summary>
    public partial interface IModelReferenceDataLibraryService
    {
        /// <summary>
        /// Query the required <see cref="ReferenceDataLibrary"/> for the <paramref name="iteration"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="iteration">The <see cref="Iteration"/></param>
        /// <returns>The <see cref="ReferenceDataLibrary"/></returns>
        Task<IEnumerable<ReferenceDataLibrary>> QueryReferenceDataLibraryAsync(NpgsqlTransaction transaction, Iteration iteration);
    }
}
