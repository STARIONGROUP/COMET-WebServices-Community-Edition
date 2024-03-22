// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonExchangeFileReader.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
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
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using CDP4JsonSerializer;

    using CDP4Orm.Dao;

    using ICSharpCode.SharpZipLib.Core;
    using ICSharpCode.SharpZipLib.Zip;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The purpose of the <see cref="JsonExchangeFileReader"/> is toread data from
    /// an E-TM-10-25 Annex C3 archive
    /// </summary>
    public class JsonExchangeFileReader : IJsonExchangeFileReader
    {
        /// <summary>
        /// Gets or sets the (injected) <see cref="ILogger"/>
        /// </summary>
        public ILogger<JsonExchangeFileReader> Logger { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IMetaInfoProvider"/>
        /// </summary>
        public IMetaInfoProvider MetaInfoProvider { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IFileBinaryService"/>
        /// </summary>
        public IFileBinaryService FileBinaryService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="ICdp4JsonSerializer"/>
        /// </summary>
        public ICdp4JsonSerializer JsonSerializer { get; set; }

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
        public ReadOnlyCollection<CDP4Common.DTO.Thing> ReadSiteDirectoryFromfile(Version version, string filePath, string password)
        {
            var memoryStream = ReadFileToMemory(filePath);
            return this.ReadSiteDirectoryDataFromStream(version, memoryStream, password);
        }

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
        public ReadOnlyCollection<CDP4Common.DTO.Thing> ReadEngineeringModelFromfile(Version version, string filePath, string password, EngineeringModelSetup engineeringModelSetup)
        {
            var memoryStream = ReadFileToMemory(filePath);
            return this.ReadEngineeringModelDataFromStream(version, memoryStream, password, engineeringModelSetup);
        }

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
        /// The zip archive password
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
        public ReadOnlyCollection<CDP4Common.DTO.Thing> ReadModelIterationFromFile(Version version, string filePath, string password, EngineeringModelSetup engineeringModelSetup, IterationSetup iterationSetup)
        {
            var memoryStream = ReadFileToMemory(filePath);
            return this.ReadIterationModelDataFromStream(version, memoryStream, password, engineeringModelSetup, iterationSetup);
        }

        /// <summary>
        /// Reads and Stores the referenced (by hash) file-binary contained in the archive.
        /// </summary>
        /// <param name="filePath">
        /// The path of the zip archive to read from
        /// </param>
        /// <param name="password">
        /// The zip archive password
        /// </param>
        /// <param name="engineeringModelSetup">
        /// The <see cref="EngineeringModelSetup"/> that holds a reference to the <see cref="EngineeringModel"/> from which
        /// the files are to be read and stored
        /// </param>
        /// <param name="fileHash">
        /// The file hash of the file binary that will be stored on disk.
        /// </param>
        public void ReadAndStoreFileBinary(string filePath, string password, EngineeringModelSetup engineeringModelSetup, string fileHash)
        {
            var memoryStream = ReadFileToMemory(filePath);
            this.ExtractFileBinaryByHash(memoryStream, password, engineeringModelSetup, fileHash);
        }

        /// <summary>
        /// Get the migration.json file from archive.
        /// </summary>
        /// <param name="filePath">
        /// The path of the zip archive to read from
        /// </param>
        /// <param name="password">
        /// The zip archive password
        /// </param>
        /// <returns>
        /// The site directory contained <see cref="CDP4Common.DTO.Thing"/> collection.
        /// </returns>
        public ReadOnlyCollection<MigrationPasswordCredentials> ReadMigrationJsonFromFile(string filePath, string password)
        {
            var memoryStream = ReadFileToMemory(filePath);
            return this.ReadMigrationJsonFromStream(memoryStream, password);
        }

        /// <summary>
        /// Read file to memory stream.
        /// </summary>
        /// <param name="filePath">
        /// The path of the zip archive to read from
        /// </param>
        /// <returns>
        /// The <see cref="MemoryStream"/> that contaisn the data of the file
        /// </returns>
        private static MemoryStream ReadFileToMemory(string filePath)
        {
            var memoryStream = new MemoryStream();

            using (Stream input = System.IO.File.OpenRead(filePath))
            {
                input.CopyTo(memoryStream);
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        /// <summary>
        /// Read site directory data from stream.
        /// </summary>
        /// <param name="version">
        /// The <see cref="Version"/> of the COMET master madel
        /// </param>
        /// <param name="stream">
        /// /// The <see cref="MemoryStream"/> to read the data from
        /// </param>
        /// <param name="password">
        /// The zip archive password
        /// </param>
        /// <returns>
        /// The site directory contained <see cref="CDP4Common.DTO.Thing"/> collection.
        /// </returns>
        /// <exception cref="FileLoadException">
        /// If file was not loaded properly
        /// </exception>
        private ReadOnlyCollection<CDP4Common.DTO.Thing> ReadSiteDirectoryDataFromStream(Version version, MemoryStream stream, string password)
        {
            try
            {
                using var zipFile = new ZipFile(stream);

                zipFile.Password = password;

                this.Logger.LogDebug("extracting Things from: SiteDirectory.json");

                // read site directory info from file
                var siteDirectoryZipEntry = zipFile.GetEntry("SiteDirectory.json");

                var returnedSiteDirectory = this.ReadInfoFromArchiveEntry(version, zipFile, siteDirectoryZipEntry);
                this.Logger.LogInformation("{count} Site Directory item(s) encountered", returnedSiteDirectory.Count);

                var returned = new List<CDP4Common.DTO.Thing>(returnedSiteDirectory);
                var processedRdls = new List<string>();

                foreach (var engineeringModelSetup in
                         returnedSiteDirectory.Where(x => x.ClassKind == ClassKind.EngineeringModelSetup)
                             .Where(i => i.GetType() == typeof(EngineeringModelSetup)).Cast<EngineeringModelSetup>())
                {
                    // based on engineering model setup load rdl chain
                    var modelRdlDto =
                        (ModelReferenceDataLibrary)
                        returnedSiteDirectory.Single(
                            x =>
                                x.ClassKind == ClassKind.ModelReferenceDataLibrary
                                && x.Iid == engineeringModelSetup.RequiredRdl.Single());

                    this.Logger.LogDebug("extracting Things from: ModelReferenceDataLibraries/{modelRdlDto}.json", modelRdlDto.Iid);

                    var modelRdlZipEntry = zipFile.GetEntry($"ModelReferenceDataLibraries/{modelRdlDto.Iid}.json");

                    var modelRdlItems = this.ReadInfoFromArchiveEntry(version, zipFile, modelRdlZipEntry);

                    this.Logger.LogInformation("{Count} Model Reference Data Library item(s) encountered", returnedSiteDirectory.Count);
                    returned.AddRange(modelRdlItems);

                    // load the reference data libraries as per the containment chain
                    var requiredRdl = modelRdlDto.RequiredRdl;

                    while (requiredRdl != null)
                    {
                        this.Logger.LogInformation("Required Reference Data Library encountered: {requiredRdl}", requiredRdl);

                        var siteRdlDto =
                            (SiteReferenceDataLibrary)
                            returnedSiteDirectory.Single(
                                x => x.ClassKind == ClassKind.SiteReferenceDataLibrary && x.Iid == requiredRdl);

                        this.Logger.LogDebug("extracting Things from: SiteReferenceDataLibraries/{siteRdlDto}.json", siteRdlDto.Iid);

                        var siteRdlFilePath = $"SiteReferenceDataLibraries/{siteRdlDto.Iid}.json";

                        if (!processedRdls.Contains(siteRdlFilePath))
                        {
                            var siteRdlZipEntry = zipFile.GetEntry(siteRdlFilePath);

                            var siteRdlItems = this.ReadInfoFromArchiveEntry(version, zipFile, siteRdlZipEntry);

                            this.Logger.LogInformation("{Count} Site Reference Data Library item(s) encountered", siteRdlItems.Count);
                            returned.AddRange(siteRdlItems);

                            // register this processedRdl
                            processedRdls.Add(siteRdlFilePath);
                        }

                        // set the requiredRdl for the next iteration
                        requiredRdl = siteRdlDto.RequiredRdl;
                    }
                }

                this.Logger.LogInformation("{Count} Site Directory items encountered", returned.Count);

                return returned.AsReadOnly();
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Failed to load file");

                throw new FileLoadException($"Failed to load file: {ex.Message}");
            }
        }

        /// <summary>
        /// Read engineering model data from stream.
        /// </summary>
        /// <param name="version">
        /// The <see cref="Version"/> of the COMET master madel
        /// </param>
        /// <param name="stream">
        /// The <see cref="MemoryStream"/> to read the data from
        /// </param>
        /// <param name="password">
        /// The zip archive password
        /// </param>
        /// <param name="engineeringModelSetup">
        /// The engineering model setup.
        /// </param>
        /// <returns>
        /// The engineering model contained <see cref="CDP4Common.DTO.Thing"/> collection.
        /// </returns>
        /// <exception cref="FileLoadException">
        /// If file was not loaded properly
        /// </exception>
        private ReadOnlyCollection<CDP4Common.DTO.Thing> ReadEngineeringModelDataFromStream(Version version, MemoryStream stream, string password, EngineeringModelSetup engineeringModelSetup)
        {
            try
            {
                using var zipFile = new ZipFile(stream);

                zipFile.Password = password;

                this.Logger.LogDebug("extracting Things from: EngineeringModels/{EngineeringModelIid}/{EngineeringModelIid}.json", engineeringModelSetup.EngineeringModelIid, engineeringModelSetup.EngineeringModelIid);

                var engineeringModelZipEntry = zipFile.GetEntry($"EngineeringModels/{engineeringModelSetup.EngineeringModelIid}/{engineeringModelSetup.EngineeringModelIid}.json");
                var engineeringModelItems = this.ReadInfoFromArchiveEntry(version, zipFile,engineeringModelZipEntry);

                this.Logger.LogInformation("{Count} Engineering Model item(s) encountered", engineeringModelItems.Count);
                return engineeringModelItems.AsReadOnly();
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Failed to load file");

                throw new FileLoadException($"Failed to load file. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// The read iteration model data from stream.
        /// </summary>
        /// <param name="version">
        /// The <see cref="Version"/> of the COMET master madel
        /// </param>
        /// <param name="stream">
        /// The <see cref="MemoryStream"/> to read the data from
        /// </param>
        /// <param name="password">
        /// The zip archive password
        /// </param>
        /// <param name="engineeringModelSetup">
        /// The <see cref="EngineeringModelSetup"/> that contains the <see cref="IterationSetup"/> that holds a reference to the
        /// <see cref="Iteration"/> from which the contained data needs to be read
        /// </param>
        /// <param name="iterationSetup">
        /// The <see cref="IterationSetup"/> that holds a reference to the <see cref="Iteration"/> from which the contained data is to be read
        /// </param>
        /// <returns>
        /// The model iteration contained <see cref="CDP4Common.DTO.Thing"/> collection.
        /// </returns>
        /// <exception cref="FileLoadException">
        /// If file was not loaded properly
        /// </exception>
        private ReadOnlyCollection<CDP4Common.DTO.Thing> ReadIterationModelDataFromStream(Version version, MemoryStream stream, string password, EngineeringModelSetup engineeringModelSetup, IterationSetup iterationSetup)
        {
            try
            {
                // read file, SiteDirectory first.
                using var zipFile = new ZipFile(stream);

                zipFile.Password = password;

                // read iteration data
                this.Logger.LogDebug("Extractring things from: EngineeringModels/{EngineeringModelIid}/Iterations/{IterationIid}.json", engineeringModelSetup.EngineeringModelIid, iterationSetup.IterationIid);

                var iterationZipEntry = zipFile.GetEntry($"EngineeringModels/{engineeringModelSetup.EngineeringModelIid}/Iterations/{iterationSetup.IterationIid}.json"); 
                var iterationItems = this.ReadInfoFromArchiveEntry(version, zipFile, iterationZipEntry);

                this.Logger.LogInformation("{Count} Iteration item(s) encountered", iterationItems.Count);
                return iterationItems;
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Failed to load file");
                
                throw new FileLoadException($"Failed to load file. Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Extract and persist the file binary (by hash).
        /// </summary>
        /// <param name="archiveStream">
        /// The <see cref="MemoryStream"/> to read the data from
        /// </param>
        /// <param name="password">
        /// The zip archive password
        /// </param>
        /// <param name="engineeringModelSetup">
        /// The <see cref="EngineeringModelSetup"/> that holds a reference to the <see cref="EngineeringModel"/> from which
        /// the files are to be read and stored
        /// </param>
        /// <param name="hash">
        /// The file hash to extract.
        /// </param>
        private void ExtractFileBinaryByHash(MemoryStream archiveStream, string password, EngineeringModelSetup engineeringModelSetup, string hash)
        {
            if (this.FileBinaryService.IsFilePersisted(hash))
            {
                // file already present in store
                return;
            }

            using var zipFile = new ZipFile(archiveStream);

            zipFile.Password = password;

            // select file binary from the archive archive
            this.Logger.LogDebug("Extracting data from EngineeringModels/{EngineeringModelIid}/FileRevisions/{hash}", engineeringModelSetup.EngineeringModelIid, hash);

            var fileZipEntry = zipFile.GetEntry($"EngineeringModels/{engineeringModelSetup.EngineeringModelIid}/FileRevisions/{hash}");

            using var stream = ReadStreamFromArchive(zipFile, fileZipEntry);

            this.Logger.LogInformation("Store file binary with hash {hash}", hash);

            this.FileBinaryService.StoreBinaryData(hash, stream);
        }

        /// <summary>
        /// Read info from a specified archive entry.
        /// </summary>
        /// <param name="version">
        /// The <see cref="Version"/> of the COMET master madel
        /// </param>
        /// <param name="zipFile">
        /// The <see cref="ZipFile"/> from which the data is to be read
        /// </param>
        /// <param name="zipEntry">
        /// The <see cref="ZipEntry"/> from which the data is to be read
        /// </param>
        /// <returns>
        /// The model iteration contained <see cref="CDP4Common.DTO.Thing"/> collection.
        /// </returns>
        private ReadOnlyCollection<CDP4Common.DTO.Thing> ReadInfoFromArchiveEntry(Version version, ZipFile zipFile, ZipEntry zipEntry)
        {
            var sw = Stopwatch.StartNew();

            // the extracted stream is closed thus needs to be reinitialized from the buffer of the old one
            var returned = new List<CDP4Common.DTO.Thing>();

            using (var stream = ReadStreamFromArchive(zipFile, zipEntry))
            {
                stream.Position = 0;

                this.JsonSerializer.Initialize(this.MetaInfoProvider, version);
                returned.AddRange(this.JsonSerializer.Deserialize(stream));
            }

            this.Logger.LogInformation("JSON Deserializer completed in {ElapsedMilliseconds} [ms]", sw.ElapsedMilliseconds);

            return returned.AsReadOnly();
        }

        /// <summary>
        /// Reads a stream of uncompressed data from the provided <see cref="ZipFile"/>
        /// </summary>
        /// <param name="zipFile">
        /// The <see cref="ZipFile"/> that contains the subject <see cref="ZipEntry"/>
        /// </param>
        /// <param name="zipEntry">
        /// The <see cref="ZipEntry"/> that provides access to the <see cref="Stream"/>
        /// </param>
        /// <returns>
        /// a <see cref="MemoryStream"/> that contains the uncompressed data from the <see cref="ZipEntry"/>
        /// </returns>
        /// <exception cref="Exception">
        /// throws exception if the file failed to open
        /// </exception>
        private static MemoryStream ReadStreamFromArchive(ZipFile zipFile, ZipEntry zipEntry)
        {
            if (zipEntry == null)
            {
                throw new ArgumentNullException(nameof(zipEntry), "Supplied archive entry is invalid");
            }

            var buffer = new byte[4096];
            var targetStream = new MemoryStream();

            using var zipStream = zipFile.GetInputStream(zipEntry);

            StreamUtils.Copy(zipStream, targetStream, buffer);

            return targetStream;
        }

        /// <summary>
        /// Reads the migration.json file from the <paramref name="memoryStream"/>
        /// </summary>
        /// <param name="memoryStream">
        /// The zip archive stream <see cref="MemoryStream" />
        /// </param>
        /// <param name="password">
        /// The zip archive password
        /// </param>
        /// <returns></returns>
        private ReadOnlyCollection<MigrationPasswordCredentials> ReadMigrationJsonFromStream(MemoryStream memoryStream, string password)
        {
            try
            {
                using var zipFile = new ZipFile(memoryStream);

                zipFile.Password = password;

                var migrationJsonZipEntry = zipFile.GetEntry("migration.json");

                if (migrationJsonZipEntry != null)
                {
                    using var stream = ReadStreamFromArchive(zipFile, migrationJsonZipEntry);

                    return this.CreateCredentialsList(stream);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Failed to load file");
            }

            return new List<MigrationPasswordCredentials>().AsReadOnly();
        }

        /// <summary>
        /// Create credentials list with the data extracted from migration.json
        /// </summary>
        /// <param name="stream">
        /// Input stream <see cref="Stream"/>
        /// </param>
        /// <returns>
        /// List of <see cref="MigrationPasswordCredentials"/>
        /// </returns>
        private ReadOnlyCollection<MigrationPasswordCredentials> CreateCredentialsList(Stream stream)
        {
            var credentialsList = new List<MigrationPasswordCredentials>();

            using var reader = new StreamReader(stream);

            try
            {
                var content = reader.ReadToEnd();
                var jsonElement = this.JsonSerializer.Deserialize<JsonElement>(content);

                if (jsonElement.TryGetProperty("credentials"u8, out var credentialValue))
                {
                    foreach (var children in credentialValue.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.Object))
                    {
                        var password = children.GetProperty("password"u8).GetString();
                        var salt = children.GetProperty("salt"u8).GetString();
                        credentialsList.Add(new MigrationPasswordCredentials(new Guid(children.GetString()!), password, salt));
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Failed to load file");
            }
            finally
            {
                reader.Close();
            }

            return credentialsList.AsReadOnly();
        }
    }
}
