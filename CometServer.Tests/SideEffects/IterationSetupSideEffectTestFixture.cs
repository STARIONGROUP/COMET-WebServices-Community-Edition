// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IterationSetupSideEffectTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.SideEffects
{
    using CDP4Authentication;

    using CDP4Common;
    using CDP4Common.DTO;

    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CometServer.Authorization;
    using CometServer.Exceptions;

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
        private Mock<ICredentialsService> mockedCredentialsService;
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
            this.mockedCredentialsService = new Mock<ICredentialsService>();
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
            this.iterationSetupSideEffect.CredentialsService = this.mockedCredentialsService.Object;
            this.iterationSetupSideEffect.RevisionService = this.mockedRevisionService.Object;

            var returnedEngineeringModels = new List<EngineeringModel> { this.engineeringModel };
            var returnedIterations = new List<Iteration> { new(Guid.NewGuid(), 1) };
            var returnedIterationSetups = new List<IterationSetup> { new(Guid.NewGuid(), 1) };

            this.mockedIterationSetupService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), this.mockedSecurityContext.Object)).Returns(returnedIterationSetups);
            this.mockedIterationSetupService.Setup(x => x.UpdateConcept(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<Iteration>(), It.IsAny<EngineeringModelSetup>())).Returns(true);

            this.mockedIterationService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), this.mockedSecurityContext.Object)).Returns(returnedIterations);
            this.mockedIterationService.Setup(x => x.DeleteConcept(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<Iteration>(), It.IsAny<EngineeringModel>())).Returns(true);
            this.mockedIterationService.Setup(x => x.CreateConcept(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<Iteration>(), It.IsAny<EngineeringModel>(), -1)).Returns(true);

            this.mockedEngineeringModelService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).Returns(returnedEngineeringModels);
            this.mockedEngineeringModelService.Setup(x => x.AddToCollectionProperty(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<Thing>())).Returns(true);

            this.mockedTransactionManager.Setup(x => x.GetTransactionTimeAsync(It.IsAny<NpgsqlTransaction>())).Returns(Task.FromResult(DateTime.UtcNow));

            this.mockedRequestUtils.Setup(x => x.GetEngineeringModelPartitionString(It.IsAny<Guid>())).Returns("EngineeringModel");
            this.mockedCredentialsService.Setup(x => x.Credentials).Returns(new Credentials { Person = new AuthenticationPerson(new Guid(), 1) });

            this.mockedRevisionService
                .Setup(
                    x => x.SaveRevisionsAsync(
                        It.IsAny<NpgsqlTransaction>(),
                        It.IsAny<string>(),
                        It.IsAny<Guid>(),
                        It.IsAny<int>())).Returns(Task.FromResult(new List<Thing>().AsEnumerable()));
        }

        [Test]
        public void VerifyAfterCreate()
        {
            var sourceId = Guid.NewGuid();

            this.mockedIterationSetupService.Setup(x => x.GetShallow(this.npgsqlTransaction, "SiteDirectory", It.IsAny<IEnumerable<Guid>>(), this.mockedSecurityContext.Object)).
              Returns(new[] {new IterationSetup(sourceId, 0)});

            var iterationSetup = new IterationSetup(Guid.NewGuid(), 1) {SourceIterationSetup = sourceId };
            this.engineeringModelSetup.IterationSetup.Add(iterationSetup.Iid);
            var originalThing = iterationSetup.DeepClone<Thing>();

            this.iterationSetupSideEffect.AfterCreate(iterationSetup, this.engineeringModelSetup, originalThing, this.npgsqlTransaction, "siteDirectory", this.mockedSecurityContext.Object);

            // Check that the other iterationSetups get frozen when creating the iterationSetup
            this.mockedIterationSetupService.Verify(x => x.UpdateConcept(this.npgsqlTransaction, "siteDirectory", It.IsAny<IterationSetup>(), this.engineeringModelSetup), Times.Once);

            // Check that a new iteration is created triggered by the the IterationSetup creation
            this.mockedIterationService.Verify(x => x.PopulateDataFromLastIteration(this.npgsqlTransaction, It.IsAny<string>(), It.IsAny<IterationSetup>(), It.IsAny<IterationSetup>(), It.IsAny<EngineeringModel>(), this.mockedSecurityContext.Object), Times.Once);
        }

        [Test]
        public void VerifyAfterDelete()
        {
            var removeIiterationSetup = new IterationSetup(Guid.NewGuid(), 1);
            var originalThing = removeIiterationSetup.DeepClone<Thing>();

            this.iterationSetupSideEffect.AfterDelete(removeIiterationSetup, this.engineeringModelSetup, originalThing, this.npgsqlTransaction, "siteDirectory", this.mockedSecurityContext.Object);

            // Check that a new iteration is created triggered by the the IterationSetup creation
            this.mockedIterationService.Verify(x => x.DeleteConcept(this.npgsqlTransaction, It.Is<string>(s => s.Contains("EngineeringModel")), It.IsAny<Iteration>(), It.IsAny<EngineeringModel>()), Times.Never);
        }

        [Test]
        public void VerifyBeforeDeleteWhenIterationIsCurrentIteration()
        {
            var iterationSetup = this.mockedIterationSetupService.Object.GetShallow(this.npgsqlTransaction, "SiteDirectory", It.IsAny<IEnumerable<Guid>>(), this.mockedSecurityContext.Object).OfType<IterationSetup>().SingleOrDefault();

            Assert.Throws<InvalidOperationException>(() => 
                this.iterationSetupSideEffect.BeforeDelete(
                    iterationSetup,
                    this.engineeringModelSetup, 
                    this.npgsqlTransaction, 
                    "siteDirectory", 
                    this.mockedSecurityContext.Object));

            this.mockedIterationSetupService.Verify(x => x.UpdateConcept(this.npgsqlTransaction, "siteDirectory", iterationSetup , this.engineeringModelSetup), Times.Never);
        }

        [Test]
        public void VerifyBeforeDeleteWhenIterationIsFrozenAndDeleted()
        {
            var iterationSetup = this.mockedIterationSetupService.Object.GetShallow(this.npgsqlTransaction, "SiteDirectory", It.IsAny<IEnumerable<Guid>>(), this.mockedSecurityContext.Object).OfType<IterationSetup>().SingleOrDefault();
            iterationSetup.FrozenOn=DateTime.Now;
            iterationSetup.IsDeleted = true;

            var originalThing = iterationSetup.DeepClone<Thing>();

            this.iterationSetupSideEffect.BeforeDelete(
                    iterationSetup,
                    this.engineeringModelSetup,
                    this.npgsqlTransaction,
                    "siteDirectory",
                    this.mockedSecurityContext.Object);

            Assert.That(iterationSetup.Iid, Is.EqualTo(originalThing.Iid));

            this.mockedIterationSetupService.Verify(x => x.UpdateConcept(this.npgsqlTransaction, "siteDirectory", iterationSetup, this.engineeringModelSetup), Times.Never);
        }

        [Test]
        public void VerifyBeforeDeleteWhenIterationIsFrozenAndMarkItLikeIsDeleted()
        {
            var iterationSetup = this.mockedIterationSetupService.Object.GetShallow(this.npgsqlTransaction, "SiteDirectory", It.IsAny<IEnumerable<Guid>>(), this.mockedSecurityContext.Object).OfType<IterationSetup>().SingleOrDefault();
            var originalThing = iterationSetup.DeepClone<Thing>();
            iterationSetup.FrozenOn = DateTime.Now;

            this.iterationSetupSideEffect.BeforeDelete(
                iterationSetup,
                this.engineeringModelSetup,
                this.npgsqlTransaction,
                "siteDirectory",
                this.mockedSecurityContext.Object);

            Assert.That(iterationSetup.IsDeleted, Is.False);

            this.iterationSetupSideEffect.AfterDelete(
                iterationSetup,
                this.engineeringModelSetup,
                originalThing,
                this.npgsqlTransaction,
                "siteDirectory",
                this.mockedSecurityContext.Object);

            Assert.That(iterationSetup.IsDeleted, Is.True);

            this.mockedIterationSetupService.Verify(x => x.UpdateConcept(this.npgsqlTransaction, "siteDirectory", iterationSetup, this.engineeringModelSetup), Times.Once);
        }

        [Test]
        public void VerifySelfReferenceError()
        {
            var iterationSetup = this.mockedIterationSetupService.Object.GetShallow(this.npgsqlTransaction, "SiteDirectory", It.IsAny<IEnumerable<Guid>>(), this.mockedSecurityContext.Object).OfType<IterationSetup>().SingleOrDefault();

            this.engineeringModelSetup.IterationSetup.Add(iterationSetup.Iid);

            var rawUpdateInfo = new ClasslessDTO() { { "SourceIterationSetup", iterationSetup.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.iterationSetupSideEffect.BeforeUpdate(
                    iterationSetup,
                    this.engineeringModelSetup,
                    this.npgsqlTransaction,
                    "siteDirectory",
                    this.mockedSecurityContext.Object,
                    rawUpdateInfo));
        }

        [Test]
        public void VerifyOutOfModelError()
        {
            var iterationSetup = this.mockedIterationSetupService.Object.GetShallow(this.npgsqlTransaction, "SiteDirectory", It.IsAny<IEnumerable<Guid>>(), this.mockedSecurityContext.Object).OfType<IterationSetup>().SingleOrDefault();

            this.engineeringModelSetup.IterationSetup.Add(iterationSetup.Iid);

            var rawUpdateInfo = new ClasslessDTO() { { "SourceIterationSetup", Guid.NewGuid() } };

            Assert.Throws<AcyclicValidationException>(
                () => this.iterationSetupSideEffect.BeforeUpdate(
                    iterationSetup,
                    this.engineeringModelSetup,
                    this.npgsqlTransaction,
                    "siteDirectory",
                    this.mockedSecurityContext.Object,
                    rawUpdateInfo));
        }

        [Test]
        public void VerifyCircularDependencyError()
        {
            var iterationSetup1 = new IterationSetup(Guid.NewGuid(), 1);
            var iterationSetup2 = new IterationSetup(Guid.NewGuid(), 1) { SourceIterationSetup = iterationSetup1.Iid };
            var iterationSetup3 = new IterationSetup(Guid.NewGuid(), 1) { SourceIterationSetup = iterationSetup2.Iid };

            this.engineeringModelSetup.IterationSetup.Add(iterationSetup1.Iid);
            this.engineeringModelSetup.IterationSetup.Add(iterationSetup2.Iid);
            this.engineeringModelSetup.IterationSetup.Add(iterationSetup3.Iid);

            this.mockedIterationSetupService
                .Setup(x => x.Get(
                    this.npgsqlTransaction,
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    this.mockedSecurityContext.Object))
                .Returns(new[] { iterationSetup1, iterationSetup2, iterationSetup3 });

            var rawUpdateInfo = new ClasslessDTO() { { "SourceIterationSetup", iterationSetup3.Iid } };

            Assert.Throws<AcyclicValidationException>(
                () => this.iterationSetupSideEffect.BeforeUpdate(
                    iterationSetup1,
                    this.engineeringModelSetup,
                    this.npgsqlTransaction,
                    "siteDirectory",
                    this.mockedSecurityContext.Object,
                    rawUpdateInfo));
        }
    }
}
