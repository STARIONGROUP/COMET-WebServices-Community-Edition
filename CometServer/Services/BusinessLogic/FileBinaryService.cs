// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileBinaryService.cs" company="RHEA System S.A.">
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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using CometServer.Configuration;

    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The purpose of the <see cref="FileBinaryService"/> is to provide file functions for storage to- and retrieval from the 
    /// the file system.
    /// </summary>
    public class FileBinaryService : IFileBinaryService
    {
        /// <summary>
        /// The number of distribution levels to each stored file, alleviating the storage system from handling excessive amount of file-entries per folder.
        /// </summary>
        private const int FileStorageDistributionLevels = 6;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBinaryService"/> class.
        /// </summary>
        public FileBinaryService()
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="IAppConfigService"/>
        /// </summary>
        public IAppConfigService AppConfigService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="ILogger"/>
        /// </summary>
        public ILogger<FileBinaryService> Logger { get; set; }

        /// <summary>
        /// Check whether the file is already persisted.
        /// </summary>
        /// <param name="hash">
        /// The hash of the file content.
        /// </param>
        /// <returns>
        /// True if already persisted on disk.
        /// </returns>
        public bool IsFilePersisted(string hash)
        {
            string filePath;
            return this.TryGetFileStoragePath(hash, out filePath);
        }

        /// <summary>
        /// Retrieve the binary data from disk.
        /// </summary>
        /// <param name="hash">
        /// The SHA1 hash of the the file.
        /// </param>
        /// <returns>
        /// The binary file stream handle of the file.
        /// </returns>
        /// <remarks>
        /// Binary files are stored within the system with the hash value as name
        /// </remarks>
        public Stream RetrieveBinaryData(string hash)
        {
            if (!this.TryGetFileStoragePath(hash, out var filePath))
            {
                throw new FileNotFoundException("The requested file does not exists");
            }

            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// Store the binary data stream on the file system.
        /// </summary>
        /// <param name="hash">
        /// The SHA1 hash of the data stream that will be stored.
        /// </param>
        /// <param name="data">
        /// The binary stream of the data that will be stored.
        /// </param>
        public void StoreBinaryData(string hash, Stream data)
        {
            var sw = new Stopwatch();
            sw.Start();
            
            this.Logger.LogDebug("Store Binary Data with hash: {hash} started", hash);

            if (this.TryGetFileStoragePath(hash, out var filePath))
            {
                this.Logger.LogDebug("The file already exists: {filePath}/{hash}", filePath, hash);

                // return as file already exists
                sw.Stop();
                return;
            }

            // create the path for the file
            var stroragePath = this.GetBinaryStoragePath(hash, true);
            filePath = Path.Combine(stroragePath, hash);
            this.Logger.LogDebug("New File storage path: {filePath}", filePath);

            using (var fileStream = File.Create(filePath))
            {
                data.Seek(0, SeekOrigin.Begin);
                data.CopyTo(fileStream);
            }

            this.Logger.LogDebug("File {filePath} stored in {ElapsedMilliseconds} [ms]", filePath, sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Calculate the SHA1 hash from a stream.
        /// </summary>
        /// <param name="stream">
        /// The stream for which to calculate the hash.
        /// </param>
        /// <returns>
        /// The hexadecimal string representation of a SHA-1 hash code.
        /// </returns>
        public string CalculateHashFromStream(Stream stream)
        {
            using var bufferedStream = new BufferedStream(stream);

            using var sha1 = SHA1.Create();

            var hash = sha1.ComputeHash(bufferedStream);
            var formatted = new StringBuilder(2 * hash.Length);
            hash.ToList().ForEach(b => formatted.AppendFormat("{0:X2}", b));
            return formatted.ToString();
        }

        /// <summary>
        /// The get file storage path.
        /// </summary>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <param name="filePath">
        /// The file Path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public bool TryGetFileStoragePath(string hash, out string filePath)
        {
            var directoryPath = this.GetBinaryStoragePath(hash);
            filePath = Path.Combine(directoryPath, hash);

            return File.Exists(filePath);
        }

        /// <summary>
        /// Get the binary storage path based on the hash.
        /// </summary>
        /// <param name="hash">
        /// The binary SHA1 hash.
        /// </param>
        /// <param name="create">
        /// Flag to also create the folder structure if not present yet (for storage purposes).
        /// </param>
        /// <returns>
        /// The determined storage path from the provided hash.
        /// </returns>
        private string GetBinaryStoragePath(string hash, bool create = false)
        {
            // create a distributed folder structure numberOfFileStorageDistributionLevels levels deep in the application root of this Webserver

            var path = this.AppConfigService.AppConfig.Midtier.FileStorageDirectory;

            foreach (var character in hash.ToLowerInvariant().Substring(0, FileStorageDistributionLevels).Select(x => x.ToString()))
            {
                var currentPath = Path.Combine(path, character);

                if (!Directory.Exists(currentPath) && create)
                {
                    Directory.CreateDirectory(currentPath);
                }

                path = currentPath;
            }

            return path;
        }
    }
}