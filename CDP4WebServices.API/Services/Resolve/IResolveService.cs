// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResolveService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;

    using CDP4Orm.Dao.Resolve;

    using Npgsql;

    /// <summary>
    /// The ResolveService interface.
    /// </summary>
    public interface IResolveService
    {
        /// <summary>
        /// Resolve the information in the supplied collection of <see cref="DtoResolveHelper"/>.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="resolvableInfo">
        /// The resolvable info placeholders.
        /// </param>
        void ResolveItems(NpgsqlTransaction transaction, string partition, Dictionary<DtoInfo, DtoResolveHelper> resolvableInfo);

        /// <summary>
        /// Resolve missing containers from data store.
        /// </summary>
        /// <param name="transaction">
        /// The transaction.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="iid">
        /// The guid of the item to resolve type name for.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> type name of the supplied item.
        /// </returns> 
        string ResolveTypeNameByGuid(NpgsqlTransaction transaction, string partition, Guid iid);
    }
}