// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicAuthenticationHandler.cs" company="RHEA System S.A.">
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
    using System;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using Carter.Response;

    using CometServer.Authentication;
    using CometServer.Configuration;
    using CometServer.Exceptions;
    using CometServer.Extensions;
    using CometServer.Health;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// An <see cref="AuthenticationHandler{BasicAuthenticationOptions}"/> that implements Basic authentication 
    /// </summary>
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        /// <summary>
        /// The (injected) <see cref="IAuthenticationPersonAuthenticator"/> used to authenticate the user
        /// </summary>
        private readonly IAuthenticationPersonAuthenticator authenticationPersonAuthenticator;

        /// <summary>
        /// The (injected) <see cref="ICometHasStartedService"/>
        /// </summary>
        private readonly ICometHasStartedService cometHasStartedService;

        /// <summary>
        /// The (inject) <see cref="IAppConfigService" />
        /// </summary>
        private readonly IAppConfigService appConfigService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthenticationHandler"/>
        /// </summary>
        /// <param name="options">
        /// The <see cref="BasicAuthenticationOptions"/> used to configure the <see cref="BasicAuthenticationHandler"/>
        /// </param>
        /// <param name="logger">
        /// The <see cref="ILoggerFactory"/> used to create the <see cref="Logger{T}"/>
        /// </param>
        /// <param name="encoder">
        /// The <see cref="UrlEncoder"/>
        /// </param>
        /// <param name="clock">
        /// The <see cref="ISystemClock"/> (obsolote)
        /// </param>
        /// <param name="authenticationPersonAuthenticator">
        /// The (injected) <see cref="IAuthenticationPersonAuthenticator"/> used to authenticate against the configured authentication store
        /// </param>
        /// <param name="cometHasStartedService">
        /// The (injected) <see cref="ICometHasStartedService"/> that is used to check whether CDP4-COMET is ready to start
        /// acceptng requests
        /// </param>
        /// <param name="appConfigService">The injected <see cref="IAppConfigService"/> that is used to enable or not this authentication handler</param>
        public BasicAuthenticationHandler(IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IAuthenticationPersonAuthenticator authenticationPersonAuthenticator, ICometHasStartedService cometHasStartedService, IAppConfigService appConfigService)
            : base(options, logger, encoder, clock)
        {
            this.authenticationPersonAuthenticator = authenticationPersonAuthenticator;
            this.cometHasStartedService = cometHasStartedService;
            this.appConfigService = appConfigService;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthenticationHandler"/>
        /// </summary>
        /// <param name="options">
        /// The <see cref="BasicAuthenticationOptions"/> used to configure the authentication handler
        /// </param>
        /// <param name="logger">
        /// The <see cref="ILoggerFactory"/> used to create the <see cref="Logger{T}"/>
        /// </param>
        /// <param name="encoder">
        /// A <see cref="UrlEncoder"/>
        /// </param>
        /// <param name="authenticationPersonAuthenticator">
        /// The (injected) <see cref="IAuthenticationPersonAuthenticator"/> used to authenticate against the configured authentication store
        /// </param>
        /// <param name="cometHasStartedService">
        /// The (injected) <see cref="ICometHasStartedService"/> that is used to check whether CDP4-COMET is ready to start
        /// accepting requests
        /// </param>
        /// <param name="appConfigService">The injected <see cref="IAppConfigService"/> that is used to enable or not this authentication handler</param>
        public BasicAuthenticationHandler(IOptionsMonitor<BasicAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, IAuthenticationPersonAuthenticator authenticationPersonAuthenticator, ICometHasStartedService cometHasStartedService, IAppConfigService appConfigService) 
            : base(options, logger, encoder)
        {
            this.authenticationPersonAuthenticator = authenticationPersonAuthenticator;
            this.cometHasStartedService = cometHasStartedService;
            this.appConfigService = appConfigService;
        }

        /// <summary>
        /// method that handles a authentication challenge (a 401 response)
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
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

            if (!this.Request.DoesAuthorizationHeaderMatches(this.Scheme.Name))
            {
                return;
            }
            
            if (!this.appConfigService.IsAuthenticationSchemeEnabled(this.Scheme.Name))
            {
                this.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                this.Response.ContentType = "application/json";
                await this.Response.AsJson("The CDP4-COMET Server does not allow basic authentication");
                return;
            }

            if (!this.Options.IsWWWAuthenticateHeaderSuppressed)
            {
                this.Response.Headers.WWWAuthenticate = $"Basic realm=\"{Options.Realm}\", charset=\"UTF-8\"";
            }

            await base.HandleChallengeAsync(properties);
        }

        /// <summary>
        /// Handles the authentication request
        /// </summary>
        /// <returns>
        /// The <see cref="AuthenticateResult"/> that denotes success or failure
        /// </returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                if (!this.cometHasStartedService.GetHasStartedAndIsReady().IsHealthy || !this.appConfigService.IsAuthenticationSchemeEnabled(this.Scheme.Name))
                {
                    return AuthenticateResult.NoResult();
                }

                var authorizationHeader = this.Request.Headers.Authorization;

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    this.Logger.LogInformation("no Authorization header provided");
                    return AuthenticateResult.NoResult();
                }

                var authHeader = AuthenticationHeaderValue.Parse(authorizationHeader);

                if (authHeader.Scheme != this.Scheme.Name)
                {
                    return AuthenticateResult.NoResult();
                }

                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var basicAuthcredentials = Encoding.UTF8.GetString(credentialBytes).Split([':'], 2);

                if (basicAuthcredentials.Length != 2)
                {
                    return AuthenticateResult.Fail("Invalid Basic authentication header format.");
                }

                var username = basicAuthcredentials[0];
                var password = basicAuthcredentials[1];

                var authenticationPerson = await this.authenticationPersonAuthenticator.Authenticate(username, password);

                if (authenticationPerson == null)
                {
                    this.Logger.LogWarning("The {username} could not be authenticated", username);
                    return AuthenticateResult.Fail("Invalid username or password.");
                }

                var claims = new[] { new Claim(ClaimTypes.Name, username) };
                var identity = new ClaimsIdentity(claims, this.Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, this.Scheme.Name);

                this.Logger.LogWarning("The {useid}:{username} successfully authenticated", authenticationPerson.Iid, username);

                // sign in and add cookie
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = false,
                };

                await this.Context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                return AuthenticateResult.Success(ticket);
            }
            catch (AuthenticatorException)
            {
                this.Logger.LogError( "Basic authentication failed - database error - service unavailable");

                this.Response.ContentType = "application/json";
                this.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                await this.Response.AsJson("Service Unavailable");

                return AuthenticateResult.NoResult();
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Basic authentication failed");
                throw;
            }
        }
    }
}
