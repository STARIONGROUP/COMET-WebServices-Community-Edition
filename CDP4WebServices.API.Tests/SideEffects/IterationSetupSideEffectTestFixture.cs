// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationSetupSideEffectTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// <summary>
//   IterationSetup Side Effect test class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.SideEffects
{
    using System;
    using System.Collections.Generic;

    using CDP4Authentication;

    using CDP4Common.DTO;

    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authentication;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Operations.SideEffects;
    using Moq;
    using Npgsql;
    using NUnit.Framework;

    [TestFixture]
    internal class IterationSetupSideEffectTestFixture
    {
        private IterationSetupSideEffect iterationSetupSideEffect;
        private EngineeringModel engineeringModel;
        private EngineeringModelSetup engineeringModelSetup;
        private NpgsqlTransaction npgsqlTransaction;
        private Mock<IIterationService> mockedIterationService;
        private Mock<IRequestUtils> mockedRequestUtils;
        private Mock<IIterationSetupService> mockedIterationSetupService;
        private Mock<IEngineeringModelService> mockedEngineeringModelService;
        private Mock<ISecurityContext> mockedSecurityContext;
        private Mock<ICdp4TransactionManager> mockedTransactionManager;
        private Mock<ICdp4RequestContext> mockedRequestContext;
        private Mock<IPersonResolver> mockedPersonResolver;
        private Mock<IRevisionService> mockedRevisionService;

        [SetUp]
        public void Setup()
        {
            this.mockedIterationService = new Mock<IIterationService>();
            this.mockedIterationSetupService = new Mock<IIterationSetupService>();
            this.mockedEngineeringModelService = new Mock<IEngineeringModelService>();
            this.mockedSecurityContext = new Mock<ISecurityContext>();
            this.mockedTransactionManager = new Mock<ICdp4TransactionManager>();
            this.mockedRequestUtils = new Mock<IRequestUtils>();
            this.mockedRequestContext = new Mock<ICdp4RequestContext>();
            this.mockedPersonResolver = new Mock<IPersonResolver>();
            this.mockedRevisionService = new Mock<IRevisionService>();

            this.engineeringModel = new EngineeringModel(Guid.NewGuid(), 1);
            this.iterationSetupSideEffect = new IterationSetupSideEffect();
            this.npgsqlTransaction = null;
            this.engineeringModelSetup = new EngineeringModelSetup(Guid.NewGuid(), 1);
            this.engineeringModel.EngineeringModelSetup = this.engineeringModelSetup.Iid;
            this.iterationSetupSideEffect.RequestUtils = this.mockedRequestUtils.Object;
            this.iterationSetupSideEffect.EngineeringModelService = this.mockedEngineeringModelService.Object;
            this.iterationSetupSideEffect.IterationService = this.mockedIterationService.Object;
            this.iterationSetupSideEffect.IterationSetupService = this.mockedIterationSetupService.Object;
            this.iterationSetupSideEffect.TransactionManager = this.mockedTransactionManager.Object;
            this.iterationSetupSideEffect.PermissionService = new PermissionService();
            this.iterationSetupSideEffect.PersonResolver = this.mockedPersonResolver.Object;
            this.iterationSetupSideEffect.RevisionService = this.mockedRevisionService.Object;

            var returnedEngineeringModels = new List<EngineeringModel> { this.engineeringModel };
            var returnedIterations = new List<Iteration> { new Iteration(Guid.NewGuid(), 1) };
            var returnedIterationSetups = new List<IterationSetup> { new IterationSetup(Guid.NewGuid(), 1) };

            this.mockedIterationSetupService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), this.mockedSecurityContext.Object)).Returns(returnedIterationSetups);
            this.mockedIterationSetupService.Setup(x => x.UpdateConcept(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<Iteration>(), It.IsAny<EngineeringModelSetup>())).Returns(true);

            this.mockedIterationService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), this.mockedSecurityContext.Object)).Returns(returnedIterations);
            this.mockedIterationService.Setup(x => x.DeleteConcept(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<Iteration>(), It.IsAny<EngineeringModel>())).Returns(true);
            this.mockedIterationService.Setup(x => x.CreateConcept(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<Iteration>(), It.IsAny<EngineeringModel>(), -1)).Returns(true);

            this.mockedEngineeringModelService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).Returns(returnedEngineeringModels);
            this.mockedEngineeringModelService.Setup(x => x.AddToCollectionProperty(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Thing>())).Returns(true);

            this.mockedTransactionManager.Setup(x => x.GetTransactionTime(It.IsAny<NpgsqlTransaction>())).Returns(DateTime.UtcNow);

            this.mockedRequestContext.Setup(x => x.AuthenticatedCredentials).Returns(new Credentials());

            this.mockedPersonResolver.Setup(x => x.ResolveParticipantCredentials(It.IsAny<NpgsqlTransaction>(), It.IsAny<Credentials>()));

            this.mockedRequestUtils.Setup(x => x.Context).Returns(this.mockedRequestContext.Object);
            this.mockedRequestUtils.Setup(x => x.GetEngineeringModelPartitionString(It.IsAny<Guid>())).Returns("EngineeringModel");
            this.mockedRequestUtils.Setup(x => x.Context.AuthenticatedCredentials).Returns(new Credentials { Person = new AuthenticationPerson(new Guid(), 1) });

            this.mockedRevisionService
                .Setup(
                    x => x.SaveRevisions(
                        It.IsAny<NpgsqlTransaction>(),
                        It.IsAny<string>(),
                        It.IsAny<Guid>(),
                        It.IsAny<int>())).Returns(new List<Thing>());
        }

        [Test]
        public void VerifyAfterCreate()
        {
            var iterationSetup = new IterationSetup(Guid.NewGuid(), 1);
            this.engineeringModelSetup.IterationSetup.Add(iterationSetup.Iid);
            var originalThing = iterationSetup.DeepClone<Thing>();

            this.iterationSetupSideEffect.AfterCreate(iterationSetup, this.engineeringModelSetup, originalThing, this.npgsqlTransaction, "siteDirectory", this.mockedSecurityContext.Object);

            // Check that the other iterationSetups get frozen when creating the iterationSetup
            this.mockedIterationSetupService.Verify(x => x.UpdateConcept(this.npgsqlTransaction, "siteDirectory", It.IsAny<IterationSetup>(), this.engineeringModelSetup), Times.Once);

            // Check that a new iteration is created triggered by the the IterationSetup creation
            this.mockedIterationService.Verify(x => x.CreateConcept(this.npgsqlTransaction, "EngineeringModel", It.IsAny<Iteration>(), It.IsAny<EngineeringModel>(), -1), Times.Once);
        }

        [Test]
        public void VerifyAfterDelete()
        {
            var removeIiterationSetup = new IterationSetup(Guid.NewGuid(), 1);
            var originalThing = removeIiterationSetup.DeepClone<Thing>();

            this.iterationSetupSideEffect.AfterDelete(removeIiterationSetup, this.engineeringModelSetup, originalThing, this.npgsqlTransaction, "siteDirectory", this.mockedSecurityContext.Object);

            // Check that a new iteration is created triggered by the the IterationSetup creation
            this.mockedIterationService.Verify(x => x.DeleteConcept(this.npgsqlTransaction, It.Is<string>(s => s.Contains("EngineeringModel")), It.IsAny<Iteration>(), It.IsAny<EngineeringModel>()), Times.Once);
        }
    }
}
