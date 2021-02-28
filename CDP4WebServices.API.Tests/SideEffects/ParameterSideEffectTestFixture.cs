// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterSideEParameterSideEffectTestFixtureffect.cs" company="RHEA System S.A.">
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

    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Types;

    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    using ActualFiniteState = CDP4Common.DTO.ActualFiniteState;
    using ActualFiniteStateList = CDP4Common.DTO.ActualFiniteStateList;
    using ElementDefinition = CDP4Common.DTO.ElementDefinition;
    using ElementUsage = CDP4Common.DTO.ElementUsage;
    using Iteration = CDP4Common.DTO.Iteration;
    using Option = CDP4Common.DTO.Option;
    using Parameter = CDP4Common.DTO.Parameter;
    using ParameterOverride = CDP4Common.DTO.ParameterOverride;
    using ParameterOverrideValueSet = CDP4Common.DTO.ParameterOverrideValueSet;
    using ParameterSubscription = CDP4Common.DTO.ParameterSubscription;
    using ParameterSubscriptionValueSet = CDP4Common.DTO.ParameterSubscriptionValueSet;
    using ParameterValueSet = CDP4Common.DTO.ParameterValueSet;

    /// <summary>
    /// Test fixture for the <see cref="ParameterSideEffect" /> class
    /// </summary>
    [TestFixture]
    public class ParameterSideEParameterSideEffectTestFixtureffect
    {
        [SetUp]
        public void Setup()
        {
            this.securityContext = new Mock<ISecurityContext>();
            this.optionService = new Mock<IOptionService>();
            this.actualFiniteStateListService = new Mock<IActualFiniteStateListService>();
            this.parameterService = new Mock<ICompoundParameterTypeService>();
            this.valueSetService = new Mock<IParameterValueSetService>();
            this.iterationService = new Mock<IIterationService>();
            this.parameterOverrideValueSetService = new Mock<IParameterOverrideValueSetService>();
            this.parameterSubscriptionValueSetService = new Mock<IParameterSubscriptionValueSetService>();
            this.parameterSubscriptionService = new Mock<IParameterSubscriptionService>();
            this.parameterOverrideService = new Mock<IParameterOverrideService>();
            this.parameterTypeComponentService = new Mock<IParameterTypeComponentService>();
            this.parameterTypeService = new Mock<IParameterTypeService>();
            this.elementUsageService = new Mock<IElementUsageService>();
            this.defaultValueArrayFactory = new Mock<IDefaultValueArrayFactory>();
            this.OldParameterContextProvider = new Mock<IOldParameterContextProvider>();

            this.organizationalParticipationResolverService = new Mock<IOrganizationalParticipationResolverService>();
            this.organizationalParticipationResolverService.Setup(x => x.ValidateCreateOrganizationalParticipation(It.IsAny<Thing>(), It.IsAny<Thing>(), It.IsAny<ISecurityContext>(), this.npgsqlTransaction, It.IsAny<string>()));

            this.npgsqlTransaction = null;

            this.iteration = new Iteration(Guid.NewGuid(), 1);
            this.option1 = new Option(Guid.NewGuid(), 1);
            this.option2 = new Option(Guid.NewGuid(), 1);

            this.iteration.Option.Add(new OrderedItem { K = 1, V = this.option1.Iid });
            this.iteration.Option.Add(new OrderedItem { K = 2, V = this.option2.Iid });

            this.actualList = new ActualFiniteStateList(Guid.NewGuid(), 1);
            this.actualState1 = new ActualFiniteState(Guid.NewGuid(), 1);
            this.actualState2 = new ActualFiniteState(Guid.NewGuid(), 1);

            this.actualList.ActualState.Add(this.actualState1.Iid);
            this.actualList.ActualState.Add(this.actualState2.Iid);

            this.parameter = new Parameter(Guid.NewGuid(), 1);

            this.cptParameterType = new CompoundParameterType(Guid.NewGuid(), 1);
            this.boolPt = new BooleanParameterType(Guid.NewGuid(), 1);
            this.cpt1 = new ParameterTypeComponent(Guid.NewGuid(), 1) { ParameterType = this.boolPt.Iid };
            this.cpt2 = new ParameterTypeComponent(Guid.NewGuid(), 1) { ParameterType = this.boolPt.Iid };

            this.cptParameterType.Component.Add(new OrderedItem { K = 1, V = this.cpt1.Iid.ToString() });
            this.cptParameterType.Component.Add(new OrderedItem { K = 2, V = this.cpt2.Iid.ToString() });

            this.sideEffect = new ParameterSideEffect
            {
                IterationService = this.iterationService.Object,
                ActualFiniteStateListService = this.actualFiniteStateListService.Object,
                ParameterValueSetService = this.valueSetService.Object,
                ParameterOverrideValueSetService = this.parameterOverrideValueSetService.Object,
                ParameterSubscriptionValueSetService = this.parameterSubscriptionValueSetService.Object,
                ParameterOverrideService = this.parameterOverrideService.Object,
                ParameterSubscriptionService = this.parameterSubscriptionService.Object,
                ParameterTypeService = this.parameterTypeService.Object,
                ElementUsageService = this.elementUsageService.Object,
                ParameterTypeComponentService = this.parameterTypeComponentService.Object,
                OptionService = this.optionService.Object,
                DefaultValueArrayFactory = this.defaultValueArrayFactory.Object,
                ParameterValueSetFactory = new ParameterValueSetFactory(),
                ParameterOverrideValueSetFactory = new ParameterOverrideValueSetFactory(),
                ParameterSubscriptionValueSetFactory = new ParameterSubscriptionValueSetFactory(),
                OldParameterContextProvider = this.OldParameterContextProvider.Object,
                OrganizationalParticipationResolverService = this.organizationalParticipationResolverService.Object
            };

            // prepare mock data
            this.elementDefinition = new ElementDefinition(Guid.NewGuid(), 1);
            this.elementDefinition.Parameter.Add(this.parameter.Iid);

            this.parameterOverride = new ParameterOverride(Guid.NewGuid(), 1)
            {
                Parameter = this.parameter.Iid
            };

            this.elementUsage = new ElementUsage(Guid.NewGuid(), 1) { ElementDefinition = this.elementDefinition.Iid, ParameterOverride = { this.parameterOverride.Iid } };

            this.parameterService.Setup(x => x.Get(It.IsAny<NpgsqlTransaction>(), "SiteDirectory", It.Is<IEnumerable<Guid>>(y => y.Contains(this.cptParameterType.Iid)), this.securityContext.Object))
                .Returns(new List<Thing> { this.cptParameterType });

            this.iterationService.Setup(x => x.GetActiveIteration(null, "partition", this.securityContext.Object))
                .Returns(this.iteration);

            this.actualFiniteStateListService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), "partition", It.Is<IEnumerable<Guid>>(y => y.Contains(this.actualList.Iid)), this.securityContext.Object))
                .Returns(new List<Thing> { this.actualList });

            this.parameterTypeService.Setup(x => x.GetShallow(this.npgsqlTransaction, "SiteDirectory", null, this.securityContext.Object))
                .Returns(new List<Thing> { this.boolPt, this.cptParameterType });

            this.parameterTypeService.Setup(x => x.GetShallow(this.npgsqlTransaction, "SiteDirectory", new List<Guid> { this.existingNotQuantityKindParameterTypeGuid }, this.securityContext.Object))
                .Returns(new List<Thing> { new BooleanParameterType(this.existingNotQuantityKindParameterTypeGuid, 1) });

            this.parameterTypeService.Setup(x => x.GetShallow(this.npgsqlTransaction, "SiteDirectory", new List<Guid> { this.existingQuantityKindParameterTypeGuid }, this.securityContext.Object))
                .Returns(new List<Thing> { new SimpleQuantityKind(this.existingQuantityKindParameterTypeGuid, 1) });

            this.parameterTypeService.Setup(x => x.GetShallow(this.npgsqlTransaction, "SiteDirectory", new List<Guid> { this.notExistingParameterTypeGuid }, this.securityContext.Object))
                .Returns(new List<Thing>());

            this.parameterTypeComponentService.Setup(x => x.GetShallow(this.npgsqlTransaction, "SiteDirectory", null, this.securityContext.Object))
                .Returns(new List<Thing> { this.cpt1, this.cpt2 });

            this.parameterOverrideService.Setup(x => x.GetShallow(this.npgsqlTransaction, "partition", null, this.securityContext.Object))
                .Returns(new List<Thing> { this.parameterOverride });

            this.elementUsageService.Setup(x => x.GetShallow(this.npgsqlTransaction, "partition", null, this.securityContext.Object))
                .Returns(new List<Thing> { this.elementUsage });

            this.scalarDefaultValueArray = new ValueArray<string>(new List<string> { "-" });
            this.compoundDefaultValueArray = new ValueArray<string>(new List<string> { "-", "-" });

            this.defaultValueArrayFactory.Setup(x => x.CreateDefaultValueArray(this.cptParameterType.Iid))
                .Returns(this.compoundDefaultValueArray);

            this.defaultValueArrayFactory.Setup(x => x.CreateDefaultValueArray(this.boolPt.Iid))
                .Returns(this.scalarDefaultValueArray);

            this.OldParameterContextProvider.Setup(x => x.GetsourceValueSet(It.IsAny<Guid?>(), It.IsAny<Guid?>())).Returns((ParameterValueSet) null);
        }

        private Mock<ISecurityContext> securityContext;
        private NpgsqlTransaction npgsqlTransaction;
        private Mock<IOptionService> optionService;
        private Mock<IActualFiniteStateListService> actualFiniteStateListService;
        private Mock<ICompoundParameterTypeService> parameterService;
        private Mock<IParameterValueSetService> valueSetService;
        private Mock<IIterationService> iterationService;
        private Mock<IParameterOverrideValueSetService> parameterOverrideValueSetService;
        private Mock<IParameterSubscriptionValueSetService> parameterSubscriptionValueSetService;
        private Mock<IParameterSubscriptionService> parameterSubscriptionService;
        private Mock<IParameterOverrideService> parameterOverrideService;
        private Mock<IParameterTypeComponentService> parameterTypeComponentService;
        private Mock<IParameterTypeService> parameterTypeService;
        private Mock<IElementUsageService> elementUsageService;
        private Mock<IDefaultValueArrayFactory> defaultValueArrayFactory;
        private Mock<IOldParameterContextProvider> OldParameterContextProvider;

        private Mock<IOrganizationalParticipationResolverService> organizationalParticipationResolverService;

        private Parameter parameter;
        private ParameterOverride parameterOverride;
        private Iteration iteration;
        private ActualFiniteStateList actualList;
        private ActualFiniteState actualState1;
        private ActualFiniteState actualState2;
        private Option option1;
        private Option option2;
        private CompoundParameterType cptParameterType;
        private BooleanParameterType boolPt;
        private ParameterTypeComponent cpt1;
        private ParameterTypeComponent cpt2;
        private ElementDefinition elementDefinition;
        private ElementUsage elementUsage;

        private ParameterSideEffect sideEffect;

        private readonly Guid existingQuantityKindParameterTypeGuid = Guid.NewGuid();
        private readonly Guid existingNotQuantityKindParameterTypeGuid = Guid.NewGuid();
        private readonly Guid notExistingParameterTypeGuid = Guid.NewGuid();
        private readonly Guid scaleGuid = Guid.NewGuid();

        private ValueArray<string> compoundDefaultValueArray;
        private ValueArray<string> scalarDefaultValueArray;

        private const string ParameterTypeTestKey = "ParameterType";
        private const string ScaleTestKey = "Scale";

        [Test]
        public void VerifyBeforeCreateSideEffectPasses()
        {
            this.parameter.ParameterType = this.existingNotQuantityKindParameterTypeGuid;

            this.sideEffect.BeforeCreate(
                this.parameter,
                this.elementDefinition,
                this.npgsqlTransaction,
                "partition",
                this.securityContext.Object);

            this.parameter.ParameterType = this.existingQuantityKindParameterTypeGuid;
            this.parameter.Scale = this.scaleGuid;

            this.sideEffect.BeforeCreate(
                this.parameter,
                this.elementDefinition,
                this.npgsqlTransaction,
                "partition",
                this.securityContext.Object);

            this.parameterTypeService.Verify(x => x.GetShallow(this.npgsqlTransaction, "partition", It.IsAny<List<Guid>>(), this.securityContext.Object),
                Times.Exactly(0));

            this.parameterTypeService.Verify(x => x.GetShallow(this.npgsqlTransaction, "SiteDirectory", It.IsAny<List<Guid>>(), this.securityContext.Object),
                Times.Exactly(2));
        }

        [Test]
        public void VerifyBeforeCreateSideEffectThrowsExceptionForNotExistingParameterType()
        {
            this.parameter.ParameterType = this.notExistingParameterTypeGuid;

            Assert.Throws<ArgumentException>(
                () => this.sideEffect.BeforeCreate(this.parameter, this.elementDefinition, this.npgsqlTransaction, "partition", this.securityContext.Object));

            this.parameterTypeService.Verify(x => x.GetShallow(this.npgsqlTransaction, "SiteDirectory", It.IsAny<List<Guid>>(), this.securityContext.Object),
                Times.Once);
        }

        [Test]
        public void VerifyBeforeCreateSideEffectThrowsExceptionForNotQuantityKindScaleNotBeingNull()
        {
            this.parameter.ParameterType = this.existingNotQuantityKindParameterTypeGuid;
            this.parameter.Scale = this.scaleGuid;

            Assert.Throws<ArgumentException>(
                () => this.sideEffect.BeforeCreate(this.parameter, this.elementDefinition, this.npgsqlTransaction, "partition", this.securityContext.Object));

            this.parameterTypeService.Verify(x => x.GetShallow(this.npgsqlTransaction, "SiteDirectory", It.IsAny<List<Guid>>(), this.securityContext.Object),
                Times.Once);
        }

        [Test]
        public void VerifyBeforeCreateSideEffectThrowsExceptionForQuantityKindScaleBeingNull()
        {
            this.parameter.ParameterType = this.existingQuantityKindParameterTypeGuid;

            Assert.Throws<ArgumentNullException>(
                () => this.sideEffect.BeforeCreate(this.parameter, this.elementDefinition, this.npgsqlTransaction, "partition", this.securityContext.Object));

            this.parameterTypeService.Verify(x => x.GetShallow(this.npgsqlTransaction, "SiteDirectory", It.IsAny<List<Guid>>(), this.securityContext.Object),
                Times.Once);
        }

        [Test]
        public void VerifyBeforeUpdateSideEffectPasses()
        {
            var rawUpdateInfo = new ClasslessDTO { { ParameterTypeTestKey, this.existingNotQuantityKindParameterTypeGuid } };

            this.sideEffect.BeforeUpdate(
                this.parameter,
                this.elementDefinition,
                this.npgsqlTransaction,
                "partition",
                this.securityContext.Object,
                rawUpdateInfo);

            rawUpdateInfo = new ClasslessDTO { { ParameterTypeTestKey, this.existingNotQuantityKindParameterTypeGuid }, { ScaleTestKey, null } };
            this.parameter.Scale = this.scaleGuid;

            this.sideEffect.BeforeUpdate(
                this.parameter,
                this.elementDefinition,
                this.npgsqlTransaction,
                "partition",
                this.securityContext.Object,
                rawUpdateInfo);

            rawUpdateInfo = new ClasslessDTO { { ParameterTypeTestKey, this.existingQuantityKindParameterTypeGuid } };

            this.sideEffect.BeforeUpdate(
                this.parameter,
                this.elementDefinition,
                this.npgsqlTransaction,
                "partition",
                this.securityContext.Object,
                rawUpdateInfo);

            rawUpdateInfo = new ClasslessDTO { { ParameterTypeTestKey, this.existingQuantityKindParameterTypeGuid }, { ScaleTestKey, this.scaleGuid } };
            this.parameter.Scale = null;

            this.sideEffect.BeforeUpdate(
                this.parameter,
                this.elementDefinition,
                this.npgsqlTransaction,
                "partition",
                this.securityContext.Object,
                rawUpdateInfo);

            this.parameterTypeService.Verify(x => x.GetShallow(this.npgsqlTransaction, "SiteDirectory", It.IsAny<List<Guid>>(), this.securityContext.Object),
                Times.Exactly(4));
        }

        [Test]
        public void VerifyBeforeUpdateSideEffectThrowsExceptionForNotExistingParameterType()
        {
            var rawUpdateInfo = new ClasslessDTO { { ParameterTypeTestKey, this.notExistingParameterTypeGuid } };

            Assert.Throws<ArgumentException>(
                () => this.sideEffect.BeforeUpdate(this.parameter, this.elementDefinition, this.npgsqlTransaction, "partition", this.securityContext.Object, rawUpdateInfo));

            this.parameterTypeService.Verify(x => x.GetShallow(this.npgsqlTransaction, "SiteDirectory", It.IsAny<List<Guid>>(), this.securityContext.Object),
                Times.Once);
        }

        [Test]
        public void VerifyBeforeUpdateSideEffectThrowsExceptionForNotQuantityKindScaleNotBeingNull()
        {
            var rawUpdateInfo = new ClasslessDTO { { ParameterTypeTestKey, this.existingNotQuantityKindParameterTypeGuid }, { ScaleTestKey, this.scaleGuid } };
            this.parameter.Scale = this.scaleGuid;

            Assert.Throws<ArgumentException>(
                () => this.sideEffect.BeforeUpdate(this.parameter, this.elementDefinition, this.npgsqlTransaction, "partition", this.securityContext.Object, rawUpdateInfo));

            rawUpdateInfo = new ClasslessDTO { { ParameterTypeTestKey, this.existingNotQuantityKindParameterTypeGuid } };

            Assert.Throws<ArgumentException>(
                () => this.sideEffect.BeforeUpdate(this.parameter, this.elementDefinition, this.npgsqlTransaction, "partition", this.securityContext.Object, rawUpdateInfo));

            rawUpdateInfo = new ClasslessDTO { { ParameterTypeTestKey, this.existingNotQuantityKindParameterTypeGuid }, { ScaleTestKey, this.scaleGuid } };
            this.parameter.Scale = null;

            Assert.Throws<ArgumentException>(
                () => this.sideEffect.BeforeUpdate(this.parameter, this.elementDefinition, this.npgsqlTransaction, "partition", this.securityContext.Object, rawUpdateInfo));

            this.parameterTypeService.Verify(x => x.GetShallow(this.npgsqlTransaction, "SiteDirectory", It.IsAny<List<Guid>>(), this.securityContext.Object),
                Times.Exactly(3));
        }

        [Test]
        public void VerifyBeforeUpdateSideEffectThrowsExceptionForQuantityKindScaleBeingNull()
        {
            var rawUpdateInfo = new ClasslessDTO { { ParameterTypeTestKey, this.existingQuantityKindParameterTypeGuid }, { ScaleTestKey, null } };
            this.parameter.Scale = this.scaleGuid;

            Assert.Throws<ArgumentNullException>(
                () => this.sideEffect.BeforeUpdate(this.parameter, this.elementDefinition, this.npgsqlTransaction, "partition", this.securityContext.Object, rawUpdateInfo));

            rawUpdateInfo = new ClasslessDTO { { ParameterTypeTestKey, this.existingQuantityKindParameterTypeGuid } };
            this.parameter.Scale = null;

            Assert.Throws<ArgumentNullException>(
                () => this.sideEffect.BeforeUpdate(this.parameter, this.elementDefinition, this.npgsqlTransaction, "partition", this.securityContext.Object, rawUpdateInfo));

            rawUpdateInfo = new ClasslessDTO { { ParameterTypeTestKey, this.existingQuantityKindParameterTypeGuid }, { ScaleTestKey, null } };

            Assert.Throws<ArgumentNullException>(
                () => this.sideEffect.BeforeUpdate(this.parameter, this.elementDefinition, this.npgsqlTransaction, "partition", this.securityContext.Object, rawUpdateInfo));

            this.parameterTypeService.Verify(x => x.GetShallow(this.npgsqlTransaction, "SiteDirectory", It.IsAny<List<Guid>>(), this.securityContext.Object),
                Times.Exactly(3));
        }

        [Test]
        public void VerifyOwnerChangedSideEffects()
        {
            this.parameter.ParameterType = this.boolPt.Iid;
            var domain1 = new DomainOfExpertise(Guid.NewGuid(), 0);
            var domain2 = new DomainOfExpertise(Guid.NewGuid(), 0);
            var originalThing = this.parameter.DeepClone<Thing>();

            this.parameter.Owner = domain1.Iid;

            var updatedParameter = new Parameter(this.parameter.Iid, 0) { ParameterType = this.boolPt.Iid, Owner = domain2.Iid, AllowDifferentOwnerOfOverride = true };
            var parameterOverride1 = new ParameterOverride(Guid.NewGuid(), 0) { Parameter = updatedParameter.Iid, Owner = domain2.Iid };
            var domain2Subscription = new ParameterSubscription(Guid.NewGuid(), 0) { Owner = domain2.Iid };
            updatedParameter.ParameterSubscription.Add(domain2Subscription.Iid);
            parameterOverride1.ParameterSubscription.Add(domain2Subscription.Iid);

            this.parameterSubscriptionService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), this.securityContext.Object))
                .Returns(new List<Thing> { domain2Subscription });

            this.parameterSubscriptionService.Setup(x => x.DeleteConcept(It.IsAny<NpgsqlTransaction>(),
                It.IsAny<string>(), It.IsAny<Thing>(), It.IsAny<Thing>())).Returns(true);

            this.sideEffect.AfterUpdate(updatedParameter, this.elementDefinition, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            // Check that the subscription owned by domain2 is deleted since domain2 is now the owner of the parameter
            this.parameterSubscriptionService.Verify(x => x.DeleteConcept(this.npgsqlTransaction, "partition", It.IsAny<ParameterSubscription>(), It.IsAny<Parameter>()),
                Times.Once);

            // Since that is the only subscription, no updates are performed on ParameterSubscriptionValueSet
            this.parameterSubscriptionValueSetService.Verify(x => x.CreateConcept(this.npgsqlTransaction, "partition", It.IsAny<ParameterSubscriptionValueSet>(), It.IsAny<ParameterSubscription>(), -1),
                Times.Never);

            // Check that since AllowDifferentOwnerOfOverride is True the owner of the parameterOverrides are not updated
            this.parameterOverrideService.Verify(x => x.UpdateConcept(this.npgsqlTransaction, "partition", It.IsAny<ParameterOverride>(), It.IsAny<ElementUsage>()),
                Times.Never);

            var updatedParameter1 = new Parameter(this.parameter.Iid, 0) { ParameterType = this.boolPt.Iid, Owner = domain2.Iid, AllowDifferentOwnerOfOverride = false };
            updatedParameter1.ParameterSubscription.Add(domain2Subscription.Iid);
            this.sideEffect.AfterUpdate(updatedParameter1, this.elementDefinition, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            // Check that since AllowDifferentOwnerOfOverride is False the owner of the parameterOverride is updated
            this.parameterOverrideService.Verify(x => x.UpdateConcept(this.npgsqlTransaction, "partition", It.IsAny<ParameterOverride>(), It.IsAny<ElementUsage>()),
                Times.Once);
        }

        [Test]
        public void VerifyThatAfterUpdateStateChangesWorks()
        {
            var originalThing = this.parameter.DeepClone<Thing>();

            this.valueSetService.Setup(
                x => x.DeleteConcept(null, "partition", It.IsAny<ParameterValueSet>(), this.parameter)).Returns(true);

            this.parameter.ParameterType = this.cptParameterType.Iid;

            this.parameter.StateDependence = null;
            var valueset = new ParameterValueSet(Guid.NewGuid(), 0);
            this.parameter.ValueSet.Add(valueset.Iid);

            var updatedParameter = new Parameter(this.parameter.Iid, 0) { StateDependence = this.actualList.Iid };
            updatedParameter.ValueSet.Add(valueset.Iid);
            updatedParameter.ParameterType = this.cptParameterType.Iid;

            this.sideEffect.AfterUpdate(updatedParameter, this.elementDefinition, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            this.valueSetService.Verify(x => x.CreateConcept(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualState == this.actualState1.Iid), updatedParameter, -1), Times.Exactly(1));
            this.valueSetService.Verify(x => x.CreateConcept(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualState == this.actualState2.Iid), updatedParameter, -1), Times.Exactly(1));
        }

        [Test]
        public void VerifyThatAfterUpdateUpdateTheOVerrideAndSubscription()
        {
            var valueset = new ParameterValueSet(Guid.NewGuid(), 0);
            valueset.Manual = new ValueArray<string>(new[] { "set" });
            valueset.Published = new ValueArray<string>(new[] { "set" });
            valueset.Computed = new ValueArray<string>(new[] { "set" });
            valueset.ValueSwitch = ParameterSwitchKind.REFERENCE;

            this.parameter.ValueSet.Add(valueset.Iid);
            this.OldParameterContextProvider.Setup(x => x.GetsourceValueSet(null, It.IsAny<Guid?>())).Returns(valueset);

            var overrideValueSet = new ParameterOverrideValueSet(Guid.NewGuid(), 1);
            overrideValueSet.ParameterValueSet = valueset.Iid;
            overrideValueSet.Manual = new ValueArray<string>(new[] { "override" });
            overrideValueSet.Published = new ValueArray<string>(new[] { "override" });
            overrideValueSet.Computed = new ValueArray<string>(new[] { "override" });
            overrideValueSet.ValueSwitch = ParameterSwitchKind.REFERENCE;

            this.parameterOverride.ValueSet.Add(overrideValueSet.Iid);

            var subscription1 = new ParameterSubscription(Guid.NewGuid(), 1);
            var subscription2 = new ParameterSubscription(Guid.NewGuid(), 2);

            var subscription1ValueSet = new ParameterSubscriptionValueSet(Guid.NewGuid(), 1);
            subscription1ValueSet.Manual = new ValueArray<string>(new[] { "sub1" });
            subscription1ValueSet.SubscribedValueSet = valueset.Iid;

            var subscription2ValueSet = new ParameterSubscriptionValueSet(Guid.NewGuid(), 1);
            subscription2ValueSet.Manual = new ValueArray<string>(new[] { "sub2" });
            subscription2ValueSet.SubscribedValueSet = overrideValueSet.Iid;

            subscription1.ValueSet.Add(subscription1ValueSet.Iid);
            subscription2.ValueSet.Add(subscription2ValueSet.Iid);

            var originalThing = this.parameter.DeepClone<Thing>();

            this.parameterOverride.ParameterSubscription.Add(subscription2.Iid);
            this.parameter.ParameterSubscription.Add(subscription1.Iid);

            this.parameterOverrideService.Setup(
                    x => x.GetShallow(this.npgsqlTransaction, "partition", null, this.securityContext.Object))
                .Returns(new List<Thing> { this.parameterOverride });

            this.parameterSubscriptionService.Setup(x =>
                    x.GetShallow(this.npgsqlTransaction, "partition",
                        It.Is<IEnumerable<Guid>>(g => g.Contains(subscription1.Iid) && g.Contains(subscription2.Iid)),
                        this.securityContext.Object))
                .Returns(new List<Thing> { subscription1, subscription2 });

            this.valueSetService.Setup(x => x.DeleteConcept(null, "partition", It.IsAny<ParameterValueSet>(), this.parameter))
                .Returns(true);

            this.parameterOverrideValueSetService
                .Setup(x => x.GetShallow(null, "partition", It.Is<IEnumerable<Guid>>(y => y.Contains(overrideValueSet.Iid)), this.securityContext.Object))
                .Returns(new[] { overrideValueSet });

            this.parameterSubscriptionValueSetService
                .Setup(x => x.GetShallow(null, "partition", It.Is<IEnumerable<Guid>>(y => y.Contains(subscription1ValueSet.Iid)), this.securityContext.Object))
                .Returns(new[] { subscription1ValueSet });

            this.parameterSubscriptionValueSetService
                .Setup(x => x.GetShallow(null, "partition", It.Is<IEnumerable<Guid>>(y => y.Contains(subscription2ValueSet.Iid)), this.securityContext.Object))
                .Returns(new[] { subscription2ValueSet });

            this.parameter.ParameterType = this.cptParameterType.Iid;
            this.parameter.StateDependence = null;

            var updatedParameter = new Parameter(this.parameter.Iid, 0) { StateDependence = this.actualList.Iid };
            updatedParameter.ValueSet.Add(valueset.Iid);
            updatedParameter.ParameterType = this.cptParameterType.Iid;
            updatedParameter.ParameterSubscription.Add(subscription1.Iid);

            this.sideEffect.AfterUpdate(updatedParameter, this.elementDefinition, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            this.valueSetService.Verify(x => x.CreateConcept(this.npgsqlTransaction, "partition", It.Is<ParameterValueSet>(y => y.Manual.Contains("set")), updatedParameter, -1),
                Times.Exactly(2));

            this.parameterOverrideValueSetService.Verify(x => x.CreateConcept(this.npgsqlTransaction, "partition", It.Is<ParameterOverrideValueSet>(y => y.Manual.Contains("override")), this.parameterOverride, -1),
                Times.Exactly(2));

            this.parameterSubscriptionValueSetService.Verify(x => x.CreateConcept(this.npgsqlTransaction, "partition", It.Is<ParameterSubscriptionValueSet>(y => y.Manual.Contains("sub1")), subscription1, -1),
                Times.Exactly(2));

            this.parameterSubscriptionValueSetService.Verify(x => x.CreateConcept(this.npgsqlTransaction, "partition", It.Is<ParameterSubscriptionValueSet>(y => y.Manual.Contains("sub2")), subscription2, -1),
                Times.Exactly(2));
        }

        [Test]
        public void VerifyThatValueSetAreCreatedCompoundNoOptionNoState()
        {
            this.parameter.ParameterType = this.cptParameterType.Iid;
            var originalThing = this.parameter.DeepClone<Thing>();
            this.sideEffect.AfterCreate(this.parameter, this.elementDefinition, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            this.valueSetService.Verify(x => x.CreateConcept(null, "partition", It.Is<ParameterValueSet>(vs => vs.Manual.Count() == this.cptParameterType.Component.Count), this.parameter, -1), Times.Exactly(1));
        }

        [Test]
        public void VerifyThatValueSetAreCreatedNoOptionWithState()
        {
            var originalThing = this.parameter.DeepClone<Thing>();
            this.parameter.ParameterType = this.cptParameterType.Iid;

            this.parameter.StateDependence = this.actualList.Iid;

            this.sideEffect.AfterCreate(this.parameter, this.elementDefinition, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            this.valueSetService.Verify(x => x.CreateConcept(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualState == this.actualState1.Iid), this.parameter, -1), Times.Exactly(1));
            this.valueSetService.Verify(x => x.CreateConcept(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualState == this.actualState2.Iid), this.parameter, -1), Times.Exactly(1));
        }

        [Test]
        public void VerifyThatValueSetAreCreatedOptionNoState()
        {
            var originalThing = this.parameter.DeepClone<Thing>();
            this.parameter.ParameterType = this.cptParameterType.Iid;

            this.parameter.IsOptionDependent = true;

            this.sideEffect.AfterCreate(this.parameter, this.elementDefinition, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            this.valueSetService.Verify(x => x.CreateConcept(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualOption == this.option1.Iid), this.parameter, -1), Times.Exactly(1));
            this.valueSetService.Verify(x => x.CreateConcept(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualOption == this.option2.Iid), this.parameter, -1), Times.Exactly(1));
        }

        [Test]
        public void VerifyThatValueSetAreCreatedOptionState()
        {
            var originalThing = this.parameter.DeepClone<Thing>();
            this.parameter.ParameterType = this.cptParameterType.Iid;

            this.parameter.IsOptionDependent = true;
            this.parameter.StateDependence = this.actualList.Iid;

            this.sideEffect.AfterCreate(this.parameter, this.elementDefinition, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            this.valueSetService.Verify(x => x.CreateConcept(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualOption == this.option1.Iid && vs.ActualState == this.actualState1.Iid), this.parameter, -1), Times.Exactly(1));
            this.valueSetService.Verify(x => x.CreateConcept(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualOption == this.option1.Iid && vs.ActualState == this.actualState2.Iid), this.parameter, -1), Times.Exactly(1));
            this.valueSetService.Verify(x => x.CreateConcept(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualOption == this.option2.Iid && vs.ActualState == this.actualState1.Iid), this.parameter, -1), Times.Exactly(1));
            this.valueSetService.Verify(x => x.CreateConcept(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualOption == this.option2.Iid && vs.ActualState == this.actualState2.Iid), this.parameter, -1), Times.Exactly(1));
        }
    }
}
