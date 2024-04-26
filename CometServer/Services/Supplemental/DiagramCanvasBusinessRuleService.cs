// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagramCanvasObjectCache.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
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
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Services.Supplemental
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    using CDP4Orm.Dao;

    using CometServer.Authorization;

    using Npgsql;

    using Thing = CDP4Common.DTO.Thing;

    public class DiagramCanvasBusinessRuleService : IDiagramCanvasBusinessRuleService
    {
        private Dictionary<Guid, List<Thing>> Cache { get; } = [];

        public IDiagramCanvasDao DiagramCanvasDao { get; set; }

        public IArchitectureDiagramDao ArchitectureDiagramDao { get; set; }

        public ICredentialsService CredentialsService { get; set; }

        public IPermissionService PermissionService { get; set; }

        public bool IsReadAllowed(NpgsqlTransaction transaction, Thing thing, string partition)
        {
            var topContainer = thing;

            if (topContainer is ArchitectureDiagram architectureDiagram)
            {
                if (architectureDiagram.IsHidden && !this.PermissionService.IsOwner(transaction, thing))
                {
                    return false;
                }

                return true;
            }

            if (topContainer is DiagramCanvas diagramCanvas)
            {
                this.IsLockedByAnotherUser(diagramCanvas, out var lockedByCurrentUser);

                if (!lockedByCurrentUser)
                {
                    return !diagramCanvas.IsHidden;
                }
            }

            return true;
        }

        public bool IsWriteAllowed(NpgsqlTransaction transaction, Thing thing, string partition)
        {
            var topContainer = thing;

            if (topContainer is ArchitectureDiagram architectureDiagram)
            {
                var originalThing = this.ArchitectureDiagramDao.Read(transaction, partition, new[] { architectureDiagram.Iid }).FirstOrDefault();

                if (originalThing == null)
                {
                    return true;
                }

                if (this.IsLockedByAnotherUser(originalThing, out _))
                {
                    throw new SecurityException($"{nameof(ArchitectureDiagram)} '{architectureDiagram.Name}' is locked by another user");
                }

                if (!this.PermissionService.IsOwner(transaction, architectureDiagram))
                {
                    throw new SecurityException($"User does not have correct ownership for {nameof(ArchitectureDiagram)} '{architectureDiagram.Name}'");
                }

                return true;
            }

            if (topContainer is DiagramCanvas diagramCanvas)
            {
                var originalThing = this.DiagramCanvasDao.Read(transaction, partition, new[] { diagramCanvas.Iid }).FirstOrDefault();

                if (originalThing == null)
                {
                    return true;
                }

                if (this.IsLockedByAnotherUser(originalThing, out _))
                {
                    throw new SecurityException($"{nameof(DiagramCanvas)} '{diagramCanvas.Name}' is locked by another user");
                }
            }

            return true;
        }

        /// <summary>
        /// Checks is a lock is present and throws an error when it is set by another user
        /// </summary>
        /// <param name="thing">
        /// The <see cref="DiagramCanvas"/> to check
        /// </param>
        /// <param name="lockedByCurrentUser">Indicates that the <see cref="DiagramCanvas"/> is locked by the current rser</param>
        /// <returns>A value indicating that a <see cref="DiagramCanvas"/> is locked by another user, or not</returns>
        public bool IsLockedByAnotherUser(DiagramCanvas thing, out bool lockedByCurrentUser)
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

        /// <summary>
        /// Checks business logic based on combination of IsHidden and LockedBy parameters
        /// </summary>
        /// <param name="classKind">The value of <see cref="DiagramCanvas.ClassKind"/></param>
        /// <param name="isHidden">The value of <see cref="DiagramCanvas.IsHidden"/></param>
        /// <param name="lockedBy">The <see cref="DiagramCanvas.LockedBy"/></param>
        /// <exception cref="Cdp4ModelValidationException">throws when business logic check fails</exception>
        public void CheckIsHiddenAndLockedBy(ClassKind classKind, bool isHidden, Guid? lockedBy)
        {
            if (classKind == ClassKind.DiagramCanvas)
            {
                // Not for sub types
                if (isHidden && lockedBy == null)
                {
                    throw new Cdp4ModelValidationException($"{nameof(DiagramCanvas)} cannot be set as hidden without also being locked by a specific {nameof(Person)}.");
                }
            }
        }

        /// <summary>
        /// Removes all the cached items.
        /// </summary>
        public void ClearAllCache()
        {
            this.Cache.Clear();
        }

        /// <summary>
        /// Removes the cached items for DiagramCanvas having Iid <paramref name="diagramCanvasIid"/>.
        /// </summary>
        public void ClearDiagramCanvasCache(Guid diagramCanvasIid)
        {
            if (this.Cache.ContainsKey(diagramCanvasIid))
            {
                this.Cache.Remove(diagramCanvasIid);
            }
        }
    }
}
