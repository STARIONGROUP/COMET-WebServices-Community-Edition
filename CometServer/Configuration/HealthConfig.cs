// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HealthConfig.cs" company="RHEA System S.A.">
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
    using System;

    using Microsoft.Extensions.Configuration;
    
    /// <summary>
    /// The health configuration
    /// </summary>
    public class HealthConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HealthConfig"/> class.
        /// </summary>
        public HealthConfig()
        {
            this.RequireHost = Array.Empty<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthConfig"/> class.
        /// </summary>
        public HealthConfig(IConfiguration configuration)
        {
            this.RequireHost = configuration.GetSection("Health:RequireHost").Get<string[]>() ?? Array.Empty<string>();
        }
        
        /// <summary>
        /// Gets or sets the required hostnames for the health services
        /// </summary>
        public string[] RequireHost { get; set; }
    }
}
