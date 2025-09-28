// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JwtBearerAuthenticationExtensionsTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The CDP4-COMET Webservices Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4-COMET Webservices Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Tests.Authentication.Bearer
{
    using CometServer.Authentication.Bearer;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="JwtBearerAuthenticationExtensions"/>.
    /// </summary>
    [TestFixture]
    public class JwtBearerAuthenticationExtensionsTestFixture
    {
        [Test]
        public void AddLocalJwtBearerAuthenticationRegistersScheme()
        {
            var services = new ServiceCollection();
            services.AddOptions();
            var builder = new AuthenticationBuilder(services);

            builder.AddLocalJwtBearerAuthentication(configure: options => options.Audience = "localAudience");

            var provider = services.BuildServiceProvider();
            var optionsMonitor = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>();

            var options = optionsMonitor.Get(JwtBearerDefaults.LocalAuthenticationScheme);
            Assert.That(options.Audience, Is.EqualTo("localAudience"));
        }

        [Test]
        public void AddExternalJwtBearerAuthenticationRegistersScheme()
        {
            var services = new ServiceCollection();
            services.AddOptions();
            var builder = new AuthenticationBuilder(services);

            builder.AddExternalJwtBearerAuthentication(configure: options => options.Audience = "externalAudience");

            var provider = services.BuildServiceProvider();
            var optionsMonitor = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>();

            var options = optionsMonitor.Get(JwtBearerDefaults.ExternalAuthenticationScheme);
            Assert.That(options.Audience, Is.EqualTo("externalAudience"));
        }
    }
}
