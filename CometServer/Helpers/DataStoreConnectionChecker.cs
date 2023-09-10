// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStoreConnectionChecker.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
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

namespace CometServer.Helpers
{
    using System;
    using System.Threading;

    using CometServer.Configuration;
    using CometServer.Services;

    using NLog;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="DataStoreConnectionChecker"/> is to check whether a connection can be made to the databse
    /// and wait for it before returning
    /// </summary>
    public static class DataStoreConnectionChecker
    {
        /// <summary>
        /// The Logger
        /// </summary>
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Checks whether a connection to the Data store can be made
        /// </summary>
        /// <param name="appConfigService">
        /// The <see cref="IAppConfigService"/> that provides application settings
        /// </param>
        /// <returns>
        /// returns true when a connection can be made within the <see cref="MidtierConfig.BacktierWaitTime"/>, false otherwise
        /// </returns>
        public static bool CheckConnection(IAppConfigService appConfigService)
        {
            var connection = new NpgsqlConnection(Utils.GetConnectionString(appConfigService.AppConfig.Backtier, appConfigService.AppConfig.Backtier.DatabaseManage));

            var startTime = DateTime.UtcNow;
            var remainingSeconds = appConfigService.AppConfig.Midtier.BacktierWaitTime;
            
            while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(appConfigService.AppConfig.Midtier.BacktierWaitTime))
            {
                try
                {
                    connection.Open();
                    connection.Close();

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Info("Waiting for the data store at {0}:{1} to become availble in {2} [s]", 
                        appConfigService.AppConfig.Backtier.HostName,
                        appConfigService.AppConfig.Backtier.Port,
                        remainingSeconds);

                    Thread.Sleep(5000);
                    remainingSeconds = remainingSeconds - 5;
                }
            }

            return false;
        }
    }
}
