// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiagramObjectService.cs" company="RHEA System S.A.">
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
    using CDP4Common.DTO;

    using CometServer.Services.Supplemental;

    using Npgsql;

    /// <summary>
    /// The <see cref="DiagramObject"/> Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class DiagramObjectService
    {
        /// <summary>
        /// Gets or sets the (injected) <see cref="IDiagramCanvasService"/>
        /// </summary>
        public IDiagramCanvasBusinessRuleService DiagramCanvasBusinessRuleService { get; set; }

        protected override bool IsInstanceModifyAllowed(NpgsqlTransaction transaction, Thing thing, string partition, string modifyOperation)
        {
            if (!base.IsInstanceModifyAllowed(transaction, thing, partition, modifyOperation))
            {
                return false;
            }

            return this.DiagramCanvasBusinessRuleService.IsWriteAllowed(transaction, thing, partition);
        }

        protected override bool IsInstanceReadAllowed(NpgsqlTransaction transaction, Thing thing, string partition)
        {
            if (!base.IsInstanceReadAllowed(transaction, thing, partition))
            {
                return false;
            }

            return this.DiagramCanvasBusinessRuleService.IsReadAllowed(transaction, thing, partition);
        }
    }
}
