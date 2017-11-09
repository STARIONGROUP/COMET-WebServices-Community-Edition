// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileArchiveService.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 System RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System.Collections.Generic;

    using CDP4Common.DTO;

    /// <summary>
    /// The purpose of the <see cref="IFileArchiveService"/> is to provide file functions for retrieval from the 
    /// file system in a zipped archive.
    /// </summary>
    public interface IFileArchiveService : IBusinessLogicService
    {
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
        /// <returns>
        /// The <see cref="string"/> folder path where the file structure is created.
        /// </returns>
        string CreateFileStructure(List<Thing> resourceResponse, string partition, dynamic routeSegments);

        /// <summary>
        /// Delete file structure with archive.
        /// </summary>
        /// <param name="folderPath">
        /// The folder path where the file structure is created.
        /// </param>
        void DeleteFileStructureWithArchive(string folderPath);

        /// <summary>
        /// Create zip archive.
        /// </summary>
        /// <param name="folderPath">
        /// The folder path where the file structure for archiving is created..
        /// </param>
        void CreateZipArchive(string folderPath);
    }
}