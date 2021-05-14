// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeFileProcessorTestFixture.cs" company="RHEA System S.A.">
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

namespace CometServer.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using CDP4JsonSerializer;

    using CometServer.Services;

    using Microsoft.VisualBasic;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ExchangeFileProcessor"/> class
    /// </summary>
    [TestFixture]
    public class ExchangeFileProcessorTestFixture
    {
        private MetaInfoProvider metaInfoProvider;

        private ExchangeFileProcessor exchangeFileProcessor;

        private FileBinaryService fileBinaryService;

        private ICdp4JsonSerializer jsonSerializer;

        private string testFilePath;
        
        [SetUp]
        public void SetUp()
        {
            this.metaInfoProvider = new MetaInfoProvider();

            this.jsonSerializer = new Cdp4JsonSerializer();

            this.exchangeFileProcessor = new ExchangeFileProcessor
            {
                MetaInfoProvider = this.metaInfoProvider,
                FileBinaryService =  this.fileBinaryService,
                JsonSerializer = this.jsonSerializer
            };

            this.testFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "Data.zip");
        }

        [Test]
        public void Verify_that_SiteDirectoryData_can_be_read_from_archive()
        {
            var version = new Version(1, 0, 0);

            var things = this.exchangeFileProcessor.ReadSiteDirectoryFromfile(version, this.testFilePath, null);

            CollectionAssert.IsNotEmpty(things);
        }

    }
}
