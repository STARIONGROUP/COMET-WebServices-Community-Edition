// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterSideEffectTestFixture.cs" company="Starion Group S.A.">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Types;

    using CometServer.Authorization;
    using CometServer.Exceptions;
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
    public class ParameterSideEffectTestFixture
    {
        private Mock<ISecurityContext> securityContext;
        private NpgsqlTransaction npgsqlTransaction;
        private Mock<IOptionService> optionService;
        private Mock<IActualFiniteStateListService> actualFiniteStateListService;
        private Mock<IParameterValueSetService> valueSetService;
        private Mock<IIterationService> iterationService;
        private Mock<IParameterOverrideValueSetService> parameterOverrideValueSetService;
        private Mock<IParameterSubscriptionValueSetService> parameterSubscriptionValueSetService;
        private Mock<IParameterSubscriptionService> parameterSubscriptionService;
        private Mock<IParameterOverrideService> parameterOverrideService;
        private Mock<IElementUsageService> elementUsageService;
        private Mock<IDefaultValueArrayFactory> defaultValueArrayFactory;
        private Mock<IOldParameterContextProvider> OldParameterContextProvider;
        private Mock<ICachedReferenceDataService> cachedReferenceDataService;
        private Mock<IElementDefinitionService> elementDefinitionService;
        private Mock<IParameterService> parameterService;

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

        private readonly Guid existingNotQuantityKindParameterTypeGuid = Guid.NewGuid();
        private readonly Guid notExistingParameterTypeGuid = Guid.NewGuid();
        private readonly Guid scaleGuid = Guid.NewGuid();

        private ValueArray<string> compoundDefaultValueArray;
        private ValueArray<string> scalarDefaultValueArray;

        private const string ParameterTypeTestKey = "ParameterType";
        private const string ScaleTestKey = "Scale";

        private static readonly string[] SetValueArrayValues = ["set"];
        private static readonly string[] OverrideValueArrayValues = ["override"];

        [SetUp]
        public void Setup()
        {
            this.securityContext = new Mock<ISecurityContext>();
            this.optionService = new Mock<IOptionService>();
            this.actualFiniteStateListService = new Mock<IActualFiniteStateListService>();
            this.valueSetService = new Mock<IParameterValueSetService>();
            this.iterationService = new Mock<IIterationService>();
            this.parameterOverrideValueSetService = new Mock<IParameterOverrideValueSetService>();
            this.parameterSubscriptionValueSetService = new Mock<IParameterSubscriptionValueSetService>();
            this.parameterSubscriptionService = new Mock<IParameterSubscriptionService>();
            this.parameterOverrideService = new Mock<IParameterOverrideService>();
            this.elementUsageService = new Mock<IElementUsageService>();
            this.elementDefinitionService = new Mock<IElementDefinitionService>();
            this.parameterService = new Mock<IParameterService>();
            this.defaultValueArrayFactory = new Mock<IDefaultValueArrayFactory>();
            this.OldParameterContextProvider = new Mock<IOldParameterContextProvider>();
            this.cachedReferenceDataService = new Mock<ICachedReferenceDataService>();

            this.organizationalParticipationResolverService = new Mock<IOrganizationalParticipationResolverService>();
            this.organizationalParticipationResolverService.Setup(x => x.ValidateCreateOrganizationalParticipationAsync(It.IsAny<Thing>(), It.IsAny<Thing>(), It.IsAny<ISecurityContext>(), this.npgsqlTransaction, It.IsAny<string>()));

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
                ElementUsageService = this.elementUsageService.Object,
                OptionService = this.optionService.Object,
                DefaultValueArrayFactory = this.defaultValueArrayFactory.Object,
                ParameterValueSetFactory = new ParameterValueSetFactory(),
                ParameterOverrideValueSetFactory = new ParameterOverrideValueSetFactory(),
                ParameterSubscriptionValueSetFactory = new ParameterSubscriptionValueSetFactory(),
                OldParameterContextProvider = this.OldParameterContextProvider.Object,
                OrganizationalParticipationResolverService = this.organizationalParticipationResolverService.Object,
                CachedReferenceDataService = this.cachedReferenceDataService.Object,
                ElementDefinitionService = this.elementDefinitionService.Object,
                ParameterService = this.parameterService.Object
            };

            // prepare mock data
            this.elementDefinition = new ElementDefinition(Guid.NewGuid(), 1);
            this.elementDefinition.Parameter.Add(this.parameter.Iid);

            this.parameterOverride = new ParameterOverride(Guid.NewGuid(), 1)
            {
                Parameter = this.parameter.Iid
            };

            this.elementUsage = new ElementUsage(Guid.NewGuid(), 1) { ElementDefinition = this.elementDefinition.Iid, ParameterOverride = { this.parameterOverride.Iid } };

            this.iterationService.Setup(x => x.GetActiveIterationAsync(null, "partition", this.securityContext.Object))
                .ReturnsAsync(this.iteration);

            this.actualFiniteStateListService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), "partition", It.Is<IEnumerable<Guid>>(y => y.Contains(this.actualList.Iid)), this.securityContext.Object))
                .ReturnsAsync(new List<Thing> { this.actualList });

            var parameterTypeDictionary = new Dictionary<Guid, ParameterType>
            {
                { this.cptParameterType.Iid, this.cptParameterType },
                { this.boolPt.Iid, this.boolPt }
            };

            this.cachedReferenceDataService.Setup(x => x.QueryParameterTypesAsync(this.npgsqlTransaction, this.securityContext.Object))
                .ReturnsAsync(parameterTypeDictionary);

            var parameterTypeComponentDictionary = new Dictionary<Guid, ParameterTypeComponent>
            {
                { this.cpt1.Iid, this.cpt1 },
                { this.cpt2.Iid, this.cpt2 }
            };

            this.cachedReferenceDataService.Setup(x => x.QueryParameterTypeComponentsAsync(this.npgsqlTransaction, this.securityContext.Object))
                .ReturnsAsync(parameterTypeComponentDictionary);

            this.parameterOverrideService.Setup(x => x.GetShallowAsync(this.npgsqlTransaction, "partition", null, this.securityContext.Object))
                .ReturnsAsync(new List<Thing> { this.parameterOverride });

            this.elementUsageService.Setup(x => x.GetShallowAsync(this.npgsqlTransaction, "partition", null, this.securityContext.Object))
                .ReturnsAsync(new List<Thing> { this.elementUsage });

            this.scalarDefaultValueArray = new ValueArray<string>(new List<string> { "-" });
            this.compoundDefaultValueArray = new ValueArray<string>(new List<string> { "-", "-" });

            this.defaultValueArrayFactory.Setup(x => x.CreateDefaultValueArray(this.cptParameterType.Iid))
                .Returns(this.compoundDefaultValueArray);

            this.defaultValueArrayFactory.Setup(x => x.CreateDefaultValueArray(this.boolPt.Iid))
                .Returns(this.scalarDefaultValueArray);

            this.OldParameterContextProvider.Setup(x => x.GetsourceValueSet(It.IsAny<Guid?>(), It.IsAny<Guid?>())).Returns((ParameterValueSet)null);
        }

        [Test]
        public void VerifyBeforeCreateSideEffectThrowsExceptionForNotExistingParameterType()
        {
            this.parameter.ParameterType = this.notExistingParameterTypeGuid;

            Assert.ThrowsAsync<ArgumentException>(
                () => this.sideEffect.BeforeCreateAsync(this.parameter, this.elementDefinition, this.npgsqlTransaction, "partition", this.securityContext.Object));
        }

        [Test]
        public void VerifyBeforeCreateSideEffectThrowsExceptionForNotQuantityKindScaleNotBeingNull()
        {
            this.parameter.ParameterType = this.existingNotQuantityKindParameterTypeGuid;
            this.parameter.Scale = this.scaleGuid;

            Assert.ThrowsAsync<ArgumentException>(
                () => this.sideEffect.BeforeCreateAsync(this.parameter, this.elementDefinition, this.npgsqlTransaction, "partition", this.securityContext.Object));
        }

        [Test]
        public void VerifyBeforeUpdateSideEffectThrowsExceptionForNotExistingParameterType()
        {
            var rawUpdateInfo = new ClasslessDTO { { ParameterTypeTestKey, this.notExistingParameterTypeGuid } };

            Assert.ThrowsAsync<ArgumentException>(
                () => this.sideEffect.BeforeUpdateAsync(this.parameter, this.elementDefinition, this.npgsqlTransaction, "partition", this.securityContext.Object, rawUpdateInfo));
        }

        [Test]
        public void VerifyBeforeUpdateSideEffectThrowsExceptionForNotQuantityKindScaleNotBeingNull()
        {
            var rawUpdateInfo = new ClasslessDTO { { ParameterTypeTestKey, this.existingNotQuantityKindParameterTypeGuid }, { ScaleTestKey, this.scaleGuid } };
            this.parameter.Scale = this.scaleGuid;

            Assert.ThrowsAsync<ArgumentException>(
                () => this.sideEffect.BeforeUpdateAsync(this.parameter, this.elementDefinition, this.npgsqlTransaction, "partition", this.securityContext.Object, rawUpdateInfo));

            rawUpdateInfo = new ClasslessDTO { { ParameterTypeTestKey, this.existingNotQuantityKindParameterTypeGuid } };

            Assert.ThrowsAsync<ArgumentException>(
                () => this.sideEffect.BeforeUpdateAsync(this.parameter, this.elementDefinition, this.npgsqlTransaction, "partition", this.securityContext.Object, rawUpdateInfo));

            rawUpdateInfo = new ClasslessDTO { { ParameterTypeTestKey, this.existingNotQuantityKindParameterTypeGuid }, { ScaleTestKey, this.scaleGuid } };
            this.parameter.Scale = null;

            Assert.ThrowsAsync<ArgumentException>(
                () => this.sideEffect.BeforeUpdateAsync(this.parameter, this.elementDefinition, this.npgsqlTransaction, "partition", this.securityContext.Object, rawUpdateInfo));
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

            this.parameterSubscriptionService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), this.securityContext.Object))
                .ReturnsAsync(new List<Thing> { domain2Subscription });

            this.parameterSubscriptionService.Setup(x => x.DeleteConceptAsync(It.IsAny<NpgsqlTransaction>(),
                It.IsAny<string>(), It.IsAny<Thing>(), It.IsAny<Thing>())).ReturnsAsync(true);

            this.sideEffect.AfterUpdateAsync(updatedParameter, this.elementDefinition, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            // Check that the subscription owned by domain2 is deleted since domain2 is now the owner of the parameter
            this.parameterSubscriptionService.Verify(x => x.DeleteConceptAsync(this.npgsqlTransaction, "partition", It.IsAny<ParameterSubscription>(), It.IsAny<Parameter>()),
                Times.Once);

            // Since that is the only subscription, no updates are performed on ParameterSubscriptionValueSet
            this.parameterSubscriptionValueSetService.Verify(x => x.CreateConceptAsync(this.npgsqlTransaction, "partition", It.IsAny<ParameterSubscriptionValueSet>(), It.IsAny<ParameterSubscription>(), -1),
                Times.Never);

            // Check that since AllowDifferentOwnerOfOverride is True the owner of the parameterOverrides are not updated
            this.parameterOverrideService.Verify(x => x.UpdateConceptAsync(this.npgsqlTransaction, "partition", It.IsAny<ParameterOverride>(), It.IsAny<ElementUsage>()),
                Times.Never);

            var updatedParameter1 = new Parameter(this.parameter.Iid, 0) { ParameterType = this.boolPt.Iid, Owner = domain2.Iid, AllowDifferentOwnerOfOverride = false };
            updatedParameter1.ParameterSubscription.Add(domain2Subscription.Iid);
            this.sideEffect.AfterUpdateAsync(updatedParameter1, this.elementDefinition, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            // Check that since AllowDifferentOwnerOfOverride is False the owner of the parameterOverride is updated
            this.parameterOverrideService.Verify(x => x.UpdateConceptAsync(this.npgsqlTransaction, "partition", It.IsAny<ParameterOverride>(), It.IsAny<ElementUsage>()),
                Times.Once);
        }

        [Test]
        public void VerifyThatAfterUpdateStateChangesWorks()
        {
            this.valueSetService.Setup(
                x => x.DeleteConceptAsync(null, "partition", It.IsAny<ParameterValueSet>(), this.parameter)).ReturnsAsync(true);

            this.parameter.ParameterType = this.cptParameterType.Iid;

            this.parameter.StateDependence = null;
            var valueset = new ParameterValueSet(Guid.NewGuid(), 0);
            this.parameter.ValueSet.Add(valueset.Iid);

            var originalThing = this.parameter.DeepClone<Thing>();

            var updatedParameter = new Parameter(this.parameter.Iid, 0) { StateDependence = this.actualList.Iid };
            updatedParameter.ValueSet.Add(valueset.Iid);
            updatedParameter.ParameterType = this.cptParameterType.Iid;

            this.sideEffect.AfterUpdateAsync(updatedParameter, this.elementDefinition, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            this.valueSetService.Verify(x => x.CreateConceptAsync(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualState == this.actualState1.Iid), updatedParameter, -1), Times.Exactly(1));
            this.valueSetService.Verify(x => x.CreateConceptAsync(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualState == this.actualState2.Iid), updatedParameter, -1), Times.Exactly(1));
        }

        [Test]
        public void VerifyThatAfterUpdateUpdateTheOVerrideAndSubscription()
        {
            var valueset = new ParameterValueSet(Guid.NewGuid(), 0)
            {
                Manual = new ValueArray<string>(SetValueArrayValues),
                Published = new ValueArray<string>(SetValueArrayValues),
                Computed = new ValueArray<string>(SetValueArrayValues),
                ValueSwitch = ParameterSwitchKind.REFERENCE
            };

            this.parameter.ValueSet.Add(valueset.Iid);
            this.OldParameterContextProvider.Setup(x => x.GetsourceValueSet(null, It.IsAny<Guid?>())).Returns(valueset);

            var overrideValueSet = new ParameterOverrideValueSet(Guid.NewGuid(), 1)
            {
                ParameterValueSet = valueset.Iid,
                Manual = new ValueArray<string>(OverrideValueArrayValues),
                Published = new ValueArray<string>(OverrideValueArrayValues),
                Computed = new ValueArray<string>(OverrideValueArrayValues),
                ValueSwitch = ParameterSwitchKind.REFERENCE
            };

            this.parameterOverride.ValueSet.Add(overrideValueSet.Iid);

            var subscription1 = new ParameterSubscription(Guid.NewGuid(), 1);
            var subscription2 = new ParameterSubscription(Guid.NewGuid(), 2);

            var subscription1ValueSet = new ParameterSubscriptionValueSet(Guid.NewGuid(), 1)
            {
                Manual = new ValueArray<string>(["sub1"]),
                SubscribedValueSet = valueset.Iid
            };

            var subscription2ValueSet = new ParameterSubscriptionValueSet(Guid.NewGuid(), 1)
            {
                Manual = new ValueArray<string>(["sub2"]),
                SubscribedValueSet = overrideValueSet.Iid
            };

            subscription1.ValueSet.Add(subscription1ValueSet.Iid);
            subscription2.ValueSet.Add(subscription2ValueSet.Iid);

            this.parameterOverride.ParameterSubscription.Add(subscription2.Iid);
            this.parameter.ParameterSubscription.Add(subscription1.Iid);

            this.parameterOverrideService.Setup(
                    x => x.GetShallowAsync(this.npgsqlTransaction, "partition", null, this.securityContext.Object))
                .ReturnsAsync(new List<Thing> { this.parameterOverride });

            this.parameterSubscriptionService.Setup(x =>
                    x.GetShallowAsync(this.npgsqlTransaction, "partition",
                        It.Is<IEnumerable<Guid>>(g => g.Contains(subscription1.Iid) && g.Contains(subscription2.Iid)),
                        this.securityContext.Object))
                .ReturnsAsync(new List<Thing> { subscription1, subscription2 });

            this.valueSetService.Setup(x => x.DeleteConceptAsync(null, "partition", It.IsAny<ParameterValueSet>(), this.parameter))
                .ReturnsAsync(true);

            this.parameterOverrideValueSetService
                .Setup(x => x.GetShallowAsync(null, "partition", It.Is<IEnumerable<Guid>>(y => y.Contains(overrideValueSet.Iid)), this.securityContext.Object))
                .ReturnsAsync([overrideValueSet]);

            this.parameterSubscriptionValueSetService
                .Setup(x => x.GetShallowAsync(null, "partition", It.Is<IEnumerable<Guid>>(y => y.Contains(subscription1ValueSet.Iid)), this.securityContext.Object))
                .ReturnsAsync([subscription1ValueSet]);

            this.parameterSubscriptionValueSetService
                .Setup(x => x.GetShallowAsync(null, "partition", It.Is<IEnumerable<Guid>>(y => y.Contains(subscription2ValueSet.Iid)), this.securityContext.Object))
                .ReturnsAsync([subscription2ValueSet]);

            this.parameter.ParameterType = this.cptParameterType.Iid;
            this.parameter.StateDependence = null;

            var originalThing = this.parameter.DeepClone<Thing>();

            var updatedParameter = new Parameter(this.parameter.Iid, 0) { StateDependence = this.actualList.Iid };
            updatedParameter.ValueSet.Add(valueset.Iid);
            updatedParameter.ParameterType = this.cptParameterType.Iid;
            updatedParameter.ParameterSubscription.Add(subscription1.Iid);

            this.sideEffect.AfterUpdateAsync(updatedParameter, this.elementDefinition, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            this.valueSetService.Verify(x => x.CreateConceptAsync(this.npgsqlTransaction, "partition", It.Is<ParameterValueSet>(y => y.Manual.Contains("set")), updatedParameter, -1),
                Times.Exactly(2));

            this.parameterOverrideValueSetService.Verify(x => x.CreateConceptAsync(this.npgsqlTransaction, "partition", It.Is<ParameterOverrideValueSet>(y => y.Manual.Contains("override")), this.parameterOverride, -1),
                Times.Exactly(2));

            this.parameterSubscriptionValueSetService.Verify(x => x.CreateConceptAsync(this.npgsqlTransaction, "partition", It.Is<ParameterSubscriptionValueSet>(y => y.Manual.Contains("sub1")), subscription1, -1),
                Times.Exactly(2));

            this.parameterSubscriptionValueSetService.Verify(x => x.CreateConceptAsync(this.npgsqlTransaction, "partition", It.Is<ParameterSubscriptionValueSet>(y => y.Manual.Contains("sub2")), subscription2, -1),
                Times.Exactly(2));
        }

        [Test]
        public void VerifyThatValueSetAreCreatedCompoundNoOptionNoState()
        {
            this.parameter.ParameterType = this.cptParameterType.Iid;

            //Updated version contains parameter
            var updatedElementDefinition = this.elementDefinition.DeepClone<ElementDefinition>();

            //original should not contains the parameter
            this.elementDefinition.Parameter.Clear();

            //Create a clone from this.parameter
            var originalThing = this.parameter.DeepClone<Thing>();

            //Setup ElementDefinitionService to retrieve updated ED having the parameter
            this.elementDefinitionService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).ReturnsAsync(new List<Thing> { updatedElementDefinition });

            //Setup ParameterService to return the newly created parameter
            this.parameterService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).ReturnsAsync(new List<Thing> { originalThing });

            this.sideEffect.AfterCreateAsync(this.parameter, this.elementDefinition, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            this.valueSetService.Verify(x => x.CreateConceptAsync(null, "partition", It.Is<ParameterValueSet>(vs => vs.Manual.Count == this.cptParameterType.Component.Count), this.parameter, -1), Times.Exactly(1));
        }

        [Test]
        public void VerifyThatValueSetAreCreatedNoOptionWithState()
        {
            this.parameter.ParameterType = this.cptParameterType.Iid;

            //Updated version contains parameter
            var updatedElementDefinition = this.elementDefinition.DeepClone<ElementDefinition>();

            //original should not contains the parameter
            this.elementDefinition.Parameter.Clear();

            //Create a clone from this.parameter
            var originalThing = this.parameter.DeepClone<Thing>();

            this.parameter.StateDependence = this.actualList.Iid;

            //Setup ElementDefinitionService to retrieve updated ED having the parameter
            this.elementDefinitionService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).ReturnsAsync(new List<Thing> { updatedElementDefinition });

            //Setup ParameterService to return the newly created parameter
            this.parameterService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).ReturnsAsync(new List<Thing> { originalThing });

            this.sideEffect.AfterCreateAsync(this.parameter, this.elementDefinition, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            this.valueSetService.Verify(x => x.CreateConceptAsync(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualState == this.actualState1.Iid), this.parameter, -1), Times.Exactly(1));
            this.valueSetService.Verify(x => x.CreateConceptAsync(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualState == this.actualState2.Iid), this.parameter, -1), Times.Exactly(1));
        }

        [Test]
        public void VerifyThatValueSetAreCreatedOptionNoState()
        {
            this.parameter.ParameterType = this.cptParameterType.Iid;

            //Updated version contains parameter
            var updatedElementDefinition = this.elementDefinition.DeepClone<ElementDefinition>();

            //original should not contains the parameter
            this.elementDefinition.Parameter.Clear();

            //Create a clone from this.parameter
            var originalThing = this.parameter.DeepClone<Thing>();

            this.parameter.IsOptionDependent = true;

            //Setup ElementDefinitionService to retrieve updated ED having the parameter
            this.elementDefinitionService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).ReturnsAsync(new List<Thing> { updatedElementDefinition });

            //Setup ParameterService to return the newly created parameter
            this.parameterService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).ReturnsAsync(new List<Thing> { originalThing });

            this.sideEffect.AfterCreateAsync(this.parameter, this.elementDefinition, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            this.valueSetService.Verify(x => x.CreateConceptAsync(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualOption == this.option1.Iid), this.parameter, -1), Times.Exactly(1));
            this.valueSetService.Verify(x => x.CreateConceptAsync(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualOption == this.option2.Iid), this.parameter, -1), Times.Exactly(1));
        }

        [Test]
        public void VerifyThatValueSetAreCreatedOptionState()
        {
            this.parameter.ParameterType = this.cptParameterType.Iid;

            //Updated version contains parameter
            var updatedElementDefinition = this.elementDefinition.DeepClone<ElementDefinition>();

            //original should not contains the parameter
            this.elementDefinition.Parameter.Clear();

            //Create a clone from this.parameter
            var originalThing = this.parameter.DeepClone<Thing>();

            this.parameter.IsOptionDependent = true;
            this.parameter.StateDependence = this.actualList.Iid;

            //Setup ElementDefinitionService to retrieve updated ED having the parameter
            this.elementDefinitionService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).ReturnsAsync(new List<Thing> { updatedElementDefinition });

            //Setup ParameterService to return the newly created parameter
            this.parameterService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).ReturnsAsync(new List<Thing> { originalThing });

            this.sideEffect.AfterCreateAsync(this.parameter, this.elementDefinition, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            this.valueSetService.Verify(x => x.CreateConceptAsync(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualOption == this.option1.Iid && vs.ActualState == this.actualState1.Iid), this.parameter, -1), Times.Exactly(1));
            this.valueSetService.Verify(x => x.CreateConceptAsync(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualOption == this.option1.Iid && vs.ActualState == this.actualState2.Iid), this.parameter, -1), Times.Exactly(1));
            this.valueSetService.Verify(x => x.CreateConceptAsync(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualOption == this.option2.Iid && vs.ActualState == this.actualState1.Iid), this.parameter, -1), Times.Exactly(1));
            this.valueSetService.Verify(x => x.CreateConceptAsync(null, "partition", It.Is<ParameterValueSet>(vs => vs.ActualOption == this.option2.Iid && vs.ActualState == this.actualState2.Iid), this.parameter, -1), Times.Exactly(1));
        }

        [Test]
        public void UpdatingParameterTypeToExistingParameterTypeFails()
        {
            var newParameter = this.parameter.DeepClone<Parameter>();
            newParameter.Iid = Guid.NewGuid();
            newParameter.ParameterType = this.cptParameterType.Iid;

            this.elementDefinition.Parameter.Add(newParameter.Iid);
            var updatedElementDefinition = this.elementDefinition.DeepClone<ElementDefinition>();

            //Setup ElementDefinitionService to retrieve updated ED having the parameter
            this.elementDefinitionService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).ReturnsAsync(new List<Thing> { updatedElementDefinition });

            //Setup ParameterService to return the newly created parameter
            this.parameterService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.Is<IEnumerable<Guid>>(y => y.Count() == 1 && y.Contains(newParameter.Iid)), It.IsAny<ISecurityContext>())).ReturnsAsync(new List<Thing> { newParameter });

            //Setup ParameterService to return the existing parameter
            this.parameterService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.Is<IEnumerable<Guid>>(y => y.Count() == 2), It.IsAny<ISecurityContext>())).ReturnsAsync(new List<Thing> { this.parameter, newParameter });

            Assert.That(() => this.sideEffect.AfterUpdateAsync(newParameter, this.elementDefinition, this.parameter, this.npgsqlTransaction, "partition", this.securityContext.Object), Throws.TypeOf<BadRequestException>().With.Message.Contain("already contains"));
        }

        [Test]
        public void AddingParameterTypeHavingExistingParameterTypeFails()
        {
            this.parameter.ParameterType = this.cptParameterType.Iid;

            var newParameter = this.parameter.DeepClone<Parameter>();
            newParameter.Iid = Guid.NewGuid();

            var updatedElementDefinition = this.elementDefinition.DeepClone<ElementDefinition>();
            updatedElementDefinition.Parameter.Add(newParameter.Iid);

            //Setup ElementDefinitionService to retrieve updated ED having the parameter
            this.elementDefinitionService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).ReturnsAsync(new List<Thing> { updatedElementDefinition });

            //Setup ParameterService to return the newly created parameter
            this.parameterService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.Is<IEnumerable<Guid>>(y => y.Contains(newParameter.Iid)), It.IsAny<ISecurityContext>())).ReturnsAsync(new List<Thing> { newParameter });

            //Setup ParameterService to return the existing parameter
            this.parameterService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.Is<IEnumerable<Guid>>(y => y.Contains(this.parameter.Iid)), It.IsAny<ISecurityContext>())).ReturnsAsync(new List<Thing> { this.parameter });

            Assert.That(() => this.sideEffect.AfterCreateAsync(newParameter, this.elementDefinition, this.parameter, this.npgsqlTransaction, "partition", this.securityContext.Object), Throws.TypeOf<BadRequestException>().With.Message.Contain("already contains"));
        }

        [Test]
        public void AddingMultipleParameterTypeHavingSameParameterTypeFails()
        {
            this.elementDefinition.Parameter.Clear();

            this.parameter.ParameterType = this.cptParameterType.Iid;

            var newParameter = this.parameter.DeepClone<Parameter>();
            newParameter.Iid = Guid.NewGuid();

            var updatedElementDefinition = this.elementDefinition.DeepClone<ElementDefinition>();
            updatedElementDefinition.Parameter.Add(this.parameter.Iid);
            updatedElementDefinition.Parameter.Add(newParameter.Iid);

            //Setup ElementDefinitionService to retrieve updated ED having the parameter
            this.elementDefinitionService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).ReturnsAsync(new List<Thing> { updatedElementDefinition });

            //Setup ParameterService to return the newly created parameter
            this.parameterService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>())).ReturnsAsync(new List<Thing> { newParameter, this.parameter });

            Assert.That(() => this.sideEffect.AfterCreateAsync(newParameter, this.elementDefinition, this.parameter, this.npgsqlTransaction, "partition", this.securityContext.Object), Throws.TypeOf<BadRequestException>().With.Message.Contain("multiple times"));
        }
    }
}
