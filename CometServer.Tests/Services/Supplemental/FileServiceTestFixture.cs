﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileServiceTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
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
    using System.Data;
    using System.Security;

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
    /// Suite of tests for the <see cref="FileService"/> class
    /// </summary>
    [TestFixture]
    public class FileServiceTestFixture
    {
        private File file;
        private FileService fileService;
        private Mock<IDomainFileStoreService> domainFileStoreService;
        private Mock<IPermissionService> permissionService;
        private Mock<ICredentialsService> credentialsService;
        private Mock<IFileDao> fileDao;
        private NpgsqlTransaction transaction;
        private Mock<ICdp4TransactionManager> transactionManager;
        private string iterationPartitionName;
        private Person person;

        [SetUp]
        public void Setup()
        {
            this.file = new File(Guid.NewGuid(), 0);
            this.fileDao = new Mock<IFileDao>();
            this.permissionService = new Mock<IPermissionService>();
            this.credentialsService = new Mock<ICredentialsService>();
            this.domainFileStoreService = new Mock<IDomainFileStoreService>();
            this.transaction = null;
            this.transactionManager = new Mock<ICdp4TransactionManager>();
            this.person = new Person(Guid.NewGuid(), 0);

            this.fileService = new FileService
            {
                PermissionService = this.permissionService.Object,
                CredentialsService = this.credentialsService.Object,
                FileDao = this.fileDao.Object,
                TransactionManager = this.transactionManager.Object,
                DomainFileStoreService = this.domainFileStoreService.Object,
            };

            this.iterationPartitionName = "Iteration_" + Guid.NewGuid();

            this.permissionService.Setup(x => x.IsOwner(It.IsAny<NpgsqlTransaction>(), this.file)).Returns(true);

            this.credentialsService.Setup(x => x.Credentials)
                .Returns(
                    new Credentials
                    {
                        Person = new AuthenticationPerson(this.person.Iid, 0)
                        {
                            UserName = "TestRunner"
                        }
                    });

            this.fileDao
                .Setup(
                    x => x.Read(It.IsAny<NpgsqlTransaction>(), this.iterationPartitionName, It.IsAny<IEnumerable<Guid>>(), It.IsAny<bool>(), DateTime.MaxValue))
                .Returns(new[] { this.file });

            this.transactionManager.Setup(x => x.IsFullAccessEnabled()).Returns(true);
            this.transactionManager.Setup(x => x.GetRawSessionInstant(It.IsAny<NpgsqlTransaction>())).Returns(DateTime.MaxValue);
        }

        [Test]
        public void VerifyCheckFileLock()
        {
            this.domainFileStoreService.Setup(x => x.HasReadAccess(It.IsAny<Thing>(),It.IsAny<IDbTransaction>(), this.iterationPartitionName)).Returns(true);

            Assert.DoesNotThrow(() => this.fileService.CheckFileLockAsync(this.transaction, this.iterationPartitionName, this.file));

            this.file.LockedBy = this.person.Iid;
            Assert.DoesNotThrow(() => this.fileService.CheckFileLockAsync(this.transaction, this.iterationPartitionName, this.file));

            this.file.LockedBy = Guid.NewGuid();
            Assert.Throws<SecurityException>(() => this.fileService.CheckFileLockAsync(this.transaction, this.iterationPartitionName, this.file));
        }

        [Test]
        public void VerifyContainerIsInstanceReadAllowed()
        {
            this.domainFileStoreService.Setup(x => x.HasReadAccess(It.IsAny<Thing>(),It.IsAny<IDbTransaction>(), this.iterationPartitionName)).Returns(true);

            Assert.That(this.fileService.IsAllowedAccordingToIsHiddenAsync(this.transaction, this.file, this.iterationPartitionName), Is.True);

            this.domainFileStoreService.Setup(x => x.HasReadAccess(It.IsAny<Thing>(),It.IsAny<IDbTransaction>(), this.iterationPartitionName)).Returns(false);

            Assert.That(this.fileService.IsAllowedAccordingToIsHiddenAsync(this.transaction, this.file, this.iterationPartitionName), Is.False);
        }
    }
}
