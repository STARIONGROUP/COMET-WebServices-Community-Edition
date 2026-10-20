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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CometServer.Configuration;
    using CometServer.Services.DataStore;

    using Microsoft.AspNetCore.Hosting.Server;
    using Microsoft.AspNetCore.Hosting.Server.Features;
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
        /// The exit code of the process when the startup failed
        /// </summary>
        public const int StartupFailedExitCode = 1;

        /// <summary>
        /// The (injected) <see cref="IServer"/> that provides the addresses the CDP4-COMET REST API listens on
        /// </summary>
        public IServer Server { get; set; }

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
        /// <param name="stoppingToken">
        /// The <see cref="stoppingToken"/> that can be used to cancel the operation
        /// </param>
        /// <returns>
        /// an awaitable <see cref="Task"/>
        /// </returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var tlsSettingErrors = this.AppConfigService.AppConfig.Backtier.ValidateTlsSettings().ToList();

                if (tlsSettingErrors.Count > 0)
                {
                    foreach (var tlsSettingError in tlsSettingErrors)
                    {
                        this.Logger.LogCritical("Invalid TLS setting for the data-store connection: {TlsSettingError}", tlsSettingError);
                    }

                    this.Logger.LogCritical("The CDP4-COMET REST API has terminated - The TLS settings for the data-store connection are invalid");
                    this.StopAfterStartupFailure();
                    return;
                }

                var dataStoreConnectionStatus = await this.DataStoreConnectionChecker.CheckConnectionAsync(stoppingToken);

                if (dataStoreConnectionStatus == DataStoreConnectionStatus.ConfigurationError)
                {
                    this.Logger.LogCritical("The CDP4-COMET REST API has terminated - The connection to the data-store failed with an error that waiting will not resolve, see the errors above");
                    this.StopAfterStartupFailure();
                    return;
                }

                if (dataStoreConnectionStatus != DataStoreConnectionStatus.Connected)
                {
                    this.Logger.LogCritical("The CDP4-COMET REST API has terminated - The data-store was not available within the configured BacktierWaitTime: {BacktierWaitTime}", this.AppConfigService.AppConfig.Midtier.BacktierWaitTime);
                    this.StopAfterStartupFailure();
                    return;
                }

                if (!await this.MigrationEngine.MigrateAllAtStartUpAsync())
                {
                    this.Logger.LogWarning("The Migrations could not be completed");
                }

                this.CometHasStartedService.SetHasStartedAndIsReady(true);

                // the addresses are only known once the web server has started, Register invokes the callback
                // immediately when that has already happened
                this.applicationLifetime.ApplicationStarted.Register(this.LogReady);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // the application is shutting down during startup, that is not a startup failure
                this.Logger.LogInformation("The startup of the CDP4-COMET REST API was cancelled because the application is shutting down");
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                this.Logger.LogCritical(ex, "The CDP4-COMET REST API has terminated while booting up");

                // Stop the application
                this.StopAfterStartupFailure();
            }
        }

        /// <summary>
        /// Sets the exit code of the process to signal that the startup failed, so that container
        /// orchestrators and restart policies (e.g. restart: on-failure) see the failure
        /// </summary>
        protected virtual void SetStartupFailedExitCode()
        {
            Environment.ExitCode = StartupFailedExitCode;
        }

        /// <summary>
        /// Stops the application after the startup failed, with a non-zero exit code
        /// </summary>
        private void StopAfterStartupFailure()
        {
            this.SetStartupFailedExitCode();
            this.applicationLifetime.StopApplication();
        }

        /// <summary>
        /// Logs that the CDP4-COMET REST API is ready to accept requests, and on which addresses
        /// </summary>
        private void LogReady()
        {
            var addresses = this.Server?.Features.Get<IServerAddressesFeature>()?.Addresses ?? new List<string>();

            this.Logger.LogInformation("CDP4-COMET REST API is ready to accept requests at {Addresses}", addresses.Count > 0 ? string.Join(", ", addresses) : "the configured Kestrel endpoints");
        }
    }
}
