// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IZipArchiveWriter.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
// 
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski,
//                 Ahmed Abulwafa Ahmed
// 
//    This file is part of CDP4 Web Services Community Edition.
//    The CDP4 Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
// 
//    The CDP4 Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4 Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Services
{
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using ICSharpCode.SharpZipLib.Zip;

    public interface IZipArchiveWriter
    {
        /// <summary>
        /// Write the header file to the zip export archive.
        /// </summary>
        /// <param name="echExchangeFileHeader">
        /// The <see cref="ExchangeFileHeader"/> that is to be written to the <see cref="ZipOutputStream"/>
        /// </param>
        /// <param name="zipOutputStream">
        /// The zip archive instance to add the information to.
        /// </param>
        void WriteHeaderToZipFile(ExchangeFileHeader echExchangeFileHeader, ZipOutputStream zipOutputStream);

        /// <summary>
        /// Writes <see cref="SiteDirectory"/> contents to the <see cref="ZipFile"/>
        /// </summary>
        /// <param name="siteDirectoryContents">
        /// The <see cref="SiteDirectory"/> data that has been pruned of all unnecessary data and that is to be written to the <see cref="ZipOutputStream"/>
        /// </param>
        /// <param name="zipOutputStream">
        /// The target <see cref="ZipOutputStream"/>
        /// </param>
        void WriteSiteDirectoryToZipFile(IEnumerable<Thing> siteDirectoryContents, ZipOutputStream zipOutputStream);

        /// <summary>
        /// Writes <see cref="EngineeringModel"/> contents to the <see cref="ZipFile"/>
        /// </summary>
        /// <param name="engineeringModelSetup">
        /// The <see cref="EngineeringModelSetup"/> that corresponds to the <see cref="EngineeringModel"/> data
        /// </param>
        /// <param name="engineeringModelContents">
        /// The <see cref="EngineeringModel"/> data that is to be written to the <see cref="ZipOutputStream"/>
        /// </param>
        /// <param name="zipOutputStream">
        /// The target <see cref="ZipOutputStream"/>
        /// </param>
        void WriteEngineeringModelToZipFile(EngineeringModelSetup engineeringModelSetup, IEnumerable<Thing> engineeringModelContents, ZipOutputStream zipOutputStream);

        /// <summary>
        /// Writes <see cref="EngineeringModel"/> contents to the <see cref="ZipFile"/>
        /// </summary>
        /// <param name="engineeringModelSetup">
        /// The <see cref="EngineeringModelSetup"/> that corresponds to the <see cref="Iteration"/> data
        /// </param>
        /// <param name="iterationSetup">
        /// The <see cref="IterationSetup"/> that corresponds to the <see cref="Iteration"/> data
        /// </param>
        /// <param name="iterationContents">
        /// The <see cref="EngineeringModel"/> data that is to be written to the <see cref="ZipOutputStream"/>
        /// </param>
        /// <param name="zipOutputStream">
        /// The target <see cref="ZipOutputStream"/>
        /// </param>
        void WriteIterationToZipFile(EngineeringModelSetup engineeringModelSetup, IterationSetup iterationSetup, IEnumerable<Thing> iterationContents, ZipOutputStream zipOutputStream);

        /// <summary>
        /// Writes the binary data of <see cref="FileRevision"/> objects to the <see cref="ZipOutputStream"/>
        /// </summary>
        /// <param name="engineeringModelSetup">
        /// The <see cref="EngineeringModelSetup"/> that corresponds to the <see cref="EngineeringModel"/> data
        /// </param>
        /// <param name="fileRevisions">
        /// The <see cref="IEnumerable{FileRevision}"/> of which the binary data is to be written to the <see cref="ZipOutputStream"/>
        /// </param>
        /// <param name="zipOutputStream">
        /// The target <see cref="ZipOutputStream"/>
        /// </param>
        void WriteFileRevisionsToZipFile(EngineeringModelSetup engineeringModelSetup, IEnumerable<FileRevision> fileRevisions, ZipOutputStream zipOutputStream);

        /// <summary>
        /// Writes <see cref="ModelReferenceDataLibrary"/> contents to the <see cref="ZipFile"/>
        /// </summary>
        /// <param name="modelReferenceDataLibrary">
        /// The <see cref="ModelReferenceDataLibrary"/> that corresponds to the <see cref="ModelReferenceDataLibrary"/> data
        /// </param>
        /// <param name="rdlContents">
        /// The <see cref="ModelReferenceDataLibrary"/> data that is to be written to the <see cref="ZipOutputStream"/>
        /// </param>
        /// <param name="zipOutputStream">
        /// The target <see cref="ZipOutputStream"/>
        /// </param>
        void WriteModelReferenceDataLibraryToZipFile(ModelReferenceDataLibrary modelReferenceDataLibrary, IEnumerable<Thing> rdlContents, ZipOutputStream zipOutputStream);

        /// <summary>
        /// Writes <see cref="SiteReferenceDataLibrary"/> contents to the <see cref="ZipFile"/>
        /// </summary>
        /// <param name="siteReferenceDataLibrary">
        /// The <see cref="SiteReferenceDataLibrary"/> that corresponds to the <see cref="SiteReferenceDataLibrary"/> data
        /// </param>
        /// <param name="rdlContents">
        /// The <see cref="SiteReferenceDataLibrary"/> data that is to be written to the <see cref="ZipOutputStream"/>
        /// </param>
        /// <param name="zipOutputStream">
        /// The target <see cref="ZipOutputStream"/>
        /// </param>
        void WriteSiteReferenceDataLibraryToZipFile(SiteReferenceDataLibrary siteReferenceDataLibrary, IEnumerable<Thing> rdlContents, ZipOutputStream zipOutputStream);

    }
}
