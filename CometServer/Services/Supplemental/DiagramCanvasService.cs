// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagramCanvasService.cs" company="RHEA System S.A.">
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

namespace CometServer.Services
{
    using System.Linq;
    using System.Security;

    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// The <see cref="DiagramCanvas"/> Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class DiagramCanvasService
    {
        /// <summary>
        /// Check whether a modify operation is allowed based on the object instance.
        /// </summary>
        /// <param name="transaction">
        /// The transaction to the database.
        /// </param>
        /// <param name="thing">
        /// The <see cref="Thing"/> to update.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) from where the requested resource will be written.
        /// </param>
        /// <param name="modifyOperation">
        /// The string representation of the type of the modify operation.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override bool IsInstanceModifyAllowed(NpgsqlTransaction transaction, Thing thing, string partition, string modifyOperation)
        {
            var result = base.IsInstanceModifyAllowed(transaction, thing, partition, modifyOperation);

            if (result)
            {
                result = this.HasWriteAccess(thing as DiagramCanvas, transaction, partition);
            }

            return result;
        }

        /// <summary>
        /// Check whether a read operation is allowed based on the object instance.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        ///  <param name="thing">
        /// The Thing to authorize a read request.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override bool IsInstanceReadAllowed(NpgsqlTransaction transaction, Thing thing, string partition)
        {
            var result = base.IsInstanceReadAllowed(transaction, thing, partition);

            if (result)
            {
                result = this.HasReadAccess(thing as DiagramCanvas, transaction, partition);
            }

            return result;
        }

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
        public bool HasReadAccess(DiagramCanvas thing, NpgsqlTransaction transaction, string partition)
        {
            if (thing is ArchitectureDiagram architectureDiagram)
            {
                // ArchitectureDiagram has its own logic, not compatible with its base class
                return this.ArchitectureDiagramService.HasReadAccess(architectureDiagram, transaction, partition);
            }

            this.IsLockedByAnotherUser(transaction, partition, thing, out var lockedByCurrentUser);

            if (!lockedByCurrentUser)
            {
                return !thing.IsHidden;
            }

            return true;
        }

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
        public bool HasWriteAccess(DiagramCanvas thing, NpgsqlTransaction transaction, string partition)
        {
            if (thing is ArchitectureDiagram architectureDiagram)
            {
                // ArchitectureDiagram has its own logic, not compatible with its base class
                return this.ArchitectureDiagramService.HasWriteAccess(architectureDiagram, transaction, partition);
            }

            var originalThing = this.DiagramCanvasDao.Read(transaction, partition, new[] { thing.Iid }).FirstOrDefault();

            if (originalThing == null)
            {
                return true;
            }

            if (this.IsLockedByAnotherUser(transaction, partition, originalThing, out _))
            {
                throw new SecurityException($"{nameof(DiagramCanvas)} '{thing.Name}' is locked by another user");
            }

            return true;
        }

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
        /// <param name="lockedByCurrentUser">Indicates that the <see cref="DiagramCanvas"/> is locked by the current rser</param>
        /// <returns>A value indicating that a <see cref="DiagramCanvas"/> is locked by another user, or not</returns>
        public bool IsLockedByAnotherUser(NpgsqlTransaction transaction, string partition, DiagramCanvas thing, out bool lockedByCurrentUser)
        {
            lockedByCurrentUser = false;

            if (thing?.LockedBy == null)
            {
                return false;
            }

            if (this.CredentialsService.Credentials.Person.Iid != thing?.LockedBy)
            {
                return true;
            }

            lockedByCurrentUser = true;
            return false;
        }
    }
}
