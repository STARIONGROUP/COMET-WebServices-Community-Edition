// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterValueSetBaseExtensions.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-COMET Webservices Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Webservices Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Extensions
{
    using System.Linq;

    using CDP4Common.DTO;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Types;

    using ParameterValueSetBase = CDP4Common.DTO.ParameterValueSetBase;

    /// <summary>
    /// Extension class for <see cref="CDP4Common.DTO.ParameterValueSetBase"/> object
    /// </summary>
    public static class ParameterValueSetBaseExtensions
    {
        /// <summary>
        /// Checks the according <see cref="EngineeringModelSetup"/>'s AutoPublish settings and handles the update of <see cref="CDP4Common.DTO.ParameterValueSetBase"/>s
        /// </summary>
        /// <param name="parameterValueSet">The <see cref="ParameterValueSetBase"/></param>
        /// <param name="engineeringModelSetup">The <see cref="EngineeringModelSetup"/></param>
        public static bool TryAutoPublish(this ParameterValueSetBase parameterValueSet, EngineeringModelSetup engineeringModelSetup)
        {
            var isChanged = false;

            if (engineeringModelSetup.AutoPublish)
            {
                var valueSwitch = parameterValueSet.ValueSwitch;
                ValueArray<string> newValueArray = null;

                switch (valueSwitch)
                {
                    case ParameterSwitchKind.COMPUTED:
                    {
                        newValueArray = parameterValueSet.Computed;
                        break;
                    }
                    case ParameterSwitchKind.MANUAL:
                    {
                        newValueArray = parameterValueSet.Manual;
                        break;
                    }
                    case ParameterSwitchKind.REFERENCE:
                    {
                        newValueArray = parameterValueSet.Reference;
                        break;
                    }
                }

                if (newValueArray != null)
                {
                    if (!parameterValueSet.Published.SequenceEqual(newValueArray))
                    {
                        parameterValueSet.Published = new ValueArray<string>(newValueArray);
                        isChanged = true;
                    }
                }
            }

            return isChanged;
        }
    }
}
