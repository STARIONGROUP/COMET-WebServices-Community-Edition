// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessPostFailureTestFixture.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2026 Starion Group S.A.
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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Security;
    using System.Threading.Tasks;

    using CDP4Common.Exceptions;

    using CDP4DalCommon.Protocol.Tasks;

    using CDP4JsonSerializer;

    using CDP4ServicesMessaging.Services.BackgroundMessageProducers;

    using CometServer.Authorization;
    using CometServer.Configuration;
    using CometServer.Exceptions;
    using CometServer.Health;
    using CometServer.Helpers;
    using CometServer.Modules;
    using CometServer.Services;
    using CometServer.Services.ChangeLog;
    using CometServer.Services.Operations;
    using CometServer.Services.Protocol;
    using CometServer.Tasks;

    using Microsoft.Extensions.Logging.Abstractions;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Verifies that the POST processing of the <see cref="SiteDirectoryApi"/> and the <see cref="EngineeringModelApi"/>
    /// turns every kind of failure into a <see cref="PostResult"/> with the right status, and records it in the
    /// <see cref="CometTask"/>: work that continues after its request has ended can report its outcome only there
    /// </summary>
    [TestFixture]
    public class ProcessPostFailureTestFixture
    {
        private Mock<ICometTaskService> cometTaskService;

        private Mock<ICdp4TransactionManager> transactionManager;

        private CometTask cometTask;

        private PostRequestData postRequestData;

        /// <summary>
        /// Each exception the POST processing handles, with the status of the response and the text of the task error
        /// </summary>
        private static IEnumerable<TestCaseData> Failures()
        {
            yield return new TestCaseData(new Cdp4ModelValidationException("x"), HttpStatusCode.BadRequest, "BAD REQUEST - x").SetArgDisplayNames(nameof(Cdp4ModelValidationException));
            yield return new TestCaseData(new InvalidOperationException("x"), HttpStatusCode.Forbidden, "FORBIDDEN - x").SetArgDisplayNames(nameof(InvalidOperationException));
            yield return new TestCaseData(new BadRequestException("x"), HttpStatusCode.BadRequest, "BAD REQUEST - x").SetArgDisplayNames(nameof(BadRequestException));
            yield return new TestCaseData(new InvalidDataException("x"), HttpStatusCode.BadRequest, "BAD REQUEST - x").SetArgDisplayNames(nameof(InvalidDataException));
            yield return new TestCaseData(new SecurityException("x"), HttpStatusCode.Unauthorized, "UNAUTHORIZED - x").SetArgDisplayNames(nameof(SecurityException));
            yield return new TestCaseData(new ThingNotFoundException("x"), HttpStatusCode.Unauthorized, "UNAUTHORIZED - x").SetArgDisplayNames(nameof(ThingNotFoundException));
            yield return new TestCaseData(new ResolveException("x"), HttpStatusCode.BadRequest, "BAD REQUEST - x").SetArgDisplayNames(nameof(ResolveException));
            yield return new TestCaseData(new ApplicationException("x"), HttpStatusCode.InternalServerError, "INTERNAL SERVER ERROR - x").SetArgDisplayNames("any other exception");
        }

        [SetUp]
        public void SetUp()
        {
            this.cometTaskService = new Mock<ICometTaskService>();
            this.transactionManager = new Mock<ICdp4TransactionManager>();

            this.cometTask = new CometTask { Id = Guid.NewGuid(), Actor = Guid.NewGuid(), StatusKind = StatusKind.PROCESSING, StartedAt = DateTime.Now };

            this.postRequestData = new PostRequestData
            {
                ContentType = ContentTypeKind.JSON,
                MethodPathName = "test",
                Path = "EngineeringModel/9ec982e4-ef72-4953-aa85-b158a95d8d56/iteration/e163c5ad-f32b-4387-b805-f4b34600bc2c",
                Version = new Version(1, 0, 0)
            };
        }

        [TestCaseSource(nameof(Failures))]
        public async Task VerifyThatTheSiteDirectoryPostTurnsAFailureIntoAFailedTask(Exception failure, HttpStatusCode expectedStatus, string expectedTaskError)
        {
            this.transactionManager.Setup(x => x.SetupTransactionAsync(It.IsAny<Credentials>())).ThrowsAsync(failure);

            var api = new TestSiteDirectoryApi(this.cometTaskService.Object);

            var result = await api.CallProcessPostAsync(this.postRequestData, this.cometTask, this.transactionManager.Object);

            this.AssertFailure(result, expectedStatus, expectedTaskError);
        }

        [TestCaseSource(nameof(Failures))]
        public async Task VerifyThatTheEngineeringModelPostTurnsAFailureIntoAFailedTask(Exception failure, HttpStatusCode expectedStatus, string expectedTaskError)
        {
            this.transactionManager.Setup(x => x.SetupTransactionAsync(It.IsAny<Credentials>())).ThrowsAsync(failure);

            var api = new TestEngineeringModelApi(this.cometTaskService.Object);

            var result = await api.CallProcessPostAsync(this.postRequestData, this.cometTask, this.transactionManager.Object);

            this.AssertFailure(result, expectedStatus, expectedTaskError);
        }

        private void AssertFailure(PostResult result, HttpStatusCode expectedStatus, string expectedTaskError)
        {
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(expectedStatus));
                Assert.That(result.ErrorMessage, Is.EqualTo("x"));
                Assert.That(result.ErrorTag, Is.Null);
            });

            this.cometTaskService.Verify(x => x.AddOrUpdateTask(this.cometTask, It.IsAny<DateTime>(), StatusKind.FAILED, expectedTaskError), Times.Once);
            this.cometTaskService.Verify(x => x.AddOrUpdateSuccessfulTask(It.IsAny<CometTask>(), It.IsAny<int>()), Times.Never);

            // no transaction was started, so there is nothing to roll back; the (null) transaction is still disposed
            this.transactionManager.Verify(x => x.TryRollbackTransaction(It.IsAny<NpgsqlTransaction>()), Times.Never);
            this.transactionManager.Verify(x => x.TryDisposeTransaction(null), Times.Once);
        }

        /// <summary>
        /// Exposes the POST processing of the <see cref="SiteDirectoryApi"/>
        /// </summary>
        private sealed class TestSiteDirectoryApi(ICometTaskService cometTaskService)
            : SiteDirectoryApi(new Mock<IAppConfigService>().Object, new Mock<ICometHasStartedService>().Object, new Mock<ITokenGeneratorService>().Object,
                NullLoggerFactory.Instance, new Mock<IBackgroundThingsMessageProducer>().Object, cometTaskService, new Mock<IDataSource>().Object, null)
        {
            public Task<PostResult> CallProcessPostAsync(PostRequestData postRequestData, CometTask cometTask, ICdp4TransactionManager transactionManager)
            {
                return this.ProcessPostAsync(postRequestData, "token", cometTask, new RequestUtils { QueryParameters = new QueryParameters { RevisionNumber = -1 } },
                    transactionManager, new Mock<ICredentialsService>().Object, new Mock<IOperationProcessor>().Object, new Mock<IRevisionService>().Object,
                    new Mock<ICdp4JsonSerializer>().Object, new Mock<IModelCreatorManager>().Object);
            }
        }

        /// <summary>
        /// Exposes the POST processing of the <see cref="EngineeringModelApi"/>
        /// </summary>
        private sealed class TestEngineeringModelApi(ICometTaskService cometTaskService)
            : EngineeringModelApi(new Mock<IAppConfigService>().Object, new Mock<ICometHasStartedService>().Object, new Mock<ITokenGeneratorService>().Object,
                NullLoggerFactory.Instance, new Mock<IBackgroundThingsMessageProducer>().Object, cometTaskService, new Mock<IDataSource>().Object, null)
        {
            public Task<PostResult> CallProcessPostAsync(PostRequestData postRequestData, CometTask cometTask, ICdp4TransactionManager transactionManager)
            {
                return this.ProcessPostAsync(postRequestData, "token", cometTask, new RequestUtils { QueryParameters = new QueryParameters { RevisionNumber = -1 } },
                    transactionManager, new Mock<ICredentialsService>().Object, new Mock<CometServer.Services.IServiceProvider>().Object, new Mock<IMetaInfoProvider>().Object,
                    new Mock<IOperationProcessor>().Object, new Mock<IRevisionService>().Object, new Mock<ICdp4JsonSerializer>().Object, new Mock<IChangeLogService>().Object);
            }
        }
    }
}
