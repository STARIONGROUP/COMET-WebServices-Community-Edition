// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicationDao.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2019 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Dao
{
    using CDP4Common.CommonData;
    using Npgsql;

    /// <summary>
    /// The Publication Data Access Object which acts as an ORM layer to the SQL database.
    /// </summary>
    public partial class PublicationDao
    {
        /// <summary>
        /// Deletes all data from the publication "current" table
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        public void DeleteAll(NpgsqlTransaction transaction, string partition)
        {
            this.DeleteAll(transaction, partition, ClassKind.Publication.ToString());
        }
    }
}
