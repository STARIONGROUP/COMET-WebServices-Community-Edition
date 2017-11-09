// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelCreatorManagerTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// <summary>
//   This the model creator manager test class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using API.Services;
    using API.Services.Authorization;
    using API.Services.Operations.SideEffects;
    using API.Services.Protocol;
    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.MetaInfo;
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