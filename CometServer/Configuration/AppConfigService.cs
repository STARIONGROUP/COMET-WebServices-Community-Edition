// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppConfigService.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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
    using System.Linq;

    using CDP4Authentication;

    using CometServer.Authentication.Basic;
    using CometServer.Authentication.Bearer;

    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// A service used to provide access to the <see cref="AppConfig"/>
    /// </summary>
    public class AppConfigService : IAppConfigService
    {
        /// <summary>
        /// Gets the injected <see cref="IAuthenticationPluginInjector"/>, uses to check wheter that external JWT authentication is enabled or not
        /// </summary>
        private readonly IAuthenticationPluginInjector authenticationPluginInjector;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigService"/>
        /// </summary>
        /// <param name="configuration">
        /// The <see cref="IConfiguration"/> used to set the properties
        /// </param>
        /// <param name="authenticationPluginInjector">The injected <see cref="IAuthenticationPluginInjector"/>, uses to check wheter that external JWT authentication is enabled or not</param>
        public AppConfigService(IConfiguration configuration, IAuthenticationPluginInjector authenticationPluginInjector)
        {
            this.AppConfig = new AppConfig(configuration);
            this.authenticationPluginInjector = authenticationPluginInjector;
        }

        /// <summary>
        /// Gets the <see cref="AppConfig"/>
        /// </summary>
        public AppConfig AppConfig { get; }

        /// <summary>
        /// Assert that an authentication scheme is enabled or not
        /// </summary>
        /// <param name="schemeName">The name of the authentication scheme</param>
        /// <returns>True if the authentication scheme is enabled, false otherwise</returns>
        public bool IsAuthenticationSchemeEnabled(string schemeName)
        {
            return schemeName switch
            {
                BasicAuthenticationDefaults.AuthenticationScheme => this.AppConfig.AuthenticationConfig.BasicAuthenticationConfig.IsEnabled,
                JwtBearerDefaults.LocalAuthenticationScheme => this.AppConfig.AuthenticationConfig.LocalJwtAuthenticationConfig.IsEnabled,
                JwtBearerDefaults.ExternalAuthenticationScheme => this.AppConfig.AuthenticationConfig.ExternalJwtAuthenticationConfig.IsEnabled 
                                                                  && this.authenticationPluginInjector.Connectors.Any(x => x.Name == "CDP4ExternalJwtAuthentication" && x.Properties.IsEnabled),
                _ => false
            };
        }
    }
}
