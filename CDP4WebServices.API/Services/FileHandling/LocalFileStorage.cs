// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalFileStorage.cs" company="RHEA System S.A.">
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

namespace CometServer.Services.FileHandling
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    using CometServer.Configuration;

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
