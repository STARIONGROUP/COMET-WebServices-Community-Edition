// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicAuthenticationHandler.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
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
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using CDP4Authentication;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.AspNetCore.Authentication;

    /// <summary>
    /// The purpose of the <see cref="BasicAuthenticationHandler"/> is to handle Basic authentication
    /// </summary>
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        /// <summary>
        /// The (injected) <see cref="IAuthenticationPersonAuthenticator"/> used to authenticate a user based on a username and password
        /// </summary>
        private readonly IAuthenticationPersonAuthenticator authenticationPersonAuthenticator;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthenticationHandler"/> class.
        /// </summary>
        /// <param name="options">
        /// The monitor for the options instance
        /// </param>
        /// <param name="logger">
        /// The <see cref="ILoggerFactory"/>
        /// </param>
        /// <param name="encoder">
        /// >The <see cref="System.Text.Encodings.Web.UrlEncoder"/>
        /// </param>
        /// <param name="clock">
        /// The <see cref="ISystemClock"/>
        /// </param>
        /// <param name="authenticationPersonAuthenticator">
        /// The (injected) <see cref="IAuthenticationPersonAuthenticator"/> used to authenticate a user based on a username and password
        /// </param>
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IAuthenticationPersonAuthenticator authenticationPersonAuthenticator) 
            : base(options, logger, encoder, clock)
        {
            this.authenticationPersonAuthenticator = authenticationPersonAuthenticator;
        }

        /// <summary>
        /// Handles the Authentication
        /// </summary>
        /// <returns>
        /// an instance of <see cref="AuthenticateResult"/>
        /// </returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!this.Request.Headers.ContainsKey("Authorization"))
            {
                this.Logger.LogInformation("The Authorization Header is missing from the request");

                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            var sw = Stopwatch.StartNew();

            string username;
            AuthenticationPerson authenticationPerson;

            try
            {
                this.Logger.LogInformation("Extracting authentication data from the Authorization Header");

                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var basicAuthcredentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                username = basicAuthcredentials[0];
                var password = basicAuthcredentials[1];

                this.Logger.LogInformation($"Authenticating user: {username}");

                authenticationPerson = await this.authenticationPersonAuthenticator.Authenticate(username, password);
            }
            catch
            {
                this.Logger.LogInformation("Invalid Authorization Header");

                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            if (authenticationPerson == null)
            {
                this.Logger.LogInformation($"$Invalid Username or Password for: {username}");

                return AuthenticateResult.Fail("Invalid Username or Password");
            }

            var claims = new[] {
                new Claim(ClaimTypes.Name, username),
            };

            var identity = new ClaimsIdentity(claims, this.Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var authenticationTicket = new AuthenticationTicket(principal, this.Scheme.Name);

            this.Logger.LogInformation($"User: {username} authenticated in {sw.ElapsedMilliseconds} [ms]");

            return AuthenticateResult.Success(authenticationTicket);
        }
    }
}
