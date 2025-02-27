// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnonymousAuthenticationHandler.cs" company="Starion Group S.A.">
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
    using System.Security.Claims;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// An <see cref="AuthenticationHandler{AnonymousAuthenticationOptions}"/> that implements Basic authentication 
    /// </summary>
    public class AnonymousAuthenticationHandler : AuthenticationHandler<AnonymousAuthenticationOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousAuthenticationHandler"/>
        /// </summary>
        /// <param name="options">
        /// The <see cref="AnonymousAuthenticationOptions"/> used to configure the <see cref="AnonymousAuthenticationHandler"/>
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
        public AnonymousAuthenticationHandler(IOptionsMonitor<AnonymousAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousAuthenticationHandler"/>
        /// </summary>
        /// <param name="options">
        /// The <see cref="AnonymousAuthenticationOptions"/> used to configure the authentication handler
        /// </param>
        /// <param name="logger">
        /// The <see cref="ILoggerFactory"/> used to create the <see cref="Logger{T}"/>
        /// </param>
        /// <param name="encoder">
        /// A <see cref="UrlEncoder"/>
        /// </param>
        public AnonymousAuthenticationHandler(IOptionsMonitor<AnonymousAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
        {
        }

        /// <summary>
        /// Handles the authentication request
        /// </summary>
        /// <returns>
        /// The <see cref="AuthenticateResult"/> that denotes success or failure
        /// </returns>
        /// <remarks>
        /// This will always reutrn AuthenticateResult.NoResult
        /// </remarks>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "anonymous") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
