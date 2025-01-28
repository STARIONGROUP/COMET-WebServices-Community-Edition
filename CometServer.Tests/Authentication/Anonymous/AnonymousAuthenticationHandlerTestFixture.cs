// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnonymousAuthenticationHandlerTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Authentication.Anonymous
{
    using System.Linq;
    using System.Security.Claims;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using CometServer.Authentication.Anonymous;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using Moq;

    using NUnit.Framework;

    [TestFixture]
    public class AnonymousAuthenticationHandlerTestFixture
    {
        public AnonymousAuthenticationHandler handler;
        public Mock<IOptionsMonitor<AnonymousAuthenticationOptions>> optionsMonitor;
        public Mock<ILoggerFactory> loggerFactory;
        public Mock<UrlEncoder> encoder;
        public Mock<ISystemClock> systemClock;
        
        [SetUp]
        public void Setup()
        {
            this.optionsMonitor = new Mock<IOptionsMonitor<AnonymousAuthenticationOptions>>();
            
            this.optionsMonitor
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns(new AnonymousAuthenticationOptions());
            
            this.loggerFactory = new Mock<ILoggerFactory>();
            var logger = new Mock<ILogger<AnonymousAuthenticationHandler>>();
            this.loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

            this.encoder = new Mock<UrlEncoder>();
            this.systemClock = new Mock<ISystemClock>();
            this.handler = new AnonymousAuthenticationHandler(this.optionsMonitor.Object, this.loggerFactory.Object, this.encoder.Object, this.systemClock.Object);
        }

        [Test]
        public async Task VerifyHandleAnonymousAuthentication()
        {
            var context = new DefaultHttpContext();

            await this.handler.InitializeAsync(new AuthenticationScheme(AnonymousAuthenticationDefaults.AuthenticationScheme, AnonymousAuthenticationDefaults.DisplayName,
                typeof(AnonymousAuthenticationHandler)), context);
            
            var result = await this.handler.AuthenticateAsync();

            Assert.Multiple(() =>
            {
                Assert.That(result.Succeeded, Is.True);
                Assert.That(result.Principal, Is.Not.Null);
                Assert.That(result.Principal.Claims.Single(x => x.Type == ClaimTypes.Name).Value, Is.EqualTo("anonymous"));
            });
        }
    }
}
