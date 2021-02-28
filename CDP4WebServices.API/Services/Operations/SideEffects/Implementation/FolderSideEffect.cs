// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderSideEffect.cs" company="RHEA System S.A.">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common;
    using CDP4Common.DTO;

    using CometServer.Helpers;
    using CometServer.Services.Authorization;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="FolderSideEffect"/> class is to execute additional logic before and after a specific operation is performed.
    /// </summary>
    public sealed class FolderSideEffect : OperationSideEffect<Folder>
    {
        /// <summary>
        /// Gets or sets the <see cref="IFolderService"/>
        /// </summary>
        public IFolderService FolderService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IDomainFileStoreService"/>.
        /// </summary>
        public IDomainFileStoreService DomainFileStoreService { get; set; }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before a create operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
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
        public override bool BeforeCreate(
            Folder thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext)
        {
            if (thing.ContainingFolder != null)
            {
                this.ValidateContainingFolder(
                    thing,
                    container,
                    transaction,
                    partition,
                    securityContext,
                    (Guid)thing.ContainingFolder);
            }

            return true;
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before an update operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Folder"/> instance that will be inspected.
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
            Folder thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            ClasslessDTO rawUpdateInfo)
        {
            if (rawUpdateInfo.ContainsKey("ContainingFolder"))
            {
                var containingFolderId = (Guid)rawUpdateInfo["ContainingFolder"];

                this.ValidateContainingFolder(
                    thing,
                    container,
                    transaction,
                    partition,
                    securityContext,
                    containingFolderId);
            }

            this.CheckSecurity(thing, transaction, partition);
        }

        /// <summary>
        /// Allows derived classes to override and execute additional logic before a delete operation.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Thing"/> instance that will be inspected.
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
        public override void BeforeDelete(Folder thing, Thing container, NpgsqlTransaction transaction, string partition, ISecurityContext securityContext)
        {
            this.CheckSecurity(thing, transaction, partition);
        }

        /// <summary>
        /// Validate containing Folder for being acyclic.
        /// </summary>
        /// <param name="thing">
        /// The <see cref="Folder"/> instance that will be inspected.
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
        /// <param name="containingFolderId">
        /// The containing folder id to check for being acyclic
        /// </param>
        public void ValidateContainingFolder(
            Folder thing,
            Thing container,
            NpgsqlTransaction transaction,
            string partition,
            ISecurityContext securityContext,
            Guid containingFolderId)
        {
            // Check for itself
            if (containingFolderId == thing.Iid)
            {
                throw new AcyclicValidationException(
                    $"Folder {thing.Name} {thing.Iid} cannot have itself as a containing Folder.");
            }

            // Check that containing folder is from the same file store
            if (!((FileStore)container).Folder.Contains(containingFolderId))
            {
                throw new AcyclicValidationException(
                    $"Folder {thing.Name} {thing.Iid} cannot have a Folder from outside the current file store.");
            }

            // Get all folders from the container
            var folders = this.FolderService.Get(transaction, partition, ((FileStore)container).Folder, securityContext)
                .Cast<Folder>().ToList();

            // Check whether containing folder is acyclic
            if (!this.IsFolderAcyclic(folders, containingFolderId, thing.Iid))
            {
                throw new AcyclicValidationException(
                    $"Folder {thing.Name} {thing.Iid} cannot have a containing Folder {containingFolderId} that leads to cyclic dependency");
            }
        }

        /// <summary>
        /// Is containing folder acyclic.
        /// </summary>
        /// <param name="folders">
        /// The folders from the store.
        /// </param>
        /// <param name="containingFolderId">
        /// The containing folder to check for being acyclic.
        /// </param>
        /// <param name="folderId">
        /// The folder id to set a containing folder to.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> whether a containing folder will not lead to cyclic dependency.
        /// </returns>
        private bool IsFolderAcyclic(List<Folder> folders, Guid containingFolderId, Guid folderId)
        {
            Guid? nextContainingFolderId = containingFolderId;

            while (nextContainingFolderId != null)
            {
                if (nextContainingFolderId == folderId)
                {
                    return false;
                }

                nextContainingFolderId = folders.FirstOrDefault(x => x.Iid == nextContainingFolderId)?.ContainingFolder;
            }

            return true;
        }

        /// <summary>
        /// Checks the <see cref="DomainFileStore"/> security
        /// </summary>
        /// <param name="folder">
        /// The instance of the <see cref="Folder"/> that is inspected.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource will be stored.
        /// </param>
        private void CheckSecurity(Folder folder, NpgsqlTransaction transaction, string partition)
        {
            this.DomainFileStoreService.CheckSecurity(
                folder,
                transaction,
                partition);
        }
    }
}