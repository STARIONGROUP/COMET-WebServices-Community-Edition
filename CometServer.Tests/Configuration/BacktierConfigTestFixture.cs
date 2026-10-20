// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BacktierConfigServiceTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the Starion implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Tests.Configuration
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;

    using CometServer.Configuration;

    using Microsoft.Extensions.Configuration;

    using Npgsql;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="BacktierConfig"/>
    /// </summary>
    [TestFixture]
    public class BacktierConfigTestFixture
    {
        private Mock<IConfiguration> configuration;

        private string tempDirectory;

        private X509Certificate2 certificateAuthority;

        private X509Certificate2 clientCertificate;

        private string certificateAuthorityFile;

        private string clientCertificateFile;

        private string clientKeyFile;

        private string encryptedClientKeyFile;

        private string clientPfxFile;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            this.tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(this.tempDirectory);

            this.certificateAuthority = TestCertificateFactory.CreateCertificateAuthority("CN=COMET Test CA");
            this.clientCertificate = TestCertificateFactory.CreateCertificate(this.certificateAuthority, "CN=cdp4", DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddDays(1));

            this.certificateAuthorityFile = this.WriteFile("ca.crt", this.certificateAuthority.ExportCertificatePem());
            this.clientCertificateFile = this.WriteFile("client.crt", this.clientCertificate.ExportCertificatePem());
            this.clientKeyFile = this.WriteFile("client.key", this.clientCertificate.GetRSAPrivateKey().ExportPkcs8PrivateKeyPem());

            var pbeParameters = new PbeParameters(PbeEncryptionAlgorithm.Aes256Cbc, HashAlgorithmName.SHA256, 100_000);
            this.encryptedClientKeyFile = this.WriteFile("client-encrypted.key", this.clientCertificate.GetRSAPrivateKey().ExportEncryptedPkcs8PrivateKeyPem("secret", pbeParameters));

            this.clientPfxFile = Path.Combine(this.tempDirectory, "client.pfx");
            File.WriteAllBytes(this.clientPfxFile, this.clientCertificate.Export(X509ContentType.Pfx, "secret"));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            this.certificateAuthority.Dispose();
            this.clientCertificate.Dispose();
            Directory.Delete(this.tempDirectory, true);
        }

        [SetUp]
        public void SetUp()
        {
            // Build the IConfiguration instance
            this.configuration = new Mock<IConfiguration>();
            this.configuration.Setup(c => c[It.IsAny<string>()]).Returns(string.Empty);
        }

        [Test]
        public void Verify_that_empty_or_missing_settings_do_not_throw()
        {
            Assert.That(() => new BacktierConfig(this.configuration.Object), Throws.Nothing);
        }

        [Test]
        public void Verify_that_TLS_defaults_keep_the_current_behaviour()
        {
            var fromConfiguration = new BacktierConfig(this.configuration.Object);
            var fromDefaults = new BacktierConfig();

            Assert.Multiple(() =>
            {
                Assert.That(fromConfiguration.SslMode, Is.EqualTo(SslMode.Prefer));
                Assert.That(fromConfiguration.CheckCertificateRevocation, Is.False);
                Assert.That(fromDefaults.SslMode, Is.EqualTo(SslMode.Prefer));
                Assert.That(fromDefaults.CheckCertificateRevocation, Is.False);
            });
        }

        [TestCase("VerifyFull", SslMode.VerifyFull)]
        [TestCase("verifyfull", SslMode.VerifyFull)]
        [TestCase(" Require ", SslMode.Require)]
        [TestCase("Disable", SslMode.Disable)]
        public void Verify_that_SslMode_is_parsed(string value, SslMode expected)
        {
            this.configuration.Setup(c => c["Backtier:SslMode"]).Returns(value);

            var backtierConfig = new BacktierConfig(this.configuration.Object);

            Assert.That(backtierConfig.SslMode, Is.EqualTo(expected));
        }

        [TestCase("VerifyFul")]
        [TestCase("7")]
        [TestCase("Allow, Prefer")]
        public void Verify_that_an_invalid_SslMode_throws_and_lists_the_valid_values(string value)
        {
            this.configuration.Setup(c => c["Backtier:SslMode"]).Returns(value);

            Assert.That(() => new BacktierConfig(this.configuration.Object),
                Throws.TypeOf<ConfigurationErrorsException>()
                    .With.Message.Contains($"'{value}'")
                    .And.Message.Contains("Disable, Allow, Prefer, Require, VerifyCA, VerifyFull"));
        }

        [TestCase("true", true)]
        [TestCase(" TRUE ", true)]
        [TestCase("false", false)]
        public void Verify_that_CheckCertificateRevocation_is_parsed(string value, bool expected)
        {
            this.configuration.Setup(c => c["Backtier:CheckCertificateRevocation"]).Returns(value);

            Assert.That(new BacktierConfig(this.configuration.Object).CheckCertificateRevocation, Is.EqualTo(expected));
        }

        [TestCase("yes")]
        [TestCase("1")]
        public void Verify_that_an_invalid_CheckCertificateRevocation_throws(string value)
        {
            this.configuration.Setup(c => c["Backtier:CheckCertificateRevocation"]).Returns(value);

            Assert.That(() => new BacktierConfig(this.configuration.Object),
                Throws.TypeOf<ConfigurationErrorsException>().With.Message.Contains($"'{value}'").And.Message.Contains("true, false"));
        }

        [Test]
        public void Verify_that_certificate_settings_are_read()
        {
            this.configuration.Setup(c => c["Backtier:RootCertificate"]).Returns("/app/certs/ca.crt");
            this.configuration.Setup(c => c["Backtier:ClientCertificate"]).Returns("/app/certs/client.crt");
            this.configuration.Setup(c => c["Backtier:ClientCertificateKey"]).Returns("/app/certs/client.key");
            this.configuration.Setup(c => c["Backtier:ClientCertificatePassword"]).Returns("secret");
            this.configuration.Setup(c => c["Backtier:CheckCertificateRevocation"]).Returns("true");

            var backtierConfig = new BacktierConfig(this.configuration.Object);

            Assert.Multiple(() =>
            {
                Assert.That(backtierConfig.RootCertificate, Is.EqualTo("/app/certs/ca.crt"));
                Assert.That(backtierConfig.ClientCertificate, Is.EqualTo("/app/certs/client.crt"));
                Assert.That(backtierConfig.ClientCertificateKey, Is.EqualTo("/app/certs/client.key"));
                Assert.That(backtierConfig.ClientCertificatePassword, Is.EqualTo("secret"));
                Assert.That(backtierConfig.CheckCertificateRevocation, Is.True);
            });
        }

        [Test]
        public void Verify_that_missing_certificate_files_are_reported()
        {
            var missingFile = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            var backtierConfig = new BacktierConfig
            {
                RootCertificate = missingFile,
                ClientCertificate = missingFile,
                ClientCertificateKey = missingFile
            };

            var errors = backtierConfig.ValidateTlsSettings().ToList();

            Assert.Multiple(() =>
            {
                Assert.That(errors, Has.Count.EqualTo(3));
                Assert.That(errors, Has.Some.Contains("Backtier:RootCertificate"));
                Assert.That(errors, Has.Some.Contains("Backtier:ClientCertificate "));
                Assert.That(errors, Has.Some.Contains("Backtier:ClientCertificateKey"));
            });
        }

        [Test]
        public void Verify_that_valid_PEM_certificate_files_are_accepted()
        {
            var backtierConfig = new BacktierConfig
            {
                RootCertificate = this.certificateAuthorityFile,
                ClientCertificate = this.clientCertificateFile,
                ClientCertificateKey = this.clientKeyFile
            };

            Assert.That(backtierConfig.ValidateTlsSettings(), Is.Empty);
        }

        [Test]
        public void Verify_that_a_DER_root_certificate_is_accepted()
        {
            var derFile = Path.Combine(this.tempDirectory, "ca.der");
            File.WriteAllBytes(derFile, this.certificateAuthority.RawData);

            var backtierConfig = new BacktierConfig { RootCertificate = derFile };

            Assert.That(backtierConfig.ValidateTlsSettings(), Is.Empty);
        }

        [Test]
        public void Verify_that_a_root_certificate_that_is_not_a_certificate_is_reported()
        {
            var backtierConfig = new BacktierConfig { RootCertificate = this.WriteFile("not-a-certificate.crt", "this is not a certificate") };

            Assert.That(backtierConfig.ValidateTlsSettings(), Has.One.Contains("is not a valid PEM or DER certificate").And.Contains("Backtier:RootCertificate"));
        }

        [Test]
        public void Verify_that_a_root_certificate_with_a_corrupt_PEM_block_is_reported()
        {
            var backtierConfig = new BacktierConfig { RootCertificate = this.WriteFile("corrupt.crt", "-----BEGIN CERTIFICATE-----\nbm90IGEgY2VydGlmaWNhdGU=\n-----END CERTIFICATE-----\n") };

            Assert.That(backtierConfig.ValidateTlsSettings(), Has.One.Contains("Backtier:RootCertificate"));
        }

        [Test]
        public void Verify_that_an_encrypted_client_key_with_the_right_password_is_accepted()
        {
            var backtierConfig = new BacktierConfig
            {
                ClientCertificate = this.clientCertificateFile,
                ClientCertificateKey = this.encryptedClientKeyFile,
                ClientCertificatePassword = "secret"
            };

            Assert.That(backtierConfig.ValidateTlsSettings(), Is.Empty);
        }

        [Test]
        public void Verify_that_an_encrypted_client_key_with_a_wrong_password_is_reported()
        {
            var backtierConfig = new BacktierConfig
            {
                ClientCertificate = this.clientCertificateFile,
                ClientCertificateKey = this.encryptedClientKeyFile,
                ClientCertificatePassword = "wrong"
            };

            Assert.That(backtierConfig.ValidateTlsSettings(), Has.One.Contains("could not be loaded").And.Contains("Backtier:ClientCertificatePassword"));
        }

        [Test]
        public void Verify_that_a_PFX_client_certificate_with_the_right_password_is_accepted()
        {
            var backtierConfig = new BacktierConfig
            {
                ClientCertificate = this.clientPfxFile,
                ClientCertificatePassword = "secret"
            };

            Assert.That(backtierConfig.ValidateTlsSettings(), Is.Empty);
        }

        [Test]
        public void Verify_that_a_PFX_client_certificate_with_a_wrong_password_is_reported()
        {
            var backtierConfig = new BacktierConfig
            {
                ClientCertificate = this.clientPfxFile,
                ClientCertificatePassword = "wrong"
            };

            Assert.That(backtierConfig.ValidateTlsSettings(), Has.One.Contains("could not be loaded").And.Contains("Backtier:ClientCertificatePassword"));
        }

        [Test]
        public void Verify_that_a_PEM_client_certificate_without_key_is_reported()
        {
            var backtierConfig = new BacktierConfig { ClientCertificate = this.clientCertificateFile };

            Assert.That(backtierConfig.ValidateTlsSettings(), Has.One.Contains("Backtier:ClientCertificate"));
        }

        [Test]
        public void Verify_that_a_client_key_without_client_certificate_is_reported()
        {
            var backtierConfig = new BacktierConfig { ClientCertificateKey = this.clientKeyFile };

            Assert.That(backtierConfig.ValidateTlsSettings(), Has.One.Contains("Backtier:ClientCertificate is not"));
        }

        [Test]
        [Platform("Win", Reason = "Only Windows prevents reading a file that another handle opened with FileShare.None")]
        public void Verify_that_an_unreadable_certificate_file_is_reported()
        {
            var lockedFile = this.WriteFile("locked.crt", this.certificateAuthority.ExportCertificatePem());

            using var lockingStream = new FileStream(lockedFile, FileMode.Open, FileAccess.Read, FileShare.None);

            var backtierConfig = new BacktierConfig { RootCertificate = lockedFile };

            Assert.That(backtierConfig.ValidateTlsSettings(), Has.One.Contains("cannot be read"));
        }

        [Test]
        public void Verify_that_the_configured_root_certificate_takes_precedence()
        {
            var backtierConfig = new BacktierConfig { RootCertificate = "/app/certs/ca.crt" };

            var rootCertificate = backtierConfig.GetEffectiveRootCertificate("/env/ca.crt", this.certificateAuthorityFile);

            Assert.Multiple(() =>
            {
                Assert.That(rootCertificate.Path, Is.EqualTo("/app/certs/ca.crt"));
                Assert.That(rootCertificate.Source, Is.EqualTo("Backtier:RootCertificate"));
            });
        }

        [Test]
        public void Verify_that_PGSSLROOTCERT_is_used_when_no_root_certificate_is_configured()
        {
            var rootCertificate = new BacktierConfig().GetEffectiveRootCertificate("/env/ca.crt", this.certificateAuthorityFile);

            Assert.Multiple(() =>
            {
                Assert.That(rootCertificate.Path, Is.EqualTo("/env/ca.crt"));
                Assert.That(rootCertificate.Source, Does.Contain("PGSSLROOTCERT"));
            });
        }

        [Test]
        public void Verify_that_the_default_root_certificate_is_used_only_when_it_exists()
        {
            var existing = new BacktierConfig().GetEffectiveRootCertificate(null, this.certificateAuthorityFile);
            var missing = new BacktierConfig().GetEffectiveRootCertificate(null, Path.Combine(this.tempDirectory, "missing-root.crt"));

            Assert.Multiple(() =>
            {
                Assert.That(existing.Path, Is.EqualTo(this.certificateAuthorityFile));
                Assert.That(existing.Source, Does.Contain("default root.crt"));
                Assert.That(missing.Path, Is.Null);
                Assert.That(missing.Source, Does.Contain("trust store of the operating system"));
            });
        }

        private string WriteFile(string name, string content)
        {
            var path = Path.Combine(this.tempDirectory, name);
            File.WriteAllText(path, content);
            return path;
        }
    }
}
