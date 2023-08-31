// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CherryPickFeatureTestFixture.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
// 
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski,
//                 Ahmed Abulwafa Ahmed
// 
//    This file is part of CDP4 Web Services Community Edition.
//    The CDP4 Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
// 
//    The CDP4 Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4 Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Tests.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using CDP4Authentication;

    using CDP4Common;
    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.MetaInfo;

    using CDP4Orm.Dao.Revision;

    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Modules;
    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authentication;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.CherryPick;
    using CDP4WebServices.API.Services.Protocol;

    using Moq;

    using Nancy;
    using Nancy.Testing;

    using Npgsql;

    using NUnit.Framework;

    using IServiceProvider = CDP4WebServices.API.Services.IServiceProvider;
    using Thing = CDP4Common.DTO.Thing;

    [TestFixture]
    public class CherryPickFeatureTestFixture
    {
        private EngineeringModelApi engineeringModelApi;
        private Mock<IPersonResolver> personResolver;
        private Mock<IObfuscationService> obfuscationService;
        private Mock<IRequestUtils> requestUtils;
        private Mock<ICdp4TransactionManager> transactionManager;
        private Mock<IServiceProvider> serviceProvider;
        private Mock<IPermissionService> permissionService;
        private Browser browser;
        private Mock<ICherryPickService> cherryPickService;
        private Mock<IContainmentService> containmentService;
        private Credentials credentials;
        private QueryParameters queryParameters;
        private Mock<IHeaderInfoProvider> headerInfoProvider;
        private Guid engineeringModelId;
        private Mock<IReadService> engineeringModelReadService;
        private Guid iterationId;
        private Mock<IRevisionResolver> revisionResolver;

        [SetUp]
        public void SetUp()
        {
            this.obfuscationService = new Mock<IObfuscationService>();
            this.personResolver = new Mock<IPersonResolver>();
            this.requestUtils = new Mock<IRequestUtils>();
            this.transactionManager = new Mock<ICdp4TransactionManager>();
            this.serviceProvider = new Mock<IServiceProvider>();
            this.permissionService = new Mock<IPermissionService>();
            this.cherryPickService = new Mock<ICherryPickService>();
            this.containmentService = new Mock<IContainmentService>();
            this.revisionResolver = new Mock<IRevisionResolver>();

            (int FromRevision, int ToRevision, IEnumerable<RevisionRegistryInfo> RevisionRegistryInfoList) value;

            this.revisionResolver.Setup(x => x.TryResolve(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<object>(), out value))
                .Returns(false);
            
            this.queryParameters = new QueryParameters()
            {
                ClassKinds = Enumerable.Empty<ClassKind>(),
                CategoriesId = Enumerable.Empty<Guid>(),
                RevisionNumber = -1,
                CherryPick = true
            };

            this.headerInfoProvider = new Mock<IHeaderInfoProvider>();

            this.credentials = new Credentials()
            {
                UserName = "admin",
                Claims = new List<string>(),
                Person = new AuthenticationPerson(Guid.NewGuid(), 1)
                {
                    IsActive = true,
                    IsDeprecated = false
                }
            };

            this.engineeringModelApi = new EngineeringModelApi()
            {
                ObfuscationService = this.obfuscationService.Object,
                PersonResolver = this.personResolver.Object,
                RequestUtils = this.requestUtils.Object,
                TransactionManager = this.transactionManager.Object,
                ServiceProvider = this.serviceProvider.Object,
                PermissionService = this.permissionService.Object,
                CherryPickService = this.cherryPickService.Object,
                ContainmentService = this.containmentService.Object,
                HeaderInfoProvider = this.headerInfoProvider.Object,
                RevisionResolver = this.revisionResolver.Object
            };

            this.engineeringModelId = Guid.NewGuid();
            this.iterationId = Guid.NewGuid();

            var cdpContext = new Mock<ICdp4RequestContext>();

            this.requestUtils.Setup(x => x.Context).Returns(cdpContext.Object);
            this.requestUtils.Setup(x => x.QueryParameters).Returns(this.queryParameters);

            this.requestUtils.Setup(x => x.GetEngineeringModelPartitionString(this.engineeringModelId))
                .Returns($"EngineeringModel_{this.engineeringModelId.ToString().Replace("-", "_")}");

            cdpContext.Setup(x => x.AuthenticatedCredentials).Returns(this.credentials);

            this.browser = new Browser(cfg =>
            { 
                cfg.Module(this.engineeringModelApi);

                cfg.RequestStartup((_, _, context) =>
                {
                    context.CurrentUser = this.credentials;
                });
            });

            var siteDirectoryService = new Mock<IReadService>();
            var engineeringModelSetupId = Guid.NewGuid();

            var siteDirectory = new SiteDirectory()
            {
                Model = new List<Guid>
                {
                    engineeringModelSetupId
                }
            };

            siteDirectoryService.Setup(x => x.Get(It.IsAny<NpgsqlTransaction>(), "SiteDirectory", null, It.IsAny<ISecurityContext>()))
                .Returns(new List<Thing> { siteDirectory });
            
            this.serviceProvider.Setup(x => x.MapToReadService("SiteDirectory")).Returns(siteDirectoryService.Object);

            var engineeringModelSetupService = new Mock<IReadService>();

            var engineeringModelSetup = new EngineeringModelSetup()
            {
                Iid = engineeringModelSetupId,
                EngineeringModelIid = this.engineeringModelId
            };

            engineeringModelSetupService.Setup(x => x.Get(It.IsAny<NpgsqlTransaction>(), "SiteDirectory", siteDirectory.Model, It.IsAny<ISecurityContext>()))
                .Returns(new List<Thing>{engineeringModelSetup});

            this.serviceProvider.Setup(x => x.MapToReadService("EngineeringModelSetup")).Returns(engineeringModelSetupService.Object);

            this.engineeringModelReadService = new Mock<IReadService>();
            var modelDataLibraryReadService = new Mock<IReadService>();
            this.serviceProvider.Setup(x => x.MapToReadService("EngineeringModel")).Returns(this.engineeringModelReadService.Object);
            this.serviceProvider.Setup(x => x.MapToReadService("ModelReferenceDataLibrary")).Returns(modelDataLibraryReadService.Object);

            this.engineeringModelReadService.Setup(x => x.Get(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(),
                It.Is<IEnumerable<Guid>>(g => g.Contains(this.engineeringModelId)),
                It.IsAny<ISecurityContext>())).Returns(new List<Thing> { new EngineeringModel() { Iid = this.engineeringModelId, Iteration = new List<Guid> { this.iterationId } } });

            modelDataLibraryReadService.Setup(x => x.Get(It.IsAny<NpgsqlTransaction>(), It.IsAny<string>(), It.IsAny<IEnumerable<Guid>>()
                , It.IsAny<ISecurityContext>())).Returns(new List<Thing>{new ModelReferenceDataLibrary()});

            var metaInfoprovider = new Mock<IMetaInfoProvider>();
            var metaInfo = new Mock<IMetaInfo>();
            var propertyMetaInfo = new PropertyMetaInfo("iteration","EngineeringModel",PropertyKind.Scalar, AggregationKind.Composite,false, false,true,1,"*",true);
            metaInfo.Setup(x => x.GetContainmentType("Iteration")).Returns(propertyMetaInfo);
            metaInfoprovider.Setup(x => x.GetMetaInfo("EngineeringModel")).Returns(metaInfo.Object);
            metaInfoprovider.Setup(x => x.GetMetaInfo(It.IsAny<EngineeringModel>())).Returns(metaInfo.Object);
            metaInfo.Setup(x => x.GetValue("Iteration", It.IsAny<EngineeringModel>())).Returns(new List<Guid>{this.iterationId});

            this.requestUtils.Setup(x => x.MetaInfoProvider).Returns(metaInfoprovider.Object);
        }

        [Test]
        public void VerifyGetRoute()
        {
            var route = $"EngineeringModel/{this.engineeringModelId}/iteration/{this.iterationId}";
            var response = this.browser.Get(route);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            this.queryParameters.ClassKinds = new List<ClassKind> { ClassKind.ElementDefinition };

            response = this.browser.Get(route);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            this.queryParameters.CategoriesId = new List<Guid> { Guid.NewGuid() };
            this.queryParameters.CherryPick = false;
            response = this.browser.Get(route);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            this.queryParameters.CherryPick = true;

            this.cherryPickService.Verify(x => x.CherryPick(It.IsAny<IReadOnlyList<Thing>>(),
                this.queryParameters.ClassKinds, this.queryParameters.CategoriesId), Times.Never);

            this.browser.Get(route);

            this.cherryPickService.Verify(x => x.CherryPick(It.IsAny<IReadOnlyList<Thing>>(),
                this.queryParameters.ClassKinds, this.queryParameters.CategoriesId), Times.Once);
        }
    }
}
