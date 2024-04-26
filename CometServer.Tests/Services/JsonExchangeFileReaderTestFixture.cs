// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonExchangeFileReaderTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
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

namespace CometServer.Tests.Services
{
    using System;
    using System.IO;
    using System.Linq;

    using CDP4Common.DTO;

    using CDP4JsonSerializer;

    using CometServer.Services;

    using Microsoft.Extensions.Logging;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="JsonExchangeFileReader"/> class
    /// </summary>
    [TestFixture]
    public class JsonExchangeFileReaderTestFixture
    {
        private Mock<ILogger<JsonExchangeFileReader>> logger;

        private MetaInfoProvider metaInfoProvider;

        private JsonExchangeFileReader jsonExchangeFileReader;

        private Mock<IFileBinaryService> fileBinaryService;

        private ICdp4JsonSerializer jsonSerializer;

        private string testFilePath;
        
        [SetUp]
        public void SetUp()
        {
            this.logger = new Mock<ILogger<JsonExchangeFileReader>>();

            this.fileBinaryService = new Mock<IFileBinaryService>();

            this.metaInfoProvider = new MetaInfoProvider();

            this.jsonSerializer = new Cdp4JsonSerializer();

            this.jsonExchangeFileReader = new JsonExchangeFileReader
            {
                MetaInfoProvider = this.metaInfoProvider,
                FileBinaryService =  this.fileBinaryService.Object,
                JsonSerializer = this.jsonSerializer,
                Logger = this.logger.Object
            };

            this.testFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "Data.zip");
        }

        [Test]
        public void Verify_that_SiteDirectoryData_can_be_read_from_archive()
        {
            var version = new Version(1, 0, 0);

            var things = this.jsonExchangeFileReader.ReadSiteDirectoryFromfile(version, this.testFilePath, null);

            Assert.That(things.Count, Is.EqualTo(110));
        }

        [Test]
        public void Verify_that_engineeringModelData_can_be_read_from_archive()
        {
            var version = new Version(1, 0, 0);

            var engineeringModelSetup = new EngineeringModelSetup(Guid.Parse("116f6253-89bb-47d4-aa24-d11d197e43c9"), 1)
            {
                EngineeringModelIid = Guid.Parse("9ec982e4-ef72-4953-aa85-b158a95d8d56")
            };

            var things = this.jsonExchangeFileReader.ReadEngineeringModelFromfile(version, this.testFilePath, null, engineeringModelSetup);

            Assert.That(things.Count, Is.EqualTo(3));
        }

        [Test]
        public void Verify_that_IterationData_can_be_read_from_archive()
        {
            var version = new Version(1, 0, 0);

            var engineeringModelSetup = new EngineeringModelSetup(Guid.Parse("116f6253-89bb-47d4-aa24-d11d197e43c9"), 1)
            {
                EngineeringModelIid = Guid.Parse("9ec982e4-ef72-4953-aa85-b158a95d8d56")
            };

            var iterationSetup = new IterationSetup(Guid.Parse("86163b0e-8341-4316-94fc-93ed60ad0dcf"), 1)
            {
                IterationIid = Guid.Parse("e163c5ad-f32b-4387-b805-f4b34600bc2c")
            };

            var things = this.jsonExchangeFileReader.ReadModelIterationFromFile(version, this.testFilePath,  null,engineeringModelSetup, iterationSetup);

            Assert.That(things.Count, Is.EqualTo(45));
        }

        [Test]
        public void Verify_that_StoreFileBinary_executes_without_failure()
        {
            var engineeringModelSetup = new EngineeringModelSetup(Guid.Parse("116f6253-89bb-47d4-aa24-d11d197e43c9"), 1)
            {
                EngineeringModelIid = Guid.Parse("9ec982e4-ef72-4953-aa85-b158a95d8d56")
            };

            Assert.That(() => this.jsonExchangeFileReader .ReadAndStoreFileBinary(this.testFilePath, null, engineeringModelSetup, "B95EC201AE3EE89D407449D692E69BB97C228A7E"),
                Throws.Nothing);

            this.fileBinaryService.Verify(x => x.StoreBinaryData("B95EC201AE3EE89D407449D692E69BB97C228A7E", It.IsAny<Stream>()),Times.Once);
        }
    }
}
