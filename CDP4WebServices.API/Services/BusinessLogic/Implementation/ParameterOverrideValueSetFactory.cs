// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleBusinessLogicService.cs" company="RHEA System S.A.">
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
    /// The purpose of the <see cref="ParameterOverrideValueSetFactory"/> is to create <see cref="ParameterOverrideValueSet"/> instances
    /// </summary>
    public class ParameterOverrideValueSetFactory : IParameterOverrideValueSetFactory
    {
        /// <summary>
        /// Creates a new <see cref="ParameterOverrideValueSet"/> where all the values are equal to a <see cref="ValueArray{String}"/> where each slot is a "-"
        /// and the <see cref="CDP4Common.EngineeringModelData.ParameterSwitchKind"/> is set to <see cref="CDP4Common.EngineeringModelData.ParameterSwitchKind.MANUAL"/>
        /// </summary>
        /// <param name="parameterValueSetIid">
        /// The unique Id of the <see cref="ParameterValueSet"/> that is referenced by the <see cref="ParameterOverrideValueSet"/>
        /// </param>
        /// <param name="valueArray">
        /// A <see cref="ValueArray{String}"/> where each slot is a "-"
        /// </param>
        /// <returns>
        /// A instance of <see cref="ParameterOverrideValueSet"/>
        /// </returns>
        public ParameterOverrideValueSet CreateWithDefaultValueArray(Guid parameterValueSetIid, ValueArray<string> valueArray)
        {
            if (valueArray.Any(value => value != "-"))
            {
                throw new ArgumentException("The valueArray must be a default valueArray that only contains \"-\"", nameof(valueArray));
            }

            var parameterOverrideValueSet = new ParameterOverrideValueSet(Guid.NewGuid(), -1)
                                                {
                                                    ParameterValueSet = parameterValueSetIid,
                                                    Manual = valueArray,
                                                    Computed = valueArray,
                                                    Reference = valueArray,
                                                    Formula = valueArray,
                                                    Published = valueArray,
                                                    ValueSwitch = CDP4Common.EngineeringModelData.ParameterSwitchKind.MANUAL
                                                };

            return parameterOverrideValueSet;
        }

        /// <summary>
        /// Creates a new <see cref="ParameterOverrideValueSet"/> from a <see cref="ParameterValueSet"/>
        /// </summary>
        /// <param name="parameterValueSet">The <see cref="ParameterValueSet"/></param>
        /// <returns>The <see cref="ParameterOverrideValueSet"/></returns>
        public ParameterOverrideValueSet CreateParameterOverrideValueSet(ParameterValueSet parameterValueSet)
        {
            if (parameterValueSet == null)
            {
                throw new ArgumentNullException(nameof(parameterValueSet), "The source ParameterValueSet cannot be null");
            }

            var parameterOverrideValueSet = new ParameterOverrideValueSet(Guid.NewGuid(), -1)
            {
                ParameterValueSet = parameterValueSet.Iid,
                Manual = parameterValueSet.Manual,
                Computed = parameterValueSet.Computed,
                Reference = parameterValueSet.Reference,
                Formula = parameterValueSet.Formula,
                Published = parameterValueSet.Published,
                ValueSwitch = parameterValueSet.ValueSwitch
            };

            return parameterOverrideValueSet;
        }

        /// <summary>
        /// Create a new <see cref="ParameterOverrideValueSet"/> given a source <see cref="ParameterOverrideValueSet"/>
        /// </summary>
        /// <param name="sourceValueSet">The source <see cref="ParameterOverrideValueSet"/></param>
        /// <param name="parameterValueSet">The associated <see cref="ParameterValueSet"/></param>
        /// <returns>The new <see cref="ParameterOverrideValueSet"/></returns>
        public ParameterOverrideValueSet CreateWithOldValues(ParameterOverrideValueSet sourceValueSet, ParameterValueSet parameterValueSet)
        {
            if (sourceValueSet == null)
            {
                return this.CreateParameterOverrideValueSet(parameterValueSet);
            }

            var parameterOverrideValueSet = new ParameterOverrideValueSet(Guid.NewGuid(), -1)
            {
                ParameterValueSet = parameterValueSet.Iid,
                Manual = sourceValueSet.Manual,
                Computed = sourceValueSet.Computed,
                Reference = sourceValueSet.Reference,
                Formula = sourceValueSet.Formula,
                Published = sourceValueSet.Published,
                ValueSwitch = sourceValueSet.ValueSwitch
            };

            return parameterOverrideValueSet;
        }
    }
}
