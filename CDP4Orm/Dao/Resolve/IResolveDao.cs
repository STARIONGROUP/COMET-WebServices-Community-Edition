// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResolveDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao.Resolve
{
    using System;
    using System.Collections.Generic;

    using Npgsql;

    /// <summary>
    /// The ContainerDao interface.
    /// </summary>
    public interface IResolveDao
    {
        /// <summary>
        /// Resolve the information from the database.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource are to be resolved from.
        /// </param>
        /// <param name="ids">
        /// The <see cref="CDP4Common.DTO.Thing"/> instances to resolve information for.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="ResolveInfo"/>.
        /// </returns>
        IEnumerable<ResolveInfo> Read(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids);
    }
}