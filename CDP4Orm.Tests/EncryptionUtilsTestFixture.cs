// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptionUtilsTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski
//
//    This file is part of COMET Web Services Community Edition. 
//    The COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Orm.Tests
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
            Assert.IsNotNull(saltString1);
            Assert.AreEqual(4 * (int)Math.Ceiling(32 / 3.0), saltString1.Length);

            var saltString2 = EncryptionUtils.GenerateRandomSaltString();
            Assert.AreNotEqual(saltString1,saltString2);
            Assert.AreEqual(4 * (int)Math.Ceiling(32 / 3.0), saltString2.Length);
        }

        [Test]
        public void VerifyThatPasswordCompareWorks()
        {
            var password = "pass";
            var salt = EncryptionUtils.GenerateRandomSaltString();

            Console.WriteLine("Salt: {0}", salt);

            var passwordTest = "pass";
            var wrongPasswordTest = "testPassword121";

            var encryptedPassword = EncryptionUtils.GenerateSaltedString(password,
                salt);

            Console.WriteLine("EncryptedPassword: {0}", encryptedPassword);

            Assert.IsTrue(EncryptionUtils.CompareSaltedStrings(passwordTest, encryptedPassword, salt));
            Assert.IsFalse(EncryptionUtils.CompareSaltedStrings(wrongPasswordTest, encryptedPassword, salt));
        }

        [Test]
        public void VerifyThatByteConversionWorks()
        {
            var password = "testPassword";

            var bytes = EncryptionUtils.GetBytesFromString(password);
            var returned = EncryptionUtils.GetStringFromBytes(bytes);

            Assert.AreEqual(password, returned);
        }
    }
}
