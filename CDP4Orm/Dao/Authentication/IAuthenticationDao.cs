// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthenticationDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao.Authentication
{
    using System.Collections.Generic;

    using CDP4Authentication;

    using Npgsql;

    /// <summary>
    /// The AuthenticationDao interface.
    /// </summary>
    public interface IAuthenticationDao
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
        /// <param name="userName">
        /// UserName to retrieve from the database.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="AuthenticationPerson"/>.
        /// </returns>
        IEnumerable<AuthenticationPerson> Read(NpgsqlTransaction transaction, string partition, string userName);
    }
}