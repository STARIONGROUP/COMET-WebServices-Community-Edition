// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataStoreController.cs" company="RHEA System S.A.">
//   Copyright (c) 2017 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services.DataStore
{
    using Npgsql;

    /// <summary>
    /// The DataStoreController interface.
    /// </summary>
    public interface IDataStoreController
    {
        /// <summary>
        /// Creates a clone of the data store.
        /// </summary>
        void CloneDataStore();

        /// <summary>
        /// The restore data store.
        /// </summary>
        /// <exception cref="System.IO.FileNotFoundException">
        /// If the file to restore from is not found
        /// </exception>
        void RestoreDataStore();

        /// <summary>
        /// Drops all connections to a data store.
        /// </summary>
        /// <param name="dataStoreName">
        /// The name of a data store.
        /// </param>
        /// <param name="connection">
        /// The connection to the managing data store.
        /// </param>
        void DropDataStoreConnections(string dataStoreName, NpgsqlConnection connection);
    }
}