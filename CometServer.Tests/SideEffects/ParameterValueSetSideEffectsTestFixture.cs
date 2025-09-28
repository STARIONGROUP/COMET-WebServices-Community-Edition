// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterValueSetSideEffectsTestFixture.cs" company="Starion Group S.A.">
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
    using System.Threading.Tasks;

    using CDP4Common;
    using CDP4Common.DTO;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Exceptions;

    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using NUnit.Framework;

    using Parameter = CDP4Common.DTO.Parameter;
    using ParameterOverride = CDP4Common.DTO.ParameterOverride;
    using ParameterOverrideValueSet = CDP4Common.DTO.ParameterOverrideValueSet;
    using ParameterSubscription = CDP4Common.DTO.ParameterSubscription;
    using ParameterSubscriptionValueSet = CDP4Common.DTO.ParameterSubscriptionValueSet;
    using ParameterValueSet = CDP4Common.DTO.ParameterValueSet;

    /// <summary>
    /// Tests the Parameter value set side effects.
    /// </summary>
    [TestFixture]
    public class ParameterValueSetSideEffectsTestFixture
    {
        private Mock<IParameterService> parameterService;
        private Mock<IParameterOverrideService> parameterOverrideService;
        private Mock<ISecurityContext> securityContext;

        [SetUp]
        public void Setup()
        {
            this.parameterService = new Mock<IParameterService>();
            this.parameterOverrideService = new Mock<IParameterOverrideService>();
            this.securityContext = new Mock<ISecurityContext>();
        }

        [Test]
        public async Task ParameterValueSetBeforeCreateAlwaysSkipsAsync()
        {
            var sideEffect = new ParameterValueSetSideEffect
            {
                ParameterService = this.parameterService.Object
            };

            var result = await sideEffect.BeforeCreateAsync(new ParameterValueSet(), new Parameter(), null, "partition", this.securityContext.Object);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ParameterValueSetBeforeDeleteThrows()
        {
            var sideEffect = new ParameterValueSetSideEffect();
            Assert.ThrowsAsync<InvalidOperationException>(() => sideEffect.BeforeDeleteAsync(new ParameterValueSet(), new Parameter(), null, "partition", this.securityContext.Object));
        }

        [Test]
        public void ParameterValueSetBeforeUpdateReturnsWhenNoSwitchKeys()
        {
            var sideEffect = new ParameterValueSetSideEffect
            {
                ParameterService = this.parameterService.Object
            };

            var rawUpdate = new ClasslessDTO();
            rawUpdate.Add("SomeOtherProperty", "value");

            Assert.DoesNotThrowAsync(() => sideEffect.BeforeUpdateAsync(new ParameterValueSet(), new Parameter(), null, "partition", this.securityContext.Object, rawUpdate));
        }

        [Test]
        public void ParameterValueSetBeforeUpdateThrowsForInvalidContainer()
        {
            var sideEffect = new ParameterValueSetSideEffect
            {
                ParameterService = this.parameterService.Object
            };

            var rawUpdate = new ClasslessDTO();
            rawUpdate.Add(ParameterSwitchKind.MANUAL.ToString(), "value");

            Assert.ThrowsAsync<ArgumentException>(() => sideEffect.BeforeUpdateAsync(new ParameterValueSet(), new Definition(), null, "partition", this.securityContext.Object, rawUpdate));
        }

        [Test]
        public async Task ParameterOverrideValueSetBeforeCreateAlwaysSkipsAsync()
        {
            var sideEffect = new ParameterOverrideValueSetSideEffect
            {
                ParameterService = this.parameterService.Object
            };

            var result = await sideEffect.BeforeCreateAsync(new ParameterOverrideValueSet(), new ParameterOverride(), null, "partition", this.securityContext.Object);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ParameterOverrideValueSetBeforeDeleteThrows()
        {
            var sideEffect = new ParameterOverrideValueSetSideEffect();
            Assert.ThrowsAsync<InvalidOperationException>(() => sideEffect.BeforeDeleteAsync(new ParameterOverrideValueSet(), new ParameterOverride(), null, "partition", this.securityContext.Object));
        }

        [Test]
        public void ParameterOverrideValueSetBeforeUpdateReturnsWhenNoSwitchKeys()
        {
            var sideEffect = new ParameterOverrideValueSetSideEffect
            {
                ParameterService = this.parameterService.Object
            };

            var rawUpdate = new ClasslessDTO();
            rawUpdate.Add("SomeOtherProperty", "value");

            Assert.DoesNotThrowAsync(() => sideEffect.BeforeUpdateAsync(new ParameterOverrideValueSet(), new ParameterOverride(), null, "partition", this.securityContext.Object, rawUpdate));
        }

        [Test]
        public void ParameterOverrideValueSetBeforeUpdateThrowsForInvalidContainer()
        {
            var sideEffect = new ParameterOverrideValueSetSideEffect
            {
                ParameterService = this.parameterService.Object
            };

            var rawUpdate = new ClasslessDTO();
            rawUpdate.Add(ParameterSwitchKind.MANUAL.ToString(), "value");

            Assert.ThrowsAsync<ArgumentException>(() => sideEffect.BeforeUpdateAsync(new ParameterOverrideValueSet(), new Definition(), null, "partition", this.securityContext.Object, rawUpdate));
        }

        [Test]
        public async Task ParameterSubscriptionValueSetBeforeCreateAlwaysSkipsAsync()
        {
            var sideEffect = new ParameterSubscriptionValueSetSideEffect
            {
                ParameterService = this.parameterService.Object,
                ParameterOverrideService = this.parameterOverrideService.Object
            };

            var result = await sideEffect.BeforeCreateAsync(new ParameterSubscriptionValueSet(), new ParameterSubscription(), null, "partition", this.securityContext.Object);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ParameterSubscriptionValueSetBeforeUpdateReturnsWhenNoSwitchKeys()
        {
            var sideEffect = new ParameterSubscriptionValueSetSideEffect
            {
                ParameterService = this.parameterService.Object,
                ParameterOverrideService = this.parameterOverrideService.Object
            };

            var rawUpdate = new ClasslessDTO();
            rawUpdate.Add("SomeOtherProperty", "value");

            Assert.DoesNotThrowAsync(() => sideEffect.BeforeUpdateAsync(new ParameterSubscriptionValueSet(), new ParameterSubscription(), null, "partition", this.securityContext.Object, rawUpdate));
        }

        [Test]
        public void ParameterSubscriptionValueSetBeforeUpdateThrowsWhenNoContainerFound()
        {
            this.parameterService.Setup(x => x.GetAsync(null, "partition", null, this.securityContext.Object))
                .ReturnsAsync(new List<Thing>());

            this.parameterOverrideService.Setup(x => x.GetAsync(null, "partition", null, this.securityContext.Object))
                .ReturnsAsync(new List<Thing>());

            var sideEffect = new ParameterSubscriptionValueSetSideEffect
            {
                ParameterService = this.parameterService.Object,
                ParameterOverrideService = this.parameterOverrideService.Object
            };

            var rawUpdate = new ClasslessDTO();
            rawUpdate.Add(ParameterSwitchKind.MANUAL.ToString(), "value");

            Assert.ThrowsAsync<ThingNotFoundException>(() => sideEffect.BeforeUpdateAsync(new ParameterSubscriptionValueSet(), new ParameterSubscription { Iid = Guid.NewGuid() }, null, "partition", this.securityContext.Object, rawUpdate));
        }

        [Test]
        public void ParameterSubscriptionValueSetBeforeDeleteThrows()
        {
            var sideEffect = new ParameterSubscriptionValueSetSideEffect();
            Assert.ThrowsAsync<InvalidOperationException>(() => sideEffect.BeforeDeleteAsync(new ParameterSubscriptionValueSet(), new ParameterSubscription(), null, "partition", this.securityContext.Object));
        }
    }
}
