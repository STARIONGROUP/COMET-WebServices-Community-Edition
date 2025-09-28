// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderInfoProviderTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Services.Headers
{
    using CometServer.Services;

    using Microsoft.AspNetCore.Http;

    using NUnit.Framework;

    /// <summary>
    /// Unit tests for header helpers.
    /// </summary>
    [TestFixture]
    public class HeaderInfoProviderTestFixture
    {
        [Test]
        public void RegisterResponseHeadersAddsExpectedValues()
        {
            var provider = new HeaderInfoProvider();
            var context = new DefaultHttpContext();
            var boundary = "myBoundary";

            provider.RegisterResponseHeaders(context.Response, ContentTypeKind.MULTIPARTMIXED, boundary);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(context.Response.Headers[HttpConstants.Cdp4ServerHeader].ToString(), Does.Contain("."));
                Assert.That(context.Response.Headers[HttpConstants.CometServerHeader].ToString(), Does.Contain("."));
                Assert.That(context.Response.Headers[HttpConstants.Cdp4CommonHeader].ToString(), Does.Contain("."));
                Assert.That(context.Response.Headers[HttpConstants.ContentTypeHeader].ToString(), Does.Contain(boundary));
            }
        }

        [Test]
        public void HttpConstantsExposeExpectedValues()
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(HttpConstants.BoundaryString, Is.EqualTo("----Boundary"));
                Assert.That(HttpConstants.MimeTypeJson, Is.EqualTo("application/json"));
                Assert.That(HttpConstants.MimeTypeMessagePack, Is.EqualTo("application/msgpack"));
                Assert.That(HttpConstants.MimeTypeOctetStream, Is.EqualTo("application/octet-stream"));
                Assert.That(HttpConstants.ContentTypeHeader, Is.EqualTo("Content-Type"));
                Assert.That(HttpConstants.ContentDispositionHeader, Is.EqualTo("Content-Disposition"));
                Assert.That(HttpConstants.ContentLengthHeader, Is.EqualTo("Content-Length"));
                Assert.That(HttpConstants.Cdp4CommonHeader, Is.EqualTo("CDP4-Common"));
                Assert.That(HttpConstants.Cdp4ServerHeader, Is.EqualTo("CDP4-Server"));
                Assert.That(HttpConstants.CometServerHeader, Is.EqualTo("COMET-Server"));
                Assert.That(HttpConstants.AcceptCdpVersionHeader, Is.EqualTo("Accept-CDP"));
                Assert.That(HttpConstants.CDPErrorTag, Is.EqualTo("CDP-Error-Tag"));
                Assert.That(HttpConstants.DefaultDataModelVersion, Is.EqualTo("1.0.0"));
            }
        }
    }
}
