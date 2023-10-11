// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UtilsTestFixture.cs" company="RHEA System S.A.">
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

namespace CometServer.Tests
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CDP4JsonSerializer;

    using CometServer.Authorization;
    using CometServer.Configuration;
    using CometServer.Helpers;
    using CometServer.Modules;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations;
    using CometServer.Services.Protocol;

    using Moq;

    using NUnit.Framework;

    using IServiceProvider = CometServer.Services.IServiceProvider;

    /// <summary>
    /// Test fixture for the <see cref="CometServer.Services.Utils"/> class
    /// </summary>
    [TestFixture]
    public class UtilsTestFixture
    {
        private readonly SiteDirectory siteDir = new SiteDirectory();

        private readonly EngineeringModelSetup modelSetup = new EngineeringModelSetup();

        private readonly IRequestUtils requestUtils = new RequestUtils { QueryParameters = new QueryParameters() };
        
        private readonly string mockedId = Guid.NewGuid().ToString();

        private Mock<IModelCreatorManager> modelCreatorManager;

        private Mock<IAppConfigService> appConfigService;

        private Mock<ICredentialsService> credentialsService;

        private Mock<IHeaderInfoProvider> headerInfoProvider;

        private Mock<IServiceProvider> serviceProvider;

        private Mock<IPermissionService> permissionService;

        private Mock<IMetaInfoProvider> metaInfoProvider;

        private Mock<IOperationProcessor> operationProcessor;

        private Mock<IFileBinaryService> fileBinaryService;

        private Mock<IFileArchiveService> fileArchiveService;

        private Mock<IRevisionService> revisionService;

        private Mock<IRevisionResolver> revisionResolver;

        private Mock<ICdp4TransactionManager> transactionManager;

        private Mock<ICdp4JsonSerializer> jsonSerializer;

        private Mock<IPermissionInstanceFilterService> permissionInstanceFilterService;

        private Mock<IProcessor> SetupMockProcessor()
        {
            var mockedProcessor = new Mock<IProcessor>();

            // setup mocked method calls
            mockedProcessor.Setup(
                x => x.ValidateContainment(It.IsAny<List<Thing>>(), It.IsAny<string>(), It.IsAny<Guid>()));

            mockedProcessor.Setup(
                x => x.GetContainmentResource(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<ISecurityContext>()))
                           .Returns(this.siteDir);

            mockedProcessor.Setup(
                x => x.GetResource(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>()))
                           .Returns(new[] { this.modelSetup });

            mockedProcessor.SetupGet(x => x.RequestUtils).Returns(this.requestUtils);

            return mockedProcessor;
        }

        [SetUp]
        public void Setup()
        {
            this.modelCreatorManager = new Mock<IModelCreatorManager>();
            this.appConfigService = new Mock<IAppConfigService>();
            this.credentialsService = new Mock<ICredentialsService>();
            this.headerInfoProvider = new Mock<IHeaderInfoProvider>();
            this.serviceProvider = new Mock<IServiceProvider>();
            this.permissionService = new Mock<IPermissionService>();
            this.metaInfoProvider = new Mock<IMetaInfoProvider>();
            this.operationProcessor = new Mock<IOperationProcessor>();
            this.fileBinaryService = new Mock<IFileBinaryService>();
            this.fileArchiveService = new Mock<IFileArchiveService>();
            this.revisionService = new Mock<IRevisionService>();
            this.revisionResolver = new Mock<IRevisionResolver>();
            this.transactionManager = new Mock<ICdp4TransactionManager>();
            this.jsonSerializer = new Mock<ICdp4JsonSerializer>();
            this.permissionInstanceFilterService= new Mock<IPermissionInstanceFilterService>();
        }

        [Test]
        public void VerifyOnlyResourceReturned()
        {
            var mockedProcessor = this.SetupMockProcessor();
            
            List<Thing> containmentCollection;

            var siteDirectoryApi = new SiteDirectoryApi(this.appConfigService.Object);

            var result = siteDirectoryApi.ProcessRequestPath(this.requestUtils, this.transactionManager.Object, mockedProcessor.Object,  "SiteDirectory", "SiteDirectory", new[] { "SiteDirectory", this.mockedId, "model", this.mockedId }, out containmentCollection);

            CollectionAssert.AreEquivalent(new[] { this.modelSetup }, result);
        }

        [Test]
        public void VerifyResourceWithContainmentReturned()
        {
            var mockedProcessor = this.SetupMockProcessor();
            
            List<Thing> containmentCollection;

            // set query parameter override
            this.requestUtils.OverrideQueryParameters = new QueryParameters { IncludeAllContainers = true };

            var siteDirectoryApi = new SiteDirectoryApi(this.appConfigService.Object);

            var result = siteDirectoryApi.ProcessRequestPath(this.requestUtils, this.transactionManager.Object, mockedProcessor.Object, "SiteDirectory", "SiteDirectory", new[] { "SiteDirectory", this.mockedId, "model", this.mockedId }, out containmentCollection);
            
            // reset query parameter override
            this.requestUtils.OverrideQueryParameters = null;
            CollectionAssert.AreEquivalent(new Thing[] { this.siteDir, this.modelSetup }, result);
        }
    }
}
