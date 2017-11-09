// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReadService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// <summary>
//   This is an auto-generated class. Any manual changes on this file will be overwritten!
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CDP4WebServices.API.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The read service interface which uses the ORM layer to interact with the data model.
    /// </summary>
    public interface IReadService
    {
        /// <summary>
        /// Get the requested data from the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="ids">
        /// Ids to retrieve from the database.
        /// </param>
        /// <param name="containerSecurityContext">
        /// The security context of the container instance.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="Thing"/>.
        /// </returns>
        IEnumerable<Thing> Get(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext containerSecurityContext);

        /// <summary>
        /// Get the requested data from the ORM layer.
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="ids">
        /// Ids to retrieve from the database.
        /// </param>
        /// <param name="securityContext">
        /// The security Context.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="Thing"/>.
        /// </returns>
        IEnumerable<Thing> GetShallow(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext securityContext);

        /// <summary>
        /// Get the requested data from the ORM layer by chaining the containment properties.
        /// </summary>
        /// <param name="transaction">
        /// The transaction object.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="ids">
        /// Ids to retrieve from the database.
        /// </param>
        /// <param name="securityContext">
        /// The security Context.
        /// </param>
        /// <returns>
        /// List of instances of <see cref="Thing"/>.
        /// </returns>
        IEnumerable<Thing> GetDeep(NpgsqlTransaction transaction, string partition, IEnumerable<Guid> ids, ISecurityContext securityContext);
    }
}
