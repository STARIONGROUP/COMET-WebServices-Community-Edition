// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermissionInstanceFilterServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace CometServer.Tests.Services.Supplemental
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.MetaInfo;

    using CDP4Orm.Dao;

    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Supplemental;

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
            this.metaInfoProvider = new MetaInfoProvider
            {
                DomainOfExpertiseMetaInfo = new DomainOfExpertiseMetaInfo(),
                IterationSetupMetaInfo = new IterationSetupMetaInfo(),
                SiteDirectoryDataAnnotationMetaInfo = new SiteDirectoryDataAnnotationMetaInfo(),
                ElementDefinitionMetaInfo = new ElementDefinitionMetaInfo(),
                ElementUsageMetaInfo = new ElementUsageMetaInfo(),
                BookMetaInfo = new BookMetaInfo(),
                ActionItemMetaInfo = new ActionItemMetaInfo(),
                SiteDirectoryMetaInfo = new SiteDirectoryMetaInfo(),
                ActualFiniteStateMetaInfo = new ActualFiniteStateMetaInfo(),
                DiagramCanvasMetaInfo = new DiagramCanvasMetaInfo(),
                EngineeringModelMetaInfo = new EngineeringModelMetaInfo(),
            };

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
            this.participantPermissionDao.Setup(x => x.Read(this.npgsqlTransaction, SiteDirectoryData, It.IsAny<IEnumerable<Guid>>(), true))
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
            this.personPermissionDao.Setup(x => x.Read(this.npgsqlTransaction, SiteDirectoryData, It.IsAny<IEnumerable<Guid>>(), true)).Returns(
                new List<PersonPermission> { this.personPermission1, this.personPermission2, this.personPermission3 });

            this.service = new PermissionInstanceFilterService
            {
                MetadataProvider = this.metaInfoProvider,
                TransactionManager = this.transactionManager.Object,
                ParticipantPermissionDao = this.participantPermissionDao.Object,
                PersonPermissionDao = this.personPermissionDao.Object
            };

        }

        [Test]
        public void VerifyThatPersonPermissionPropertyIsFilteredForPureRequest()
        {
            var result = this.service.FilterOutPermissions(this.personRoles, new Version("1.0.0")).OfType<PersonRole>().ToArray();

            CollectionAssert.AreEquivalent(
                result[0].PersonPermission,
                new List<Guid> { this.personPermission1id, this.personPermission2id });
        }

        [Test]
        public void VerifyThatPersonPermissionPropertyIsNotFilteredForCDP4Request()
        {
            var result = this.service.FilterOutPermissions(this.personRoles, new Version("1.1.0")).OfType<PersonRole>().ToArray();

            CollectionAssert.AreEquivalent(
                result[0].PersonPermission,
                new List<Guid> { this.personPermission1id, this.personPermission2id, this.personPermission3id });
        }

        [Test]
        public void VerifyThatParticipantPermissionPropertyIsFilteredForPureRequest()
        {
            var result = this.service.FilterOutPermissions(this.participantRoles, new Version("1.0.0")).OfType<ParticipantRole>().ToArray();

            CollectionAssert.AreEquivalent(
                result[0].ParticipantPermission,
                new List<Guid> { this.participantPermission1id, this.participantPermission2id });
        }

        [Test]
        public void VerifyThatParticipantPermissionPropertyIsNotFilteredForCDP4Request()
        {
            var result = this.service.FilterOutPermissions(this.participantRoles, new Version("1.1.0")).OfType<ParticipantRole>().ToArray();

            CollectionAssert.AreEquivalent(
                result[0].ParticipantPermission,
                new List<Guid> { this.participantPermission1id, this.participantPermission2id, this.participantPermission3id });
        }

        [Test]
        public void VerifyThatRoleAndPermissionAreFilteredCorrectly()
        {
            var personRole = new PersonRole(Guid.NewGuid(), 0);
            var participantRole = new ParticipantRole(Guid.NewGuid(), 0);

            this.personPermission1 = new PersonPermission(Guid.NewGuid(), 0);
            this.personPermission1.ObjectClass = ClassKind.ActionItem;
            this.personPermission2 = new PersonPermission(Guid.NewGuid(), 0);
            this.personPermission2.ObjectClass = ClassKind.SiteDirectory;
            this.personPermission3 = new PersonPermission(Guid.NewGuid(), 0);
            this.personPermission3.ObjectClass = ClassKind.ActualFiniteState;

            this.participantPermission1 = new ParticipantPermission(Guid.NewGuid(), 0);
            this.participantPermission1.ObjectClass = ClassKind.DiagramCanvas;
            this.participantPermission2 = new ParticipantPermission(Guid.NewGuid(), 0);
            this.participantPermission2.ObjectClass = ClassKind.EngineeringModel;
            this.participantPermission3 = new ParticipantPermission(Guid.NewGuid(), 0);
            this.participantPermission3.ObjectClass = ClassKind.ElementDefinition;

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

            var result = this.service.FilterOutPermissions(
                input,
                new Version(1, 0)).ToArray();
            Assert.IsFalse(result.Contains(this.personPermission1));
            Assert.IsFalse(result.Contains(this.participantPermission1));
            Assert.AreEqual(personRole.PersonPermission.Count, 2);
            Assert.AreEqual(participantRole.ParticipantPermission.Count, 2);
        }
    }
}
