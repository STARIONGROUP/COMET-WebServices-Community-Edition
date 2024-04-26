// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDiagramCanvasObjectCache.cs" company="RHEA System S.A.">
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

    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    using Npgsql;

    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// Defines the properties and methods of a DiagramCanvas Object tree
    /// </summary>
    public interface IDiagramCanvasBusinessRuleService
    {
        bool IsReadAllowed(NpgsqlTransaction transaction, Thing thing, string partition);

        bool IsWriteAllowed(NpgsqlTransaction transaction, Thing thing, string partition);

        //IEnumerable<Thing> ReadDiagramCanvas(DiagramThingBase diagramThing);

        /// <summary>
        /// Removes all the cached items.
        /// </summary>
        void ClearAllCache();

        /// <summary>
        /// Removes the cached items for DiagramCanvas having Iid <paramref name="diagramCanvasIid"/>.
        /// </summary>
        void ClearDiagramCanvasCache(Guid diagramCanvasIid);

        /// <summary>
        /// Checks business logic based on combination of IsHidden and LockedBy parameters
        /// </summary>
        /// <param name="classKind">The value of <see cref="DiagramCanvas.ClassKind"/></param>
        /// <param name="isHidden">The value of <see cref="DiagramCanvas.IsHidden"/></param>
        /// <param name="lockedBy">The <see cref="DiagramCanvas.LockedBy"/></param>
        /// <exception cref="Cdp4ModelValidationException">throws when business logic check fails</exception>
        void CheckIsHiddenAndLockedBy(ClassKind classKind, bool isHidden, Guid? lockedBy);
    }
}
