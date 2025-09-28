// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestHelperTestFixture.cs" company="Starion Group S.A.">
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

namespace CometServer.Tests.Helpers
{
    using System;
    using System.Collections.Generic;

    using CometServer.Exceptions;
    using CometServer.Helpers;
    using CometServer.Services.Protocol;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;

    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="HttpRequestHelper"/>.
    /// </summary>
    [TestFixture]
    public class HttpRequestHelperTestFixture
    {
        [Test]
        public void ValidateSupportedQueryParameterThrowsForUnsupportedKey()
        {
            var query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { "unexpected", "value" }
            });

            Assert.Throws<BadRequestException>(() => HttpRequestHelper.ValidateSupportedQueryParameter(query, new[] { "expected" }));
        }

        [Test]
        public void ValidateSupportedQueryParameterValidatesRevisionPairs()
        {
            var query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { QueryParameters.RevisionFromQuery, "1" },
                { "other", "value" }
            });

            Assert.Throws<BadRequestException>(() => HttpRequestHelper.ValidateSupportedQueryParameter(query, new[] { QueryParameters.RevisionFromQuery, QueryParameters.RevisionToQuery, "other" }));
        }

        [Test]
        public void ValidateSupportedQueryParameterAcceptsValidInput()
        {
            var query = new QueryCollection(new Dictionary<string, StringValues>
            {
                { QueryParameters.RevisionFromQuery, "1" },
                { QueryParameters.RevisionToQuery, "2" }
            });

            Assert.DoesNotThrow(() => HttpRequestHelper.ValidateSupportedQueryParameter(query, new[] { QueryParameters.RevisionFromQuery, QueryParameters.RevisionToQuery }));
        }

        [Test]
        public void ParseRouteSegmentsSplitsPath()
        {
            var segments = HttpRequestHelper.ParseRouteSegments("/api/v1/thing");

            Assert.That(segments, Is.EqualTo(new[] { "api", "v1", "thing" }));
        }
    }
}
