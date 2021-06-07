// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterGroupService.cs" company="RHEA System S.A.">
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
    using System.Collections.Generic;
    
    using Authorization;
    
    using CDP4Common.Dto;
    using CDP4Common.DTO;
    
    using Npgsql;

    /// <summary>
    /// Extension for the code-generated <see cref="ParameterGroupService"/>
    /// </summary>
    public partial class ParameterGroupService
    {
        /// <summary>
        /// Copy the <paramref name="sourceThing"/> into the target <paramref name="partition"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="sourceThing">The source <see cref="Thing"/> to copy</param>
        /// <param name="targetContainer">The target container <see cref="Thing"/></param>
        /// <param name="allSourceThings">All source <see cref="Thing"/>s in the current copy operation</param>
        /// <param name="copyinfo">The <see cref="CopyInfo"/></param>
        /// <param name="sourceToCopyMap">A dictionary mapping identifiers of original to copy</param>
        /// <param name="rdls">The <see cref="ReferenceDataLibrary"/></param>
        /// <param name="targetEngineeringModelSetup">The target <see cref="EngineeringModelSetup"/></param>
        /// <param name="securityContext">The <see cref="ISecurityContext"/></param>
        public override void Copy(NpgsqlTransaction transaction, string partition, Thing sourceThing, Thing targetContainer, IReadOnlyList<Thing> allSourceThings, CopyInfo copyinfo,
            Dictionary<Guid, Guid> sourceToCopyMap, IReadOnlyList<ReferenceDataLibrary> rdls, EngineeringModelSetup targetEngineeringModelSetup, ISecurityContext securityContext)
        {
            if (!(sourceThing is ParameterGroup sourceParameterGroup))
            {
                throw new InvalidOperationException("The source is not of the right type");
            }

            var copy = sourceParameterGroup.DeepClone<ParameterGroup>();
            copy.Iid = sourceToCopyMap[sourceParameterGroup.Iid];

            if (copy.ContainingGroup.HasValue)
            {
                copy.ContainingGroup = sourceToCopyMap[copy.ContainingGroup.Value];
            }

            this.ParameterGroupDao.Write(transaction, partition, copy, targetContainer);
        }
    }
}
