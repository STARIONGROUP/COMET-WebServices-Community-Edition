// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CometStartUpService.cs" company="Starion Group S.A.">
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

namespace CometServer.Health
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using CometServer.Configuration;
    using CometServer.Services.DataStore;

    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The <see cref="CometStartUpService"/> is responsible for executing business logic to start up the CDP4-COMET server
    /// </summary>
    public class CometStartUpService : BackgroundService
    {
        /// <summary>
        /// The (injected) <see cref="IHostApplicationLifetime"/> used to stop the application
        /// in case an error occurs during startup
        /// </summary>
        private readonly IHostApplicationLifetime applicationLifetime;

        /// <summary>
        /// The (injected) <see cref="ILogger"/>
        /// </summary>
        public ILogger<CometStartUpService> Logger { get; set; }

        /// <summary>
        /// The (injected) <see cref="IAppConfigService"/> used to access the application configuraton settings
        /// </summary>
        public IAppConfigService AppConfigService { get; set; }

        /// <summary>
        /// The (injected) <see cref="IDataStoreConnectionChecker"/> used to check whether the datastore
        /// is available within the configured waiting time
        /// </summary>
        public IDataStoreConnectionChecker DataStoreConnectionChecker { get; set; }

        /// <summary>
        /// The (injected) <see cref="IMigrationEngine"/> used to perform any migrations at startup
        /// </summary>
        public IMigrationEngine MigrationEngine { get; set; }

        /// <summary>
        /// The (injected) <see cref="ICometHasStartedService"/> 
        /// </summary>
        public ICometHasStartedService CometHasStartedService { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CometStartUpService"/>
        /// </summary>
        /// <param name="applicationLifetime">
        /// The (injected) <see cref="IHostApplicationLifetime"/>
        /// </param>
        public CometStartUpService(IHostApplicationLifetime applicationLifetime)
        {
            this.applicationLifetime = applicationLifetime;
        }

        /// <summary>
        /// Executs the tasks to startup the CDP4-COMET server
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="cancellationToken"/> that can be used to cancel the operation
        /// </param>
        /// <returns>
        /// an awaitable <see cref="Task"/>
        /// </returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!await this.DataStoreConnectionChecker.CheckConnectionAsync(cancellationToken))
                {
                    this.Logger.LogCritical("The CDP4-COMET REST API has terminated - The data-store was not availble within the configured BacktierWaitTime: {BacktierWaitTime}", this.AppConfigService.AppConfig.Midtier.BacktierWaitTime);
                    this.applicationLifetime.StopApplication();
                    return;
                }

                if (!await this.MigrationEngine.MigrateAllAtStartUpAsync())
                {
                    this.Logger.LogWarning("The Migrations could not be completed");
                }

                this.CometHasStartedService.SetHasStartedAndIsReady(true);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                this.Logger.LogCritical(ex, "The CDP4-COMET REST API has terminated while booting up");

                // Stop the application
                this.applicationLifetime.StopApplication();
            }
        }
    }
}
