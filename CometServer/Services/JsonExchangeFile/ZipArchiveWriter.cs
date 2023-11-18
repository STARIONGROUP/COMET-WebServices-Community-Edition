// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZipArchiveWriter.cs" company="RHEA System S.A.">
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using CDP4Common.DTO;

    using CDP4JsonSerializer;

    using ICSharpCode.SharpZipLib.Zip;

    using Microsoft.Extensions.Logging;

    public class ZipArchiveWriter : IZipArchiveWriter
    {
        /// <summary>
        /// Gets or sets the (injected) <see cref="ILogger"/>
        /// </summary>
        public ILogger<ZipArchiveWriter> Logger { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="ICdp4JsonSerializer"/>
        /// </summary>
        public ICdp4JsonSerializer JsonSerializer { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IFileBinaryService"/>
        /// </summary>
        public IFileBinaryService FileBinaryService { get; set; }

        /// <summary>
        /// Write the header file to the <see cref="ZipOutputStream"/>
        /// </summary>
        /// <param name="echExchangeFileHeader">
        /// The <see cref="ExchangeFileHeader"/> that is to be written to the <see cref="zipOutputStream"/>
        /// </param>
        /// <param name="zipOutputStream">
        /// The <see cref="ZipOutputStream"/> instance to add the information to.
        /// </param>
        public void WriteHeaderToZipFile(ExchangeFileHeader echExchangeFileHeader, ZipOutputStream zipOutputStream)
        {
            var zipEntry = new ZipEntry("Header.json")
            {
                Comment = "The Header for this file based source",
                DateTime = DateTime.UtcNow
            };

            zipOutputStream.PutNextEntry(zipEntry);

            using var memoryStream = new MemoryStream();

            this.JsonSerializer.SerializeToStream(echExchangeFileHeader, memoryStream);

            memoryStream.Position = 0;
            memoryStream.CopyTo(zipOutputStream);

            zipOutputStream.CloseEntry();
        }

        /// <summary>
        /// Writes <see cref="SiteDirectory"/> contents to the <see cref="ZipOutputStream"/>
        /// </summary>
        /// <param name="siteDirectoryContents">
        /// The <see cref="SiteDirectory"/> data that has been pruned of all unnecessary data
        /// </param>
        /// <param name="zipOutputStream">
        /// The <see cref="ZipOutputStream"/> instance to add the information to.
        /// </param>
        public void WriteSiteDirectoryToZipFile(IEnumerable<Thing> siteDirectoryContents, ZipOutputStream zipOutputStream)
        {
            var zipEntry = new ZipEntry("SiteDirectory.json")
            {
                Comment = "The SiteDirectory for this file based source",
                DateTime = DateTime.UtcNow
            };

            zipOutputStream.PutNextEntry(zipEntry);

            using var memoryStream = new MemoryStream();

            this.JsonSerializer.SerializeToStream(siteDirectoryContents, memoryStream);

            memoryStream.Position = 0;
            memoryStream.CopyTo(zipOutputStream);

            zipOutputStream.CloseEntry();
        }

        /// <summary>
        /// Writes <see cref="EngineeringModel"/> contents to the <see cref="ZipOutputStream"/>
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
        public void WriteEngineeringModelToZipFile(EngineeringModelSetup engineeringModelSetup, IEnumerable<Thing> engineeringModelContents, ZipOutputStream zipOutputStream)
        {
            var fileRevisionsZipEntry = new ZipEntry($"EngineeringModels/{engineeringModelSetup.EngineeringModelIid}/FileRevisions/")
            {
                Comment = $"The {engineeringModelSetup.ShortName} EngineeringModel FileRevisions",
                DateTime = DateTime.UtcNow
            };

            zipOutputStream.PutNextEntry(fileRevisionsZipEntry);

            var engineeringModelZipEntry = new ZipEntry($"EngineeringModels/{engineeringModelSetup.EngineeringModelIid}/{engineeringModelSetup.EngineeringModelIid}.json")
            {
                Comment = $"The {engineeringModelSetup.ShortName} EngineeringModel",
                DateTime = DateTime.UtcNow
            };

            zipOutputStream.PutNextEntry(engineeringModelZipEntry);
            
            using var memoryStream = new MemoryStream();

            this.JsonSerializer.SerializeToStream(engineeringModelContents, memoryStream);

            memoryStream.Position = 0;
            memoryStream.CopyTo(zipOutputStream);

            zipOutputStream.CloseEntry();
        }

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
        public void WriteIterationToZipFile(EngineeringModelSetup engineeringModelSetup, IterationSetup iterationSetup, IEnumerable<Thing> iterationContents, ZipOutputStream zipOutputStream)
        {
            var zipEntry = new ZipEntry($"EngineeringModels/{engineeringModelSetup.EngineeringModelIid}/Iterations/{iterationSetup.IterationIid}.json")
            {
                Comment = $"The {engineeringModelSetup.ShortName} EngineeringModel",
                DateTime = DateTime.UtcNow
            };

            zipOutputStream.PutNextEntry(zipEntry);

            using var memoryStream = new MemoryStream();

            this.JsonSerializer.SerializeToStream(iterationContents, memoryStream);

            memoryStream.Position = 0;
            memoryStream.CopyTo(zipOutputStream);

            zipOutputStream.CloseEntry();
        }

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
        public void WriteFileRevisionsToZipFile(EngineeringModelSetup engineeringModelSetup, IEnumerable<FileRevision> fileRevisions, ZipOutputStream zipOutputStream)
        {
            var uniqueContent = 
                fileRevisions
                    .Select(x => x.ContentHash)
                    .Distinct()
                    .Select(x => new { ContentHash = x });

            foreach (var fileRevision in uniqueContent)
            {
                if (this.FileBinaryService.IsFilePersisted(fileRevision.ContentHash))
                {
                    var fileRevisionZipEntry = new ZipEntry($"EngineeringModels/{engineeringModelSetup.EngineeringModelIid}/FileRevisions/{fileRevision.ContentHash}")
                    {
                        Comment = $"FileRevision: {fileRevision.ContentHash}",
                        DateTime = DateTime.UtcNow
                    };

                    zipOutputStream.PutNextEntry(fileRevisionZipEntry);

                    using var fileStream = this.FileBinaryService.RetrieveBinaryData(fileRevision.ContentHash);
                    fileStream.Position = 0;
                    fileStream.CopyTo(zipOutputStream);

                    zipOutputStream.CloseEntry();
                }
            }
        }

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
        public void WriteModelReferenceDataLibraryToZipFile(ModelReferenceDataLibrary modelReferenceDataLibrary, IEnumerable<Thing> rdlContents, ZipOutputStream zipOutputStream)
        {
            var zipEntry = new ZipEntry($"ModelReferenceDataLibraries/{modelReferenceDataLibrary.Iid}.json")
            {
                Comment = $"The {modelReferenceDataLibrary.ShortName} ModelReferenceDataLibrary",
                DateTime = DateTime.UtcNow
            };

            zipOutputStream.PutNextEntry(zipEntry);

            using var memoryStream = new MemoryStream();

            this.JsonSerializer.SerializeToStream(rdlContents, memoryStream);

            memoryStream.Position = 0;
            memoryStream.CopyTo(zipOutputStream);

            zipOutputStream.CloseEntry();
        }

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
        public void WriteSiteReferenceDataLibraryToZipFile(SiteReferenceDataLibrary siteReferenceDataLibrary, IEnumerable<Thing> rdlContents, ZipOutputStream zipOutputStream)
        {
            var zipEntry = new ZipEntry($"SiteReferenceDataLibraries/{siteReferenceDataLibrary.Iid}.json")
            {
                Comment = $"The {siteReferenceDataLibrary.ShortName} SiteReferenceDataLibrary",
                DateTime = DateTime.UtcNow
            };

            zipOutputStream.PutNextEntry(zipEntry);

            using var memoryStream = new MemoryStream();

            this.JsonSerializer.SerializeToStream(rdlContents, memoryStream);

            memoryStream.Position = 0;
            memoryStream.CopyTo(zipOutputStream);

            zipOutputStream.CloseEntry();
        }
    }
}
