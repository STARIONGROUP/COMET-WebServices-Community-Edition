// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStoreController.cs" company="RHEA System S.A.">
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

namespace CometServer.Services.DataStore
{
    using System.IO;

    using Configuration;

    using NLog;

    using Npgsql;

    /// <summary>
    /// The data store controller.
    /// </summary>
    public class DataStoreController : IDataStoreController
    {
        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the <see cref="IAppConfigService"/>
        /// </summary>
        public IAppConfigService AppConfigService { get; set; }

        /// <summary>
        /// Creates a clone of the data store.
        /// </summary>
        public void CloneDataStore()
        {
            Logger.Info("start data store clone process");

            var backtier = this.AppConfigService.AppConfig.Backtier;

            using (var connection = new NpgsqlConnection(Utils.GetConnectionString(backtier, backtier.DatabaseManage)))
            {
                connection.Open();

                // Create a clone of the database
                using (var cmd = new NpgsqlCommand())
                {
                    Logger.Debug("Clone the data store");

                    this.DropDataStoreConnections(backtier.Database, connection);

                    cmd.Connection = connection;
                    
                    cmd.CommandText = $"CREATE DATABASE {backtier.DatabaseRestore} WITH OWNER = {backtier.UserName} TEMPLATE = {backtier.Database} ENCODING = 'UTF8';";

                    cmd.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        /// <summary>
        /// The restore data store.
        /// </summary>
        /// <exception cref="FileNotFoundException">
        /// If the file to restore from is not found
        /// </exception>
        public void RestoreDataStore()
        {
            Logger.Info("start data store restore process");

            var backtier = this.AppConfigService.AppConfig.Backtier;

            // Connect to the restore database
            using (var connection = new NpgsqlConnection(Utils.GetConnectionString(backtier, backtier.DatabaseManage)))
            {
                connection.Open();

                // Drop the existing database
                using (var cmd = new NpgsqlCommand())
                {
                    Logger.Debug("Drop the data store");

                    this.DropDataStoreConnections(backtier.Database, connection);

                    cmd.Connection = connection;
                    
                    cmd.CommandText = $"DROP DATABASE IF EXISTS {backtier.Database};";

                    cmd.ExecuteNonQuery();
                }

                // Create a new database with a restore template
                using (var cmd = new NpgsqlCommand())
                {
                    Logger.Debug("Clone the restore data store");

                    cmd.Connection = connection;

                    cmd.CommandText = $"CREATE DATABASE {backtier.Database} WITH OWNER = {backtier.UserName} TEMPLATE = {backtier.DatabaseRestore} ENCODING = 'UTF8';";

                    cmd.ExecuteNonQuery();
                }

                connection.Close();
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
        public void DropDataStoreConnections(string dataStoreName, NpgsqlConnection connection)
        {
            using (var cmd = new NpgsqlCommand())
            {
                Logger.Debug("Drop all connections to the data store");

                NpgsqlConnection.ClearPool(new NpgsqlConnection(Utils.GetConnectionString(this.AppConfigService.AppConfig.Backtier, dataStoreName)));

                cmd.Connection = connection;

                cmd.CommandText = $"SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = '{dataStoreName}' AND pid <> pg_backend_pid();";

                cmd.ExecuteNonQuery();
            }
        }
    }
}
