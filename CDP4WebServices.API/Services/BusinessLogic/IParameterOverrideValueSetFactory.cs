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
    /// The purpose of the <see cref="IParameterOverrideValueSetFactory"/> is to create <see cref="ParameterOverrideValueSet"/> instances
    /// </summary>
    public interface IParameterOverrideValueSetFactory : IBusinessLogicService
    {
        /// <summary>
        /// Creates a new <see cref="ParameterOverrideValueSet"/> where all the values are equal to a <see cref="ValueArray{String}"/> where each slot is a "-"
        /// and the <see cref="CDP4Common.EngineeringModelData.ParameterSwitchKind"/> is set to <see cref="CDP4Common.EngineeringModelData.ParameterSwitchKind.MANUAL"/>
        /// </summary>
        /// <param name="parameterValueSetIid">
        /// The unique Id of te <see cref="ParameterValueSet"/> that is referenced by the <see cref="ParameterOverrideValueSet"/>
        /// </param>
        /// <param name="valueArray">
        /// A <see cref="ValueArray{String}"/> where each slot is a "-"
        /// </param>
        /// <returns>
        /// A instance of <see cref="ParameterOverrideValueSet"/>
        /// </returns>
        ParameterOverrideValueSet CreateWithDefaultValueArray(Guid parameterValueSetIid, ValueArray<string> valueArray);

        /// <summary>
        /// Creates a new <see cref="ParameterOverrideValueSet"/> from a <see cref="ParameterValueSet"/>
        /// </summary>
        /// <param name="parameterValueSet">The <see cref="ParameterValueSet"/></param>
        /// <param name="owner">The owner</param>
        /// <returns>The <see cref="ParameterOverrideValueSet"/></returns>
        ParameterOverrideValueSet CreateParameterOverrideValueSet(ParameterValueSet parameterValueSet, Guid owner);
    }
}
