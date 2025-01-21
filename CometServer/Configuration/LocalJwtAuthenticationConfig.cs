// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalJwtAuthenticationConfig.cs" company="Starion Group S.A.">
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
//    Affero General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Configuration
{
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// The purpose of the <see cref="LocalJwtAuthenticationConfig"/> is to provide an object-oriented
    /// and type-safe access to the Local JWT Authentication settings.
    /// </summary>
    public class LocalJwtAuthenticationConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalJwtAuthenticationConfig"/> class
        /// </summary>
        public LocalJwtAuthenticationConfig()
        {
            this.IsEnabled = true;
            this.ValidIssuer = "CDP4-COMET";
            this.ValidAudience = "localhost:5000";
            this.SymmetricSecurityKey = "needs-to-be-updated-with-a-secret";
            }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalJwtAuthenticationConfig"/> class
        /// </summary>
        /// <param name="configuration">
        /// The injected <see cref="IConfiguration"/> used to access the application configuration
        /// </param>
        public LocalJwtAuthenticationConfig(IConfiguration configuration)
        {
            this.IsEnabled = bool.Parse(configuration["Authentication:LocalJwtBearer:IsEnabled"]);
            this.ValidIssuer = configuration["Authentication:LocalJwtBearer:ValidIssuer"];
            this.ValidAudience = configuration["Authentication:LocalJwtBearer:ValidAudience"];
            this.SymmetricSecurityKey = configuration["Authentication:LocalJwtBearer:SymmetricSecurityKey"];
            
            if (int.TryParse(configuration["Authentication:LocalJwtBearer:TokenExpirationMinutes"], out var tokenExpirationMinutes))
            {
                this.TokenExpirationMinutes = tokenExpirationMinutes;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether Local JWT Authentication is enabled or not
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the valid issuer, the valid issuer is the hostname of the issuer of the bearer tokens
        /// </summary>
        /// <remarks>
        /// this needs to a hostname:port such as localhost:5000 
        /// </remarks>
        public string ValidIssuer { get; set; }

        /// <summary>
        /// Gets or sets the valid audience which is a name of the issuer.
        /// </summary>
        /// <remarks>
        /// this can be a name (string) or a hostname:port
        /// </remarks>
        public string ValidAudience { get; set; }

        /// <summary>
        /// Gets or sets the symmetric security key with which the bearer tokens are generated and also validated
        /// </summary>
        public string SymmetricSecurityKey { get; set; }

        /// <summary>
        /// Gets or sets the expiration time of a generated JWT Token, in minutes (defaults: 30)
        /// </summary>
        public int TokenExpirationMinutes { get; set; } = 30;
    }
}
