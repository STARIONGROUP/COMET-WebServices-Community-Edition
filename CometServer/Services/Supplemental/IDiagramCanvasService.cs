// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDiagramCanvasService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, 
//            Antoine Théate, Omar Elebiary, Jaime Bernar
//
//    This file is part of CDP4-COMET Web Services Community Edition. 
//    The CDP4-COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
//
//    The CDP4-COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using Npgsql;

namespace CometServer.Services
{
    using CDP4Common.DTO;

    /// <summary>
    /// The DiagramCanvas Service Interface which uses the ORM layer to interact with the data model.
    /// </summary>
    public partial interface IDiagramCanvasService
    {
        /// <summary>
        /// Checks the <see cref="DiagramCanvas"/> READ security
        /// </summary>
        /// <param name="thing">
        /// The instance of the <see cref="DiagramCanvas"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <returns>A boolean value indicating if read is allowed</returns>
        bool HasReadAccess(DiagramCanvas thing, NpgsqlTransaction transaction, string partition);

        /// <summary>
        /// Checks the <see cref="DiagramCanvas"/> WRITE security
        /// </summary>
        /// <param name="thing">
        /// The instance of the <see cref="DiagramCanvas"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <returns>A boolean value indicating if write is allowed</returns>
        bool HasWriteAccess(DiagramCanvas thing, NpgsqlTransaction transaction, string partition);

        /// <summary>
        /// Checks is a lock is present and throws an error when it is set by another user
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="thing">
        /// The <see cref="DiagramCanvas"/> to check
        /// </param>
        /// <param name="lockedByCurrentUser">Indicates that the <see cref="DiagramCanvas"/> is locked by the current user</param>
        /// <returns>A value indicating that a <see cref="DiagramCanvas"/> is locked by another user, or not</returns>
        bool IsLockedByAnotherUser(NpgsqlTransaction transaction, string partition, DiagramCanvas thing, out bool lockedByCurrentUser);
    }
}
