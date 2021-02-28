// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeLogTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
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

    using Castle.Components.DictionaryAdapter;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.MetaInfo;

    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Protocol;

    using Microsoft.AspNetCore.Http;

    using Moq;

    using NUnit.Framework;

    using Thing = CDP4Common.DTO.Thing;

    [TestFixture]
    internal class HttpRequestHelperTestFixture
    {
        [Test]
        public void VeriyThatValidateworks1()
        {
            var request = new HttpRequest("blabla", "blabla", "blabla");

            request.Query["revisionNumber"] = 1;

            var requestUtil = new Mock<IRequestUtils>();
            requestUtil.SetupSet(x => x.QueryParameters = It.IsAny<IQueryParameters>());

            Assert.DoesNotThrow(() => HttpRequestHelper.ValidateSupportedQueryParameter(request, requestUtil.Object, new[] { QueryParameters.RevisionNumberQuery }));
            requestUtil.VerifySet(x => x.QueryParameters = It.IsAny<IQueryParameters>());
        }

        [Test]
        public void VeriyThatValidateworks2()
        {
            var request = new HttpRequest("blabla", "blabla", "blabla");

            request.Query["revisionFrom"] = 1;
            request.Query["revisionTo"] = 2;

            var requestUtil = new Mock<IRequestUtils>();
            requestUtil.SetupSet(x => x.QueryParameters = It.IsAny<IQueryParameters>());

            Assert.DoesNotThrow(() => HttpRequestHelper.ValidateSupportedQueryParameter(request, requestUtil.Object, new[] { QueryParameters.RevisionFromQuery, QueryParameters.RevisionToQuery }));
            requestUtil.VerifySet(x => x.QueryParameters = It.IsAny<IQueryParameters>());
        }

        [Test]
        public void VeriyThatValidateworks3()
        {
            var request = new HttpRequest("blabla", "blabla", "blabla");

            request.Query["revisionFrom"] = 1;
            request.Query["revisionTo"] = 2;

            var requestUtil = new Mock<IRequestUtils>();
            requestUtil.SetupSet(x => x.QueryParameters = It.IsAny<IQueryParameters>());

            Assert.Throws<InvalidOperationException>(() => HttpRequestHelper.ValidateSupportedQueryParameter(request, requestUtil.Object, new string[0]));
        }

        [Test]
        public void VeriyThatValidateworks4()
        {
            var request = new HttpRequest("blabla", "blabla", "blabla");

            request.Query["revisionFrom"] = 1;
            request.Query["revisionNumber"] = 2;

            var requestUtil = new Mock<IRequestUtils>();
            requestUtil.SetupSet(x => x.QueryParameters = It.IsAny<IQueryParameters>());

            Assert.Throws<InvalidOperationException>(() => HttpRequestHelper.ValidateSupportedQueryParameter(request, requestUtil.Object, new[] { QueryParameters.RevisionNumberQuery, QueryParameters.RevisionFromQuery, QueryParameters.RevisionToQuery }));
        }

        [Test]
        public void VeriyThatValidateworks5()
        {
            var request = new HttpRequest("blabla", "blabla", "blabla");

            request.Query["revisionNumber"] = 1;
            request.Query["revisionTo"] = 2;

            var requestUtil = new Mock<IRequestUtils>();
            requestUtil.SetupSet(x => x.QueryParameters = It.IsAny<IQueryParameters>());

            Assert.Throws<InvalidOperationException>(() => HttpRequestHelper.ValidateSupportedQueryParameter(request, requestUtil.Object, new[] { QueryParameters.RevisionNumberQuery, QueryParameters.RevisionFromQuery, QueryParameters.RevisionToQuery }));
        }

        [Test]
        public void VerifyParser()
        {
            dynamic routeParams = new DynamicDictionary();
            routeParams["uri"] = "1/2/3";
            var routessegments = (string[])HttpRequestHelper.ParseRouteSegments(routeParams, "Sitedirectory");
            Assert.AreEqual(4, routessegments.Length);
        }
    }
}