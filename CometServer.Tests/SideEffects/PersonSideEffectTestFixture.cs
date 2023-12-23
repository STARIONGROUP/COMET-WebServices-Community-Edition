// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonSideEffectTestFixture.cs" company="RHEA System S.A.">
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

namespace CometServer.Tests.SideEffects
{
    using System;

    using CDP4Authentication;

    using CDP4Common;
    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.Authorization;
    using CometServer.Services;
    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="PersonSideEffect"/> class.
    /// </summary>
    [TestFixture]
    public class PersonSideEffectTestFixture
    {
        private const string ClearTextPassword = "clearText";

        private const string PasswordKey = "Password";

        private const string TestValue = "Test";

        private PersonSideEffect personSideEffect;
        
        private Mock<IPersonDao> personDao;

        private ClasslessDTO rawUpdateinfo;

        private Mock<ICredentialsService> credentialsService;

        [SetUp]
        public void SetUp()
        {
            this.personDao = new Mock<IPersonDao>();

            this.credentialsService = new Mock<ICredentialsService>();

            this.personSideEffect = new PersonSideEffect { PersonDao = this.personDao.Object, CredentialsService = this.credentialsService.Object};
        }

        [Test]
        public void VerifyThatPasswordChangeIsEncapsulated()
        {
            this.rawUpdateinfo = new ClasslessDTO()
                                     {
                                            { TestValue, TestValue },
                                            { PasswordKey, ClearTextPassword }
                                     };

            this.personSideEffect.BeforeUpdate(null, null, null, null, null, this.rawUpdateinfo);

            var expectedPasswordText = $"{this.personSideEffect.PersonDao.PasswordChangeToken}{ClearTextPassword}{this.personSideEffect.PersonDao.PasswordChangeToken}";
            
            // assert same number of entries in raw update info object
            Assert.That(this.rawUpdateinfo.Count, Is.EqualTo(2));
            Assert.That(this.rawUpdateinfo[TestValue].ToString(), Is.EqualTo(TestValue));

            // assert encapsulated cleartext password
            Assert.That(this.rawUpdateinfo[PasswordKey].ToString(), Is.EqualTo(expectedPasswordText));
        }

        [Test]
        public void VerifyThatNoPasswordChangeIsIgnored()
        {
            this.rawUpdateinfo = new ClasslessDTO()
                                     {
                                             { TestValue, TestValue }
                                     };

            this.personSideEffect.BeforeUpdate(null, null, null, null, null, this.rawUpdateinfo);

            // assert same number of entries in raw update info object
            Assert.That(this.rawUpdateinfo.Count, Is.EqualTo(1));
            Assert.That(this.rawUpdateinfo[TestValue].ToString(), Is.EqualTo(TestValue));
        }

        [Test]
        public void Verify_that_when_Person_tries_to_update_themselves_not_setting_isactive_to_false_or_isdeprecated_to_true_is_allowed()
        {
            var personIid = Guid.NewGuid();
            var personRileIid = Guid.NewGuid();

            var credentials = new Credentials();
            credentials.Person = new AuthenticationPerson(personIid, 1);

            this.credentialsService.Setup(x => x.Credentials).Returns(credentials);

            var orginalPerson = new Person(personIid, 1)
            {
                Role = personRileIid,
                IsActive = true,
                IsDeprecated = false
            };

            var updatedPerson = new Person(personIid, 2)
            {
                Role = personRileIid,
                IsActive = true,
                IsDeprecated = false,
                GivenName = "new name"
            };

            Assert.That(() => this.personSideEffect.AfterUpdate(updatedPerson, null, orginalPerson, null, null, null),
                Throws.Nothing);
        }

        [Test]
        public void Verify_that_when_Person_tries_to_change_own_person_role_is_not_allowed()
        {
            var personIid = Guid.NewGuid();
            var personRileIid = Guid.NewGuid();

            var credentials = new Credentials
            {
                Person = new AuthenticationPerson(personIid, 1)
            };

            this.credentialsService.Setup(x => x.Credentials).Returns(credentials);

            var orginalPerson = new Person(personIid, 1)
            {
                Role = personRileIid,
                IsActive = true,
                IsDeprecated = false
            };

            var updatedPerson = new Person(personIid, 2)
            {
                Role = Guid.NewGuid(),
                IsActive = true,
                IsDeprecated = false
            };

            Assert.That(() => this.personSideEffect.AfterUpdate(updatedPerson, null, orginalPerson, null, null, null),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("Update to role of the Person making the request is not allowed"));
        }

        [Test]
        public void Verify_that_when_Person_tries_to_update_themselves_setting_isactive_to_false_or_isdeprecated_to_true_is_not_allowed()
        {
            var personIid = Guid.NewGuid();
            var personRileIid = Guid.NewGuid();

            var credentials = new Credentials
            {
                Person = new AuthenticationPerson(personIid, 1)
            };

            this.credentialsService.Setup(x => x.Credentials).Returns(credentials);

            var orginalPerson = new Person(personIid, 1)
            {
                Role = personRileIid,
                IsActive = true,
                IsDeprecated = false
            };

            var updatedPerson = new Person(personIid, 2)
            {
                Role = personRileIid,
                IsActive = false,
                IsDeprecated = false,
                GivenName = "new name"
            };

            Assert.That(() => this.personSideEffect.AfterUpdate(updatedPerson, null, orginalPerson, null, null, null),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("Update IsActive = false to own Person is not allowed"));

            updatedPerson.IsActive = true;
            updatedPerson.IsDeprecated = true;

            Assert.That(() => this.personSideEffect.AfterUpdate(updatedPerson, null, orginalPerson, null, null, null),
                Throws.TypeOf<InvalidOperationException>()
                    .With.Message.EqualTo("Update IsDeprecated = true to own Person is not allowed"));
        }
    }
}
