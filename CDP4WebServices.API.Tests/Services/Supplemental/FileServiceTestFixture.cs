// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileServiceTestFixture.cs" company="RHEA System S.A.">
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
    using System.Security;

    using CDP4Authentication;

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authentication;
    using CometServer.Services.Authorization;

    using Moq;
    
    using Npgsql;
    
    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="FileService"/>
    /// </summary>
    [TestFixture]
    public class FileServiceTestFixture
    {
        private File file;
        private IFileService fileService;
        private Mock<IPermissionService> permissionService;
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
            this.transaction = null;
            this.transactionManager = new Mock<ICdp4TransactionManager>();
            this.person = new Person(Guid.NewGuid(), 0);

            this.fileService = new FileService
            {
                PermissionService = this.permissionService.Object,
                FileDao = this.fileDao.Object,
                TransactionManager = this.transactionManager.Object
            };

            this.iterationPartitionName = "Iteration_" + Guid.NewGuid();

            this.permissionService.Setup(x => x.IsOwner(It.IsAny<NpgsqlTransaction>(), this.file)).Returns(true);

            this.permissionService.Setup(x => x.Credentials)
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
                    x => x.Read(It.IsAny<NpgsqlTransaction>(), this.iterationPartitionName, It.IsAny<IEnumerable<Guid>>(), It.IsAny<bool>()))
                .Returns(new[] { this.file });

            this.transactionManager.Setup(x => x.IsFullAccessEnabled()).Returns(true);
        }

        [Test]
        public void VerifyCheckFileLock()
        {
            Assert.DoesNotThrow(() => this.fileService.CheckFileLock(this.transaction, this.iterationPartitionName, this.file));

            this.file.LockedBy = this.person.Iid;
            Assert.DoesNotThrow(() => this.fileService.CheckFileLock(this.transaction, this.iterationPartitionName, this.file));

            this.file.LockedBy = Guid.NewGuid();
            Assert.Throws<SecurityException>(() => this.fileService.CheckFileLock(this.transaction, this.iterationPartitionName, this.file));
        }
    }
}
