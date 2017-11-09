// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IContainerDao.cs" company="RHEA System S.A.">
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
    public interface IContainerDao
    {
        /// <summary>
        /// Read the data from the database.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="typeName">
        /// The type Name of the <see cref="CDP4Common.DTO.Thing"/> for which to return the container information.
        /// </param>
        /// <param name="ids">
        /// The <see cref="CDP4Common.DTO.Thing"/> instances for which to retrieve the container information.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="ContainerInfo"/>.
        /// </returns>
        IEnumerable<Tuple<Guid, ContainerInfo>> Read(NpgsqlTransaction transaction, string partition, string typeName, IEnumerable<Guid> ids);
    }
}