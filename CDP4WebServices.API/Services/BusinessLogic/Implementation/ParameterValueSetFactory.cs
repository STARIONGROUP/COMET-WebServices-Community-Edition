// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterValueSetFactory.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2018 RHEA System S.A.
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
        /// <param name="sourceValueSet">
        /// The source <see cref="ParameterValueSet"/> that the new <see cref="ParameterValueSet"/> will be created from
        /// </param>
        /// <param name="valueArray">
        /// A <see cref="ValueArray{String}"/> where each slot is a "-"
        /// </param>
        /// <returns>
        /// A instance of <see cref="ParameterValueSet"/>
        /// </returns>
        public ParameterValueSet CreateNewParameterValueSetFromSource(Guid? optionIid, Guid? actualStateIid, ParameterValueSet sourceValueSet, ValueArray<string> valueArray)
        {
            if (valueArray.Any(value => value != "-"))
            {
                throw new ArgumentException("The valueArray must be a default valueArray that only contains \"-\"", "valueArray");
            }

            var parameterValueSet = new ParameterValueSet(Guid.NewGuid(), -1)
            {
                ActualOption = optionIid,
                ActualState = actualStateIid,
                Manual = sourceValueSet != null ? sourceValueSet.Manual : valueArray,
                Computed = sourceValueSet != null ? sourceValueSet.Computed : valueArray,
                Reference = sourceValueSet != null ? sourceValueSet.Reference : valueArray,
                Formula = sourceValueSet != null ? sourceValueSet.Formula : valueArray,
                Published = sourceValueSet != null ? sourceValueSet.Published : valueArray,
                ValueSwitch = sourceValueSet != null ? sourceValueSet.ValueSwitch : CDP4Common.EngineeringModelData.ParameterSwitchKind.MANUAL
            };

            return parameterValueSet;
        }
    }
}
