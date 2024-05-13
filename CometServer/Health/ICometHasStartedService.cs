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
    /// The purpose of the <see cref="ICometHasStartedService"/> is to provide a shared resource
    /// that can be used to check whether the CDP4-COMET has succcesfuly started and is ready
    /// to accept traffic
    /// </summary>
    public interface ICometHasStartedService
    {
        /// <summary>
        /// Gets the <see cref="DateTime"/> at which the CDP4-COMET finished the
        /// startup process and when it was ready to start accepting requests
        /// </summary>
        /// <returns>
        /// A <see cref="ServerStatus"/> that has no value when not yet started and ready
        /// or a UTC datetime when started and ready
        /// </returns>
        public ServerStatus GetHasStartedAndIsReady();

        /// <summary>
        /// Sets the <see cref="ICometHasStartedService"/> to indicate that the
        /// CDP4-COMET server has started and is ready to accept requests
        /// </summary>
        public void SetHasStartedAndIsReady(bool isHealthy);
    }
}
