// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObfuscationServiceTestFixture.cs" company="Starion Group S.A.">
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

    using CDP4Common.DTO;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Types;

    using CometServer.Authorization;

    using Microsoft.Extensions.Logging;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="ObfuscationService"/>.
    /// </summary>
    [TestFixture]
    public class ObfuscationServiceTestFixture
    {
        private ObfuscationService service;

        [SetUp]
        public void Setup()
        {
            this.service = new ObfuscationService
            {
                Logger = Mock.Of<ILogger<ObfuscationService>>()
            };
        }

        [Test]
        public void ObfuscateResponseDoesNothingForDefaultParticipant()
        {
            var credentials = new Credentials
            {
                IsDefaultOrganizationalParticipant = true
            };

            var elementDefinition = new ElementDefinition { Name = "Original", ShortName = "original" };

            this.service.ObfuscateResponse(new List<Thing> { elementDefinition }, credentials);

            Assert.That(elementDefinition.Name, Is.EqualTo("Original"));
        }

        [Test]
        public void ObfuscateResponseHidesRestrictedContent()
        {
            var elementDefinitionId = Guid.NewGuid();
            var parameterId = Guid.NewGuid();
            var valueSetId = Guid.NewGuid();
            var parameterGroupId = Guid.NewGuid();
            var aliasId = Guid.NewGuid();
            var hyperlinkId = Guid.NewGuid();

            var valueArray = new ValueArray<string>(new List<string> { "value" });

            var parameterValueSet = new ParameterValueSet
            {
                Iid = valueSetId,
                Manual = valueArray,
                Reference = valueArray,
                Computed = valueArray,
                Published = valueArray,
                Formula = valueArray,
                ValueSwitch = ParameterSwitchKind.MANUAL
            };

            var parameter = new Parameter
            {
                Iid = parameterId,
                ValueSets = new List<Guid> { valueSetId },
                ParameterSubscription = new List<Guid>()
            };

            var parameterGroup = new ParameterGroup { Iid = parameterGroupId, Name = "Group" };
            var alias = new Alias { Iid = aliasId, Content = "Alias" };
            var hyperlink = new HyperLink { Iid = hyperlinkId, Content = "Link", Uri = "http://example.com" };

            var elementDefinition = new ElementDefinition
            {
                Iid = elementDefinitionId,
                Name = "Visible",
                ShortName = "visible",
                Parameter = new List<Guid> { parameterId },
                ParameterGroup = new List<Guid> { parameterGroupId },
                Alias = new List<Guid> { aliasId },
                HyperLink = new List<Guid> { hyperlinkId }
            };

            var credentials = new Credentials
            {
                IsDefaultOrganizationalParticipant = false,
                OrganizationalParticipant = null
            };

            var resourceResponse = new List<Thing>
            {
                elementDefinition,
                parameter,
                parameterValueSet,
                parameterGroup,
                alias,
                hyperlink
            };

            this.service.ObfuscateResponse(resourceResponse, credentials);

            Assert.That(elementDefinition.Name, Is.EqualTo("Hidden Element Definition"));
            Assert.That(parameterGroup.Name, Is.EqualTo("Hidden Group"));
            Assert.That(alias.Content, Is.EqualTo("Hidden Alias"));
            Assert.That(hyperlink.Content, Is.EqualTo("Hidden Alias"));
            Assert.That(parameterValueSet.Manual[0], Is.EqualTo("-"));
        }
    }
}
