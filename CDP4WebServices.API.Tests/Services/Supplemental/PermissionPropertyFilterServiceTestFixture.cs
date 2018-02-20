// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionPropertyFilterServiceTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.Services.Supplemental
{
    using System;
    using System.Collections.Generic;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.MetaInfo;

    using CDP4Orm.Dao;

    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Supplemental;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="PermissionPropertyFilterService"/>
    /// </summary>
    [TestFixture]
    public class PermissionPropertyFilterServiceTestFixture
    {
        private const string SiteDirectoryData = "SiteDirectory";

        private Mock<IRequestUtils> requestUtils;

        private Mock<IMetaInfoProvider> metaInfoProvider;

        private Mock<ICdp4TransactionManager> transactionManager;

        private Mock<IParticipantPermissionDao> participantPermissionDao;

        private Mock<IPersonPermissionDao> personPermissionDao;

        private NpgsqlTransaction npgsqlTransaction;

        private PermissionPropertyFilterService permissionPropertyFilterService;

        private List<PersonRole> personRoles;

        private List<ParticipantRole> participantRoles;

        private Guid personPermission1id = Guid.NewGuid();
        
        private Guid personPermission2id = Guid.NewGuid();

        private Guid personPermission3id = Guid.NewGuid();

        private Guid participantPermission1id = Guid.NewGuid();

        private Guid participantPermission2id = Guid.NewGuid();

        private Guid participantPermission3id = Guid.NewGuid();

        private PersonPermission personPermission1;

        private PersonPermission personPermission2;

        private PersonPermission personPermission3;

        private ParticipantPermission participantPermission1;

        private ParticipantPermission participantPermission2;

        private ParticipantPermission participantPermission3;

        [SetUp]
        public void Setup()
        {
            this.npgsqlTransaction = null;

            this.personRoles = new List<PersonRole>
                                   {
                                       new PersonRole
                                           {
                                               Iid = Guid.NewGuid(),
                                               RevisionNumber = 1,
                                               PersonPermission =
                                                   new List<Guid>
                                                       {
                                                           this
                                                               .personPermission1id,
                                                           this
                                                               .personPermission2id,
                                                           this
                                                               .personPermission3id
                                                       }
                                           }
                                   };
            this.participantRoles = new List<ParticipantRole>
                                        {
                                            new ParticipantRole
                                                {
                                                    Iid = Guid.NewGuid(),
                                                    RevisionNumber = 1,
                                                    ParticipantPermission =
                                                        new List<Guid>
                                                            {
                                                                this
                                                                    .participantPermission1id,
                                                                this
                                                                    .participantPermission2id,
                                                                this
                                                                    .participantPermission3id
                                                            }
                                                }
                                        };

            this.requestUtils = new Mock<IRequestUtils>();
            this.metaInfoProvider = new Mock<IMetaInfoProvider>();

            this.requestUtils.Setup(x => x.MetaInfoProvider).Returns(this.metaInfoProvider.Object);
            this.metaInfoProvider.Setup(x => x.GetMetaInfo("DomainOfExpertise"))
                .Returns(new DomainOfExpertiseMetaInfo());
            this.metaInfoProvider.Setup(x => x.GetMetaInfo("IterationSetup"))
                .Returns(new IterationSetupMetaInfo());
            this.metaInfoProvider.Setup(x => x.GetMetaInfo("SiteDirectoryDataAnnotation"))
                .Returns(new SiteDirectoryDataAnnotationMetaInfo());
            this.metaInfoProvider.Setup(x => x.GetMetaInfo("ElementDefinition"))
                .Returns(new ElementDefinitionMetaInfo());
            this.metaInfoProvider.Setup(x => x.GetMetaInfo("ElementUsage"))
                .Returns(new ElementUsageMetaInfo());
            this.metaInfoProvider.Setup(x => x.GetMetaInfo("Book"))
                .Returns(new BookMetaInfo());

            this.transactionManager = new Mock<ICdp4TransactionManager>();
            NpgsqlConnection connection = null;
            this.transactionManager.Setup(x => x.SetupTransaction(ref connection, null))
                .Returns(this.npgsqlTransaction);

            this.participantPermission1 = new ParticipantPermission(this.participantPermission1id, 1);
            this.participantPermission1.ObjectClass = ClassKind.DomainOfExpertise;
            this.participantPermission2 = new ParticipantPermission(this.participantPermission2id, 1);
            this.participantPermission2.ObjectClass = ClassKind.IterationSetup;
            this.participantPermission3 = new ParticipantPermission(this.participantPermission3id, 1);
            this.participantPermission3.ObjectClass = ClassKind.SiteDirectoryDataAnnotation;
            this.participantPermissionDao = new Mock<IParticipantPermissionDao>();
            this.participantPermissionDao.Setup(x => x.Read(this.npgsqlTransaction, SiteDirectoryData, null, true))
                .Returns(
                    new List<ParticipantPermission>
                        {
                            this.participantPermission1,
                            this.participantPermission2,
                            this.participantPermission3
                        });

            this.personPermission1 = new PersonPermission(this.personPermission1id, 1);
            this.personPermission1.ObjectClass = ClassKind.DomainOfExpertise;
            this.personPermission2 = new PersonPermission(this.personPermission2id, 1);
            this.personPermission2.ObjectClass = ClassKind.IterationSetup;
            this.personPermission3 = new PersonPermission(this.personPermission3id, 1);
            this.personPermission3.ObjectClass = ClassKind.SiteDirectoryDataAnnotation;
            this.personPermissionDao = new Mock<IPersonPermissionDao>();
            this.personPermissionDao.Setup(x => x.Read(this.npgsqlTransaction, SiteDirectoryData, null, true)).Returns(
                new List<PersonPermission> { this.personPermission1, this.personPermission2, this.personPermission3 });
        }

        [Test]
        public void VerifyThatPersonPermissionPropertyIsFilteredForPureRequest()
        {
            this.permissionPropertyFilterService = new PermissionPropertyFilterService();

            this.permissionPropertyFilterService.RequestUtils = this.requestUtils.Object;
            this.permissionPropertyFilterService.TransactionManager = this.transactionManager.Object;
            this.permissionPropertyFilterService.PersonPermissionDao = this.personPermissionDao.Object;
            this.permissionPropertyFilterService.ParticipantPermissionDao = this.participantPermissionDao.Object;

            this.permissionPropertyFilterService.FilterPersonPermissionProperty(this.personRoles, new Version("1.0.0"));

            CollectionAssert.AreEquivalent(
                this.personRoles[0].PersonPermission,
                new List<Guid> { this.personPermission1id, this.personPermission2id });
        }

        [Test]
        public void VerifyThatPersonPermissionPropertyIsNotFilteredForCDP4Request()
        {
            this.permissionPropertyFilterService = new PermissionPropertyFilterService();

            this.permissionPropertyFilterService.RequestUtils = this.requestUtils.Object;
            this.permissionPropertyFilterService.TransactionManager = this.transactionManager.Object;
            this.permissionPropertyFilterService.PersonPermissionDao = this.personPermissionDao.Object;
            this.permissionPropertyFilterService.ParticipantPermissionDao = this.participantPermissionDao.Object;

            this.permissionPropertyFilterService.FilterPersonPermissionProperty(this.personRoles, new Version("1.1.0"));

            CollectionAssert.AreEquivalent(
                this.personRoles[0].PersonPermission,
                new List<Guid> { this.personPermission1id, this.personPermission2id, this.personPermission3id });
        }

        [Test]
        public void VerifyThatParticipantPermissionPropertyIsFilteredForPureRequest()
        {
            this.permissionPropertyFilterService = new PermissionPropertyFilterService();

            this.permissionPropertyFilterService.RequestUtils = this.requestUtils.Object;
            this.permissionPropertyFilterService.TransactionManager = this.transactionManager.Object;
            this.permissionPropertyFilterService.ParticipantPermissionDao = this.participantPermissionDao.Object;
            this.permissionPropertyFilterService.ParticipantPermissionDao = this.participantPermissionDao.Object;

            this.permissionPropertyFilterService.FilterParticipantPermissionProperty(this.participantRoles, new Version("1.0.0"));

            CollectionAssert.AreEquivalent(
                this.participantRoles[0].ParticipantPermission,
                new List<Guid> { this.participantPermission1id, this.participantPermission2id });
        }

        [Test]
        public void VerifyThatParticipantPermissionPropertyIsNotFilteredForCDP4Request()
        {
            this.permissionPropertyFilterService = new PermissionPropertyFilterService();

            this.permissionPropertyFilterService.RequestUtils = this.requestUtils.Object;
            this.permissionPropertyFilterService.TransactionManager = this.transactionManager.Object;
            this.permissionPropertyFilterService.ParticipantPermissionDao = this.participantPermissionDao.Object;
            this.permissionPropertyFilterService.ParticipantPermissionDao = this.participantPermissionDao.Object;

            this.permissionPropertyFilterService.FilterParticipantPermissionProperty(this.participantRoles, new Version("1.1.0"));

            CollectionAssert.AreEquivalent(
                this.participantRoles[0].ParticipantPermission,
                new List<Guid> { this.participantPermission1id, this.participantPermission2id, this.participantPermission3id });
        }
    }
}