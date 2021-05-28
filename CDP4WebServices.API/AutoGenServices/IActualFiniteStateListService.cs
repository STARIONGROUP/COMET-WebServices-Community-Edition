// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActualFiniteStateListService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2018 RHEA System S.A.
// </copyright>
// <summary>
//   This is an auto-generated class. Any manual changes on this file will be overwritten!
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using CDP4Common.DTO;
using Npgsql;

namespace CDP4WebServices.API.Services
{
    /// <summary>
    /// The ActualFiniteStateList Service Interface which uses the ORM layer to interact with the data model.
    /// </summary>
    public partial interface IActualFiniteStateListService : IPersistService
    {
        /// <summary>
        /// Upsert the supplied DTO instance.
        /// </summary>
        /// <param name="transaction">
        /// The current <see cref="NpgsqlTransaction"/> to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be updated.
        /// </param>
        /// <param name="thing">
        /// The <see cref="Thing"/> to create.
        /// </param>
        /// <param name="container">
        /// The container instance of the DTO to be updated.
        /// </param>
        /// <returns>
        /// True if the update was successful.
        /// </returns>
        bool UpsertConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1);
    }
}
