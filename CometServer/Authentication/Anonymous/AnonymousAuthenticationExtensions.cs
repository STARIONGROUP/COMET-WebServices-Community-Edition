// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnonymousAuthenticationExtensions.cs" company="Starion Group S.A.">
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

namespace CometServer.Authentication.Anonymous
{
    using System;

    using Microsoft.AspNetCore.Authentication;

    /// <summary>
    /// Extension methods with a receiver of <see cref="AuthenticationBuilder"/>
    /// with which Anonymous Authentication can be added to the ASP.NET Core authentication pipeline.
    /// </summary>
    public static class AnonymousAuthenticationExtensions
    {
        /// <summary>
        /// The default configuration action does nothing
        /// </summary>
        public static void DefaultConfigureAction(AnonymousAuthenticationOptions options)
        {
        }

        /// <summary>
        /// Adds basic authentication with a custom authentication scheme name and display name
        /// </summary>
        public static AuthenticationBuilder AddAnonymousAuthentication(
            this AuthenticationBuilder builder,
            string displayName = AnonymousAuthenticationDefaults.DisplayName,
            Action<AnonymousAuthenticationOptions> configure = null)
            => builder.AddScheme<AnonymousAuthenticationOptions, AnonymousAuthenticationHandler>(
                AnonymousAuthenticationDefaults.AuthenticationScheme, displayName, configure ?? DefaultConfigureAction);
    }
}
