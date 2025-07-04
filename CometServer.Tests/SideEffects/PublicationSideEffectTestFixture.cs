﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicationSideEffectTestFixture.cs" company="Starion Group S.A.">
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

    using CDP4Common.DTO;
    using CDP4Common.Types;

    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    [TestFixture]
    public class PublicationSideEffectTestFixture
    {
        private PublicationSideEffect publicationSideEffect;
        private Mock<IParameterService> parameterService;
        private Mock<IParameterOverrideService> OverideService;
        private Mock<IParameterValueSetService> parameterValueSetService;
        private Mock<IParameterOverrideValueSetService> overrideValueSetService;
        private Mock<ICdp4TransactionManager> transactionManager;
        private Mock<ISecurityContext> securityContext;
        private Iteration iteration;
        private NpgsqlTransaction npgsqlTransaction;
        private Publication publication;

        [SetUp]
        public void Setup()
        {
            this.OverideService = new Mock<IParameterOverrideService>();
            this.parameterValueSetService = new Mock<IParameterValueSetService>();
            this.overrideValueSetService = new Mock<IParameterOverrideValueSetService>();
            this.parameterService = new Mock<IParameterService>();
            this.securityContext = new Mock<ISecurityContext>();
            this.transactionManager = new Mock<ICdp4TransactionManager>();

            this.publicationSideEffect = new PublicationSideEffect
            {
                ParameterService = this.parameterService.Object,
                ParameterOverrideService = this.OverideService.Object,
                ParameterValueSetService = this.parameterValueSetService.Object,
                ParameterOverrideValueSetService = this.overrideValueSetService.Object,
                TransactionManager = this.transactionManager.Object
            };

            this.npgsqlTransaction = null;
            var valuearray = new ValueArray<string>(["-"]);

            var option1 = new Option(Guid.NewGuid(), 1);
            var option2 = new Option(Guid.NewGuid(), 1);
            var orderedOption1 = new OrderedItem { V = option1 };
            var orderedOption2 = new OrderedItem { V = option2 };
            this.iteration = new Iteration(Guid.NewGuid(), 1);
            var actualFiniteState = new ActualFiniteState(Guid.NewGuid(), 1);

            var parameterValueSet1 = new ParameterValueSet(Guid.NewGuid(), 1)
            {
                ActualState = actualFiniteState.Iid,
                ActualOption = option1.Iid,
                Manual = valuearray,
                Computed = valuearray,
                Reference = valuearray
            };

            var parameterValueSet2 = new ParameterValueSet(Guid.NewGuid(), 1)
            {
                ActualState = actualFiniteState.Iid,
                ActualOption = option2.Iid,
                Manual = valuearray,
                Computed = valuearray,
                Reference = valuearray
            };

            var parameter = new Parameter(Guid.NewGuid(), 1)
            {
                IsOptionDependent = true,
                StateDependence = actualFiniteState.Iid
            };

            parameter.ValueSet.Add(parameterValueSet1.Iid);
            parameter.ValueSet.Add(parameterValueSet2.Iid);
            var actualFiniteStateList = new ActualFiniteStateList(Guid.NewGuid(), 1);
            actualFiniteStateList.ActualState.Add(actualFiniteState.Iid);
            var parameterOverride = new ParameterOverride(Guid.NewGuid(), 1) { Parameter = parameter.Iid };

            var overrideValueset1 = new ParameterOverrideValueSet(Guid.NewGuid(), 1)
            {
                ParameterValueSet = parameterValueSet1.Iid,
                Manual = valuearray,
                Computed = valuearray,
                Reference = valuearray
            };

            var overrideValueset2 = new ParameterOverrideValueSet(Guid.NewGuid(), 1)
            {
                ParameterValueSet = parameterValueSet2.Iid,
                Manual = valuearray,
                Computed = valuearray,
                Reference = valuearray
            };

            parameterOverride.ValueSet.Add(overrideValueset1.Iid);
            parameterOverride.ValueSet.Add(overrideValueset2.Iid);

            var publishedParametersAndOverridesIids = new List<Guid> { parameter.Iid, parameterOverride.Iid };
            this.publication = new Publication(Guid.NewGuid(), 1);
            this.publication.PublishedParameter.AddRange(publishedParametersAndOverridesIids);
            this.iteration.Publication.Add(this.publication.Iid);
            this.iteration.Option.Add(orderedOption1);
            this.iteration.Option.Add(orderedOption2);

            this.parameterService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), this.securityContext.Object)).ReturnsAsync([parameter]);
            this.OverideService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), this.securityContext.Object)).ReturnsAsync([parameterOverride]);
            this.parameterValueSetService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), this.securityContext.Object)).ReturnsAsync([parameterValueSet1, parameterValueSet2]);
            this.overrideValueSetService.Setup(x => x.GetShallowAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), this.securityContext.Object)).ReturnsAsync([overrideValueset1, overrideValueset2]);

            this.parameterValueSetService.Setup(x => x.UpdateConceptAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<ParameterValueSetBase>(), It.IsAny<ParameterOrOverrideBase>())).ReturnsAsync(true);
            this.overrideValueSetService.Setup(x => x.UpdateConceptAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<ParameterValueSetBase>(), It.IsAny<ParameterOrOverrideBase>())).ReturnsAsync(true);

            this.transactionManager.Setup(x => x.GetTransactionTimeAsync(It.IsAny<NpgsqlTransaction>())).ReturnsAsync(DateTime.Now);
        }

        [Test]
        public async Task VerifyBeforeCreate()
        {
            await this.publicationSideEffect.BeforeCreateAsync(this.publication, this.iteration, this.npgsqlTransaction, "EngineeringModel", this.securityContext.Object);

            // Check that the value sets of the parameters and parameterOverrides included in this publications are updated
            this.parameterValueSetService.Verify(x =>
                    x.UpdateConceptAsync(this.npgsqlTransaction, "EngineeringModel", It.IsAny<ParameterValueSetBase>(), It.IsAny<ParameterOrOverrideBase>()),
                Times.Exactly(2));

            this.overrideValueSetService.Verify(x =>
                    x.UpdateConceptAsync(this.npgsqlTransaction, "EngineeringModel", It.IsAny<ParameterValueSetBase>(), It.IsAny<ParameterOrOverrideBase>()),
                Times.Exactly(2));
        }
    }
}
