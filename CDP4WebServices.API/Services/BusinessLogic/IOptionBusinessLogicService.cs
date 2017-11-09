// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOptionBusinessLogicService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System.Collections.Generic;
    using CDP4Common.DTO;

    /// <summary>
    /// The purpose of the <see cref="IOptionService"/> is to provide services related to <see cref="Option"/>s
    /// </summary>
    public interface IOptionBusinessLogicService : IBusinessLogicService
    {
        /// <summary>
        /// Initializes the <see cref="IOptionBusinessLogicService"/>
        /// </summary>
        /// <param name="iteration">The current <see cref="Iteration"/></param>
        /// <param name="options">The current <see cref="Option"/>s</param>
        void Initialize(Iteration iteration, IReadOnlyCollection<Option> options);

        /// <summary>
        /// Computes the default <see cref="Option"/> for an iteration
        /// </summary>
        /// <returns>The default <see cref="Option"/> if any</returns>
        Option GetDefaultOption();
    }
}
