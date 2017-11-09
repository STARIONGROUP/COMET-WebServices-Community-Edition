// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UtilsTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// <summary>
//   This the Service utility test class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;
    using CDP4WebServices.API.Modules;
    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Protocol;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the <see cref="CDP4WebServices.API.Services.Utils"/> class
    /// </summary>
    [TestFixture]
    public class UtilsTestFixture
    {
        private readonly SiteDirectory siteDir = new SiteDirectory();
        private readonly EngineeringModelSetup modelSetup = new EngineeringModelSetup();
        private readonly IRequestUtils requestUtils = new RequestUtils { QueryParameters = new QueryParameters() };
        
        private readonly string mockedId = Guid.NewGuid().ToString();

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

        [Test]
        public void VerifyOnlyResourceReturned()
        {
            var mockedProcessor = this.SetupMockProcessor();
            List<Thing> containmentCollection;
            var result = new SiteDirectoryApi { RequestUtils = this.requestUtils }.ProcessRequestPath(mockedProcessor.Object, "SiteDirectory", "SiteDirectory", new[] { "SiteDirectory", this.mockedId, "model", this.mockedId }, out containmentCollection);

            CollectionAssert.AreEquivalent(new[] { this.modelSetup }, result);
        }

        [Test]
        public void VerifyResourceWithContainmentReturned()
        {
            var mockedProcessor = this.SetupMockProcessor();
            List<Thing> containmentCollection;

            // set query parameter override
            this.requestUtils.OverrideQueryParameters = new QueryParameters { IncludeAllContainers = true };

            var result = new SiteDirectoryApi { RequestUtils = this.requestUtils }.ProcessRequestPath(mockedProcessor.Object, "SiteDirectory", "SiteDirectory", new[] { "SiteDirectory", this.mockedId, "model", this.mockedId }, out containmentCollection);
            
            // reset query parameter override
            this.requestUtils.OverrideQueryParameters = null;
            CollectionAssert.AreEquivalent(new Thing[] { this.siteDir, this.modelSetup }, result);
        }
    }
}
