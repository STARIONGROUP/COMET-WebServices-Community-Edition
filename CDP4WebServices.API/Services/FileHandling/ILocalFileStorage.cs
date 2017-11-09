// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILocalFileStorage.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// <remarks>
// Based on source http://bytefish.de/blog/file_upload_nancy/
// </remarks>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.FileHandling
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// The FileUploadHandler interface.
    /// </summary>
    public interface ILocalFileStorage
    {
        /// <summary>
        /// Save the binary file stream to disk.
        /// </summary>
        /// <param name="stream">
        /// The binary file stream.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task<string> StreamFileToDisk(Stream stream);

        /// <summary>
        /// The remove file from disk.
        /// </summary>
        /// <param name="targetFile">
        /// The target file.
        /// </param>
        void RemoveFileFromDisk(string targetFile);
    }
}
