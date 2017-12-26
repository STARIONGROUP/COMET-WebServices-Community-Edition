// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderSideEffect.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.Operations.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common;
    using CDP4Common.DTO;

    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services.Authorization;

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
        public override void BeforeCreate(
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
                    string.Format("Folder {0} {1} cannot have itself as a containing Folder.", thing.Name, thing.Iid));
            }

            // Check that containing folder is from the same file store
            if (!((FileStore)container).Folder.Contains(containingFolderId))
            {
                throw new AcyclicValidationException(
                    string.Format(
                        "Folder {0} {1} cannot have a Folder from outside the current file store.",
                        thing.Name,
                        thing.Iid));
            }

            // Get all folders from the container
            var folders = this.FolderService.Get(transaction, partition, ((FileStore)container).Folder, securityContext)
                .Cast<Folder>().ToList();

            // Check whether containing folder is acyclic
            if (!this.IsFolderAcyclic(folders, containingFolderId, thing.Iid))
            {
                throw new AcyclicValidationException(
                    string.Format(
                        "Folder {0} {1} cannot have a containing Folder {2} that leads to cyclic dependency",
                        thing.Name,
                        thing.Iid,
                        containingFolderId));
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
    }
}