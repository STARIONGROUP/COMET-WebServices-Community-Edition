﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RootModule.cs" company="Starion Group S.A.">
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

namespace CometServer.Modules
{
    using Carter;

    using CometServer.Authentication.Anonymous;
    using CometServer.Resources;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    /// <summary>
    /// The <see cref="CarterModule"/> responsipble for handling HTTP requets for the root /
    /// </summary>
    public class RootModule : CarterModule
    {
        /// <summary>
        /// Add the routes to the <see cref="IEndpointRouteBuilder"/>
        /// </summary>
        /// <param name="app">
        /// The <see cref="IEndpointRouteBuilder"/> to which the routes are added
        /// </param>
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/", async (HttpRequest req, HttpResponse res, IResourceLoader resourceLoader) =>
            {
                var modelVersionHtml =
                    string.Join(", ", 
                    resourceLoader.QueryModelVersions());

                var rootPageTemplate = 
                    resourceLoader.QueryRootPage()
                        .Replace("{{basePath}}", req.PathBase)
                        .Replace("{{sdkVersion}}", resourceLoader.QuerySDKVersion())
                        .Replace("{{apiVersion}}", resourceLoader.QueryVersion())
                        .Replace("{{modelVersions}}", modelVersionHtml);

                res.ContentType = "text/html";
                await res.WriteAsync(rootPageTemplate);
                await res.CompleteAsync();
            }).RequireAuthorization(new[] { AnonymousAuthenticationDefaults.AuthenticationScheme });
        }
    }
}
