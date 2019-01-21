// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPublicationService.cs" company="RHEA System S.A.">
//   Copyright (c) 2017-2019 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using Npgsql;

    /// <summary>
    /// The Publication Service Interface which uses the ORM layer to interact with the data model.
    /// </summary>
    public partial interface IPublicationService
    {
        /// <summary>
        /// Deletes all data from the current publication table
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        void DeleteAll(NpgsqlTransaction transaction, string partition);
    }
}