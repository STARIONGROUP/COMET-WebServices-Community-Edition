// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UtilsTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using CDP4ServicesMessaging.Services.BackgroundMessageProducers;

    using CometServer.Configuration;
    using CometServer.Health;
    using CometServer.Helpers;
    using CometServer.Modules;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Protocol;
    using CometServer.Tasks;

    using Microsoft.Extensions.Logging;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the <see cref="CometServer.Services.Utils"/> class
    /// </summary>
    [TestFixture]
    public class UtilsTestFixture
    {
        private readonly SiteDirectory siteDir = new();

        private readonly EngineeringModelSetup modelSetup = new();

        private readonly IRequestUtils requestUtils = new RequestUtils { QueryParameters = new QueryParameters() };
        
        private readonly string mockedId = Guid.NewGuid().ToString();

        private Mock<ICometHasStartedService> cometHasStartedService;

        private Mock<ILoggerFactory> loggerFactory;

        private Mock<ITokenGeneratorService> tokenGeneratorService;

        private Mock<IAppConfigService> appConfigService;

        private Mock<ICdp4TransactionManager> transactionManager;

        private Mock<IBackgroundThingsMessageProducer> thingsMessageProducer;

        private Mock<ICometTaskService> cometTaskService;

        private Mock<IProcessor> SetupMockProcessor()
        {
            var mockedProcessor = new Mock<IProcessor>();

            // setup mocked method calls
            mockedProcessor.Setup(
                x => x.ValidateContainment(It.IsAny<List<Thing>>(), It.IsAny<string>(), It.IsAny<Guid>()));

            mockedProcessor.Setup(
                x => x.GetContainmentResource(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<ISecurityContext>()))
                           .ReturnsAsync(this.siteDir);

            mockedProcessor.Setup(
                x => x.GetResourceAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>()))
                           .ReturnsAsync([this.modelSetup]);

            mockedProcessor.SetupGet(x => x.RequestUtils).Returns(this.requestUtils);

            return mockedProcessor;
        }

        [SetUp]
        public void Setup()
        {
            this.loggerFactory = new Mock<ILoggerFactory>();
            this.appConfigService = new Mock<IAppConfigService>();
            
            this.transactionManager = new Mock<ICdp4TransactionManager>();
            
            this.tokenGeneratorService = new Mock<ITokenGeneratorService>();
            this.cometHasStartedService = new Mock<ICometHasStartedService>();
            this.thingsMessageProducer = new Mock<IBackgroundThingsMessageProducer>();
            this.cometTaskService = new Mock<ICometTaskService>();
        }

        [Test]
        public async Task VerifyOnlyResourceReturned()
        {
            var mockedProcessor = this.SetupMockProcessor();

            var siteDirectoryApi = new SiteDirectoryApi(this.appConfigService.Object, this.cometHasStartedService.Object, this.tokenGeneratorService.Object, this.loggerFactory.Object, this.thingsMessageProducer.Object, this.cometTaskService.Object);

            var result = await siteDirectoryApi.ProcessRequestPathAsync(this.requestUtils, this.transactionManager.Object, mockedProcessor.Object,  "SiteDirectory", "SiteDirectory", ["SiteDirectory", this.mockedId, "model", this.mockedId]);

            Assert.That(result.RequestedResources, Is.EqualTo(new[] { this.modelSetup}));
        }

        [Test]
        public async Task VerifyResourceWithContainmentReturned()
        {
            var mockedProcessor = this.SetupMockProcessor();

            // set query parameter override
            this.requestUtils.OverrideQueryParameters = new QueryParameters { IncludeAllContainers = true };

            var siteDirectoryApi = new SiteDirectoryApi(this.appConfigService.Object, this.cometHasStartedService.Object, this.tokenGeneratorService.Object, this.loggerFactory.Object, this.thingsMessageProducer.Object, this.cometTaskService.Object);

            var result = await siteDirectoryApi.ProcessRequestPathAsync(this.requestUtils, this.transactionManager.Object, mockedProcessor.Object, "SiteDirectory", "SiteDirectory", ["SiteDirectory", this.mockedId, "model", this.mockedId]);
            
            // reset query parameter override
            this.requestUtils.OverrideQueryParameters = null;
            Assert.That(result.RequestedResources, Is.EqualTo(new Thing[] { this.siteDir, this.modelSetup }));
        }
    }
}
