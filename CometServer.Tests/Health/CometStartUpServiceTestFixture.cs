// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CometStartUpServiceTestFixture" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
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

namespace CometServer.Tests.Health
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using CometServer.Configuration;
    using CometServer.Health;
    using CometServer.Services.DataStore;

    using Microsoft.AspNetCore.Hosting.Server;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class CometStartUpServiceTestFixture
    {
        private Mock<IAppConfigService> appConfigService;

        private Mock<ILogger<CometStartUpService>> logger;

        private Mock<IHostApplicationLifetime> applicationLifetime;

        private Mock<IDataStoreConnectionChecker> dataStoreConnectionChecker;

        private Mock<IMigrationEngine> migrateEngine;

        private Mock<ICometHasStartedService> cometHasStartedService;

        private TestCometStartUpService testCometStartUpService;

        [SetUp]
        public void SetUp()
        {
            var appConfig = new AppConfig();
            this.appConfigService = new Mock<IAppConfigService>();
            this.appConfigService.Setup(x => x.AppConfig).Returns(appConfig);

            this.logger = new Mock<ILogger<CometStartUpService>>();
            this.applicationLifetime = new Mock<IHostApplicationLifetime>();
            this.dataStoreConnectionChecker = new Mock<IDataStoreConnectionChecker>();
            this.migrateEngine = new Mock<IMigrationEngine>();
            this.cometHasStartedService = new Mock<ICometHasStartedService>();

            this.testCometStartUpService = new TestCometStartUpService(this.applicationLifetime.Object)
            {
                AppConfigService = this.appConfigService.Object,
                Logger = this.logger.Object,
                DataStoreConnectionChecker = this.dataStoreConnectionChecker.Object,
                MigrationEngine = this.migrateEngine.Object,
                CometHasStartedService = this.cometHasStartedService.Object
            };
        }

        [Test]
        public async Task Verify_that_ExecuteAsync_performs_as_expected_in_success_scenario()
        {
            var cts = new CancellationTokenSource();

            this.dataStoreConnectionChecker.Setup(x => x.CheckConnectionAsync(cts.Token)).ReturnsAsync(DataStoreConnectionStatus.Connected);
            this.migrateEngine.Setup(x => x.MigrateAllAtStartUpAsync()).ReturnsAsync(true);

            await this.testCometStartUpService.ExecuteAsync(cts.Token);

            this.cometHasStartedService.Verify(x => x.SetHasStartedAndIsReady(true), Times.Once);
        }

        [Test]
        public async Task Verify_that_if_dbase_connection_check_fails_app_stops()
        {
            var cts = new CancellationTokenSource();

            this.dataStoreConnectionChecker.Setup(x => x.CheckConnectionAsync(cts.Token)).ReturnsAsync(DataStoreConnectionStatus.TimedOut);
            
            await this.testCometStartUpService.ExecuteAsync(cts.Token);

            this.applicationLifetime.Verify(x => x.StopApplication(), Times.Once);
            this.cometHasStartedService.Verify(x => x.SetHasStartedAndIsReady(true), Times.Never);
        }

        [Test]
        public async Task Verify_that_if_TLS_settings_are_invalid_app_stops_before_connecting()
        {
            var cts = new CancellationTokenSource();

            this.appConfigService.Object.AppConfig.Backtier.RootCertificate = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());

            await this.testCometStartUpService.ExecuteAsync(cts.Token);

            this.applicationLifetime.Verify(x => x.StopApplication(), Times.Once);
            this.dataStoreConnectionChecker.Verify(x => x.CheckConnectionAsync(It.IsAny<CancellationToken>()), Times.Never);
            this.cometHasStartedService.Verify(x => x.SetHasStartedAndIsReady(true), Times.Never);
        }

        [Test]
        public async Task Verify_that_if_the_connection_fails_with_a_configuration_error_app_stops()
        {
            var cts = new CancellationTokenSource();

            this.dataStoreConnectionChecker.Setup(x => x.CheckConnectionAsync(cts.Token)).ReturnsAsync(DataStoreConnectionStatus.ConfigurationError);

            await this.testCometStartUpService.ExecuteAsync(cts.Token);

            this.applicationLifetime.Verify(x => x.StopApplication(), Times.Once);
            this.migrateEngine.Verify(x => x.MigrateAllAtStartUpAsync(), Times.Never);
            this.cometHasStartedService.Verify(x => x.SetHasStartedAndIsReady(true), Times.Never);
        }

        [Test]
        public async Task Verify_that_ready_is_logged_only_after_a_successful_startup()
        {
            var cts = new CancellationTokenSource();

            var applicationStarted = new CancellationTokenSource();
            applicationStarted.Cancel();
            this.applicationLifetime.Setup(x => x.ApplicationStarted).Returns(applicationStarted.Token);

            this.dataStoreConnectionChecker.Setup(x => x.CheckConnectionAsync(cts.Token)).ReturnsAsync(DataStoreConnectionStatus.ConfigurationError);

            await this.testCometStartUpService.ExecuteAsync(cts.Token);

            this.VerifyReadyLogged(Times.Never());

            this.dataStoreConnectionChecker.Setup(x => x.CheckConnectionAsync(cts.Token)).ReturnsAsync(DataStoreConnectionStatus.Connected);
            this.migrateEngine.Setup(x => x.MigrateAllAtStartUpAsync()).ReturnsAsync(true);

            await this.testCometStartUpService.ExecuteAsync(cts.Token);

            this.VerifyReadyLogged(Times.Once());
        }

        private void VerifyReadyLogged(Times times)
        {
            this.logger.Verify(x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) => state.ToString().Contains("ready to accept requests")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                times);
        }

        [Test]
        public async Task Verify_that_ready_is_logged_with_the_listening_addresses()
        {
            var cts = new CancellationTokenSource();

            var applicationStarted = new CancellationTokenSource();
            applicationStarted.Cancel();
            this.applicationLifetime.Setup(x => x.ApplicationStarted).Returns(applicationStarted.Token);

            var serverAddressesFeature = new ServerAddressesFeature();
            serverAddressesFeature.Addresses.Add("http://0.0.0.0:5000");
            serverAddressesFeature.Addresses.Add("https://0.0.0.0:5001");

            var features = new FeatureCollection();
            features.Set<IServerAddressesFeature>(serverAddressesFeature);

            var server = new Mock<IServer>();
            server.Setup(x => x.Features).Returns(features);
            this.testCometStartUpService.Server = server.Object;

            this.dataStoreConnectionChecker.Setup(x => x.CheckConnectionAsync(cts.Token)).ReturnsAsync(DataStoreConnectionStatus.Connected);
            this.migrateEngine.Setup(x => x.MigrateAllAtStartUpAsync()).ReturnsAsync(true);

            await this.testCometStartUpService.ExecuteAsync(cts.Token);

            this.VerifyLogged(LogLevel.Information, "ready to accept requests at http://0.0.0.0:5000, https://0.0.0.0:5001", Times.Once());
        }

        [TestCase(DataStoreConnectionStatus.ConfigurationError)]
        [TestCase(DataStoreConnectionStatus.TimedOut)]
        public async Task Verify_that_a_failed_connection_sets_the_failure_exit_code(DataStoreConnectionStatus status)
        {
            var cts = new CancellationTokenSource();

            this.dataStoreConnectionChecker.Setup(x => x.CheckConnectionAsync(cts.Token)).ReturnsAsync(status);

            await this.testCometStartUpService.ExecuteAsync(cts.Token);

            Assert.That(this.testCometStartUpService.StartupFailedExitCodeSetCount, Is.EqualTo(1));
        }

        [Test]
        public async Task Verify_that_invalid_TLS_settings_set_the_failure_exit_code()
        {
            this.appConfigService.Object.AppConfig.Backtier.RootCertificate = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());

            await this.testCometStartUpService.ExecuteAsync(CancellationToken.None);

            Assert.That(this.testCometStartUpService.StartupFailedExitCodeSetCount, Is.EqualTo(1));
        }

        [Test]
        public async Task Verify_that_an_unexpected_exception_sets_the_failure_exit_code()
        {
            var cts = new CancellationTokenSource();

            this.dataStoreConnectionChecker.Setup(x => x.CheckConnectionAsync(cts.Token)).ThrowsAsync(new InvalidOperationException("boom"));

            await this.testCometStartUpService.ExecuteAsync(cts.Token);

            Assert.That(this.testCometStartUpService.StartupFailedExitCodeSetCount, Is.EqualTo(1));
            this.applicationLifetime.Verify(x => x.StopApplication(), Times.Once);
        }

        [Test]
        public async Task Verify_that_a_successful_startup_does_not_set_the_failure_exit_code()
        {
            var cts = new CancellationTokenSource();

            this.dataStoreConnectionChecker.Setup(x => x.CheckConnectionAsync(cts.Token)).ReturnsAsync(DataStoreConnectionStatus.Connected);
            this.migrateEngine.Setup(x => x.MigrateAllAtStartUpAsync()).ReturnsAsync(true);

            await this.testCometStartUpService.ExecuteAsync(cts.Token);

            Assert.That(this.testCometStartUpService.StartupFailedExitCodeSetCount, Is.Zero);
            this.applicationLifetime.Verify(x => x.StopApplication(), Times.Never);
        }

        [Test]
        public async Task Verify_that_a_shutdown_during_startup_is_not_a_startup_failure()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            this.dataStoreConnectionChecker.Setup(x => x.CheckConnectionAsync(cts.Token)).ThrowsAsync(new OperationCanceledException(cts.Token));

            await this.testCometStartUpService.ExecuteAsync(cts.Token);

            Assert.That(this.testCometStartUpService.StartupFailedExitCodeSetCount, Is.Zero);
            this.applicationLifetime.Verify(x => x.StopApplication(), Times.Never);
            this.VerifyLogged(LogLevel.Critical, "terminated", Times.Never());
        }

        private void VerifyLogged(LogLevel logLevel, string expectedText, Times times)
        {
            this.logger.Verify(x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) => state.ToString().Contains(expectedText)),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                times);
        }

        private class TestCometStartUpService : CometStartUpService
        {
            public TestCometStartUpService(IHostApplicationLifetime applicationLifetime) : base(applicationLifetime)
            {
            }

            public int StartupFailedExitCodeSetCount { get; private set; }

            public new Task ExecuteAsync(CancellationToken cancellationToken)
            {
                return base.ExecuteAsync(cancellationToken);
            }

            protected override void SetStartupFailedExitCode()
            {
                // do not change the exit code of the test process
                this.StartupFailedExitCodeSetCount++;
            }
        }
    }
}
