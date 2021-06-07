// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OptionSideEffectTestFixture.cs" company="RHEA System S.A.">
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

    using CDP4Common.DTO;

    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="OptionSideEffect"/>
    /// </summary>
    [TestFixture]
    public class OptionSideEffectTestFixture
    {
        private OptionSideEffect optionSideEffect;

        private Mock<IOptionService> optionService;

        private Mock<IEngineeringModelSetupService> engineeringModelSetupService;

        private Mock<IEngineeringModelService> engineeringModelService;

        private Mock<IIterationService> iterationService;

        private Mock<IIterationSetupService> iterationSetupService;

        private Mock<ISecurityContext> securityContext;

        private Mock<IRequestUtils> requestUtils;

        private Iteration iteration;

        private IterationSetup iterationSetup;

        private Iteration updatedIteration;

        private Option option1;

        private Option option2;

        private List<Option> options;

        private EngineeringModelSetup engineeringModelSetup;

        private EngineeringModel engineeringModel;

        private NpgsqlTransaction npgsqlTransaction;

        private readonly string iterationPartition = $"{CDP4Orm.Dao.Utils.IterationSubPartition}_partition";

        private readonly string engineeringModelPartition = $"{CDP4Orm.Dao.Utils.EngineeringModelPartition}_partition";

        [SetUp]
        public void SetUp()
        {
            this.optionService = new Mock<IOptionService>();
            this.engineeringModelSetupService = new Mock<IEngineeringModelSetupService>();
            this.iterationService = new Mock<IIterationService>();
            this.iterationSetupService = new Mock<IIterationSetupService>();
            this.securityContext = new Mock<ISecurityContext>();
            this.engineeringModelService = new Mock<IEngineeringModelService>();
            this.requestUtils = new Mock<IRequestUtils>();
            this.npgsqlTransaction = null;

            this.option1 = new Option(Guid.NewGuid(), 0);
            this.option2 = new Option(Guid.NewGuid(), 0);
            this.options = new List<Option>();
            this.optionService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(),
                It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).Returns(options);

            this.iterationSetup = new IterationSetup
            {
                Iid = Guid.NewGuid()
            };

            this.iteration = new Iteration
            {
                Iid = Guid.NewGuid(),
                IterationSetup = this.iterationSetup.Iid
            };

            this.iteration.DefaultOption = this.option1.Iid;

            this.iterationSetup.IterationIid = this.iteration.Iid;
            
            this.updatedIteration = this.iteration.DeepClone<Iteration>();

            this.engineeringModelSetup = new EngineeringModelSetup
            {
                Iid = Guid.NewGuid(),
                IterationSetup = new List<Guid> { this.iterationSetup.Iid },
            };

            this.engineeringModel = new EngineeringModel
            {
                Iid = Guid.NewGuid(),
                EngineeringModelSetup = this.engineeringModelSetup.Iid
            };
            this.engineeringModelSetup.EngineeringModelIid = this.engineeringModel.Iid;

            this.optionSideEffect = new OptionSideEffect
            {
                OptionService = this.optionService.Object,
                EngineeringModelSetupService = this.engineeringModelSetupService.Object,
                EngineeringModelService = this.engineeringModelService.Object,
                IterationService = this.iterationService.Object,
                IterationSetupService = this.iterationSetupService.Object,
                RequestUtils = this.requestUtils.Object
            };
        }

        [Test]
        public void Verify_that_when_an_iteration_contains_no_options_an_option_can_be_added()
        {
            Assert.That(this.optionSideEffect.BeforeCreate(this.option1, this.iteration, null, null, null),
                Is.True);
        }

        [Test]
        public void Verify_that_after_delete_option_everything_an_EngineeringModel_is_a_catalogue_no_more_than_one_option_can_be_added()
        {
            this.engineeringModelSetup.Kind= CDP4Common.SiteDirectoryData.EngineeringModelKind.MODEL_CATALOGUE;
            this.options.Add(this.option1);
            this.options.Add(this.option2);

            this.SetupMethodCallsForDeleteOptionTest();

            Assert.Throws<InvalidOperationException>(() =>
                this.optionSideEffect.BeforeCreate(
                    this.option1,
                    this.iteration,
                    this.npgsqlTransaction,
                    this.iterationPartition,
                    this.securityContext.Object));
        }

        [Test]
        public void Verify_that_when_an_EngineeringModel_is_not_a_Catalogue_more_than_one_option_can_be_added()
        {
            this.engineeringModelSetup.Kind = CDP4Common.SiteDirectoryData.EngineeringModelKind.STUDY_MODEL;
            this.options.Add(this.option1);
            this.options.Add(this.option2);

            this.SetupMethodCallsForDeleteOptionTest();

            Assert.That(
                this.optionSideEffect.BeforeCreate(
                    this.option1,
                    this.iteration,
                    this.npgsqlTransaction,
                    this.iterationPartition,
                    this.securityContext.Object), Is.True);
        }

        [Test]
        public void Verify_that_DefaultOption_is_not_set_and_not_saved_when_Option_is_deleted()
        {
            this.SetupMethodCallsForDeleteOptionTest();

            this.optionSideEffect.AfterDelete(
                this.option1,
                this.iteration,
                this.option1,
                this.npgsqlTransaction,
                this.iterationPartition,
                this.securityContext.Object);

            Assert.IsNotNull(this.iteration.DefaultOption);
            Assert.IsNull(this.updatedIteration.DefaultOption);

            this.iterationService
                .Verify(
                    x => x.UpdateConcept(this.npgsqlTransaction, this.engineeringModelPartition, this.updatedIteration,
                        this.engineeringModel), Times.Once);
        }

        [Test]
        public void Verify_that_DefaultOption_is_set_and_saved_when_Option_is_deleted()
        {
            this.SetupMethodCallsForDeleteOptionTest();

            this.optionSideEffect.AfterDelete(
                this.option2,
                this.iteration,
                this.option2,
                this.npgsqlTransaction,
                this.iterationPartition,
                this.securityContext.Object);

            Assert.IsNotNull(this.updatedIteration.DefaultOption);
            this.iterationService
                .Verify(
                    x => x.UpdateConcept(this.npgsqlTransaction, this.engineeringModelPartition, this.updatedIteration,
                        this.engineeringModel), Times.Never);
        }

        [Test]
        public void Verify_that_DefaultOption_is_not_set_and_not_saved_when_DefaultOption_is_deleted_and_DefautOption_was_already_reset_earlier()
        {
            this.SetupMethodCallsForDeleteOptionTest();

            this.updatedIteration.DefaultOption = null;

            this.optionSideEffect.AfterDelete(
                this.option1,
                this.iteration,
                this.option1,
                this.npgsqlTransaction,
                this.iterationPartition,
                this.securityContext.Object);

            Assert.IsNotNull(this.iteration.DefaultOption);
            this.iterationService
                .Verify(
                    x => x.UpdateConcept(this.npgsqlTransaction, this.engineeringModelPartition, this.updatedIteration,
                        this.engineeringModel), Times.Never);
        }

        /// <summary>
        ///  Sets up fake method calls on mocked classes specifically for the unit tests that check
        ///  <see cref="Iteration.DefaultOption" /> handling when an <see cref="Option" /> is deleted.
        /// </summary>
        private void SetupMethodCallsForDeleteOptionTest()
        {
            this.requestUtils
                .Setup(x => x.GetEngineeringModelPartitionString(this.engineeringModelSetup.EngineeringModelIid))
                .Returns(this.engineeringModelPartition);

            var iterationSetups = new List<IterationSetup>();
            iterationSetups.Add(this.iterationSetup);
            
            this.iterationSetupService
                .Setup(x => x.GetShallow(this.npgsqlTransaction, CDP4Orm.Dao.Utils.SiteDirectoryPartition,
                    new[] { this.iteration.IterationSetup }, this.securityContext.Object))
                .Returns(iterationSetups);

            var engineeringModelSetups = new List<EngineeringModelSetup>(); 
            engineeringModelSetups.Add(this.engineeringModelSetup);
            
            this.engineeringModelSetupService
                .Setup(x => x.GetShallow(this.npgsqlTransaction, CDP4Orm.Dao.Utils.SiteDirectoryPartition,
                    null, this.securityContext.Object))
                .Returns(engineeringModelSetups);

            var newIterations = new List<Iteration>();
            newIterations.Add(this.updatedIteration);

            this.iterationService
                .Setup(x => x.GetShallow(this.npgsqlTransaction, this.engineeringModelPartition,
                    new[] { this.iteration.Iid }, this.securityContext.Object))
                .Returns(newIterations);

            var engineeringModels = new List<EngineeringModel>();
            engineeringModels.Add(this.engineeringModel);

            this.engineeringModelService
                .Setup(x => x.GetShallow(this.npgsqlTransaction, this.engineeringModelPartition,
                    new[] { this.engineeringModelSetup.EngineeringModelIid }, this.securityContext.Object))
                .Returns(engineeringModels);
        }
    }
}