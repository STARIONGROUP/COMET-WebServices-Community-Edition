// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterSubscriptionSideEffectTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
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

    using CDP4Common.DTO;
    using CDP4Common.Exceptions;
    using CDP4Common.Types;

    using CometServer.Authorization;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Microsoft.Extensions.Logging;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="ParameterSubscriptionSideEffect" />
    /// </summary>
    [TestFixture]
    public class ParameterSubscriptionSideEffectTestFixture
    {
        private Mock<ILogger<ParameterSubscriptionSideEffect>> logger;
        private Mock<ISecurityContext> securityContext;
        private Mock<IParameterSubscriptionValueSetService> parameterSubscriptionValueSetService;
        private Mock<IParameterValueSetService> parameterValueSetService;
        private Mock<IParameterOverrideValueSetService> parameterValueSetOverrideService;
        private Mock<IParameterSubscriptionService> parameterSubscriptionService;
        private Mock<IParameterService> parameterService;
        private Mock<IParameterOverrideService> parameterOverrideService;
        private Mock<IOrganizationalParticipationResolverService> organizationalParticipationResolverService;

        private NpgsqlTransaction npgsqlTransaction;
        private ParameterSubscriptionSideEffect sideEffect;

        [SetUp]
        public void Setup()
        {
            this.logger = new Mock<ILogger<ParameterSubscriptionSideEffect>>();
            this.securityContext = new Mock<ISecurityContext>();
            this.npgsqlTransaction = null;
            this.parameterValueSetService = new Mock<IParameterValueSetService>();
            this.parameterValueSetOverrideService = new Mock<IParameterOverrideValueSetService>();
            this.parameterSubscriptionService = new Mock<IParameterSubscriptionService>();
            this.parameterService = new Mock<IParameterService>();
            this.parameterOverrideService = new Mock<IParameterOverrideService>();

            this.organizationalParticipationResolverService = new Mock<IOrganizationalParticipationResolverService>();
            this.organizationalParticipationResolverService.Setup(x => x.ValidateCreateOrganizationalParticipationAsync(It.IsAny<Thing>(), It.IsAny<Thing>(), It.IsAny<ISecurityContext>(), this.npgsqlTransaction, It.IsAny<string>()));

            this.parameterSubscriptionValueSetService = new Mock<IParameterSubscriptionValueSetService>();

            this.parameterSubscriptionValueSetService.Setup(
                x => x.CreateConceptAsync(
                    It.IsAny<NpgsqlTransaction>(),
                    It.IsAny<string>(),
                    It.IsAny<ParameterSubscriptionValueSet>(),
                    It.IsAny<ParameterSubscription>(),
                    It.IsAny<long>())).Returns(true);

            this.sideEffect = new ParameterSubscriptionSideEffect
            {
                ParameterSubscriptionValueSetService = this.parameterSubscriptionValueSetService.Object,
                ParameterValueSetService = this.parameterValueSetService.Object,
                ParameterOverrideValueSetService = this.parameterValueSetOverrideService.Object,
                DefaultValueArrayFactory = new DefaultValueArrayFactory(),
                ParameterSubscriptionService = this.parameterSubscriptionService.Object,
                ParameterService = this.parameterService.Object,
                ParameterOverrideService = this.parameterOverrideService.Object,
                OrganizationalParticipationResolverService = this.organizationalParticipationResolverService.Object,
                Logger = this.logger.Object
            };
        }
        
        [Test]
        public void CheckThatMultipleSubscriptionCannotBeCreatedForSameOwner()
        {
            var subOwnerGuid = Guid.NewGuid();
            var existingSub = new ParameterSubscription(Guid.NewGuid(), 1) { Owner = subOwnerGuid };
            var parameterSubscription = new ParameterSubscription(Guid.NewGuid(), 1) { Owner = subOwnerGuid };

            var parameter = new Parameter(Guid.NewGuid(), 1) { Owner = Guid.NewGuid() };
            parameter.ValueSet = new List<Guid> { Guid.NewGuid() };
            parameter.ParameterSubscription.Add(existingSub.Iid);

            this.parameterService
                .Setup(x => x.GetShallowAsync(this.npgsqlTransaction, "partition", It.Is<IEnumerable<Guid>>(i => i.Single() == parameter.Iid), this.securityContext.Object))
                .Returns(new Thing[] { parameter });

            this.parameterSubscriptionService.Setup(x => x.GetShallowAsync(this.npgsqlTransaction, "partition", It.Is<IEnumerable<Guid>>(y => y.Contains(existingSub.Iid)), this.securityContext.Object)).Returns(new List<Thing> { existingSub });

            Assert.That(this.sideEffect.BeforeCreateAsync(parameterSubscription, parameter, this.npgsqlTransaction, "partition", this.securityContext.Object), Is.False);
        }

        [Test]
        public void VerifyThatTheWhenOwnerOfTheParameterAndSubscriptionAreEqualExceptionIsThrown()
        {
            var owner = Guid.NewGuid();
            var parameterSubscription = new ParameterSubscription(Guid.NewGuid(), 1) { Owner = owner };

            var parameter = new Parameter(Guid.NewGuid(), 1) { Owner = owner };
            parameter.ValueSet.Add(Guid.NewGuid());
            parameter.ParameterSubscription.Add(parameterSubscription.Iid);

            this.parameterService
                .Setup(x => x.GetShallowAsync(this.npgsqlTransaction, "partition", It.Is<IEnumerable<Guid>>(i => i.Single() == parameter.Iid), this.securityContext.Object))
                .Returns(new Thing[] { parameter });

            Assert.Throws<Cdp4ModelValidationException>(() => this.sideEffect.BeforeCreateAsync(parameterSubscription, parameter, this.npgsqlTransaction, "partition", this.securityContext.Object));

            Assert.Throws<Cdp4ModelValidationException>(() => this.sideEffect.BeforeUpdateAsync(parameterSubscription, parameter, this.npgsqlTransaction, "partition", this.securityContext.Object, null));
        }

        [Test]
        public void VerifyThatWhenAParameterSubscriptionIsPostedValueSetsAreCreated()
        {
            this.parameterValueSetService
                .Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), "partition", It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>()))
                .Returns(new[] { new ParameterValueSet(Guid.NewGuid(), 0) { Manual = new ValueArray<string>(new[] { "1", "2" }) } });

            var parameterSubscription = new ParameterSubscription(Guid.NewGuid(), 1) { Owner = Guid.NewGuid() };
            var originalparameterSubscription = new ParameterSubscription(parameterSubscription.Iid, 1);

            var parameter = new Parameter(Guid.NewGuid(), 1) { Owner = Guid.NewGuid() };
            parameter.ValueSet = new List<Guid> { Guid.NewGuid() };

            this.parameterService
                .Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), "partition", It.Is<IEnumerable<Guid>>(enu => enu.Contains(parameter.Iid)), It.IsAny<ISecurityContext>()))
                .Returns(new[] { parameter });

            this.sideEffect.AfterCreateAsync(parameterSubscription, parameter, originalparameterSubscription, this.npgsqlTransaction, "partition", this.securityContext.Object);

            this.parameterSubscriptionValueSetService.Verify(x => x.CreateConceptAsync(this.npgsqlTransaction, "partition", It.Is<ParameterSubscriptionValueSet>(s => s.Manual.Count == 2), It.IsAny<ParameterSubscription>(), It.IsAny<long>()), Times.Once);
        }
    }
}
