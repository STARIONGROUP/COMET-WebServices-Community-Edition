// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IActualFiniteStateService.cs" company="RHEA System S.A.">
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
    /// The ActualFiniteState Service Interface which uses the ORM layer to interact with the data model.
    /// </summary>
    public partial interface IActualFiniteStateService : IPersistService
    {
        bool UpsertConcept(NpgsqlTransaction transaction, string partition, Thing thing, Thing container, long sequence = -1);
    }
}
