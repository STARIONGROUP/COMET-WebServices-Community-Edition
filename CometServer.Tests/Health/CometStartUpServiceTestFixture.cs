// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CometStartUpServiceTestFixture" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
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
    using System.Threading;
    using System.Threading.Tasks;

    using CometServer.Configuration;
    using CometServer.Health;
    using CometServer.Services.DataStore;

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

            this.dataStoreConnectionChecker.Setup(x => x.CheckConnectionAsync(cts.Token)).Returns(true);
            this.migrateEngine.Setup(x => x.MigrateAllAtStartUpAsync()).Returns(true);

            await this.testCometStartUpService.ExecuteAsync(cts.Token);

            this.cometHasStartedService.Verify(x => x.SetHasStartedAndIsReady(true), Times.Once);
        }

        [Test]
        public async Task Verify_that_if_dbase_connection_check_fails_app_stops()
        {
            var cts = new CancellationTokenSource();

            this.dataStoreConnectionChecker.Setup(x => x.CheckConnectionAsync(cts.Token)).Returns(false);
            
            await this.testCometStartUpService.ExecuteAsync(cts.Token);

            this.applicationLifetime.Verify(x => x.StopApplication(), Times.Once);
            this.cometHasStartedService.Verify(x => x.SetHasStartedAndIsReady(true), Times.Never);
        }

        private class TestCometStartUpService : CometStartUpService
        {
            public TestCometStartUpService(IHostApplicationLifetime applicationLifetime) : base(applicationLifetime)
            {
            }

            public new Task ExecuteAsync(CancellationToken cancellationToken)
            {
                return base.ExecuteAsync(cancellationToken);
            }
        }
    }
}
