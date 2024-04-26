// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModelCreatorManager.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

namespace CometServer.Services.Operations
{
    using System;

    using Authorization;
    
    using CDP4Common.DTO;
    
    using Npgsql;

    /// <summary>
    /// The interface for classes that handles the creation of an <see cref="EngineeringModel"/> based on a source
    /// </summary>
    public interface IModelCreatorManager
    {
        /// <summary>
        /// Gets a value that indicates whether tre user trigger were disabled
        /// </summary>
        bool IsUserTriggerDisable { get; }

        /// <summary>
        /// Creates an <see cref="EngineeringModelSetup"/> from a source
        /// </summary>
        /// <param name="source">The identifier of the source <see cref="EngineeringModelSetup"/></param>
        /// <param name="setupToCreate">The new <see cref="EngineeringModelSetup"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="securityContext">The security context</param>
        void CreateEngineeringModelSetupFromSource(Guid source, EngineeringModelSetup setupToCreate, NpgsqlTransaction transaction, ISecurityContext securityContext);

        /// <summary>
        /// Copy the engineering-model data from the source to the target <see cref="EngineeringModel"/>
        /// </summary>
        /// <param name="newModelSetup">The new <see cref="EngineeringModelSetup"/></param>
        /// <param name="transaction">The current transaction</param>
        /// <param name="securityContext">The security context</param>
        void CopyEngineeringModelData(EngineeringModelSetup newModelSetup, NpgsqlTransaction transaction, ISecurityContext securityContext);

        /// <summary>
        /// Enable the user-triggers in the engineering-model and iteration schema
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        void EnableUserTrigger(NpgsqlTransaction transaction);
    }
}