// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDomainOfExpertiseDao.cs" company="RHEA System S.A.">
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
    /// The Domain of expertise dao interface.
    /// </summary>
    public partial interface IDomainOfExpertiseDao
    {
        /// <summary>
        /// Read the data from the database based on <see cref="Person"/> id and an <see cref="EngineeringModelSetup"/> id.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="personId">
        /// The <see cref="Person.Iid"/> to retrieve participant info for from the database.
        /// </param>
        /// <param name="engineeringModelSetupId">
        /// The <see cref="EngineeringModelSetup.Iid"/> to retrieve domain info for from the database.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="DomainOfExpertise"/>.
        /// </returns>
        IEnumerable<DomainOfExpertise> ReadByPersonAndEngineeringModelSetup(NpgsqlTransaction transaction, string partition, Guid personId, Guid engineeringModelSetupId);
    }
}
