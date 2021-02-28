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

    using CDP4Common.DTO;
    using CDP4Common.Types;

    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the <see cref="ParameterSideEffect"/> class
    /// </summary>
    [TestFixture]
    public class ParameterOverrideSideEffectTestFixture
    {
        private Mock<ISecurityContext> securityContext;

        private NpgsqlTransaction npgsqlTransaction;

        private Mock<IParameterValueSetService> valueSetService;
        private Mock<IParameterOverrideValueSetService> parameterOverrideValueSetService;
        private Mock<IParameterSubscriptionService> parameterSubscriptionService;
        private Mock<IParameterService> parameterService;

        private Mock<IParameterSubscriptionValueSetService> subscriptionValueSetService;

        private Mock<IDefaultValueArrayFactory> defaultValueArrayFactory;

        private Parameter parameter;
        private ParameterValueSet valueset;
        private ParameterOverride parameterOverride;
        private ElementDefinition elementDefinition;
        private ElementUsage elementUsage;

        private ParameterOverrideSideEffect sideEffect;

        [SetUp]
        public void Setup()
        {
            this.securityContext = new Mock<ISecurityContext>();
            this.valueSetService = new Mock<IParameterValueSetService>();
            this.parameterOverrideValueSetService = new Mock<IParameterOverrideValueSetService>();
            this.parameterSubscriptionService = new Mock<IParameterSubscriptionService>();
            this.parameterService = new Mock<IParameterService>();
            this.subscriptionValueSetService = new Mock<IParameterSubscriptionValueSetService>();
            this.defaultValueArrayFactory = new Mock<IDefaultValueArrayFactory>();

            this.defaultValueArrayFactory.Setup(x => x.CreateDefaultValueArray(It.IsAny<Guid>())).Returns(new ValueArray<string>(new[] {"-"}));

            this.npgsqlTransaction = null;

            this.parameter = new Parameter(Guid.NewGuid(), 1);
            this.valueset = new ParameterValueSet(Guid.NewGuid(), 1);
            this.valueset.Manual = new ValueArray<string>(new [] {"1"});
            this.valueset.Computed = new ValueArray<string>(new [] {"1"});
            this.valueset.Formula = new ValueArray<string>(new [] {"1"});
            this.valueset.Published = new ValueArray<string>(new [] {"1"});
            this.valueset.Reference = new ValueArray<string>(new [] {"1"});
            this.parameter.ValueSet.Add(this.valueset.Iid);
            this.parameter.ParameterType = Guid.NewGuid();
            this.parameter.Owner = Guid.NewGuid();

            
            this.sideEffect = new ParameterOverrideSideEffect
                              {
                                  ParameterValueSetService = this.valueSetService.Object,
                                  ParameterOverrideValueSetService = this.parameterOverrideValueSetService.Object,
                                  ParameterService = this.parameterService.Object,
                                  ParameterSubscriptionService = this.parameterSubscriptionService.Object,
                                  ParameterOverrideValueSetFactory = new ParameterOverrideValueSetFactory(),
                                  ParameterSubscriptionValueSetFactory = new ParameterSubscriptionValueSetFactory(),
                                  ParameterSubscriptionValueSetService = this.subscriptionValueSetService.Object,
                                  DefaultValueArrayFactory = this.defaultValueArrayFactory.Object
                              };

            // prepare mock data
            this.elementDefinition = new ElementDefinition(Guid.NewGuid(), 1);
            this.elementDefinition.Parameter.Add(this.parameter.Iid);
            this.parameterOverride = new ParameterOverride(Guid.NewGuid(), 1) { Parameter = this.parameter.Iid };
            this.elementUsage = new ElementUsage(Guid.NewGuid(), 1) { ElementDefinition = this.elementDefinition.Iid, ParameterOverride = { this.parameterOverride.Iid }};

            this.parameterService.Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.Is<IEnumerable<Guid>>(y => y.Contains(this.parameter.Iid)), this.securityContext.Object))
                    .Returns(new List<Thing> { this.parameter });

            this.valueSetService.
                Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.Is<IEnumerable<Guid>>(y => y.Contains(this.valueset.Iid)), this.securityContext.Object)).
                Returns(new[] { this.valueset });
        }

        [Test]
        public void VerifyThatValueSetAreCreated()
        {
            var originalThing = this.parameterOverride.DeepClone<Thing>();
            this.sideEffect.AfterCreate(this.parameterOverride, this.elementUsage, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            this.parameterOverrideValueSetService.Verify(x => x.CreateConcept(null, "partition", It.IsAny<ParameterOverrideValueSet>(), this.parameterOverride, -1), Times.Exactly(1));
        }


        [Test]
        public void VerifyOwnerChangedSideEffects()
        {
            var domain1 = new DomainOfExpertise(Guid.NewGuid(), 0);
            var domain2 = new DomainOfExpertise(Guid.NewGuid(), 0);

            var subscription = new ParameterSubscription(Guid.NewGuid(), 1)
            {
                Owner = domain2.Iid
            };

            this.parameterOverride.ParameterSubscription.Add(subscription.Iid);
            this.parameterOverride.Owner = domain1.Iid;

            var originalThing = this.parameterOverride.DeepClone<Thing>();

            this.parameterOverride.Owner = domain2.Iid;

            this.parameterSubscriptionService.
                Setup(x => x.GetShallow(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.Is<IEnumerable<Guid>>(y => y.Contains(subscription.Iid)), this.securityContext.Object)).
                Returns(new[] {subscription});
            this.parameterSubscriptionService.
                Setup(x => x.DeleteConcept(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<ParameterSubscription>(), It.IsAny<ParameterOverride>())).Returns(true);

            this.sideEffect.AfterUpdate(this.parameterOverride, this.elementUsage, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);

            this.parameterSubscriptionService.Verify(x => x.DeleteConcept(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<ParameterSubscription>(), It.IsAny<ParameterOverride>()));
        }

        [Test]
        public void VerifySubscriptionsAreCreated()
        {
            var originalThing = this.parameterOverride.DeepClone<Thing>();
            this.parameterOverride.Owner = this.parameter.Owner;
            var subscription1 = new ParameterSubscription(Guid.NewGuid(), 0) { Owner = Guid.NewGuid() };
            var subscription2 = new ParameterSubscription(Guid.NewGuid(), 0) { Owner = Guid.NewGuid() };

            this.parameter.ParameterSubscription.Add(subscription1.Iid);
            this.parameter.ParameterSubscription.Add(subscription2.Iid);

            this.parameterSubscriptionService.Setup(x => x.GetShallow(null, It.IsAny<string>(), null, this.securityContext.Object)).Returns(new[] { subscription2, subscription1 });


            this.sideEffect.AfterCreate(this.parameterOverride, this.elementUsage, originalThing, this.npgsqlTransaction, "partition", this.securityContext.Object);


            this.parameterSubscriptionService.Verify(x => x.CreateConcept(null, It.IsAny<string>(), It.IsAny<ParameterSubscription>(), It.IsAny<ParameterOverride>(), It.IsAny<long>()), Times.Exactly(2));
            this.subscriptionValueSetService.Verify(x => x.CreateConcept(null, It.IsAny<string>(), It.IsAny<ParameterSubscriptionValueSet>(), It.IsAny<ParameterSubscription>(), It.IsAny<long>()), Times.Exactly(2));
        }
    }
}
