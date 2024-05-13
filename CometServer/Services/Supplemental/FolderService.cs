// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderService.cs" company="Starion Group S.A.">
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
    using CDP4Common.DTO;

    using Npgsql;

    /// <summary>
    /// The <see cref="Folder"/> Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class FolderService
    {
        /// <summary>
        /// Gets or sets the <see cref="IDomainFileStoreService"/>.
        /// </summary>
        public IDomainFileStoreService DomainFileStoreService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICommonFileStoreService"/>.
        /// </summary>
        public ICommonFileStoreService CommonFileStoreService { get; set; }

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
                if (partition.StartsWith("EngineeringModel_"))
                {
                    return true;
                }

                result = this.IsAllowedAccordingToIsHidden(transaction, thing, partition);
            }

            return result;
        }

        /// <summary>
        /// Checks if the <see cref="Participant"/> is allowed to read (and therefore also write to) a <see cref="DomainFileStore"/>
        /// based on the state of the <see cref="DomainFileStore"/>'s <see cref="DomainFileStore.IsHidden"/> property.
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="thing">
        /// The <see cref="Thing"/> to check.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsAllowedAccordingToIsHidden(NpgsqlTransaction transaction, Thing thing, string partition)
        {
            if (partition.StartsWith("EngineeringModel_"))
            {
                return this.CommonFileStoreService.HasReadAccess(thing, transaction, partition);
            }
            else
            {
                return this.DomainFileStoreService.HasReadAccess(thing, transaction, partition);
            }
        }
    }
}
