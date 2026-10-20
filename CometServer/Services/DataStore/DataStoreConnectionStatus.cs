// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataStoreConnectionStatus.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2026 Starion Group S.A.
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

namespace CometServer.Services.DataStore
{
    /// <summary>
    /// The result of checking the connection to the data store
    /// </summary>
    public enum DataStoreConnectionStatus
    {
        /// <summary>
        /// A connection to the data store was made
        /// </summary>
        Connected,

        /// <summary>
        /// The data store did not become available within the configured BacktierWaitTime
        /// </summary>
        TimedOut,

        /// <summary>
        /// The connection failed because of a configuration error that waiting will not resolve,
        /// such as a rejected server certificate or a wrong password
        /// </summary>
        ConfigurationError
    }
}
