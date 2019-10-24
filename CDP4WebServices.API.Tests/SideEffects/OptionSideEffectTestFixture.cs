// <copyright file="OptionSideEffectTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.SideEffects
{
    using System;
    using System.Collections.Generic;
    using CDP4Common.DTO;
    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Operations.SideEffects;
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

        private Mock<IIterationSetupService> iterationSetupService;

        private Iteration iteration;

        private Option option;

        private List<Option> options;

        [SetUp]
        public void SetUp()
        {
            this.optionService = new Mock<IOptionService>();
            this.engineeringModelSetupService = new Mock<IEngineeringModelSetupService>();
            this.iterationSetupService = new Mock<IIterationSetupService>();

            this.optionSideEffect = new OptionSideEffect
            {
                OptionService = this.optionService.Object,
                EngineeringModelSetupService = this.engineeringModelSetupService.Object,
                IterationSetupService = this.iterationSetupService.Object
            };

            this.iteration = new Iteration(Guid.NewGuid(), 0);
            iteration.IterationSetup = Guid.NewGuid();
            this.option = new Option(Guid.NewGuid(), 0);

            this.options = new List<Option>();
            this.optionService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(),
                It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).Returns(options);
        }

        [Test]
        public void Verify_that_when_an_iteration_contains_no_options_an_option_can_be_added()
        {
            Assert.That(this.optionSideEffect.BeforeCreate(this.option, this.iteration, null, null, null),
                Is.True);
        }

        [Test]
        public void Verify_that_when_an_EngineeringModel_is_a_catalogue_no_more_than_one_option_can_be_added()
        {
            this.options.Add(new Option(Guid.NewGuid(), 0));

            var iterationSetup = new IterationSetup(Guid.NewGuid(), 0) {IterationIid = this.iteration.Iid};

            this.iterationSetupService.Setup(x =>
                x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<ISecurityContext>())).Returns(new List<IterationSetup>() {iterationSetup});
            
            var engineeringModelSetup = new EngineeringModelSetup(Guid.NewGuid(), 0);
            engineeringModelSetup.IterationSetup.Add(iterationSetup.Iid);
            engineeringModelSetup.Kind = CDP4Common.SiteDirectoryData.EngineeringModelKind.MODEL_CATALOGUE;

            this.engineeringModelSetupService.Setup(x =>
                x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<ISecurityContext>())).Returns(new List<EngineeringModelSetup>() {engineeringModelSetup});

            Assert.Throws<InvalidOperationException>(() =>
                this.optionSideEffect.BeforeCreate(this.option, this.iteration, null, null, null));
        }

        [Test]
        public void Verify_that_when_an_EngineeringModel_is_not_a_Catalogue_more_than_one_option_can_be_added()
        {
            this.options.Add(new Option(Guid.NewGuid(), 0));
            
            var iterationSetup = new IterationSetup(Guid.NewGuid(), 0) { IterationIid = this.iteration.Iid };

            this.iterationSetupService.Setup(x =>
                x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<ISecurityContext>())).Returns(new List<IterationSetup>() { iterationSetup });

            var engineeringModelSetup = new EngineeringModelSetup(Guid.NewGuid(), 0);
            engineeringModelSetup.IterationSetup.Add(iterationSetup.Iid);
            engineeringModelSetup.Kind = CDP4Common.SiteDirectoryData.EngineeringModelKind.STUDY_MODEL;

            this.engineeringModelSetupService.Setup(x =>
                x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<ISecurityContext>())).Returns(new List<EngineeringModelSetup>() { engineeringModelSetup });

           Assert.That(this.optionSideEffect.BeforeCreate(this.option, this.iteration, null, null, null), Is.True);
        }
    }
}