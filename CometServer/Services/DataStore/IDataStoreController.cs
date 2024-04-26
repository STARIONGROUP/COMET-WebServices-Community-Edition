// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataStoreController.cs" company="Starion Group S.A.">
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

namespace CometServer.Services.DataStore
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
