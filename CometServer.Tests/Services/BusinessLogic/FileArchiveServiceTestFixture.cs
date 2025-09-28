// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileArchiveServiceTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

namespace CometServer.Tests.Services.BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using CDP4Common.DTO;

    using CometServer.Configuration;
    using CometServer.Services;

    using Microsoft.Extensions.Logging;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="FileArchiveService"/>.
    /// </summary>
    [TestFixture]
    public class FileArchiveServiceTestFixture
    {
        private string temporaryRoot;
        private FileArchiveService fileArchiveService;

        [SetUp]
        public void Setup()
        {
            this.temporaryRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(this.temporaryRoot);

            var appConfig = new AppConfig
            {
                Midtier =
                {
                    TemporaryFileStorageDirectory = this.temporaryRoot
                }
            };

            var appConfigService = new Mock<IAppConfigService>();
            appConfigService.Setup(x => x.AppConfig).Returns(appConfig);

            this.fileArchiveService = new FileArchiveService
            {
                AppConfigService = appConfigService.Object,
                Logger = Mock.Of<ILogger<FileArchiveService>>()
            };
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(this.temporaryRoot))
            {
                Directory.Delete(this.temporaryRoot, true);
            }
        }

        [Test]
        public void CreateFolderAndFileStructureOnDiskThrowsWhenResourceNotFileStore()
        {
            var things = new List<Thing> { new ElementDefinition() };

            Assert.ThrowsAsync<InvalidOperationException>(() => this.fileArchiveService.CreateFolderAndFileStructureOnDiskAsync(things, "partition", Array.Empty<string>()));
        }

        [Test]
        public void CreateZipArchiveCreatesZipFile()
        {
            var sourceDirectory = Directory.CreateDirectory(Path.Combine(this.temporaryRoot, "source"));
            File.WriteAllText(Path.Combine(sourceDirectory.FullName, "file.txt"), "content");

            this.fileArchiveService.CreateZipArchive(sourceDirectory);

            var expectedArchive = Path.Combine(this.temporaryRoot, "source.zip");
            Assert.That(File.Exists(expectedArchive), Is.True);
        }

        [Test]
        public void DeleteFolderAndFileStructureRemovesFolderAndZip()
        {
            var folder = Directory.CreateDirectory(Path.Combine(this.temporaryRoot, "toDelete"));
            var zipPath = Path.Combine(this.temporaryRoot, "toDelete.zip");
            File.WriteAllText(Path.Combine(folder.FullName, "file.txt"), "content");
            File.WriteAllText(zipPath, "zipcontent");

            this.fileArchiveService.DeleteFolderAndFileStructureAndArchive(folder);

            Assert.That(Directory.Exists(folder.FullName), Is.False);
            Assert.That(File.Exists(zipPath), Is.False);
        }
    }
}
