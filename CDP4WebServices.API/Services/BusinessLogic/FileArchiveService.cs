// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileArchiveService.cs" company="RHEA System S.A.">
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

namespace CometServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.Types;

    using CometServer.Helpers;
    using CometServer.Services.Authentication;
    using CometServer.Services.Authorization;

    using Ionic.Zip;

    using NLog;

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
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the CDP4 request context.
        /// </summary>
        public ICdp4RequestContext Cdp4Context { get; set; }

        /// <summary>
        /// Gets or sets the commonFileStore service.
        /// </summary>
        public ICommonFileStoreService CommonFileStoreService { get; set; }

        /// <summary>
        /// Gets or sets the domainFileStore service.
        /// </summary>
        public IDomainFileStoreService DomainFileStoreService { get; set; }

        /// <summary>
        /// Gets or sets the file type service.
        /// </summary>
        public IFileTypeService FileTypeService { get; set; }

        /// <summary>
        /// Gets or sets the transaction manager for this request.
        /// </summary>
        public ICdp4TransactionManager TransactionManager { get; set; }

        /// <summary>
        /// Gets or sets the file binary service.
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
        public string CreateFileStructure(List<Thing> resourceResponse, string partition, dynamic routeSegments)
        {
            if (resourceResponse[0].ClassKind == ClassKind.CommonFileStore
                || resourceResponse[0].ClassKind == ClassKind.DomainFileStore
                || resourceResponse[0].ClassKind == ClassKind.Folder)
            {
                var folderPath = this.CreateTemporaryFolderOnDisk();
                try
                {
                    this.CreateFileStructureOnDisk(resourceResponse[0], partition, folderPath, routeSegments);
                    return folderPath;
                }
                catch (Exception exception)
                {
                    var logMessage = string.Format(
                        "An attempt to create a file structure was unsuccsessful. Exited with the error: {0}.",
                        exception.Message);
                    Logger.Error(logMessage);

                    this.DeleteFileStructure(folderPath);
                }
            }

            throw new InvalidOperationException(
                "It is prohibited to get fileData for " + resourceResponse[0].ClassKind);
        }

        /// <summary>
        /// Create zip archive.
        /// </summary>
        /// <param name="folderPath">
        /// The folder path where the file structure for archiving is created..
        /// </param>
        public void CreateZipArchive(string folderPath)
        {
            // Create a zip archive from the created file structure
            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(folderPath);
                zip.Save(folderPath + ".zip");
            }
        }

        /// <summary>
        /// Delete file structure with archive.
        /// </summary>
        /// <param name="folderPath">
        /// The folder path where the file structure is created.
        /// </param>
        public void DeleteFileStructureWithArchive(string folderPath)
        {
            // Delete created file structure and archive from the disk
            Directory.Delete(folderPath, true);
            System.IO.File.Delete(folderPath + ".zip");

            var logMessage = string.Format(
                "File structure {0} and archive {1} are deleted.",
                folderPath,
                folderPath + ".zip");
            Logger.Info(logMessage);
        }

        /// <summary>
        /// Delete file structure.
        /// </summary>
        /// <param name="folderPath">
        /// The folder path where the file structure is created.
        /// </param>
        private void DeleteFileStructure(string folderPath)
        {
            Directory.Delete(folderPath, true);

            var logMessage = string.Format("File structure {0} is deleted.", folderPath);
            Logger.Info(logMessage);
        }

        /// <summary>
        /// Create a temporary folder on disk with a random name.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/> name of the created folder.
        /// </returns>
        private string CreateTemporaryFolderOnDisk()
        {
            // Specify a name for a random folder.
            string folderPath = Guid.NewGuid().ToString();

            Directory.CreateDirectory(folderPath);

            var logMessage = string.Format("Temporary folder {0} is created.", folderPath);
            Logger.Info(logMessage);

            return folderPath;
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
        private void CreateFileStructureOnDisk(Thing thing, string partition, string folderPath, dynamic routeSegments)
        {
            var logMessage = string.Format(
                "File structure creation is started into the temporary folder {0}.",
                folderPath);
            Logger.Info(logMessage);

            var credentials = this.Cdp4Context.Context.CurrentUser as Credentials;
            var authorizedContext = new RequestSecurityContext { ContainerReadAllowed = true };

            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = this.TransactionManager.SetupTransaction(ref connection, credentials);

            try
            {
                List<Thing> things = this.GetFileStoreThings(
                    thing,
                    routeSegments,
                    partition,
                    transaction,
                    authorizedContext);

                if (things.Any())
                {
                    // Get all folders
                    var folders = things.Where(i => i.GetType() == typeof(Folder)).Cast<Folder>().ToList();

                    // Get all files 
                    var files = things.Where(i => i.GetType() == typeof(File))
                        .Cast<File>().ToList();

                    // Get all files 
                    var fileRevisions = things.Where(i => i.GetType() == typeof(FileRevision)).Cast<FileRevision>()
                        .ToList();

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
                            this.GetFoldersAndFiles(rootFolder, folders, files, fileRevisions, folderPath, fileTypes);
                        }

                        // Get root files
                        this.GetFiles(null, files, fileRevisions, folderPath, fileTypes);
                    }
                    else
                    {
                        // Find child folders and files for the folder
                        this.GetFoldersAndFiles(thing as Folder, folders, files, fileRevisions, folderPath, fileTypes);
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                if (transaction != null && !transaction.IsCompleted)
                {
                    transaction.Rollback();
                }

                Logger.Error(ex, "Failed to create a file structure on the disk.");
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
        private List<Thing> GetFileStoreThings(
            Thing thing,
            dynamic routeSegments,
            string partition,
            NpgsqlTransaction transaction,
            RequestSecurityContext authorizedContext)
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
        /// Get folders and files for the given root folder.
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
        private void GetFoldersAndFiles(
            Folder rootFolder,
            List<Folder> folders,
            List<File> files,
            List<FileRevision> fileRevisions,
            string folderPath,
            List<FileType> fileTypes)
        {
            var path = folderPath + "\\" + rootFolder.Name;
            Directory.CreateDirectory(path);

            var logMessage = string.Format("Directory {0} is created.", path);
            Logger.Info(logMessage);

            // Recursively create all child folders
            // Get folders that is of the root folder
            var subFolders = folders.Where(folder => folder.ContainingFolder == rootFolder.Iid).ToList();

            if (subFolders.Any())
            {
                foreach (var subFolder in subFolders)
                {
                    this.GetFoldersAndFiles(subFolder, folders, files, fileRevisions, path, fileTypes);
                }
            }

            this.GetFiles(rootFolder, files, fileRevisions, path, fileTypes);
        }

        /// <summary>
        /// Get files for the given root folder.
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
        private void GetFiles(
            Folder rootFolder,
            List<File> files,
            List<FileRevision> fileRevisions,
            string folderPath,
            List<FileType> fileTypes)
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

                if (tempSubFileRevisions.Any())
                {
                    var subFileRevision =
                        tempSubFileRevisions.Aggregate((i1, i2) => i1.RevisionNumber > i2.RevisionNumber ? i1 : i2);
                    subFileRevisions.Add(subFileRevision);
                }
            }

            // Copy all binary files from disk storage to the current folder
            foreach (var subFileRevision in subFileRevisions)
            {
                string storageFilePath;
                this.FileBinaryService.TryGetFileStoragePath(subFileRevision.ContentHash, out storageFilePath);

                // Determine an extension for the file
                string extension = string.Empty;
                foreach (var orderedItem in subFileRevision.FileType)
                {
                    string tempExtension = fileTypes.Single(x => x.Iid == Guid.Parse(orderedItem.V.ToString()))
                        .Extension;

                    extension += "." + (tempExtension != "?" ? tempExtension : "unknown");
                }

                // Use Path class to manipulate file and directory paths.
                string destFile = Path.Combine(folderPath, subFileRevision.Name + extension);
                System.IO.File.Copy(storageFilePath, destFile, true);

                var logMessage = string.Format(
                    "File {0}/{1} is copied from {2} to {3}.",
                    subFileRevision.ContentHash,
                    subFileRevision.Name + extension,
                    storageFilePath,
                    folderPath);

                Logger.Info(logMessage);
            }
        }
    }
}
