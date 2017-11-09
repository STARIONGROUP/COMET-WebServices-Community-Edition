// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IParameterSubscriptionValueSetFactory.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using CDP4Common.DTO;
    using CDP4Common.Types;

    /// <summary>
    /// The purpose of the <see cref="IParameterSubscriptionValueSetFactory"/> is to create <see cref="ParameterSubscriptionValueSet"/> instances
    /// </summary>
    public interface IParameterSubscriptionValueSetFactory : IBusinessLogicService
    {
        /// <summary>
        /// Creates a new <see cref="ParameterSubscriptionValueSet"/> where manual value is equal to a <see cref="ValueArray{String}"/> where each slot is a "-"
        /// and the <see cref="CDP4Common.EngineeringModelData.ParameterSwitchKind"/> is set to <see cref="CDP4Common.EngineeringModelData.ParameterSwitchKind.MANUAL"/>
        /// </summary>
        /// <param name="subscribedValueSetIid">
        /// The unique id of the <see cref="ParameterValueSetBase"/> that is referenced by the <see cref="ParameterSubscriptionValueSet"/>s that are to be created
        /// </param>
        /// <param name="valueArray">
        /// A <see cref="ValueArray{String}"/> where each slot is a "-"
        /// </param>
        /// <returns>
        /// A instance of <see cref="ParameterSubscriptionValueSet"/>
        /// </returns>
        ParameterSubscriptionValueSet CreateWithDefaultValueArray(Guid subscribedValueSetIid, ValueArray<string> valueArray);
    }
}
