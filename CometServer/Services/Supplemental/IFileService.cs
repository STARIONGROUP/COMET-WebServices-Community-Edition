// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileService.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
//
//    Author: Sam Geren�, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Th�ate
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
    /// The File Service Interface which uses the ORM layer to interact with the data model.
    /// </summary>
    public partial interface IFileService
    {
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
        bool IsAllowedAccordingToIsHidden(NpgsqlTransaction transaction, Thing thing, string partition);

        /// <summary>
        /// Checks is a file lock is present and throws an error when it is set by another user
        /// </summary>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="file">
        /// The <see cref="File"/> to check
        /// </param>
        void CheckFileLock(NpgsqlTransaction transaction, string partition, File file);
    }
}
