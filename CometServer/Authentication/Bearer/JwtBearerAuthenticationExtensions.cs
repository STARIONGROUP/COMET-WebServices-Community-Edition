// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JwtBearerAuthenticationExtensions.cs" company="Starion Group S.A.">
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

namespace CometServer.Authentication.Bearer
{
    using System;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;

    /// <summary>
    /// Extension methods with a receiver of <see cref="AuthenticationBuilder"/>
    /// with which JWT Bearer Authentication can be added to the ASP.NET Core authentication pipeline.
    /// </summary>
    public static class JwtBearerAuthenticationExtensions
    {
        /// <summary>
        /// The default configuration action does nothing
        /// </summary>
        public static void DefaultConfigureAction(JwtBearerOptions options)
        {
        }

        /// <summary>
        /// Adds local JWT authentication with a custom authentication scheme name and display name
        /// </summary>
        public static AuthenticationBuilder AddLocalJwtBearerAuthentication(
            this AuthenticationBuilder builder,
            string displayName = JwtBearerDefaults.LocalAuthenticationScheme,
            Action<JwtBearerOptions> configure = null)
            => builder.AddScheme<JwtBearerOptions, JwtBearerAuthenticationHandler>(
                JwtBearerDefaults.LocalAuthenticationScheme, displayName, configure ?? DefaultConfigureAction);

        /// <summary>
        /// Adds local JWT authentication with a custom authentication scheme name and display name
        /// </summary>
        public static AuthenticationBuilder AddExternalJwtBearerAuthentication(
            this AuthenticationBuilder builder,
            string displayName = JwtBearerDefaults.ExternalAuthenticationScheme,
            Action<JwtBearerOptions> configure = null)
            => builder.AddScheme<JwtBearerOptions, JwtBearerAuthenticationHandler>(
                JwtBearerDefaults.ExternalAuthenticationScheme, displayName, configure ?? DefaultConfigureAction);
    }
}
