// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleSideEffectTestFixture.cs" company="Starion Group S.A.">
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
    using System.Threading.Tasks;

    using CDP4Common.DTO;

    using CometServer.Authorization;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Verifies the simple side effect implementations that only delegate to the organizational participation resolver.
    /// </summary>
    [TestFixture]
    public class SimpleSideEffectTestFixture
    {
        private Mock<IOrganizationalParticipationResolverService> resolverService;

        private Mock<ISecurityContext> securityContext;

        [SetUp]
        public void Setup()
        {
            this.resolverService = new Mock<IOrganizationalParticipationResolverService>();
            this.securityContext = new Mock<ISecurityContext>();
        }

        [Test]
        public async Task AliasSideEffectDelegatesToResolverAsync()
        {
            var sideEffect = new AliasSideEffect
            {
                OrganizationalParticipationResolverService = this.resolverService.Object
            };

            var alias = new Alias();
            var container = new ElementDefinition();

            var result = await sideEffect.BeforeCreateAsync(alias, container, null, "partition", this.securityContext.Object);

            Assert.That(result, Is.True);
            this.resolverService.Verify(
                r => r.ValidateCreateOrganizationalParticipationAsync(alias, container, this.securityContext.Object, null, "partition"),
                Times.Once);
        }

        [Test]
        public async Task CitationSideEffectDelegatesToResolverAsync()
        {
            var sideEffect = new CitationSideEffect
            {
                OrganizationalParticipationResolverService = this.resolverService.Object
            };

            var citation = new Citation();
            var container = new Definition();

            var result = await sideEffect.BeforeCreateAsync(citation, container, null, "partition", this.securityContext.Object);

            Assert.That(result, Is.True);

            this.resolverService.Verify(
                r => r.ValidateCreateOrganizationalParticipationAsync(citation, container, this.securityContext.Object, null, "partition"),
                Times.Once);
        }

        [Test]
        public async Task HyperLinkSideEffectDelegatesToResolverAsync()
        {
            var sideEffect = new HyperLinkSideEffect
            {
                OrganizationalParticipationResolverService = this.resolverService.Object
            };

            var hyperlink = new HyperLink();
            var container = new Definition();

            var result = await sideEffect.BeforeCreateAsync(hyperlink, container, null, "partition", this.securityContext.Object);

            Assert.That(result, Is.True);

            this.resolverService.Verify(
                r => r.ValidateCreateOrganizationalParticipationAsync(hyperlink, container, this.securityContext.Object, null, "partition"),
                Times.Once);
        }
    }
}
