// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonServiceTestFixture.cs" company="RHEA System S.A.">
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
    /// Suite of tests for the <see cref="PersonService"/>
    /// </summary>
    [TestFixture]
    public class PersonServiceTestFixture
    {
        private Person person;
        private IPersonService personService;
        private Mock<IPermissionService> permissionService;
        private Mock<IPersonDao> personDao;
        private Mock<ICdp4TransactionManager> transactionManager;
        private string schemaName;

        [SetUp]
        public void Setup()
        {
            this.person = new Person(Guid.NewGuid(), 0);
            this.personDao = new Mock<IPersonDao>();
            this.permissionService = new Mock<IPermissionService>();
            this.transactionManager = new Mock<ICdp4TransactionManager>();
            this.schemaName = Cdp4TransactionManager.SITE_DIRECTORY_PARTITION;

            this.permissionService.Setup(x => x.IsOwner(It.IsAny<NpgsqlTransaction>(), this.person)).Returns(true);

            this.permissionService.Setup(x => x.Credentials).Returns(new Credentials
            {
                Person = new AuthenticationPerson(this.person.Iid, 0)
                {
                    UserName = "TestRunner"
                }
            });

            this.personDao.Setup(x => x.Read(It.IsAny<NpgsqlTransaction>(), Cdp4TransactionManager.SITE_DIRECTORY_PARTITION, It.IsAny<IEnumerable<Guid>>(), It.IsAny<bool>()))
                .Returns(new[] { this.person });

            this.personService = new PersonService
            {
                PermissionService = this.permissionService.Object,
                PersonDao = this.personDao.Object,
                TransactionManager = this.transactionManager.Object
            };
        }

        [Test]
        public void VerifyUpdateCredentials_Initialization()
        {
            var credentials = new MigrationPasswordCredentials(this.person.Iid, "hashedpassword", "salt");
            Assert.AreEqual("hashedpassword", credentials.Password);
            Assert.AreEqual("salt", credentials.Salt);
            Assert.AreEqual(this.person.Iid, credentials.Iid);
        }

        [Test]
        public void VerifyUpdateCredentials_NoAccess()
        {
            this.transactionManager.Setup(x => x.IsFullAccessEnabled()).Returns(false);

            Assert.Throws<SecurityException>(() => this.personService.UpdateCredentials(It.IsAny<NpgsqlTransaction>(), this.schemaName, this.person, null));
        }

        [Test]
        public void VerifyUpdateCredentials_FullAccess()
        {
            var credentials = new MigrationPasswordCredentials(this.person.Iid, "hashedpassword", "salt");

            this.transactionManager.Setup(x => x.IsFullAccessEnabled()).Returns(true);

            this.personDao.Setup(x => x.UpdateCredentials(It.IsAny<NpgsqlTransaction>(), Cdp4TransactionManager.SITE_DIRECTORY_PARTITION, this.person, credentials)).Returns(true);

            Assert.DoesNotThrow(() => this.personService.UpdateCredentials(It.IsAny<NpgsqlTransaction>(), this.schemaName, this.person, credentials));
        }
    }
}
