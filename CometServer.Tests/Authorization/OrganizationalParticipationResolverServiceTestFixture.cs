// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrganizationalParticipationResolverServiceTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using CometServer.Authorization;
    using CometServer.Services;
    using CometServer.Services.Authorization;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="OrganizationalParticipationResolverService"/>.
    /// </summary>
    [TestFixture]
    public class OrganizationalParticipationResolverServiceTestFixture
    {
        private OrganizationalParticipationResolverService service;
        private Mock<IElementDefinitionService> elementDefinitionService;
        private Mock<IElementUsageService> elementUsageService;
        private Mock<ICredentialsService> credentialsService;
        private Mock<ISecurityContext> securityContext;

        [SetUp]
        public void Setup()
        {
            this.elementDefinitionService = new Mock<IElementDefinitionService>();
            this.elementUsageService = new Mock<IElementUsageService>();
            this.credentialsService = new Mock<ICredentialsService>();
            this.securityContext = new Mock<ISecurityContext>();

            this.service = new OrganizationalParticipationResolverService
            {
                ElementDefinitionService = this.elementDefinitionService.Object,
                ElementUsageService = this.elementUsageService.Object,
                CredentialsService = this.credentialsService.Object,
                Logger = Mock.Of<Microsoft.Extensions.Logging.ILogger<OrganizationalParticipationResolverService>>()
            };
        }

        [Test]
        public void ValidateCreateOrganizationalParticipationAsyncReturnsForSiteDirectory()
        {
            Assert.DoesNotThrowAsync(() => this.service.ValidateCreateOrganizationalParticipationAsync(new ElementDefinition(), new ElementDefinition(), this.securityContext.Object, null, "SiteDirectory"));
            this.credentialsService.VerifyGet(c => c.Credentials, Times.Never);
        }

        [Test]
        public void ValidateCreateOrganizationalParticipationAsyncThrowsWhenNoParticipant()
        {
            var credentials = new Credentials
            {
                EngineeringModelSetup = new EngineeringModelSetup { OrganizationalParticipant = new List<Guid> { Guid.NewGuid() } },
                IsDefaultOrganizationalParticipant = false,
                OrganizationalParticipant = null
            };

            this.credentialsService.SetupGet(c => c.Credentials).Returns(credentials);

            Assert.ThrowsAsync<InvalidOperationException>(() => this.service.ValidateCreateOrganizationalParticipationAsync(new ElementDefinition(), new ElementDefinition(), this.securityContext.Object, null, "partition"));
        }

        [Test]
        public async Task ResolveApplicableOrganizationalParticipationsReturnsTrueForKnownThing()
        {
            var participantId = Guid.NewGuid();
            var elementDefinitionId = Guid.NewGuid();
            var iteration = new Iteration
            {
                Element = new List<Guid> { elementDefinitionId }
            };

            var elementDefinition = new ElementDefinition
            {
                Iid = elementDefinitionId,
                OrganizationalParticipant = new List<Guid> { participantId }
            };

            this.elementDefinitionService.Setup(x => x.GetAsync(null, "partition", iteration.Element, It.IsAny<ISecurityContext>()))
                .ReturnsAsync(new List<Thing> { elementDefinition });
            this.elementDefinitionService.Setup(x => x.GetDeepAsync(null, "partition", It.IsAny<IEnumerable<Guid>>(), It.IsAny<ISecurityContext>()))
                .ReturnsAsync(new List<Thing> { elementDefinition });

            var thingToCheck = new ElementDefinition
            {
                Iid = elementDefinitionId,
                ClassKind = ClassKind.ElementDefinition
            };

            var result = await this.service.ResolveApplicableOrganizationalParticipationsAsync(null, "partition", iteration, thingToCheck, participantId);

            Assert.That(result, Is.True);
        }
    }
}
