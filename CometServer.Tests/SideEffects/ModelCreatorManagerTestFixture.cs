// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelCreatorManagerTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
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

namespace CometServer.Tests.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.MetaInfo;

    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations;
    using CometServer.Services.Protocol;
    
    using Moq;

    using Npgsql;

    using NUnit.Framework;

    using Thing = CDP4Common.DTO.Thing;

    [TestFixture]
    internal class ModelCreatorManagerTestFixture
    {
        private ModelCreatorManager modelCreatorManager;

        private Mock<IRequestUtils> RequestUtils;

        private Mock<IMetaInfoProvider> MetainfoProvider;

        private Mock<IEngineeringModelSetupService> modelSetupService;

        private Mock<ICdp4TransactionManager> transactionManager;

        private List<Thing> requestUtilCache;

        [SetUp]
        public void Setup()
        {
            this.RequestUtils = new Mock<IRequestUtils>();
            this.MetainfoProvider = new Mock<IMetaInfoProvider>();
            this.modelSetupService = new Mock<IEngineeringModelSetupService>();
            this.transactionManager = new Mock<ICdp4TransactionManager>();

            this.modelCreatorManager = new ModelCreatorManager()
            {
                RequestUtils = this.RequestUtils.Object,
                MetainfoProvider = this.MetainfoProvider.Object,
                EngineeringModelSetupService = this.modelSetupService.Object,
                TransactionManager = this.transactionManager.Object
            };

            this.transactionManager.Setup(x => x.GetTransactionTime(It.IsAny<NpgsqlTransaction>())).Returns(DateTime.Now);

            this.requestUtilCache = new List<Thing>();

            this.RequestUtils.Setup(x => x.QueryParameters).Returns(new QueryParameters());
            this.RequestUtils.Setup(x => x.Cache).Returns(this.requestUtilCache);

            this.MetainfoProvider.Setup(x => x.GetMetaInfo(ClassKind.EngineeringModelSetup.ToString())).Returns(new EngineeringModelSetupMetaInfo());
            this.MetainfoProvider.Setup(x => x.GetMetaInfo(ClassKind.ModelReferenceDataLibrary.ToString())).Returns(new ModelReferenceDataLibraryMetaInfo());
            this.MetainfoProvider.Setup(x => x.GetMetaInfo(ClassKind.Category.ToString())).Returns(new CategoryMetaInfo());
            this.MetainfoProvider.Setup(x => x.GetMetaInfo(ClassKind.Participant.ToString())).Returns(new ParticipantMetaInfo());
            this.MetainfoProvider.Setup(x => x.GetMetaInfo(ClassKind.IterationSetup.ToString())).Returns(new IterationSetupMetaInfo());
        }

        [Test]
        public void VerifyThatModelSetupIsCopiedCorrectly()
        {
            var modelSetup = new EngineeringModelSetup(Guid.NewGuid(), 0);
            var mrdl = new ModelReferenceDataLibrary(Guid.NewGuid(), 0);
            var cat = new Category(Guid.NewGuid(), 0);

            var participant = new Participant(Guid.NewGuid(), 0)
            {
              Person = Guid.NewGuid(),
              Role = Guid.NewGuid()
            };

            var iterationSetup1 = new IterationSetup(Guid.NewGuid(), 0)
            {
                FrozenOn = DateTime.Now,
                IterationNumber = 1
            };

            var iterationSetup2 = new IterationSetup(Guid.NewGuid(), 0)
            {
                SourceIterationSetup = iterationSetup1.Iid,
                IterationNumber = 2
            };

            participant.Domain.Add(Guid.NewGuid());
            mrdl.DefinedCategory.Add(cat.Iid);
            mrdl.RequiredRdl = Guid.NewGuid();
            modelSetup.RequiredRdl.Add(mrdl.Iid);
            modelSetup.IterationSetup.Add(iterationSetup1.Iid);
            modelSetup.IterationSetup.Add(iterationSetup2.Iid);
            modelSetup.Participant.Add(participant.Iid);

            this.modelSetupService.Setup(x => x.GetDeep(It.IsAny<NpgsqlTransaction>(), "SiteDirectory", It.Is<IEnumerable<Guid>>(y => y.Contains(modelSetup.Iid)), It.IsAny<ISecurityContext>()))
                .Returns(new Thing[] { modelSetup, mrdl, cat, participant, iterationSetup1, iterationSetup2 });

            var copy = new EngineeringModelSetup(Guid.NewGuid(), 0);

            this.modelCreatorManager.CreateEngineeringModelSetupFromSource(modelSetup.Iid, copy, null, null);

            Assert.That(copy.RequiredRdl, Is.Not.Empty);
            Assert.That(copy.IterationSetup, Is.Not.Empty);
            Assert.That(copy.Participant, Is.Not.Empty);
            Assert.That(copy.IterationSetup.Count, Is.EqualTo(1));
            Assert.That(this.requestUtilCache.Count, Is.EqualTo(5));
            Assert.That(this.requestUtilCache.OfType<IterationSetup>().Count, Is.EqualTo(1));
            Assert.That(this.requestUtilCache.OfType<IterationSetup>().Single().IterationNumber, Is.EqualTo(1));
        }
    }
}