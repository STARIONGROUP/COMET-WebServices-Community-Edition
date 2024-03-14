// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptionUtilsTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
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

namespace CDP4Authentication.Tests
{
    using System;

    using CDP4Authentication;

    using NUnit.Framework;

    [TestFixture]
    public class EncryptionUtilsTestFixture
    {
        [Test]
        public void VerifyThatSaltIsGenerated()
        {
            var saltString1 = EncryptionUtils.GenerateRandomSaltString();
            Assert.That(saltString1, Is.Not.Null);
            Assert.That(saltString1.Length, Is.EqualTo(4 * (int)Math.Ceiling(32 / 3.0)));

            var saltString2 = EncryptionUtils.GenerateRandomSaltString();
            Assert.That(saltString1, Is.Not.EqualTo(saltString2));
            Assert.That(saltString2.Length, Is.EqualTo(4 * (int)Math.Ceiling(32 / 3.0)));
        }

        [Test]
        public void VerifyThatPasswordCompareWorks()
        {
            var password = "pass";
            var salt = EncryptionUtils.GenerateRandomSaltString();

            var passwordTest = "pass";
            var wrongPasswordTest = "testPassword121";

            var encryptedPassword = EncryptionUtils.GenerateSaltedString(password, salt);

            Assert.That(EncryptionUtils.CompareSaltedStrings(passwordTest, encryptedPassword, salt), Is.True);
            Assert.That(EncryptionUtils.CompareSaltedStrings(wrongPasswordTest, encryptedPassword, salt), Is.False);
        }

        [Test]
        public void VerifyThatByteConversionWorks()
        {
            var password = "testPassword";

            var bytes = EncryptionUtils.GetBytesFromString(password);
            var returned = EncryptionUtils.GetStringFromBytes(bytes);

            Assert.That(returned, Is.EqualTo(password));
        }
    }
}
