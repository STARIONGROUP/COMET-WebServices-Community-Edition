// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIterationSetupDao.cs" company="RHEA System S.A.">
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
    /// The IterationSetup Service Interface which uses the ORM layer to interact with the data model.
    /// </summary>
    public partial interface IIterationSetupDao
    {
        /// <summary>
        /// Read the data from the database based on <see cref="IterationSetup.Iid"/>.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="iterationId">
        /// The iteration Id.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="IterationSetup"/>.
        /// </returns>
        IEnumerable<IterationSetup> ReadByIteration(NpgsqlTransaction transaction, string partition, Guid iterationId);

        /// <summary>
        /// Read the data from the database based on <see cref="EngineeringModelSetup.Iid"/>.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="engineeringModelSetupId">
        /// The <see cref="EngineeringModelSetup"/> Id.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="IterationSetup"/>.
        /// </returns>
        IEnumerable<IterationSetup> ReadByEngineeringModelSetup(NpgsqlTransaction transaction, string partition, Guid engineeringModelSetupId);
    }
}
