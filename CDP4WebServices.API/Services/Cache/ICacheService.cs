// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICacheService.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// The CacheService interface.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Save a <see cref="Thing"/> to a cache table
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The database partition (schema) where the requested resource is stored.</param>
        /// <param name="thing">The revised <see cref="Thing"/></param>
        void WriteToCache(NpgsqlTransaction transaction, string partition, Thing thing);
    }
}