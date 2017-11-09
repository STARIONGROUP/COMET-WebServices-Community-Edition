// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileBinaryService.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System.IO;

    /// <summary>
    /// The purpose of the <see cref="IFileBinaryService"/> is to provide file functions for storage to- and retrieval from the 
    /// the file system.
    /// </summary>
    public interface IFileBinaryService : IBusinessLogicService
    {
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
        Stream RetrieveBinaryData(string hash);

        /// <summary>
        /// Store the binary data stream on the file system.
        /// </summary>
        /// <param name="hash">
        /// The SHA1 hash of the data stream that will be stored.
        /// </param>
        /// <param name="data">
        /// The binary stream of the data that will be stored.
        /// </param>
        void StoreBinaryData(string hash, Stream data);

        /// <summary>
        /// Utility method to calculate the SHA1 hash from a stream.
        /// </summary>
        /// <param name="stream">
        /// The stream for which to calculate the hash.
        /// </param>
        /// <returns>
        /// The SHA1 hash string.
        /// </returns>
        string CalculateHashFromStream(Stream stream);

        /// <summary>
        /// Check whether the file is already persisted.
        /// </summary>
        /// <param name="hash">
        /// The hash of the file content.
        /// </param>
        /// <returns>
        /// True if already persisted on disk.
        /// </returns>
        bool IsFilePersisted(string hash);

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
        bool TryGetFileStoragePath(string hash, out string filePath);
    }
}
