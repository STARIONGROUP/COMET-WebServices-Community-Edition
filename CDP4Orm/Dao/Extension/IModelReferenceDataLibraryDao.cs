// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelReferenceDataLibraryDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
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
