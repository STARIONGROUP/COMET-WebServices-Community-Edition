// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterSubscriptionValueSetFactory.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Services
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
                throw new ArgumentException("The valueArray must be a default valueArray that only contains \"-\"", nameof(valueArray));
            }

            var parameterSubscriptionValueSet = new ParameterSubscriptionValueSet(Guid.NewGuid(), -1)
                        {
                            SubscribedValueSet = subscribedValueSetIid,
                            Manual = valueArray,
                            ValueSwitch = CDP4Common.EngineeringModelData.ParameterSwitchKind.MANUAL
                        };

            return parameterSubscriptionValueSet;
        }

        /// <summary>
        /// Create a <see cref="ParameterSubscriptionValueSet"/> from an old <see cref="ParameterSubscriptionValueSet"/>
        /// </summary>
        /// <param name="oldValueSet">The old <see cref="ParameterSubscriptionValueSet"/>. If null the default values are used</param>
        /// <param name="subscribedValueSetGuid">The subscribed value-set</param>
        /// <param name="defaultValueArray">The default value-array</param>
        /// <returns>The new <see cref="ParameterSubscriptionValueSet"/></returns>
        public ParameterSubscriptionValueSet CreateWithOldValues(ParameterSubscriptionValueSet oldValueSet, Guid subscribedValueSetGuid, ValueArray<string> defaultValueArray)
        {
            if (oldValueSet == null)
            {
                return this.CreateWithDefaultValueArray(subscribedValueSetGuid, defaultValueArray);
            }

            var parameterSubscriptionValueSet = new ParameterSubscriptionValueSet(Guid.NewGuid(), -1)
            {
                SubscribedValueSet = subscribedValueSetGuid,
                Manual = oldValueSet.Manual,
                ValueSwitch = oldValueSet.ValueSwitch
            };

            return parameterSubscriptionValueSet;
        }
    }
}
