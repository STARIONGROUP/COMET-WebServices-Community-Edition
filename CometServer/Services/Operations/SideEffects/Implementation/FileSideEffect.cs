// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSideEffect.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Services.Operations.SideEffects
{
    using CDP4Common;
    using CDP4Common.DTO;

    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="FileSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class FileSideEffect : OperationSideEffect<File>
    {
        /// <summary>
        /// Gets or sets the <see cref="IDomainFileStoreService"/>.
        /// </summary>
        public IDomainFileStoreService DomainFileStoreService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IFileService"/>.
        /// </summary>
        public IFileService FileService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="File"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        /// <param name="rawUpdateInfo">
        /// The raw update info that was serialized from the user posted request.
        /// The <see cref="ClasslessDTO"/> instance only contains values for properties that are to be updated.
        /// It is important to note that this variable is not to be changed likely as it can/will change the operation processor outcome.
        /// </param>
        public override void BeforeUpdate(
            File thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            this.CheckSecurity(thing, transaction, partition);
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before a delete operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="File"/> instance that will be inspected.
        /// </param>
        /// <param name="container">
        /// The container instance of the <see cref="Thing"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        /// <param name="securityContext">
        /// The security Context used for permission checking.
        /// </param>
        public override void BeforeDelete(File thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            this.CheckSecurity(thing, transaction, partition);
        }

        /// <summary>
        /// Checks the <see cref="DomainFileStore"/> and file lock security
        /// </summary>
        /// <param name="file">
        /// The instance of the <see cref="File"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        private void CheckSecurity(File file, NpgsqlTransaction transaction, string partition)
        {
            this.FileService.CheckFileLock(transaction, partition, file);

            this.DomainFileStoreService.CheckSecurity(
                file,
                transaction,
                partition);
        }
    }
}