// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantPermissionSideEffectTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
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