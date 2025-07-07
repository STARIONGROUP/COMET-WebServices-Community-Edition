// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataSource.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the Starion implementation of ECSS-E-TM-10-25 Annex A and Annex C.
// 
//    The CDP4-COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4-COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Helpers
{
    using System.Threading.Tasks;

    using Npgsql;

    /// <summary>
    /// Interface for a data source that provides a connection to a PostgreSQL database.
    /// </summary>
    public interface IDataSource
    {
        /// <summary>
        /// Creates a new <see cref="NpgsqlConnection"/> class.
        /// </summary>
        /// <returns>The <see cref="NpgsqlConnection"/></returns>
        NpgsqlConnection CreateConnection();

        /// <summary>
        /// Tries to clear all idle connections.
        /// </summary>
        void ClearConnections();

        /// <summary>
        /// Creates and opens a new <see cref="NpgsqlConnection"/> asynchronously.
        /// </summary>
        /// <returns>An awaitable <see cref="ValueTask{T}"/> of type <see cref="NpgsqlConnection"/></returns>
        ValueTask<NpgsqlConnection> OpenNewConnectionAsync();

        /// <summary>
        /// Creates and opens a new <see cref="NpgsqlConnection"/> asynchronously.
        /// </summary>
        /// <returns>An awaitable <see cref="ValueTask{T}"/> of type <see cref="NpgsqlConnection"/></returns>
        ValueTask<NpgsqlConnection> OpenNewConnectionToManageDatabaseAsync();
    }
}
