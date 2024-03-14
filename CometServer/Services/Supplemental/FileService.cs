// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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
    using System.Linq;
    using System.Security;

    using CDP4Common.DTO;

    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The <see cref="File"/> Service which uses the ORM layer to interact with the data model.
    /// </summary>
    public sealed partial class FileService
    {
        /// <summary>
        /// Gets or sets the <see cref="IDomainFileStoreService"/>.
        /// </summary>
        public IDomainFileStoreService DomainFileStoreService { get; set; }

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
                result = this.IsAllowedAccordingToIsHidden(transaction, thing, partition);
            }

            return result;
        }

        /// <summary>
        /// Checks if the <see cref="Participant"/> is allowed to read (and therefore also write to) a <see cref="DomainFileStore"/>
        /// based on the state of the <see cref="DomainFileStore"/>'s <see cref="DomainFileStore.IsHidden"/> property.
        /// This is not applicable to <see cref="CommonFileStore"/> data.
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
            if (partition.StartsWith("Iteration"))
            {
                return this.DomainFileStoreService.HasReadAccess(thing, transaction, partition);
            }

            return true;
        }

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
        public void CheckFileLock(NpgsqlTransaction transaction, string partition, File file)
        {
            var currentStoredFile = this.GetShallow(transaction, partition, new [] {file.Iid}, new RequestSecurityContext { ContainerReadAllowed = true }).FirstOrDefault() as File;

            if (!new object[] { this.CredentialsService.Credentials.Person.Iid, null }.Contains(currentStoredFile?.LockedBy))
            {
                throw new SecurityException($"{nameof(File)} is locked by another user");
            }
        }
    }
}
