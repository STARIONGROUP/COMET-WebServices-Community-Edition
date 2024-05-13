// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteDirectoryApi.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
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

namespace CometServer.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security;
    using System.Threading.Tasks;

    using Carter.Response;

    using CDP4Common.DTO;
    using CDP4Common.Exceptions;

    using CDP4DalCommon.Tasks;

    using CDP4JsonSerializer;

    using CDP4MessagePackSerializer;

    using CDP4ServicesMessaging.Services.BackgroundMessageProducers;

    using CometServer.Authorization;
    using CometServer.Configuration;
    using CometServer.Exceptions;
    using CometServer.Extensions;
    using CometServer.Health;
    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations;
    using CometServer.Services.Protocol;
    using CometServer.Tasks;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using Npgsql;

    /// <summary>
    /// This is an API endpoint class to support interaction with the site directory contained model data
    /// </summary>
    public class SiteDirectoryApi : ApiBase
    {
        /// <summary>
        /// The top container.
        /// </summary>
        private const string TopContainer = "SiteDirectory";

        /// <summary>
        /// A <see cref="ILogger{SiteDirectoryApi}"/> instance
        /// </summary>
        private readonly ILogger<SiteDirectoryApi> logger;

        /// <summary>
        /// The supported get query parameters.
        /// </summary>
        private static readonly string[] SupportedGetQueryParameters =
            {
                QueryParameters.ExtentQuery, 
                QueryParameters.IncludeReferenceDataQuery,
                QueryParameters.IncludeAllContainersQuery, 
                QueryParameters.IncludeFileDataQuery,
                QueryParameters.RevisionNumberQuery, 
                QueryParameters.RevisionFromQuery, 
                QueryParameters.RevisionToQuery
            };

        /// <summary>
        /// The supported post query parameter.
        /// </summary>
        private static readonly string[] SupportedPostQueryParameter =
            { 
                QueryParameters.RevisionNumberQuery, 
                QueryParameters.ExportQuery,
                QueryParameters.WaitTimeQuery
            };

        /// <summary>
        /// Initializes a new instance of the &lt;see cref="ExchangeFileImportyApi"/&gt;
        /// </summary>
        /// <param name="appConfigService">
        /// The (injected) <see cref="IAppConfigService"/>
        /// </param>
        /// <param name="cometHasStartedService">
        /// The (injected) <see cref="ICometHasStartedService"/> that is used to check whether CDP4-COMET is ready to start
        /// acceptng requests
        /// </param>
        /// <param name="tokenGeneratorService">
        /// The (injected) <see cref="ITokenGeneratorService"/> used generate HTTP request tokens
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to create typed loggers
        /// </param>
        /// <param name="thingsMessageProducer">
        /// The (injected) <see cref="IBackgroundThingsMessageProducer"/> used to schedule things messages to be sent
        /// </param>
        /// <param name="cometTaskService">
        /// The (injected) <see cref="ICometTaskService"/> used to register and access running <see cref="CometTask"/>s
        /// </param>
        public SiteDirectoryApi(IAppConfigService appConfigService, ICometHasStartedService cometHasStartedService, ITokenGeneratorService tokenGeneratorService, ILoggerFactory loggerFactory, IBackgroundThingsMessageProducer thingsMessageProducer, ICometTaskService cometTaskService) 
            : base(appConfigService, cometHasStartedService, tokenGeneratorService, loggerFactory, thingsMessageProducer, cometTaskService)
        {
            this.logger = loggerFactory == null ? NullLogger<SiteDirectoryApi>.Instance : loggerFactory.CreateLogger<SiteDirectoryApi>();
        }

        /// <summary>
        /// Add the routes to the <see cref="IEndpointRouteBuilder"/>
        /// </summary>
        /// <param name="app">
        /// The <see cref="IEndpointRouteBuilder"/> to which the routes are added
        /// </param>
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("SiteDirectory",
                async (HttpRequest req, HttpResponse res, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ICredentialsService credentialsService, IHeaderInfoProvider headerInfoProvider, Services.IServiceProvider serviceProvider, IMetaInfoProvider metaInfoProvider, IRevisionService revisionService, IRevisionResolver revisionResolver, ICdp4JsonSerializer jsonSerializer, IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService) =>
                {
                    if (!await this.IsServerReady(res))
                    {
                        return;
                    }

                    if (!req.HttpContext.User.Identity.IsAuthenticated)
                    {
                        res.UpdateWithNotAuthenticatedSettings();
                        await res.AsJson("not authenticated");
                    }
                    else
                    {
                        try
                        {
                            await this.Authorize(this.AppConfigService, credentialsService, req.HttpContext.User.Identity.Name);
                        }
                        catch (AuthorizationException)
                        {
                            this.logger.LogWarning("The GET REQUEST was not authorized for {Identity}", req.HttpContext.User.Identity.Name);

                            res.UpdateWithNotAutherizedSettings();
                            await res.AsJson("not authorized");
                            return;
                        }

                        await this.GetResponseData(req, res, requestUtils, transactionManager, credentialsService, headerInfoProvider, serviceProvider, metaInfoProvider, revisionService, revisionResolver, jsonSerializer, messagePackSerializer, permissionInstanceFilterService);
                    }
                });

            app.MapGet("SiteDirectory/{*path}",
                async (HttpRequest req, HttpResponse res, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ICredentialsService credentialsService, IHeaderInfoProvider headerInfoProvider, Services.IServiceProvider serviceProvider, IMetaInfoProvider metaInfoProvider, IRevisionService revisionService, IRevisionResolver revisionResolver, ICdp4JsonSerializer jsonSerializer, IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService) =>
                {
                    if (!await this.IsServerReady(res))
                    {
                        return;
                    }

                    if (!req.HttpContext.User.Identity.IsAuthenticated)
                    {
                        res.UpdateWithNotAuthenticatedSettings();
                        await res.AsJson("not authenticated");
                    }
                    else
                    {
                        try
                        {
                            await this.Authorize(this.AppConfigService, credentialsService, req.HttpContext.User.Identity.Name);
                        }
                        catch (AuthorizationException)
                        {
                            this.logger.LogWarning("The GET REQUEST was not authorized for {Identity}", req.HttpContext.User.Identity.Name);

                            res.UpdateWithNotAutherizedSettings();
                            await res.AsJson("not authorized");
                            return;
                        }

                        await this.GetResponseData(req, res, requestUtils, transactionManager, credentialsService, headerInfoProvider, serviceProvider, metaInfoProvider, revisionService, revisionResolver, jsonSerializer, messagePackSerializer, permissionInstanceFilterService);
                    }
                });

            app.MapPost("SiteDirectory/{iid:guid}",
                async (HttpRequest req, HttpResponse res, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ICredentialsService credentialsService, IHeaderInfoProvider headerInfoProvider, IMetaInfoProvider metaInfoProvider, IOperationProcessor operationProcessor, IRevisionService revisionService, ICdp4JsonSerializer jsonSerializer, IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService, IModelCreatorManager modelCreatorManager) =>
                {
                    if (!await this.IsServerReady(res))
                    {
                        return;
                    }

                    if (!req.HttpContext.User.Identity.IsAuthenticated)
                    {
                        res.UpdateWithNotAuthenticatedSettings();
                        await res.AsJson("not authenticated");
                    }
                    else
                    {
                        var requestToken = this.TokenGeneratorService.GenerateRandomToken();

                        try
                        {
                            await this.Authorize(this.AppConfigService, credentialsService, req.HttpContext.User.Identity.Name);
                        }
                        catch (AuthorizationException)
                        {
                            this.logger.LogWarning("The {requestToken} POST REQUEST was not authorized for {Identity}", requestToken, req.HttpContext.User.Identity.Name);

                            res.UpdateWithNotAutherizedSettings();
                            await res.AsJson("not authorized");
                            return;
                        }

                        var cometTask = this.CreateAndRegisterCometTask(credentialsService.Credentials.Person.Iid, TopContainer, requestToken);
                        
                        PostRequestData postRequestData = null;

                        try
                        {
                            postRequestData = this.ProcessPostRequest(req, requestUtils, metaInfoProvider, jsonSerializer);
                        }
                        catch (BadRequestException ex)
                        {
                            this.logger.LogWarning("Request {requestToken} failed as BadRequest \n {ErrorMessage}", requestToken, ex.Message);

                            this.CometTaskService.AddOrUpdateTask(cometTask, finishedAt: DateTime.Now, statusKind: StatusKind.FAILED, error: $"BAD REQUEST - {ex.Message}");

                            res.StatusCode = (int)HttpStatusCode.BadRequest;
                            await res.AsJson($"exception:{ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            this.logger.LogError("Request {requestToken} failed as InternalServerError \n {ErrorMessage}", requestToken, ex.Message);

                            this.CometTaskService.AddOrUpdateTask(cometTask, finishedAt: DateTime.Now, statusKind: StatusKind.FAILED, error: $"INTERNAL SERVER ERROR - {ex.Message}");

                            res.StatusCode = (int)HttpStatusCode.InternalServerError;
                            await res.AsJson($"exception:{ex.Message}");
                        }

                        if (postRequestData.IsMultiPart )
                        {
                            this.logger.LogWarning("Request {requestToken} failed as BadRequest: The SiteDirectory does not support MultiPart POST request", requestToken);

                            this.CometTaskService.AddOrUpdateTask(cometTask, finishedAt: DateTime.Now, statusKind: StatusKind.FAILED, error: $"BAD REQUEST - The SiteDirectory does not support MultiPart POST request");

                            res.StatusCode = (int)HttpStatusCode.BadRequest;
                            await res.AsJson("The SiteDirectory does not support MultiPart POST request");
                        }

                        if (requestUtils.QueryParameters.WaitTime > 0)
                        {
                            await this.EnqueCometTaskForPostRequest(postRequestData, requestToken, res, cometTask, requestUtils, transactionManager, credentialsService, headerInfoProvider, metaInfoProvider, operationProcessor, revisionService, jsonSerializer, messagePackSerializer, permissionInstanceFilterService, modelCreatorManager);
                        }
                        else
                        {
                            await this.PostResponseData(postRequestData, requestToken, res, cometTask, requestUtils, transactionManager, credentialsService, headerInfoProvider, metaInfoProvider, operationProcessor, revisionService, jsonSerializer, messagePackSerializer, permissionInstanceFilterService, modelCreatorManager);
                        }
                    }
                });
        }

        /// <summary>
        /// Handles the GET request
        /// </summary>
        /// <param name="httpRequest">
        /// The <see cref="HttpRequest"/> that is being handled
        /// </param>
        /// <param name="httpResponse">
        /// The <see cref="HttpResponse"/> to which the results will be written
        /// </param>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <param name="transactionManager">
        /// The <see cref="ICdp4TransactionManager"/> that provides database transaction and connection services
        /// </param>
        /// <param name="credentialsService">
        /// The <see cref="ICredentialsService"/> used to provide authorization and <see cref="Credentials"/>
        /// services while handling a request
        /// </param>
        /// <param name="headerInfoProvider">
        /// The injected <see cref="IHeaderInfoProvider"/> instance used to process HTTP headers
        /// </param>
        /// <param name="serviceProvider">
        /// The <see cref="Services.IServiceProvider"/> that can be used to resolve other services
        /// </param>
        /// <param name="metaInfoProvider">
        /// The <see cref="IMetaInfoProvider"/> used to provide metadata for any kind of <see cref="Thing"/>
        /// </param>
        /// <param name="revisionService">
        /// The <see cref="IRevisionService"/> that supports revision based retrievel of data
        /// </param>
        /// <param name="revisionResolver">
        /// The <see cref="IRevisionResolver"/> used to resolve the revision numbers that belong to the revisionFrom and revisionTo parameters.
        /// </param>
        /// <param name="jsonSerializer">
        /// The <see cref="ICdp4JsonSerializer"/> used to serialize data to JSON
        /// </param>
        /// <param name="messagePackSerializer">
        /// The <see cref="IMessagePackSerializer"/> used to serialize data to MessagePack
        /// </param>
        /// <param name="permissionInstanceFilterService">
        /// The <see cref="IPermissionInstanceFilterService"/> that is used to filter out any <see cref="PersonPermission"/>
        /// and <see cref="ParticipantPermission"/> that is not supported by the requested data-model version
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/>
        /// </returns>
        protected async Task GetResponseData(HttpRequest httpRequest, HttpResponse httpResponse, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ICredentialsService credentialsService, IHeaderInfoProvider headerInfoProvider, Services.IServiceProvider serviceProvider, IMetaInfoProvider metaInfoProvider, IRevisionService revisionService, IRevisionResolver revisionResolver, ICdp4JsonSerializer jsonSerializer, IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            transactionManager.SetCachedDtoReadEnabled(true);

            var reqsw = Stopwatch.StartNew();
            var requestToken = this.TokenGeneratorService.GenerateRandomToken();

            var contentTypeKind = httpRequest.QueryContentTypeKind();

            this.logger.LogInformation("{request}:{requestToken} - START HTTP REQUEST PROCESSING", httpRequest.QueryNameMethodPath(), requestToken);

            try
            {
                // validate (and set) the supplied query parameters
                HttpRequestHelper.ValidateSupportedQueryParameter(httpRequest.Query, SupportedGetQueryParameters);
                var queryParameters = httpRequest.Query.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value.FirstOrDefault());
                requestUtils.QueryParameters = new QueryParameters(queryParameters);

                var fromRevision = requestUtils.QueryParameters.RevisionNumber;

                IReadOnlyList<Thing> things = null;

                var dbsw = Stopwatch.StartNew();
                this.logger.LogDebug("{request}:{requestToken} - Database operations started", httpRequest.QueryNameMethodPath(), requestToken);

                // get prepared data source transaction
                transaction = transactionManager.SetupTransaction(ref connection, credentialsService.Credentials);

                if (fromRevision > -1)
                {
                    // gather all Things that are newer then the indicated revision
                    things = revisionService.Get(transaction, TopContainer, fromRevision, true).ToList();
                }
                else if (revisionResolver.TryResolve(transaction, TopContainer, requestUtils.QueryParameters.RevisionFrom, requestUtils.QueryParameters.RevisionTo, out var resolvedValues))
                {
                    var routeSegments = HttpRequestHelper.ParseRouteSegments(httpRequest.Path);

                    var iid = routeSegments.Last();

                    if (!Guid.TryParse(iid, out var guid))
                    {
                        httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                        await httpResponse.AsJson("The identifier of the object to query was not found or the route is invalid.");
                        return;
                    }

                    things = revisionService.Get(transaction, TopContainer, guid, resolvedValues.FromRevision, resolvedValues.ToRevision).ToList();
                }
                else
                {
                    // gather all Things as indicated by the request URI 
                    var routeSegments = HttpRequestHelper.ParseRouteSegments(httpRequest.Path);
                    
                    things = this.GetContainmentResponse(requestUtils, transactionManager, serviceProvider, metaInfoProvider, transaction, TopContainer, routeSegments).ToList();
                }

                await transaction.CommitAsync();

                this.logger.LogDebug("{request}:{requestToken} - Database operations completed in {sw} [ms]", httpRequest.QueryNameMethodPath(), requestToken, dbsw.ElapsedMilliseconds);
                dbsw.Stop();

                this.logger.LogInformation("{request}:{requestToken} - Return response started", httpRequest.QueryNameMethodPath(), requestToken);

                var version = httpRequest.QueryDataModelVersion();

                switch (contentTypeKind)
                {
                    case ContentTypeKind.JSON:
                        await this.WriteJsonResponse(headerInfoProvider, metaInfoProvider, jsonSerializer, permissionInstanceFilterService, things, version, httpResponse, HttpStatusCode.OK, requestToken);
                        break;
                    case ContentTypeKind.MESSAGEPACK:
                        await this.WriteMessagePackResponse(headerInfoProvider, messagePackSerializer, permissionInstanceFilterService, things, version, httpResponse, HttpStatusCode.OK, requestToken);
                        break;
                }
            }
            catch (SecurityException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogWarning("{request}:{requestToken} - Unauthorized request returned after {ElapsedMilliseconds} [ms]", httpRequest.QueryNameMethodPath(), requestToken, reqsw.ElapsedMilliseconds);

                // error handling
                httpResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (ThingNotFoundException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogWarning("{request}:{requestToken} - Unauthorized (Thing Not Found) request returned after {ElapsedMilliseconds} [ms]", httpRequest.QueryNameMethodPath(), requestToken, reqsw.ElapsedMilliseconds);

                httpResponse.StatusCode = (int)HttpStatusCode.NotFound;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }
                
                this.logger.LogError(ex,"{request}:{requestToken} - Failed after {ElapsedMilliseconds} [ms]", httpRequest.QueryNameMethodPath(), requestToken, reqsw.ElapsedMilliseconds);

                // error handling
                httpResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            finally
            {
                if (transaction != null)
                {
                    await transaction.DisposeAsync();
                }

                if (connection != null)
                {
                    await connection.DisposeAsync();
                }

                this.logger.LogInformation("{request}:{requestToken} - Response returned in {sw} [ms]", httpRequest.QueryNameMethodPath(), requestToken, reqsw.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Process the <see cref="HttpRequest"/> and extracts the relevant information into an instance of <see cref="PostRequestData"/>
        /// </summary>
        /// <param name="httpRequest">
        /// The <see cref="HttpRequest"/> that is to be processed
        /// </param>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <param name="metaInfoProvider">
        /// The <see cref="IMetaInfoProvider"/> used to provide metadata for any kind of <see cref="Thing"/>
        /// </param>
        /// <param name="jsonSerializer">
        /// The <see cref="ICdp4JsonSerializer"/> used to serialize data to JSOIN
        /// </param>
        /// <returns>
        /// An instance of <see cref="PostRequestData"/>
        /// </returns>
        protected PostRequestData ProcessPostRequest(HttpRequest httpRequest, IRequestUtils requestUtils, IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer)
        {
            HttpRequestHelper.ValidateSupportedQueryParameter(httpRequest.Query, SupportedPostQueryParameter);
            var queryParameters = httpRequest.Query.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value.FirstOrDefault());
            requestUtils.QueryParameters = new QueryParameters(queryParameters);

            var postRequestData = new PostRequestData
            {
                ContentType = httpRequest.QueryContentTypeKind(),
                MethodPathName = httpRequest.QueryNameMethodPath(),
                Version = httpRequest.QueryDataModelVersion(),
                IsMultiPart = httpRequest.GetMultipartBoundary() != string.Empty
            };

            jsonSerializer.Initialize(metaInfoProvider, postRequestData.Version);
            postRequestData.OperationData = jsonSerializer.Deserialize<CdpPostOperation>(httpRequest.Body);

            return postRequestData;
        }

        /// <summary>
        /// Enques the handling of hte POST request on a background task. In case it takes longer to complete than the specified wait time a <see cref="CometTask"/>
        /// is returned. Otherwise a 10-25 response is retunred.
        /// </summary>
        /// <param name="postRequestData">
        /// The <see cref="PostRequestData"/> that is to be enqueued
        /// </param>
        /// <param name="requestToken">
        /// The request token used to track <see cref="HttpRequest"/> tracking
        /// </param>
        /// <param name="httpResponse">
        /// The <see cref="HttpResponse"/> to which the results will be written
        /// </param>
        /// <param name="cometTask">
        /// The <see cref="CometTask"/> that is linked to the current POST request
        /// </param>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <param name="transactionManager">
        /// The <see cref="ICdp4TransactionManager"/> that provides database transaction and connection services
        /// </param>
        /// <param name="credentialsService">
        /// The <see cref="ICredentialsService"/> used to provide authorization and <see cref="Credentials"/>
        /// services while handling a request
        /// </param>
        /// <param name="headerInfoProvider">
        /// The injected <see cref="IHeaderInfoProvider"/> instance used to process HTTP headers
        /// </param>
        /// <param name="metaInfoProvider">
        /// The <see cref="IMetaInfoProvider"/> used to provide metadata for any kind of <see cref="Thing"/>
        /// </param>
        /// <param name="operationProcessor">
        /// The <see cref="IOperationProcessor"/> used to process the POST operation
        /// </param>
        /// <param name="revisionService">
        /// The <see cref="IRevisionService"/> that supports revision based retrievel of data
        /// </param>
        /// <param name="jsonSerializer">
        /// The <see cref="ICdp4JsonSerializer"/> used to serialize data to JSOIN
        /// </param>
        /// <param name="messagePackSerializer">
        /// The <see cref="IMessagePackSerializer"/> used to serialize data to MessagePack
        /// </param>
        /// <param name="permissionInstanceFilterService">
        /// The <see cref="IPermissionInstanceFilterService"/> that is used to filter out any <see cref="PersonPermission"/>
        /// and <see cref="ParticipantPermission"/> that is not supported by the requested data-model version
        /// </param>
        /// <param name="modelCreatorManager">
        /// The <see cref="IModelCreatorManager"/> used to create an <see cref="EngineeringModel"/>
        /// </param>
        /// <returns>
        /// an awaitable <see cref="Task"/>
        /// </returns>
        protected async Task EnqueCometTaskForPostRequest(PostRequestData postRequestData, string requestToken, HttpResponse httpResponse, CometTask cometTask, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ICredentialsService credentialsService, IHeaderInfoProvider headerInfoProvider, IMetaInfoProvider metaInfoProvider, IOperationProcessor operationProcessor, IRevisionService revisionService, ICdp4JsonSerializer jsonSerializer, IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService, IModelCreatorManager modelCreatorManager)
        {
            var longRunningCometTask = Task.Run(() =>
            {
                var task = this.PostResponseData(postRequestData, requestToken, httpResponse, cometTask, requestUtils, transactionManager, credentialsService, headerInfoProvider, metaInfoProvider, operationProcessor, revisionService, jsonSerializer, messagePackSerializer, permissionInstanceFilterService, modelCreatorManager);
                this.logger.LogTrace(task.IsCompletedSuccessfully.ToString());
            });

            var completed = longRunningCometTask.Wait(TimeSpan.FromSeconds(requestUtils.QueryParameters.WaitTime));

            if (completed)
            {
                this.logger.LogInformation("The task {id} for actor {actor} completed within the requested wait time {waittime}", cometTask.Id, cometTask.Actor, requestUtils.QueryParameters.WaitTime);
            }
            else
            {
                this.logger.LogInformation("The task {id} for actor {actor} did not complete within the requested wait time {waittime}; the CometTask is returned.", cometTask.Id, cometTask.Actor, requestUtils.QueryParameters.WaitTime);

                await httpResponse.AsJson(cometTask);
            }
        }

        /// <summary>
        /// Handles the POST requset
        /// </summary>
        /// <param name="postRequestData">
        /// The <see cref="PostRequestData"/> that is being handled
        /// </param>
        /// <param name="requestToken">
        /// The request token used to track HttpRequest tracking
        /// </param>
        /// <param name="httpResponse">
        /// The <see cref="HttpResponse"/> to which the results will be written
        /// </param>
        /// <param name="cometTask">
        /// The <see cref="CometTask"/> that is linked to the current POST request
        /// </param>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <param name="transactionManager">
        /// The <see cref="ICdp4TransactionManager"/> that provides database transaction and connection services
        /// </param>
        /// <param name="credentialsService">
        /// The <see cref="ICredentialsService"/> used to provide authorization and <see cref="Credentials"/>
        /// services while handling a request
        /// </param>
        /// <param name="headerInfoProvider">
        /// The injected <see cref="IHeaderInfoProvider"/> instance used to process HTTP headers
        /// </param>
        /// <param name="metaInfoProvider">
        /// The <see cref="IMetaInfoProvider"/> used to provide metadata for any kind of <see cref="Thing"/>
        /// </param>
        /// <param name="jsonSerializer">
        /// The <see cref="ICdp4JsonSerializer"/> used to serialize data to JSON
        /// </param>
        /// <param name="messagePackSerializer">
        /// The <see cref="IMessagePackSerializer"/> used to serialize data to MessagePack
        /// </param>
        /// <param name="permissionInstanceFilterService">
        /// The <see cref="IPermissionInstanceFilterService"/> that is used to filter out any <see cref="PersonPermission"/>
        /// and <see cref="ParticipantPermission"/> that is not supported by the requested data-model version
        /// </param>
        /// <param name="modelCreatorManager">
        /// The <see cref="IModelCreatorManager"/> used to create an <see cref="EngineeringModel"/>
        /// </param>
        /// <param name="operationProcessor">
        /// The <see cref="IOperationProcessor"/> used to process the POST operation
        /// </param>
        /// <param name="revisionService">
        /// The <see cref="IRevisionService"/> that supports revision based retrievel of data
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/>
        /// </returns>
        protected async Task PostResponseData(PostRequestData postRequestData, string requestToken, HttpResponse httpResponse, CometTask cometTask, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ICredentialsService credentialsService, IHeaderInfoProvider headerInfoProvider, IMetaInfoProvider metaInfoProvider, IOperationProcessor operationProcessor, IRevisionService revisionService, ICdp4JsonSerializer jsonSerializer, IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService, IModelCreatorManager modelCreatorManager)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            var reqsw = Stopwatch.StartNew();
            
            this.logger.LogInformation("{request}:{requestToken} - START HTTP REQUEST PROCESSING", postRequestData.MethodPathName, requestToken);

            try
            {
                var dbsw = Stopwatch.StartNew();
                this.logger.LogDebug("{request}:{requestToken} - Database operations started", postRequestData.MethodPathName, requestToken);

                transaction = transactionManager.SetupTransaction(ref connection, credentialsService.Credentials);

                // defer all reference data check until after transaction commit
                using (var command = new NpgsqlCommand("SET CONSTRAINTS ALL DEFERRED", transaction.Connection, transaction))
                {
                    command.ExecuteNonQuery();
                }

                // retrieve the revision for this transaction (or get next revision if it does not exist)
                var transactionRevision = revisionService.GetRevisionForTransaction(transaction, TopContainer);

                operationProcessor.Process(postRequestData.OperationData, transaction, TopContainer);

                // save revision-history
                var actor = credentialsService.Credentials.Person.Iid;
                var changedThings = revisionService.SaveRevisions(transaction, TopContainer, actor, transactionRevision).ToList();

                // commit the operation + revision-history
                await transaction.CommitAsync();

                this.CometTaskService.AddOrUpdateSuccessfulTask(cometTask, transactionRevision);

                this.logger.LogDebug("{request}:{requestToken} - Database operations completed in {sw} [ms]", postRequestData.MethodPathName, requestToken, dbsw.ElapsedMilliseconds);

                // Sends changed things to the AMQP message bus
                await this.PrepareAndQueueThingsMessage(postRequestData.OperationData, changedThings, actor, jsonSerializer);

                if (modelCreatorManager.IsUserTriggerDisable)
                {
                    // re-enable user triggers
                    transaction = transactionManager.SetupTransaction(ref connection, credentialsService.Credentials);
                    modelCreatorManager.EnableUserTrigger(transaction);
                    await transaction.CommitAsync();
                }

                if (requestUtils.QueryParameters.RevisionNumber == -1)
                {
                    switch (postRequestData.ContentType)
                    {
                        case ContentTypeKind.JSON:
                            await this.WriteJsonResponse(headerInfoProvider, metaInfoProvider, jsonSerializer, permissionInstanceFilterService, changedThings, postRequestData.Version, httpResponse);
                            break;
                        case ContentTypeKind.MESSAGEPACK:
                            await this.WriteMessagePackResponse(headerInfoProvider, messagePackSerializer, permissionInstanceFilterService, changedThings, postRequestData.Version, httpResponse);
                            break;
                    }
                    
                    return;
                }

                // get the latest revision state including revisions that may have happened meanwhile
                var fromRevision = requestUtils.QueryParameters.RevisionNumber;

                // use new transaction to include latest database state
                transaction = transactionManager.SetupTransaction(ref connection, credentialsService.Credentials);
                var revisionResponse = revisionService.Get(transaction, TopContainer, fromRevision, true).ToArray();
                await transaction.CommitAsync();

                this.logger.LogDebug("{request}:{requestToken} - Database operations completed in {sw} [ms]", postRequestData.MethodPathName, requestToken, dbsw.ElapsedMilliseconds);

                switch (postRequestData.ContentType)
                {
                    case ContentTypeKind.JSON:
                        await this.WriteJsonResponse(headerInfoProvider, metaInfoProvider, jsonSerializer, permissionInstanceFilterService, revisionResponse, postRequestData.Version, httpResponse);
                        break;
                    case ContentTypeKind.MESSAGEPACK:
                        await this.WriteMessagePackResponse(headerInfoProvider, messagePackSerializer, permissionInstanceFilterService, revisionResponse, postRequestData.Version, httpResponse);
                        break;
                }
            }
            catch (Cdp4ModelValidationException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogWarning("{request}:{requestToken} - Failed after {ElapsedMilliseconds} [ms] \n {ErrorMessage}", postRequestData.MethodPathName, requestToken, reqsw.ElapsedMilliseconds, ex.Message);

                this.CometTaskService.AddOrUpdateTask(cometTask, finishedAt: DateTime.Now, statusKind: StatusKind.FAILED, error: $"BAD REQUEST - {ex.Message}");

                httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogWarning("{request}:{requestToken} - Failed after {ElapsedMilliseconds} [ms] \n {ErrorMessage}", postRequestData.MethodPathName, requestToken, reqsw.ElapsedMilliseconds, ex.Message);

                this.CometTaskService.AddOrUpdateTask(cometTask, finishedAt: DateTime.Now, statusKind: StatusKind.FAILED, error: $"FORBIDDEN - {ex.Message}");

                httpResponse.StatusCode = (int)HttpStatusCode.Forbidden;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (BadRequestException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogWarning("{request}:{requestToken} - BadRequest failed after {ElapsedMilliseconds} [ms] \n {ErrorMessage}", postRequestData.MethodPathName, requestToken, reqsw.ElapsedMilliseconds, ex.Message);

                this.CometTaskService.AddOrUpdateTask(cometTask, finishedAt: DateTime.Now, statusKind: StatusKind.FAILED, error: $"BAD REQUEST - {ex.Message}");

                httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (InvalidDataException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogWarning("{request}:{requestToken} - InvalidData failed after {ElapsedMilliseconds} [ms] \n {ErrorMessage}", postRequestData.MethodPathName, requestToken, reqsw.ElapsedMilliseconds, ex.Message);

                // error handling
                httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (SecurityException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogWarning("{request}:{requestToken} - Unauthorized request returned after {ElapsedMilliseconds} [ms]", postRequestData.MethodPathName, requestToken, reqsw.ElapsedMilliseconds);

                this.CometTaskService.AddOrUpdateTask(cometTask, finishedAt: DateTime.Now, statusKind: StatusKind.FAILED, error: $"UNAUTHORIZED - {ex.Message}");

                httpResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (ThingNotFoundException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogWarning("{request}:{requestToken} - Unauthorized (Thing Not Found) request returned after {ElapsedMilliseconds} [ms]", postRequestData.MethodPathName, requestToken, reqsw.ElapsedMilliseconds);

                this.CometTaskService.AddOrUpdateTask(cometTask, finishedAt: DateTime.Now, statusKind: StatusKind.FAILED, error: $"UNAUTHORIZED - {ex.Message}");

                // error handling: Use Unauthorized as a user is not allowed to see if the thing is not there or a user is not allowed to see it
                httpResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (ResolveException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogWarning("{request}:{requestToken} - BadRequest (Thing Not Resolved) returned after {ElapsedMilliseconds} [ms]", postRequestData.MethodPathName, requestToken, reqsw.ElapsedMilliseconds);

                this.CometTaskService.AddOrUpdateTask(cometTask, finishedAt: DateTime.Now, statusKind: StatusKind.FAILED, error: $"BAD REQUEST - {ex.Message}");

                // error handling: Use BadRequest as the user is probably creating conflicting changes, or the data has changed server side resulting in a change that is not allowed
                httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogError(ex, "{request}:{requestToken} - Failed after {ElapsedMilliseconds} [ms]", postRequestData.MethodPathName, requestToken, reqsw.ElapsedMilliseconds);

                this.CometTaskService.AddOrUpdateTask(cometTask, finishedAt: DateTime.Now, statusKind: StatusKind.FAILED, error: $"INTERNAL SERVER ERROR - {ex.Message}");

                httpResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            finally
            {
                if (transaction != null)
                {
                    await transaction.DisposeAsync();
                }

                if (connection != null)
                {
                    await connection.DisposeAsync();
                }

                this.logger.LogInformation("{request}:{requestToken} - Response returned in {sw} [ms]", postRequestData.MethodPathName, requestToken, reqsw.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Get the SiteDirectory containment response from the request path.
        /// </summary>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <param name="transactionManager">
        /// The <see cref="ICdp4TransactionManager"/> that provides database transaction and connection services
        /// </param>
        /// <param name="serviceProvider">
        /// The <see cref="Services.IServiceProvider"/> that can be used to resolve other services
        /// </param>
        /// <param name="metaInfoProvider">
        /// The <see cref="IMetaInfoProvider"/> used to provide metadata for any kind of <see cref="Thing"/>
        /// </param>
        /// <param name="transaction">
        /// The current transaction to the database.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="routeParams">
        /// The route Parameters.
        /// </param>
        /// <returns>
        /// The list of containment <see cref="Thing"/>.
        /// </returns>
        private IEnumerable<Thing> GetContainmentResponse(IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, Services.IServiceProvider serviceProvider, IMetaInfoProvider metaInfoProvider, NpgsqlTransaction transaction, string partition, string[] routeParams)
        {
            var processor = new ResourceProcessor(transaction, serviceProvider, requestUtils, metaInfoProvider);

            if (routeParams.Length == 1)
            {
                // sitedirectory singleton resource request (IncludeReferenceData is handled in the sitedirectory service logic)

                var things = processor.GetResource(TopContainer, partition, null, new RequestSecurityContext { ContainerReadAllowed = true });

                foreach (var thing in things)
                {
                    yield return thing;
                }
            }
            else
            {
                var things = this.ProcessRequestPath(requestUtils, transactionManager, processor, TopContainer, partition, routeParams, out var resolvedResourcePath);

                foreach (var thing in things)
                {
                    yield return thing;
                }

                if (resolvedResourcePath.Count > 1 && requestUtils.QueryParameters.IncludeReferenceData)
                {
                    // add reference data information if the resource is a model reference data library
                    if (resolvedResourcePath.Last().GetType() == typeof(ModelReferenceDataLibrary))
                    {
                        foreach (var thing in this.CollectReferenceDataLibraryChain(requestUtils, processor, (ModelReferenceDataLibrary)resolvedResourcePath.Last()))
                        {
                            yield return thing;
                        }
                    }
                }
            }
        }
    }
}
