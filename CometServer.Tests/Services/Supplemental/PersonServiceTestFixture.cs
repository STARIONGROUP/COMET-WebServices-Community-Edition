// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonServiceTestFixture.cs" company="Starion Group S.A.">
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
    /// Suite of tests for the <see cref="PersonService"/>
    /// </summary>
    [TestFixture]
    public class PersonServiceTestFixture
    {
        private Person person;
        private PersonService personService;
        private Mock<IPermissionService> permissionService;
        private Mock<ICredentialsService> credentialsService;
        private Mock<IPersonDao> personDao;
        private Mock<ICdp4TransactionManager> transactionManager;
        private string schemaName;

        [SetUp]
        public void Setup()
        {
            this.person = new Person(Guid.NewGuid(), 0);
            this.personDao = new Mock<IPersonDao>();
            this.permissionService = new Mock<IPermissionService>();
            this.credentialsService = new Mock<ICredentialsService>();
            this.transactionManager = new Mock<ICdp4TransactionManager>();
            this.schemaName = Cdp4TransactionManager.SITE_DIRECTORY_PARTITION;

            this.permissionService.Setup(x => x.IsOwnerAsync(It.IsAny<NpgsqlTransaction>(), this.person)).Returns(true);

            var credentials = new Credentials();

            credentials.Person = new AuthenticationPerson(Guid.NewGuid(), 1)
            {
                UserName = "jdoe"
            };

            this.credentialsService.Setup(x => x.Credentials).Returns(credentials);

            this.personDao.Setup(x => x.Read(It.IsAny<NpgsqlTransaction>(), Cdp4TransactionManager.SITE_DIRECTORY_PARTITION, It.IsAny<IEnumerable<Guid>>(), It.IsAny<bool>(), null))
                .Returns(new[] { this.person });

            this.personService = new PersonService
            {
                PermissionService = this.permissionService.Object,
                CredentialsService = this.credentialsService.Object,
                PersonDao = this.personDao.Object,
                TransactionManager = this.transactionManager.Object
            };
        }

        [Test]
        public void VerifyUpdateCredentials_Initialization()
        {
            var credentials = new MigrationPasswordCredentials(this.person.Iid, "hashedpassword", "salt");

            Assert.That(credentials.Password, Is.EqualTo("hashedpassword"));
            Assert.That(credentials.Salt, Is.EqualTo("salt"));
            Assert.That(credentials.Iid, Is.EqualTo(this.person.Iid));
        }

        [Test]
        public void VerifyUpdateCredentials_NoAccess()
        {
            this.transactionManager.Setup(x => x.IsFullAccessEnabled()).Returns(false);

            Assert.Throws<SecurityException>(() => this.personService.UpdateCredentialsAsync(It.IsAny<NpgsqlTransaction>(), this.schemaName, this.person, null));
        }

        [Test]
        public void VerifyUpdateCredentials_FullAccess()
        {
            var credentials = new MigrationPasswordCredentials(this.person.Iid, "hashedpassword", "salt");

            this.transactionManager.Setup(x => x.IsFullAccessEnabled()).Returns(true);

            this.personDao.Setup(x => x.UpdateCredentialsAsync(It.IsAny<NpgsqlTransaction>(), Cdp4TransactionManager.SITE_DIRECTORY_PARTITION, this.person, credentials)).Returns(true);

            Assert.DoesNotThrow(() => this.personService.UpdateCredentialsAsync(It.IsAny<NpgsqlTransaction>(), this.schemaName, this.person, credentials));
        }
    }
}
