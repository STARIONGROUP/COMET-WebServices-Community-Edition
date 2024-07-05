// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIterationService.cs" company="Starion Group S.A.">
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

namespace CometServer.Services
{
    using Authorization;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// The Iteration Service Interface which uses the ORM layer to interact with the data model.
    /// </summary>
    public partial interface IIterationService
    {
        /// <summary>
        /// Copy the tables from a source to an Iteration partition
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="sourcePartition">
        /// The source iteration partition
        /// </param>
        /// <param name="targetPartition">
        /// The target iteration partition
        /// </param>
        void CopyIteration(NpgsqlTransaction transaction, string sourcePartition, string targetPartition);

        /// <summary>
        /// Copy the tables from a source to an Iteration partition and reset ITimeStampedThing.CreatedOn
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="sourcePartition">
        /// The source iteration partition
        /// </param>
        /// <param name="targetPartition">
        /// The target iteration partition
        /// </param>
        void CopyIterationAndResetCreatedOn(NpgsqlTransaction transaction, string sourcePartition, string targetPartition);

        /// <summary>
        /// Delete all organizational participations from element definitions
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="targetPartition">
        /// The target iteration partition
        /// </param>
        void DeleteAllrganizationalParticipantThings(NpgsqlTransaction transaction, string targetPartition);

        /// <summary>
        /// Copy the tables from a source to an Iteration partition
        /// </summary>
        /// <param name="transaction">
        /// The current transaction
        /// </param>
        /// <param name="sourcePartition">
        /// The source iteration partition
        /// </param>
        /// <param name="enable">
        /// A value indicating whether the user trigger shall be enabled
        /// </param>
        void ModifyUserTrigger(NpgsqlTransaction transaction, string sourcePartition, bool enable);

        /// <summary>
        /// Gets the active <see cref="Iteration"/> for the given <paramref name="partition"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="partition">The current partition</param>
        /// <param name="securityContext">The security context</param>
        /// <returns>The active <see cref="Iteration"/></returns>
        Iteration GetActiveIteration(NpgsqlTransaction transaction, string partition, ISecurityContext securityContext);

        /// <summary>
        /// Populates the data of a new iteration using the last iteration
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="iterationPartition">The iteration partition</param>
        /// <param name="iterationSetup">The new <see cref="IterationSetup"/></param>
        /// <param name="sourceIterationSetup">The source <see cref="IterationSetup"/></param>
        /// <param name="engineeringModel">The current <see cref="EngineeringModel"/></param>
        /// <param name="securityContext">The <see cref="ISecurityContext"/></param>
        void PopulateDataFromLastIteration(NpgsqlTransaction transaction, string iterationPartition, IterationSetup iterationSetup, IterationSetup sourceIterationSetup, EngineeringModel engineeringModel, ISecurityContext securityContext);

        /// <summary>
        /// Populates the data of a new iteration using a specific source <paramref name="sourceIterationSetup"/>
        /// </summary>
        /// <param name="transaction">The current transaction</param>
        /// <param name="iterationPartition">The iteration partition</param>
        /// <param name="iterationSetup">The new <see cref="IterationSetup"/></param>
        /// <param name="sourceIterationSetup">The source <see cref="IterationSetup"/></param>
        /// <param name="engineeringModel">The current <see cref="EngineeringModel"/></param>
        /// <param name="securityContext">The <see cref="ISecurityContext"/></param>
        void PopulateDataFromOlderIteration(NpgsqlTransaction transaction, string iterationPartition, IterationSetup iterationSetup, IterationSetup sourceIterationSetup,
            EngineeringModel engineeringModel, ISecurityContext securityContext);

    }
}