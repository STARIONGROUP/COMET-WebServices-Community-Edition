// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpResponseExtensionsTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Modules
{
    using CometServer.Modules;

    using Microsoft.AspNetCore.Http;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="HttpResponseExtensions"/>.
    /// </summary>
    [TestFixture]
    public class HttpResponseExtensionsTestFixture
    {
        [Test]
        public void UpdateWithNotAuthenticatedSettingsAppliesExpectedValues()
        {
            var response = new DefaultHttpContext().Response;

            response.UpdateWithNotAuthenticatedSettings();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.ContentType, Is.EqualTo("application/json"));
                Assert.That(response.StatusCode, Is.EqualTo(401));
                Assert.That(response.Headers["WWW-Authenticate"].ToString(), Is.EqualTo("Basic"));
            }
        }

        [Test]
        public void UpdateWithNotBearerAuthenticatedSettingsAppliesExpectedValues()
        {
            var response = new DefaultHttpContext().Response;

            response.UpdateWithNotBearerAuthenticatedSettings();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.ContentType, Is.EqualTo("application/json"));
                Assert.That(response.StatusCode, Is.EqualTo(401));
                Assert.That(response.Headers["WWW-Authenticate"].ToString(), Is.EqualTo("Bearer"));
            }
        }

        [Test]
        public void UpdateWithNotAuthorizedSettingsSetsStatusCode()
        {
            var response = new DefaultHttpContext().Response;

            response.UpdateWithNotAutherizedSettings();

            Assert.That(response.StatusCode, Is.EqualTo(403));
        }
    }
}
