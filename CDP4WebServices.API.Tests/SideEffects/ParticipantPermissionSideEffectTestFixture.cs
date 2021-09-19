// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantPermissionSideEffectTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski
//
//    This file is part of COMET Web Services Community Edition. 
//    The COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.SideEffects
{
    using System;

    using CDP4Common;
    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// Suite of tests for the <see cref="ParticipantPermissionSideEffect"/>
    /// </summary>
    [TestFixture]
    public class ParticipantPermissionSideEffectTestFixture
    {
        private Mock<ISecurityContext> securityContext;

        private Mock<IAccessRightKindValidationService> permissiveAccessRightKindValidationService;

        private Mock<IAccessRightKindValidationService> denyingAccessRightKindValidationService;

        private NpgsqlTransaction npgsqlTransaction;

        private ParticipantPermissionSideEffect sideEffect;

        [SetUp]
        public void Setup()
        {
            this.securityContext = new Mock<ISecurityContext>();
            this.npgsqlTransaction = null;

            this.permissiveAccessRightKindValidationService = new Mock<IAccessRightKindValidationService>();
            this.permissiveAccessRightKindValidationService.Setup(x => x.IsParticipantPermissionValid(It.IsAny<Thing>()))
                .Returns(true);

            this.denyingAccessRightKindValidationService = new Mock<IAccessRightKindValidationService>();
            this.denyingAccessRightKindValidationService.Setup(x => x.IsParticipantPermissionValid(It.IsAny<Thing>()))
                .Returns(false);
        }

        [Test]
        public void VerifyThatExceptionIsThrownWhenSetAccessRightIsNotValid()
        {
            var participantPermission =
                new ParticipantPermission(Guid.NewGuid(), 1)
                    {
                        ObjectClass = ClassKind.DiagramCanvas,
                        AccessRight = ParticipantAccessRightKind.MODIFY_IF_OWNER
                    };

            this.sideEffect =
                new ParticipantPermissionSideEffect()
                    {
                        AccessRightKindValidationService =
                            this.denyingAccessRightKindValidationService.Object
                    };

            Assert.Throws<InvalidOperationException>(
                () => this.sideEffect.BeforeCreate(
                    participantPermission,
                    null,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object));

            Assert.Throws<InvalidOperationException>(
                () => this.sideEffect.AfterUpdate(
                    participantPermission,
                    null,
                    null,
                    this.npgsqlTransaction,
                    "partition",
                    this.securityContext.Object));
        }

        [Test]
        public void VerifyThatSideEffectPassedWhenSetAccessRightIsValid()
        {
            var participantPermission =
                new ParticipantPermission(Guid.NewGuid(), 1)
                    {
                        ObjectClass = ClassKind.Book,
                        AccessRight = ParticipantAccessRightKind.MODIFY_IF_OWNER
                };

            this.sideEffect =
                new ParticipantPermissionSideEffect()
                    {
                        AccessRightKindValidationService =
                            this.permissiveAccessRightKindValidationService.Object
                    };
            this.sideEffect.BeforeCreate(
                participantPermission,
                null,
                this.npgsqlTransaction,
                "partition",
                this.securityContext.Object);

            this.sideEffect.AfterUpdate(
                participantPermission,
                null,
                null,
                this.npgsqlTransaction,
                "partition",
                this.securityContext.Object);

            this.permissiveAccessRightKindValidationService.Verify(
                x => x.IsParticipantPermissionValid(It.IsAny<ParticipantPermission>()),
                Times.Exactly(2));
        }
    }
}