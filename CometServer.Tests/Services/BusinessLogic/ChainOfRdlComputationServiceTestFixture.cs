﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChainOfRdlComputationServiceTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Services.BusinessLogic
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.Services;

    using Microsoft.Extensions.Logging;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ChainOfRdlComputationService"/> class
    /// </summary>
    [TestFixture]
    public class ChainOfRdlComputationServiceTestFixture
    {
        private readonly Mock<ILogger<ChainOfRdlComputationService>> logger = new();

        private readonly Mock<IModelReferenceDataLibraryDao> modelReferenceDataLibraryDao = new();

        private readonly Mock<ISiteReferenceDataLibraryDao> siteReferenceDataLibraryDao = new();

        private List<ModelReferenceDataLibrary> modelReferenceDataLibraries;

        private List<SiteReferenceDataLibrary> siteReferenceDataLibraries;

        private ChainOfRdlComputationService chainOfRdlComputationService;

        [SetUp]
        public void SetUp()
        {
            this.CreateTestData();

            this.modelReferenceDataLibraryDao.Setup(x => x.ReadAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<bool>(), null))
                .Returns(Task.FromResult<IEnumerable<ModelReferenceDataLibrary>>(this.modelReferenceDataLibraries));

            this.siteReferenceDataLibraryDao.Setup(x => x.ReadAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<bool>(), null))
                .Returns(Task.FromResult<IEnumerable<SiteReferenceDataLibrary>>(this.siteReferenceDataLibraries));

            this.chainOfRdlComputationService = new ChainOfRdlComputationService
            {
                Logger = this.logger.Object,
                ModelReferenceDataLibraryDao = this.modelReferenceDataLibraryDao.Object,
                SiteReferenceDataLibraryDao = this.siteReferenceDataLibraryDao.Object
            };
        }

        [Test]
        public async Task Verify_that_QueryReferenceDataLibraryDependency_returns_expected_identifiers_for_single_EngineeringModelSetup()
        {
            var engineeringModelSetup_a = new EngineeringModelSetup(Guid.NewGuid(), 1)
            {
                RequiredRdl = [Guid.Parse("38c92eac-4aa5-4418-9b25-9ef76daab374")]
            };

            var engineeringModelSetups = new List<EngineeringModelSetup> { engineeringModelSetup_a };

            var expectedResult = new List<Guid> { Guid.Parse("a43bcec9-3293-4ffa-adf1-2ba36de90d7c"), Guid.Parse("71a05233-52ac-46bb-9d72-03ff99dc32e1"), Guid.Parse("4b2089e5-431e-4e18-96e3-6afcae0ac39a") };
            var result = await this.chainOfRdlComputationService.QueryReferenceDataLibraryDependencyAsync(null, engineeringModelSetups);

            Assert.That(result, Is.EquivalentTo(expectedResult));

            var engineeringModelSetup_b = new EngineeringModelSetup(Guid.NewGuid(), 1)
            {
                RequiredRdl = [Guid.Parse("cf796a26-ec8f-47f6-8f29-7ea3e7083ff8")]
            };

            engineeringModelSetups = [engineeringModelSetup_b];
            expectedResult = [Guid.Parse("71a05233-52ac-46bb-9d72-03ff99dc32e1"), Guid.Parse("4b2089e5-431e-4e18-96e3-6afcae0ac39a")];
            result = await this.chainOfRdlComputationService.QueryReferenceDataLibraryDependencyAsync(null, engineeringModelSetups);

            Assert.That(result, Is.EquivalentTo(expectedResult));
        }

        [Test]
        public async Task Verify_that_QueryReferenceDataLibraryDependency_returns_expected_identifiers_for_multiple_EngineeringModelSetup()
        {
            var engineeringModelSetup_a = new EngineeringModelSetup(Guid.NewGuid(), 1)
            {
                RequiredRdl = [Guid.Parse("38c92eac-4aa5-4418-9b25-9ef76daab374")]
            };

            var engineeringModelSetup_aa = new EngineeringModelSetup(Guid.NewGuid(), 1)
            {
                RequiredRdl = [Guid.Parse("0469a960-0114-41d9-93f2-62862114502d")]
            };

            var engineeringModelSetups = new List<EngineeringModelSetup> { engineeringModelSetup_a, engineeringModelSetup_aa };

            var expectedResult = new List<Guid> { Guid.Parse("a43bcec9-3293-4ffa-adf1-2ba36de90d7c"), Guid.Parse("71a05233-52ac-46bb-9d72-03ff99dc32e1"), Guid.Parse("4b2089e5-431e-4e18-96e3-6afcae0ac39a") };
            var result = await this.chainOfRdlComputationService.QueryReferenceDataLibraryDependencyAsync(null, engineeringModelSetups);

            Assert.That(result, Is.EquivalentTo(expectedResult));
        }

        [Test]
        public async Task Verify_that_QueryReferenceDataLibraryDependency_retuns_empty_result()
        {
            var engineeringModelSetup_a = new EngineeringModelSetup(Guid.NewGuid(), 1);
            var engineeringModelSetups = new List<EngineeringModelSetup> { engineeringModelSetup_a };

            var result = await this.chainOfRdlComputationService.QueryReferenceDataLibraryDependencyAsync(null, engineeringModelSetups);
            Assert.That(result, Is.Empty);

            engineeringModelSetup_a.RequiredRdl = [Guid.NewGuid(), Guid.NewGuid()];
            result = await this.chainOfRdlComputationService.QueryReferenceDataLibraryDependencyAsync(null, engineeringModelSetups);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task Verify_that_when_the_modelrdl_cannot_be_found_empty_empty_is_returned()
        {
            var engineeringModelSetup_a = new EngineeringModelSetup(Guid.NewGuid(), 1)
            {
                RequiredRdl = [Guid.NewGuid()]
            };

            var engineeringModelSetups = new List<EngineeringModelSetup> { engineeringModelSetup_a };

            var result = await this.chainOfRdlComputationService.QueryReferenceDataLibraryDependencyAsync(null, engineeringModelSetups);

            Assert.That(result, Is.Empty);
        }

        private void CreateTestData()
        {
            // modelref_a  -> siteref_a -> siteref_b -> siteref_c
            // modelref_aa -> siteref_a -> siteref_b -> siteref_c
            // modelref_b  -> siteref_b -> siteref_c
            // modelref_c  -> siteref_c

            this.modelReferenceDataLibraries = new List<ModelReferenceDataLibrary>();
            var modelReferenceDataLibrary_a = new ModelReferenceDataLibrary(Guid.Parse("38c92eac-4aa5-4418-9b25-9ef76daab374"), 1);
            this.modelReferenceDataLibraries.Add(modelReferenceDataLibrary_a);
            var modelReferenceDataLibrary_aa = new ModelReferenceDataLibrary(Guid.Parse("0469a960-0114-41d9-93f2-62862114502d"), 1);
            this.modelReferenceDataLibraries.Add(modelReferenceDataLibrary_aa);
            var modelReferenceDataLibrary_b = new ModelReferenceDataLibrary(Guid.Parse("cf796a26-ec8f-47f6-8f29-7ea3e7083ff8"), 1);
            this.modelReferenceDataLibraries.Add(modelReferenceDataLibrary_b);
            var modelReferenceDataLibrary_c = new ModelReferenceDataLibrary(Guid.Parse("e707d516-bea8-4486-b9c5-16b5ee58a020"), 1);
            this.modelReferenceDataLibraries.Add(modelReferenceDataLibrary_c);

            this.siteReferenceDataLibraries = new List<SiteReferenceDataLibrary>();
            var siteReferenceDataLibrary_a = new SiteReferenceDataLibrary(Guid.Parse("a43bcec9-3293-4ffa-adf1-2ba36de90d7c"), 1);
            this.siteReferenceDataLibraries.Add(siteReferenceDataLibrary_a);
            var siteReferenceDataLibrary_b = new SiteReferenceDataLibrary(Guid.Parse("71a05233-52ac-46bb-9d72-03ff99dc32e1"), 1);
            this.siteReferenceDataLibraries.Add(siteReferenceDataLibrary_b);
            var siteReferenceDataLibrary_c = new SiteReferenceDataLibrary(Guid.Parse("4b2089e5-431e-4e18-96e3-6afcae0ac39a"), 1);
            this.siteReferenceDataLibraries.Add(siteReferenceDataLibrary_c);
            var siteReferenceDataLibrary_cc = new SiteReferenceDataLibrary(Guid.Parse("4d8e02c1-6f01-4cb2-b294-2503d7821a46"), 1);
            this.siteReferenceDataLibraries.Add(siteReferenceDataLibrary_cc);

            siteReferenceDataLibrary_a.RequiredRdl = siteReferenceDataLibrary_b.Iid;
            siteReferenceDataLibrary_b.RequiredRdl = siteReferenceDataLibrary_c.Iid;

            modelReferenceDataLibrary_a.RequiredRdl = siteReferenceDataLibrary_a.Iid;
            modelReferenceDataLibrary_aa.RequiredRdl = siteReferenceDataLibrary_a.Iid;
            modelReferenceDataLibrary_b.RequiredRdl = siteReferenceDataLibrary_b.Iid;
            modelReferenceDataLibrary_c.RequiredRdl = siteReferenceDataLibrary_c.Iid;
        }
    }
}
