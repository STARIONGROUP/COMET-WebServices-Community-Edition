// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementDefinitionSideEffectTestFixture.cs" company="RHEA System S.A.">
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

    using CDP4Common;
    using CDP4Common.DTO;

    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    using Utils = CDP4Orm.Dao.Utils;

    /// <summary>
    /// Suite of tests for the <see cref="ElementDefinitionSideEffect" />
    /// </summary>
    [TestFixture]
    public class ElementDefinitionSideEffectTestFixture
    {
        private NpgsqlTransaction npgsqlTransaction;

        private Mock<ISecurityContext> securityContext;
        private Mock<IElementDefinitionService> elementDefinitionService;
        private Mock<IElementUsageService> elementUsageService;
        private Mock<IIterationService> iterationService;
        private Mock<IIterationSetupService> iterationSetupService;
        private Mock<IEngineeringModelSetupService> engineeringModelSetupService;
        private Mock<IEngineeringModelService> engineeringModelService;
        private Mock<IRequestUtils> requestUtils;

        private ElementDefinitionSideEffect sideEffect;

        private ElementDefinition edA;
        private ElementDefinition edB;
        private ElementDefinition edC;
        private ElementDefinition edD;

        private ElementUsage euA;
        private ElementUsage euB;
        private ElementUsage euC;
        private ElementUsage euD;
        private ElementUsage euE;

        private Iteration iteration;
        private Iteration updatedIteration;
        private IterationSetup iterationSetup;
        private EngineeringModelSetup engineeringModelSetup;
        private EngineeringModel engineeringModel;

        private ClasslessDTO rawUpdateInfo;

        private readonly string iterationPartition = $"{Utils.IterationSubPartition}_partition";
        private readonly string engineeringModelPartition = $"{Utils.EngineeringModelPartition}_partition";
        private const string TestKey = "ContainedElement";

        [SetUp]
        public void Setup()
        {
            this.npgsqlTransaction = null;
            this.securityContext = new Mock<ISecurityContext>();
            this.iterationService = new Mock<IIterationService>();
            this.iterationSetupService = new Mock<IIterationSetupService>();
            this.engineeringModelSetupService = new Mock<IEngineeringModelSetupService>();
            this.engineeringModelService = new Mock<IEngineeringModelService>();
            this.requestUtils = new Mock<IRequestUtils>();

            ////                    edA
            ////                   /    \
            ////                 euA    euB
            ////                 /         \
            ////              edB           edC
            ////                           /   \
            ////                         euC    euD   euE will be added. euE-edD does not throw an exception, but euD-edA does throw
            ////                        /         \     \
            ////                      edD          edA   edD
            this.edD = new ElementDefinition { Iid = Guid.NewGuid() };
            this.euE = new ElementUsage { Iid = Guid.NewGuid(), ElementDefinition = this.edD.Iid };

            this.euC = new ElementUsage { Iid = Guid.NewGuid(), ElementDefinition = this.edD.Iid };

            this.edC = new ElementDefinition
            {
                Iid = Guid.NewGuid(),
                ContainedElement = new List<Guid> { this.euC.Iid }
            };
            this.euB = new ElementUsage { Iid = Guid.NewGuid(), ElementDefinition = this.edC.Iid };

            this.edB = new ElementDefinition { Iid = Guid.NewGuid() };
            this.euA = new ElementUsage { Iid = Guid.NewGuid(), ElementDefinition = this.edB.Iid };

            this.edA = new ElementDefinition
            {
                Iid = Guid.NewGuid(),
                ContainedElement = new List<Guid> { this.euA.Iid, this.euB.Iid }
            };
            this.euD = new ElementUsage { Iid = Guid.NewGuid(), ElementDefinition = this.edA.Iid };

            this.elementDefinitionService = new Mock<IElementDefinitionService>();
            this.elementDefinitionService
                .Setup(x => x.Get(this.npgsqlTransaction, It.IsAny<string>(), null, It.IsAny<ISecurityContext>()))
                .Returns(new List<Thing> { this.edA, this.edB, this.edC, this.edD });

            this.elementUsageService = new Mock<IElementUsageService>();
            this.elementUsageService
                .Setup(x => x.Get(this.npgsqlTransaction, It.IsAny<string>(), null, It.IsAny<ISecurityContext>()))
                .Returns(new List<Thing> { this.euA, this.euB, this.euC, this.euD, this.euE });

            this.iterationSetup = new IterationSetup
            {
                Iid = Guid.NewGuid()
            };

            this.iteration = new Iteration
            {
                Iid = Guid.NewGuid(),
                Element = new List<Guid>
                {
                    this.edA.Iid,
                    this.edB.Iid,
                    this.edC.Iid,
                    this.edD.Iid
                },
                TopElement = this.edA.Iid,
                IterationSetup = this.iterationSetup.Iid
            };
            this.updatedIteration = this.iteration.DeepClone<Iteration>();

            this.iterationSetup.IterationIid = this.iteration.Iid;

            this.engineeringModelSetup = new EngineeringModelSetup
            {
                Iid = Guid.NewGuid(),
                IterationSetup = new List<Guid> { this.iterationSetup.Iid }
            };

            this.engineeringModel = new EngineeringModel
            {
                Iid = Guid.NewGuid(),
                EngineeringModelSetup = this.engineeringModelSetup.Iid
            };
            this.engineeringModelSetup.EngineeringModelIid = this.engineeringModel.Iid;

            this.sideEffect = new ElementDefinitionSideEffect
            {
                ElementDefinitionService =
                    this.elementDefinitionService.Object,
                ElementUsageService =
                    this.elementUsageService.Object,
                IterationService = this.iterationService.Object,
                IterationSetupService = this.iterationSetupService.Object,
                EngineeringModelSetupService = this.engineeringModelSetupService.Object,
                EngineeringModelService = this.engineeringModelService.Object,
                RequestUtils = this.requestUtils.Object
            };
        }

        [Test]
        public void VerifyThatExceptionIsNotThrownWhenElementUsageDoesNotLeadToCircularDependency()
        {
            this.rawUpdateInfo = new ClasslessDTO { { TestKey, new List<Guid> { this.euE.Iid } } };

            Assert.DoesNotThrow(
                () => this.sideEffect.BeforeUpdate(
                    this.edC,
                    this.iteration,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenElementUsageLeadsToCircularDependency()
        {
            this.rawUpdateInfo = new ClasslessDTO { { TestKey, new List<Guid> { this.euD.Iid } } };

            Assert.Throws<AcyclicValidationException>(
                () => this.sideEffect.BeforeUpdate(
                    this.edC,
                    this.iteration,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object,
                    this.rawUpdateInfo));
        }

        [Test]
        public void VerifyThatResetOfTopElementIsNotSetAndNotSavedWhenNonTopElementIsDeleted()
        {
            this.SetupMethodCallsForTopElementTest(SetupMethodCallsForTopElementTestScenario.All);

            this.sideEffect.BeforeDelete(
                this.edD,
                this.iteration,
                this.npgsqlTransaction,
                this.iterationPartition,
                this.securityContext.Object);

            Assert.IsNotNull(this.iteration.TopElement);
            this.iterationService
                .Verify(
                    x => x.UpdateConcept(this.npgsqlTransaction, this.engineeringModelPartition, this.updatedIteration,
                        this.engineeringModel), Times.Never);
        }

        [Test]
        public void
            VerifyThatResetOfTopElementIsNotSetAndNotSavedWhenTopElementIsDeletedAndTopElementWasAlreadyResetAtAnEarlierStage()
        {
            this.SetupMethodCallsForTopElementTest(SetupMethodCallsForTopElementTestScenario.All);
            this.updatedIteration.TopElement = null;

            this.sideEffect.BeforeDelete(
                this.edA,
                this.iteration,
                this.npgsqlTransaction,
                this.iterationPartition,
                this.securityContext.Object);

            Assert.IsNotNull(this.iteration.TopElement);
            this.iterationService
                .Verify(
                    x => x.UpdateConcept(this.npgsqlTransaction, this.engineeringModelPartition, this.updatedIteration,
                        this.engineeringModel), Times.Never);
        }

        [Test]
        public void VerifyThatResetOfTopElementIsSetAndSavedWhenTopElementIsDeleted()
        {
            this.SetupMethodCallsForTopElementTest(SetupMethodCallsForTopElementTestScenario.All);

            this.sideEffect.BeforeDelete(
                this.edA,
                this.iteration,
                this.npgsqlTransaction,
                this.iterationPartition,
                this.securityContext.Object);

            Assert.IsNull(this.updatedIteration.TopElement);
            this.iterationService
                .Verify(
                    x => x.UpdateConcept(this.npgsqlTransaction, this.engineeringModelPartition, this.updatedIteration,
                        this.engineeringModel), Times.Once);
        }

        [Test]
        public void VerifyThatResetOfTopElementThrowsErrorWhenTopElementIsDeletedAndContainerIsNull()
        {
            this.SetupMethodCallsForTopElementTest(SetupMethodCallsForTopElementTestScenario.All);

            Assert.Throws<ArgumentNullException>(() =>
                this.sideEffect.BeforeDelete(
                    this.edA,
                    null,
                    this.npgsqlTransaction,
                    this.iterationPartition,
                    this.securityContext.Object)
            );

            this.iterationService
                .Verify(
                    x => x.UpdateConcept(this.npgsqlTransaction, this.engineeringModelPartition, this.updatedIteration,
                        this.engineeringModel), Times.Never);
        }

        [Test]
        public void VerifyThatResetOfTopElementThrowsErrorWhenTopElementIsDeletedAndContainerIsOfWrongType()
        {
            this.SetupMethodCallsForTopElementTest(SetupMethodCallsForTopElementTestScenario.All);

            Assert.Throws<ArgumentException>(() =>
                this.sideEffect.BeforeDelete(
                    this.edA,
                    this.edB,
                    this.npgsqlTransaction,
                    this.iterationPartition,
                    this.securityContext.Object)
            );

            this.iterationService
                .Verify(
                    x => x.UpdateConcept(this.npgsqlTransaction, this.engineeringModelPartition, this.updatedIteration,
                        this.engineeringModel), Times.Never);
        }

        [Test]
        public void VerifyThatResetOfTopElementThrowsErrorWhenTopElementIsDeletedForUnknownEngineeringModel()
        {
            this.SetupMethodCallsForTopElementTest(SetupMethodCallsForTopElementTestScenario.EngineeringModelNotFound);

            Assert.Throws<KeyNotFoundException>(() =>
                this.sideEffect.BeforeDelete(
                    this.edA,
                    this.iteration,
                    this.npgsqlTransaction,
                    this.iterationPartition,
                    this.securityContext.Object)
            );

            Assert.IsNull(this.updatedIteration.TopElement);
            this.iterationService
                .Verify(
                    x => x.UpdateConcept(this.npgsqlTransaction, this.engineeringModelPartition, this.updatedIteration,
                        this.engineeringModel), Times.Never);
        }

        [Test]
        public void VerifyThatResetOfTopElementThrowsErrorWhenTopElementIsDeletedForUnknownEngineeringModelSetup()
        {
            this.SetupMethodCallsForTopElementTest(SetupMethodCallsForTopElementTestScenario
                .EngineeringModelSetupNotFound);

            Assert.Throws<KeyNotFoundException>(() =>
                this.sideEffect.BeforeDelete(
                    this.edA,
                    this.iteration,
                    this.npgsqlTransaction,
                    this.iterationPartition,
                    this.securityContext.Object)
            );
            this.iterationService
                .Verify(
                    x => x.UpdateConcept(this.npgsqlTransaction, this.engineeringModelPartition, this.updatedIteration,
                        this.engineeringModel), Times.Never);
        }

        [Test]
        public void VerifyThatResetOfTopElementThrowsErrorWhenTopElementIsDeletedForUnknownIteration()
        {
            this.SetupMethodCallsForTopElementTest(SetupMethodCallsForTopElementTestScenario.IterationNotFound);

            Assert.Throws<KeyNotFoundException>(() =>
                this.sideEffect.BeforeDelete(
                    this.edA,
                    this.iteration,
                    this.npgsqlTransaction,
                    this.iterationPartition,
                    this.securityContext.Object)
            );

            this.iterationService
                .Verify(
                    x => x.UpdateConcept(this.npgsqlTransaction, this.engineeringModelPartition, this.updatedIteration,
                        this.engineeringModel), Times.Never);
        }

        [Test]
        public void VerifyThatResetOfTopElementThrowsErrorWhenTopElementIsDeletedForUnknownIterationSetup()
        {
            this.SetupMethodCallsForTopElementTest(SetupMethodCallsForTopElementTestScenario.IterationSetupNotFound);

            Assert.Throws<KeyNotFoundException>(() =>
                this.sideEffect.BeforeDelete(
                    this.edA,
                    this.iteration,
                    this.npgsqlTransaction,
                    this.iterationPartition,
                    this.securityContext.Object)
            );

            this.iterationService
                .Verify(
                    x => x.UpdateConcept(this.npgsqlTransaction, this.engineeringModelPartition, this.updatedIteration,
                        this.engineeringModel), Times.Never);
        }

        /// <summary>
        ///  Descriptive enum to present the <see cref="SetupMethodCallsForTopElementTest" /> method a self-descriptive value on
        ///  how fake method calls should be setup.
        ///  Uses Flags attribute for convenience
        ///  <See cref="https://docs.microsoft.com/en-us/dotnet/api/system.flagsattribute" />
        /// </summary>
        [Flags]
        private enum SetupMethodCallsForTopElementTestScenario
        {
            IterationSetup = 1,
            EngineeringModelSetup = 2,
            Iteration = 4,
            EngineeringModel = 8,
            IterationSetupNotFound = EngineeringModelSetup | Iteration | EngineeringModel,
            EngineeringModelSetupNotFound = IterationSetup | Iteration | EngineeringModel,
            IterationNotFound = IterationSetup | EngineeringModelSetup | EngineeringModel,
            EngineeringModelNotFound = IterationSetup | EngineeringModelSetup | Iteration,
            All = IterationSetup | EngineeringModelSetup | Iteration | EngineeringModel
        }

        /// <summary>
        ///  Sets up fake method calls on mocked classes specifically for the unit tests that check
        ///  <see cref="Iteration.TopElement" /> handling when an <see cref="ElementDefinition" /> is deleted.
        /// </summary>
        /// <param name="setupMethodCallsForTopElementTestScenario">
        ///  The <see cref="SetupMethodCallsForTopElementTestScenario" />
        ///  When an enum value flag is not set then the corresponding faked method call will NOT return any data.
        /// </param>
        private void SetupMethodCallsForTopElementTest(
            SetupMethodCallsForTopElementTestScenario setupMethodCallsForTopElementTestScenario)
        {
            this.requestUtils
                .Setup(x => x.GetEngineeringModelPartitionString(this.engineeringModelSetup.EngineeringModelIid))
                .Returns(this.engineeringModelPartition);

            var iterationSetups = new List<IterationSetup>();
            if (setupMethodCallsForTopElementTestScenario.HasFlag(SetupMethodCallsForTopElementTestScenario
                .IterationSetup))
            {
                iterationSetups.Add(this.iterationSetup);
            }

            this.iterationSetupService
                .Setup(x => x.GetShallow(this.npgsqlTransaction, Utils.SiteDirectoryPartition,
                    new[] { this.iteration.IterationSetup }, this.securityContext.Object))
                .Returns(iterationSetups);

            var engineeringModelSetups = new List<EngineeringModelSetup>();
            if (setupMethodCallsForTopElementTestScenario.HasFlag(SetupMethodCallsForTopElementTestScenario
                .EngineeringModelSetup))
            {
                engineeringModelSetups.Add(this.engineeringModelSetup);
            }

            this.engineeringModelSetupService
                .Setup(x => x.GetShallow(this.npgsqlTransaction, Utils.SiteDirectoryPartition,
                    null, this.securityContext.Object))
                .Returns(engineeringModelSetups);

            var newIterations = new List<Iteration>();
            if (setupMethodCallsForTopElementTestScenario.HasFlag(SetupMethodCallsForTopElementTestScenario.Iteration))
            {
                newIterations.Add(this.updatedIteration);
            }

            this.iterationService
                .Setup(x => x.GetShallow(this.npgsqlTransaction, this.engineeringModelPartition,
                    new[] { this.iteration.Iid }, this.securityContext.Object))
                .Returns(newIterations);

            var engineeringModels = new List<EngineeringModel>();

            if (setupMethodCallsForTopElementTestScenario.HasFlag(SetupMethodCallsForTopElementTestScenario
                .EngineeringModel))
            {
                engineeringModels.Add(this.engineeringModel);
            }

            this.engineeringModelService
                .Setup(x => x.GetShallow(this.npgsqlTransaction, this.engineeringModelPartition,
                    new[] { this.engineeringModelSetup.EngineeringModelIid }, this.securityContext.Object))
                .Returns(engineeringModels);
        }
    }
}