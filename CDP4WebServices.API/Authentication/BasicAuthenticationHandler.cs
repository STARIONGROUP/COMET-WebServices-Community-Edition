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
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.AspNetCore.Authentication;

    /// <summary>
    /// The purpose of the <see cref="BasicAuthenticationHandler"/> is to handle Basic authentication
    /// </summary>
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        /// <summary>
        /// The injected <see cref="IUserValidator"/>
        /// </summary>
        private readonly IUserValidator userValidator;

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
        /// <param name="userValidator">
        /// The (injected) <see cref="IUserValidator"/> used to validate (authenticate) the user
        /// </param>
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IUserValidator userValidator) 
            : base(options, logger, encoder, clock)
        {
            this.userValidator = userValidator;
        }

        /// <summary>
        /// Handles the Authentication
        /// </summary>
        /// <returns>
        /// an instance of <see cref="AuthenticateResult"/>
        /// </returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            var username = string.Empty;
            ICredentials credentials = null;
            
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var basicAuthcredentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                username = basicAuthcredentials[0];
                var password = basicAuthcredentials[1];

                credentials = this.userValidator.Validate(username, password);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            if (credentials == null)
            {
                return AuthenticateResult.Fail("Invalid Username or Password");
            }

            var claims = new[] {
                new Claim(ClaimTypes.Name, username),
            };

            var identity = new ClaimsIdentity(claims, this.Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var authenticationTicket = new AuthenticationTicket(principal, this.Scheme.Name);

            return AuthenticateResult.Success(authenticationTicket);
        }
    }
}
