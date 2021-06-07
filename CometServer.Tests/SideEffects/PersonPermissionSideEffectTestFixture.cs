// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonPermissionSideEffectTestFixture.cs" company="RHEA System S.A.">
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

namespace CometServer.Tests.SideEffects
{
    using System;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// Suite of tests for the <see cref="PersonPermissionSideEffect"/>
    /// </summary>
    [TestFixture]
    public class PersonPermissionSideEffectTestFixture
    {
        private Mock<ISecurityContext> securityContext;

        private Mock<IAccessRightKindValidationService> permissiveAccessRightKindValidationService;

        private Mock<IAccessRightKindValidationService> denyingAccessRightKindValidationService;

        private NpgsqlTransaction npgsqlTransaction;

        private PersonPermissionSideEffect sideEffect;

        [SetUp]
        public void Setup()
        {
            this.securityContext = new Mock<ISecurityContext>();
            this.npgsqlTransaction = null;

            this.permissiveAccessRightKindValidationService = new Mock<IAccessRightKindValidationService>();
            this.permissiveAccessRightKindValidationService.Setup(x => x.IsPersonPermissionValid(It.IsAny<Thing>()))
                .Returns(true);

            this.denyingAccessRightKindValidationService = new Mock<IAccessRightKindValidationService>();
            this.denyingAccessRightKindValidationService.Setup(x => x.IsPersonPermissionValid(It.IsAny<Thing>()))
                .Returns(false);
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenSetAccessRightIsNotValid()
        {
            var personPermission =
                new PersonPermission(Guid.NewGuid(), 1)
                    {
                        ObjectClass = ClassKind.SiteDirectory,
                        AccessRight = PersonAccessRightKind.MODIFY_IF_PARTICIPANT
                    };

            this.sideEffect =
                new PersonPermissionSideEffect()
                    {
                        AccessRightKindValidationService =
                            this.denyingAccessRightKindValidationService.Object
                    };

            Assert.Throws<InvalidOperationException>(
                () => this.sideEffect.BeforeCreate(
                    personPermission,
                    null,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object));

            Assert.Throws<InvalidOperationException>(
                () =>this.sideEffect.AfterUpdate(
                    personPermission, 
                    null, 
                    null, 
                    this.npgsqlTransaction, 
                    "partition", 
                    this.securityContext.Object));
        }

        [Test]
        public void VerifyThatSideEffectPassedWhenSetAccessRightIsValid()
        {
            var personPermission =
                new PersonPermission(Guid.NewGuid(), 1)
                    {
                        ObjectClass = ClassKind.SiteDirectory,
                        AccessRight = PersonAccessRightKind.MODIFY_IF_PARTICIPANT
                    };

            this.sideEffect =
                new PersonPermissionSideEffect()
                    {
                        AccessRightKindValidationService =
                            this.permissiveAccessRightKindValidationService.Object
                    };
            this.sideEffect.BeforeCreate(
                personPermission,
                null,
                this.npgsqlTransaction,
                "partition",
                this.securityContext.Object);

            this.sideEffect.AfterUpdate(
                personPermission,
                null,
                null,
                this.npgsqlTransaction,
                "partition",
                this.securityContext.Object);

            this.permissiveAccessRightKindValidationService.Verify(
                x => x.IsPersonPermissionValid(It.IsAny<PersonPermission>()),
                Times.Exactly(2));
        }
    }
}