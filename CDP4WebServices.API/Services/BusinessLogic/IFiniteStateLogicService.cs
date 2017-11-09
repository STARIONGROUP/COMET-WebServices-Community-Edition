// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFiniteStateLogicService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using CDP4Common.DTO;

    /// <summary>
    /// The purpose of the <see cref="IFiniteStateLogicService"/> is to provide services for <see cref="ActualFiniteState"/>  and <see cref="PossibleFiniteState"/> instances
    /// </summary>
    public interface IFiniteStateLogicService : IBusinessLogicService
    {
        // todo add get default (task T2815 CDP4WEBSERVICES)
    }
}
