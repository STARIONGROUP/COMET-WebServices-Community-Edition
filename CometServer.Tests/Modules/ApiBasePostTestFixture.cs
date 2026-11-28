// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiBasePostTestFixture.cs" company="Starion Group S.A.">
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
    using System.Threading.Tasks;

    using Autofac;

    using Carter;
    using Carter.Response;

    using CDP4Common.DTO;

    using CDP4DalCommon.Protocol.Tasks;

    using CDP4JsonSerializer;

    using CDP4MessagePackSerializer;

    using CDP4ServicesMessaging.Services.BackgroundMessageProducers;

    using CometServer.Configuration;
    using CometServer.Health;
    using CometServer.Helpers;
    using CometServer.Modules;
    using CometServer.Services;
    using CometServer.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging.Abstractions;

    using Moq;

    using NUnit.Framework;

    /// <summary>
    /// Test fixture for the handling of a POST request with a wait time in <see cref="ApiBase"/>: the services of the
    /// POST come from a lifetime scope of the POST, which must stay alive until the work has finished and the response
    /// has been written, and the work must not write to the response of a request that has ended
    /// </summary>
    [TestFixture]
    public class ApiBasePostTestFixture
    {
        private const int WaitTime = 1;

        private IContainer container;

        private PostLifetimeScope postLifetimeScope;

        private DisposeTracker servicesOfThePost;

        private Mock<ICometTaskService> cometTaskService;

        private TestApi api;

        private CometTask cometTask;

        private PostRequestData postRequestData;

        [SetUp]
        public void SetUp()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<DisposeTracker>().AsSelf().InstancePerLifetimeScope();
            this.container = builder.Build();

            // stands for the scope that the request handler starts and resolves the services of the POST from
            this.postLifetimeScope = new PostLifetimeScope(this.container.BeginLifetimeScope());
            this.servicesOfThePost = this.postLifetimeScope.Resolve<DisposeTracker>();

            this.cometTaskService = new Mock<ICometTaskService>();

            this.api = new TestApi(this.cometTaskService.Object, this.container);

            this.cometTask = new CometTask { Id = Guid.NewGuid(), Actor = Guid.NewGuid(), StatusKind = StatusKind.PROCESSING, StartedAt = DateTime.Now };

            this.postRequestData = new PostRequestData
            {
                ContentType = ContentTypeKind.JSON,
                MethodPathName = "test",
                Version = new Version(1, 0, 0)
            };
        }

        [TearDown]
        public void TearDown()
        {
            this.container.Dispose();
        }

        [Test]
        public async Task VerifyThatTheResultIsWrittenAndTheScopeDisposedWhenTheWorkCompletesWithinTheWaitTime()
        {
            var expected = PostResult.Success([]);
            PostResult written = null;
            var servicesWereAliveWhileWriting = false;
            var httpContext = CreateHttpContext();

            await this.api.CallProcessPostWithWaitTimeAsync(this.postLifetimeScope, WaitTime, this.postRequestData, this.cometTask,
                () => Task.FromResult(expected),
                postResult =>
                {
                    written = postResult;
                    servicesWereAliveWhileWriting = !this.servicesOfThePost.IsDisposed;
                    return Task.CompletedTask;
                },
                httpContext.Response);

            Assert.Multiple(() =>
            {
                Assert.That(written, Is.SameAs(expected));
                Assert.That(servicesWereAliveWhileWriting, Is.True, "the response is written with the services of the POST");
                Assert.That(this.servicesOfThePost.IsDisposed, Is.True, "the scope of the POST is disposed when the POST has been handled");
                Assert.That(httpContext.Response.Body.Length, Is.Zero, "the CometTask must not be written as well");
            });
        }

        [Test]
        public async Task VerifyThatTheServicesStayAliveUntilTheWorkHasFinishedWhenTheWorkExceedsTheWaitTime()
        {
            var releaseWork = new TaskCompletionSource();
            var workFinished = new TaskCompletionSource<bool>();
            var resultWritten = false;
            var httpContext = CreateHttpContext();

            await this.api.CallProcessPostWithWaitTimeAsync(this.postLifetimeScope, WaitTime, this.postRequestData, this.cometTask,
                async () =>
                {
                    await releaseWork.Task;

                    // the request has ended by now: the services of the POST must still be usable
                    workFinished.SetResult(!this.servicesOfThePost.IsDisposed);
                    return PostResult.Success([]);
                },
                _ =>
                {
                    resultWritten = true;
                    return Task.CompletedTask;
                },
                httpContext.Response);

            var body = await ReadBodyAsync(httpContext);

            Assert.Multiple(() =>
            {
                Assert.That(body, Does.Contain(this.cometTask.Id.ToString()), "the CometTask is returned");
                Assert.That(body, Does.Contain("\"statusKind\":0"), "the CometTask is PROCESSING");
                Assert.That(this.servicesOfThePost.IsDisposed, Is.False, "the work is still running");
            });

            releaseWork.SetResult();
            var servicesWereAliveAtTheEndOfTheWork = await workFinished.Task.WaitAsync(TimeSpan.FromSeconds(10));

            await this.WaitUntilTheServicesOfThePostAreDisposedAsync();

            Assert.Multiple(() =>
            {
                Assert.That(servicesWereAliveAtTheEndOfTheWork, Is.True);
                Assert.That(resultWritten, Is.False, "nothing may be written to the response after the request has ended");
                Assert.That(this.servicesOfThePost.IsDisposed, Is.True, "the scope of the POST is disposed when the work has finished");
            });
        }

        [Test]
        public async Task VerifyThatWorkThatThrowsWithinTheWaitTimeIsWrittenAsAFailure()
        {
            PostResult written = null;
            var httpContext = CreateHttpContext();

            await this.api.CallProcessPostWithWaitTimeAsync(this.postLifetimeScope, WaitTime, this.postRequestData, this.cometTask,
                () => throw new InvalidDataException("broken"),
                postResult =>
                {
                    written = postResult;
                    return Task.CompletedTask;
                },
                httpContext.Response);

            Assert.Multiple(() =>
            {
                Assert.That(written.IsSuccess, Is.False);
                Assert.That(written.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
                Assert.That(written.ErrorMessage, Is.EqualTo("broken"));
                Assert.That(this.servicesOfThePost.IsDisposed, Is.True);
            });

            this.cometTaskService.Verify(x => x.AddOrUpdateTask(this.cometTask, It.IsAny<DateTime>(), StatusKind.FAILED, "INTERNAL SERVER ERROR - broken"), Times.Once);
        }

        [Test]
        public async Task VerifyThatWorkThatFailsAfterTheWaitTimeEndsWithAFailedTaskAndWritesNothing()
        {
            var releaseWork = new TaskCompletionSource();
            var resultWritten = false;
            var httpContext = CreateHttpContext();

            await this.api.CallProcessPostWithWaitTimeAsync(this.postLifetimeScope, WaitTime, this.postRequestData, this.cometTask,
                async () =>
                {
                    await releaseWork.Task;
                    throw new InvalidDataException("broken");
                },
                _ =>
                {
                    resultWritten = true;
                    return Task.CompletedTask;
                },
                httpContext.Response);

            var body = await ReadBodyAsync(httpContext);

            releaseWork.SetResult();
            await this.WaitUntilTheServicesOfThePostAreDisposedAsync();

            Assert.Multiple(() =>
            {
                Assert.That(body, Does.Contain(this.cometTask.Id.ToString()), "only the CometTask is returned");
                Assert.That(resultWritten, Is.False, "nothing may be written to the response after the request has ended");
                Assert.That(this.servicesOfThePost.IsDisposed, Is.True);
            });

            this.cometTaskService.Verify(x => x.AddOrUpdateTask(this.cometTask, It.IsAny<DateTime>(), StatusKind.FAILED, "INTERNAL SERVER ERROR - broken"), Times.Once);
        }

        [TestCase(HttpStatusCode.BadRequest, "BAD REQUEST - message")]
        [TestCase(HttpStatusCode.Forbidden, "FORBIDDEN - message")]
        [TestCase(HttpStatusCode.Unauthorized, "UNAUTHORIZED - message")]
        [TestCase(HttpStatusCode.InternalServerError, "INTERNAL SERVER ERROR - message")]
        public void VerifyThatAFailedPostIsRecordedInItsTask(HttpStatusCode statusCode, string expectedError)
        {
            var result = this.api.CallFailPost(this.cometTask, statusCode, "message", "#TAG");

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(statusCode));
                Assert.That(result.ErrorMessage, Is.EqualTo("message"));
                Assert.That(result.ErrorTag, Is.EqualTo("#TAG"));
            });

            this.cometTaskService.Verify(x => x.AddOrUpdateTask(this.cometTask, It.IsAny<DateTime>(), StatusKind.FAILED, expectedError), Times.Once);
        }

        [Test]
        public async Task VerifyThatAFailedPostResultIsWrittenWithItsStatusAndErrorTag()
        {
            var httpContext = CreateHttpContext();

            await this.api.CallWritePostResultAsync(PostResult.Failure(HttpStatusCode.BadRequest, "frozen", "#FROZEN_ITERATION"), this.postRequestData, httpContext.Response);

            var body = await ReadBodyAsync(httpContext);

            Assert.Multiple(() =>
            {
                Assert.That(httpContext.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
                Assert.That(httpContext.Response.Headers[HttpConstants.CDPErrorTag].ToString(), Is.EqualTo("#FROZEN_ITERATION"));
                Assert.That(body, Does.Contain("exception:frozen"));
            });
        }

        [TestCase(ContentTypeKind.JSON)]
        [TestCase(ContentTypeKind.MESSAGEPACK)]
        public async Task VerifyThatASuccessfulPostResultIsWrittenWithTheSerializerOfItsContentType(ContentTypeKind contentTypeKind)
        {
            var httpContext = CreateHttpContext();
            var things = new List<Thing>();
            var filteredThings = new List<Thing>();

            var headerInfoProvider = new Mock<IHeaderInfoProvider>();
            var jsonSerializer = new Mock<ICdp4JsonSerializer>();
            var messagePackSerializer = new Mock<IMessagePackSerializer>();
            var permissionInstanceFilterService = new Mock<IPermissionInstanceFilterService>();
            permissionInstanceFilterService.Setup(x => x.FilterOutPermissionsAsync(things, this.postRequestData.Version)).ReturnsAsync(filteredThings);

            this.postRequestData.ContentType = contentTypeKind;

            await this.api.CallWritePostResultAsync(PostResult.Success(things), this.postRequestData, httpContext.Response,
                headerInfoProvider.Object, new Mock<IMetaInfoProvider>().Object, jsonSerializer.Object, messagePackSerializer.Object, permissionInstanceFilterService.Object);

            var isJson = contentTypeKind == ContentTypeKind.JSON;

            Assert.Multiple(() =>
            {
                Assert.That(httpContext.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
                Assert.That(httpContext.Response.Headers.ContainsKey(HttpConstants.CDPErrorTag), Is.False);
            });

            headerInfoProvider.Verify(x => x.RegisterResponseHeaders(httpContext.Response, contentTypeKind, ""), Times.Once);
            jsonSerializer.Verify(x => x.SerializeToStream(It.IsAny<object>(), It.IsAny<Stream>()), isJson ? Times.Once() : Times.Never());
            messagePackSerializer.Verify(x => x.SerializeToStream(It.IsAny<IEnumerable<Thing>>(), It.IsAny<Stream>()), isJson ? Times.Never() : Times.Once());
        }

        [Test]
        public async Task VerifyThatAFailedPostResultWithoutErrorTagAddsNoErrorTagHeader()
        {
            var httpContext = CreateHttpContext();

            await this.api.CallWritePostResultAsync(PostResult.Failure(HttpStatusCode.Forbidden, "no"), this.postRequestData, httpContext.Response);

            Assert.Multiple(() =>
            {
                Assert.That(httpContext.Response.StatusCode, Is.EqualTo((int)HttpStatusCode.Forbidden));
                Assert.That(httpContext.Response.Headers.ContainsKey(HttpConstants.CDPErrorTag), Is.False);
            });
        }

        [Test]
        public void VerifyThatAWaitTimeOfZeroIsRejectedAndTheScopeIsDisposed()
        {
            var workStarted = false;

            Assert.That(async () => await this.api.CallProcessPostWithWaitTimeAsync(this.postLifetimeScope, 0, this.postRequestData, this.cometTask,
                    () =>
                    {
                        workStarted = true;
                        return Task.FromResult(PostResult.Success([]));
                    },
                    _ => Task.CompletedTask,
                    CreateHttpContext().Response),
                Throws.TypeOf<ArgumentOutOfRangeException>());

            Assert.Multiple(() =>
            {
                Assert.That(workStarted, Is.False);
                Assert.That(this.servicesOfThePost.IsDisposed, Is.True, "nobody else would dispose the scope");
            });
        }

        private async Task WaitUntilTheServicesOfThePostAreDisposedAsync()
        {
            // the work disposes the scope after it has returned its result
            for (var i = 0; i < 100 && !this.servicesOfThePost.IsDisposed; i++)
            {
                await Task.Delay(50);
            }
        }

        private static async Task<string> ReadBodyAsync(DefaultHttpContext httpContext)
        {
            httpContext.Response.Body.Position = 0;
            return await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
        }

        private static DefaultHttpContext CreateHttpContext()
        {
            return new DefaultHttpContext
            {
                // Carter writes JSON through a response negotiator
                RequestServices = new ServiceCollection().AddOptions().AddSingleton<IResponseNegotiator, DefaultJsonResponseNegotiator>().BuildServiceProvider(),
                Response = { Body = new MemoryStream() }
            };
        }

        /// <summary>
        /// Records whether the lifetime scope that created it has been disposed
        /// </summary>
        private sealed class DisposeTracker : IDisposable
        {
            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                this.IsDisposed = true;
            }
        }

        /// <summary>
        /// Exposes the protected POST helpers of <see cref="ApiBase"/>
        /// </summary>
        private sealed class TestApi(ICometTaskService cometTaskService, ILifetimeScope lifetimeScope)
            : SiteDirectoryApi(new Mock<IAppConfigService>().Object, new Mock<ICometHasStartedService>().Object, new Mock<ITokenGeneratorService>().Object,
                NullLoggerFactory.Instance, new Mock<IBackgroundThingsMessageProducer>().Object, cometTaskService, new Mock<IDataSource>().Object, lifetimeScope)
        {
            public Task CallProcessPostWithWaitTimeAsync(PostLifetimeScope postLifetimeScope, int waitTime, PostRequestData postRequestData, CometTask cometTask, Func<Task<PostResult>> processPost, Func<PostResult, Task> writeResult, HttpResponse httpResponse)
            {
                return this.ProcessPostWithWaitTimeAsync(postLifetimeScope, waitTime, postRequestData, "token", cometTask, processPost, writeResult, httpResponse);
            }

            public PostResult CallFailPost(CometTask cometTask, HttpStatusCode statusCode, string errorMessage, string errorTag)
            {
                return this.FailPost(cometTask, statusCode, errorMessage, errorTag);
            }

            public Task CallWritePostResultAsync(PostResult postResult, PostRequestData postRequestData, HttpResponse httpResponse)
            {
                return this.WritePostResultAsync(postResult, postRequestData, httpResponse, null, null, null, null, null);
            }

            public Task CallWritePostResultAsync(PostResult postResult, PostRequestData postRequestData, HttpResponse httpResponse, IHeaderInfoProvider headerInfoProvider, IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService)
            {
                return this.WritePostResultAsync(postResult, postRequestData, httpResponse, headerInfoProvider, metaInfoProvider, jsonSerializer, messagePackSerializer, permissionInstanceFilterService);
            }
        }
    }
}
