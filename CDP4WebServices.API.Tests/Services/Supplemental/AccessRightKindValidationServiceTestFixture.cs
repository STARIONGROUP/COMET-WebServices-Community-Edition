// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccessRightKindValidationServiceTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.Services
{
    using System;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using CDP4WebServices.API.Services;

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

        private Mock<IMetaInfoProvider> metaInfoProvider;

        [SetUp]
        public void Setup()
        {
            this.requestUtils = new Mock<IRequestUtils>();
            this.metaInfoProvider = new Mock<IMetaInfoProvider>();

            this.requestUtils.Setup(x => x.MetaInfoProvider).Returns(this.metaInfoProvider.Object);

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

            Assert.False(this.service.IsPersonPermissionValid(personPermission1));
            Assert.True(this.service.IsPersonPermissionValid(personPermission2));
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

            Assert.False(this.service.IsPersonPermissionValid(personPermission1));
            Assert.True(this.service.IsPersonPermissionValid(personPermission2));
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
            Assert.False(this.service.IsPersonPermissionValid(personPermission1));

            var personPermission2 =
                new PersonPermission(Guid.NewGuid(), 1)
                    {
                        ObjectClass = ClassKind.Person,
                        AccessRight = PersonAccessRightKind.MODIFY_OWN_PERSON
                    };
            Assert.True(this.service.IsPersonPermissionValid(personPermission2));
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

            Assert.False(this.service.IsParticipantPermissionValid(participantPermission1));
            Assert.True(this.service.IsParticipantPermissionValid(participantPermission2));
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