// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EncryptionUtils.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4Authentication
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Provides static helper methods to help with encryption
    /// </summary>
    public static class EncryptionUtils
    {
        /// <summary>
        /// The maximum number of bytes in a salt byte array.
        /// </summary>
        private const int SaltMaximumNumberOfBytes = 32;

        /// <summary>
        /// Generates a salted hash from an input string.
        /// </summary>
        /// <param name="inputString">
        /// The input string.
        /// </param>
        /// <param name="salt">
        /// The salt byte array.
        /// </param>
        /// <returns>
        /// A salted byte array.
        /// </returns>
        private static byte[] GenerateSaltedHash(string inputString, byte[] salt)
        {
            var algorithm = new SHA256Managed();
            var plainText = GetBytesFromString(inputString);
            var plainTextWithSaltBytes = new byte[plainText.Length + salt.Length];

            for (var i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }

            for (var i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        /// <summary>
        /// Generates a base 64 salted string from an input string and a byte array salt.
        /// </summary>
        /// <param name="inputString">
        /// The input string.
        /// </param>
        /// <param name="salt">
        /// The salt string.
        /// </param>
        /// <returns>
        /// Salted base 64 string.
        /// </returns>
        public static string GenerateSaltedString(string inputString, string salt)
        {
            var saltBytes = GetBytesFromBase64String(salt);
            return Convert.ToBase64String(GenerateSaltedHash(inputString, saltBytes));
        }

        /// <summary>
        /// Compares the input string with an encrypted string
        /// </summary>
        /// <param name="inputString">
        /// The input string.
        /// </param>
        /// <param name="encryptedString">
        /// The previously encoded string.
        /// </param>
        /// <param name="salt">
        /// The salt string.
        /// </param>
        /// <returns>
        /// True if the strings match.
        /// </returns>
        public static bool CompareSaltedStrings(string inputString, string encryptedString, string salt)
        {
            return string.Equals(GenerateSaltedString(inputString, salt), encryptedString);
        }

        /// <summary>
        /// Compares the input string with an encrypted string. (WSP specific)
        /// </summary>
        /// <param name="password">
        /// The input password string.
        /// </param>
        /// <param name="encryptedPassword">
        /// The previously password encoded string.
        /// </param>
        /// <param name="salt">
        /// The salt string.
        /// </param>
        /// <param name="serverSalt">
        /// The WSP server salt string.
        /// </param>
        /// <returns>
        /// True if the strings match.
        /// </returns>
        public static bool CompareWspSaltedString(string password, string encryptedPassword, string salt, string serverSalt)
        {
            return BuildWspSaltedString(password, salt, serverSalt).ToLower().Equals(encryptedPassword.ToLower());
        }

        /// <summary>
        /// Generates a base 64 salted string from an input string and a byte array salt. (WSP specific)
        /// </summary>
        /// <param name="password">
        /// The input password string.
        /// </param>
        /// <param name="salt">
        /// The salt string.
        /// </param>
        /// <param name="serverSalt">
        /// The WSP server salt string.
        /// </param>
        /// <returns>
        /// Salted base 64 string.
        /// </returns>
        public static string BuildWspSaltedString(string password, string salt, string serverSalt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltBytes = Encoding.UTF8.GetBytes(salt);
            var serverSaltBytes = Encoding.UTF8.GetBytes(serverSalt);

            string hashedPassword;

            using (var hasher = SHA256.Create())
            {
                hasher.Initialize();
                hasher.TransformBlock(serverSaltBytes, 0, serverSaltBytes.Length, null, 0);
                hasher.TransformBlock(passwordBytes, 0, passwordBytes.Length, null, 0);
                hasher.TransformFinalBlock(saltBytes, 0, saltBytes.Length);

                hashedPassword = BitConverter.ToString(hasher.Hash).Replace("-", "");
            }

            return hashedPassword;
        }

        /// <summary>
        /// Converts a string to a byte array.
        /// </summary>
        /// <remarks>
        /// source <see cref="http://stackoverflow.com/questions/472906/how-to-get-a-consistent-byte-representation-of-strings-in-c-sharp-without-manual"/>
        /// </remarks>
        /// <param name="inputString">
        /// The string to convert.
        /// </param>
        /// <returns>
        /// A byte array representation of the string
        /// </returns>
        public static byte[] GetBytesFromString(string inputString)
        {
            var bytes = new byte[inputString.Length * sizeof(char)];
            Buffer.BlockCopy(inputString.ToCharArray(), 0, bytes, 0, bytes.Length);

            return bytes;
        }

        /// <summary>
        /// Converts a byte array to a string.
        /// </summary>
        /// <remarks>
        /// source <see cref="http://stackoverflow.com/questions/472906/how-to-get-a-consistent-byte-representation-of-strings-in-c-sharp-without-manual"/>
        /// </remarks>
        /// <param name="bytes">
        /// The byte array to convert.
        /// </param>
        /// <returns>
        /// A string representation of the byte array.
        /// </returns>
        public static string GetStringFromBytes(byte[] bytes)
        {
            var chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);

            return new string(chars);
        }

        /// <summary>
        /// Generates a random 32 bytes non zero byte salt string
        /// </summary>
        /// <remarks>
        /// source <see cref="http://codereview.stackexchange.com/questions/93614/salt-generation-in-c"/>
        /// </remarks>
        /// <returns>
        /// A random 32 bytes non zero byte salt.
        /// </returns>
        private static byte[] GenerateRandomSaltBytes()
        {
            var random = new RNGCryptoServiceProvider();
            var salt = new byte[SaltMaximumNumberOfBytes];

            // Build the random bytes
            random.GetNonZeroBytes(salt);
            return salt;
        }

        /// <summary>
        /// Generates a random 32 bytes non zero byte salt string
        /// </summary>
        /// <remarks>
        /// source <see cref="http://codereview.stackexchange.com/questions/93614/salt-generation-in-c"/>
        /// </remarks>
        /// <returns>
        /// A random 32 bytes non zero byte salt string.
        /// </returns>
        public static string GenerateRandomSaltString()
        {
            return Convert.ToBase64String(GenerateRandomSaltBytes());
        }

        /// <summary>
        /// Converts a base 64 string to a byte array.
        /// </summary>
        /// <remarks>The input string really should be a Base64String to make any sense.</remarks>
        /// <param name="inputString">
        /// The string to convert.
        /// </param>
        /// <returns>
        /// A byte array representation of the string.
        /// </returns>
        private static byte[] GetBytesFromBase64String(string inputString)
        {
            return Convert.FromBase64String(inputString);
        }
    }
}
