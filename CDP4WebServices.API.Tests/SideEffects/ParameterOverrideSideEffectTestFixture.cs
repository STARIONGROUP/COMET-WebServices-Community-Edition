// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterOverrideSideEffectTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.SideEffects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CDP4Common.DTO;
    using CDP4Common.Types;
    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Operations.SideEffects;
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

            
            this.sideEffect = new ParameterOverrideSideEffect
                              {
                                  ParameterValueSetService = this.valueSetService.Object,
                                  ParameterOverrideValueSetService = this.parameterOverrideValueSetService.Object,
                                  ParameterService = this.parameterService.Object,
                                  ParameterSubscriptionService = this.parameterSubscriptionService.Object,
                                  ParameterOverrideValueSetFactory = new ParameterOverrideValueSetFactory(),
                                  ParameterSubscriptionValueSetFactory = new ParameterSubscriptionValueSetFactory(),
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
    }
}
