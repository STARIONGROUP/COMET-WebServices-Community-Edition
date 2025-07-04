﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStoreConnectionChecker.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
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

namespace CometServer.Services.DataStore
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using CometServer.Configuration;
    using CometServer.Helpers;
    using CometServer.Services;

    using Microsoft.Extensions.Logging;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="DataStoreConnectionChecker"/> is to check whether a connection can be made to the databse
    /// and wait for it before returning
    /// </summary>
    public class DataStoreConnectionChecker : IDataStoreConnectionChecker
    {
        /// <summary>
        /// Gets or sets the (injected) <see cref="ILogger"/>
        /// </summary>
        public ILogger<DataStoreConnectionChecker> Logger { get; set; }

        /// <summary>
        /// Gets or sets the (injected) <see cref="IDataSource"/>
        /// </summary>
        public IAppConfigService AppConfigService { get; set; }

        /// <summary>
        /// Gets or sets the DataSource manager.
        /// </summary>
        public IDataSource DataSource { get; set; }

        /// <summary>
        /// Checks whether a connection to the Data store can be made
        /// </summary>
        /// <returns>
        /// returns true when a connection can be made within the <see cref="MidtierConfig.BacktierWaitTime"/>, false otherwise
        /// </returns>
        public async Task<bool> CheckConnectionAsync(CancellationToken cancellationToken)
        {
            var startTime = DateTime.UtcNow;

            var backtierWaitTime = this.AppConfigService.AppConfig.Midtier.BacktierWaitTime;
            var remainingSeconds = backtierWaitTime;

            while (DateTime.UtcNow - startTime < TimeSpan.FromSeconds(backtierWaitTime) && !cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await using var connection = await this.DataSource.OpenNewConnectionAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    this.Logger.LogInformation(ex, "Waiting for the data store at {HostName}:{Port} to become availble in {RemainingSeconds} [s]",
                        this.AppConfigService.AppConfig.Backtier.HostName,
                        this.AppConfigService.AppConfig.Backtier.Port,
                        remainingSeconds);

                    await Task.Delay(5000, cancellationToken);
                    remainingSeconds -= 5;
                }
            }

            return false;
        }
    }
}
