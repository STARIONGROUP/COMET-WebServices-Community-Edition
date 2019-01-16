// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICopySourceService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2019 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;
    using CDP4Common.Dto;
    using CDP4Common.DTO;
    using Npgsql;

    /// <summary>
    /// The interface for the service that handles data access to the source of a copy operation (of iteration data)
    /// </summary>
    public interface ICopySourceService : IBusinessLogicService
    {
        /// <summary>
        /// Gets the source data for the copy operation
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="copyinfo">The <see cref="CopyInfo"/></param>
        /// <param name="requestPartition">The contextual partition</param>
        /// <returns>The source data</returns>
        IReadOnlyList<Thing> GetCopySourceData(NpgsqlTransaction transaction, CopyInfo copyinfo, string requestPartition);

        /// <summary>
        /// Generates the copy references
        /// </summary>
        /// <param name="allSourceThings">The source <see cref="Thing"/></param>
        /// <returns>The identifier maps</returns>
        Dictionary<Guid, Guid> GenerateCopyReference(IReadOnlyList<Thing> allSourceThings);
    }
}
