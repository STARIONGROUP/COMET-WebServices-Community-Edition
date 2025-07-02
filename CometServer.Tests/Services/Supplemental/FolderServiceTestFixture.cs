// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FolderServiceTestFixture.cs" company="Starion Group S.A.">
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

namespace CDP4WebServices.API.Tests.Services.Supplemental
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;

    using CDP4Authentication;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.Authorization;
    using CometServer.Helpers;
    using CometServer.Services;

    using Moq;
    
    using Npgsql;
    
    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="FolderService"/> class
    /// </summary>
    [TestFixture]
    public class FolderServiceTestFixture
    {
        private Folder folder;
        private FolderService folderService;
        private Mock<IDomainFileStoreService> domainFileStoreService;
        private Mock<IPermissionService> permissionService;
        private Mock<ICredentialsService> credentialsService;
        private Mock<IFolderDao> folderDao;
        private NpgsqlTransaction transaction;
        private Mock<ICdp4TransactionManager> transactionManager;
        private string iterationPartitionName;
        private string engineeringModelPartitionName;
        private Person person;
        private Mock<ICommonFileStoreService> commonFileStoreService;

        [SetUp]
        public void Setup()
        {
            this.folder = new Folder(Guid.NewGuid(), 0);
            this.folderDao = new Mock<IFolderDao>();
            this.permissionService = new Mock<IPermissionService>();
            this.credentialsService = new Mock<ICredentialsService>();
            this.domainFileStoreService = new Mock<IDomainFileStoreService>();
            this.commonFileStoreService = new Mock<ICommonFileStoreService>();
            this.transaction = null;
            this.transactionManager = new Mock<ICdp4TransactionManager>();
            this.person = new Person(Guid.NewGuid(), 0);

            this.folderService = new FolderService
            {
                PermissionService = this.permissionService.Object,
                FolderDao = this.folderDao.Object,
                TransactionManager = this.transactionManager.Object,
                DomainFileStoreService = this.domainFileStoreService.Object,
                CommonFileStoreService = this.commonFileStoreService.Object
            };

            this.iterationPartitionName = "Iteration_" + Guid.NewGuid();
            this.engineeringModelPartitionName = "EngineeringModel_" + Guid.NewGuid();

            this.permissionService.Setup(x => x.IsOwnerAsync(It.IsAny<NpgsqlTransaction>(), this.folder)).Returns(Task.FromResult(true));

            this.credentialsService.Setup(x => x.Credentials)
                .Returns(
                    new Credentials
                    {
                        Person = new AuthenticationPerson(this.person.Iid, 0)
                        {
                            UserName = "TestRunner"
                        }
                    });

            this.folderDao
                .Setup(
                    x => x.ReadAsync(It.IsAny<NpgsqlTransaction>(), this.iterationPartitionName, It.IsAny<IEnumerable<Guid>>(), It.IsAny<bool>(), null))
                .Returns(Task.FromResult<IEnumerable<Folder>>([this.folder]));

            this.transactionManager.Setup(x => x.IsFullAccessEnabled()).Returns(true);
        }

        [Test]
        public void VerifyContainerIsInstanceReadAllowedForDomainFileStore()
        {
            this.domainFileStoreService.Setup(x => x.HasReadAccessAsync(It.IsAny<Thing>(),It.IsAny<IDbTransaction>(), this.iterationPartitionName)).Returns(Task.FromResult(true));

            Assert.That(this.folderService.IsAllowedAccordingToIsHidden(this.transaction, this.folder, this.iterationPartitionName), Is.True);

            this.domainFileStoreService.Setup(x => x.HasReadAccessAsync(It.IsAny<Thing>(),It.IsAny<IDbTransaction>(), this.iterationPartitionName)).Returns(Task.FromResult(false));

            Assert.That(this.folderService.IsAllowedAccordingToIsHidden(this.transaction, this.folder, this.iterationPartitionName), Is.False);
        }

        [Test]
        public void VerifyContainerIsInstanceReadAllowedForCommonFileStore()
        {
            this.commonFileStoreService.Setup(x => x.HasReadAccess(It.IsAny<Thing>(),It.IsAny<IDbTransaction>(), this.engineeringModelPartitionName)).Returns(Task.FromResult(true));

            Assert.That(this.folderService.IsAllowedAccordingToIsHidden(this.transaction, this.folder, this.engineeringModelPartitionName), Is.True);

            this.commonFileStoreService.Setup(x => x.HasReadAccess(It.IsAny<Thing>(),It.IsAny<IDbTransaction>(), this.engineeringModelPartitionName)).Returns(Task.FromResult(false));

            Assert.That(this.folderService.IsAllowedAccordingToIsHidden(this.transaction, this.folder, this.engineeringModelPartitionName), Is.False);
        }
    }
}
