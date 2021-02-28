// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterOverrideValueSetFactory.cs" company="RHEA System S.A.">
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
