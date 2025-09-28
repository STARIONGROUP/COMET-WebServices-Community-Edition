// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileBinaryServiceTestFixture.cs" company="Starion Group S.A.">
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
    using System.IO;
    using System.Text;

    using CometServer.Configuration;
    using CometServer.Services;

    using Microsoft.Extensions.Logging;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Unit tests for <see cref="FileBinaryService"/>.
    /// </summary>
    [TestFixture]
    public class FileBinaryServiceTestFixture
    {
        private FileBinaryService fileBinaryService;
        private string storageRoot;

        [SetUp]
        public void Setup()
        {
            this.storageRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(this.storageRoot);

            var appConfig = new AppConfig
            {
                Midtier =
                {
                    FileStorageDirectory = this.storageRoot
                }
            };

            var appConfigService = new Mock<IAppConfigService>();
            appConfigService.Setup(x => x.AppConfig).Returns(appConfig);

            this.fileBinaryService = new FileBinaryService
            {
                AppConfigService = appConfigService.Object,
                Logger = Mock.Of<ILogger<FileBinaryService>>()
            };
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(this.storageRoot))
            {
                Directory.Delete(this.storageRoot, true);
            }
        }

        [Test]
        public void StoreAndRetrieveBinaryDataRoundTripsContent()
        {
            var data = Encoding.UTF8.GetBytes("Hello World");
            var hash = "ABCDEF1234567890ABCDEF1234567890ABCDEF12";

            using (var stream = new MemoryStream(data))
            {
                this.fileBinaryService.StoreBinaryData(hash, stream);
            }

            Assert.That(this.fileBinaryService.IsFilePersisted(hash), Is.True);

            using var retrieved = this.fileBinaryService.RetrieveBinaryData(hash);
            using var reader = new MemoryStream();
            retrieved.CopyTo(reader);

            Assert.That(reader.ToArray(), Is.EqualTo(data));
        }

        [Test]
        public void StoreBinaryDataIsIdempotent()
        {
            var hash = "1234567890ABCDEF1234567890ABCDEF12345678";
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("First"));
            this.fileBinaryService.StoreBinaryData(hash, stream);

            using var duplicateStream = new MemoryStream(Encoding.UTF8.GetBytes("Second"));
            this.fileBinaryService.StoreBinaryData(hash, duplicateStream);

            using var retrieved = this.fileBinaryService.RetrieveBinaryData(hash);
            using var reader = new StreamReader(retrieved);
            Assert.That(reader.ReadToEnd(), Is.EqualTo("First"));
        }

        [Test]
        public void TryGetFileStoragePathReturnsFalseWhenMissing()
        {
            var result = this.fileBinaryService.TryGetFileStoragePath("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF", out var filePath);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.False);
                Assert.That(filePath, Does.Contain(Path.Combine(this.storageRoot, "f")));
            }
        }

        [Test]
        public void RetrieveBinaryDataThrowsWhenMissing()
        {
            Assert.Throws<FileNotFoundException>(() => this.fileBinaryService.RetrieveBinaryData("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"));
        }
    }
}
