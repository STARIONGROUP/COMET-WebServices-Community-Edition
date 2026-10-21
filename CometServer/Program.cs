// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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
    using System.Configuration;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    using Autofac.Extensions.DependencyInjection;

    using CDP4ServicesMessaging.Services.BackgroundMessageProducers;

    using CometServer.Configuration;
    using CometServer.Resources;

    using Microsoft.Extensions.Hosting;
    using Microsoft.AspNetCore.Hosting;
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
            var backgroundMessageProducer = host.Services.GetService<IBackgroundThingsMessageProducer>();

            try
            {
                var resourceLoader = host.Services.GetService<IResourceLoader>();

                logger.LogInformation(resourceLoader.QueryLogo());

                logger.LogInformation("################################################################");

                logger.LogInformation("Starting CDP4-COMET Services v{Version}", resourceLoader.QueryVersion());

                logger.LogInformation("Runtime: {FrameworkDescription} on {OSDescription} ({RuntimeIdentifier})", RuntimeInformation.FrameworkDescription, RuntimeInformation.OSDescription, RuntimeInformation.RuntimeIdentifier);

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
                logger.LogInformation("Backtier-ConnectionTimeout: {ConnectionTimeout}", appConfigService.AppConfig.Backtier.ConnectionTimeout);
                logger.LogInformation("Backtier-Keepalive: {Keepalive}", appConfigService.AppConfig.Backtier.Keepalive);
                logger.LogInformation("Backtier-MaximumPoolSize: {MaximumPoolSize}", appConfigService.AppConfig.Backtier.MaximumPoolSize);
                logger.LogInformation("Backtier-IsDbSeedEnabled: {IsDbSeedEnabled}", appConfigService.AppConfig.Backtier.IsDbSeedEnabled);
                logger.LogInformation("Backtier-IsDbImportEnabled: {IsDbImportEnabled}", appConfigService.AppConfig.Backtier.IsDbImportEnabled);
                logger.LogInformation("Backtier-IsDbRestoreEnabled: {IsDbRestoreEnabled}", appConfigService.AppConfig.Backtier.IsDbRestoreEnabled);
                logger.LogInformation("Backtier-SslMode: {SslMode}", appConfigService.AppConfig.Backtier.SslMode);
                var rootCertificate = appConfigService.AppConfig.Backtier.GetEffectiveRootCertificate();
                logger.LogInformation("Backtier-RootCertificate: {RootCertificate} ({RootCertificateSource})", rootCertificate.Path ?? "none", rootCertificate.Source);
                logger.LogInformation("Backtier-ClientCertificate: {ClientCertificate}", string.IsNullOrWhiteSpace(appConfigService.AppConfig.Backtier.ClientCertificate) ? "none" : appConfigService.AppConfig.Backtier.ClientCertificate);
                logger.LogInformation("Backtier-CheckCertificateRevocation: {CheckCertificateRevocation}", appConfigService.AppConfig.Backtier.CheckCertificateRevocation);

                logger.LogInformation("ThingsMessageProducer-IsEnabled: {IsEnabled}", appConfigService.AppConfig.ServiceMessagingConfig.IsEnabled);

                logger.LogInformation("################################################################");

                logger.LogInformation("CDP4-COMET REST API is starting, requests are accepted once the data store has been checked and the migrations have run");

                await host.RunAsync();

                if (Environment.ExitCode != 0)
                {
                    logger.LogCritical("Terminated CDP4-COMET WebServices because the startup failed, see the errors above (exit code {ExitCode})", Environment.ExitCode);
                    return Environment.ExitCode;
                }

                logger.LogInformation("Terminated CDP4-COMET WebServices cleanly");
                return 0;
            }
            catch (Exception e)
            {
                var configurationError = FindConfigurationError(e);

                if (configurationError != null)
                {
                    // an invalid setting: the message says what to change, the stack trace only helps when debugging
                    logger.LogCritical("Terminated CDP4-COMET WebServices because of an invalid configuration: {ConfigurationError}", configurationError.Message);
                    logger.LogDebug(e, "The configuration could not be loaded");
                    return -1;
                }

                logger.LogCritical(e, "An unhandled exception occurred during startup-bootstrapping");
                return -1;
            }
            finally
            {
                if (backgroundMessageProducer is not null)
                {
                    await backgroundMessageProducer.DequeueAsync();
                }
            }
        }

        /// <summary>
        /// Finds a <see cref="ConfigurationErrorsException"/> in the exception or its inner exceptions, for instance when
        /// the dependency injection container wraps it while creating the <see cref="IAppConfigService"/>
        /// </summary>
        /// <param name="exception">
        /// The exception
        /// </param>
        /// <returns>
        /// The <see cref="ConfigurationErrorsException"/>, or null when there is none
        /// </returns>
        internal static ConfigurationErrorsException FindConfigurationError(Exception exception)
        {
            for (var current = exception; current != null; current = current.InnerException)
            {
                if (current is ConfigurationErrorsException configurationErrorsException)
                {
                    return configurationErrorsException;
                }
            }

            return null;
        }
    }
}
