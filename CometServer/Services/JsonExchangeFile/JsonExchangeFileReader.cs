// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonExchangeFileReader.cs" company="RHEA System S.A.">
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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using CDP4JsonSerializer;

    using CDP4Orm.Dao;
    using Ionic.Zip;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The purpose of the <see cref="JsonExchangeFileReader"/> is toread data from
    /// an E-TM-10-25 Annex C3 archive
    /// </summary>
    public class JsonExchangeFileReader : IJsonExchangeFileReader
    {
        /// <summary>
        /// The exchange file name format.
        /// </summary>
        private const string ExchangeFileNameFormat = "{0}.json";

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
        /// Get the site directory from file.
        /// </summary>
        /// <param name="version">
        /// The <see cref="Version"/> of the COMET master madel
        /// </param>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The site directory contained <see cref="Thing"/> collection.
        /// </returns>
        public IEnumerable<CDP4Common.DTO.Thing> ReadSiteDirectoryFromfile(Version version, string filePath, string password)
        {
            var memoryStream = ReadFileToMemory(filePath);
            return this.ReadSiteDirectoryDataFromStream(version, memoryStream, password);
        }

        /// <summary>
        /// Get the engineering model from file.
        /// </summary>
        /// <param name="version">
        /// The <see cref="Version"/> of the COMET master madel
        /// </param>
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
        public IEnumerable<CDP4Common.DTO.Thing> ReadEngineeringModelFromfile(Version version,  string filePath,  string password, EngineeringModelSetup engineeringModelSetup)
        {
            var memoryStream = ReadFileToMemory(filePath);
            return this.ReadEngineeringModelDataFromStream(version, memoryStream, password, engineeringModelSetup);
        }

        /// <summary>
        /// Get the model iteration from file.
        /// </summary>
        /// <param name="version">
        /// The <see cref="Version"/> of the COMET master madel
        /// </param>
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
        public IEnumerable<CDP4Common.DTO.Thing> ReadModelIterationFromFile(Version version, string filePath, string password, IterationSetup iterationSetup)
        {
            var memoryStream = ReadFileToMemory(filePath);
            return this.ReadIterationModelDataFromStream(version, memoryStream, password, iterationSetup);
        }
        
        /// <summary>
        /// Stores the referenced (by hash) file-binary contained in the archive.
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
        public void StoreFileBinary(string filePath, string password, string fileHash)
        {
            var memoryStream = ReadFileToMemory(filePath);
            this.ExtractFileBinaryByHash(memoryStream, password, fileHash);
        }

        /// <summary>
        /// Read file to memory stream.
        /// </summary>
        /// <param name="filePath">
        /// The file path.
        /// </param>
        /// <returns>
        /// The <see cref="MemoryStream"/>.
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
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The site directory contained <see cref="Thing"/> collection.
        /// </returns>
        /// <exception cref="FileLoadException">
        /// If file was not loaded properly
        /// </exception>
        private IEnumerable<CDP4Common.DTO.Thing> ReadSiteDirectoryDataFromStream(Version version, MemoryStream stream, string password)
        {
            try
            {
                // read file, SiteDirectory first.
                using (var zip = ZipFile.Read(stream))
                {
                    // read site directory info from file
                    var siteDirectoryZipEntry = zip.Entries.SingleOrDefault(x => x.FileName.EndsWith("SiteDirectory.json"));

                    var returnedSiteDirectory = this.ReadInfoFromArchiveEntry(version, siteDirectoryZipEntry, password);
                    this.Logger.LogInformation("{count} Site Directory item(s) encountered", returnedSiteDirectory.Count);

                    var returned = new List<CDP4Common.DTO.Thing>(returnedSiteDirectory);
                    var processedRdls = new List<string>();

                    foreach (
                        var engineeringModelSetup in
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

                        var modelRdlFilePath = string.Format(ExchangeFileNameFormat, modelRdlDto.Iid);
                        var modelRdlZipEntry = zip.Entries.SingleOrDefault(x => x.FileName.EndsWith(modelRdlFilePath));
                        var modelRdlItems = this.ReadInfoFromArchiveEntry(version, modelRdlZipEntry, password);

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

                            var siteRdlFilePath = string.Format(ExchangeFileNameFormat, siteRdlDto.Iid);

                            if (!processedRdls.Contains(siteRdlFilePath))
                            {
                                var siteRdlZipEntry =
                                    zip.Entries.SingleOrDefault(x => x.FileName.EndsWith(siteRdlFilePath));

                                var siteRdlItems = this.ReadInfoFromArchiveEntry(version, siteRdlZipEntry, password);

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
                    return returned;
                }
            }
            catch (Exception ex)
            {
                var msg = $"Failed to load file: {ex.Message}";
                this.Logger.LogError(msg);

                throw new FileLoadException(msg);
            }
        }

        /// <summary>
        /// Read engineering model data from stream.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="engineeringModelSetup">
        /// The engineering model setup.
        /// </param>
        /// <returns>
        /// The engineering model contained <see cref="Thing"/> collection.
        /// </returns>
        /// <exception cref="FileLoadException">
        /// If file was not loaded properly
        /// </exception>
        private IEnumerable<CDP4Common.DTO.Thing> ReadEngineeringModelDataFromStream(Version version,
            MemoryStream stream,
            string password,
            EngineeringModelSetup engineeringModelSetup)
        {
            try
            {
                // read file, SiteDirectory first.
                using (var zip = ZipFile.Read(stream))
                {
                    // read engineeringmodel data
                    var engineeringModelFilePath = string.Format(ExchangeFileNameFormat, engineeringModelSetup.EngineeringModelIid);
                    var engineeringModelZipEntry = zip.Entries.SingleOrDefault(x => x.FileName.EndsWith(engineeringModelFilePath));
                    var engineeringModelItems = this.ReadInfoFromArchiveEntry(version, engineeringModelZipEntry, password);

                    this.Logger.LogInformation("{Count} Engineering Model item(s) encountered", engineeringModelItems.Count);
                    return engineeringModelItems;
                }
            }
            catch (Exception ex)
            {
                var msg = $"Failed to load file. Error: {ex.Message}";
                this.Logger.LogError(msg);

                throw new FileLoadException(msg);
            }
        }

        /// <summary>
        /// The read iteration model data from stream.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="iterationSetup">
        /// The iteration setup.
        /// </param>
        /// <returns>
        /// The model iteration contained <see cref="Thing"/> collection.
        /// </returns>
        /// <exception cref="FileLoadException">
        /// If file was not loaded properly
        /// </exception>
        private IEnumerable<CDP4Common.DTO.Thing> ReadIterationModelDataFromStream(Version version,
            MemoryStream stream,
            string password,
            IterationSetup iterationSetup)
        {
            try
            {
                // read file, SiteDirectory first.
                using (var zip = ZipFile.Read(stream))
                {
                    // read iteration data
                    var iterationFilePath = string.Format(ExchangeFileNameFormat, iterationSetup.IterationIid);
                    var iterationZipEntry = zip.Entries.SingleOrDefault(x => x.FileName.EndsWith(iterationFilePath));
                    var iterationItems = this.ReadInfoFromArchiveEntry(version, iterationZipEntry, password);

                    this.Logger.LogInformation("{Count} Iteration item(s) encountered", iterationItems.Count);
                    return iterationItems;
                }
            }
            catch (Exception ex)
            {
                var msg = $"Failed to load file. Error: {ex.Message}";
                this.Logger.LogError(msg);

                throw new FileLoadException(msg);
            }
        }

        /// <summary>
        /// Extract and persist the file binary (by hash).
        /// </summary>
        /// <param name="archiveStream">
        /// The archive Stream.
        /// </param>
        /// <param name="archivePassword">
        /// The archive Password.
        /// </param>
        /// <param name="hash">
        /// The file hash to extract.
        /// </param>
        private void ExtractFileBinaryByHash(MemoryStream archiveStream, string archivePassword, string hash)
        {
            if (this.FileBinaryService.IsFilePersisted(hash))
            {
                // file already present in store
                return;
            }

            using (var zip = ZipFile.Read(archiveStream))
            {
                // select file binary from the archive archive
                var fileZipEntry = zip.Entries.SingleOrDefault(x => x.FileName.EndsWith(hash));

                using (var stream = this.ReadStreamFromArchive(fileZipEntry, archivePassword))
                {
                    this.Logger.LogInformation("Store file binary with hash {hash}", hash);
                    this.FileBinaryService.StoreBinaryData(hash, stream);
                }
            }
        }

        /// <summary>
        /// Read info from a specified archive entry.
        /// </summary>
        /// <param name="zipEntry">
        /// The zip entry.
        /// </param>
        /// <param name="archivePassword">
        /// The password of the archive.
        /// </param>
        /// <returns>
        /// The model iteration contained <see cref="Thing"/> collection.
        /// </returns>
        /// <exception cref="Exception">
        /// throws exception if the file failed to open
        /// </exception>
        private List<CDP4Common.DTO.Thing> ReadInfoFromArchiveEntry(Version version, ZipEntry zipEntry, string archivePassword)
        {
            var watch = Stopwatch.StartNew();

            // the extracted stream is closed thus needs to be reinitialized from the buffer of the old one
            IEnumerable<CDP4Common.DTO.Thing> returned;

            using (var stream = this.ReadStreamFromArchive(zipEntry, archivePassword))
            {
                this.JsonSerializer.Initialize(this.MetaInfoProvider, version);
                returned = this.JsonSerializer.Deserialize(stream);
            }

            watch.Stop();
            this.Logger.LogInformation("JSON Deserializer completed in {ElapsedMilliseconds} [ms]", watch.ElapsedMilliseconds);
            return returned.ToList();
        }

        /// <summary>
        /// Read info from a specified archive entry.
        /// </summary>
        /// <param name="zipEntry">
        /// The zip entry.
        /// </param>
        /// <param name="archivePassword">
        /// The password of the archive.
        /// </param>
        /// <returns>
        /// The zip entry stream.
        /// </returns>
        /// <exception cref="Exception">
        /// throws exception if the file failed to open
        /// </exception>
        private Stream ReadStreamFromArchive(ZipEntry zipEntry, string archivePassword)
        {
            if (zipEntry == null)
            {
                throw new ArgumentNullException(nameof(zipEntry), "Supplied archive entry is invalid");
            }

            var watch = Stopwatch.StartNew();

            var extractStream = new MemoryStream();

            try
            {
                zipEntry.Password = archivePassword;
                zipEntry.Extract(extractStream);
            }
            catch (Exception ex)
            {
                var msg = $"Failed to open file. Error: {ex.Message}";
                this.Logger.LogError(msg);

                throw new FileLoadException(msg);
            }

            watch.Stop();
            this.Logger.LogInformation("JSONFile GET completed in {ElapsedMilliseconds} [ms]", watch.ElapsedMilliseconds);

            return new MemoryStream(extractStream.ToArray());
        }

        /// <summary>
        /// Get the migration.json file from archive.
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
        public IList<MigrationPasswordCredentials> ReadMigrationJsonFromFile(string filePath, string password)
        {
            var memoryStream = ReadFileToMemory(filePath);
            return this.ReadMigrationJsonFromStream(memoryStream, password);
        }

        /// <summary>
        /// Read migration.json file from stream
        /// </summary>
        /// <param name="memoryStream">The zip archive stream <see cref="MemoryStream" /></param>
        /// <param name="password">The password</param>
        /// <returns></returns>
        private IList<MigrationPasswordCredentials> ReadMigrationJsonFromStream(MemoryStream memoryStream, string password)
        {
            var credentials = new List<MigrationPasswordCredentials>();

            try
            {
                using (var zip = ZipFile.Read(memoryStream))
                {
                    var migrationJsonZipEntry =
                        zip.Entries.SingleOrDefault(x => x.FileName.EndsWith("migration.json"));

                    if (migrationJsonZipEntry != null)
                    {
                        using (var stream = this.ReadStreamFromArchive(migrationJsonZipEntry, password))
                        {
                            credentials = CreateCredentialsList(stream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError($"Failed to load file. Error: {ex.Message}");
            }

            return credentials;
        }

        /// <summary>
        /// Create credentials list with the data extracted from migration.json
        /// </summary>
        /// <param name="stream">Input stream <see cref="Stream"/></param>
        /// <returns>List of <see cref="MigrationPasswordCredentials"/></returns>
        private List<MigrationPasswordCredentials> CreateCredentialsList(Stream stream)
        {
            var credentialsList = new List<MigrationPasswordCredentials>();

            using (var reader = new StreamReader(stream))
            {
                try
                {
                    var content = reader.ReadToEnd();
                    var parsedContent = JObject.Parse(content);

                    if (parsedContent?["credentials"] != null)
                    {
                        foreach (var children in parsedContent["credentials"].Children())
                        {
                            if (!(children is JProperty property))
                            {
                                continue;
                            }

                            var password = (property.First["password"] as JValue)?.Value.ToString();
                            var salt = (property.First["salt"] as JValue)?.Value.ToString();
                            credentialsList.Add(new MigrationPasswordCredentials(new Guid(property.Name), password, salt));
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.LogError($"Failed to load file. Error: {ex.Message}");
                }
                finally
                {
                    reader.Close();
                }
            }

            return credentialsList;
        }
    }
}
