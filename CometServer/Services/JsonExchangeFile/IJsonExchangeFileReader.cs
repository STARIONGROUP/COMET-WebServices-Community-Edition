// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IJsonExchangeFileReader.cs" company="Starion Group S.A.">
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
    using System;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using System.Collections.ObjectModel;

    /// <summary>
    /// The <see cref="IJsonExchangeFileReader"/> interface that defines methods to read data from
    /// an E-TM-10-25 Annex C3 archive
    /// </summary>
    public interface IJsonExchangeFileReader
    {
        /// <summary>
        /// Reads the <see cref="CDP4Common.DTO.Thing"/>s contained by the <see cref="SiteDirectory"/>
        /// </summary>
        /// <param name="version">
        /// The <see cref="Version"/> of the COMET master madel
        /// </param>
        /// <param name="filePath">
        /// The path of the zip archive to read from
        /// </param>
        /// <param name="password">
        /// The zip archive password
        /// </param>
        /// <returns>
        /// The <see cref="CDP4Common.DTO.Thing"/>s contained by the <see cref="SiteDirectory"/>
        /// </returns>
        ReadOnlyCollection<Thing> ReadSiteDirectoryFromfile(Version version, string filePath, string password);

        /// <summary>
        /// Reads the <see cref="CDP4Common.DTO.Thing"/>s contained by the <see cref="EngineeringModel"/>
        /// referenced by the provided <see cref="EngineeringModelSetup"/>
        /// </summary>
        /// <param name="version">
        /// The <see cref="Version"/> of the COMET master madel
        /// </param>
        /// <param name="filePath">
        /// The path of the zip archive to read from
        /// </param>
        /// <param name="password">
        /// The zip archive password
        /// </param>
        /// <param name="engineeringModelSetup">
        /// The <see cref="EngineeringModelSetup"/> that holds a reference to the <see cref="EngineeringModel"/> from which
        /// the <see cref="CDP4Common.DTO.Thing"/>s are to be read
        /// </param>
        /// <returns>
        /// The <see cref="CDP4Common.DTO.Thing"/>s contained by the <see cref="EngineeringModel"/>
        /// </returns>
        ReadOnlyCollection<Thing> ReadEngineeringModelFromfile(Version version, string filePath, string password, EngineeringModelSetup engineeringModelSetup);

        /// <summary>
        ///  Reads the <see cref="CDP4Common.DTO.Thing"/>s contained by the <see cref="Iteration"/> referenced by the
        /// <see cref="EngineeringModelSetup"/> and contained <see cref="IterationSetup"/>
        /// </summary>
        /// <param name="version">
        /// The <see cref="Version"/> of the COMET master madel
        /// </param>
        /// <param name="filePath">
        /// The path of the zip archive to read from
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="engineeringModelSetup">
        /// The <see cref="EngineeringModelSetup"/> that contains the <see cref="IterationSetup"/> that holds a reference to the
        /// <see cref="Iteration"/> from which the contained data needs to be read
        ///  </param>
        /// <param name="iterationSetup">
        /// The <see cref="IterationSetup"/> that holds a reference to the <see cref="Iteration"/> from which the contained data is to be read
        /// </param>
        /// <returns>
        /// The <see cref="CDP4Common.DTO.Thing"/>s contained by the <see cref="Iteration"/>
        /// </returns>
        ReadOnlyCollection<Thing> ReadModelIterationFromFile(Version version, string filePath, string password, EngineeringModelSetup engineeringModelSetup, IterationSetup iterationSetup);

        /// <summary>
        /// Get user credentials from migration.json if exists
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <param name="password">The password</param>
        /// <returns>The deserialized collection that contains hashed password and salt</returns>
        ReadOnlyCollection<MigrationPasswordCredentials> ReadMigrationJsonFromFile(string filePath, string password);

        /// <summary>
        /// Reads and Stores the referenced (by hash) file-binary contained in the archive.
        /// </summary>
        /// <param name="filePath">
        /// The file path of the zip archive being processed.
        /// </param>
        /// <param name="password">
        /// The archive password.
        /// </param>
        /// <param name="engineeringModelSetup">
        /// The <see cref="EngineeringModelSetup"/> that holds a reference to the <see cref="EngineeringModel"/> from which
        /// the files are to be read and stored
        /// </param>
        /// <param name="fileHash">
        /// The file hash of the file binary that will be stored on disk.
        /// </param>
        void ReadAndStoreFileBinary(string filePath, string password, EngineeringModelSetup engineeringModelSetup, string fileHash);
    }
}
