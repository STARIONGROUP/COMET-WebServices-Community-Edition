// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommonFileStoreServiceTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

    using CDP4Authentication;

    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    using CDP4Orm.Dao;

    using CometServer.Authorization;
    using CometServer.Helpers;
    using CometServer.Services;
    
    using Moq;
    
    using Npgsql;
    
    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="CommonFileStoreService"/>
    /// </summary>
    [TestFixture]
    public class CommonFileStoreServiceTestFixture
    {
        private static Folder folder = new Folder(Guid.NewGuid(), 0);
        private static File file = new File(Guid.NewGuid(), 0);
        private static CommonFileStore commonFileStore = new CommonFileStore(Guid.NewGuid(), 0);

        private CommonFileStoreService commonFileStoreService;
        private Mock<IPermissionService> permissionService;
        private Mock<ICredentialsService> credentialsService;
        private Mock<ICommonFileStoreDao> commonFileStoreDao;
        private Mock<IDbTransaction> transaction;
        private Mock<ICdp4TransactionManager> transactionManager;
        private string engineeringModelPartitionName;
        private EngineeringModel engineeringModel;

        [SetUp]
        public void Setup()
        {
            this.commonFileStoreDao = new Mock<ICommonFileStoreDao>();
            this.permissionService = new Mock<IPermissionService>();
            this.credentialsService = new Mock<ICredentialsService>();
            this.transaction = new Mock<IDbTransaction>();
            this.transactionManager = new Mock<ICdp4TransactionManager>();

            this.commonFileStoreService = new CommonFileStoreService
            {
                PermissionService = this.permissionService.Object,
                CredentialsService = this.credentialsService.Object,
                CommonFileStoreDao = this.commonFileStoreDao.Object,
                TransactionManager = this.transactionManager.Object
            };

            this.engineeringModelPartitionName = "EngineeringModel_" + Guid.NewGuid();

            commonFileStore.File.Clear();
            commonFileStore.File.Add(file.Iid);

            commonFileStore.Folder.Clear();
            commonFileStore.Folder.Add(folder.Iid);

            this.engineeringModel = new EngineeringModel(Guid.NewGuid(), 0)
            {
                CommonFileStore = new List<Guid>
                {
                    commonFileStore.Iid
                }
            };

            this.permissionService.Setup(x => x.IsOwner(It.IsAny<NpgsqlTransaction>(), commonFileStore)).Returns(true);
            this.permissionService.Setup(x => x.IsOwner(It.IsAny<NpgsqlTransaction>(), folder)).Returns(true);
            this.permissionService.Setup(x => x.IsOwner(It.IsAny<NpgsqlTransaction>(), file)).Returns(true);

            this.credentialsService.Setup(x => x.Credentials)
                .Returns(
                    new Credentials
                    {
                        Person = new AuthenticationPerson(Guid.NewGuid(), 0)
                        {
                            UserName = "TestRunner"
                        }
                    });

            // Without a commonFileStoreSelector, SingleOrDefault(commonFileStoreSelector) could fail, because multiple <see cref="CommonFileStore"/>s could exist.
            // Also if a new CommonFileStore including Files and Folders are created in the same webservice call, then GetShallow for the new CommonFileStores might not return
            // the to be created <see cref="CommonFileStore"/>. The isHidden check will then be ignored.
            var extraCommonFileStoreToTestCommonFileStoreSelectors = new CommonFileStore(Guid.NewGuid(), 0);

            this.commonFileStoreDao
                .Setup(
                    x => x.Read(It.IsAny<NpgsqlTransaction>(), this.engineeringModelPartitionName, It.IsAny<IEnumerable<Guid>>(), It.IsAny<bool>(), DateTime.MaxValue))
                .Returns(new[] { commonFileStore, extraCommonFileStoreToTestCommonFileStoreSelectors });

            this.transactionManager.Setup(x => x.IsFullAccessEnabled()).Returns(true);
            this.transactionManager.Setup(x => x.GetRawSessionInstant(It.IsAny<NpgsqlTransaction>())).Returns(DateTime.MaxValue);
        }

        [Test]
        [TestCaseSource(nameof(TestWriteCases))]
        public void VerifyHasReadAccess<T>(T thing, bool shouldFail) where T : Thing
        {
            if (shouldFail)
            {
                Assert.Throws<Cdp4ModelValidationException>(() => this.commonFileStoreService.HasReadAccess(
                    thing,
                    this.transaction.Object,
                    this.engineeringModelPartitionName));
            }
            else
            {
                Assert.That(this.commonFileStoreService.HasReadAccess(
                    thing,
                    this.transaction.Object,
                    this.engineeringModelPartitionName), Is.True);

                this.permissionService.Setup(x => x.IsOwner(It.IsAny<NpgsqlTransaction>(), commonFileStore)).Returns(false);

                Assert.That(this.commonFileStoreService.HasReadAccess(
                    thing,
                    this.transaction.Object,
                    this.engineeringModelPartitionName), Is.True);
            }
        }

        [Test]
        [TestCaseSource(nameof(TestWriteCases))]
        public void VerifyCheckSecurity<T>(T thing, bool shouldFail) where T : Thing
        {
            if (shouldFail)
            {
                Assert.Throws<SecurityException>(() => this.commonFileStoreService.HasWriteAccess(
                    thing,
                    this.transaction.Object,
                    this.engineeringModelPartitionName));

                this.permissionService.Setup(x => x.IsOwner(It.IsAny<NpgsqlTransaction>(), It.IsAny<ElementDefinition>())).Returns(true);

                Assert.Throws<Cdp4ModelValidationException>(() => this.commonFileStoreService.HasWriteAccess(
                    thing,
                    this.transaction.Object,
                    this.engineeringModelPartitionName));
            }
            else
            {
                Assert.DoesNotThrow(() => this.commonFileStoreService.HasWriteAccess(
                    thing,
                    this.transaction.Object,
                    this.engineeringModelPartitionName));

                this.permissionService.Setup(x => x.IsOwner(It.IsAny<NpgsqlTransaction>(), thing)).Returns(false);

                Assert.Throws<SecurityException>(() => this.commonFileStoreService.HasWriteAccess(
                    thing,
                    this.transaction.Object,
                    this.engineeringModelPartitionName));
            }
        }

        public static IEnumerable TestWriteCases()
        {
            yield return new object[] { file, false };
            yield return new object[] { folder, false };
            yield return new object[] { commonFileStore, false };
            yield return new object[] { new ElementDefinition(), true };
        }

        public static IEnumerable TestReadCases()
        {
            yield return new object[] { file, false };
            yield return new object[] { folder, false };
            yield return new object[] { commonFileStore, false };
            yield return new object[] { new ElementDefinition(), true };
        }
    }
}
