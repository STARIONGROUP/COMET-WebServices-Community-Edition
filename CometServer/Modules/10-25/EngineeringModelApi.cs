// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelApi.cs" company="Starion Group S.A.">
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

    using CometServer.Authentication.Basic;
    using CometServer.Authentication.Bearer;
    using CometServer.Authorization;
    using CometServer.Configuration;
    using CometServer.Exceptions;
    using CometServer.Extensions;
    using CometServer.Health;
    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.ChangeLog;
    using CometServer.Services.CherryPick;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations;
    using CometServer.Services.Protocol;
    using CometServer.Tasks;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using Npgsql;
    
    /// <summary>
    /// This is an API endpoint class to support interaction with the engineering model contained model data
    /// </summary>
    public class EngineeringModelApi : ApiBase
    {
        /// <summary>
        /// The top container.
        /// </summary>
        private const string TopContainer = "EngineeringModel";

        /// <summary>
        /// The (injected) <see cref="ILogger{EngineeringModelApi}"/> instance
        /// </summary>
        private readonly ILogger<EngineeringModelApi> logger;

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
            QueryParameters.RevisionToQuery,
            QueryParameters.ClassKindQuery,
            QueryParameters.CategoryQuery,
            QueryParameters.CherryPickQuery
        };

        /// <summary>
        /// The supported post query parameter.
        /// </summary>
        private static readonly string[] SupportedPostQueryParameter =
        {
            QueryParameters.RevisionNumberQuery,
            QueryParameters.WaitTimeQuery
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineeringModelApi"/> class
        /// </summary>
        /// <param name="appConfigService">
        /// The (injected) <see cref="IAppConfigService"/> used to read application configuration
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
        public EngineeringModelApi(IAppConfigService appConfigService, ICometHasStartedService cometHasStartedService, ITokenGeneratorService tokenGeneratorService, ILoggerFactory loggerFactory, IBackgroundThingsMessageProducer thingsMessageProducer, ICometTaskService cometTaskService)
            : base(appConfigService, cometHasStartedService, tokenGeneratorService, loggerFactory, thingsMessageProducer, cometTaskService)
        {
            this.logger = loggerFactory == null ? NullLogger<EngineeringModelApi>.Instance : loggerFactory.CreateLogger<EngineeringModelApi>();
        }

        /// <summary>
        /// Add the routes to the <see cref="IEndpointRouteBuilder"/>
        /// </summary>
        /// <param name="app">
        /// The <see cref="IEndpointRouteBuilder"/> to which the routes are added
        /// </param>
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("EngineeringModel/{ids:EnumerableOfGuid}", this.GetEngineeringModelsShallow).RequireAuthorization(this.AuthenticationSchemes);

            app.MapGet("EngineeringModel/*", this.GetEngineeringModelsShallow).RequireAuthorization(this.AuthenticationSchemes);

            app.MapGet("EngineeringModel/{*path}", 
                async (HttpRequest req, HttpResponse res, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ICredentialsService credentialsService, IHeaderInfoProvider headerInfoProvider, Services.IServiceProvider serviceProvider, IMetaInfoProvider metaInfoProvider, IFileBinaryService fileBinaryService, IFileArchiveService fileArchiveService, IRevisionService revisionService, IRevisionResolver revisionResolver, ICdp4JsonSerializer jsonSerializer, IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService, IObfuscationService obfuscationService, ICherryPickService cherryPickService, IContainmentService containmentService) =>
            {
                if (!await this.IsServerReady(res))
                {
                    return;
                }
                
                var identity = req.HttpContext.User.Identity!.Name;

                try
                {
                    identity = await this.Authorize(this.AppConfigService, credentialsService, req);
                }
                catch (AuthorizationException)
                {
                    this.logger.LogWarning("The GET REQUEST was not authorized for {Identity}", identity);

                    res.UpdateWithNotAutherizedSettings();
                    await res.AsJson("not authorized");
                    return;
                }

                await this.GetResponseData(req, res, requestUtils, transactionManager, credentialsService, headerInfoProvider, serviceProvider, metaInfoProvider, fileBinaryService, fileArchiveService, revisionService, revisionResolver, jsonSerializer, messagePackSerializer, permissionInstanceFilterService, obfuscationService, cherryPickService, containmentService);
            }).RequireAuthorization(this.AuthenticationSchemes);

            app.MapPost("EngineeringModel/{engineeringModelIid:guid}/iteration/{iterationIid:guid}", 
                async (HttpRequest req, HttpResponse res, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ICredentialsService credentialsService, IHeaderInfoProvider headerInfoProvider, Services.IServiceProvider serviceProvider, IMetaInfoProvider metaInfoProvider, IOperationProcessor operationProcessor, IFileBinaryService fileBinaryService, IRevisionService revisionService, ICdp4JsonSerializer jsonSerializer, IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService, IChangeLogService changeLogService) =>
            {
                if (!await this.IsServerReady(res))
                {
                    return;
                }
                
                var requestToken = this.TokenGeneratorService.GenerateRandomToken();
                var identity = req.HttpContext.User.Identity!.Name;

                try
                {
                    identity = await this.Authorize(this.AppConfigService, credentialsService, req);
                }
                catch (AuthorizationException)
                {
                    this.logger.LogWarning("The {RequestToken} POST REQUEST was not authorized for {Identity}", identity);

                    res.UpdateWithNotAutherizedSettings();
                    await res.AsJson("not authorized");
                    return;
                }

                var cometTask = this.CreateAndRegisterCometTask(credentialsService.Credentials.Person.Iid, TopContainer, requestToken);
                
                PostRequestData postRequestData = null;

                try
                {
                    postRequestData = await this.ProcessPostRequest(req, requestUtils, metaInfoProvider, jsonSerializer, fileBinaryService);
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

                    cometTask.FinishedAt = DateTime.Now;
                    cometTask.StatusKind = StatusKind.FAILED;
                    cometTask.Error = $"INTERNAL SERVER ERROR - {ex.Message}";
                    this.CometTaskService.AddOrUpdateTask(cometTask);

                    res.StatusCode = (int)HttpStatusCode.InternalServerError;
                    await res.AsJson($"exception:{ex.Message}");
                }

                if (postRequestData.IsMultiPart && requestUtils.QueryParameters.WaitTime > 0)
                {
                    this.logger.LogWarning("Request {requestToken} failed as BadRequest: a POST request may not have a wait time when it is multipart", requestToken);

                    this.CometTaskService.AddOrUpdateTask(cometTask, finishedAt: DateTime.Now, statusKind: StatusKind.FAILED, error: "BAD REQUEST - a POST request may not have a wait time when it is multipart");

                    res.StatusCode = (int)HttpStatusCode.BadRequest;
                    await res.AsJson("a POST request may not have a wait time when it is multipart");
                }

                if (requestUtils.QueryParameters.WaitTime > 0)
                {
                    await this.EnqueCometTaskForPostRequest(postRequestData, requestToken, res, cometTask, requestUtils, transactionManager, credentialsService, headerInfoProvider, serviceProvider, metaInfoProvider, operationProcessor, revisionService, jsonSerializer, messagePackSerializer, permissionInstanceFilterService, changeLogService);
                }
                else
                {
                    await this.PostResponseData(postRequestData, requestToken, res, cometTask, requestUtils, transactionManager, credentialsService, headerInfoProvider, serviceProvider, metaInfoProvider, operationProcessor, revisionService, jsonSerializer, messagePackSerializer, permissionInstanceFilterService, changeLogService);
                }
            }).RequireAuthorization(this.AuthenticationSchemes);
        }

        /// <summary>
        /// Handles the GET request to retrieve all shallow engineering models or the specified by <paramref name="ids"/>
        /// </summary>
        /// <param name="request">The HTTP request object.</param>
        /// <param name="response">The HTTP response object.</param>
        /// <param name="ids">The collection of EngineeringModel Ids</param>
        /// <param name="requestUtils">The utility for handling requests.</param>
        /// <param name="transactionManager">The transaction manager.</param>
        /// <param name="credentialsService">The service for handling credentials.</param>
        /// <param name="headerInfoProvider">The provider for header information.</param>
        /// <param name="serviceProvider">
        /// The <see cref="Services.IServiceProvider"/> that can be used to resolve other services
        /// </param>
        /// <param name="metaInfoProvider">
        /// The <see cref="IMetaInfoProvider"/> used to provide metadata for any kind of <see cref="Thing"/>
        /// </param>
        /// <param name="jsonSerializer">The JSON serializer.</param>
        /// <param name="messagePackSerializer">The MessagePack serializer.</param>
        /// <param name="permissionInstanceFilterService">The service for filtering permission instances.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task GetEngineeringModelsShallow(HttpRequest request, HttpResponse response, string ids, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, 
            ICredentialsService credentialsService, IHeaderInfoProvider headerInfoProvider, Services.IServiceProvider serviceProvider, 
            IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, 
            IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService)
        {
            if (!await this.IsServerReady(response))
            {
                return;
            }

            var identity = request.HttpContext.User.Identity!.Name;
            
            try
            {
               identity = await this.Authorize(this.AppConfigService, credentialsService, request);
            }
            catch (AuthorizationException)
            {
                this.logger.LogWarning("The GET REQUEST was not authorized for {Identity}", identity);
                response.UpdateWithNotAutherizedSettings();
                await response.AsJson("not authorized");
                return;
            }
            
            NpgsqlConnection connection = null;
            var credentials = credentialsService.Credentials;
            var transaction = transactionManager.SetupTransaction(ref connection, credentials);

            transactionManager.SetCachedDtoReadEnabled(true);
            transactionManager.SetFullAccessState(true);

            var stopwatch = Stopwatch.StartNew();
            var requestToken = this.TokenGeneratorService.GenerateRandomToken();

            this.logger.LogInformation("{request}:{requestToken} - START HTTP REQUEST PROCESSING", request.QueryNameMethodPath(), requestToken);
            
            try
            {
                var engineeringModels = new List<Thing>();
                var allEngineeringModelIds = new List<Guid>();

                var processor = new ResourceProcessor(transaction, serviceProvider, requestUtils, metaInfoProvider);
                requestUtils.OverrideQueryParameters = new QueryParameters() { ExtentDeep = false };

                var securityContext = new RequestSecurityContext();

                // get all the Participants and filter out only those Participants that are active and for the current Person
                var activeAndAssignedParticipantsForCurrentPerson = processor.GetResource("Participant", SiteDirectoryData, null, securityContext)
                    .OfType<Participant>().Where(x => x.IsActive && x.Person == credentials.Person.Iid);

                // get all EngineeringModelSetup
                var engineeringModelSetups = processor.GetResource("EngineeringModelSetup", SiteDirectoryData, null, securityContext)
                    .OfType<EngineeringModelSetup>();

                if (ids is null)
                {
                    foreach (var participant in activeAndAssignedParticipantsForCurrentPerson)
                    {
                        foreach (var engineeringModelSetup in engineeringModelSetups)
                        {
                            if (engineeringModelSetup.Participant.Contains(participant.Iid))
                            {
                                allEngineeringModelIds.Add(engineeringModelSetup.EngineeringModelIid);
                            }
                        }
                    }
                }
                else if (ids.TryParseEnumerableOfGuid(out var identifiers))
                {
                    foreach (var participant in activeAndAssignedParticipantsForCurrentPerson)
                    {
                        foreach (var engineeringModelSetup in engineeringModelSetups)
                        {
                            if (engineeringModelSetup.Participant.Contains(participant.Iid) && identifiers.Contains(engineeringModelSetup.EngineeringModelIid))
                            {
                                allEngineeringModelIds.Add(engineeringModelSetup.EngineeringModelIid);
                            }
                        }
                    }
                }

                foreach (var engineeringModelIid in allEngineeringModelIds.Distinct())
                {
                    var partition = requestUtils.GetEngineeringModelPartitionString(engineeringModelIid);

                    var things = this.ProcessRequestPath(requestUtils, transactionManager, processor, TopContainer, partition,
                        new[] { nameof(EngineeringModel), engineeringModelIid.ToString() }, out _);

                    engineeringModels.AddRange(things);
                }

                var contentTypeKind = request.QueryContentTypeKind();
                var version = request.QueryDataModelVersion();

                if (contentTypeKind == ContentTypeKind.JSON)
                {
                    await this.WriteJsonResponse(headerInfoProvider, metaInfoProvider, jsonSerializer, permissionInstanceFilterService, engineeringModels, version, response, HttpStatusCode.OK, requestToken);
                }
                else if (contentTypeKind == ContentTypeKind.MESSAGEPACK)
                {
                    await this.WriteMessagePackResponse(headerInfoProvider, messagePackSerializer, permissionInstanceFilterService, engineeringModels, version, response, HttpStatusCode.OK, requestToken);
                }
                else
                {
                    throw new NotSupportedException($"ContentType {contentTypeKind} is not supported for this request");
                }
            }
            catch (SecurityException exception)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogWarning("{request}:{requestToken} - Unauthorized request returned after {ElapsedMilliseconds} [ms]", request.QueryNameMethodPath(), requestToken, stopwatch.ElapsedMilliseconds);

                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await response.AsJson($"exception:{exception.Message}");
            }
            catch (ThingNotFoundException exception)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogWarning("{request}:{requestToken} - Unauthorized (Thing Not Found) request returned after {ElapsedMilliseconds} [ms]", request.QueryNameMethodPath(), requestToken, stopwatch.ElapsedMilliseconds);

                response.StatusCode = (int)HttpStatusCode.NotFound;
                await response.AsJson($"exception:{exception.Message}");
            }
            catch (Exception exception)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogError(exception, "{request}:{requestToken} - Failed after {ElapsedMilliseconds} [ms]", request.QueryNameMethodPath(), requestToken, stopwatch.ElapsedMilliseconds);

                // error handling
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await response.AsJson($"exception:{exception.Message}");
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

                this.logger.LogInformation("{request}:{requestToken} - Response returned in {sw} [ms]", request.QueryNameMethodPath(), requestToken, stopwatch.ElapsedMilliseconds);
            }
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
        /// <param name="revisionService">
        /// The <see cref="IRevisionService"/> that supports revision based retrievel of data
        /// </param>
        /// <param name="revisionResolver">
        /// The <see cref="IRevisionResolver"/> used to resolve the revision numbers that belong to the revisionFrom and revisionTo parameters.
        /// </param>
        /// <param name="jsonSerializer">
        /// The <see cref="ICdp4JsonSerializer"/> used to serialize data to JSOIN
        /// </param>
        /// <param name="messagePackSerializer"></param>
        /// <param name="permissionInstanceFilterService">
        /// The <see cref="IPermissionInstanceFilterService"/> used to filter instances from the queried data
        /// </param>
        /// <param name="serviceProvider">
        /// The <see cref="Services.IServiceProvider"/> that can be used to resolve other services
        /// </param>
        /// <param name="metaInfoProvider">
        /// The <see cref="IMetaInfoProvider"/> used to provide metadata for any kind of <see cref="Thing"/>
        /// </param>
        /// <param name="fileBinaryService">
        /// The (injected)  <see cref="IFileBinaryService"/> used to operate on files
        /// </param>
        /// <param name="fileArchiveService">
        /// The (injected) <see cref="IFileArchiveService"/> to store and retrieve files to and from the filesystem
        /// </param>
        /// <param name="obfuscationService">
        /// The obfuscation service that obscures properties and children of Element Definitions based on OrganizationalParticipation of
        /// an EngineeringModelSetup
        /// </param>
        /// <param name="cherryPickService">
        /// The <see cref="ICherryPickService"/> provides capabilities on cherry picking <see cref="CDP4Common.DTO.Thing"/> inside an <see cref="Iteration"/>
        /// </param>
        /// <param name="containmentService">
        /// The <see cref="IContainmentService"/> provides capabilities to retrieve containment on <see cref="CDP4Common.DTO.Thing"/>
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/>
        /// </returns>
        protected async Task GetResponseData(HttpRequest httpRequest, HttpResponse httpResponse, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ICredentialsService credentialsService, IHeaderInfoProvider headerInfoProvider, Services.IServiceProvider serviceProvider, IMetaInfoProvider metaInfoProvider, IFileBinaryService fileBinaryService, IFileArchiveService fileArchiveService, IRevisionService revisionService, IRevisionResolver revisionResolver, ICdp4JsonSerializer jsonSerializer, IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService, IObfuscationService obfuscationService, ICherryPickService cherryPickService, IContainmentService containmentService)
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

                var version = httpRequest.QueryDataModelVersion();

                // the route pattern enforces that there is at least one route segment
                var routeSegments = HttpRequestHelper.ParseRouteSegments(httpRequest.Path);

                var resourceResponse = new List<Thing>();
                var fromRevision = requestUtils.QueryParameters.RevisionNumber;
                var iterationContextId = Guid.Empty;

                var iterationContextRequest = routeSegments.Length >= 4 &&
                                              routeSegments[2] == "iteration" &&
                                              Guid.TryParse(routeSegments[3], out iterationContextId);

                // get prepared data source transaction
                var credentials = credentialsService.Credentials;

                var dbsw = Stopwatch.StartNew();
                this.logger.LogDebug("{request}:{requestToken} - Database operations started", httpRequest.QueryNameMethodPath(), requestToken);

                transaction = iterationContextRequest
                    ? transactionManager.SetupTransaction(ref connection, credentials, iterationContextId)
                    : transactionManager.SetupTransaction(ref connection, credentials);

                var processor = new ResourceProcessor(transaction, serviceProvider, requestUtils, metaInfoProvider);
                var modelSetup = DetermineEngineeringModelSetup(requestUtils, transactionManager, processor, routeSegments);
                var partition = requestUtils.GetEngineeringModelPartitionString(modelSetup.EngineeringModelIid);

                // set the participant information
                if (credentials != null)
                {
                    credentials.EngineeringModelSetup = modelSetup;
                    credentialsService.ResolveParticipantCredentials(transaction);
                }

                if (fromRevision > -1)
                {
                    // gather all Things that are newer then the indicated revision
                    resourceResponse.AddRange(revisionService.Get(transaction, partition, fromRevision, false));
                }
                else if (revisionResolver.TryResolve(transaction, partition, requestUtils.QueryParameters.RevisionFrom, requestUtils.QueryParameters.RevisionTo, out var resolvedValues))
                {
                    var iid = routeSegments.Last();

                    if (!Guid.TryParse(iid, out var guid))
                    {
                        httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                        await httpResponse.AsJson("The identifier of the object to query was not found or the route is invalid.");
                        return;
                    }

                    resourceResponse.AddRange(revisionService.Get(transaction, partition, guid, resolvedValues.FromRevision, resolvedValues.ToRevision));
                }
                else
                {
                    if (requestUtils.QueryParameters.CherryPick)
                    {
                        requestUtils.QueryParameters.ExtentDeep = true;
                        requestUtils.QueryParameters.IncludeReferenceData = true;
                    }

                    if (routeSegments.Length == 4 && routeSegments[2] == "iteration" && requestUtils.QueryParameters.ExtentDeep)
                    {
                        string[] engineeringModelRouteSegments = { routeSegments[0], routeSegments[1] };

                        // gather all Things at engineeringmodel level as indicated by the request URI 
                        resourceResponse.AddRange(this.GetContainmentResponse(requestUtils, transactionManager, processor, partition, modelSetup, engineeringModelRouteSegments));

                        // find and remove the engineeringModelInstance, that will be retrieved in the second go.
                        var engineeringModel = resourceResponse.SingleOrDefault(x => x.ClassKind == CDP4Common.CommonData.ClassKind.EngineeringModel);
                        resourceResponse.Remove(engineeringModel);
                    }

                    // gather all Things as indicated by the request URI 
                    resourceResponse.AddRange(this.GetContainmentResponse(requestUtils, transactionManager, processor, partition, modelSetup, routeSegments));

                    if (resourceResponse.Count == 0)
                    {
                        httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                        await httpResponse.AsJson("The identifier of the object to query was not found or the route is invalid.");
                        return;
                    }
                }

                if (requestUtils.QueryParameters.CherryPick)
                {
                    if (routeSegments.Length != 4 || routeSegments[2] != "iteration")
                    {
                        httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                        await httpResponse.AsJson($"The {QueryParameters.CherryPickQuery} feature is only available when reading an iteration");
                        return;
                    }

                    if (!requestUtils.QueryParameters.ClassKinds.Any() || !requestUtils.QueryParameters.CategoriesId.Any())
                    {
                        httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                        await httpResponse.AsJson($"The {QueryParameters.ClassKindQuery} and {QueryParameters.CategoryQuery} parameters are required to use {QueryParameters.CherryPickQuery}");
                        return;
                    }

                    resourceResponse = CherryPick(requestUtils, cherryPickService, containmentService, resourceResponse);
                }
                else if (requestUtils.QueryParameters.ClassKinds.Any())
                {
                    httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    await httpResponse.AsJson($"The {QueryParameters.ClassKindQuery} parameters can only be used with {QueryParameters.CherryPickQuery} enabled");
                    return;
                }
                else if (requestUtils.QueryParameters.CategoriesId.Any())
                {
                    httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    await httpResponse.AsJson($"The {QueryParameters.CategoryQuery} parameters can only be used with {QueryParameters.CherryPickQuery} enabled");
                    return;
                }

                await transaction.CommitAsync();

                this.logger.LogDebug("{request}:{requestToken} - Database operations completed in {sw} [ms]", httpRequest.QueryNameMethodPath(), requestToken, dbsw.ElapsedMilliseconds);

                // obfuscate if needed
                if (modelSetup.OrganizationalParticipant.Count != 0)
                {
                    var obfsw = Stopwatch.StartNew();
                    
                    this.logger.LogInformation("{request}:{requestToken} - Model has Organizational Participation assigned. Obfuscation enabled.", httpRequest.QueryNameMethodPath(), requestToken);

                    obfuscationService.ObfuscateResponse(resourceResponse, credentials);

                    obfsw.Stop();
                    this.logger.LogInformation("{request}:{requestToken} - Obfuscation completed in {sw} [ms]", httpRequest.QueryNameMethodPath(), requestToken, obfsw.ElapsedMilliseconds);
                }

                var fileRevisions = resourceResponse.OfType<FileRevision>().ToList();

                if (requestUtils.QueryParameters.IncludeFileData && fileRevisions.Any())
                {
                    // return multipart response including file binaries
                    this.WriteMultipartResponse(headerInfoProvider, metaInfoProvider, jsonSerializer, fileBinaryService, permissionInstanceFilterService, fileRevisions, resourceResponse, version, httpResponse);
                    return;
                }

                if (requestUtils.QueryParameters.IncludeFileData)
                {
                    var routeSegmentList = routeSegments.ToList();

                    if (this.IsValidDomainFileStoreArchiveRoute(routeSegmentList))
                    {
                        var iterationPartition = requestUtils.GetIterationPartitionString(modelSetup.EngineeringModelIid);

                        this.WriteArchivedResponse(headerInfoProvider, metaInfoProvider, jsonSerializer, fileArchiveService, permissionInstanceFilterService, resourceResponse, iterationPartition, routeSegments, version, httpResponse);
                        return;
                    }

                    if (this.IsValidCommonFileStoreArchiveRoute(routeSegmentList))
                    {
                        this.WriteArchivedResponse(headerInfoProvider, metaInfoProvider, jsonSerializer, fileArchiveService, permissionInstanceFilterService, resourceResponse, partition, routeSegments, version, httpResponse);
                        return;
                    }
                }

                switch (contentTypeKind)
                {
                    case ContentTypeKind.JSON:
                        await this.WriteJsonResponse(headerInfoProvider, metaInfoProvider, jsonSerializer, permissionInstanceFilterService, resourceResponse, version, httpResponse, HttpStatusCode.OK, requestToken);
                        break;
                    case ContentTypeKind.MESSAGEPACK:
                        await this.WriteMessagePackResponse(headerInfoProvider, messagePackSerializer, permissionInstanceFilterService, resourceResponse, version, httpResponse, HttpStatusCode.OK, requestToken);
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
        /// Cherry Picks <see cref="Thing" />s inside read <see cref="Thing"/>s based on provided <see cref="CDP4Common.CommonData.ClassKind"/> and <see cref="Category"/> filters
        /// </summary>
        /// <param name="cherryPickService">
        /// The <see cref="ICherryPickService"/> provides capabilities on cherry picking <see cref="CDP4Common.DTO.Thing"/> inside an <see cref="Iteration"/>
        /// </param>
        /// <param name="containmentService">
        /// The <see cref="IContainmentService"/> provides capabilities to retrieve containment on <see cref="CDP4Common.DTO.Thing"/>
        /// </param>
        /// <param name="resourceResponse">
        /// A collection of read <see cref="Thing"/>s
        /// </param>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <returns></returns>
        private static List<Thing> CherryPick(IRequestUtils requestUtils, ICherryPickService cherryPickService, IContainmentService containmentService, IReadOnlyList<Thing> resourceResponse)
        {
            var cherryPickedThings = cherryPickService.CherryPick(resourceResponse, requestUtils.QueryParameters.ClassKinds, requestUtils.QueryParameters.CategoriesId)
                .ToList();

            var containedThings = containmentService.QueryContainedThings(cherryPickedThings, resourceResponse, true,
                CDP4Common.CommonData.ClassKind.Parameter, CDP4Common.CommonData.ClassKind.ParameterOverride, CDP4Common.CommonData.ClassKind.ParameterValueSet, CDP4Common.CommonData.ClassKind.ParameterOverrideValueSet);

            var containers = containmentService.QueryContainersTree(cherryPickedThings, resourceResponse);

            cherryPickedThings.AddRange(containedThings);
            cherryPickedThings.AddRange(containers);
            return cherryPickedThings.DistinctBy(x => x.Iid).ToList();
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
        /// <param name="fileBinaryService">
        /// The (injected)  <see cref="IFileBinaryService"/> used to operate on files
        /// </param>
        /// <returns>
        /// An instance of <see cref="PostRequestData"/>
        /// </returns>
        protected async Task< PostRequestData> ProcessPostRequest(HttpRequest httpRequest, IRequestUtils requestUtils, IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, IFileBinaryService fileBinaryService)
        {
            HttpRequestHelper.ValidateSupportedQueryParameter(httpRequest.Query, SupportedPostQueryParameter);
            var queryParameters = httpRequest.Query.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value.FirstOrDefault());
            requestUtils.QueryParameters = new QueryParameters(queryParameters);

            var postRequestData = new PostRequestData
            {
                ContentType = httpRequest.QueryContentTypeKind(),
                MethodPathName = httpRequest.QueryNameMethodPath(),
                Version = httpRequest.QueryDataModelVersion(),
                IsMultiPart = httpRequest.GetMultipartBoundary() != string.Empty,
                MultiPartBoundary = httpRequest.GetMultipartBoundary(),
                Path = httpRequest.Path
            };

            Stream bodyStream;
            if (postRequestData.IsMultiPart)
            {
                var requestStream = new MemoryStream();
                await httpRequest.Body.CopyToAsync(requestStream);

                bodyStream = await this.ExtractJsonBodyStreamFromMultiPartMessage(requestStream, postRequestData.MultiPartBoundary);
                postRequestData.Files = await ExtractFilesFromMultipartMessage(fileBinaryService, requestStream, postRequestData.MultiPartBoundary);

                // - New File: 
                //      create -> File, FileRevision
                //      update -> FileStore

                // - New FileRevision:
                //      create -> FileRevision
                //      update -> File

                // - Update File:
                //      update -> File

                // - ANY Filerevision metadata object in operation without contentHash -> ERROR.
                // - NO matching file hash found in request body and NO matching file hash found on server -> ERROR
                // - Matching file hash found in request body ??? is allready checked to find binary in request...
                // - ANY file binary in request without corresponding metadata -> ERROR
            }
            else
            {
                // use the single request (JSON) stream
                bodyStream = httpRequest.Body;
            }

            jsonSerializer.Initialize(metaInfoProvider, postRequestData.Version);
            postRequestData.OperationData = jsonSerializer.Deserialize<CdpPostOperation>(bodyStream);

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
        /// <param name="serviceProvider">
        /// The <see cref="Services.IServiceProvider"/> that can be used to resolve other services
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
        /// <param name="changeLogService">
        /// Creates changelog data based on the changes made to certain <see cref="Thing"/>s.
        /// </param>
        /// <returns>
        /// an awaitable <see cref="Task"/>
        /// </returns>
        protected async Task EnqueCometTaskForPostRequest(PostRequestData postRequestData, string requestToken, HttpResponse httpResponse, CometTask cometTask, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ICredentialsService credentialsService, IHeaderInfoProvider headerInfoProvider, Services.IServiceProvider serviceProvider, IMetaInfoProvider metaInfoProvider, IOperationProcessor operationProcessor, IRevisionService revisionService, ICdp4JsonSerializer jsonSerializer, IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService, IChangeLogService changeLogService)
        {
            var longRunningCometTask = Task.Run(() =>
            {
                var task = this.PostResponseData(postRequestData, requestToken, httpResponse, cometTask, requestUtils, transactionManager, credentialsService, headerInfoProvider, serviceProvider, metaInfoProvider, operationProcessor, revisionService, jsonSerializer, messagePackSerializer, permissionInstanceFilterService, changeLogService);
                this.logger.LogTrace(task.IsCompletedSuccessfully.ToString());
            });

            var completed = longRunningCometTask.Wait(TimeSpan.FromSeconds(requestUtils.QueryParameters.WaitTime));

            if (completed)
            {
                this.logger.LogInformation("The task {id} for actor {actor} completed within the requsted wait time {waittime}", cometTask.Id, cometTask.Actor, requestUtils.QueryParameters.WaitTime);
            }
            else
            {
                this.logger.LogInformation("The task {id} for actor {actor} did not complete within the requsted wait time {waittime}; the CometTask is returned.", cometTask.Id, cometTask.Actor, requestUtils.QueryParameters.WaitTime);

                await httpResponse.AsJson(cometTask);
            }
        }

        /// <summary>
        /// Handles the POST requset
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
        /// <param name="operationProcessor">
        /// The <see cref="IOperationProcessor"/> used to process the POST operation
        /// </param>
        /// <param name="revisionService">
        /// The <see cref="IRevisionService"/> that supports revision based retrievel of data
        /// </param>
        /// <param name="jsonSerializer">
        /// The <see cref="ICdp4JsonSerializer"/> used to serialize data to JSOIN
        /// </param>
        /// <param name="messagePackSerializer"></param>
        /// <param name="permissionInstanceFilterService">
        /// The <see cref="IPermissionInstanceFilterService"/> used to filter instances from the queried data
        /// </param>
        /// <param name="serviceProvider">
        /// The <see cref="Services.IServiceProvider"/> that can be used to resolve other services
        /// </param>
        /// <param name="metaInfoProvider">
        /// The <see cref="IMetaInfoProvider"/> used to provide metadata for any kind of <see cref="Thing"/>
        /// </param>
        /// <param name="changeLogService"></param>
        /// <returns>
        /// An awaitable <see cref="Task"/>
        /// </returns>
        protected async Task PostResponseData(PostRequestData postRequestData, string requestToken, HttpResponse httpResponse, CometTask cometTask, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ICredentialsService credentialsService, IHeaderInfoProvider headerInfoProvider, Services.IServiceProvider serviceProvider, IMetaInfoProvider metaInfoProvider, IOperationProcessor operationProcessor, IRevisionService revisionService, ICdp4JsonSerializer jsonSerializer, IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService, IChangeLogService changeLogService)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            var reqsw = Stopwatch.StartNew();

            this.logger.LogInformation("{request}:{requestToken} - START HTTP REQUEST PROCESSING", postRequestData.MethodPathName.Sanitize(), requestToken);

            try
            {
                this.logger.LogDebug("{request} - Request {requestToken} is mutlipart: {isMultiPart}", postRequestData.MethodPathName.Sanitize(), requestToken, postRequestData.IsMultiPart);

                var dbsw = Stopwatch.StartNew();
                this.logger.LogDebug("{request}:{requestToken} - Database operations started", postRequestData.MethodPathName.Sanitize(), requestToken);

                // get prepared data source transaction
                var credentials = credentialsService.Credentials;
                transaction = transactionManager.SetupTransaction(ref connection, credentials);

                // the route pattern enforces that there is atleast one route segment
                var routeSegments = HttpRequestHelper.ParseRouteSegments(postRequestData.Path);

                var resourceProcessor = new ResourceProcessor(transaction, serviceProvider, requestUtils, metaInfoProvider);

                var modelSetup = DetermineEngineeringModelSetup(requestUtils, transactionManager, resourceProcessor, routeSegments);

                var partition = requestUtils.GetEngineeringModelPartitionString(modelSetup.EngineeringModelIid);

                if (credentials != null)
                {
                    credentialsService.Credentials.EngineeringModelSetup = modelSetup;
                    credentialsService.ResolveParticipantCredentials(transaction);
                    credentialsService.Credentials.IsParticipant = true;

                    var iteration = DetermineIteration(resourceProcessor, partition, routeSegments);

                    if (!IsPostAllowedOnIterationSetup(resourceProcessor, iteration))
                    {
                        this.AddErrorTagHeader(httpResponse, "#FROZEN_ITERATION");

                        throw new Cdp4ModelValidationException($"It is not allowed to write data to a Frozen {nameof(IterationSetup)}, or its full containment tree.");
                    }

                    credentialsService.Credentials.Iteration = iteration;
                }

                // defer all reference data check until after transaction commit
                using (var command = new NpgsqlCommand("SET CONSTRAINTS ALL DEFERRED;", transaction.Connection, transaction))
                {
                    command.ExecuteNonQuery();
                }

                // retrieve the revision for this transaction (or get next revision if it does not exist)
                var transactionRevision = revisionService.GetRevisionForTransaction(transaction, partition);

                operationProcessor.Process(postRequestData.OperationData, transaction, partition, postRequestData.Files);

                var actor = credentialsService.Credentials.Person.Iid;

                if (this.AppConfigService.AppConfig.Changelog.CollectChanges)
                {
                    var initiallyChangedThings = revisionService.GetCurrentChanges(transaction, partition, transactionRevision, true).ToList();
                    changeLogService?.TryAppendModelChangeLogData(transaction, partition, actor, transactionRevision, postRequestData.OperationData, initiallyChangedThings);
                }

                // save revision-history
                var changedThings = revisionService.SaveRevisions(transaction, partition, actor, transactionRevision).ToList();
                
                await transaction.CommitAsync();

                // Sends changed things to the AMQP message bus
                await this.PrepareAndQueueThingsMessage(postRequestData.OperationData, changedThings, actor, jsonSerializer);

                this.CometTaskService.AddOrUpdateSuccessfulTask(cometTask, transactionRevision);

                this.logger.LogDebug("{request}:{requestToken} - Database operations completed in {sw} [ms]", postRequestData.MethodPathName.Sanitize(), requestToken, dbsw.ElapsedMilliseconds);

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

                var fromRevision = requestUtils.QueryParameters.RevisionNumber;

                // use new transaction to include latest database state
                transaction = transactionManager.SetupTransaction(ref connection, credentials);
                var revisionResponse = revisionService.Get(transaction, partition, fromRevision, true).ToArray();

                await transaction.CommitAsync();

                this.logger.LogDebug("{request}:{requestToken} - Database operations completed in {sw} [ms]", postRequestData.MethodPathName.Sanitize(), requestToken, dbsw.ElapsedMilliseconds);

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

                this.logger.LogWarning("{request}:{requestToken} - Failed after {ElapsedMilliseconds} [ms] \n {ErrorMessage}", postRequestData.MethodPathName.Sanitize(), requestToken, reqsw.ElapsedMilliseconds, ex.Message);

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

                this.logger.LogWarning("{request}:{requestToken} - Failed after {ElapsedMilliseconds} [ms] \n {ErrorMessage}", postRequestData.MethodPathName.Sanitize(), requestToken, reqsw.ElapsedMilliseconds, ex.Message);

                this.CometTaskService.AddOrUpdateTask(cometTask, finishedAt: DateTime.Now, statusKind: StatusKind.FAILED, error: $"FORBIDDEN - {ex.Message}");

                // error handling
                httpResponse.StatusCode = (int)HttpStatusCode.Forbidden;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (BadRequestException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogWarning("{request}:{requestToken} - BadRequest failed after {ElapsedMilliseconds} [ms] \n {ErrorMessage}", postRequestData.MethodPathName.Sanitize(), requestToken, reqsw.ElapsedMilliseconds, ex.Message);

                this.CometTaskService.AddOrUpdateTask(cometTask, finishedAt: DateTime.Now, statusKind: StatusKind.FAILED, error: $"BAD REQUEST - {ex.Message}");

                // error handling
                httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (InvalidDataException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogWarning("{request}:{requestToken} - InvalidData failed after {ElapsedMilliseconds} [ms] \n {ErrorMessage}", postRequestData.MethodPathName.Sanitize(), requestToken, reqsw.ElapsedMilliseconds, ex.Message);

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

                this.logger.LogWarning("{request}:{requestToken} - Unauthorized request returned after {ElapsedMilliseconds} [ms]", postRequestData.MethodPathName.Sanitize(), requestToken, reqsw.ElapsedMilliseconds);

                this.CometTaskService.AddOrUpdateTask(cometTask, finishedAt: DateTime.Now, statusKind: StatusKind.FAILED, error: $"UNAUTHORIZED - {ex.Message}");

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

                this.logger.LogWarning("{request}:{requestToken} - Unauthorized (Thing Not Found) request returned after {ElapsedMilliseconds} [ms]", postRequestData.MethodPathName.Sanitize(), requestToken, reqsw.ElapsedMilliseconds);

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

                this.logger.LogWarning("{request}:{requestToken} - BadRequest (Thing Not Resolved) returned after {ElapsedMilliseconds} [ms]", postRequestData.MethodPathName.Sanitize(), requestToken, reqsw.ElapsedMilliseconds);

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

                this.logger.LogError(ex, "{request}:{requestToken} - Failed after {ElapsedMilliseconds} [ms]", postRequestData.MethodPathName.Sanitize(), requestToken, reqsw.ElapsedMilliseconds);

                this.CometTaskService.AddOrUpdateTask(cometTask, finishedAt: DateTime.Now, statusKind: StatusKind.FAILED, error: $"INTERNAL SERVER ERROR - {ex.Message}");

                // error handling
                httpResponse.StatusCode = (int) HttpStatusCode.InternalServerError;
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

                this.logger.LogInformation("{request}:{requestToken} - Response returned in {sw} [ms]", postRequestData.MethodPathName.Sanitize(), requestToken, reqsw.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Extracts the JSON part from the multi-part message
        /// </summary>
        /// <returns>
        /// A <see cref="MemoryStream"/> that contains the posted JSON
        /// </returns>
        private async Task<Stream> ExtractJsonBodyStreamFromMultiPartMessage(MemoryStream stream, string boundary)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var multipartReader = new MultipartReader(boundary, stream);
            
            var section = await multipartReader.ReadNextSectionAsync();

            while (section != null)
            {
                if (section.ContentType == "application/json")
                {
                    var bodyStream = section.Body;

                    if (this.logger.IsEnabled(LogLevel.Trace))
                    {
                        var streamReader = new StreamReader(bodyStream);
                        var multipartjson = await streamReader.ReadToEndAsync();
                        bodyStream.Position = 0;

                        this.logger.LogTrace("multipart post JSON: {multipartjson}", multipartjson);
                    }

                    return bodyStream;
                }
            }

            throw new InvalidOperationException("A multipart request must contain a JSON part");
        }

        /// <summary>
        /// Extracts the files from the multi-part message
        /// </summary>
        /// <param name="fileBinaryService">
        /// The (injected)  <see cref="IFileBinaryService"/> used to operate on files
        /// </param>
        /// <param name="stream">
        /// The <see cref="MemoryStream"/> that contains the multi-part messsage
        /// </param>
        /// <param name="boundary">
        /// The multipart boundary string.
        /// </param>
        /// <returns>
        /// A <see cref="MemoryStream"/> that contains the posted multipart message
        /// </returns>
        private static async Task<Dictionary<string, Stream>> ExtractFilesFromMultipartMessage(IFileBinaryService fileBinaryService, MemoryStream stream, string boundary)
        {
            var fileDictionary = new Dictionary<string, Stream>();

            stream.Seek(0, SeekOrigin.Begin);
            var multipartReader = new MultipartReader(boundary, stream);

            var section = await multipartReader.ReadNextSectionAsync();

            while (section != null)
            {
                if (section.ContentType == HttpConstants.MimeTypeOctetStream)
                {
                    var hash = fileBinaryService.CalculateHashFromStream(section.Body);

                    fileDictionary.Add(hash, section.Body);
                }

                section = await multipartReader.ReadNextSectionAsync();
            }

            return fileDictionary;
        }

        /// <summary>
        /// Get the EngineeringModel containment response from the request path.
        /// </summary>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <param name="transactionManager">
        /// The <see cref="ICdp4TransactionManager"/> that provides database transaction and connection services
        /// </param>
        /// <param name="resourceProcessor">
        /// The resource Processor.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="modelSetup">
        /// The current engineering model setup.
        /// </param>
        /// <param name="routeSegments">
        /// The route segments of the request path.
        /// </param>
        /// <returns>
        /// The list of containment <see cref="Thing"/>.
        /// </returns>
        private IEnumerable<Thing> GetContainmentResponse(IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IProcessor resourceProcessor, string partition, EngineeringModelSetup modelSetup, string[] routeSegments)
        {
            foreach (var thing in this.ProcessRequestPath(requestUtils, transactionManager, resourceProcessor, TopContainer, partition, routeSegments, out _))
            {
                yield return thing;
            }

            if (requestUtils.QueryParameters.IncludeReferenceData)
            {
                transactionManager.SetDefaultContext(resourceProcessor.Transaction);

                foreach (var thing in this.CollectReferenceDataLibraryChain(requestUtils, resourceProcessor, modelSetup))
                {
                    yield return thing;
                }
            }
        }

        /// <summary>
        /// Determine the engineering model setup based on the supplied routeSegments.
        /// </summary>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <param name="transactionManager">
        /// The <see cref="ICdp4TransactionManager"/> that provides database transaction and connection services
        /// </param>
        /// <param name="processor">
        /// The processor instance.
        /// </param>
        /// <param name="routeSegments">
        /// The route segments constructed from the request path.
        /// </param>
        /// <returns>
        /// The resolved <see cref="EngineeringModelSetup"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// If engineering model could not be resolved
        /// </exception>
        private static EngineeringModelSetup DetermineEngineeringModelSetup(IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ResourceProcessor processor, string[] routeSegments)
        {
            // override query parameters to return only extent shallow
            requestUtils.OverrideQueryParameters = new QueryParameters();

            var securityContext = new RequestSecurityContext { ContainerReadAllowed = true };

            // set the transaction to default context to retrieve SiteDirectory data
            transactionManager.SetDefaultContext(processor.Transaction);

            // take first segment and try to resolve the engineering model setup for further processing
            var siteDir = (SiteDirectory)processor.GetResource("SiteDirectory", SiteDirectoryData, null, securityContext).Single();
            var requestedModelId = routeSegments[1].ParseIdentifier();
            var engineeringModelSetups = processor.GetResource("EngineeringModelSetup", SiteDirectoryData, siteDir.Model, securityContext);
            var modelSetups = engineeringModelSetups.Where(x => ((EngineeringModelSetup)x).EngineeringModelIid == requestedModelId).ToList();

            if (modelSetups.Count != 1)
            {
                throw new ThingNotFoundException($"EngineeringModelSetup representing EngineeringModel {requestedModelId} could not be resolved");
            }

            // override query parameters to return only extent shallow
            requestUtils.OverrideQueryParameters = null;

            return (EngineeringModelSetup)modelSetups.Single();
        }

        /// <summary>
        /// Determine the iteration based on the supplied routeSegments.
        /// </summary>
        /// <param name="processor">
        /// The processor instance.
        /// </param>
        /// <param name="partition">The partition of the search.</param>
        /// <param name="routeSegments">
        /// The route segments constructed from the request path.
        /// </param>
        /// <returns>
        /// The resolved <see cref="Iteration"/>.
        /// </returns>
        private static Iteration DetermineIteration(ResourceProcessor processor, string partition, string[] routeSegments)
        {
            if (routeSegments.Length >= 4 && routeSegments[2] == "iteration")
            {
                var securityContext = new RequestSecurityContext { ContainerReadAllowed = true };

                var requestedIterationId = routeSegments[3].ParseIdentifier();
                var iterations = processor.GetResource("Iteration", partition, new List<Guid> { requestedIterationId }, securityContext).ToList();

                if (iterations.Count != 1)
                {
                    throw new ThingNotFoundException($"Iteration {requestedIterationId} could not be resolved");
                }

                return (Iteration)iterations.Single();
            }

            return null;
        }

        /// <summary>
        /// Determine is a Post request is allowed based on the <see cref="IterationSetup"/>s data.
        /// </summary>
        /// <param name="processor">
        /// The processor instance.
        /// </param>
        /// <param name="iteration">
        /// The <see cref="Iteration"/>
        /// </param>
        /// <returns>
        /// A boolean indication if writing data to an <see cref="Iteration"/> is allowed
        /// </returns>
        private static bool IsPostAllowedOnIterationSetup(ResourceProcessor processor, Iteration iteration)
        {
            var securityContext = new RequestSecurityContext { ContainerReadAllowed = true };

            var partition = "SiteDirectory";
            var requestedIterationSetupId = iteration.IterationSetup;
            var iterationSetups = processor.GetResource("IterationSetup", partition, new List<Guid> { requestedIterationSetupId }, securityContext).OfType<IterationSetup>().ToList();

            if (iterationSetups.Count != 1)
            {
                throw new ThingNotFoundException($"IterationSetup {requestedIterationSetupId} could not be resolved");
            }

            var iterationSetup = iterationSetups.First();

            return !iterationSetup.FrozenOn.HasValue;
        }

        /// <summary>
        /// Checks if a route is a valid route for returning a filearchive response for a <see cref="DomainFileStore"/>
        /// </summary>
        /// <param name="routeSegmentList"><see cref="IEnumerable{String}"/> that contains the route segments</param>
        /// <returns>True if the route is valid, otherwise false</returns>
        public bool IsValidDomainFileStoreArchiveRoute(IList<string> routeSegmentList)
        {
            var domainFileStoreRouteSegment = routeSegmentList.IndexOf("domainFileStore");

            if (domainFileStoreRouteSegment < 0)
            {
                return false;
            }

            if (routeSegmentList[domainFileStoreRouteSegment - 2] != "iteration")
            {
                return false;
            }

            if (routeSegmentList.Count == domainFileStoreRouteSegment + 2)
            {
                return true;
            }

            if (routeSegmentList.Count == domainFileStoreRouteSegment + 4 && routeSegmentList[domainFileStoreRouteSegment + 2] == "folder")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a route is a valid route for returning a filearchive response for a <see cref="CommonFileStore"/>
        /// </summary>
        /// <param name="routeSegmentList"><see cref="IEnumerable{String}"/> that contains the route segments</param>
        /// <returns>True if the route is valid, otherwise false</returns>
        public bool IsValidCommonFileStoreArchiveRoute(IList<string> routeSegmentList)
        {
            var commonFileStoreRouteSegment = routeSegmentList.IndexOf("commonFileStore");

            if (commonFileStoreRouteSegment < 0)
            {
                return false;
            }

            if (routeSegmentList.Count == commonFileStoreRouteSegment + 2)
            {
                return true;
            }

            if (routeSegmentList.Count == commonFileStoreRouteSegment + 4 && routeSegmentList[commonFileStoreRouteSegment + 2] == "folder")
            {
                return true;
            }

            return false;
        }
    }
}
