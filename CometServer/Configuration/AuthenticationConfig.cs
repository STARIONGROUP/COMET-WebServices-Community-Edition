// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticatonConfig.cs" company="RHEA System S.A.">
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

namespace CometServer.Configuration
{
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// The purpose of the <see cref="AuthenticationConfig"/> is to provide an object-oriented
    /// and type-safe access to the Authentication settings.
    /// </summary>
    public class AuthenticationConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationConfig"/> class
        /// </summary>
        public AuthenticationConfig()
        {
            this.BasicAuthenticationConfig = new BasicAuthenticationConfig();
            this.LocalJwtAuthenticationConfig = new LocalJwtAuthenticationConfig();
            this.ExternalJwtAuthenticationConfig = new ExternalJwtAuthenticationConfig();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationConfig"/> class
        /// </summary>
        /// <param name="configuration">
        /// The injected <see cref="IConfiguration"/> used to access the application configuration
        /// </param>
        public AuthenticationConfig(IConfiguration configuration)
        {
            this.BasicAuthenticationConfig = new BasicAuthenticationConfig(configuration);
            this.LocalJwtAuthenticationConfig = new LocalJwtAuthenticationConfig(configuration);
            this.ExternalJwtAuthenticationConfig = new ExternalJwtAuthenticationConfig(configuration);
        }

        /// <summary>
        /// Gets or sets the <see cref="BasicAuthenticationConfig"/>
        /// </summary>
        public BasicAuthenticationConfig BasicAuthenticationConfig { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="LocalJwtAuthenticationConfig"/>
        /// </summary>
        public LocalJwtAuthenticationConfig LocalJwtAuthenticationConfig { get; set; }
        
        /// <summary>
        /// Gets or sets the <see cref="ExternalJwtAuthenticationConfig"/>
        /// </summary>
        public ExternalJwtAuthenticationConfig ExternalJwtAuthenticationConfig { get; set; }
    }
}
