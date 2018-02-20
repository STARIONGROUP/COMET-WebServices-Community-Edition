namespace CDP4WebServices.API.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using API.Helpers;
    using API.Services;
    using API.Services.Protocol;
    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.MetaInfo;
    using Moq;
    using Nancy;
    using NUnit.Framework;
    using Thing = CDP4Common.DTO.Thing;

    [TestFixture]
    internal class HttpRequestHelperTestFixture
    {
        [Test]
        public void VeriyThatValidateworks1()
        {
            var request = new Request("blabla", "blabla", "blabla");

            request.Query["revisionNumber"] = 1;

            var requestUtil = new Mock<IRequestUtils>();
            requestUtil.SetupSet(x => x.QueryParameters = It.IsAny<IQueryParameters>());

            Assert.DoesNotThrow(() => HttpRequestHelper.ValidateSupportedQueryParameter(request, requestUtil.Object, new[] { QueryParameters.RevisionNumberQuery }));
            requestUtil.VerifySet(x => x.QueryParameters = It.IsAny<IQueryParameters>());
        }

        [Test]
        public void VeriyThatValidateworks2()
        {
            var request = new Request("blabla", "blabla", "blabla");

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
            var request = new Request("blabla", "blabla", "blabla");

            request.Query["revisionFrom"] = 1;
            request.Query["revisionTo"] = 2;

            var requestUtil = new Mock<IRequestUtils>();
            requestUtil.SetupSet(x => x.QueryParameters = It.IsAny<IQueryParameters>());

            Assert.Throws<InvalidOperationException>(() => HttpRequestHelper.ValidateSupportedQueryParameter(request, requestUtil.Object, new string[0]));
        }

        [Test]
        public void VeriyThatValidateworks4()
        {
            var request = new Request("blabla", "blabla", "blabla");

            request.Query["revisionFrom"] = 1;
            request.Query["revisionNumber"] = 2;

            var requestUtil = new Mock<IRequestUtils>();
            requestUtil.SetupSet(x => x.QueryParameters = It.IsAny<IQueryParameters>());

            Assert.Throws<InvalidOperationException>(() => HttpRequestHelper.ValidateSupportedQueryParameter(request, requestUtil.Object, new[] { QueryParameters.RevisionNumberQuery, QueryParameters.RevisionFromQuery, QueryParameters.RevisionToQuery }));
        }

        [Test]
        public void VeriyThatValidateworks5()
        {
            var request = new Request("blabla", "blabla", "blabla");

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