// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A. All rights reserverd
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServer
{
    using System;
    using System.Reflection;

    using CDP4WebServices.API.Configuration;

    using Microsoft.Owin.Hosting;
    using Mono.Unix;
    using Mono.Unix.Native;

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
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int Main(string[] args)
        {
            try
            {
                Logger.Info("################################################################");
                Logger.Info($"Starting CDP4 Services v{Assembly.GetEntryAssembly().GetName().Version}");

                // load application configuration from file
                AppConfig.Load();

                Logger.Info("Configuration Loaded:");

                Logger.Debug($"Midtier-Protocol: {AppConfig.Current.Midtier.Protocol}");
                Logger.Debug($"Midtier-HostName: {AppConfig.Current.Midtier.HostName}");
                Logger.Debug($"Midtier-Port: {AppConfig.Current.Midtier.Port}");
                Logger.Debug($"Midtier-UploadDirectory: {AppConfig.Current.Midtier.UploadDirectory}");
                Logger.Debug($"Midtier-FileStorageDirectory: {AppConfig.Current.Midtier.FileStorageDirectory}");
                Logger.Debug($"Backtier-HostName: {AppConfig.Current.Backtier.HostName}" );
                Logger.Debug($"Backtier-Port: {AppConfig.Current.Backtier.Port}");
                Logger.Debug($"Backtier-Database: {AppConfig.Current.Backtier.Database}");
                Logger.Debug($"Backtier-DatabaseRestore: {AppConfig.Current.Backtier.DatabaseRestore}");
                Logger.Debug($"Backtier-DatabaseManage: {AppConfig.Current.Backtier.DatabaseManage}");
                Logger.Debug($"Backtier-StatementTimeout: {AppConfig.Current.Backtier.StatementTimeout}");
                Logger.Debug($"Backtier-IsDbSeedEnabled: {AppConfig.Current.Backtier.IsDbSeedEnabled}");
                Logger.Debug($"Backtier-IsDbRestoreEnabled: {AppConfig.Current.Backtier.IsDbRestoreEnabled}");

                var hostString = $"{AppConfig.Current.Midtier.Protocol}://{AppConfig.Current.Midtier.HostName}:{AppConfig.Current.Midtier.Port}";

                using (WebApp.Start<Startup>(hostString))
                {
                    if (IsRunningOnMono())
                    {
                        Logger.Info("CDP4 Services Running on Mono Runtime @ {0}", hostString);

                        var terminationSignals = GetUnixTerminationSignals();
                        UnixSignal.WaitAny(terminationSignals);
                    }
                    else
                    {
                        Logger.Info("CDP4 Services Running on .NET Runtime @ {0}", hostString);

                        Console.WriteLine("Running on {0}", hostString);
                        Console.WriteLine("Press enter to exit");
                        Console.ReadLine();
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                // global catch all
                Logger.Fatal(ex, "The CDP4 Services encountered an unrecoverable error");
                return 42;
            }
        }

        /// <summary>
        /// Check if hosting from mono.
        /// </summary>
        /// <returns>
        /// true if mono hosted
        /// </returns>
        private static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }

        /// <summary>
        /// Get the Unix termination <see cref="UnixSignal"/> to listen for in Mono hosted applications. 
        /// </summary>
        /// <returns>
        /// An array of <see cref="UnixSignal"/> termination signals"
        /// </returns>
        private static UnixSignal[] GetUnixTerminationSignals()
        {
            return new[]
                       {
                           new UnixSignal(Signum.SIGINT),
                           new UnixSignal(Signum.SIGTERM),
                           new UnixSignal(Signum.SIGQUIT),
                           new UnixSignal(Signum.SIGHUP)
                       };
        }
    }
}
