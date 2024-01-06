// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicAuthenticationExtensions.cs" company="RHEA System S.A.">
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

namespace CometServer.Authentication.Basic
{
    using Microsoft.AspNetCore.Authentication;

    using System;

    /// <summary>
    /// Extension methods with a receiver of <see cref="AuthenticationBuilder"/>
    /// with which Basic Authentication can be added to the ASP.NET Core authentication pipeline.
    /// </summary>
    public static class BasicAuthenticationExtensions
    {
        /// <summary>
        /// The default configuration action does nothing
        /// </summary>
        public static void DefaultConfigureAction(BasicAuthenticationOptions options)
        {
        }

        /// <summary>
        /// Adds basic authentication with a custom authentication scheme name and display name
        /// </summary>
        public static AuthenticationBuilder AddBasicAuthentication(
            this AuthenticationBuilder builder,
            string displayName = BasicAuthenticationDefaults.DisplayName,
            Action<BasicAuthenticationOptions> configure = null)
            => builder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(
                BasicAuthenticationDefaults.AuthenticationScheme, displayName, configure ?? DefaultConfigureAction);
    }
}
