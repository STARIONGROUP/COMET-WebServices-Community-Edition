// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterValueSetFactory.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Linq;
    using CDP4Common.DTO;
    using CDP4Common.Types;

    /// <summary>
    /// The purpose of the <see cref="ParameterValueSetFactory"/> is to create <see cref="ParameterValueSet"/> instances
    /// </summary>
    public class ParameterValueSetFactory : IParameterValueSetFactory
    {
        /// <summary>
        /// Creates a new <see cref="ParameterValueSet"/> where all the values are equal to a <see cref="ValueArray{String}"/> where each slot is a "-"
        /// and the <see cref="CDP4Common.EngineeringModelData.ParameterSwitchKind"/> is set to <see cref="CDP4Common.EngineeringModelData.ParameterSwitchKind.MANUAL"/>
        /// </summary>
        /// <param name="optionIid">
        /// The unique Id of the <see cref="Option"/> that is referenced by the <see cref="ParameterValueSet"/>
        /// </param>
        /// <param name="actualStateIid">
        /// The unique Id of the <see cref="ActualFiniteState"/> that is referenced by the <see cref="ParameterValueSet"/>
        /// </param>
        /// <param name="valueArray">
        /// A <see cref="ValueArray{String}"/> where each slot is a "-"
        /// </param>
        /// <returns>
        /// A instance of <see cref="ParameterValueSet"/>
        /// </returns>
        public ParameterValueSet CreateWithDefaultValueArray(Guid? optionIid, Guid? actualStateIid, ValueArray<string> valueArray)
        {
            if (valueArray.Any(value => value != "-"))
            {
                throw new ArgumentException("The valueArray must be a default valueArray that only contains \"-\"", "valueArray");
            }

            var parameterValueSet = new ParameterValueSet(Guid.NewGuid(), -1)
            {
                ActualOption = optionIid,
                ActualState = actualStateIid,
                Manual = valueArray,
                Computed = valueArray,
                Reference = valueArray,
                Formula = valueArray,
                Published = valueArray,
                ValueSwitch = CDP4Common.EngineeringModelData.ParameterSwitchKind.MANUAL
            };

            return parameterValueSet;
        }
    }
}
