// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelReferenceDataLibraryService.cs" company="RHEA System S.A.">
//   Copyright (c) 2015-2019 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System.Collections.Generic;
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
        IEnumerable<ReferenceDataLibrary> QueryReferenceDataLibrary(NpgsqlTransaction transaction, Iteration iteration);
    }
}
