// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonExchangeFileReaderTestFixture.cs" company="RHEA System S.A.">
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

namespace CometServer.Tests.Services
{
    using System;
    using System.IO;

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

        private FileBinaryService fileBinaryService;

        private ICdp4JsonSerializer jsonSerializer;

        private string testFilePath;
        
        [SetUp]
        public void SetUp()
        {
            this.logger = new Mock<ILogger<JsonExchangeFileReader>>();

            this.metaInfoProvider = new MetaInfoProvider();

            this.jsonSerializer = new Cdp4JsonSerializer();

            this.jsonExchangeFileReader = new JsonExchangeFileReader
            {
                MetaInfoProvider = this.metaInfoProvider,
                FileBinaryService =  this.fileBinaryService,
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

            CollectionAssert.IsNotEmpty(things);
        }

    }
}
