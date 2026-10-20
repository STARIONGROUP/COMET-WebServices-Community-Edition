// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UtilsConnectionStringTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2026 Starion Group S.A.
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

namespace CometServer.Tests.Services
{
    using CometServer.Configuration;
    using CometServer.Services;
    using CometServer.Services.DataStore;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the connection string methods of <see cref="Utils"/>
    /// </summary>
    [TestFixture]
    public class UtilsConnectionStringTestFixture
    {
        private BacktierConfig backtierConfig;

        [SetUp]
        public void SetUp()
        {
            this.backtierConfig = new BacktierConfig
            {
                HostName = "comet-db",
                Port = 5433,
                UserName = "cdp4",
                Password = "cdp4",
                Database = "cdp4server",
                DatabaseManage = "cdp4manage",
                StatementTimeout = 180,
                ConnectionTimeout = 15,
                Keepalive = 0,
                MaximumPoolSize = 90
            };
        }

        [Test]
        public void Verify_that_the_connection_string_contains_the_configured_settings()
        {
            var builder = new NpgsqlConnectionStringBuilder(Utils.GetConnectionString(this.backtierConfig));

            Assert.Multiple(() =>
            {
                Assert.That(builder.Host, Is.EqualTo("comet-db"));
                Assert.That(builder.Port, Is.EqualTo(5433));
                Assert.That(builder.Username, Is.EqualTo("cdp4"));
                Assert.That(builder.Password, Is.EqualTo("cdp4"));
                Assert.That(builder.Database, Is.EqualTo("cdp4server"));
                Assert.That(builder.CommandTimeout, Is.EqualTo(180));
                Assert.That(builder.Timeout, Is.EqualTo(15));
                Assert.That(builder.MaxPoolSize, Is.EqualTo(90));
                Assert.That(builder.SslMode, Is.EqualTo(SslMode.Prefer));
                Assert.That(builder.RootCertificate, Is.Null);
                Assert.That(builder.SslCertificate, Is.Null);
                Assert.That(builder.SslKey, Is.Null);
                Assert.That(builder.SslPassword, Is.Null);
                Assert.That(builder.CheckCertificateRevocation, Is.False);
            });
        }

        [Test]
        public void Verify_that_the_manage_connection_string_uses_the_manage_database_and_a_single_connection()
        {
            var builder = new NpgsqlConnectionStringBuilder(Utils.GetManageDatabaseConnectionString(this.backtierConfig));

            Assert.Multiple(() =>
            {
                Assert.That(builder.Database, Is.EqualTo("cdp4manage"));
                Assert.That(builder.MaxPoolSize, Is.EqualTo(1));
            });
        }

        [Test]
        public void Verify_that_a_maximum_pool_size_of_zero_keeps_the_npgsql_default()
        {
            this.backtierConfig.MaximumPoolSize = 0;

            var builder = new NpgsqlConnectionStringBuilder(Utils.GetConnectionString(this.backtierConfig));

            Assert.That(builder.MaxPoolSize, Is.EqualTo(new NpgsqlConnectionStringBuilder().MaxPoolSize));
        }

        [Test]
        public void Verify_that_keepalive_and_timeouts_are_set_on_both_connection_strings()
        {
            this.backtierConfig.Keepalive = 30;
            this.backtierConfig.ConnectionTimeout = 20;
            this.backtierConfig.StatementTimeout = 240;

            foreach (var connectionString in new[] { Utils.GetConnectionString(this.backtierConfig), Utils.GetManageDatabaseConnectionString(this.backtierConfig) })
            {
                var builder = new NpgsqlConnectionStringBuilder(connectionString);

                Assert.Multiple(() =>
                {
                    Assert.That(builder.KeepAlive, Is.EqualTo(30));
                    Assert.That(builder.Timeout, Is.EqualTo(20));
                    Assert.That(builder.CommandTimeout, Is.EqualTo(240));
                });
            }
        }

        [Test]
        public void Verify_that_zero_keepalive_and_connection_timeout_keep_the_npgsql_defaults()
        {
            this.backtierConfig.Keepalive = 0;
            this.backtierConfig.ConnectionTimeout = 0;

            var defaults = new NpgsqlConnectionStringBuilder();

            foreach (var connectionString in new[] { Utils.GetConnectionString(this.backtierConfig), Utils.GetManageDatabaseConnectionString(this.backtierConfig) })
            {
                var builder = new NpgsqlConnectionStringBuilder(connectionString);

                Assert.Multiple(() =>
                {
                    Assert.That(builder.KeepAlive, Is.EqualTo(defaults.KeepAlive));
                    Assert.That(builder.Timeout, Is.EqualTo(defaults.Timeout));
                });
            }
        }

        [TestCase("pa;ss")]
        [TestCase("pa=ss")]
        [TestCase("p'a\"ss")]
        [TestCase("pass word;Database=other")]
        public void Verify_that_passwords_with_special_characters_are_kept_intact(string password)
        {
            this.backtierConfig.Password = password;

            var builder = new NpgsqlConnectionStringBuilder(Utils.GetConnectionString(this.backtierConfig));
            var manageBuilder = new NpgsqlConnectionStringBuilder(Utils.GetManageDatabaseConnectionString(this.backtierConfig));

            Assert.Multiple(() =>
            {
                Assert.That(builder.Password, Is.EqualTo(password));
                Assert.That(builder.Database, Is.EqualTo("cdp4server"));
                Assert.That(manageBuilder.Password, Is.EqualTo(password));
                Assert.That(manageBuilder.Database, Is.EqualTo("cdp4manage"));
            });
        }

        [Test]
        public void Verify_that_both_connection_strings_get_the_same_TLS_settings()
        {
            this.backtierConfig.SslMode = SslMode.VerifyFull;
            this.backtierConfig.RootCertificate = "/app/certs/ca.crt";
            this.backtierConfig.ClientCertificate = "/app/certs/client.crt";
            this.backtierConfig.ClientCertificateKey = "/app/certs/client.key";
            this.backtierConfig.ClientCertificatePassword = "secret";
            this.backtierConfig.CheckCertificateRevocation = true;

            var builders = new[]
            {
                new NpgsqlConnectionStringBuilder(Utils.GetConnectionString(this.backtierConfig)),
                new NpgsqlConnectionStringBuilder(Utils.GetManageDatabaseConnectionString(this.backtierConfig))
            };

            foreach (var builder in builders)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(builder.SslMode, Is.EqualTo(SslMode.VerifyFull));
                    Assert.That(builder.RootCertificate, Is.EqualTo("/app/certs/ca.crt"));
                    Assert.That(builder.SslCertificate, Is.EqualTo("/app/certs/client.crt"));
                    Assert.That(builder.SslKey, Is.EqualTo("/app/certs/client.key"));
                    Assert.That(builder.SslPassword, Is.EqualTo("secret"));
                    Assert.That(builder.CheckCertificateRevocation, Is.True);
                });
            }
        }

        [TestCase(SslMode.VerifyFull, true, false)]
        [TestCase(SslMode.VerifyFull, false, true)]
        [TestCase(SslMode.VerifyCA, true, true)]
        [TestCase(SslMode.Require, true, true)]
        [TestCase(SslMode.Prefer, true, true)]
        [TestCase(SslMode.Prefer, false, true)]
        [TestCase(SslMode.Disable, false, true)]
        public void Verify_that_a_security_warning_is_required_unless_encrypted_and_fully_verified(SslMode sslMode, bool isSecure, bool expected)
        {
            Assert.That(DataStoreConnectionChecker.IsConnectionSecurityWarningRequired(sslMode, isSecure), Is.EqualTo(expected));
        }
    }
}
