// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeFileProcessor.cs" company="RHEA System S.A.">
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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using CDP4Common.CommonData;

    using CDP4JsonSerializer;

    using CDP4Orm.Dao;

    using Ionic.Zip;

    using Newtonsoft.Json.Linq;

    using NLog;

    using EngineeringModelSetup = CDP4Common.DTO.EngineeringModelSetup;
    using IterationSetup = CDP4Common.DTO.IterationSetup;
    using ModelReferenceDataLibrary = CDP4Common.DTO.ModelReferenceDataLibrary;
    using SiteReferenceDataLibrary = CDP4Common.DTO.SiteReferenceDataLibrary;
    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// The exchange file processor.
    /// </summary>
    public class ExchangeFileProcessor : IExchangeFileProcessor
    {
        /// <summary>
        /// The exchange file name format.
        /// </summary>
        private const string ExchangeFileNameFormat = "{0}.json";

        /// <summary>
        /// The NLog logger
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the request utils.
        /// </summary>
        public IRequestUtils RequestUtils { get; set; }

        /// <summary>
        /// Gets or sets the file n binary service.
        /// </summary>
        public IFileBinaryService FileBinaryService { get; set; }

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
        public IEnumerable<Thing> ReadSiteDirectoryFromfile(string filePath, string password)
        {
            var memoryStream = this.ReadFileToMemory(filePath);
            return this.ReadSiteDirectoryDataFromStream(memoryStream, password);
        }

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
        public IEnumerable<Thing> ReadEngineeringModelFromfile(
            string filePath,
            string password,
            EngineeringModelSetup engineeringModelSetup)
        {
            var memoryStream = this.ReadFileToMemory(filePath);
            return this.ReadEngineeringModelDataFromStream(memoryStream, password, engineeringModelSetup);
        }

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
        public IEnumerable<Thing> ReadModelIterationFromFile(
            string filePath,
            string password,
            IterationSetup iterationSetup)
        {
            var memoryStream = this.ReadFileToMemory(filePath);
            return this.ReadIterationModelDataFromStream(memoryStream, password, iterationSetup);
        }

        /// <summary>
        /// Gets or sets the <see cref="ICdp4JsonSerializer"/>
        /// </summary>
        /// <remarks>
        /// Injected
        /// </remarks>
        public ICdp4JsonSerializer JsonSerializer { get; set; }

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
        public void StoreFileBinary(string filePath, string password, string fileHash)
        {
            var memoryStream = this.ReadFileToMemory(filePath);
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
        private MemoryStream ReadFileToMemory(string filePath)
        {
            var memoryStream = new MemoryStream();

            using (Stream input = File.OpenRead(filePath))
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
        private IEnumerable<Thing> ReadSiteDirectoryDataFromStream(MemoryStream stream, string password)
        {
            try
            {
                // read file, SiteDirectory first.
                using (var zip = ZipFile.Read(stream))
                {
                    // read site directory info from file
                    var siteDirectoryZipEntry =
                        zip.Entries.SingleOrDefault(x => x.FileName.EndsWith("SiteDirectory.json"));

                    var returnedSiteDirectory = this.ReadInfoFromArchiveEntry(siteDirectoryZipEntry, password);
                    Logger.Info("{0} Site Directory item(s) encountered", returnedSiteDirectory.Count);

                    var returned = new List<Thing>(returnedSiteDirectory);
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
                        var modelRdlItems = this.ReadInfoFromArchiveEntry(modelRdlZipEntry, password);

                        Logger.Info("{0} Model Reference Data Library item(s) encountered", returnedSiteDirectory.Count);
                        returned.AddRange(modelRdlItems);

                        // load the reference data libraries as per the containment chain
                        var requiredRdl = modelRdlDto.RequiredRdl;

                        while (requiredRdl != null)
                        {
                            Logger.Info("Required Reference Data Library encountered: {0}", requiredRdl);

                            var siteRdlDto =
                                (SiteReferenceDataLibrary)
                                returnedSiteDirectory.Single(
                                    x => x.ClassKind == ClassKind.SiteReferenceDataLibrary && x.Iid == requiredRdl);

                            var siteRdlFilePath = string.Format(ExchangeFileNameFormat, siteRdlDto.Iid);

                            if (!processedRdls.Contains(siteRdlFilePath))
                            {
                                var siteRdlZipEntry =
                                    zip.Entries.SingleOrDefault(x => x.FileName.EndsWith(siteRdlFilePath));

                                var siteRdlItems = this.ReadInfoFromArchiveEntry(siteRdlZipEntry, password);

                                Logger.Info("{0} Site Reference Data Library item(s) encountered", siteRdlItems.Count);
                                returned.AddRange(siteRdlItems);

                                // register this processedRdl
                                processedRdls.Add(siteRdlFilePath);
                            }

                            // set the requiredRdl for the next iteration
                            requiredRdl = siteRdlDto.RequiredRdl;
                        }
                    }

                    Logger.Info("{0} Site Directory items encountered", returned.Count);
                    return returned;
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}: {1}", "Failed to load file. Error", ex.Message);
                Logger.Error(msg);

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
        private IEnumerable<Thing> ReadEngineeringModelDataFromStream(
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
                    var engineeringModelItems = this.ReadInfoFromArchiveEntry(engineeringModelZipEntry, password);

                    Logger.Info("{0} Engineering Model item(s) encountered", engineeringModelItems.Count);
                    return engineeringModelItems;
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}: {1}", "Failed to load file. Error", ex.Message);
                Logger.Error(msg);

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
        private IEnumerable<Thing> ReadIterationModelDataFromStream(
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
                    var iterationItems = this.ReadInfoFromArchiveEntry(iterationZipEntry, password);

                    Logger.Info("{0} Iteration item(s) encountered", iterationItems.Count);
                    return iterationItems;
                }
            }
            catch (Exception ex)
            {
                var msg = string.Format("{0}: {1}", "Failed to load file. Error", ex.Message);
                Logger.Error(msg);

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
                    Logger.Info("Store file binary with hash {0}", hash);
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
        private List<Thing> ReadInfoFromArchiveEntry(ZipEntry zipEntry, string archivePassword)
        {
            var watch = Stopwatch.StartNew();

            // the extracted stream is closed thus needs to be reinitialized from the buffer of the old one
            IEnumerable<Thing> returned;

            using (var stream = this.ReadStreamFromArchive(zipEntry, archivePassword))
            {
                this.JsonSerializer.Initialize(this.RequestUtils.MetaInfoProvider, this.RequestUtils.GetRequestDataModelVersion);
                returned = this.JsonSerializer.Deserialize(stream);
            }

            watch.Stop();
            Logger.Info("JSON Deserializer completed in {0} ", watch.Elapsed);
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
                throw new ArgumentNullException("zipEntry", "Supplied archive entry is invalid");
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
                var msg = string.Format("{0}: {1}", "Failed to open file. Error", ex.Message);
                Logger.Error(msg);

                throw new FileLoadException(msg);
            }

            watch.Stop();
            Logger.Info("JSONFile GET completed in {0} ", watch.Elapsed);

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
            var memoryStream = this.ReadFileToMemory(filePath);
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
                Logger.Error($"Failed to load file. Error: {ex.Message}");
            }

            return credentials;
        }

        /// <summary>
        /// Create credentials list with the data extracted from migration.json
        /// </summary>
        /// <param name="stream">Input stream <see cref="Stream"/></param>
        /// <returns>List of <see cref="MigrationPasswordCredentials"/></returns>
        private static List<MigrationPasswordCredentials> CreateCredentialsList(Stream stream)
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
                    Logger.Error($"Failed to load file. Error: {ex.Message}");
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
