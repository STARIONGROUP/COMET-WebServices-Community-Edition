// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelReferenceDataLibraryServiceTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Services.Supplemental
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

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
                SiteReferenceDataLibraryDao = this.siteReferenceDataLibraryDao.Object,
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
            this.engineeringModelSetupDao.Setup(x => x.ReadAsync(
                        It.IsAny<NpgsqlTransaction>(), 
                        It.IsAny<string>(), 
                        It.IsAny<IEnumerable<Guid>>(), 
                        It.IsAny<bool>(),
                        null))
                .Returns(Task.FromResult<IEnumerable<EngineeringModelSetup>>(new List<EngineeringModelSetup>()));

            Assert.ThrowsAsync<InvalidOperationException>(
                () => this.modelReferenceDataLibraryService.QueryReferenceDataLibraryAsync(null, this.iteration)
                );
        }

        [Test]
        public void VerifyQueryReferenceDataLibraryThrowsOnNotHavingModelReferenceDataLibrary()
        {
            this.engineeringModelSetupDao.Setup(x => x.ReadAsync(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<bool>(), 
                    null))
                .Returns(Task.FromResult<IEnumerable<EngineeringModelSetup>>(new List<EngineeringModelSetup> {this.engineeringModelSetup}));

            Assert.ThrowsAsync<InvalidOperationException>(
                () => this.modelReferenceDataLibraryService.QueryReferenceDataLibraryAsync(null, this.iteration)
            );
        }

        [Test]
        public async Task VerifyQueryReferenceDataLibraryWorksForModelReferenceDataLibrary()
        {
            this.engineeringModelSetupDao.Setup(x => x.ReadAsync(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<bool>(),
                    null))
                .Returns(Task.FromResult<IEnumerable<EngineeringModelSetup>>(new List<EngineeringModelSetup> { this.engineeringModelSetup }));

            this.modelReferenceDataLibraryDao.Setup( x => x.ReadAsync(
                It.IsAny<NpgsqlTransaction>(),
                It.IsAny<string>(),
                It.IsAny<IEnumerable<Guid>>(),
                It.IsAny<bool>(), 
                null))
                .Returns(Task.FromResult<IEnumerable<ModelReferenceDataLibrary>>(new List<ModelReferenceDataLibrary> { this.modelReferenceDataLibrary}));

            var result = await this.modelReferenceDataLibraryService.QueryReferenceDataLibraryAsync(null, this.iteration);
            
            Assert.That(result, Is.EqualTo(new List<ReferenceDataLibrary> { this.modelReferenceDataLibrary }));
        }

        [Test]
        public async Task VerifyQueryReferenceDataLibraryWorksForModelReferenceDataLibraryWithSingleSiteReferenceLibraryInChainOfRDLs()
        {
            this.engineeringModelSetupDao.Setup(x => x.ReadAsync(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<bool>(), 
                    null))
                .Returns(Task.FromResult<IEnumerable<EngineeringModelSetup>>(new List<EngineeringModelSetup> { this.engineeringModelSetup }));

            this.engineeringModelSetup.RequiredRdl.Add(this.modelReferenceDataLibrary.Iid);

            this.modelReferenceDataLibraryDao.Setup(x => x.ReadAsync(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    new [] { this.modelReferenceDataLibrary.Iid},
                    It.IsAny<bool>(),
                    null))
                .Returns(Task.FromResult<IEnumerable<ModelReferenceDataLibrary>>(new List<ModelReferenceDataLibrary> { this.modelReferenceDataLibrary }));

            this.siteReferenceDataLibraryDao.Setup(x => x.ReadAsync(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    new[] { this.siteReferenceDataLibrary1.Iid },
                    It.IsAny<bool>(), 
                    null))
                .Returns(Task.FromResult<IEnumerable<SiteReferenceDataLibrary>>(new List<SiteReferenceDataLibrary> { this.siteReferenceDataLibrary1 }));

            this.modelReferenceDataLibrary.RequiredRdl = this.siteReferenceDataLibrary1.Iid;

            var result = await this.modelReferenceDataLibraryService.QueryReferenceDataLibraryAsync(null, this.iteration);

            Assert.That(result, Is.EqualTo(new List<ReferenceDataLibrary> { this.modelReferenceDataLibrary, this.siteReferenceDataLibrary1 }));
        }

        [Test]
        public async Task VerifyQueryReferenceDataLibraryWorksForModelReferenceDataLibraryWithMultipleSiteReferenceLibrariesInChainOfRDLs()
        {
            this.engineeringModelSetupDao.Setup(x => x.ReadAsync(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<bool>(),
                    null))
                .Returns(Task.FromResult<IEnumerable<EngineeringModelSetup>>(new List<EngineeringModelSetup> { this.engineeringModelSetup }));

            this.engineeringModelSetup.RequiredRdl.Add(this.modelReferenceDataLibrary.Iid);

            this.modelReferenceDataLibraryDao.Setup(x => x.ReadAsync(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    new[] { this.modelReferenceDataLibrary.Iid },
                    It.IsAny<bool>(),
                    null))
                .Returns(Task.FromResult<IEnumerable<ModelReferenceDataLibrary>>(new List<ModelReferenceDataLibrary> { this.modelReferenceDataLibrary }));

            this.siteReferenceDataLibraryDao.Setup(x => x.ReadAsync(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    new[] { this.siteReferenceDataLibrary1.Iid },
                    It.IsAny<bool>(),
                    null))
                .Returns(Task.FromResult<IEnumerable<SiteReferenceDataLibrary>>(new List<SiteReferenceDataLibrary> { this.siteReferenceDataLibrary1 }));

            this.siteReferenceDataLibraryDao.Setup(x => x.ReadAsync(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    new[] { this.siteReferenceDataLibrary2.Iid },
                    It.IsAny<bool>(),
                    null))
                .Returns(Task.FromResult<IEnumerable<SiteReferenceDataLibrary>>(new List<SiteReferenceDataLibrary> { this.siteReferenceDataLibrary2 }));

            this.modelReferenceDataLibrary.RequiredRdl = this.siteReferenceDataLibrary1.Iid;
            this.siteReferenceDataLibrary1.RequiredRdl = this.siteReferenceDataLibrary2.Iid;

            var result = await this.modelReferenceDataLibraryService.QueryReferenceDataLibraryAsync(null, this.iteration);

            Assert.That(result, Is.EqualTo(new List<ReferenceDataLibrary> { this.modelReferenceDataLibrary, this.siteReferenceDataLibrary1, this.siteReferenceDataLibrary2 }));
        }
    }
}
