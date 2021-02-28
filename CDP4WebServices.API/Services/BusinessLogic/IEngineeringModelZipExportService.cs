// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEngineeringModelZipExportService.cs" company="RHEA System S.A.">
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
