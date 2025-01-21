// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BasicAuthenticationHandlerTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Authentication.Basic
{
    using System;
    using System.Collections.Generic;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using Carter;

    using CDP4Authentication;

    using CometServer.Authentication;
    using CometServer.Authentication.Anonymous;
    using CometServer.Authentication.Basic;
    using CometServer.Configuration;
    using CometServer.Health;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Primitives;
    using Microsoft.Net.Http.Headers;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class BasicAuthenticationHandlerTestFixture
    {
        private BasicAuthenticationHandler handler;
        private Mock<IOptionsMonitor<BasicAuthenticationOptions>> optionsMonitor;
        private Mock<ILoggerFactory> loggerFactory;
        private Mock<UrlEncoder> encoder;
        private Mock<ISystemClock> systemClock;
        private Mock<IAuthenticationPersonAuthenticator> authenticatonPersonAuthenticator;
        private Mock<ICometHasStartedService> cometHasStartedService;
        private Mock<IAppConfigService> appConfigService;
        private Mock<IResponseNegotiator> jsonResponseNegotiator;
        private Mock<IServiceProvider> requestService;
        private Mock<IAuthenticationService> authenticationService;
        
        [SetUp]
        public void Setup()
        {
            this.optionsMonitor = new Mock<IOptionsMonitor<BasicAuthenticationOptions>>();
            
            this.optionsMonitor
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns(new BasicAuthenticationOptions());
            
            this.loggerFactory = new Mock<ILoggerFactory>();
            var logger = new Mock<ILogger<BasicAuthenticationHandler>>();
            this.loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            this.encoder = new Mock<UrlEncoder>();
            this.systemClock = new Mock<ISystemClock>();
            this.authenticatonPersonAuthenticator = new Mock<IAuthenticationPersonAuthenticator>();
            this.cometHasStartedService = new Mock<ICometHasStartedService>();
            this.appConfigService = new Mock<IAppConfigService>();
            this.jsonResponseNegotiator = new Mock<IResponseNegotiator>();
            this.jsonResponseNegotiator.Setup(x => x.CanHandle(new MediaTypeHeaderValue("application/json"))).Returns(true);
            this.requestService = new Mock<IServiceProvider>();
            this.requestService.Setup(x => x.GetService(typeof(IEnumerable<IResponseNegotiator>))).Returns(new List<IResponseNegotiator>{this.jsonResponseNegotiator.Object});
            this.authenticationService = new Mock<IAuthenticationService>();
            
            this.requestService.Setup(x => x.GetService(typeof(IAuthenticationService))).Returns(this.authenticationService.Object);
            
            this.handler = new BasicAuthenticationHandler(this.optionsMonitor.Object, this.loggerFactory.Object, this.encoder.Object, this.systemClock.Object, 
                this.authenticatonPersonAuthenticator.Object, this.cometHasStartedService.Object,this.appConfigService.Object);
        }

        [Test]
        public async Task VerifyAuthenticationFailsWhenNotStarted()
        {
            var context = new DefaultHttpContext
            {
                RequestServices = this.requestService.Object
            };

            context.Request.Headers.Authorization = new StringValues("Basic");
            
            this.cometHasStartedService.Setup(x => x.GetHasStartedAndIsReady()).Returns(new ServerStatus(false, DateTime.Now));

            await this.handler.InitializeAsync(new AuthenticationScheme(BasicAuthenticationDefaults.AuthenticationScheme, BasicAuthenticationDefaults.DisplayName, typeof(BasicAuthenticationHandler)), context);
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
            
            context.Request.Headers.Authorization = new StringValues("Basic");

            this.cometHasStartedService.Setup(x => x.GetHasStartedAndIsReady()).Returns(new ServerStatus(true, DateTime.Now));
            await this.handler.InitializeAsync(new AuthenticationScheme(BasicAuthenticationDefaults.AuthenticationScheme, BasicAuthenticationDefaults.DisplayName, typeof(BasicAuthenticationHandler)), context);
            
            this.appConfigService.Setup(x => x.IsAuthenticationSchemeEnabled(this.handler.Scheme.Name)).Returns(false);
            
            var result = await this.handler.AuthenticateAsync();

            Assert.That(result.Succeeded, Is.False);

            await this.handler.ChallengeAsync(result.Properties);

            Assert.That(context.Response.StatusCode, Is.EqualTo(403));
        }

        [Test]
        public async Task VerifyAuthenticationNotSucceedNorFailedWithEmptyHeader()
        {
            var context = new DefaultHttpContext
            {
                RequestServices = this.requestService.Object
            }; 
            
            this.cometHasStartedService.Setup(x => x.GetHasStartedAndIsReady()).Returns(new ServerStatus(true, DateTime.Now));
            await this.handler.InitializeAsync(new AuthenticationScheme(BasicAuthenticationDefaults.AuthenticationScheme, BasicAuthenticationDefaults.DisplayName, typeof(BasicAuthenticationHandler)), context);
            
            this.appConfigService.Setup(x => x.IsAuthenticationSchemeEnabled(this.handler.Scheme.Name)).Returns(true);
            var result = await this.handler.AuthenticateAsync();

            Assert.That(result.Succeeded, Is.False);

            await this.handler.ChallengeAsync(result.Properties);

            Assert.That(context.Response.StatusCode, Is.EqualTo(200));
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
            await this.handler.InitializeAsync(new AuthenticationScheme(BasicAuthenticationDefaults.AuthenticationScheme, BasicAuthenticationDefaults.DisplayName, typeof(BasicAuthenticationHandler)), context);
            
            this.appConfigService.Setup(x => x.IsAuthenticationSchemeEnabled(this.handler.Scheme.Name)).Returns(true);
            var result = await this.handler.AuthenticateAsync();

            Assert.That(result.Succeeded, Is.False);

            await this.handler.ChallengeAsync(result.Properties);

            Assert.That(context.Response.StatusCode, Is.EqualTo(200));
        }
        
        [Test]
        public async Task VerifyAuthenticationFailsWithInvalidCredentialsFormat()
        {
            var context = new DefaultHttpContext
            {
                RequestServices = this.requestService.Object
            };

            const string credentials = "user";
            
            context.Request.Headers.Authorization = new StringValues($"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(credentials))}");
            
            this.cometHasStartedService.Setup(x => x.GetHasStartedAndIsReady()).Returns(new ServerStatus(true, DateTime.Now));
            await this.handler.InitializeAsync(new AuthenticationScheme(BasicAuthenticationDefaults.AuthenticationScheme, BasicAuthenticationDefaults.DisplayName, typeof(BasicAuthenticationHandler)), context);
            
            this.appConfigService.Setup(x => x.IsAuthenticationSchemeEnabled(this.handler.Scheme.Name)).Returns(true);
            var result = await this.handler.AuthenticateAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result.Failure, Is.Not.Null);
                Assert.That(result.Failure.Message, Is.EqualTo("Invalid Basic authentication header format."));
            });

            await this.handler.ChallengeAsync(result.Properties);

            Assert.That(context.Response.StatusCode, Is.EqualTo(401));
        }
        
        [Test]
        public async Task VerifyAuthenticationFailsdWithInvalidCredential()
        {
            var context = new DefaultHttpContext
            {
                RequestServices = this.requestService.Object
            };

            const string username ="user";
            const string password = "password";
            
            context.Request.Headers.Authorization = new StringValues($"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{username}:{password}"))}");
            
            this.cometHasStartedService.Setup(x => x.GetHasStartedAndIsReady()).Returns(new ServerStatus(true, DateTime.Now));
            await this.handler.InitializeAsync(new AuthenticationScheme(BasicAuthenticationDefaults.AuthenticationScheme, BasicAuthenticationDefaults.DisplayName, typeof(BasicAuthenticationHandler)), context);
            
            this.appConfigService.Setup(x => x.IsAuthenticationSchemeEnabled(this.handler.Scheme.Name)).Returns(true);
            this.authenticatonPersonAuthenticator.Setup(x => x.Authenticate(username, password)).ReturnsAsync((AuthenticationPerson)null);
            var result = await this.handler.AuthenticateAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result.Failure, Is.Not.Null);
                Assert.That(result.Failure.Message, Is.EqualTo("Invalid username or password."));
            });

            await this.handler.ChallengeAsync(result.Properties);

            Assert.That(context.Response.StatusCode, Is.EqualTo(401));
        }
        
        [Test]
        public async Task VerifyAuthenticationSuccessWithValidCredentials()
        {
            var context = new DefaultHttpContext
            {
                RequestServices = this.requestService.Object
            };

            const string username ="user";
            const string password = "password";
            
            context.Request.Headers.Authorization = new StringValues($"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{username}:{password}"))}");
            
            this.cometHasStartedService.Setup(x => x.GetHasStartedAndIsReady()).Returns(new ServerStatus(true, DateTime.Now));
            await this.handler.InitializeAsync(new AuthenticationScheme(BasicAuthenticationDefaults.AuthenticationScheme, BasicAuthenticationDefaults.DisplayName, typeof(BasicAuthenticationHandler)), context);
            
            this.appConfigService.Setup(x => x.IsAuthenticationSchemeEnabled(this.handler.Scheme.Name)).Returns(true);
            this.authenticatonPersonAuthenticator.Setup(x => x.Authenticate(username, password)).ReturnsAsync(new AuthenticationPerson(Guid.NewGuid(), 1));
            var result = await this.handler.AuthenticateAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(result.Principal!.Identity!.Name, Is.EqualTo(username));
            });
        }
    }
}
