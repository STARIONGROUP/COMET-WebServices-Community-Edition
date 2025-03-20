// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JwtBearerAuthenticationHandlerTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Authentication.Bearer
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using Carter;

    using CDP4Authentication;

    using CometServer.Authentication;
    using CometServer.Authentication.Bearer;
    using CometServer.Authorization;
    using CometServer.Configuration;
    using CometServer.Health;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Primitives;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Net.Http.Headers;

    using Moq;

    using NUnit.Framework;

    using JwtBearerDefaults = CometServer.Authentication.Bearer.JwtBearerDefaults;

    [TestFixture]
    public class JwtBearerAuthenticationHandlerTestFixture
    {
        private JwtBearerAuthenticationHandler handler;
        private Mock<IOptionsMonitor<JwtBearerOptions>> optionsMonitor;
        private Mock<ILoggerFactory> loggerFactory;
        private Mock<UrlEncoder> encoder;
        private Mock<ISystemClock> systemClock;
        private Mock<ICometHasStartedService> cometHasStartedService;
        private Mock<IAppConfigService> appConfigService;
        private Mock<IResponseNegotiator> jsonResponseNegotiator;
        private Mock<IServiceProvider> requestService;
        private Mock<ICredentialsService> credentialsService;
        private JwtTokenService jwtTokenService;
        private const string key = "68D58F267F5FFEC6B63A5907B3D96B80F101DA5AE167418FCFF416EB123C896B";

        [SetUp]
        public void Setup()
        {
            var appConfig = new AppConfig();
            appConfig.AuthenticationConfig.LocalJwtAuthenticationConfig.SymmetricSecurityKey = key;
            
            this.optionsMonitor = new Mock<IOptionsMonitor<JwtBearerOptions>>();
            
            this.optionsMonitor
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns(new JwtBearerOptions()
                {
                    TokenValidationParameters =
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                        ValidateIssuer = true,
                        ValidIssuer =  appConfig.AuthenticationConfig.LocalJwtAuthenticationConfig.ValidIssuer,
                        ValidateAudience = true,
                        ValidAudience = appConfig.AuthenticationConfig.LocalJwtAuthenticationConfig.ValidAudience     
                    }
                });
            
            this.loggerFactory = new Mock<ILoggerFactory>();
            var logger = new Mock<ILogger<JwtBearerAuthenticationHandler>>();
            this.loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            this.encoder = new Mock<UrlEncoder>();
            this.systemClock = new Mock<ISystemClock>();
            this.cometHasStartedService = new Mock<ICometHasStartedService>();
            this.appConfigService = new Mock<IAppConfigService>();
            this.jsonResponseNegotiator = new Mock<IResponseNegotiator>();
            this.jsonResponseNegotiator.Setup(x => x.CanHandle(new MediaTypeHeaderValue("application/json"))).Returns(true);
            this.requestService = new Mock<IServiceProvider>();
            this.requestService.Setup(x => x.GetService(typeof(IEnumerable<IResponseNegotiator>))).Returns(new List<IResponseNegotiator>{this.jsonResponseNegotiator.Object});
            
            this.handler = new JwtBearerAuthenticationHandler(this.optionsMonitor.Object, this.loggerFactory.Object, this.encoder.Object, this.systemClock.Object, 
                 this.cometHasStartedService.Object,this.appConfigService.Object);

            this.credentialsService = new Mock<ICredentialsService>();
            this.appConfigService.Setup( x=> x.AppConfig).Returns(appConfig);
            this.jwtTokenService = new JwtTokenService(new NullLogger<JwtTokenService>(), this.appConfigService.Object, this.credentialsService.Object, this.optionsMonitor.Object);
        }
        
        [Test]
        public async Task VerifyAuthenticationFailsWhenNotStarted()
        {
            var context = new DefaultHttpContext
            {
                RequestServices = this.requestService.Object
            };

            context.Request.Headers.Authorization = new StringValues(JwtBearerDefaults.LocalAuthenticationScheme);
            
            this.cometHasStartedService.Setup(x => x.GetHasStartedAndIsReady()).Returns(new ServerStatus(false, DateTime.Now));

            await this.handler.InitializeAsync(new AuthenticationScheme(JwtBearerDefaults.LocalAuthenticationScheme, "", typeof(JwtBearerAuthenticationHandler)), context);
            var result = await this.handler.AuthenticateAsync();

            Assert.That(result.Succeeded, Is.False);

            await this.handler.ChallengeAsync(result.Properties);

            Assert.That(context.Response.StatusCode, Is.EqualTo(503));
        }

        [Test]
        public async Task VerifyAuthenticationFailsBasedOnConfig()
        {
            var context = new DefaultHttpContext
            {
                RequestServices = this.requestService.Object
            };
            
            context.Request.Headers.Authorization = new StringValues(JwtBearerDefaults.LocalAuthenticationScheme);

            this.cometHasStartedService.Setup(x => x.GetHasStartedAndIsReady()).Returns(new ServerStatus(true, DateTime.Now));
            await this.handler.InitializeAsync(new AuthenticationScheme(JwtBearerDefaults.LocalAuthenticationScheme, "", typeof(JwtBearerAuthenticationHandler)), context);
            
            this.appConfigService.Setup(x => x.IsAuthenticationSchemeEnabled(this.handler.Scheme.Name)).Returns(false);
            
            var result = await this.handler.AuthenticateAsync();

            Assert.That(result.Succeeded, Is.False);

            await this.handler.ChallengeAsync(result.Properties);

            Assert.That(context.Response.StatusCode, Is.EqualTo(403));
        }
        
        [Test]
        public async Task VerifyAuthenticationNotSucceedNorFailedWithInvalidHeader()
        {
            var context = new DefaultHttpContext
            {
                RequestServices = this.requestService.Object
            }; 
            
            context.Request.Headers.Authorization = new StringValues("Invalid");
            
            this.cometHasStartedService.Setup(x => x.GetHasStartedAndIsReady()).Returns(new ServerStatus(true, DateTime.Now));
            await this.handler.InitializeAsync(new AuthenticationScheme(JwtBearerDefaults.LocalAuthenticationScheme, "", typeof(JwtBearerAuthenticationHandler)), context);
            
            this.appConfigService.Setup(x => x.IsAuthenticationSchemeEnabled(this.handler.Scheme.Name)).Returns(true);
            var result = await this.handler.AuthenticateAsync();

            Assert.That(result.Succeeded, Is.False);

            await this.handler.ChallengeAsync(result.Properties);

            Assert.That(context.Response.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task VerifyAuthenticationFailsWithInvalidToken()
        {
            var context = new DefaultHttpContext
            {
                RequestServices = this.requestService.Object
            }; 
            
            context.Request.Headers.Authorization = new StringValues($"{JwtBearerDefaults.LocalAuthenticationScheme} InvalidToken");
            
            this.cometHasStartedService.Setup(x => x.GetHasStartedAndIsReady()).Returns(new ServerStatus(true, DateTime.Now));
            await this.handler.InitializeAsync(new AuthenticationScheme(JwtBearerDefaults.LocalAuthenticationScheme, "", typeof(JwtBearerAuthenticationHandler)), context);
            
            this.appConfigService.Setup(x => x.IsAuthenticationSchemeEnabled(this.handler.Scheme.Name)).Returns(true);
            var result = await this.handler.AuthenticateAsync();

            Assert.That(result.Failure, Is.Not.Null);

            await this.handler.ChallengeAsync(result.Properties);

            Assert.That(context.Response.StatusCode, Is.EqualTo(401));
        }
        
        [Test]
        public async Task VerifyAuthenticationSuccessWithValidToken()
        {
            var context = new DefaultHttpContext
            {
                RequestServices = this.requestService.Object
            };

            var token = this.jwtTokenService.GenerateTokens(new AuthenticationPerson(Guid.NewGuid(), 1)
            {
                UserName = "admin"
            });
            
            context.Request.Headers.Authorization = new StringValues($"{JwtBearerDefaults.LocalAuthenticationScheme} {token.AccessToken}");
            
            this.cometHasStartedService.Setup(x => x.GetHasStartedAndIsReady()).Returns(new ServerStatus(true, DateTime.Now));
            await this.handler.InitializeAsync(new AuthenticationScheme(JwtBearerDefaults.LocalAuthenticationScheme, "", typeof(JwtBearerAuthenticationHandler)), context);
            
            this.appConfigService.Setup(x => x.IsAuthenticationSchemeEnabled(this.handler.Scheme.Name)).Returns(true);
            var result = await this.handler.AuthenticateAsync();

            Assert.That(result.Succeeded, Is.True);
        }
        
        [Test]
        public async Task VerifyAuthenticationFailsWithEmptyHeader()
        {
            var context = new DefaultHttpContext
            {
                RequestServices = this.requestService.Object
            }; 
            
            this.cometHasStartedService.Setup(x => x.GetHasStartedAndIsReady()).Returns(new ServerStatus(true, DateTime.Now));
            await this.handler.InitializeAsync(new AuthenticationScheme(JwtBearerDefaults.LocalAuthenticationScheme, "", typeof(JwtBearerAuthenticationHandler)), context);
            
            this.appConfigService.Setup(x => x.IsAuthenticationSchemeEnabled(this.handler.Scheme.Name)).Returns(true);
            var result = await this.handler.AuthenticateAsync();

            Assert.That(result.Succeeded, Is.False);

            await this.handler.ChallengeAsync(result.Properties);

            Assert.That(context.Response.StatusCode, Is.EqualTo(401));
        }
    }
}
