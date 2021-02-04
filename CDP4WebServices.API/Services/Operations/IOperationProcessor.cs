// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOperationProcessor.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations
{
    using System.Collections.Generic;
    using System.IO;

    using CDP4Common.DTO;

    using CDP4Orm.Dao.Resolve;

    using Npgsql;

    /// <summary>
    /// The injectable OperationProcessor interface that allows processing of a <see cref="CdpPostOperation"/>.
    /// </summary>
    public interface IOperationProcessor
    {
        /// <summary>
        /// Process the posted operation message.
        /// </summary>
        /// <param name="operation">
        /// The operation.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="fileStore">
        /// The optional file binaries that were included in the request.
        /// </param>
        void Process(CdpPostOperation operation, NpgsqlTransaction transaction, string partition, Dictionary<string, Stream> fileStore = null);

        /// <summary>
        /// Gets the operation <see cref="Thing"/> instance cache.
        /// </summary>
        Dictionary<DtoInfo, DtoResolveHelper> OperationThingCache { get; }
    }
}