namespace CDP4WebServices.API.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

            Assert.DoesNotThrow(() => HttpRequestHelper.ValidateSupportedQueryParameter(request, requestUtil.Object, new [] {QueryParameters.RevisionNumberQuery}));
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

        [Test]
        public void VerifyThatRoleAndPermissionAreFilteredCorrectly()
        {
            var requestUtil = new Mock<IRequestUtils>();
            var metaInfoProvider = new Mock<IMetaInfoProvider>();
            requestUtil.Setup(x => x.GetRequestDataModelVersion).Returns(new Version(1, 0));
            metaInfoProvider.Setup(x => x.GetMetaInfo(ClassKind.ActionItem.ToString())).Returns(new ActionItemMetaInfo());
            metaInfoProvider.Setup(x => x.GetMetaInfo(ClassKind.SiteDirectory.ToString())).Returns(new SiteDirectoryMetaInfo());
            metaInfoProvider.Setup(x => x.GetMetaInfo(ClassKind.ActualFiniteState.ToString())).Returns(new ActualFiniteStateMetaInfo());
            metaInfoProvider.Setup(x => x.GetMetaInfo(ClassKind.DiagramCanvas.ToString())).Returns(new DiagramCanvasMetaInfo());
            metaInfoProvider.Setup(x => x.GetMetaInfo(ClassKind.EngineeringModel.ToString())).Returns(new EngineeringModelMetaInfo());
            metaInfoProvider.Setup(x => x.GetMetaInfo(ClassKind.ElementDefinition.ToString())).Returns(new ElementDefinitionMetaInfo());
            requestUtil.Setup(x => x.MetaInfoProvider).Returns(metaInfoProvider.Object);
            var personRole = new PersonRole(Guid.NewGuid(), 0);
            var participantRole = new ParticipantRole(Guid.NewGuid(), 0);

            var personPermission1 = new PersonPermission(Guid.NewGuid(), 0);
            personPermission1.ObjectClass = ClassKind.ActionItem;
            var personPermission2 = new PersonPermission(Guid.NewGuid(), 0);
            personPermission2.ObjectClass = ClassKind.SiteDirectory;
            var personPermission3 = new PersonPermission(Guid.NewGuid(), 0);
            personPermission3.ObjectClass = ClassKind.ActualFiniteState;

            var participantPermission1 = new ParticipantPermission(Guid.NewGuid(), 0);
            participantPermission1.ObjectClass = ClassKind.DiagramCanvas;
            var participantPermission2 = new ParticipantPermission(Guid.NewGuid(), 0);
            participantPermission2.ObjectClass = ClassKind.EngineeringModel;
            var participantPermission3 = new ParticipantPermission(Guid.NewGuid(), 0);
            participantPermission3.ObjectClass = ClassKind.ElementDefinition;

            personRole.PersonPermission.Add(personPermission3.Iid);
            personRole.PersonPermission.Add(personPermission2.Iid);
            personRole.PersonPermission.Add(personPermission1.Iid);

            participantRole.ParticipantPermission.Add(participantPermission1.Iid);
            participantRole.ParticipantPermission.Add(participantPermission2.Iid);
            participantRole.ParticipantPermission.Add(participantPermission3.Iid);

            var input = new List<Thing>
            {
                personRole,
                participantRole,
                personPermission1,
                personPermission2,
                personPermission3,
                participantPermission1,
                participantPermission2,
                participantPermission3
            };

            var result = HttpRequestHelper.FilterOutPermissions(input, requestUtil.Object).ToArray();
            Assert.IsFalse(result.Contains(personPermission1));
            Assert.IsFalse(result.Contains(participantPermission1));
            Assert.AreEqual(personRole.PersonPermission.Count, 2);
            Assert.AreEqual(participantRole.ParticipantPermission.Count, 2);
        }
    }
}