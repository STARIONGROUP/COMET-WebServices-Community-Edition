// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JwtTokenService.cs" company="Starion Group S.A.">
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

namespace CometServer.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    using CDP4Authentication;

    using CometServer.Authorization;
    using CometServer.Configuration;
    using CometServer.Services;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.JsonWebTokens;
    using Microsoft.IdentityModel.Tokens;

    using Npgsql;

    using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

    /// <summary>
    /// The purpose of the <see cref="JwtTokenService" /> is to generate a JWT token based on the provided authenticated user
    /// </summary>
    public class JwtTokenService : IJwtTokenService
    {
        /// <summary>
        /// Gets the value of the "typ" <see cref="Claim" /> in case of refresh token
        /// </summary>
        private const string RefreshClaimValue = "Refresh";

        /// <summary>
        /// Gets the value of the "typ" <see cref="Claim" /> in case of access token
        /// </summary>
        private const string AccessClaimValue = "Bearer";

        /// <summary>
        /// The (injected) <see cref="IAppConfigService" />
        /// </summary>
        private readonly IAppConfigService appConfigService;

        /// <summary>
        /// The injected <see cref="ICredentialsService" />, used to retrieve the previously authenticated
        /// <see cref="AuthenticationPerson" /> based on the
        /// refresh token
        /// </summary>
        private readonly ICredentialsService credentialsService;

        /// <summary>
        /// The (injected) logger
        /// </summary>
        private readonly ILogger<JwtTokenService> logger;

        /// <summary>
        /// Gets the configured <see cref="JwtBearerOptions"/>
        /// </summary>
        private readonly JwtBearerOptions bearerOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtTokenService" />
        /// </summary>
        /// <param name="logger">
        /// The (injected) logger
        /// </param>
        /// <param name="appConfigService">
        /// The (injected) <see cref="IAppConfigService" />
        /// </param>
        /// <param name="credentialsService">The (injected) <see cref="ICredentialsService" /></param>
        /// <param name="options">The (injected) <see cref="IOptionsMonitor{TOptions}"/> to retrieve <see cref="JwtBearerOptions"/> for the local authentication scheme</param>
        public JwtTokenService(ILogger<JwtTokenService> logger, IAppConfigService appConfigService, ICredentialsService credentialsService, IOptionsMonitor<JwtBearerOptions> options)
        {
            this.logger = logger;
            this.appConfigService = appConfigService;
            this.credentialsService = credentialsService;
            this.bearerOptions = options.Get(CometServer.Authentication.Bearer.JwtBearerDefaults.LocalAuthenticationScheme);
        }

        /// <summary>
        /// Generates <see cref="AuthenticationTokens" /> based on <see cref="AuthenticationPerson" />
        /// </summary>
        /// <param name="authenticationPerson">The used <see cref="AuthenticationPerson" /></param>
        /// <returns>The generated <see cref="AuthenticationTokens" /></returns>
        public AuthenticationTokens GenerateTokens(AuthenticationPerson authenticationPerson)
        {
            var issuedAtTime = DateTimeOffset.UtcNow;
            var accessExpirationTime = issuedAtTime.AddMinutes(this.appConfigService.AppConfig.AuthenticationConfig.LocalJwtAuthenticationConfig.TokenExpirationMinutes);
            var refreshExpirationTime = issuedAtTime.AddMinutes(this.appConfigService.AppConfig.AuthenticationConfig.LocalJwtAuthenticationConfig.RefreshExpirationMinutes);
            
            var commonClaims = CreateCommonClaims(authenticationPerson, issuedAtTime);
            var accessClaims = CreateAccessClaims(authenticationPerson, accessExpirationTime);
            var refreshClaims = CreateRefreshClaims(refreshExpirationTime);

            var signingCredentials = this.CreateSigningCredentials();

            var accessToken = this.CreateJwtSecurityToken([..commonClaims, ..accessClaims], signingCredentials);
            var refreshToken = this.CreateJwtSecurityToken([..commonClaims, ..refreshClaims], signingCredentials);

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            this.logger.LogInformation("JWT AuthenticationTokens created");
            return new AuthenticationTokens(jwtSecurityTokenHandler.WriteToken(accessToken), jwtSecurityTokenHandler.WriteToken(refreshToken));
        }

        /// <summary>
        /// Tries to generate <see cref="AuthenticationTokens" /> from a refresh token
        /// </summary>
        /// <param name="refreshToken">The refresh token that should be used</param>
        /// <returns>The generated <see cref="AuthenticationTokens"/></returns>
        public async Task<AuthenticationTokens> TryGenerateTokenFromRefreshToken(string refreshToken)
        {
            var tokenHandler = this.bearerOptions.TokenHandlers.Single();

            if (tokenHandler.ReadToken(refreshToken) is not JsonWebToken securityToken
                                                         || securityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Typ) is not { Value: RefreshClaimValue })
            {
                throw new ArgumentException("Invalid refresh token", nameof(refreshToken));
            }

            var tokenValidationResult = await tokenHandler.ValidateTokenAsync(securityToken, this.bearerOptions.TokenValidationParameters);

            if (!tokenValidationResult.IsValid)
            {
                throw tokenValidationResult.Exception;
            }

            var subClaim = securityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);

            if (subClaim == null || !Guid.TryParse(subClaim.Value, out var userId))
            {
                throw new ArgumentException("Invalid refresh token", nameof(refreshToken));
            }

            await using var connection = new NpgsqlConnection(Utils.GetConnectionString(this.appConfigService.AppConfig.Backtier, this.appConfigService.AppConfig.Backtier.Database));

            await connection.OpenAsync();

            await using var transaction = await connection.BeginTransactionAsync();

            await this.credentialsService.ResolveCredentials(transaction, userId);
            return this.GenerateTokens(this.credentialsService.Credentials.Person);
        }

        /// <summary>
        /// Creates a <see cref="JwtSecurityToken" />
        /// </summary>
        /// <param name="claims">
        /// The provided list of <see cref="Claim" />s
        /// </param>
        /// <param name="credentials">
        /// The <see cref="SigningCredentials" /> used to sing the token
        /// </param>
        /// <returns>The created <see cref="JwtSecurityToken" /></returns>
        private JwtSecurityToken CreateJwtSecurityToken(List<Claim> claims, SigningCredentials credentials)
        {
            var jwtSecurityToken = new JwtSecurityToken(
                this.appConfigService.AppConfig.AuthenticationConfig.LocalJwtAuthenticationConfig.ValidIssuer,
                this.appConfigService.AppConfig.AuthenticationConfig.LocalJwtAuthenticationConfig.ValidAudience,
                claims,
                signingCredentials: credentials
            );

            return jwtSecurityToken;
        }

        /// <summary>
        /// Creates an instance of <see cref="SigningCredentials" /> using the configured
        /// symmetric security key
        /// </summary>
        /// <returns>
        /// an instance of <see cref="SigningCredentials" />
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

        /// <summary>
        /// Creates <see cref="Claim" />s that are common to the access and refresh token
        /// </summary>
        /// <param name="authenticationPerson">
        /// The <see cref="AuthenticationPerson" /> used for the "sub" <see cref="Claim" />
        /// </param>
        /// <param name="issuedAtTime">The <see cref="DateTimeOffset" /> used for the "iat" <see cref="Claim" /></param>
        /// <returns>A collection of created <see cref="Claim" /></returns>
        private static List<Claim> CreateCommonClaims(AuthenticationPerson authenticationPerson, DateTimeOffset issuedAtTime)
        {
            return
            [
                new Claim(JwtRegisteredClaimNames.Sub, authenticationPerson.Iid.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, issuedAtTime.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            ];
        }

        /// <summary>
        /// Creates the authentication access claims for the <see cref="AuthenticationPerson" />
        /// </summary>
        /// <param name="authenticationPerson">The <see cref="AuthenticationPerson" /> to use for generate <see cref="Claim" />s</param>
        /// <param name="accessExpirationTime">The <see cref="DateTimeOffset"/> for the expiration time</param>
        /// <returns>
        /// A <see cref="List{T}" /> of <see cref="Claim" /> generated from the <see cref="AuthenticationPerson" />
        /// </returns>
        private static List<Claim> CreateAccessClaims(AuthenticationPerson authenticationPerson, DateTimeOffset accessExpirationTime)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Typ, AccessClaimValue),
                new (JwtRegisteredClaimNames.Exp, accessExpirationTime.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new(ClaimTypes.Name, authenticationPerson.UserName),
                new(ClaimTypes.NameIdentifier, authenticationPerson.Iid.ToString())
            };

            return claims;
        }

        /// <summary>
        /// Creates the authentication refresh claims
        /// </summary>
        /// <param name="refreshExpirationTime">The <see cref="DateTimeOffset"/> for the expiration time</param>
        /// <returns>A <see cref="List{T}" /> of <see cref="Claim" /></returns>
        private static List<Claim> CreateRefreshClaims(DateTimeOffset refreshExpirationTime)
        {
            return
            [
                new (JwtRegisteredClaimNames.Exp, refreshExpirationTime.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Typ, RefreshClaimValue)
            ];
        }
    }
}
