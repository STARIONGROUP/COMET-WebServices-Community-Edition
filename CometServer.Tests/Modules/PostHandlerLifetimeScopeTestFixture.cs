// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostHandlerLifetimeScopeTestFixture.cs" company="Starion Group S.A.">
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
    using System.IO;
    using System.Net;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    using Autofac;

    using Carter;
    using Carter.Response;

    using CDP4Authentication;

    using CDP4DalCommon.Protocol.Operations;
    using CDP4DalCommon.Protocol.Tasks;

    using CDP4JsonSerializer;

    using CDP4MessagePackSerializer;

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
    using CometServer.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging.Abstractions;

    using Moq;

    using Npgsql;

    using NUnit.Framework;

    /// <summary>
    /// Verifies that the POST handlers of the <see cref="SiteDirectoryApi"/> and the <see cref="EngineeringModelApi"/>
    /// dispose the lifetime scope of the POST on every path that does not hand it over, and that they keep it alive
    /// when the work continues after the request has ended
    /// </summary>
    [TestFixture]
    public class PostHandlerLifetimeScopeTestFixture
    {
        private const string SiteDirectoryPath = "/SiteDirectory/f13de6f8-b03a-46e7-a492-53b2f260f294";

        private const string IterationPath = "/EngineeringModel/9ec982e4-ef72-4953-aa85-b158a95d8d56/iteration/e163c5ad-f32b-4387-b805-f4b34600bc2c";

        private IContainer container;

        private Mock<ICdp4TransactionManager> transactionManager;

        private Mock<IAsyncDisposable> transactionManagerDisposal;

        private Mock<ICometTaskService> cometTaskService;

        private Mock<ICometHasStartedService> cometHasStartedService;

        [SetUp]
        public void SetUp()
        {
            this.transactionManager = new Mock<ICdp4TransactionManager>();

            // the transaction manager holds the database connection of the POST: its disposal shows that the scope was disposed
            this.transactionManagerDisposal = this.transactionManager.As<IAsyncDisposable>();
            this.transactionManagerDisposal.Setup(x => x.DisposeAsync()).Returns(ValueTask.CompletedTask);

            var credentialsService = new Mock<ICredentialsService>();
            credentialsService.Setup(x => x.Credentials).Returns(new Credentials { Person = new AuthenticationPerson(Guid.NewGuid(), 1) });

            var jsonSerializer = new Mock<ICdp4JsonSerializer>();
            jsonSerializer.Setup(x => x.Deserialize<PostOperation>(It.IsAny<Stream>())).Returns(new PostOperation());

            var builder = new ContainerBuilder();
            builder.RegisterType<RequestUtils>().As<IRequestUtils>().InstancePerLifetimeScope();
            builder.Register(_ => this.transactionManager.Object).As<ICdp4TransactionManager>().InstancePerLifetimeScope();
            builder.Register(_ => credentialsService.Object).As<ICredentialsService>().InstancePerLifetimeScope();
            builder.Register(_ => jsonSerializer.Object).As<ICdp4JsonSerializer>().InstancePerLifetimeScope();
            builder.Register(_ => new Mock<IHeaderInfoProvider>().Object).As<IHeaderInfoProvider>().InstancePerLifetimeScope();
            builder.Register(_ => new Mock<CometServer.Services.IServiceProvider>().Object).As<CometServer.Services.IServiceProvider>().InstancePerLifetimeScope();
            builder.Register(_ => new Mock<IMetaInfoProvider>().Object).As<IMetaInfoProvider>().InstancePerLifetimeScope();
            builder.Register(_ => new Mock<IOperationProcessor>().Object).As<IOperationProcessor>().InstancePerLifetimeScope();
            builder.Register(_ => new Mock<IFileBinaryService>().Object).As<IFileBinaryService>().InstancePerLifetimeScope();
            builder.Register(_ => new Mock<IRevisionService>().Object).As<IRevisionService>().InstancePerLifetimeScope();
            builder.Register(_ => new Mock<IMessagePackSerializer>().Object).As<IMessagePackSerializer>().InstancePerLifetimeScope();
            builder.Register(_ => new Mock<IPermissionInstanceFilterService>().Object).As<IPermissionInstanceFilterService>().InstancePerLifetimeScope();
            builder.Register(_ => new Mock<IChangeLogService>().Object).As<IChangeLogService>().InstancePerLifetimeScope();
            builder.Register(_ => new Mock<IModelCreatorManager>().Object).As<IModelCreatorManager>().InstancePerLifetimeScope();
            this.container = builder.Build();

            this.cometTaskService = new Mock<ICometTaskService>();

            this.cometHasStartedService = new Mock<ICometHasStartedService>();
            this.cometHasStartedService.Setup(x => x.GetHasStartedAndIsReady()).Returns(new ServerStatus(true, DateTime.Now));
        }

        [TearDown]
        public void TearDown()
        {
            this.container.Dispose();
        }

        [TestCase(SiteDirectoryPath)]
        [TestCase(IterationPath)]
        public async Task VerifyThatAPostThatIsNotAuthorizedDisposesItsScope(string path)
        {
            var httpContext = CreateHttpContext(path, "");

            await this.PostAsync(path, httpContext, authorizationFailure: new AuthorizationException("no"));

            // UpdateWithNotAutherizedSettings answers an authenticated user that is not authorized with 403
            Assert.That(httpContext.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.Forbidden));
            this.transactionManagerDisposal.Verify(x => x.DisposeAsync(), Times.Once);
        }

        [TestCase(SiteDirectoryPath)]
        [TestCase(IterationPath)]
        public async Task VerifyThatAPostWithAnUnsupportedQueryParameterDisposesItsScope(string path)
        {
            var httpContext = CreateHttpContext(path, "?unsupported=1");

            await this.PostAsync(path, httpContext);

            Assert.That(httpContext.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            this.transactionManagerDisposal.Verify(x => x.DisposeAsync(), Times.Once);
        }

        [TestCase(SiteDirectoryPath)]
        [TestCase(IterationPath)]
        public async Task VerifyThatAPostWithoutWaitTimeDisposesItsScopeAfterItHasBeenHandled(string path)
        {
            this.transactionManager.Setup(x => x.SetupTransactionAsync(It.IsAny<Credentials>())).ThrowsAsync(new BadRequestException("x"));

            var httpContext = CreateHttpContext(path, "");

            await this.PostAsync(path, httpContext);

            Assert.That(httpContext.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
            this.transactionManagerDisposal.Verify(x => x.DisposeAsync(), Times.Once);
        }

        [TestCase(SiteDirectoryPath)]
        [TestCase(IterationPath)]
        public async Task VerifyThatAPostThatExceedsItsWaitTimeKeepsItsScopeUntilTheWorkHasFinished(string path)
        {
            var releaseWork = new TaskCompletionSource();

            this.transactionManager.Setup(x => x.SetupTransactionAsync(It.IsAny<Credentials>()))
                .Returns(async () =>
                {
                    await releaseWork.Task;
                    throw new BadRequestException("x");
#pragma warning disable CS0162
                    return (NpgsqlTransaction)null;
#pragma warning restore CS0162
                });

            var httpContext = CreateHttpContext(path, "?waitTime=1");

            await this.PostAsync(path, httpContext);

            httpContext.Response.Body.Position = 0;
            var body = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();

            Assert.That(body, Does.Contain("\"statusKind\":0"), "the CometTask is returned");

            // the request has ended, but the work still uses the services of the POST
            this.transactionManagerDisposal.Verify(x => x.DisposeAsync(), Times.Never);

            releaseWork.SetResult();

            for (var i = 0; i < 100; i++)
            {
                try
                {
                    this.transactionManagerDisposal.Verify(x => x.DisposeAsync(), Times.Once);
                    break;
                }
                catch (MockException) when (i < 99)
                {
                    await Task.Delay(50);
                }
            }

            this.cometTaskService.Verify(x => x.AddOrUpdateTask(It.IsAny<CometTask>(), It.IsAny<DateTime>(), StatusKind.FAILED, "BAD REQUEST - x"), Times.Once);
        }

        private Task PostAsync(string path, HttpContext httpContext, Exception authorizationFailure = null)
        {
            if (path == SiteDirectoryPath)
            {
                var siteDirectoryApi = new TestSiteDirectoryApi(this.cometHasStartedService.Object, this.cometTaskService.Object, this.container) { AuthorizationFailure = authorizationFailure };
                return siteDirectoryApi.CallPostSiteDirectoryAsync(httpContext.Request, httpContext.Response);
            }

            var engineeringModelApi = new TestEngineeringModelApi(this.cometHasStartedService.Object, this.cometTaskService.Object, this.container) { AuthorizationFailure = authorizationFailure };
            return engineeringModelApi.CallPostIterationAsync(httpContext.Request, httpContext.Response);
        }

        private static DefaultHttpContext CreateHttpContext(string path, string queryString)
        {
            var httpContext = new DefaultHttpContext
            {
                // Carter writes JSON through a response negotiator
                RequestServices = new ServiceCollection().AddOptions().AddSingleton<IResponseNegotiator, DefaultJsonResponseNegotiator>().BuildServiceProvider(),
                User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, "admin")], "Basic")),
                Response = { Body = new MemoryStream() }
            };

            httpContext.Request.Method = "POST";
            httpContext.Request.Path = path;
            httpContext.Request.QueryString = new QueryString(queryString);
            httpContext.Request.ContentType = "application/json";
            httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("{}"));

            return httpContext;
        }

        /// <summary>
        /// A <see cref="SiteDirectoryApi"/> that authorizes without a database
        /// </summary>
        private sealed class TestSiteDirectoryApi(ICometHasStartedService cometHasStartedService, ICometTaskService cometTaskService, ILifetimeScope lifetimeScope)
            : SiteDirectoryApi(new Mock<IAppConfigService>().Object, cometHasStartedService, new Mock<ITokenGeneratorService>().Object,
                NullLoggerFactory.Instance, new Mock<IBackgroundThingsMessageProducer>().Object, cometTaskService, new Mock<IDataSource>().Object, lifetimeScope)
        {
            public Exception AuthorizationFailure { get; init; }

            public Task CallPostSiteDirectoryAsync(HttpRequest req, HttpResponse res)
            {
                return this.PostSiteDirectoryAsync(req, res);
            }

            protected override Task<string> AuthorizeAsync(ICredentialsService credentialsService, HttpRequest request)
            {
                return this.AuthorizationFailure != null ? Task.FromException<string>(this.AuthorizationFailure) : Task.FromResult("admin");
            }
        }

        /// <summary>
        /// An <see cref="EngineeringModelApi"/> that authorizes without a database
        /// </summary>
        private sealed class TestEngineeringModelApi(ICometHasStartedService cometHasStartedService, ICometTaskService cometTaskService, ILifetimeScope lifetimeScope)
            : EngineeringModelApi(new Mock<IAppConfigService>().Object, cometHasStartedService, new Mock<ITokenGeneratorService>().Object,
                NullLoggerFactory.Instance, new Mock<IBackgroundThingsMessageProducer>().Object, cometTaskService, new Mock<IDataSource>().Object, lifetimeScope)
        {
            public Exception AuthorizationFailure { get; init; }

            public Task CallPostIterationAsync(HttpRequest req, HttpResponse res)
            {
                return this.PostIterationAsync(req, res);
            }

            protected override Task<string> AuthorizeAsync(ICredentialsService credentialsService, HttpRequest request)
            {
                return this.AuthorizationFailure != null ? Task.FromException<string>(this.AuthorizationFailure) : Task.FromResult("admin");
            }
        }
    }
}
