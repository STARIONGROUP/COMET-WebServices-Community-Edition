// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptionUtilsTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
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
