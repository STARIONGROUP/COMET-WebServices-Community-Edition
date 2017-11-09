// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheService.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using CDP4Common.DTO;
    using CDP4Orm.Dao.Revision;

    using Npgsql;

    /// <summary>
    /// A service that allows revision based retrieval of concepts from the data store.
    /// </summary>
    public class CacheService : ICacheService
    {
        /// <summary>
        /// Gets or sets the cache dao.
        /// </summary>
        public ICacheDao CacheDao { get; set; }

        /// <summary>
        /// Save a <see cref="Thing"/> to a cache table
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The database partition (schema) where the requested resource is stored.</param>
        /// <param name="thing">The revised <see cref="Thing"/></param>
        public void WriteToCache(NpgsqlTransaction transaction, string partition, Thing thing)
        {
            this.CacheDao.Write(transaction, partition, thing);
        }
    }
}
