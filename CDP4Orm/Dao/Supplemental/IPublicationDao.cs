// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPublicationDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2017-2019 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    using Npgsql;

    /// <summary>
    /// The Publication Dao Interface which uses the ORM layer to interact with the data model.
    /// </summary>
    public partial interface IPublicationDao
    {
        /// <summary>
        /// Deletes all data from the publication "current" table
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        void DeleteAll(NpgsqlTransaction transaction, string partition);
    }
}