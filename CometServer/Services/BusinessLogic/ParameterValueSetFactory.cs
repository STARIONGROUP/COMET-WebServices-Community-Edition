// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterValueSetFactory.cs" company="RHEA System S.A.">
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
