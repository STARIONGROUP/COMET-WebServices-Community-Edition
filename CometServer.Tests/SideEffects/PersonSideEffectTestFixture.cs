// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PersonSideEffectTestFixture.cs" company="RHEA System S.A.">
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

namespace CometServer.Tests.SideEffects
{
    using CDP4Common;

    using CDP4Orm.Dao;

    using CometServer.Services.Operations.SideEffects;

    using Moq;

    using Npgsql;

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

        /// <summary>
        /// The <see cref="GlossarySideEffect"/> that is being tested
        /// </summary>
        private PersonSideEffect personSideEffect;
        
        /// <summary>
        /// The mocked <see cref="ITermService"/> used to operate on <see cref="Term"/>s.
        /// </summary>
        private Mock<IPersonDao> personDao;

        private NpgsqlTransaction npgsqlTransaction;

        private ClasslessDTO rawUpdateinfo;

        [SetUp]
        public void SetUp()
        {
            this.npgsqlTransaction = null;

            this.personDao = new Mock<IPersonDao>();

            this.personSideEffect = new PersonSideEffect { PersonDao = this.personDao.Object };
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
            var expectedPasswordText = string.Format(
                "{1}{0}{1}",
                ClearTextPassword,
                this.personSideEffect.PersonDao.PasswordChangeToken);
            
            // assert same number of entries in raw update info object
            Assert.AreEqual(2, this.rawUpdateinfo.Count);
            Assert.AreEqual(TestValue, this.rawUpdateinfo[TestValue].ToString());

            // assert encapsulated cleartext password
            Assert.AreEqual(expectedPasswordText, this.rawUpdateinfo[PasswordKey].ToString());
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
            Assert.AreEqual(1, this.rawUpdateinfo.Count);
            Assert.AreEqual(TestValue, this.rawUpdateinfo[TestValue].ToString());
        }
    }
}
