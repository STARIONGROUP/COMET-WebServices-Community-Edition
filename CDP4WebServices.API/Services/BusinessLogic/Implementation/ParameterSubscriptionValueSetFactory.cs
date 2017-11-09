// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterSubscriptionValueSetFactory.cs" company="RHEA System S.A.">
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
    /// The purpose of the <see cref="IParameterSubscriptionValueSetFactory"/> is to create <see cref="ParameterSubscriptionValueSet"/> instances
    /// </summary>
    public class ParameterSubscriptionValueSetFactory : IParameterSubscriptionValueSetFactory
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
        public ParameterSubscriptionValueSet CreateWithDefaultValueArray(Guid subscribedValueSetIid, ValueArray<string> valueArray)
        {
            if (valueArray.Any(value => value != "-"))
            {
                throw new ArgumentException("The valueArray must be a default valueArray that only contains \"-\"", "valueArray");
            }

            var parameterSubscriptionValueSet = new ParameterSubscriptionValueSet(Guid.NewGuid(), -1)
                        {
                            SubscribedValueSet = subscribedValueSetIid,
                            Manual = valueArray,
                            ValueSwitch = CDP4Common.EngineeringModelData.ParameterSwitchKind.MANUAL                            
                        };

            return parameterSubscriptionValueSet;
        }
    }
}
