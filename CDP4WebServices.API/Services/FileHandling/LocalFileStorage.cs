// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalFileStorage.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski
//
//    This file is part of COMET Web Services Community Edition. 
//    The COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
//
// Based on source http://bytefish.de/blog/file_upload_nancy/
//
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.FileHandling
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    using CDP4WebServices.API.Configuration;

    using Nancy;

    using NLog;

    /// <summary>
    /// The local storage handler.
    /// </summary>
    public class LocalFileStorage : ILocalFileStorage
    {
        /// <summary>
        /// The root path provider.
        /// </summary>
        private readonly IRootPathProvider rootPathProvider;

        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileStorage"/> class.
        /// </summary>
        /// <param name="rootPathProvider">
        /// The (injected) root path provider.
        /// </param>
        public LocalFileStorage(IRootPathProvider rootPathProvider)
        {
            this.rootPathProvider = rootPathProvider;
        }

        /// <summary>
        /// Save the binary file stream to disk.
        /// </summary>
        /// <param name="stream">
        /// The binary file stream.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<string> StreamFileToDisk(Stream stream)
        {
            var targetFile = Path.Combine(this.GetDirectory(), Guid.NewGuid().ToString());

            var sw = Stopwatch.StartNew();
            
            Logger.Debug("Start writing file: {0}", targetFile);
            
            using (var destinationStream = File.Create(targetFile))
            {
                await stream.CopyToAsync(destinationStream);
            }

            Logger.Debug("Finished writing file {0} in {1} [ms]", targetFile, sw.ElapsedMilliseconds);

            return targetFile;
        }

        /// <summary>
        /// The remove file from disk.
        /// </summary>
        /// <param name="targetFile">
        /// The target file.
        /// </param>
        public void RemoveFileFromDisk(string targetFile)
        {
            File.Delete(targetFile);
        }

        /// <summary>
        /// The get directory.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetDirectory()
        {
            var uploadDirectory = Path.Combine(this.rootPathProvider.GetRootPath(), AppConfig.Current.Midtier.UploadDirectory);

            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            return uploadDirectory;
        }
    }
}
