// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalFileStorage.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// <remarks>
// Based on source http://bytefish.de/blog/file_upload_nancy/
// </remarks>
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
