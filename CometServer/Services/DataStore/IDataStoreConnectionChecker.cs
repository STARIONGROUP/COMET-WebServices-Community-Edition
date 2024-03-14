// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataStoreConnectionChecker.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
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
    using System.Threading;

    using CometServer.Configuration;
    
    /// <summary>
    /// The purpose of the <see cref="IDataStoreConnectionChecker"/> is to check whether a connection can be made to the databse
    /// and wait for it before returning
    /// </summary>
    public interface IDataStoreConnectionChecker
    {
        /// <summary>
        /// Checks whether a connection to the Data store can be made
        /// </summary>
        /// <param name="cancellationToken">
        /// The <see cref="cancellationToken"/> that can be used to cancel the operation
        /// </param>
        /// <returns>
        /// returns true when a connection can be made within the <see cref="MidtierConfig.BacktierWaitTime"/>, false otherwise
        /// </returns>
        public bool CheckConnection(CancellationToken cancellationToken);
    }
}
