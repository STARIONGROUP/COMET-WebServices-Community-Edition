// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileArchiveService.cs" company="Starion Group S.A.">
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
    using System.Collections.Generic;
    using System.IO;

    using CDP4Common.DTO;

    /// <summary>
    /// The purpose of the <see cref="IFileArchiveService"/> is to provide file functions for retrieval from the 
    /// file system in a zipped archive.
    /// </summary>
    public interface IFileArchiveService : IBusinessLogicService
    {
        /// <summary>
        /// Create Folders and Files on Disk and return the <see cref="DirectoryInfo"/> of the
        /// created top Folder
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
        /// The <see cref="DirectoryInfo"/> that holds a reference to the top folder that is created
        /// </returns>
        DirectoryInfo CreateFolderAndFileStructureOnDisk(List<Thing> resourceResponse, string partition, string[] routeSegments);

        /// <summary>
        /// Delete file structure and temporary ZIP archive.
        /// </summary>
        /// <param name="directoryInfo">
        /// The <see cref="DirectoryInfo"/> that is to be deleted. The name of the <see cref="DirectoryInfo"/>
        /// is equal to the name of the temporary ZIP archive that is deleted as well.
        /// </param>
        void DeleteFolderAndFileStructureAndArchive(DirectoryInfo directoryInfo);

        /// <summary>
        /// Create zip archive from the provided <see cref="DirectoryInfo"/>
        /// </summary>
        /// <param name="directoryInfo">
        /// The location of the top folder that is to be archived into a ZIP file
        /// </param>
        void CreateZipArchive(DirectoryInfo directoryInfo);
    }
}
