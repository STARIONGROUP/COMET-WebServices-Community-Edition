// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChainOfRdlComputationServiceTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski
//
//    This file is part of COMET Web Services Community Edition. 
//    The COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.Services.Supplemental
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CDP4WebServices.API.Services;

    using NUnit.Framework;

    using Moq;

    using Npgsql;

    /// <summary>
    /// Suite of tests for the <see cref="ChainOfRdlComputationService"/> class
    /// </summary>
    [TestFixture]
    public class ChainOfRdlComputationServiceTestFixture
    {
        private Mock<IModelReferenceDataLibraryDao> modelReferenceDataLibraryDao = new Mock<IModelReferenceDataLibraryDao>();

        private Mock<ISiteReferenceDataLibraryDao> siteReferenceDataLibraryDao = new Mock<ISiteReferenceDataLibraryDao>();

        private List<ModelReferenceDataLibrary> modelReferenceDataLibraries;

        private List<SiteReferenceDataLibrary> siteReferenceDataLibraries;

        private ChainOfRdlComputationService chainOfRdlComputationService;
        
        [SetUp]
        public void SetUp()
        {
            this.CreateTestData();

            this.modelReferenceDataLibraryDao.Setup(x => x.Read(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<bool>()))
                .Returns(this.modelReferenceDataLibraries);
            this.siteReferenceDataLibraryDao.Setup(x => x.Read(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<bool>()))
                .Returns(this.siteReferenceDataLibraries);

            this.chainOfRdlComputationService = new ChainOfRdlComputationService();
            this.chainOfRdlComputationService.ModelReferenceDataLibraryDao = this.modelReferenceDataLibraryDao.Object;
            this.chainOfRdlComputationService.SiteReferenceDataLibraryDao = this.siteReferenceDataLibraryDao.Object;
        }

        [Test]
        public void Verify_that_QueryReferenceDataLibraryDependency_returns_expected_identifiers_for_single_EngineeringModelSetup()
        {
            var engineeringModelSetup_a = new EngineeringModelSetup(Guid.NewGuid(), 1);
            engineeringModelSetup_a.RequiredRdl = new List<Guid> { Guid.Parse("38c92eac-4aa5-4418-9b25-9ef76daab374") };

            var engineeringModelSetups = new List<EngineeringModelSetup> { engineeringModelSetup_a };

            var expectedResult = new List<Guid> { Guid.Parse("a43bcec9-3293-4ffa-adf1-2ba36de90d7c"), Guid.Parse("71a05233-52ac-46bb-9d72-03ff99dc32e1"), Guid.Parse("4b2089e5-431e-4e18-96e3-6afcae0ac39a") };
            var result = this.chainOfRdlComputationService.QueryReferenceDataLibraryDependency(null, engineeringModelSetups);

            CollectionAssert.AreEquivalent(expectedResult, result);

            var engineeringModelSetup_b = new EngineeringModelSetup(Guid.NewGuid(), 1);
            engineeringModelSetup_b.RequiredRdl = new List<Guid> { Guid.Parse("cf796a26-ec8f-47f6-8f29-7ea3e7083ff8") };

            engineeringModelSetups = new List<EngineeringModelSetup> { engineeringModelSetup_b };
            expectedResult = new List<Guid> { Guid.Parse("71a05233-52ac-46bb-9d72-03ff99dc32e1"), Guid.Parse("4b2089e5-431e-4e18-96e3-6afcae0ac39a") };
            result = this.chainOfRdlComputationService.QueryReferenceDataLibraryDependency(null, engineeringModelSetups);
            CollectionAssert.AreEquivalent(expectedResult, result);
        }

        [Test]
        public void Verify_that_QueryReferenceDataLibraryDependency_returns_expected_identifiers_for_multiple_EngineeringModelSetup()
        {
            var engineeringModelSetup_a = new EngineeringModelSetup(Guid.NewGuid(), 1);
            engineeringModelSetup_a.RequiredRdl = new List<Guid> { Guid.Parse("38c92eac-4aa5-4418-9b25-9ef76daab374") };

            var engineeringModelSetup_aa = new EngineeringModelSetup(Guid.NewGuid(), 1);
            engineeringModelSetup_aa.RequiredRdl = new List<Guid> { Guid.Parse("0469a960-0114-41d9-93f2-62862114502d") };

            var engineeringModelSetups = new List<EngineeringModelSetup> { engineeringModelSetup_a, engineeringModelSetup_aa };

            var expectedResult = new List<Guid> { Guid.Parse("a43bcec9-3293-4ffa-adf1-2ba36de90d7c"), Guid.Parse("71a05233-52ac-46bb-9d72-03ff99dc32e1"), Guid.Parse("4b2089e5-431e-4e18-96e3-6afcae0ac39a") };
            var result = this.chainOfRdlComputationService.QueryReferenceDataLibraryDependency(null, engineeringModelSetups);

            CollectionAssert.AreEquivalent(expectedResult, result);
        }

        [Test]
        public void Verify_that_QueryReferenceDataLibraryDependency_retuns_empty_result()
        {
            var engineeringModelSetup_a = new EngineeringModelSetup(Guid.NewGuid(), 1);
            var engineeringModelSetups = new List<EngineeringModelSetup> { engineeringModelSetup_a };
            
            var result = this.chainOfRdlComputationService.QueryReferenceDataLibraryDependency(null, engineeringModelSetups);
            Assert.That(result, Is.Empty);

            engineeringModelSetup_a.RequiredRdl = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            result = this.chainOfRdlComputationService.QueryReferenceDataLibraryDependency(null, engineeringModelSetups);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Verify_that_when_the_modelrdl_cannot_be_found_empty_empty_is_returned()
        {
            var engineeringModelSetup_a = new EngineeringModelSetup(Guid.NewGuid(), 1);
            engineeringModelSetup_a.RequiredRdl = new List<Guid> { Guid.NewGuid() };
            var engineeringModelSetups = new List<EngineeringModelSetup> { engineeringModelSetup_a };

            var result = this.chainOfRdlComputationService.QueryReferenceDataLibraryDependency(null, engineeringModelSetups);

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
