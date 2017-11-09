// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEngineeringModelZipExportService.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The purpose of the <see cref="IEngineeringModelZipExportService"/> is to provide file functions for exporting an engineering model 
    /// in a zipped archive.
    /// </summary>
    public interface IEngineeringModelZipExportService : IBusinessLogicService
    {
        /// <summary>
        /// Create a zip export file with EngineeringModels from supplied EngineeringModelSetupGuids and paths to files.
        /// </summary>
        /// <param name="engineeringModelSetupGuids">
        /// The provided list of <see cref="Guid"/> of EngineeringModelSetups to write to the zip file
        /// </param>
        /// <param name="files">
        /// The path to the files that need to be uploaded. If <paramref name="files"/> is null, then no files are to be uploaded
        /// </param>
        /// <returns>
        /// The <see cref="string"/> path of the created archive.
        /// </returns>
        string CreateZipExportFile(IEnumerable<Guid> engineeringModelSetupGuids, IEnumerable<string> files);
    }
}
