// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStoreController.cs" company="RHEA System S.A.">
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
        /// Creates a clone of the data store.
        /// </summary>
        public void CloneDataStore()
        {
            Logger.Info("start data store clone process");

            using (var connection = new NpgsqlConnection(Utils.GetConnectionString(AppConfig.Current.Backtier.DatabaseManage)))
            {
                connection.Open();

                // Create a clone of the database
                using (var cmd = new NpgsqlCommand())
                {
                    Logger.Debug("Clone the data store");

                    this.DropDataStoreConnections(AppConfig.Current.Backtier.Database, connection);

                    cmd.Connection = connection;
                    var commandDefinition = "CREATE DATABASE {0} WITH OWNER = {1} TEMPLATE = {2}  ENCODING = 'UTF8';";
                    cmd.CommandText = string.Format(
                        commandDefinition,
                        /*0*/ AppConfig.Current.Backtier.DatabaseRestore,
                        /*1*/ AppConfig.Current.Backtier.UserName,
                        /*2*/ AppConfig.Current.Backtier.Database);
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

            // Connect to the restore database
            using (var connection = new NpgsqlConnection(Utils.GetConnectionString(AppConfig.Current.Backtier.DatabaseManage)))
            {
                connection.Open();

                // Drop the existing database
                using (var cmd = new NpgsqlCommand())
                {
                    Logger.Debug("Drop the data store");

                    this.DropDataStoreConnections(AppConfig.Current.Backtier.Database, connection);

                    cmd.Connection = connection;
                    var commandDefinition = "DROP DATABASE IF EXISTS {0};";
                    cmd.CommandText = string.Format(
                        commandDefinition,
                        /*0*/ AppConfig.Current.Backtier.Database);
                    cmd.ExecuteNonQuery();
                }

                // Create a new database with a restore template
                using (var cmd = new NpgsqlCommand())
                {
                    Logger.Debug("Clone the restore data store");

                    cmd.Connection = connection;
                    var commandDefinition = "CREATE DATABASE {0} WITH OWNER = {1} TEMPLATE = {2}  ENCODING = 'UTF8';";
                    cmd.CommandText = string.Format(
                        commandDefinition,
                        /*0*/ AppConfig.Current.Backtier.Database,
                        /*1*/ AppConfig.Current.Backtier.UserName,
                        /*2*/ AppConfig.Current.Backtier.DatabaseRestore);
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

                NpgsqlConnection.ClearPool(new NpgsqlConnection(Utils.GetConnectionString(dataStoreName)));

                cmd.Connection = connection;
                var commandDefinition =
                    "SELECT pg_terminate_backend(pg_stat_activity.pid) FROM pg_stat_activity WHERE pg_stat_activity.datname = '{0}' AND pid <> pg_backend_pid();";
                cmd.CommandText = string.Format(commandDefinition, dataStoreName);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
