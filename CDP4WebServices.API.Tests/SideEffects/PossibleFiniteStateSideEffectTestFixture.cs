// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PrefixedUnitSideEffectTestFixture.cs" company="RHEA System S.A.">
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
    using CDP4Common.Types;

    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    [TestFixture]
    public class PossibleFiniteStateSideEffectTestFixture
    {
        private PossibleFiniteStateSideEffect sideEffect = new PossibleFiniteStateSideEffect();

        private NpgsqlTransaction transaction;

        private Mock<ISecurityContext> securityContext;
        private Mock<IActualFiniteStateListService> actualFiniteStateListService; 
        private Mock<IActualFiniteStateService> actualFiniteStateService;
        private Mock<IPossibleFiniteStateListService> possibleFiniteStateListslService;
        private Mock<IParameterValueSetService> parameterValueSetService;
        private Mock<IParameterOverrideValueSetService> parameterOverrideValueSetService;
        private Mock<IParameterSubscriptionValueSetService> parameterSubscriptionValueSetService;
        private Mock<IParameterService> parameterService;
        private Mock<IParameterOverrideService> parameterOverrideService;
        private Mock<IParameterSubscriptionService> parameterSubscriptionService;
        private Mock<IIterationService> iterationService;
        private Mock<ICompoundParameterTypeService> compoundParameterTypeService;
        private FiniteStateLogicService finiteStateLogicService;
        private StateDependentParameterUpdateService parameterUpdateService;

        private string partition = "partition";

        private Iteration iteration;
        private PossibleFiniteStateList psl1;
        private PossibleFiniteStateList psl2;

        private PossibleFiniteState ps11;
        private PossibleFiniteState ps12;

        private PossibleFiniteState ps21;
        private PossibleFiniteState ps22;

        private ActualFiniteStateList asl1;
        private ActualFiniteStateList asl2;

        private ActualFiniteState as11;
        private ActualFiniteState as12;

        private ActualFiniteState as21;
        private ActualFiniteState as22;

        private Option option1;
        private Parameter parameter1;
        private Parameter parameter2;
        private ParameterOverride parameterOverride1;
        private ParameterOverride parameterOverride2;
        private ParameterSubscription parameterSubscription1;
        private ParameterSubscription parameterSubscription2;

        private ParameterValueSet pvs11;
        private ParameterValueSet pvs12;
        private ParameterValueSet pvs21;
        private ParameterValueSet pvs22;

        private ParameterOverrideValueSet povs11;
        private ParameterOverrideValueSet povs12;
        private ParameterOverrideValueSet povs21;
        private ParameterOverrideValueSet povs22;

        private ParameterSubscriptionValueSet psvs11;
        private ParameterSubscriptionValueSet psvs12;
        private ParameterSubscriptionValueSet psvs21;
        private ParameterSubscriptionValueSet psvs22;

        private Guid owner = Guid.NewGuid();

        private List<string> initValue = new List<string> { "init" };
        private Mock<IDefaultValueArrayFactory> defaultValueArrayFactory;


        [SetUp]
        public void Setup()
        {
            this.securityContext = new Mock<ISecurityContext>();
            this.actualFiniteStateListService = new Mock<IActualFiniteStateListService>();
            this.actualFiniteStateService = new Mock<IActualFiniteStateService>();
            this.possibleFiniteStateListslService = new Mock<IPossibleFiniteStateListService>();
            this.parameterValueSetService = new Mock<IParameterValueSetService>();
            this.parameterOverrideValueSetService = new Mock<IParameterOverrideValueSetService>();
            this.parameterService = new Mock<IParameterService>();
            this.parameterOverrideService = new Mock<IParameterOverrideService>();
            this.parameterSubscriptionService = new Mock<IParameterSubscriptionService>();
            this.parameterSubscriptionValueSetService = new Mock<IParameterSubscriptionValueSetService>();
            this.iterationService = new Mock<IIterationService>();
            this.defaultValueArrayFactory = new Mock<IDefaultValueArrayFactory>();
            this.compoundParameterTypeService = new Mock<ICompoundParameterTypeService>();
            this.parameterUpdateService = new StateDependentParameterUpdateService();
            this.finiteStateLogicService = new FiniteStateLogicService();

            this.parameterUpdateService.ParameterValueSetService = this.parameterValueSetService.Object;
            this.parameterUpdateService.ParameterOverrideValueSetService = this.parameterOverrideValueSetService.Object;
            this.parameterUpdateService.ParameterService = this.parameterService.Object;
            this.parameterUpdateService.ParameterOverrideService = this.parameterOverrideService.Object;
            this.parameterUpdateService.ParameterSubscriptionService = this.parameterSubscriptionService.Object;
            this.parameterUpdateService.ParameterSubscriptionValueSetService = this.parameterSubscriptionValueSetService.Object;
            this.parameterUpdateService.CompoundParameterTypeService = this.compoundParameterTypeService.Object;

            this.iteration = new Iteration(Guid.NewGuid(), 1);
            this.option1 = new Option(Guid.NewGuid(), 1);

            this.iteration.Option.Add(new OrderedItem {K = 1, V = this.option1.Iid.ToString()});

            this.psl1 = new PossibleFiniteStateList(Guid.NewGuid(), 1);
            this.psl2 = new PossibleFiniteStateList(Guid.NewGuid(), 1);

            this.ps11 = new PossibleFiniteState(Guid.NewGuid(), 1);
            this.ps12 = new PossibleFiniteState(Guid.NewGuid(), 1);

            this.ps21 = new PossibleFiniteState(Guid.NewGuid(), 1);
            this.ps22 = new PossibleFiniteState(Guid.NewGuid(), 1);

            this.asl1 = new ActualFiniteStateList(Guid.NewGuid(), 1);
            this.asl2 = new ActualFiniteStateList(Guid.NewGuid(), 1);

            this.iteration.PossibleFiniteStateList.Add(this.psl1.Iid);
            this.iteration.PossibleFiniteStateList.Add(this.psl2.Iid);

            this.iteration.ActualFiniteStateList.Add(this.asl1.Iid);
            this.iteration.ActualFiniteStateList.Add(this.asl2.Iid);

            this.psl1.PossibleState.Add(new OrderedItem {K = 1, V = this.ps11.Iid.ToString()});
            this.psl1.PossibleState.Add(new OrderedItem {K = 2, V = this.ps12.Iid.ToString()});
            this.psl2.PossibleState.Add(new OrderedItem {K = 1, V = this.ps21.Iid.ToString()});
            this.psl2.PossibleState.Add(new OrderedItem {K = 2, V = this.ps22.Iid.ToString()});

            this.asl1.PossibleFiniteStateList.Add(new OrderedItem {K = 1, V = this.psl1.Iid.ToString()});
            this.asl1.PossibleFiniteStateList.Add(new OrderedItem {K = 2, V = this.psl2.Iid.ToString()});

            // initializes actual states actual states
            this.as11 = new ActualFiniteState(Guid.NewGuid(), 1);
            this.as11.PossibleState.Add(this.ps11.Iid);
            this.as11.PossibleState.Add(this.ps21.Iid);

            this.as12 = new ActualFiniteState(Guid.NewGuid(), 1);
            this.as12.PossibleState.Add(this.ps11.Iid);
            this.as12.PossibleState.Add(this.ps22.Iid);

            this.asl1.ActualState.Add(this.as11.Iid);
            this.asl1.ActualState.Add(this.as12.Iid);

            this.asl2.PossibleFiniteStateList.Add(new OrderedItem {K = 1, V = this.psl2.Iid.ToString()});
            this.as21 = new ActualFiniteState(Guid.NewGuid(), 1);
            this.as21.PossibleState.Add(this.ps21.Iid);
            this.as22 = new ActualFiniteState(Guid.NewGuid(), 1);
            this.as22.PossibleState.Add(this.ps22.Iid);

            this.asl2.ActualState.Add(this.as21.Iid);
            this.asl2.ActualState.Add(this.as22.Iid);

            this.possibleFiniteStateListslService.Setup(
                x => x.GetShallow(this.transaction, this.partition, It.IsAny<IEnumerable<Guid>>(), this.securityContext.Object))
                .Returns(new List<Thing> {this.psl1, this.psl2});

            this.actualFiniteStateListService.Setup(
                x => x.GetShallow(this.transaction, this.partition, null, this.securityContext.Object))
                .Returns(new List<Thing> {this.asl1, this.asl2});

            this.actualFiniteStateService.Setup(
                x => x.GetShallow(this.transaction, this.partition, It.IsAny<IEnumerable<Guid>>(), this.securityContext.Object))
                .Returns(new List<Thing> {this.as11, this.as12});

            this.iterationService.Setup(x => x.GetActiveIteration(this.transaction, this.partition, this.securityContext.Object))
                .Returns(this.iteration);

            this.parameter1 = new Parameter(Guid.NewGuid(), 1);
            this.parameter1.StateDependence = this.asl1.Iid;

            this.parameter2 = new Parameter(Guid.NewGuid(), 1);
            this.parameter2.StateDependence = this.asl1.Iid;
            this.parameter2.IsOptionDependent = true;

            this.parameterOverride1 = new ParameterOverride(Guid.NewGuid(), 1);
            this.parameterOverride1.Parameter = this.parameter1.Iid;

            this.parameterOverride2 = new ParameterOverride(Guid.NewGuid(), 1);
            this.parameterOverride2.Parameter = this.parameter2.Iid;

            this.parameterSubscription1 = new ParameterSubscription(Guid.NewGuid(), 1);
            this.parameterSubscription2 = new ParameterSubscription(Guid.NewGuid(), 1);

            this.parameter1.ParameterSubscription.Add(this.parameterSubscription1.Iid);
            this.parameterOverride2.ParameterSubscription.Add(this.parameterSubscription2.Iid);

            this.pvs11 = new ParameterValueSet(Guid.NewGuid(), 1)
            {
                Manual = new ValueArray<string>(this.initValue),
                Computed = new ValueArray<string>(this.initValue),
                Reference = new ValueArray<string>(this.initValue),
                Published = new ValueArray<string>(this.initValue),
                Formula = new ValueArray<string>(this.initValue),
                ValueSwitch = CDP4Common.EngineeringModelData.ParameterSwitchKind.REFERENCE,
            };
            this.pvs12 = new ParameterValueSet(Guid.NewGuid(), 1)
            {
                Manual = new ValueArray<string>(this.initValue),
                Computed = new ValueArray<string>(this.initValue),
                Reference = new ValueArray<string>(this.initValue),
                Published = new ValueArray<string>(this.initValue),
                Formula = new ValueArray<string>(this.initValue),
                ValueSwitch = CDP4Common.EngineeringModelData.ParameterSwitchKind.REFERENCE,
            };
            this.pvs21 = new ParameterValueSet(Guid.NewGuid(), 1)
            {
                Manual = new ValueArray<string>(this.initValue),
                Computed = new ValueArray<string>(this.initValue),
                Reference = new ValueArray<string>(this.initValue),
                Published = new ValueArray<string>(this.initValue),
                Formula = new ValueArray<string>(this.initValue),
                ValueSwitch = CDP4Common.EngineeringModelData.ParameterSwitchKind.REFERENCE,
            };
            this.pvs22 = new ParameterValueSet(Guid.NewGuid(), 1)
            {
                Manual = new ValueArray<string>(this.initValue),
                Computed = new ValueArray<string>(this.initValue),
                Reference = new ValueArray<string>(this.initValue),
                Published = new ValueArray<string>(this.initValue),
                Formula = new ValueArray<string>(this.initValue),
                ValueSwitch = CDP4Common.EngineeringModelData.ParameterSwitchKind.REFERENCE,
            };

            this.povs11 = new ParameterOverrideValueSet(Guid.NewGuid(), 1)
            {
                Manual = new ValueArray<string>(this.initValue),
                Computed = new ValueArray<string>(this.initValue),
                Reference = new ValueArray<string>(this.initValue),
                Published = new ValueArray<string>(this.initValue),
                Formula = new ValueArray<string>(this.initValue),
                ValueSwitch = CDP4Common.EngineeringModelData.ParameterSwitchKind.REFERENCE,
                ParameterValueSet = this.pvs11.Iid
            };

            this.povs12 = new ParameterOverrideValueSet(Guid.NewGuid(), 1)
            {
                Manual = new ValueArray<string>(this.initValue),
                Computed = new ValueArray<string>(this.initValue),
                Reference = new ValueArray<string>(this.initValue),
                Published = new ValueArray<string>(this.initValue),
                Formula = new ValueArray<string>(this.initValue),
                ValueSwitch = CDP4Common.EngineeringModelData.ParameterSwitchKind.REFERENCE,
                ParameterValueSet = this.pvs11.Iid
            };
            this.povs21 = new ParameterOverrideValueSet(Guid.NewGuid(), 1)
            {
                Manual = new ValueArray<string>(this.initValue),
                Computed = new ValueArray<string>(this.initValue),
                Reference = new ValueArray<string>(this.initValue),
                Published = new ValueArray<string>(this.initValue),
                Formula = new ValueArray<string>(this.initValue),
                ValueSwitch = CDP4Common.EngineeringModelData.ParameterSwitchKind.REFERENCE,
                ParameterValueSet = this.pvs11.Iid
            };
            this.povs22 = new ParameterOverrideValueSet(Guid.NewGuid(), 1)
            {
                Manual = new ValueArray<string>(this.initValue),
                Computed = new ValueArray<string>(this.initValue),
                Reference = new ValueArray<string>(this.initValue),
                Published = new ValueArray<string>(this.initValue),
                Formula = new ValueArray<string>(this.initValue),
                ValueSwitch = CDP4Common.EngineeringModelData.ParameterSwitchKind.REFERENCE,
                ParameterValueSet = this.pvs11.Iid
            };

            this.psvs11 = new ParameterSubscriptionValueSet(Guid.NewGuid(), 1)
            {
                Manual = new ValueArray<string>(this.initValue),
                ValueSwitch = CDP4Common.EngineeringModelData.ParameterSwitchKind.REFERENCE,
                SubscribedValueSet = this.pvs11.Iid
            };
            this.psvs12 = new ParameterSubscriptionValueSet(Guid.NewGuid(), 1)
            {
                Manual = new ValueArray<string>(this.initValue),
                ValueSwitch = CDP4Common.EngineeringModelData.ParameterSwitchKind.REFERENCE,
                SubscribedValueSet = this.pvs12.Iid
            };
            this.psvs21 = new ParameterSubscriptionValueSet(Guid.NewGuid(), 1)
            {
                Manual = new ValueArray<string>(this.initValue),
                ValueSwitch = CDP4Common.EngineeringModelData.ParameterSwitchKind.REFERENCE,
                SubscribedValueSet = this.povs21.Iid
            };
            this.psvs22 = new ParameterSubscriptionValueSet(Guid.NewGuid(), 1)
            {
                Manual = new ValueArray<string>(this.initValue),
                ValueSwitch = CDP4Common.EngineeringModelData.ParameterSwitchKind.REFERENCE,
                SubscribedValueSet = this.povs22.Iid
            };

            this.parameter1.ValueSet.Add(this.pvs11.Iid);
            this.parameter1.ValueSet.Add(this.pvs12.Iid);
            this.parameter2.ValueSet.Add(this.pvs21.Iid);
            this.parameter2.ValueSet.Add(this.pvs22.Iid);

            this.parameterOverride1.ValueSet.Add(this.povs11.Iid);
            this.parameterOverride1.ValueSet.Add(this.povs12.Iid);
            this.parameterOverride2.ValueSet.Add(this.povs21.Iid);
            this.parameterOverride2.ValueSet.Add(this.povs22.Iid);

            this.parameterSubscription1.ValueSet.Add(this.psvs11.Iid);
            this.parameterSubscription1.ValueSet.Add(this.psvs12.Iid);
            this.parameterSubscription2.ValueSet.Add(this.psvs21.Iid);
            this.parameterSubscription2.ValueSet.Add(this.psvs22.Iid);

            this.parameterService.Setup(x => x.GetShallow(this.transaction, this.partition, null, this.securityContext.Object))
                .Returns(new List<Thing> { this.parameter1, this.parameter2 });

            this.parameterOverrideService.Setup(x => x.GetShallow(this.transaction, this.partition, null, this.securityContext.Object))
                .Returns(new List<Thing> { this.parameterOverride1, this.parameterOverride2 });

            this.parameterSubscriptionService.Setup(x => x.GetShallow(this.transaction, this.partition, null, this.securityContext.Object))
                .Returns(new List<Thing> { this.parameterSubscription1, this.parameterSubscription2 });

            this.parameterValueSetService.Setup(
                x => x.GetShallow(this.transaction, this.partition, this.parameter1.ValueSet, this.securityContext.Object))
                .Returns(new List<Thing> {this.pvs11, this.pvs12});
            this.parameterValueSetService.Setup(
                x => x.GetShallow(this.transaction, this.partition, this.parameter2.ValueSet, this.securityContext.Object))
                .Returns(new List<Thing> {this.pvs21, this.pvs22});

            this.parameterOverrideValueSetService.Setup(
                x =>
                    x.GetShallow(this.transaction, this.partition, this.parameterOverride1.ValueSet,
                        this.securityContext.Object))
                .Returns(new List<Thing> {this.povs11, this.povs12});
            this.parameterOverrideValueSetService.Setup(
                x =>
                    x.GetShallow(this.transaction, this.partition, this.parameterOverride2.ValueSet,
                        this.securityContext.Object))
                .Returns(new List<Thing> {this.povs21, this.povs22});

            this.parameterSubscriptionValueSetService.Setup(
                x =>
                    x.GetShallow(this.transaction, this.partition, this.parameterSubscription1.ValueSet,
                        this.securityContext.Object))
                .Returns(new List<Thing> {this.psvs11, this.psvs12});
            this.parameterSubscriptionValueSetService.Setup(
                x =>
                    x.GetShallow(this.transaction, this.partition, this.parameterSubscription2.ValueSet,
                        this.securityContext.Object))
                .Returns(new List<Thing> {this.psvs21, this.psvs22});

            this.compoundParameterTypeService.Setup(
                x => x.GetShallow(this.transaction, this.partition, It.IsAny<IEnumerable<Guid>>(), this.securityContext.Object))
                .Returns(new List<Thing>());

            this.defaultValueArrayFactory.Setup(x => x.CreateDefaultValueArray(It.IsAny<Guid>())).Returns(new ValueArray<string>(new[] { "-" }));
            this.finiteStateLogicService.ActualFiniteStateListService = this.actualFiniteStateListService.Object;
            this.finiteStateLogicService.ActualFiniteStateService = this.actualFiniteStateService.Object;
            this.finiteStateLogicService.IterationService = this.iterationService.Object;
            this.finiteStateLogicService.PossibleFiniteStateListService = this.possibleFiniteStateListslService.Object;
            this.finiteStateLogicService.StateDependentParameterUpdateService = this.parameterUpdateService;

            this.sideEffect.FiniteStateLogicService = this.finiteStateLogicService;

            this.parameterUpdateService.DefaultValueSetFactory = this.defaultValueArrayFactory.Object;
            this.parameterValueSetService.Setup(x => x.DeleteConcept(this.transaction, this.partition, It.IsAny<ParameterValueSet>(), It.IsAny<Parameter>())).Returns(true);
            this.parameterOverrideValueSetService.Setup(x => x.DeleteConcept(this.transaction, this.partition, It.IsAny<ParameterOverrideValueSet>(), It.IsAny<ParameterOverride>())).Returns(true);
            this.parameterSubscriptionValueSetService.Setup(x => x.DeleteConcept(this.transaction, this.partition, It.IsAny<ParameterSubscriptionValueSet>(), It.IsAny<ParameterSubscription>())).Returns(true);
        }

        [Test]
        public void VerifyThatAfterCreateWorks()
        {
            var originalThing = this.ps12.DeepClone<Thing>();
            this.actualFiniteStateService.Setup(
                x => x.DeleteConcept(this.transaction, this.partition, this.as11, this.asl1)).Returns(true);
            this.actualFiniteStateService.Setup(
                x => x.DeleteConcept(this.transaction, this.partition, this.as12, this.asl1)).Returns(true);

            this.sideEffect.AfterCreate(this.ps12, this.psl1, originalThing, this.transaction, this.partition, this.securityContext.Object);

            this.actualFiniteStateService.Verify(x => x.CreateConcept(this.transaction, this.partition, It.IsAny<ActualFiniteState>(), this.asl1, -1), Times.Exactly(4));

            this.parameterValueSetService.Verify(x => x.CreateConcept(this.transaction, this.partition, It.IsAny<ParameterValueSet>(), It.IsAny<Parameter>(), -1), Times.Exactly(8));

            this.parameterOverrideValueSetService.Verify(x => x.CreateConcept(this.transaction, this.partition, It.IsAny<ParameterOverrideValueSet>(), It.IsAny<ParameterOverride>(), -1), Times.Exactly(8));

            this.parameterSubscriptionValueSetService.Verify(x => x.CreateConcept(this.transaction, this.partition, It.IsAny<ParameterSubscriptionValueSet>(), It.IsAny<ParameterSubscription>(), -1), Times.Exactly(8));

            this.actualFiniteStateService.Verify(x => x.DeleteConcept(this.transaction, this.partition, this.as11, this.asl1), Times.Once);
            this.actualFiniteStateService.Verify(x => x.DeleteConcept(this.transaction, this.partition, this.as12, this.asl1), Times.Once);
        }

        [Test]
        public void VerifyThatBeforeDeleteWorksNormalOperation()
        {
            // A PossibleFiniteState is deleted but other states still exist
            this.sideEffect.BeforeDelete(this.ps22, this.psl2, this.transaction, this.partition, this.securityContext.Object);

            this.actualFiniteStateListService.Verify(x => x.GetShallow(this.transaction, this.partition, null, this.securityContext.Object), Times.Never);
            this.possibleFiniteStateListslService.Verify(x => x.DeleteConcept(this.transaction, this.partition, It.IsAny<Thing>(), It.IsAny<Thing>()), Times.Never);
        }

        [Test]
        public void VerifyThatBeforeDeleteWorksExceptionIsThrownForTheLastPossibleFiniteState()
        {
            this.psl1.PossibleState.Clear();
            this.psl1.PossibleState.Add(new OrderedItem { K = 1, V = this.ps11.Iid.ToString() });

            Assert.Throws<InvalidOperationException>(
                () => this.sideEffect.BeforeDelete(this.ps11, this.psl1, this.transaction, this.partition, this.securityContext.Object));
        }
    }
}