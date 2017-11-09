// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleBusinessLogicService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using CDP4Common.DTO;
    using CDP4Common.Types;

    /// <summary>
    /// The purpose of the <see cref="IParameterValueSetFactory"/> is to create <see cref="ParameterValueSet"/> instances
    /// </summary>
    public interface IParameterValueSetFactory : IBusinessLogicService
    {
        /// <summary>
        /// Creates a new <see cref="ParameterValueSet"/> where all the values are equal to a <see cref="ValueArray{String}"/> where each slot is a "-"
        /// and the <see cref="CDP4Common.EngineeringModelData.ParameterSwitchKind"/> is set to <see cref="CDP4Common.EngineeringModelData.ParameterSwitchKind.MANUAL"/>
        /// </summary>
        /// <param name="optionIid">
        /// The unique Id of te <see cref="Option"/> that is referenced by the <see cref="ParameterValueSet"/>
        /// </param>
        /// <param name="actualStateIid">
        /// The unique Id of te <see cref="ActualFiniteState"/> that is referenced by the <see cref="ParameterValueSet"/>
        /// </param>
        /// <param name="valueArray">
        /// A <see cref="ValueArray{String}"/> where each slot is a "-"
        /// </param>
        /// <returns>
        /// A instance of <see cref="ParameterValueSet"/>
        /// </returns>
        ParameterValueSet CreateWithDefaultValueArray(Guid? optionIid, Guid? actualStateIid, ValueArray<string> valueArray);
    }
}
