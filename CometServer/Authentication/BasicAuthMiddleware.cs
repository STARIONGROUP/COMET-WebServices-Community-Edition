// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicAuthenticatonMiddleware.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2022 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Authentication
{
    using System;
    using System.Diagnostics;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The purpose of the <see cref="BasicAuthenticatonMiddleware"/> is to add basic authentication to the pipeline
    /// </summary>
    public class BasicAuthenticatonMiddleware
    {
        /// <summary>
        /// The <see cref="RequestDelegate"/> of the pipeline
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// The <see cref="ILogger"/> used for logging
        /// </summary>
        private readonly ILogger<BasicAuthenticatonMiddleware> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthMiddleware"/>
        /// </summary>
        /// <param name="next">
        /// The <see cref="RequestDelegate"/> of the request pipeline
        /// </param>
        /// <param name="loggerFactory">
        /// The <see cref="ILoggerFactory"/> to enable logging
        /// </param>
        public BasicAuthenticatonMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;

            this.logger = loggerFactory.CreateLogger<BasicAuthenticatonMiddleware>();
        }

        /// <summary>
        /// Invokes the basic authentication logic
        /// </summary>
        /// <param name="context">
        /// The <see cref="HttpContext"/>
        /// </param>
        /// <param name="authenticationPersonAuthenticator">
        /// The (injected) <see cref="IAuthenticationPersonAuthenticator"/> used to authenticate the user against
        /// the standard COMET user database or an injected authenticator plugin
        /// </param>
        /// <returns>
        /// an awaitable task
        /// </returns>
        public async Task Invoke(HttpContext context, IAuthenticationPersonAuthenticator authenticationPersonAuthenticator)
        {
            var sw = Stopwatch.StartNew();

            string username = string.Empty;

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);
                if (authHeader != null && authHeader.Scheme != "Basic")
                {
                    await this.next(context);
                    return;
                }

                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var basicAuthcredentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                username = basicAuthcredentials[0];
                var password = basicAuthcredentials[1];

                var authenticationPerson = await authenticationPersonAuthenticator.Authenticate(username, password);

                if (authenticationPerson != null)
                {
                    var claims = new[] {
                        new Claim(ClaimTypes.Name, username),
                    };

                    var identity = new ClaimsIdentity(claims, "Basic");
                    var principal = new ClaimsPrincipal(identity);
                    var authenticationTicket = new AuthenticationTicket(principal, "Basic");

                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        IsPersistent = false,
                    };

                    // sign in and add cookie
                    await context.SignInAsync("CDP4", principal, authProperties);

                    context.User = principal;
                }
            }
            catch(Exception ex)
            {
                // do nothing if invalid auth header
                // user is not attached to context so request won't have access to secure routes
                this.logger.LogDebug(ex, $"The {username} could not be authenticated using Basic Authentication" );
            }

            await this.next(context);
        }
    }

    /// <summary>
    /// The purpose of the <see cref="BasicAuthenticatonMiddlewareExtensions"/> is to add the <see cref="BasicAuthenticatonMiddleware"/>
    /// to the request pipeline us the Use syntax
    /// </summary>
    public static class BasicAuthenticatonMiddlewareExtensions
    {
        /// <summary>
        /// Add the <see cref="BasicAuthenticatonMiddleware"/> to the request pipeline
        /// </summary>
        /// <param name="builder">
        /// The <see cref="IApplicationBuilder"/>
        /// </param>
        /// <returns>
        /// The application builder
        /// </returns>
        /// <remarks>
        /// Used for Basic Authentication
        /// </remarks>
        public static IApplicationBuilder UseBasicAuthenticatonMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BasicAuthenticatonMiddleware>();
        }
    }
}
