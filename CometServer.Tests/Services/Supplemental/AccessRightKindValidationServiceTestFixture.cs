﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessRightKindValidationServiceTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Services
{
    using System;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using CometServer.Services;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="AccessRightKindValidationService"/>
    /// </summary>
    [TestFixture]
    public class AccessRightKindValidationServiceTestFixture
    {
        private AccessRightKindValidationService service;

        private Mock<IRequestUtils> requestUtils;

        [SetUp]
        public void Setup()
        {
            this.requestUtils = new Mock<IRequestUtils>();
            this.service = new AccessRightKindValidationService() { RequestUtils = this.requestUtils.Object };
        }

        [Test]
        public void VerifyThatModifyIfParticipantCanBeSetOnlyForEngineeringModelSetupObjectClass()
        {
            var personPermission1 =
                new PersonPermission(Guid.NewGuid(), 1)
                    {
                        ObjectClass = ClassKind.SiteDirectory,
                        AccessRight = PersonAccessRightKind.MODIFY_IF_PARTICIPANT
                    };

            var personPermission2 =
                new PersonPermission(Guid.NewGuid(), 1)
                    {
                        ObjectClass = ClassKind.EngineeringModelSetup,
                        AccessRight = PersonAccessRightKind.MODIFY_IF_PARTICIPANT
                    };

            Assert.That(this.service.IsPersonPermissionValid(personPermission1), Is.False);
            Assert.That(this.service.IsPersonPermissionValid(personPermission2), Is.True);
        }

        [Test]
        public void VerifyThatReadIfParticipantCanBeSetOnlyForEngineeringModelSetupObjectClass()
        {
            var personPermission1 =
                new PersonPermission(Guid.NewGuid(), 1)
                    {
                        ObjectClass = ClassKind.SiteDirectory,
                        AccessRight = PersonAccessRightKind.READ_IF_PARTICIPANT
                    };

            var personPermission2 =
                new PersonPermission(Guid.NewGuid(), 1)
                    {
                        ObjectClass = ClassKind.EngineeringModelSetup,
                        AccessRight = PersonAccessRightKind.READ_IF_PARTICIPANT
                    };

            Assert.That(this.service.IsPersonPermissionValid(personPermission1), Is.False);
            Assert.That(this.service.IsPersonPermissionValid(personPermission2), Is.True);
        }

        [Test]
        public void VerifyThatModifyOwnPersonCanBeSetOnlyForPersonObjectClass()
        {
            var personPermission1 =
                new PersonPermission(Guid.NewGuid(), 1)
                    {
                        ObjectClass = ClassKind.IterationSetup,
                        AccessRight = PersonAccessRightKind.MODIFY_OWN_PERSON
                    };
            Assert.That(this.service.IsPersonPermissionValid(personPermission1), Is.False);

            var personPermission2 =
                new PersonPermission(Guid.NewGuid(), 1)
                    {
                        ObjectClass = ClassKind.Person,
                        AccessRight = PersonAccessRightKind.MODIFY_OWN_PERSON
                    };
            Assert.That(this.service.IsPersonPermissionValid(personPermission2), Is.True);
        }

        [Test]
        public void VerifyThatModifyIfOwnerCanBeSetOnlyForOwnedObjectClass()
        {
            var participantPermission1 =
                new ParticipantPermission(Guid.NewGuid(), 1)
                    {
                        ObjectClass = ClassKind.DiagramCanvas,
                        AccessRight = ParticipantAccessRightKind
                            .MODIFY_IF_OWNER
                    };

            var participantPermission2 =
                new ParticipantPermission(Guid.NewGuid(), 1)
                    {
                        ObjectClass = ClassKind.Book,
                        AccessRight = ParticipantAccessRightKind
                            .MODIFY_IF_OWNER
                    };

            Assert.That(this.service.IsParticipantPermissionValid(participantPermission1), Is.False);
            Assert.That(this.service.IsParticipantPermissionValid(participantPermission2), Is.True);
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenInappropriateThingIsSupplied()
        {
            var book = new Book(Guid.NewGuid(), 1);

            var page = new Page(Guid.NewGuid(), 1);

            Assert.Throws<InvalidCastException>(() => this.service.IsPersonPermissionValid(book));
            Assert.Throws<InvalidCastException>(() => this.service.IsParticipantPermissionValid(page));
        }
    }
}