// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptionUtilsTestFixture.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WspDatabaseAuthentication.Tests
{
    using CDP4Authentication;

    using NUnit.Framework;

    [TestFixture]
    public class EncryptionUtilsTestFixture
    {
        [Test]
        public void VerifyThatWspPasswordCompareWorks()
        {
            const string password = "pass";
            var salt = EncryptionUtils.GenerateRandomSaltString();
            var serverSalt = EncryptionUtils.GenerateRandomSaltString();

            var encryptedPassword = CDP4WspDatabaseAuthentication.EncryptionUtils.BuildWspSaltedString(password,
                salt, serverSalt);

            const string passwordToTest = "pass";
            const string wrongPasswordToTest = "pass1";

            Assert.IsTrue(CDP4WspDatabaseAuthentication.EncryptionUtils.CompareWspSaltedString(passwordToTest, encryptedPassword, salt, serverSalt));
            Assert.IsFalse(CDP4WspDatabaseAuthentication.EncryptionUtils.CompareWspSaltedString(wrongPasswordToTest, encryptedPassword, salt, serverSalt));
        }
    }
}
