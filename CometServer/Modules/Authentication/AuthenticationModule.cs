// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationModule.cs" company="RHEA System S.A.">
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

namespace CometServer.Modules
{
    using System;

    using Carter;
    using Carter.Response;

    using CometServer.Authentication;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
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
            app.MapPost("/login", async (LoginUser loginUser, HttpResponse res, IAuthenticationPersonAuthenticator authenticationPersonAuthenticator, IJwtTokenService jwtTokenService) => 
            {
                var authenticationPerson = await authenticationPersonAuthenticator.Authenticate(loginUser.UserName, loginUser.Password);

                if (authenticationPerson != null)
                {
                    var token = jwtTokenService.CreateToken(authenticationPerson);
                    res.StatusCode = 200;
                    await res.WriteAsync(token);
                    return;
                }

                res.UpdateWithNotBearerAuthenticatedSettings();
                await res.AsJson("not authenticated");
            });

            app.MapPost("/logout", async (HttpRequest req, HttpResponse res) => 
            {
                //return webServiceAuthentication.LogOutResponse(req.HttpContext);
                throw new NotImplementedException();
            });
        }

        private class LoginUser
        {
            public string UserName { get; set;}

            public string Password { get; set;}
        }
    }
}
