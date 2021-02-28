// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelCreatorManagerTestFixture.cs" company="RHEA System S.A.">
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

namespace CometServer.Tests.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.MetaInfo;

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

        [SetUp]
        public void Setup()
        {
            this.RequestUtils = new Mock<IRequestUtils>();
            this.MetainfoProvider = new Mock<IMetaInfoProvider>();
            this.modelSetupService = new Mock<IEngineeringModelSetupService>();

            this.modelCreatorManager = new ModelCreatorManager()
            {
                RequestUtils = this.RequestUtils.Object,
                MetainfoProvider = this.MetainfoProvider.Object,
                EngineeringModelSetupService = this.modelSetupService.Object
            };

            this.RequestUtils.Setup(x => x.QueryParameters).Returns(new QueryParameters());
            this.RequestUtils.Setup(x => x.Cache).Returns(new List<Thing>());

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

            var iterationSetup = new IterationSetup(Guid.NewGuid(), 0);

            participant.Domain.Add(Guid.NewGuid());
            mrdl.DefinedCategory.Add(cat.Iid);
            mrdl.RequiredRdl = Guid.NewGuid();
            modelSetup.RequiredRdl.Add(mrdl.Iid);
            modelSetup.IterationSetup.Add(iterationSetup.Iid);
            modelSetup.Participant.Add(participant.Iid);

            this.modelSetupService.Setup(x => x.GetDeep(It.IsAny<NpgsqlTransaction>(), "SiteDirectory", It.Is<IEnumerable<Guid>>(y => y.Contains(modelSetup.Iid)), It.IsAny<ISecurityContext>())).
                Returns(new Thing[] { modelSetup, mrdl, cat, participant, iterationSetup });

            var copy = new EngineeringModelSetup(Guid.NewGuid(), 0);
            this.modelCreatorManager.CreateEngineeringModelSetupFromSource(modelSetup.Iid, copy, null, null);

            Assert.IsNotEmpty(copy.RequiredRdl);
            Assert.IsNotEmpty(copy.IterationSetup);
            Assert.IsNotEmpty(copy.Participant);
        }
    }
}