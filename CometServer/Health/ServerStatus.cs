// --------------------------------------------------------------------------------------------------------------------
// <copyright file="$FILENAME$" company="Starion Group S.A.">
//    Copyright (c) 2015-$CURRENT_YEAR$ Starion Group S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

namespace CometServer.Health
{
    using System;

    /// <summary>
    /// A struct that indicates the status of the service
    /// </summary>
    public struct ServerStatus
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerStatus"/> structy
        /// </summary>
        /// <param name="isHealthy">
        /// a value indicating whether the status is healthy or not
        /// </param>
        /// <param name="dateTime">
        /// a <see cref="DateTime"/> value typically used to set the <see cref="DateTime"/>
        /// when the <see cref="ServerStatus"/> was created
        /// </param>
        public ServerStatus(bool isHealthy, DateTime dateTime)
        {
            this.IsHealthy = isHealthy;
            this.DateTime = dateTime;
        }

        /// <summary>
        /// Gets a value indicating whether the status is healthy or not
        /// </summary>
        public bool IsHealthy { get; private set; }

        /// <summary>
        /// Gets the asscoicated status <see cref="DateTime"/>
        /// </summary>
        public DateTime DateTime { get; private set; }
    }
}
