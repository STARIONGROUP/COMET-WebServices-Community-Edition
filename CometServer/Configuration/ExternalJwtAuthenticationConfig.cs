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
    using System;
    using System.Linq;

    using CDP4Authentication;

    using CometServer.Enumerations;

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
            this.IsEnabled = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalJwtAuthenticationConfig"/> class based on the defined <see cref="IConfiguration"/>
        /// </summary>
        /// <param name="configuration">The injected <see cref="IConfiguration" /></param>
        public ExternalJwtAuthenticationConfig(IConfiguration configuration)
        {
            this.IsEnabled = bool.Parse(configuration["Authentication:ExternalJwtBearer:IsEnabled"]!);
            this.ValidAudience = configuration["Authentication:ExternalJwtBearer:ValidAudience"];
            this.Authority = configuration["Authentication:ExternalJwtBearer:Authority"];
            this.ValidIssuer = configuration["Authentication:ExternalJwtBearer:ValidIssuer"];
            this.IdentifierClaimName = configuration["Authentication:ExternalJwtBearer:IdentifierClaimName"];
            this.PersonIdentifierPropertyKind = Enum.Parse<PersonIdentifierPropertyKind>(configuration["Authentication:ExternalJwtBearer:PersonIdentifierPropertyKind"]!);
        }

        /// <summary>
        /// Gets or sets the Valid Issuer that issue the JWT token
        /// </summary>
        public string ValidIssuer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the External JWT Authentication is enabled or not
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the valid audience which is a name of the issuer.
        /// </summary>
        /// <remarks>
        /// this can be a name (string) or a hostname:port
        /// </remarks>
        public string ValidAudience { get; set; }

        /// <summary>
        /// Gets or sets the URI to access the JWT authority
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Gets or sets the name of the Claim that should be used as identifier
        /// </summary>
        public string IdentifierClaimName { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PersonIdentifierPropertyKind"/> to use for the authorization part
        /// </summary>
        public PersonIdentifierPropertyKind PersonIdentifierPropertyKind { get; set; }
    }
}
