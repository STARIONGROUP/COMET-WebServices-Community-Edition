// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationModule.cs" company="Starion Group S.A.">
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
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using Carter;
    using Carter.Response;

    using CometServer.Authentication;
    using CometServer.Authentication.Bearer;
    using CometServer.Configuration;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Routing;

    /// <summary>
    /// The authentication module handler. Handles routes to do with authentication settings.
    /// </summary>
    public class AuthenticationModule : CarterModule
    {
        /// <summary>
        /// Add the routes to the <see cref="IEndpointRouteBuilder"/>
        /// </summary>
        /// <param name="app">
        /// The <see cref="IEndpointRouteBuilder"/> to which the routes are added
        /// </param>
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/login", async (LoginUser loginUser, HttpResponse res, IAuthenticationPersonAuthenticator authenticationPersonAuthenticator, IJwtTokenService jwtTokenService
            , IAppConfigService appConfigService) => 
            {
                if (!appConfigService.IsAuthenticationSchemeEnabled(JwtBearerDefaults.LocalAuthenticationScheme))
                {
                    res.StatusCode = (int)HttpStatusCode.Forbidden;
                    await res.WriteAsync("Local JWT Authentication is disabled.");
                    return;
                }

                var authenticationPerson = await authenticationPersonAuthenticator.Authenticate(loginUser.UserName, loginUser.Password);

                if (authenticationPerson != null)
                {
                    var token = jwtTokenService.GenerateTokens(authenticationPerson);
                    res.StatusCode = 200;
                    await res.AsJson(token);
                    return;
                }

                res.UpdateWithNotBearerAuthenticatedSettings();
                await res.AsJson("not authenticated");
            });

            app.MapPost("/logout", async (HttpRequest req, HttpResponse res) =>
            {
                throw new NotImplementedException();
            }).RequireAuthorization(ApiBase.AuthenticationSchemes);

            app.MapPost("/refresh", RefreshToken);
            
            app.MapGet("/auth/schemes", ProvideEnabledAuthenticationScheme);
        }

        /// <summary>
        /// Refreshes <see cref="AuthenticationTokens"/> based on a provided <paramref name="refreshToken"/>, if a valid one is provided
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="jwtTokenService">The <see cref="IJwtTokenService"/> used to refresh tokens</param>
        /// <param name="appConfigService">The <see cref="IAppConfigService"/> used to verify that the refresh token process is enabled</param>
        /// <param name="refreshToken">The provided refresh token</param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        private static async Task RefreshToken(HttpContext context, IJwtTokenService jwtTokenService, IAppConfigService appConfigService, [FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }
            
            if (!appConfigService.IsAuthenticationSchemeEnabled(JwtBearerDefaults.LocalAuthenticationScheme))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsync("Local JWT Authentication is disabled.");
                return;
            }

            try
            {
                var tokens = await jwtTokenService.TryGenerateTokenFromRefreshToken(refreshToken);

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.AsJson(tokens);
            }
            catch
            {
                context.Response.UpdateWithNotBearerAuthenticatedSettings();
                await context.Response.AsJson("not authenticated");
            }
        }

        /// <summary>
        /// Provides all enabled authentication scheme that could be used
        /// </summary>
        /// <param name="res">The <see cref="HttpResponse"/></param>
        /// <param name="appConfigService">The injected <see cref="IAppConfigService"/> uses to provides authentication scheme status</param>
        /// <returns>An awaitable <see cref="Task"/></returns>
        private static Task ProvideEnabledAuthenticationScheme(HttpResponse res, IAppConfigService appConfigService)
        {
            var enabledSchemes = ApiBase.AuthenticationSchemes.Where(appConfigService.IsAuthenticationSchemeEnabled).ToList();
            
            var authority = appConfigService.IsAuthenticationSchemeEnabled(JwtBearerDefaults.ExternalAuthenticationScheme) 
                ? appConfigService.AppConfig.AuthenticationConfig.ExternalJwtAuthenticationConfig.Authority 
                : string.Empty;
            
            var clientId = appConfigService.IsAuthenticationSchemeEnabled(JwtBearerDefaults.ExternalAuthenticationScheme) 
                ? appConfigService.AppConfig.AuthenticationConfig.ExternalJwtAuthenticationConfig.ClientId 
                : string.Empty;

            return res.AsJson(new { Schemes = enabledSchemes, Authority = authority, ClientId = clientId });
        }

        /// <summary>
        /// Data model class that defines properties required to proceed to login action
        /// </summary>
        private class LoginUser
        {
            /// <summary>
            /// Gets or sets the user name  used as credentials
            /// </summary>
            public string UserName { get; set;}

            /// <summary>
            /// Gets or sets the Password used as credentials
            /// </summary>
            public string Password { get; set;}
        }
    }
}
