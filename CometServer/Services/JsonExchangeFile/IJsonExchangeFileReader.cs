// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJsonExchangeFileReader.cs" company="RHEA System S.A.">
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

    using CDP4Common.DTO;

    using CDP4JsonSerializer;

    using CDP4Orm.Dao;

    using System.Collections.Generic;

    /// <summary>
    /// The <see cref="IJsonExchangeFileReader"/> interface that defines methods to read data from
    /// an E-TM-10-25 Annex C3 archive
    /// </summary>
    public interface IJsonExchangeFileReader
    {
        /// <summary>
        /// Get the site directory from file.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The site directory contained <see cref="Thing"/> collection.
        /// </returns>
        IEnumerable<Thing> ReadSiteDirectoryFromfile(Version version, string filePath, string password);

        /// <summary>
        /// Get the engineering model from file.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="engineeringModelSetup">
        /// The engineering model setup.
        /// </param>
        /// <returns>
        /// The deserialized engineering model contained <see cref="Thing"/> collection.
        /// </returns>
        IEnumerable<Thing> ReadEngineeringModelFromfile(Version version, string filePath, string password, EngineeringModelSetup engineeringModelSetup);

        /// <summary>
        /// Get the model iteration from file.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="iterationSetup">
        /// The iteration setup.
        /// </param>
        /// <returns>
        /// The deserialized iteration contained <see cref="Thing"/> collection.
        /// </returns>
        IEnumerable<Thing> ReadModelIterationFromFile(Version version, string filePath, string password, IterationSetup iterationSetup);

        /// <summary>
        /// Get user credentials from migration.json if exists
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <param name="password">The password</param>
        /// <returns>The deserialized collection that contains hashed password and salt</returns>
        IList<MigrationPasswordCredentials> ReadMigrationJsonFromFile(string filePath, string password);

        /// <summary>
        /// Gets or sets the <see cref="ICdp4JsonSerializer"/>
        /// </summary>
        ICdp4JsonSerializer JsonSerializer { get; set; }

        /// <summary>
        /// Stores the referenced (by hash) file file binary contained in the archive.
        /// </summary>
        /// <param name="filePath">
        /// The file path of the zip archive being processed.
        /// </param>
        /// <param name="password">
        /// The archive password.
        /// </param>
        /// <param name="fileHash">
        /// The file hash of the file binary that will be stored on disk.
        /// </param>
        void StoreFileBinary(string filePath, string password, string fileHash);
    }
}
