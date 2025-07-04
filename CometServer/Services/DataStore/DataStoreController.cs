// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStoreController.cs" company="Starion Group S.A.">
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
    using System.IO;
    using System.Threading.Tasks;

    using CometServer.Helpers;

    using Configuration;

    using Microsoft.Extensions.Logging;

    using Npgsql;

    using NpgsqlTypes;

    /// <summary>
    /// The data store controller.
    /// </summary>
    public class DataStoreController : IDataStoreController
    {
        /// <summary>
        /// Gets or sets the (injected) <see cref="ILogger"/>
        /// </summary>
        public ILogger<DataStoreController> Logger { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IAppConfigService"/>
        /// </summary>
        public IAppConfigService AppConfigService { get; set; }

        /// <summary>
        /// Gets or sets the (injected) DataSource manager.
        /// </summary>
        public IDataSource DataSource { get; set; }

        /// <summary>
        /// Creates a clone of the data store.
        /// </summary>
        public async Task CloneDataStore()
        {
            this.Logger.LogInformation("start data store clone process");

            var backtier = this.AppConfigService.AppConfig.Backtier;

            await using var connection = await this.DataSource.OpenNewConnectionAsync();

            // Create a clone of the database
            await using (var cmd = new NpgsqlCommand())
            {
                this.Logger.LogDebug("Clone the data store");

                await this.DropDataStoreConnections(backtier.Database, connection);

                cmd.Connection = connection;
                    
                cmd.CommandText = $"CREATE DATABASE {backtier.DatabaseRestore} WITH OWNER = {backtier.UserName} TEMPLATE = {backtier.Database} ENCODING = 'UTF8';";

                await cmd.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// The restore data store.
        /// </summary>
        /// <exception cref="FileNotFoundException">
        /// If the file to restore from is not found
        /// </exception>
        public async Task RestoreDataStore()
        {
            this.Logger.LogInformation("start data store restore process");

            var backtier = this.AppConfigService.AppConfig.Backtier;

            // Connect to the restore database
            await using var connection = await this.DataSource.OpenNewConnectionAsync();

            // Drop the existing database
            await using (var cmd = new NpgsqlCommand())
            {
                this.Logger.LogDebug("Drop the data store");

                await this.DropDataStoreConnections(backtier.Database, connection);

                cmd.Connection = connection;
                    
                cmd.CommandText = $"DROP DATABASE IF EXISTS {backtier.Database};";

                await cmd.ExecuteNonQueryAsync();
            }

            // Create a new database with a restore template
            await using (var cmd = new NpgsqlCommand())
            {
                this.Logger.LogDebug("Clone the restore data store");

                cmd.Connection = connection;

                cmd.CommandText = $"CREATE DATABASE {backtier.Database} WITH OWNER = {backtier.UserName} TEMPLATE = {backtier.DatabaseRestore} ENCODING = 'UTF8';";

                await cmd.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Drops all connections to a data store.
        /// </summary>
        /// <param name="dataStoreName">
        /// The name of a data store.
        /// </param>
        /// <param name="connection">
        /// The connection to the managing data store.
        /// </param>
        public async Task DropDataStoreConnections(string dataStoreName, NpgsqlConnection connection)
        {
            await using var cmd = new NpgsqlCommand();
            
            this.Logger.LogDebug("Drop all connections to the data store");

            NpgsqlConnection.ClearPool(new NpgsqlConnection(Utils.GetConnectionString(this.AppConfigService.AppConfig.Backtier, dataStoreName)));

            cmd.Connection = connection;

            cmd.CommandText = "SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = :dataStoreName AND pid <> pg_backend_pid();";
            cmd.Parameters.Add("dataStoreName", NpgsqlDbType.Varchar).Value = dataStoreName;

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
