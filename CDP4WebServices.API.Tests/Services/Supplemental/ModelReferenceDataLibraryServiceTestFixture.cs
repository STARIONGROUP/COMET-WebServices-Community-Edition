// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelReferenceDataLibraryServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace CometServer.Tests.Services.Supplemental
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.Helpers;
    using CometServer.Services;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    [TestFixture]
    public class ModelReferenceDataLibraryServiceTestFixture
    {
        private Mock<ISiteReferenceDataLibraryDao> siteReferenceDataLibraryDao;
        private Mock<IEngineeringModelSetupDao> engineeringModelSetupDao;
        private Mock<IModelReferenceDataLibraryDao> modelReferenceDataLibraryDao;
        
        private Mock<ICdp4TransactionManager> transactionManager;
        private ModelReferenceDataLibrary modelReferenceDataLibrary;
        private SiteReferenceDataLibrary siteReferenceDataLibrary1;
        private SiteReferenceDataLibrary siteReferenceDataLibrary2;
        private Iteration iteration;
        private IterationSetup iterationSetup;
        private EngineeringModelSetup engineeringModelSetup;

        private ModelReferenceDataLibraryService modelReferenceDataLibraryService;

        [SetUp]
        public void Setup()
        {
            this.siteReferenceDataLibraryDao = new Mock<ISiteReferenceDataLibraryDao>();
            this.engineeringModelSetupDao = new Mock<IEngineeringModelSetupDao>();
            this.modelReferenceDataLibraryDao = new Mock<IModelReferenceDataLibraryDao>();
            this.transactionManager = new Mock<ICdp4TransactionManager>();

            this.transactionManager.Setup(x => x.IsFullAccessEnabled()).Returns(true);

            this.modelReferenceDataLibraryService = new ModelReferenceDataLibraryService
            {
                EngineeringModelSetupDao = this.engineeringModelSetupDao.Object,
                SiteReferenceDataLibraryDato = this.siteReferenceDataLibraryDao.Object,
                ModelReferenceDataLibraryDao = this.modelReferenceDataLibraryDao.Object,
                TransactionManager = this.transactionManager.Object
            };

            this.siteReferenceDataLibrary1 = new SiteReferenceDataLibrary(Guid.NewGuid(), 0);
            this.siteReferenceDataLibrary2 = new SiteReferenceDataLibrary(Guid.NewGuid(), 0);
            this.modelReferenceDataLibrary = new ModelReferenceDataLibrary(Guid.NewGuid(), 0);

            this.iteration = new Iteration(Guid.NewGuid(), 0);
            this.iterationSetup = new IterationSetup(Guid.NewGuid(), 0);
            this.engineeringModelSetup = new EngineeringModelSetup(Guid.NewGuid(), 0);

            this.iteration.IterationSetup = this.iterationSetup.Iid;

            this.engineeringModelSetup.IterationSetup.Add(this.iterationSetup.Iid);
        }

        [Test]
        public void VerifyQueryReferenceDataLibraryThrowsOnUnknownEngineeringModelSetup()
        {
            this.engineeringModelSetupDao.Setup(x => x.Read(
                        It.IsAny<NpgsqlTransaction>(), 
                        It.IsAny<string>(), 
                        It.IsAny<IEnumerable<Guid>>(), 
                        It.IsAny<bool>()))
                .Returns(new List<EngineeringModelSetup>());

            Assert.Throws<InvalidOperationException>(
                () => this.modelReferenceDataLibraryService.QueryReferenceDataLibrary(null, this.iteration)
                );
        }

        [Test]
        public void VerifyQueryReferenceDataLibraryThrowsOnNotHavingModelReferenceDataLibrary()
        {
            this.engineeringModelSetupDao.Setup(x => x.Read(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<bool>()))
                .Returns(new List<EngineeringModelSetup> {this.engineeringModelSetup});

            Assert.Throws<InvalidOperationException>(
                () => this.modelReferenceDataLibraryService.QueryReferenceDataLibrary(null, this.iteration)
            );
        }

        [Test]
        public void VerifyQueryReferenceDataLibraryWorksForModelReferenceDataLibrary()
        {
            this.engineeringModelSetupDao.Setup(x => x.Read(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<bool>()))
                .Returns(new List<EngineeringModelSetup> { this.engineeringModelSetup });

            this.modelReferenceDataLibraryDao.Setup( x => x.Read(
                It.IsAny<NpgsqlTransaction>(),
                It.IsAny<string>(),
                It.IsAny<IEnumerable<Guid>>(),
                It.IsAny<bool>()))
                .Returns(new List<ModelReferenceDataLibrary> { this.modelReferenceDataLibrary});

            var result = this.modelReferenceDataLibraryService.QueryReferenceDataLibrary(null, this.iteration);

            CollectionAssert.AreEqual(new List<ReferenceDataLibrary> { this.modelReferenceDataLibrary }, result);
        }

        [Test]
        public void VerifyQueryReferenceDataLibraryWorksForModelReferenceDataLibraryWithSingleSiteReferenceLibraryInChainOfRDLs()
        {
            this.engineeringModelSetupDao.Setup(x => x.Read(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<bool>()))
                .Returns(new List<EngineeringModelSetup> { this.engineeringModelSetup });

            this.engineeringModelSetup.RequiredRdl.Add(this.modelReferenceDataLibrary.Iid);

            this.modelReferenceDataLibraryDao.Setup(x => x.Read(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    new [] { this.modelReferenceDataLibrary.Iid},
                    It.IsAny<bool>()))
                .Returns(new List<ModelReferenceDataLibrary> { this.modelReferenceDataLibrary });

            this.siteReferenceDataLibraryDao.Setup(x => x.Read(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    new[] { this.siteReferenceDataLibrary1.Iid },
                    It.IsAny<bool>()))
                .Returns(new List<SiteReferenceDataLibrary> { this.siteReferenceDataLibrary1 });

            this.modelReferenceDataLibrary.RequiredRdl = this.siteReferenceDataLibrary1.Iid;

            var result = this.modelReferenceDataLibraryService.QueryReferenceDataLibrary(null, this.iteration);

            CollectionAssert.AreEqual(new List<ReferenceDataLibrary> { this.modelReferenceDataLibrary, this.siteReferenceDataLibrary1 }, result);
        }

        [Test]
        public void VerifyQueryReferenceDataLibraryWorksForModelReferenceDataLibraryWithMultipleSiteReferenceLibrariesInChainOfRDLs()
        {
            this.engineeringModelSetupDao.Setup(x => x.Read(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<bool>()))
                .Returns(new List<EngineeringModelSetup> { this.engineeringModelSetup });

            this.engineeringModelSetup.RequiredRdl.Add(this.modelReferenceDataLibrary.Iid);

            this.modelReferenceDataLibraryDao.Setup(x => x.Read(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    new[] { this.modelReferenceDataLibrary.Iid },
                    It.IsAny<bool>()))
                .Returns(new List<ModelReferenceDataLibrary> { this.modelReferenceDataLibrary });

            this.siteReferenceDataLibraryDao.Setup(x => x.Read(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    new[] { this.siteReferenceDataLibrary1.Iid },
                    It.IsAny<bool>()))
                .Returns(new List<SiteReferenceDataLibrary> { this.siteReferenceDataLibrary1 });

            this.siteReferenceDataLibraryDao.Setup(x => x.Read(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    new[] { this.siteReferenceDataLibrary2.Iid },
                    It.IsAny<bool>()))
                .Returns(new List<SiteReferenceDataLibrary> { this.siteReferenceDataLibrary2 });

            this.modelReferenceDataLibrary.RequiredRdl = this.siteReferenceDataLibrary1.Iid;
            this.siteReferenceDataLibrary1.RequiredRdl = this.siteReferenceDataLibrary2.Iid;

            var result = this.modelReferenceDataLibraryService.QueryReferenceDataLibrary(null, this.iteration);

            CollectionAssert.AreEqual(new List<ReferenceDataLibrary> { this.modelReferenceDataLibrary, this.siteReferenceDataLibrary1, this.siteReferenceDataLibrary2 }, result);
        }
    }
}
