// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationPluginInjectorTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Authorization
{
    using CometServer.Authentication;

    using Microsoft.Extensions.Logging;

    using Moq;

    using NUnit.Framework;
    using System.IO;

    /// <summary>
    /// Test fixture for the <see cref="AuthenticationPluginInjector"/> class
    /// </summary>
    [TestFixture]
    public class AuthenticationPluginInjectorTestFixture
    {
        private Mock<ILogger<AuthenticationPluginInjector>> logger;

        [SetUp]
        public void SetUp()
        {
            this.logger = new Mock<ILogger<AuthenticationPluginInjector>>();

            // create Authentication directory for testing purposes 
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "Authentication");
            Directory.CreateDirectory(path);
        }

        [Test]
        public void Verify_that_the_AuthenticationPluginInjector_can_be_constructed()
        {
            var authenticationPluginInjector = new AuthenticationPluginInjector(this.logger.Object);

            Assert.That(authenticationPluginInjector.Plugins, Is.Empty);

            Assert.That(authenticationPluginInjector.Connectors, Is.Empty);
        }
    }
}
