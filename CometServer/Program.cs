// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-COMET Webservices Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Webservices Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading.Tasks;

    using Autofac;
    using Autofac.Extensions.DependencyInjection;

    using CometServer.ChangeNotification;
    using CometServer.Configuration;
    using CometServer.Resources;
    using CometServer.Services.DataStore;

    using Hangfire;
    
    using Microsoft.Extensions.Hosting;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Serilog;

    /// <summary>
    /// The <see cref="Program"/> is the entry point for the console application
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The entry point of the application
        /// </summary>
        /// <param name="args">
        /// the command line arguments
        /// </param>
        public static async Task<int> Main(string[] args)
        {
            Console.Title = "CDP4-COMET WebServices";

            var builder = Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder
                        .UseKestrel(options =>
                            options.AllowSynchronousIO = true
                        )
                        .UseStartup<Startup>();
                })
                .UseSerilog((hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(hostingContext.Configuration) // Read configuration from appsettings.json
                        .WriteTo.Console(); // Add the console sink for logging to the console
                });

            var host = builder.Build();

            var logger = host.Services.GetService<ILogger<Program>>();

            try
            {
                var resourceLoader = host.Services.GetService<IResourceLoader>();
                var logo = resourceLoader.QueryLogo();

                logger.LogInformation(logo);

                logger.LogInformation("################################################################");
                logger.LogInformation($"Starting CDP4-COMET Services v{Assembly.GetEntryAssembly().GetName().Version}");

                var appConfigService = host.Services.GetService<IAppConfigService>();

                logger.LogInformation("Configuration Loaded:");

                logger.LogInformation($"Midtier-UploadDirectory: {appConfigService.AppConfig.Midtier.UploadDirectory}");
                logger.LogInformation($"Midtier-FileStorageDirectory: {appConfigService.AppConfig.Midtier.FileStorageDirectory}");
                logger.LogInformation($"Backtier-HostName: {appConfigService.AppConfig.Backtier.HostName}");
                logger.LogInformation($"Backtier-Port: {appConfigService.AppConfig.Backtier.Port}");
                logger.LogInformation($"Backtier-Database: {appConfigService.AppConfig.Backtier.Database}");
                logger.LogInformation($"Backtier-DatabaseRestore: {appConfigService.AppConfig.Backtier.DatabaseRestore}");
                logger.LogInformation($"Backtier-DatabaseManage: {appConfigService.AppConfig.Backtier.DatabaseManage}");
                logger.LogInformation($"Backtier-StatementTimeout: {appConfigService.AppConfig.Backtier.StatementTimeout}");
                logger.LogInformation($"Backtier-IsDbSeedEnabled: {appConfigService.AppConfig.Backtier.IsDbSeedEnabled}");
                logger.LogInformation($"Backtier-IsDbImportEnabled: {appConfigService.AppConfig.Backtier.IsDbImportEnabled}");
                logger.LogInformation($"Backtier-IsDbRestoreEnabled: {appConfigService.AppConfig.Backtier.IsDbRestoreEnabled}");

                var dataStoreConnectionChecker = host.Services.GetService<IDataStoreConnectionChecker>();
                var dataStoreAvailable = dataStoreConnectionChecker.CheckConnection();

                if (!dataStoreAvailable)
                {
                    logger.LogCritical("The CDP4-COMET REST API has terminated - The data-store was not availble within the configured BacktierWaitTime: {BacktierWaitTime}", appConfigService.AppConfig.Midtier.BacktierWaitTime);
                    return 0;
                }
                
                logger.LogInformation("The data-store has become available for connections within the configured BacktierWaitTime: {BacktierWaitTime}", appConfigService.AppConfig.Midtier.BacktierWaitTime);
                
                var migrationEngine = host.Services.GetService<IMigrationEngine>();
                migrationEngine.MigrateAllAtStartUp();

                ConfigureRecurringJobs(appConfigService, logger);

                var configuration = host.Services.GetService<IConfiguration>();
                var uri = configuration.GetSection("Kestrel:Endpoints:Http:Url").Value;

                logger.LogInformation("CDP4-COMET REST API Ready to accept connections at {uri}", uri);

                await host.RunAsync();

                logger.LogInformation("Terminated CDP4-COMET WebServices cleanly");
                return 0;
            }
            catch (Exception e)
            {
                logger.LogCritical(e,"An unhandled exception occurred during startup-bootstrapping");
                return -1;
            }
        }

        /// <summary>
        /// Configure the recurring jobs
        /// </summary>
        /// <param name="appConfigService">
        /// The <see cref="IAppConfigService"/> that provides the configuration used to configure the recurring jobs
        /// </param>
        /// <param name="logger">
        /// The <see cref="ILogger{Program}"/> used to log
        /// </param>
        public static void ConfigureRecurringJobs(IAppConfigService appConfigService, ILogger<Program> logger)
        {
            var sw = Stopwatch.StartNew();

            var builder = new ContainerBuilder();
            builder.RegisterType<ChangeNoticationService>().InstancePerBackgroundJob();
            GlobalConfiguration.Configuration.UseAutofacActivator(builder.Build());

            if (appConfigService.AppConfig.Changelog.AllowEmailNotification)
            {
                RecurringJob.AddOrUpdate<ChangeNoticationService>("ChangeNotificationService.Execute", notificationService => notificationService.Execute(), Cron.Weekly(DayOfWeek.Monday, 0, 15));
            }

            logger.LogInformation($"Cron jobs configured in {sw.ElapsedMilliseconds} ms");
        }
    }
}
