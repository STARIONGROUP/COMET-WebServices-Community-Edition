// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStoreConnectionCheckerTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Services.DataStore
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Sockets;
    using System.Security.Authentication;
    using System.Threading;
    using System.Threading.Tasks;

    using CometServer.Configuration;
    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.DataStore;

    using Microsoft.Extensions.Logging;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Suite of tests for the <see cref="DataStoreConnectionChecker"/>
    /// </summary>
    [TestFixture]
    public class DataStoreConnectionCheckerTestFixture
    {
        private Mock<IDataSource> dataSource;

        private Mock<IPostgreSqlTlsDiagnostics> postgreSqlTlsDiagnostics;

        private Mock<ILogger<DataStoreConnectionChecker>> logger;

        private AppConfig appConfig;

        private DataStoreConnectionChecker dataStoreConnectionChecker;

        [SetUp]
        public void SetUp()
        {
            this.appConfig = new AppConfig();
            this.appConfig.Midtier.BacktierWaitTime = 300;
            this.appConfig.Backtier.SslMode = SslMode.VerifyFull;

            var appConfigService = new Mock<IAppConfigService>();
            appConfigService.Setup(x => x.AppConfig).Returns(this.appConfig);

            this.dataSource = new Mock<IDataSource>();
            this.postgreSqlTlsDiagnostics = new Mock<IPostgreSqlTlsDiagnostics>();
            this.postgreSqlTlsDiagnostics.Setup(x => x.DiagnoseAsync(It.IsAny<BacktierConfig>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string> { "PROBLEM: the server certificate expired" });

            this.logger = new Mock<ILogger<DataStoreConnectionChecker>>();

            this.dataStoreConnectionChecker = new DataStoreConnectionChecker
            {
                AppConfigService = appConfigService.Object,
                DataSource = this.dataSource.Object,
                PostgreSqlTlsDiagnostics = this.postgreSqlTlsDiagnostics.Object,
                Logger = this.logger.Object,
                RetryDelay = TimeSpan.FromMilliseconds(100)
            };
        }

        [Test]
        public async Task Verify_that_a_rejected_certificate_stops_on_the_first_attempt_and_runs_the_diagnostics()
        {
            var exception = new NpgsqlException("Exception while performing SSL handshake", new AuthenticationException("The remote certificate was rejected by the provided RemoteCertificateValidationCallback."));

            this.dataSource.Setup(x => x.OpenNewConnectionAsync()).Returns(ValueTask.FromException<NpgsqlConnection>(exception));

            var stopwatch = Stopwatch.StartNew();

            var result = await this.dataStoreConnectionChecker.CheckConnectionAsync(CancellationToken.None);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.EqualTo(DataStoreConnectionStatus.ConfigurationError));
                Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(4)));
            });

            this.dataSource.Verify(x => x.OpenNewConnectionAsync(), Times.Once);
            this.postgreSqlTlsDiagnostics.Verify(x => x.DiagnoseAsync(this.appConfig.Backtier, It.IsAny<CancellationToken>()), Times.Once);
            this.VerifyLogged(LogLevel.Critical, "PROBLEM: the server certificate expired");
        }

        [Test]
        public async Task Verify_that_a_server_without_TLS_stops_on_the_first_attempt_without_diagnostics()
        {
            var exception = new NpgsqlException("SSL connection requested. No SSL enabled connection from this host is configured.");

            this.dataSource.Setup(x => x.OpenNewConnectionAsync()).Returns(ValueTask.FromException<NpgsqlConnection>(exception));

            var result = await this.dataStoreConnectionChecker.CheckConnectionAsync(CancellationToken.None);

            Assert.That(result, Is.EqualTo(DataStoreConnectionStatus.ConfigurationError));
            this.dataSource.Verify(x => x.OpenNewConnectionAsync(), Times.Once);
            this.postgreSqlTlsDiagnostics.Verify(x => x.DiagnoseAsync(It.IsAny<BacktierConfig>(), It.IsAny<CancellationToken>()), Times.Never);
            this.VerifyLogged(LogLevel.Critical, "does not accept TLS connections");
        }

        [Test]
        public async Task Verify_that_a_transient_error_is_retried_until_the_wait_time_and_logs_the_reason()
        {
            this.appConfig.Midtier.BacktierWaitTime = 1;

            var exception = new NpgsqlException("Failed to connect to 127.0.0.1:5432", new SocketException((int)SocketError.ConnectionRefused));

            this.dataSource.Setup(x => x.OpenNewConnectionAsync()).Returns(ValueTask.FromException<NpgsqlConnection>(exception));

            var result = await this.dataStoreConnectionChecker.CheckConnectionAsync(CancellationToken.None);

            Assert.That(result, Is.EqualTo(DataStoreConnectionStatus.TimedOut));
            this.postgreSqlTlsDiagnostics.Verify(x => x.DiagnoseAsync(It.IsAny<BacktierConfig>(), It.IsAny<CancellationToken>()), Times.Never);
            this.VerifyLogged(LogLevel.Information, "Failed to connect to 127.0.0.1:5432");
        }

        [Test]
        public async Task Verify_that_a_successful_connection_returns_Connected_and_warns_when_the_TLS_state_is_unknown()
        {
            // a connection that was never opened: reading pg_stat_ssl fails, which must not fail the check
            this.dataSource.Setup(x => x.OpenNewConnectionAsync()).Returns(() => ValueTask.FromResult(new NpgsqlConnection()));

            var result = await this.dataStoreConnectionChecker.CheckConnectionAsync(CancellationToken.None);

            Assert.That(result, Is.EqualTo(DataStoreConnectionStatus.Connected));
            this.VerifyLogged(LogLevel.Warning, "Could not determine whether the connection to the data store");
        }

        [Test]
        public void Verify_that_cancellation_is_not_treated_as_a_transient_error()
        {
            this.dataSource.Setup(x => x.OpenNewConnectionAsync()).Returns(ValueTask.FromException<NpgsqlConnection>(new OperationCanceledException()));

            Assert.That(async () => await this.dataStoreConnectionChecker.CheckConnectionAsync(CancellationToken.None), Throws.InstanceOf<OperationCanceledException>());

            this.dataSource.Verify(x => x.OpenNewConnectionAsync(), Times.Once);
            this.VerifyLogged(LogLevel.Information, "Waiting for the data store", Times.Never());
        }

        [Test]
        public async Task Verify_that_diagnostic_findings_without_problem_are_logged_as_information()
        {
            this.postgreSqlTlsDiagnostics.Setup(x => x.DiagnoseAsync(It.IsAny<BacktierConfig>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<string> { "The PostgreSQL server negotiated Tls13", "PROBLEM: the server certificate expired" });

            var exception = new NpgsqlException("Exception while performing SSL handshake", new AuthenticationException("rejected"));
            this.dataSource.Setup(x => x.OpenNewConnectionAsync()).Returns(ValueTask.FromException<NpgsqlConnection>(exception));

            await this.dataStoreConnectionChecker.CheckConnectionAsync(CancellationToken.None);

            this.VerifyLogged(LogLevel.Information, "The PostgreSQL server negotiated Tls13");
            this.VerifyLogged(LogLevel.Critical, "The PostgreSQL server negotiated Tls13", Times.Never());
            this.VerifyLogged(LogLevel.Critical, "PROBLEM: the server certificate expired");
        }

        [Test]
        public async Task Verify_that_a_handshake_failure_without_diagnostics_service_still_returns_ConfigurationError()
        {
            this.dataStoreConnectionChecker.PostgreSqlTlsDiagnostics = null;

            var exception = new NpgsqlException("Exception while performing SSL handshake", new AuthenticationException("rejected"));
            this.dataSource.Setup(x => x.OpenNewConnectionAsync()).Returns(ValueTask.FromException<NpgsqlConnection>(exception));

            var result = await this.dataStoreConnectionChecker.CheckConnectionAsync(CancellationToken.None);

            Assert.That(result, Is.EqualTo(DataStoreConnectionStatus.ConfigurationError));
        }

        [TestCase(SslMode.Disable, true)]
        [TestCase(SslMode.Allow, true)]
        [TestCase(SslMode.VerifyFull, false)]
        public void Verify_that_a_rejected_host_mentions_the_SslMode_when_the_connection_is_not_encrypted(SslMode sslMode, bool expectHint)
        {
            this.appConfig.Backtier.SslMode = sslMode;

            // a server message in another language than English
            var exception = new PostgresException("kein pg_hba.conf-Eintrag für Host »172.17.0.1«, Benutzer »cdp4«, Datenbank »cdp4server«, keine Verschlüsselung", "FATAL", "FATAL", PostgresErrorCodes.InvalidAuthorizationSpecification);

            var description = DataStoreConnectionChecker.DescribeConfigurationError(exception, this.appConfig.Backtier);

            Assert.Multiple(() =>
            {
                Assert.That(description, Does.Contain("pg_hba.conf"));
                Assert.That(description.Contains("set Backtier:SslMode to VerifyFull"), Is.EqualTo(expectHint));
            });
        }

        [Test]
        public async Task Verify_that_a_wrong_password_stops_on_the_first_attempt_without_diagnostics()
        {
            var exception = new PostgresException("password authentication failed for user \"cdp4\"", "FATAL", "FATAL", PostgresErrorCodes.InvalidPassword);

            this.dataSource.Setup(x => x.OpenNewConnectionAsync()).Returns(ValueTask.FromException<NpgsqlConnection>(exception));

            var result = await this.dataStoreConnectionChecker.CheckConnectionAsync(CancellationToken.None);

            Assert.That(result, Is.EqualTo(DataStoreConnectionStatus.ConfigurationError));
            this.dataSource.Verify(x => x.OpenNewConnectionAsync(), Times.Once);
            this.postgreSqlTlsDiagnostics.Verify(x => x.DiagnoseAsync(It.IsAny<BacktierConfig>(), It.IsAny<CancellationToken>()), Times.Never);
            this.VerifyLogged(LogLevel.Critical, "Check Backtier:UserName and Backtier:Password");
        }

        [Test]
        public async Task Verify_that_a_database_that_is_starting_up_is_retried()
        {
            this.appConfig.Midtier.BacktierWaitTime = 1;

            var exception = new PostgresException("the database system is starting up", "FATAL", "FATAL", PostgresErrorCodes.CannotConnectNow);

            this.dataSource.Setup(x => x.OpenNewConnectionAsync()).Returns(ValueTask.FromException<NpgsqlConnection>(exception));

            var result = await this.dataStoreConnectionChecker.CheckConnectionAsync(CancellationToken.None);

            Assert.That(result, Is.EqualTo(DataStoreConnectionStatus.TimedOut));
            this.VerifyLogged(LogLevel.Information, "57P03: the database system is starting up");
        }

        [TestCase(SslMode.VerifyFull, "server certificate was rejected")]
        [TestCase(SslMode.Require, "The TLS handshake failed (Backtier:SslMode Require)")]
        public void Verify_that_a_handshake_failure_is_a_configuration_error(SslMode sslMode, string expected)
        {
            var exception = new NpgsqlException("Exception while performing SSL handshake", new AuthenticationException("rejected"));

            this.appConfig.Backtier.SslMode = sslMode;

            Assert.That(DataStoreConnectionChecker.DescribeConfigurationError(exception, this.appConfig.Backtier), Does.Contain(expected));
        }

        [Test]
        public void Verify_that_a_server_requiring_encryption_is_a_configuration_error()
        {
            var exception = new PostgresException("no pg_hba.conf entry for host \"172.17.0.1\", user \"cdp4\", database \"cdp4server\", no encryption", "FATAL", "FATAL", PostgresErrorCodes.InvalidAuthorizationSpecification);

            this.appConfig.Backtier.SslMode = SslMode.Disable;

            Assert.That(DataStoreConnectionChecker.DescribeConfigurationError(exception, this.appConfig.Backtier), Does.Contain("only accepts encrypted connections").And.Contain("Backtier:SslMode is Disable"));
        }

        [TestCase(PostgresErrorCodes.InvalidPassword, "rejected the credentials of user 'cdp4'")]
        [TestCase(PostgresErrorCodes.InvalidAuthorizationSpecification, "does not allow user 'cdp4' to connect to database 'cdp4server' from this host")]
        [TestCase(PostgresErrorCodes.InvalidCatalogName, "The database 'cdp4server' does not exist")]
        [TestCase(PostgresErrorCodes.InsufficientPrivilege, "Grant the CONNECT privilege")]
        public void Verify_that_permanent_server_errors_are_configuration_errors(string sqlState, string expected)
        {
            this.appConfig.Backtier.UserName = "cdp4";
            this.appConfig.Backtier.Database = "cdp4server";

            var exception = new PostgresException("server message", "FATAL", "FATAL", sqlState);

            Assert.That(DataStoreConnectionChecker.DescribeConfigurationError(exception, this.appConfig.Backtier), Does.Contain(expected));
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task Verify_that_a_missing_database_is_retried_when_it_can_be_created_from_an_exchange_file(bool isDbSeedEnabled, bool isDbImportEnabled)
        {
            this.appConfig.Midtier.BacktierWaitTime = 1;
            this.appConfig.Backtier.Database = "cdp4server";
            this.appConfig.Backtier.IsDbSeedEnabled = isDbSeedEnabled;
            this.appConfig.Backtier.IsDbImportEnabled = isDbImportEnabled;

            var exception = new PostgresException("database \"cdp4server\" does not exist", "FATAL", "FATAL", PostgresErrorCodes.InvalidCatalogName);

            this.dataSource.Setup(x => x.OpenNewConnectionAsync()).Returns(ValueTask.FromException<NpgsqlConnection>(exception));

            var result = await this.dataStoreConnectionChecker.CheckConnectionAsync(CancellationToken.None);

            Assert.That(result, Is.EqualTo(DataStoreConnectionStatus.TimedOut));
            this.dataSource.Verify(x => x.OpenNewConnectionAsync(), Times.AtLeast(2));
            this.VerifyLogged(LogLevel.Information, "The database 'cdp4server' is created when an Annex C.3 file is posted to /Data/Exchange");
        }

        [Test]
        public void Verify_that_a_missing_database_is_a_configuration_error_when_it_cannot_be_created()
        {
            this.appConfig.Backtier.Database = "cdp4server";
            this.appConfig.Backtier.IsDbSeedEnabled = false;
            this.appConfig.Backtier.IsDbImportEnabled = false;

            var exception = new PostgresException("database \"cdp4server\" does not exist", "FATAL", "FATAL", PostgresErrorCodes.InvalidCatalogName);

            Assert.That(DataStoreConnectionChecker.DescribeConfigurationError(exception, this.appConfig.Backtier), Does.Contain("enable Backtier:IsDbSeedEnabled or Backtier:IsDbImportEnabled"));
        }

        [Test]
        public void Verify_that_transient_errors_are_not_configuration_errors()
        {
            Assert.Multiple(() =>
            {
                Assert.That(DataStoreConnectionChecker.DescribeConfigurationError(new NpgsqlException("Failed to connect", new SocketException()), this.appConfig.Backtier), Is.Null);
                Assert.That(DataStoreConnectionChecker.DescribeConfigurationError(new PostgresException("the database system is starting up", "FATAL", "FATAL", PostgresErrorCodes.CannotConnectNow), this.appConfig.Backtier), Is.Null);
                Assert.That(DataStoreConnectionChecker.DescribeConfigurationError(new PostgresException("sorry, too many clients already", "FATAL", "FATAL", PostgresErrorCodes.TooManyConnections), this.appConfig.Backtier), Is.Null);
            });
        }

        [Test]
        public void Verify_that_the_reason_contains_all_inner_messages_once()
        {
            var exception = new NpgsqlException("Exception while performing SSL handshake", new AuthenticationException("rejected", new InvalidOperationException("rejected")));

            Assert.That(DataStoreConnectionChecker.GetReason(exception), Is.EqualTo("Exception while performing SSL handshake -> rejected"));
        }

        private void VerifyLogged(LogLevel logLevel, string expectedText, Times? times = null)
        {
            this.logger.Verify(x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) => state.ToString().Contains(expectedText)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                times ?? Times.AtLeastOnce());
        }
    }
}
