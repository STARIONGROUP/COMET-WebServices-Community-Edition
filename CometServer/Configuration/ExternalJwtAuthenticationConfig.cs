// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalJwtAuthenticationConfig.cs" company="RHEA System S.A.">
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

namespace CometServer.Configuration
{
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// The purpose of the <see cref="ExternalJwtAuthenticationConfig"/> is to provide an object-oriented
    /// and type-safe access to the External JWT Authentication settings.
    /// </summary>
    public class ExternalJwtAuthenticationConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalJwtAuthenticationConfig"/> class
        /// </summary>
        public ExternalJwtAuthenticationConfig()
        {
            this.IsEnabled = true;
        }

        public ExternalJwtAuthenticationConfig(IConfiguration configuration)
        {
            this.IsEnabled = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the External JWT Authentication is enabled or not
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
