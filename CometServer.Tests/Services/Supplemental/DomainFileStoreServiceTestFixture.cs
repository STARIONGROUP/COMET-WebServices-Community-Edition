// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomainFileStoreServiceTestFixture.cs" company="Starion Group S.A.">
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
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Security;
    using System.Threading.Tasks;

    using CDP4Authentication;

    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    using CDP4Orm.Dao;

    using CometServer.Authorization;
    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authorization;

    using Moq;
    
    using Npgsql;
    
    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="DomainFileStoreService"/>
    /// </summary>
    [TestFixture]
    public class DomainFileStoreServiceTestFixture
    {
        private static Folder folder = new(Guid.NewGuid(), 0);
        private static File file = new(Guid.NewGuid(), 0);
        private static DomainFileStore domainFileStore = new(Guid.NewGuid(), 0);

        private DomainFileStoreService domainFileStoreService;
        private Mock<IPermissionService> permissionService;
        private Mock<ICredentialsService> credentialsService;
        private Mock<IIterationService> iterationService;
        private Mock<IDomainFileStoreDao> domainFileStoreDao;
        private Mock<IDbTransaction> transaction;
        private Mock<ICdp4TransactionManager> transactionManager;
        private string iterationPartitionName;
        private Iteration iteration;

        [SetUp]
        public void Setup()
        {
            this.domainFileStoreDao = new Mock<IDomainFileStoreDao>();
            this.iterationService = new Mock<IIterationService>();
            this.permissionService = new Mock<IPermissionService>();
            this.credentialsService = new Mock<ICredentialsService>();
            this.transaction = new Mock<IDbTransaction>();
            this.transactionManager = new Mock<ICdp4TransactionManager>();

            this.domainFileStoreService = new DomainFileStoreService
            {
                IterationService = this.iterationService.Object,
                PermissionService = this.permissionService.Object,
                CredentialsService = this.credentialsService.Object,
                DomainFileStoreDao = this.domainFileStoreDao.Object,
                TransactionManager = this.transactionManager.Object
            };

            this.iterationPartitionName = "Iteration_" + Guid.NewGuid();

            domainFileStore.IsHidden = false;

            domainFileStore.File.Clear();
            domainFileStore.File.Add(file.Iid);

            domainFileStore.Folder.Clear();
            domainFileStore.Folder.Add(folder.Iid);

            this.iteration = new Iteration(Guid.NewGuid(), 0)
            {
                DomainFileStore = [domainFileStore.Iid]
            };

            this.iterationService
                .Setup(x => x.GetActiveIterationAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<ISecurityContext>()))
                .Returns(Task.FromResult(this.iteration));

            this.permissionService.Setup(x => x.IsOwnerAsync(It.IsAny<NpgsqlTransaction>(), domainFileStore)).Returns(Task.FromResult(true));
            this.permissionService.Setup(x => x.IsOwnerAsync(It.IsAny<NpgsqlTransaction>(), folder)).Returns(Task.FromResult(true));
            this.permissionService.Setup(x => x.IsOwnerAsync(It.IsAny<NpgsqlTransaction>(), file)).Returns(Task.FromResult(true));

            this.credentialsService.Setup(x => x.Credentials)
                .Returns(
                    new Credentials
                    {
                        Person = new AuthenticationPerson(Guid.NewGuid(), 0)
                        {
                            UserName = "TestRunner"
                        }
                    });

            // Without a domainFileStoreSelector, SingleOrDefault(domainFileStoreSelector) could fail, because multiple <see cref="DomainFileStore"/>s could exist.
            // Also if a new DomainFileStore including Files and Folders are created in the same webservice call, then GetShallow for the new DomainFileStores might not return
            // the to be created <see cref="DomainFileStore"/>. The isHidden check will then be ignored.
            var extraDomainFileStoreToTestDomainFileStoreSelectors = new DomainFileStore(Guid.NewGuid(), 0);

            this.domainFileStoreDao
                .Setup(
                    x => x.ReadAsync(It.IsAny<NpgsqlTransaction>(), this.iterationPartitionName, It.IsAny<IEnumerable<Guid>>(), It.IsAny<bool>(), DateTime.MaxValue))
                .Returns(Task.FromResult<IEnumerable<DomainFileStore>>([domainFileStore, extraDomainFileStoreToTestDomainFileStoreSelectors]));

            this.transactionManager.Setup(x => x.IsFullAccessEnabled()).Returns(true);
            this.transactionManager.Setup(x => x.GetRawSessionInstantAsync(It.IsAny<NpgsqlTransaction>())).Returns(Task.FromResult<object>(DateTime.MaxValue));
        }

        [Test]
        public void VerifyIsAllowedAccordingToIsHidden()
        {
            Assert.That(async () => await this.domainFileStoreService.IsAllowedAccordingToIsHiddenAsync(this.transaction.Object, domainFileStore), Is.True);

            domainFileStore.IsHidden = true;
            Assert.That(async () => await this.domainFileStoreService.IsAllowedAccordingToIsHiddenAsync(this.transaction.Object, domainFileStore), Is.True);

            this.permissionService.Setup(x => x.IsOwnerAsync(It.IsAny<NpgsqlTransaction>(), domainFileStore)).Returns(Task.FromResult(false));
            Assert.That(async () => await this.domainFileStoreService.IsAllowedAccordingToIsHiddenAsync(this.transaction.Object, domainFileStore), Is.False);

            domainFileStore.IsHidden = false;
            Assert.That(async () => await this.domainFileStoreService.IsAllowedAccordingToIsHiddenAsync(this.transaction.Object, domainFileStore), Is.True);
        }

        [Test]
        public void VerifyCheckAllowedAccordingToIsHidden()
        {
            Assert.DoesNotThrowAsync(() => this.domainFileStoreService.CheckAllowedAccordingToIsHidden(this.transaction.Object, domainFileStore));

            domainFileStore.IsHidden = true;
            Assert.DoesNotThrowAsync(() => this.domainFileStoreService.CheckAllowedAccordingToIsHidden(this.transaction.Object, domainFileStore));

            this.permissionService.Setup(x => x.IsOwnerAsync(It.IsAny<NpgsqlTransaction>(), domainFileStore)).Returns(Task.FromResult(false));
            Assert.That(() => this.domainFileStoreService.CheckAllowedAccordingToIsHidden(this.transaction.Object, domainFileStore), Throws.TypeOf<ThingNotFoundException>());

            domainFileStore.IsHidden = false;
            Assert.That(() => this.domainFileStoreService.CheckAllowedAccordingToIsHidden(this.transaction.Object, domainFileStore), Throws.Nothing);
        }

        [Test]
        [TestCaseSource(nameof(TestWriteCases))]
        public void VerifyHasReadAccess<T>(T thing, bool shouldFail) where T : Thing
        {
            if (shouldFail)
            {
                Assert.That(async () => await this.domainFileStoreService.HasReadAccessAsync(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName), Throws.TypeOf<Cdp4ModelValidationException>());
            }
            else
            {
                Assert.That(async () => await this.domainFileStoreService.HasReadAccessAsync(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName), Is.True);

                domainFileStore.IsHidden = true;

                Assert.That(async () => await this.domainFileStoreService.HasReadAccessAsync(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName), Is.True);

                this.permissionService.Setup(x => x.IsOwnerAsync(It.IsAny<NpgsqlTransaction>(), domainFileStore)).Returns(Task.FromResult(false));

                Assert.That(async () => await this.domainFileStoreService.HasReadAccessAsync(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName), Is.False);

                domainFileStore.IsHidden = false;

                Assert.That(async () => await this.domainFileStoreService.HasReadAccessAsync(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName), Is.True);
            }
        }

        [Test]
        [TestCaseSource(nameof(TestWriteCases))]
        public void VerifyCheckSecurity<T>(T thing, bool shouldFail) where T : Thing
        {
            if (shouldFail)
            {
                Assert.That(() => this.domainFileStoreService.HasWriteAccessAsync(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName), Throws.TypeOf<SecurityException>());

                this.permissionService.Setup(x => x.IsOwnerAsync(It.IsAny<NpgsqlTransaction>(), It.IsAny<ElementDefinition>())).Returns(Task.FromResult(true));

                Assert.That(() => this.domainFileStoreService.HasWriteAccessAsync(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName), Throws.TypeOf<Cdp4ModelValidationException>());
            }
            else
            {
                Assert.That(() => this.domainFileStoreService.HasWriteAccessAsync(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName), Throws.Nothing);

                domainFileStore.IsHidden = true;

                Assert.That(() => this.domainFileStoreService.HasWriteAccessAsync(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName), Throws.Nothing);

                this.permissionService.Setup(x => x.IsOwnerAsync(It.IsAny<NpgsqlTransaction>(), thing)).Returns(Task.FromResult(false));

                Assert.That(() => this.domainFileStoreService.HasWriteAccessAsync(
                    thing,
                    this.transaction.Object,
                    this.iterationPartitionName), Throws.TypeOf<SecurityException>());
            }
        }

        public static IEnumerable TestWriteCases()
        {
            yield return new object[] { file, false };
            yield return new object[] { folder, false };
            yield return new object[] { domainFileStore, false };
            yield return new object[] { new ElementDefinition(), true };
        }
    }
}
