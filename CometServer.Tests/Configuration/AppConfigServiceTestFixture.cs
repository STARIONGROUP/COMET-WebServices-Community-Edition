// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppConfigServiceTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

namespace CometServer.Tests.Configuration
{
    using System.IO;

    using CometServer.Configuration;

    using Microsoft.Extensions.Configuration;

    using NUnit.Framework;
    
    /// <summary>
    /// Suite of tests for the <see cref="AppConfigService"/>
    /// </summary>
    [TestFixture]
    public class AppConfigServiceTestFixture
    {
        private IConfiguration configuration;

        [SetUp]
        public void SetUp()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Data", "appsettings.json");

            // Initialize the configuration builder
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path, optional: false, reloadOnChange: true);

            // Build the IConfiguration instance
            this.configuration = configurationBuilder.Build();
        }

        [Test]
        public void Verify_that_configuration_is_loaded_from_appsettings()
        {
            var appConfigService = new AppConfigService(this.configuration);
            
            Assert.Multiple(() =>
            {
                Assert.That(appConfigService.AppConfig.Backtier.Database, Is.EqualTo("cdp4server"));

                Assert.That(appConfigService.AppConfig.Changelog.CollectChanges, Is.True);

                Assert.That(appConfigService.AppConfig.EmailService.SMTP, Is.EqualTo("smtp.cdp4.org"));

                Assert.That(appConfigService.AppConfig.HealthConfig.RequireHost, Is.Empty);

                Assert.That(appConfigService.AppConfig.Midtier.BacktierWaitTime, Is.EqualTo(333));

                Assert.That(appConfigService.AppConfig.ServiceMessagingConfig.IsEnabled, Is.False);
                Assert.That(appConfigService.AppConfig.ServiceMessagingConfig.Port, Is.EqualTo(1234));
                Assert.That(appConfigService.AppConfig.ServiceMessagingConfig.HostName, Is.EqualTo("message-broker"));
                Assert.That(appConfigService.AppConfig.ServiceMessagingConfig.TimeSpanBetweenAttempts, Is.EqualTo(5));
                
                Assert.That(appConfigService.AppConfig.AuthenticationConfig.BasicAuthenticationConfig.IsEnabled, Is.True);
                Assert.That(appConfigService.AppConfig.AuthenticationConfig.LocalJwtAuthenticationConfig.IsEnabled, Is.True);
                Assert.That(appConfigService.AppConfig.AuthenticationConfig.ExternalJwtAuthenticationConfig.IsEnabled, Is.True);
            });
        }
    }
}
