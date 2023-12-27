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
    using System.Threading.Tasks;

    using Autofac.Extensions.DependencyInjection;

    using CometServer.Configuration;
    using CometServer.Resources;

    using Microsoft.Extensions.Hosting;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Serilog;
    using Autofac;
    using CometServer.ChangeNotification;
    using Hangfire;
    using System.Diagnostics;

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

                logger.LogInformation("Starting CDP4-COMET Services v{version}", resourceLoader.QueryVersion());

                var appConfigService = host.Services.GetService<IAppConfigService>();

                logger.LogInformation("Configuration Loaded:");

                logger.LogInformation("Midtier-UploadDirectory: {UploadDirectory}", appConfigService.AppConfig.Midtier.UploadDirectory);
                logger.LogInformation("Midtier-FileStorageDirectory: {FileStorageDirectory}", appConfigService.AppConfig.Midtier.FileStorageDirectory);
                logger.LogInformation("Midtier-TemporaryFileStorageDirectory: {TemporaryFileStorageDirectory}", appConfigService.AppConfig.Midtier.TemporaryFileStorageDirectory);
                logger.LogInformation("Midtier-IsExportEnabled: {IsExportEnabled}", appConfigService.AppConfig.Midtier.IsExportEnabled);
                logger.LogInformation("Midtier-ExportDirectory: {ExportDirectory}", appConfigService.AppConfig.Midtier.ExportDirectory);
                logger.LogInformation("Midtier-BacktierWaitTime: {BacktierWaitTime}", appConfigService.AppConfig.Midtier.BacktierWaitTime);

                logger.LogInformation("Changelog-CollectChanges: {CollectChanges}", appConfigService.AppConfig.Changelog.CollectChanges);
                logger.LogInformation("Changelog-AllowEmailNotification: {AllowEmailNotification}", appConfigService.AppConfig.Changelog.AllowEmailNotification);

                logger.LogInformation("Backtier-HostName: {HostName}", appConfigService.AppConfig.Backtier.HostName);
                logger.LogInformation("Backtier-Port: {Port}", appConfigService.AppConfig.Backtier.Port);
                logger.LogInformation("Backtier-Database: {Database}", appConfigService.AppConfig.Backtier.Database);
                logger.LogInformation("Backtier-DatabaseRestore: {DatabaseRestore}", appConfigService.AppConfig.Backtier.DatabaseRestore);
                logger.LogInformation("Backtier-DatabaseManage: {DatabaseManage}", appConfigService.AppConfig.Backtier.DatabaseManage);
                logger.LogInformation("Backtier-StatementTimeout: {StatementTimeout}", appConfigService.AppConfig.Backtier.StatementTimeout);
                logger.LogInformation("Backtier-IsDbSeedEnabled: {IsDbSeedEnabled}", appConfigService.AppConfig.Backtier.IsDbSeedEnabled);
                logger.LogInformation("Backtier-IsDbImportEnabled: {IsDbImportEnabled}", appConfigService.AppConfig.Backtier.IsDbImportEnabled);
                logger.LogInformation("Backtier-IsDbRestoreEnabled: {IsDbRestoreEnabled}", appConfigService.AppConfig.Backtier.IsDbRestoreEnabled);

                logger.LogInformation("################################################################");

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
    }
}
