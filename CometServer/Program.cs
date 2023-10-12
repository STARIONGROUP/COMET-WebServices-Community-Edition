// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of CDP4-COMET Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
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
    using CometServer.Helpers;

    using Hangfire;

    using Microsoft.Extensions.Hosting;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using NLog;

    /// <summary>
    /// The <see cref="Program"/> is the entry point for the console application
    /// </summary>
    public class Program
    {
        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The entry point of the application
        /// </summary>
        /// <param name="args">
        /// the command line arguments
        /// </param>
        public static async Task<int> Main(string[] args)
        {
            Console.Title = "CDP4-COMET WebServices";

            try
            {
                // ASP.NET Core 3.0+:
                // The UseServiceProviderFactory call attaches the
                // Autofac provider to the generic hosting mechanism.
                var host = Host.CreateDefaultBuilder(args)
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .ConfigureWebHostDefaults(webHostBuilder =>
                    {
                        webHostBuilder
                            .UseKestrel(options => 
                                options.AllowSynchronousIO = true
                            )
                            .UseStartup<Startup>();
                    })
                    .Build();

                Logger.Info("################################################################");
                Logger.Info($"Starting CDP4-COMET Services v{Assembly.GetEntryAssembly().GetName().Version}");

                var appConfigService = host.Services.GetService<IAppConfigService>();

                Logger.Info("Configuration Loaded:");

                Logger.Debug($"Midtier-UploadDirectory: {appConfigService.AppConfig.Midtier.UploadDirectory}");
                Logger.Debug($"Midtier-FileStorageDirectory: {appConfigService.AppConfig.Midtier.FileStorageDirectory}");
                Logger.Debug($"Backtier-HostName: {appConfigService.AppConfig.Backtier.HostName}");
                Logger.Debug($"Backtier-Port: {appConfigService.AppConfig.Backtier.Port}");
                Logger.Debug($"Backtier-Database: {appConfigService.AppConfig.Backtier.Database}");
                Logger.Debug($"Backtier-DatabaseRestore: {appConfigService.AppConfig.Backtier.DatabaseRestore}");
                Logger.Debug($"Backtier-DatabaseManage: {appConfigService.AppConfig.Backtier.DatabaseManage}");
                Logger.Debug($"Backtier-StatementTimeout: {appConfigService.AppConfig.Backtier.StatementTimeout}");
                Logger.Debug($"Backtier-IsDbSeedEnabled: {appConfigService.AppConfig.Backtier.IsDbSeedEnabled}");
                Logger.Debug($"Backtier-IsDbImportEnabled: {appConfigService.AppConfig.Backtier.IsDbImportEnabled}");
                Logger.Debug($"Backtier-IsDbRestoreEnabled: {appConfigService.AppConfig.Backtier.IsDbRestoreEnabled}");


                var dataStoreAvailable = DataStoreConnectionChecker.CheckConnection(appConfigService);
                if (!dataStoreAvailable)
                {
                    Logger.Warn("The CDP4-COMET REST API has terminated - The data-store was not availble within the configured BacktierWaitTime: {0}", appConfigService.AppConfig.Midtier.BacktierWaitTime);
                    return 0;
                }
                else
                {
                    Logger.Info("The data-store has become available for connections within the configured BacktierWaitTime: {0}", appConfigService.AppConfig.Midtier.BacktierWaitTime);
                }

                MigrationEngine.MigrateAllAtStartUp(appConfigService);
                ConfigureRecurringJobs(appConfigService);

                var configuration = host.Services.GetService<IConfiguration>();
                var uri = configuration.GetSection("Kestrel:Endpoints:Http:Url").Value;

                Logger.Info("CDP4-COMET REST API Ready to accept connections at {0}", uri);

                await host.RunAsync();

                Logger.Info("Terminated CDP4-COMET WebServices cleanly");
                return 0;
            }
            catch (Exception e)
            {
                Logger.Fatal("An unhandled exception occurred during startup-bootstrapping");
                return -1;
            }
        }

        /// <summary>
        /// Configure the recurring jobs
        /// </summary>
        /// <param name="appConfigService">
        /// The <see cref="IAppConfigService"/> that provides the configuration used to configure the recurring jobs
        /// </param>
        public static void ConfigureRecurringJobs(IAppConfigService appConfigService)
        {
            var sw = Stopwatch.StartNew();

            var builder = new ContainerBuilder();
            builder.RegisterType<ChangeNoticationService>().InstancePerBackgroundJob();
            GlobalConfiguration.Configuration.UseAutofacActivator(builder.Build());

            if (appConfigService.AppConfig.Changelog.AllowEmailNotification)
            {
                RecurringJob.AddOrUpdate<ChangeNoticationService>("ChangeNotificationService.Execute", notificationService => notificationService.Execute(), Cron.Weekly(DayOfWeek.Monday, 0, 15));
            }

            Logger.Info($"Cron jobs configured in {sw.ElapsedMilliseconds} ms");
        }
    }
}
