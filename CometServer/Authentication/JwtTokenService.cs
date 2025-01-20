// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JwtTokenService.cs" company="RHEA System S.A.">
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

namespace CometServer.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    using CometServer.Configuration;

    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;

    using CDP4Authentication;

    /// <summary>
    /// The purpose of the <see cref="JwtTokenService"/> is to generate a JWT token based on the provided authenticated
    /// user
    /// </summary>
    public class JwtTokenService : IJwtTokenService
    {
        /// <summary>
        /// Provides the expiration times of a generated JWT token, in minutes
        /// </summary>
        private const int ExpirationMinutes = 30;

        /// <summary>
        /// The (injected) logger
        /// </summary>
        private readonly ILogger<JwtTokenService> logger;

        /// <summary>
        /// The (injected) <see cref="IAppConfigService"/>
        /// </summary>
        private readonly IAppConfigService appConfigService;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtTokenService"/>
        /// </summary>
        /// <param name="logger">
        /// The (injected) logger
        /// </param>
        /// <param name="appConfigService">
        /// The (injected) <see cref="IAppConfigService"/>
        /// </param>
        public JwtTokenService(ILogger<JwtTokenService> logger, IAppConfigService appConfigService)
        {
            this.logger = logger;
            this.appConfigService = appConfigService;
        }

        /// <summary>
        /// Creates a JWT token based on the <see cref="AuthenticationPerson"/> and the settings provided
        /// by the <see cref="IAppConfigService"/>
        /// </summary>
        /// <param name="authenticationPerson">
        /// The subject <see cref="AuthenticationPerson"/>
        /// </param>
        /// <returns>The created JWT token</returns>
        public string CreateToken(AuthenticationPerson authenticationPerson)
        {
            var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);

            var jwtSecurityToken = this.CreateJwtSecurityToken(
                CreateClaims(authenticationPerson),
                this.CreateSigningCredentials(),
                expiration
            );

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            this.logger.LogInformation("JWT Token created");

            return jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
        }

        /// <summary>
        /// Creates a <see cref="JwtSecurityToken"/>
        /// </summary>
        /// <param name="claims">
        /// The provided list of <see cref="Claim"/>s
        /// </param>
        /// <param name="credentials">
        /// The <see cref="SigningCredentials"/> used to sing the token
        /// </param>
        /// <param name="expiration">
        /// The expiration date
        /// </param>
        /// <returns>The created <see cref="JwtSecurityToken"/></returns>
        private JwtSecurityToken CreateJwtSecurityToken(List<Claim> claims, SigningCredentials credentials,
            DateTime expiration)
        {
            var jwtSecurityToken = new JwtSecurityToken(
                this.appConfigService.AppConfig.AuthenticationConfig.LocalJwtAuthenticationConfig.ValidIssuer,
                this.appConfigService.AppConfig.AuthenticationConfig.LocalJwtAuthenticationConfig.ValidAudience,
                claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return jwtSecurityToken;
        }

        /// <summary>
        /// Creates the claims for the <see cref="AuthenticationPerson"/>
        /// </summary>
        /// <param name="authenticationPerson">The <see cref="AuthenticationPerson"/> to use for generate <see cref="Claim"/>s</param>
        /// <returns>A <see cref="List{T}"/> of <see cref="Claim"/> generated from the <see cref="AuthenticationPerson"/></returns>
        private static List<Claim> CreateClaims(AuthenticationPerson authenticationPerson)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, authenticationPerson.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim(ClaimTypes.Name, authenticationPerson.UserName),
                new Claim(ClaimTypes.NameIdentifier, authenticationPerson.Iid.ToString())
            };

            return claims;
        }

        /// <summary>
        /// Creates an instance of <see cref="SigningCredentials"/> using the configured
        /// symmetric security key
        /// </summary>
        /// <returns>
        /// an instance of <see cref="SigningCredentials"/>
        /// </returns>
        private SigningCredentials CreateSigningCredentials()
        {
            var symmetricSecurityKey = this.appConfigService.AppConfig.AuthenticationConfig.LocalJwtAuthenticationConfig.SymmetricSecurityKey;

            return new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(symmetricSecurityKey)
                ),
                SecurityAlgorithms.HmacSha512Signature
            );
        }
    }
}
