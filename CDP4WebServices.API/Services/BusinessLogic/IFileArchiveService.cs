// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileArchiveService.cs" company="RHEA System S.A.">
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
