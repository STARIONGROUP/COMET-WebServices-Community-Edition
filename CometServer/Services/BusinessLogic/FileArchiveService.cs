// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileArchiveService.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.Types;

    using CometServer.Authorization;
    using CometServer.Configuration;
    using CometServer.Helpers;
    using CometServer.Services.Authorization;

    using ICSharpCode.SharpZipLib.Zip;

    using Microsoft.Extensions.Logging;

    using Npgsql;

    using File = CDP4Common.DTO.File;
    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// The purpose of the <see cref="FileArchiveService"/> is to provide file functions for for retrieval from the 
    /// file system in a zipped archive.
    /// </summary>
    public class FileArchiveService : IFileArchiveService
    {
        /// <summary>
        /// Gets or sets the (injected) <see cref="ILogger"/>
        /// </summary>
        public ILogger<FileArchiveService> Logger { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IAppConfigService"/>
        /// </summary>
        public IAppConfigService AppConfigService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="ICredentialsService"/>
        /// </summary>
        public ICredentialsService CredentialsService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="ICommonFileStoreService"/>
        /// </summary>
        public ICommonFileStoreService CommonFileStoreService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IDomainFileStoreService"/>
        /// </summary>
        public IDomainFileStoreService DomainFileStoreService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IFileTypeService"/>
        /// </summary>
        public IFileTypeService FileTypeService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="ICdp4TransactionManager"/>
        /// </summary>
        public ICdp4TransactionManager TransactionManager { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IFileBinaryService"/>
        /// </summary>
        public IFileBinaryService FileBinaryService { get; set; }

        /// <summary>
        /// Create file structure.
        /// </summary>
        /// <param name="resourceResponse">
        /// The resource response.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="routeSegments">
        /// The route segments.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Throws exception when response does not contain a thing that allows to get file data for.
        /// </exception>
        /// <returns>
        /// The <see cref="string"/> folder path where the file structure is created.
        /// </returns>
        public DirectoryInfo CreateFolderAndFileStructureOnDisk(List<Thing> resourceResponse, string partition, string[] routeSegments)
        {
            if (resourceResponse[0].ClassKind == ClassKind.CommonFileStore
                || resourceResponse[0].ClassKind == ClassKind.DomainFileStore
                || resourceResponse[0].ClassKind == ClassKind.Folder)
            {
                var temporaryFolderOnDiskDirectoryInfo = this.CreateTemporaryFolderOnDisk();

                try
                {
                    this.CreateFolderAndFileStructureOnDisk(resourceResponse[0], partition, temporaryFolderOnDiskDirectoryInfo.FullName, routeSegments);
                    return temporaryFolderOnDiskDirectoryInfo;
                }
                catch (Exception exception)
                {
                    this.Logger.LogError("An attempt to create a file structure was unsuccsessful. Exited with the error: {exceptionMessage}.", exception.Message);

                    temporaryFolderOnDiskDirectoryInfo.Delete(true);
                }
            }

            throw new InvalidOperationException("It is prohibited to get fileData for " + resourceResponse[0].ClassKind);
        }

        /// <summary>
        /// Create zip archive from the provided <see cref="DirectoryInfo"/>
        /// </summary>
        /// <param name="directoryInfo">
        /// The location of the top folder that is to be archived into a ZIP file
        /// </param>
        /// <remarks>
        /// Empty folders and folders are ignored as entries
        /// </remarks>
        public void CreateZipArchive(DirectoryInfo directoryInfo)
        {
            var fastZip = new FastZip();

            var zipArchive = Path.Combine(directoryInfo.Parent.FullName, $"{directoryInfo.Name}.zip");

            string fileFilter = null;
            string directoryFilter = null;

            fastZip.CreateZip(zipArchive, directoryInfo.FullName, true, fileFilter, directoryFilter);
        }

        /// <summary>
        /// Delete file structure and temporary ZIP archive.
        /// </summary>
        /// <param name="directoryInfo">
        /// The <see cref="DirectoryInfo"/> that is to be deleted. The name of the <see cref="DirectoryInfo"/>
        /// is equal to the name of the temporary ZIP archive that is deleted as well.
        /// </param>
        public void DeleteFolderAndFileStructureAndArchive(DirectoryInfo directoryInfo)
        {
            // Delete created file structure and archive from the disk
            directoryInfo.Delete(true);

            var zipArchive = Path.Combine(directoryInfo.Parent.FullName, $"{directoryInfo.Name}.zip");

            System.IO.File.Delete(zipArchive);

            this.Logger.LogInformation("File structure {folderPath} and archive {zipArchive}.zip are deleted.", directoryInfo.FullName, zipArchive);
        }

        /// <summary>
        /// Create a temporary folder on disk with a random name.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/> name of the created folder.
        /// </returns>
        private DirectoryInfo CreateTemporaryFolderOnDisk()
        {
            // Specify a name for a random folder.
            var folderPath = Guid.NewGuid().ToString();

            folderPath = Path.Combine(this.AppConfigService.AppConfig.Midtier.TemporaryFileStorageDirectory, folderPath);

            this.Logger.LogDebug("Creating temporary folder {folderPath}.", folderPath);

            var directoryInfo = Directory.CreateDirectory(folderPath);

            this.Logger.LogInformation("Temporary folder {folderPath} is created.", folderPath);

            return directoryInfo;
        }

        /// <summary>
        /// The create file structure on disk.
        /// </summary>
        /// <param name="thing">
        /// The thing to get file data for.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="folderPath">
        /// The folder path where the structure will be created.
        /// </param>
        /// <param name="routeSegments">
        /// The route segments.
        /// </param>
        private void CreateFolderAndFileStructureOnDisk(Thing thing, string partition, string folderPath, string[] routeSegments)
        {
            this.Logger.LogInformation("File structure creation is started into the temporary folder {folderPath}.", folderPath);

            var credentials = this.CredentialsService.Credentials;

            var authorizedContext = new RequestSecurityContext { ContainerReadAllowed = true };

            NpgsqlConnection connection = null;
            var transaction = this.TransactionManager.SetupTransaction(ref connection, credentials);

            try
            {
                var things = this.GetFileStoreThings(thing, routeSegments, partition, transaction, authorizedContext);

                if (things.Count != 0)
                {
                    // Get all folders
                    var folders = things.Where(i => i.GetType() == typeof(Folder)).Cast<Folder>().ToList();

                    // Get all files 
                    var files = things.Where(i => i.GetType() == typeof(File)).Cast<File>().ToList();

                    // Get all files 
                    var fileRevisions = things.Where(i => i.GetType() == typeof(FileRevision)).Cast<FileRevision>().ToList();

                    // Get all fileType iids
                    var fileTypeOrderedItems = new List<OrderedItem>();
                    foreach (var fileRevision in fileRevisions)
                    {
                        fileTypeOrderedItems.AddRange(fileRevision.FileType);
                    }

                    // Get fileTypes iids
                    var fileTypeIids = new List<Guid>();
                    foreach (var fileTypeOrderedItem in fileTypeOrderedItems)
                    {
                        fileTypeIids.Add(Guid.Parse(fileTypeOrderedItem.V.ToString()));
                    }

                    // Get all fileTypes
                    var fileTypes = this.FileTypeService
                        .GetShallow(transaction, "SiteDirectory", fileTypeIids.Distinct(), authorizedContext)
                        .OfType<FileType>().ToList();

                    if (thing.ClassKind != ClassKind.Folder)
                    {
                        // Get all root folders
                        var rootFolders = folders.Where(folder => folder.ContainingFolder == null);

                        // Iterate all root folders and find child folders and files for the file store
                        foreach (var rootFolder in rootFolders)
                        {
                            this.GetFoldersAndFilesAndCopy(rootFolder, folders, files, fileRevisions, folderPath, fileTypes);
                        }

                        // Get root files
                        this.GetAndCopyFiles(null, files, fileRevisions, folderPath, fileTypes);
                    }
                    else
                    {
                        // Find child folders and files for the folder
                        this.GetFoldersAndFilesAndCopy(thing as Folder, folders, files, fileRevisions, folderPath, fileTypes);
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }

                this.Logger.LogError(ex, "Failed to create a file structure on the disk.");
            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }

                if (connection != null)
                {
                    connection.Dispose();
                }
            }
        }

        /// <summary>
        /// Get all things from the fileStore.
        /// </summary>
        /// <param name="thing">
        /// The thing to get file data for.
        /// </param>
        /// <param name="routeSegments">
        /// The route segments.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="authorizedContext">
        /// The security context of the container instance.
        /// </param>
        /// <returns>
        /// The list of things form a fileStore.
        /// </returns>
        private List<Thing> GetFileStoreThings(Thing thing, string[] routeSegments, string partition, NpgsqlTransaction transaction, RequestSecurityContext authorizedContext)
        {
            var iids = new List<Guid>();

            if (routeSegments.Length == 8 && routeSegments[6] == "folder")
            {
                iids.Add(Guid.Parse(routeSegments[5]));
                return this.DomainFileStoreService.GetDeep(transaction, partition, iids, authorizedContext).ToList();
            }

            if (routeSegments.Length == 6 && routeSegments[4] == "folder")
            {
                iids.Add(Guid.Parse(routeSegments[3]));
                return this.CommonFileStoreService.GetDeep(transaction, partition, iids, authorizedContext).ToList();
            }

            if (routeSegments.Length == 6 && routeSegments[4] == "domainFileStore")
            {
                iids.Add(thing.Iid);
                return this.DomainFileStoreService.GetDeep(transaction, partition, iids, authorizedContext).ToList();
            }

            if (routeSegments.Length == 4 && routeSegments[2] == "commonFileStore")
            {
                iids.Add(thing.Iid);
                return this.CommonFileStoreService.GetDeep(transaction, partition, iids, authorizedContext).ToList();
            }

            return new List<Thing>();
        }

        /// <summary>
        /// Get folders and files for the given <paramref name="rootFolder"/> and copy the complete
        /// structure (including fiels) to the target <paramref name="folderPath"/>
        /// </summary>
        /// <param name="rootFolder">
        /// The root folder name where the structure will be created.
        /// </param>
        /// <param name="folders">
        /// The folders from the fileStore.
        /// </param>
        /// <param name="files">
        /// The files from the fileStore.
        /// </param>
        /// <param name="fileRevisions">
        /// The file revisions of files from the fileStore.
        /// </param>
        /// <param name="folderPath">
        /// The folder path where the structure will be created.
        /// </param>
        /// <param name="fileTypes">
        /// The file types of files from the fileStore.
        /// </param>
        private void GetFoldersAndFilesAndCopy(Folder rootFolder, List<Folder> folders, List<File> files, List<FileRevision> fileRevisions, string folderPath, List<FileType> fileTypes)
        {
            var path = Path.Combine(folderPath, rootFolder.Name);

            this.Logger.LogInformation("Starting to create Directory: {path}", path);
            
            Directory.CreateDirectory(path);

            this.Logger.LogInformation("Directory {path} is created.", path);

            // Recursively create all child folders
            // Get folders that is of the root folder
            var subFolders = folders.Where(folder => folder.ContainingFolder == rootFolder.Iid).ToList();

            if (subFolders.Count != 0)
            {
                foreach (var subFolder in subFolders)
                {
                    this.GetFoldersAndFilesAndCopy(subFolder, folders, files, fileRevisions, path, fileTypes);
                }
            }

            this.GetAndCopyFiles(rootFolder, files, fileRevisions, path, fileTypes);
        }

        /// <summary>
        /// Get and Copy the files from the <see cref="IFileBinaryService"/> to the target <paramref name="folderPath"/>
        /// </summary>
        /// <param name="rootFolder">
        /// The root folder name to get files for.
        /// </param>
        /// <param name="files">
        /// The files from the fileStore.
        /// </param>
        /// <param name="fileRevisions">
        /// The file revisions of files from the fileStore.
        /// </param>
        /// <param name="folderPath">
        /// The folder path where the files will be copied.
        /// </param>
        /// <param name="fileTypes">
        /// The file types.
        /// </param>
        private void GetAndCopyFiles(Folder rootFolder, List<File> files, List<FileRevision> fileRevisions, string folderPath, List<FileType> fileTypes)
        {
            var subFileRevisions = new List<FileRevision>();

            // Get all file revisions for the current folder
            foreach (var file in files)
            {
                var tempSubFileRevisions = rootFolder == null
                                               ? fileRevisions
                                                   .Where(
                                                       fileRevision => fileRevision.ContainingFolder == null
                                                                       && file.FileRevision.Contains(fileRevision.Iid))
                                                   .ToList()
                                               : fileRevisions
                                                   .Where(
                                                       fileRevision => fileRevision.ContainingFolder == rootFolder.Iid
                                                                       && file.FileRevision.Contains(fileRevision.Iid))
                                                   .ToList();

                if (tempSubFileRevisions.Count != 0)
                {
                    var subFileRevision = tempSubFileRevisions.Aggregate((i1, i2) => i1.RevisionNumber > i2.RevisionNumber ? i1 : i2);

                    subFileRevisions.Add(subFileRevision);
                }
            }

            // Copy all binary files from disk storage to the current folder
            foreach (var subFileRevision in subFileRevisions)
            {
                this.FileBinaryService.TryGetFileStoragePath(subFileRevision.ContentHash, out var storageFilePath);

                // Determine an extension for the file
                var extension = new StringBuilder();
                foreach (var orderedItem in subFileRevision.FileType)
                {
                    var tempExtension = fileTypes.Single(x => x.Iid == Guid.Parse(orderedItem.V.ToString()))
                        .Extension;

                    extension.Append('.').Append(tempExtension != "?" ? tempExtension : "unknown");
                }

                var destFile = Path.Combine(folderPath, $"{subFileRevision.Name}{extension}");

                this.Logger.LogDebug("Copying {storageFilePath} to {destFile}", storageFilePath, destFile);

                System.IO.File.Copy(storageFilePath, destFile, true);

                this.Logger.LogInformation("File {subFileRevisionContentHash}/{subFileRevisionName}{extension} is copied from {storageFilePath} to {folderPath}.", subFileRevision.ContentHash, subFileRevision.Name, extension, storageFilePath, folderPath);
            }
        }
    }
}
