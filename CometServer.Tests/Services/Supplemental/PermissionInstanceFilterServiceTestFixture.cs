// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionInstanceFilterServiceTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Services.Supplemental
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Supplemental;

    using Microsoft.Extensions.Logging;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// Suite of tests for the <see cref="PermissionInstanceFilterService"/>
    /// </summary>
    [TestFixture]
    public class PermissionInstanceFilterServiceTestFixture
    {
        private const string SiteDirectoryData = "SiteDirectory";

        private Mock<ILogger<PermissionInstanceFilterService>> logger;

        private Mock<ICdp4TransactionManager> transactionManager;

        private Mock<IParticipantPermissionDao> participantPermissionDao;

        private Mock<IPersonPermissionDao> personPermissionDao;

        private IMetaInfoProvider metaInfoProvider;

        private NpgsqlTransaction npgsqlTransaction;

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

        private PermissionInstanceFilterService service;

        [SetUp]
        public void Setup()
        {
            this.npgsqlTransaction = null;
            this.metaInfoProvider = new MetaInfoProvider();

            this.personRoles =
            [
                new PersonRole
                {
                    Iid = Guid.NewGuid(),
                    RevisionNumber = 1,
                    PersonPermission =
                    [
                        this
                            .personPermission1id,

                        this
                            .personPermission2id,

                        this
                            .personPermission3id
                    ]
                }
            ];

            this.participantRoles =
            [
                new ParticipantRole
                {
                    Iid = Guid.NewGuid(),
                    RevisionNumber = 1,
                    ParticipantPermission =
                    [
                        this
                            .participantPermission1id,

                        this
                            .participantPermission2id,

                        this
                            .participantPermission3id
                    ]
                }
            ];

            this.logger = new Mock<ILogger<PermissionInstanceFilterService>>();

            this.transactionManager = new Mock<ICdp4TransactionManager>();

            this.transactionManager.Setup(x => x.SetupTransactionAsync(null))
                .Returns(Task.FromResult(this.npgsqlTransaction));

            this.participantPermission1 = new ParticipantPermission(this.participantPermission1id, 1)
            {
                ObjectClass = ClassKind.DomainOfExpertise
            };

            this.participantPermission2 = new ParticipantPermission(this.participantPermission2id, 1)
            {
                ObjectClass = ClassKind.IterationSetup
            };

            this.participantPermission3 = new ParticipantPermission(this.participantPermission3id, 1)
            {
                ObjectClass = ClassKind.SiteDirectoryDataAnnotation
            };

            this.participantPermissionDao = new Mock<IParticipantPermissionDao>();

            this.participantPermissionDao.Setup(x => x.ReadAsync(this.npgsqlTransaction, SiteDirectoryData, It.IsAny<IEnumerable<Guid>>(), true, null))
                .Returns(
                    Task.FromResult<IEnumerable<ParticipantPermission>>(new List<ParticipantPermission>
                    {
                        this.participantPermission1,
                        this.participantPermission2,
                        this.participantPermission3
                    }));

            this.personPermission1 = new PersonPermission(this.personPermission1id, 1)
            {
                ObjectClass = ClassKind.DomainOfExpertise
            };

            this.personPermission2 = new PersonPermission(this.personPermission2id, 1)
            {
                ObjectClass = ClassKind.IterationSetup
            };

            this.personPermission3 = new PersonPermission(this.personPermission3id, 1)
            {
                ObjectClass = ClassKind.SiteDirectoryDataAnnotation
            };

            this.personPermissionDao = new Mock<IPersonPermissionDao>();

            this.personPermissionDao.Setup(x => x.ReadAsync(this.npgsqlTransaction, SiteDirectoryData, It.IsAny<IEnumerable<Guid>>(), true, null)).Returns(
                Task.FromResult<IEnumerable<PersonPermission>>(new List<PersonPermission> { this.personPermission1, this.personPermission2, this.personPermission3 }));

            this.service = new PermissionInstanceFilterService
            {
                MetadataProvider = this.metaInfoProvider,
                TransactionManager = this.transactionManager.Object,
                ParticipantPermissionDao = this.participantPermissionDao.Object,
                PersonPermissionDao = this.personPermissionDao.Object,
                Logger = this.logger.Object
            };
        }

        [Test]
        public async Task VerifyThatPersonPermissionPropertyIsFilteredForPureRequest()
        {
            var result = (await this.service.FilterOutPermissionsAsync(this.personRoles, new Version("1.0.0"))).OfType<PersonRole>().ToArray();

            Assert.That(result[0].PersonPermission, Is.EquivalentTo(new List<Guid> { this.personPermission1id, this.personPermission2id }));
        }

        [Test]
        public async Task VerifyThatPersonPermissionPropertyIsNotFilteredForCDP4Request()
        {
            var result = (await this.service.FilterOutPermissionsAsync(this.personRoles, new Version("1.1.0"))).OfType<PersonRole>().ToArray();

            Assert.That(result[0].PersonPermission, Is.EquivalentTo(new List<Guid> { this.personPermission1id, this.personPermission2id, this.personPermission3id }));
        }

        [Test]
        public async Task VerifyThatParticipantPermissionPropertyIsFilteredForPureRequest()
        {
            var result = (await this.service.FilterOutPermissionsAsync(this.participantRoles, new Version("1.0.0"))).OfType<ParticipantRole>().ToArray();

            Assert.That(result[0].ParticipantPermission, Is.EquivalentTo(new List<Guid> { this.participantPermission1id, this.participantPermission2id }));
        }

        [Test]
        public async Task VerifyThatParticipantPermissionPropertyIsNotFilteredForCDP4Request()
        {
            var result = (await this.service.FilterOutPermissionsAsync(this.participantRoles, new Version("1.1.0"))).OfType<ParticipantRole>().ToArray();

            Assert.That(result[0].ParticipantPermission, Is.EquivalentTo(new List<Guid> { this.participantPermission1id, this.participantPermission2id, this.participantPermission3id }));
        }

        [Test]
        public async Task VerifyThatRoleAndPermissionAreFilteredCorrectly()
        {
            var personRole = new PersonRole(Guid.NewGuid(), 0);
            var participantRole = new ParticipantRole(Guid.NewGuid(), 0);

            this.personPermission1 = new PersonPermission(Guid.NewGuid(), 0)
            {
                ObjectClass = ClassKind.ActionItem
            };

            this.personPermission2 = new PersonPermission(Guid.NewGuid(), 0)
            {
                ObjectClass = ClassKind.SiteDirectory
            };

            this.personPermission3 = new PersonPermission(Guid.NewGuid(), 0)
            {
                ObjectClass = ClassKind.ActualFiniteState
            };

            this.participantPermission1 = new ParticipantPermission(Guid.NewGuid(), 0)
            {
                ObjectClass = ClassKind.DiagramCanvas
            };

            this.participantPermission2 = new ParticipantPermission(Guid.NewGuid(), 0)
            {
                ObjectClass = ClassKind.EngineeringModel
            };

            this.participantPermission3 = new ParticipantPermission(Guid.NewGuid(), 0)
            {
                ObjectClass = ClassKind.ElementDefinition
            };

            personRole.PersonPermission.Add(this.personPermission3.Iid);
            personRole.PersonPermission.Add(this.personPermission2.Iid);
            personRole.PersonPermission.Add(this.personPermission1.Iid);

            participantRole.ParticipantPermission.Add(this.participantPermission1.Iid);
            participantRole.ParticipantPermission.Add(this.participantPermission2.Iid);
            participantRole.ParticipantPermission.Add(this.participantPermission3.Iid);

            var input = new List<Thing>
            {
                personRole,
                participantRole,
                this.personPermission1,
                this.personPermission2,
                this.personPermission3,
                this.participantPermission1,
                this.participantPermission2,
                this.participantPermission3
            };

            var result = (await this.service.FilterOutPermissionsAsync(
                input,
                new Version(1, 0))).ToArray();

            Assert.That(result, Does.Not.Contain(this.personPermission1));
            Assert.That(result, Does.Not.Contain(this.participantPermission1));
            Assert.That(personRole.PersonPermission.Count, Is.EqualTo(2));
            Assert.That(participantRole.ParticipantPermission.Count, Is.EqualTo(2));
        }
    }
}
