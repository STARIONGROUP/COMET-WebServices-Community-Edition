// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeDateTime.cs" company="RHEA System S.A.">
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

namespace CometServer.Services
{
    using System;

    /// <summary>
    /// Exchange file date time format that holds the local and universal time.
    /// </summary>
    public class ExchangeDateTime
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeDateTime"/> class.
        /// </summary>
        /// <param name="dateTime">
        /// The date time.
        /// </param>
        public ExchangeDateTime(DateTime dateTime)
        {
            this.Local = dateTime.ToLocalTime();
            this.Utc = dateTime.ToUniversalTime();
        }

        /// <summary>
        /// Gets or sets the local date time.
        /// </summary>
        public DateTime Local { get; set; }

        /// <summary>
        /// Gets or sets the universal date time.
        /// </summary>
        public DateTime Utc { get; set; }
    }
}
