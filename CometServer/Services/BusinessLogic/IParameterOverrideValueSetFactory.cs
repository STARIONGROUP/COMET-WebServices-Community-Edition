// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IParameterOverrideValueSetFactory.cs" company="RHEA System S.A.">
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
        /// <returns>The <see cref="ParameterOverrideValueSet"/></returns>
        ParameterOverrideValueSet CreateParameterOverrideValueSet(ParameterValueSet parameterValueSet);

        /// <summary>
        /// Create a new <see cref="ParameterOverrideValueSet"/> given a source <see cref="ParameterOverrideValueSet"/>
        /// </summary>
        /// <param name="sourceValueSet">The source <see cref="ParameterOverrideValueSet"/></param>
        /// <param name="parameterValueSet">The associated <see cref="ParameterValueSet"/></param>
        /// <returns>The new <see cref="ParameterOverrideValueSet"/></returns>
        ParameterOverrideValueSet CreateWithOldValues(ParameterOverrideValueSet sourceValueSet, ParameterValueSet parameterValueSet);
    }
}
