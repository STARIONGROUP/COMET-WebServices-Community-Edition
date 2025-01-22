// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JwtBearerAuthenticationHandler.cs" company="Starion Group S.A.">
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
    using System.Net;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using Carter.Response;

    using CometServer.Configuration;
    using CometServer.Extensions;
    using CometServer.Health;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// The <see cref="JwtBearerAuthenticationHandler" /> is a custom <see cref="JwtBearerHandler" /> that allows authentication based on defined configuration
    /// </summary>
    public class JwtBearerAuthenticationHandler : JwtBearerHandler
    {
        /// <summary>
        /// The injected <see cref="IAppConfigService" />
        /// </summary>
        private readonly IAppConfigService appConfigService;

        /// <summary>
        /// The injected <see cref="ICometHasStartedService" /> that checks that all required services are loaded
        /// </summary>
        private readonly ICometHasStartedService cometHasStartedService;

        /// <summary>
        /// Initializes a new instance of <see cref="JwtBearerAuthenticationHandler" />.
        /// </summary>
        /// <param name="options">The monitor for the options instance.</param>
        /// <param name="logger">The <see cref="T:Microsoft.Extensions.Logging.ILoggerFactory" />.</param>
        /// <param name="encoder">The <see cref="T:System.Text.Encodings.Web.UrlEncoder" />.</param>
        /// <param name="clock">The <see cref="T:Microsoft.AspNetCore.Authentication.ISystemClock" />.</param>
        /// <param name="cometHasStartedService">
        /// The (injected) <see cref="ICometHasStartedService" /> that is used to check whether CDP4-COMET is ready to start
        /// accepting requests
        /// </param>
        /// <param name="appConfigService">The injected <see cref="IAppConfigService" /></param>
        public JwtBearerAuthenticationHandler(IOptionsMonitor<JwtBearerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
            ICometHasStartedService cometHasStartedService, IAppConfigService appConfigService) : base(options, logger, encoder, clock)
        {
            this.cometHasStartedService = cometHasStartedService;
            this.appConfigService = appConfigService;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="JwtBearerAuthenticationHandler" />.
        /// </summary>
        /// <param name="options">The monitor for the options instance.</param>
        /// <param name="logger">The <see cref="T:Microsoft.Extensions.Logging.ILoggerFactory" />.</param>
        /// <param name="encoder">The <see cref="T:System.Text.Encodings.Web.UrlEncoder" />.</param>
        /// <param name="cometHasStartedService">
        /// The (injected) <see cref="ICometHasStartedService" /> that is used to check whether CDP4-COMET is ready to start
        /// accepting requests
        /// </param>
        /// <param name="appConfigService">The injected <see cref="IAppConfigService" /></param>
        public JwtBearerAuthenticationHandler(IOptionsMonitor<JwtBearerOptions> options, ILoggerFactory logger, UrlEncoder encoder,
            ICometHasStartedService cometHasStartedService, IAppConfigService appConfigService) : base(options, logger, encoder)
        {
            this.cometHasStartedService = cometHasStartedService;
            this.appConfigService = appConfigService;
        }

        /// <summary>
        /// Override this method to deal with 401 challenge concerns, if an authentication scheme in question
        /// deals an authentication interaction as part of it's request flow. (like adding a response header, or
        /// changing the 401 result to 302 of a login page or external sign-in location.)
        /// </summary>
        /// <param name="properties"></param>
        /// <returns>A Task.</returns>
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            if (this.Response.HasStarted)
            {
                return;
            }

            if (!this.cometHasStartedService.GetHasStartedAndIsReady().IsHealthy)
            {
                this.Response.ContentType = "application/json";
                this.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                await this.Response.AsJson("The CDP4-COMET Server has not yet started and is not ready to accept requests");

                return;
            }
            
            var authorizationHeader = this.Request.Headers.Authorization;

            if (string.IsNullOrEmpty(authorizationHeader))
            {
                this.Response.ContentType = "application/json";
                this.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await this.Response.AsJson("Missing Authorization Header");
                return;
            }
            
            if (!this.Request.DoesAuthorizationHeaderMatches(this.Scheme.Name))
            {
                return;
            }

            if (!this.appConfigService.IsAuthenticationSchemeEnabled(this.Scheme.Name))
            {
                this.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                this.Response.ContentType = "application/json";
                await this.Response.AsJson($"The CDP4-COMET Server does not allow {this.Scheme.Name} Authentication");
                return;
            }

            await base.HandleChallengeAsync(properties);
        }

        /// <summary>Allows derived types to handle authentication.</summary>
        /// <returns>The <see cref="T:Microsoft.AspNetCore.Authentication.AuthenticateResult" />.</returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                if (!this.cometHasStartedService.GetHasStartedAndIsReady().IsHealthy
                    || !this.appConfigService.IsAuthenticationSchemeEnabled(this.Scheme.Name))
                {
                    return AuthenticateResult.NoResult();
                }
                
                var authorizationHeader = this.Request.Headers.Authorization;

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    this.Logger.LogInformation("no Authorization header provided");
                    return AuthenticateResult.NoResult();
                }

                if (!this.Request.DoesAuthorizationHeaderMatches(this.Scheme.Name))
                {
                    return AuthenticateResult.NoResult();
                }

                var authHeader = this.Request.Headers.Authorization.ToString();
                this.Request.Headers.Authorization = string.Concat("Bearer ", authHeader.AsSpan(this.Scheme.Name.Length + 1));

                var value = await base.HandleAuthenticateAsync();
                this.Request.Headers.Authorization = authHeader;
                return value;
            }
            catch (SecurityTokenValidationException)
            {
                return AuthenticateResult.Fail("Invalid token");
            }
            catch (SecurityTokenMalformedException)
            {
                return AuthenticateResult.Fail("Malformed Token");
            }
            catch (Exception)
            {
                return AuthenticateResult.Fail("Invalid authentication token");
            }
        }
    }
}
