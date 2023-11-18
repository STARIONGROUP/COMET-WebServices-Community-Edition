// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelApi.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

    using CDP4JsonSerializer;

    using CDP4MessagePackSerializer;

    using CometServer.Authorization;
    using CometServer.Configuration;
    using CometServer.Exceptions;
    using CometServer.Extensions;
    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.ChangeLog;
    using CometServer.Services.CherryPick;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations;
    using CometServer.Services.Protocol;

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
        /// A <see cref="ILogger{EngineeringModelApi}"/> instance
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
            QueryParameters.RevisionNumberQuery
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineeringModelApi"/> class
        /// </summary>
        /// <param name="appConfigService">
        /// The (injected) <see cref="IAppConfigService"/> used to read application configuration
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to create typed loggers
        /// </param>
        public EngineeringModelApi(IAppConfigService appConfigService, ILoggerFactory loggerFactory) 
            : base(appConfigService, loggerFactory)
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
            app.MapGet("EngineeringModel/{*path}", 
                async (HttpRequest req, HttpResponse res, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ICredentialsService credentialsService, IHeaderInfoProvider headerInfoProvider, Services.IServiceProvider serviceProvider, IMetaInfoProvider metaInfoProvider, IFileBinaryService fileBinaryService, IFileArchiveService fileArchiveService, IRevisionService revisionService, IRevisionResolver revisionResolver, ICdp4JsonSerializer jsonSerializer, IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService, IObfuscationService obfuscationService, ICherryPickService cherryPickService, IContainmentService containmentService) =>
            {
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
                    catch (AuthorizationException e)
                    {
                        this.logger.LogWarning("The GET REQUEST was not authorized for {identity}", req.HttpContext.User.Identity.Name);

                        res.UpdateWithNotAutherizedSettings();
                        await res.AsJson("not authorized");
                        return;
                    }

                    await this.GetResponseData(req, res, requestUtils, transactionManager, credentialsService, headerInfoProvider, serviceProvider, metaInfoProvider, fileBinaryService, fileArchiveService, revisionService, revisionResolver, jsonSerializer, messagePackSerializer, permissionInstanceFilterService, obfuscationService, cherryPickService, containmentService);
                }
            });

            app.MapPost("EngineeringModel/{engineeringModelIid:guid}/iteration/{iterationIid:guid}", 
                async (HttpRequest req, HttpResponse res, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ICredentialsService credentialsService, IHeaderInfoProvider headerInfoProvider, Services.IServiceProvider serviceProvider, IMetaInfoProvider metaInfoProvider, IOperationProcessor operationProcessor, IFileBinaryService fileBinaryService, IRevisionService revisionService, ICdp4JsonSerializer jsonSerializer, IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService, IChangeLogService changeLogService) =>
            {
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
                        this.logger.LogWarning("The POST REQUEST was not authorized for {identity}", req.HttpContext.User.Identity.Name);

                        res.UpdateWithNotAutherizedSettings();
                        await res.AsJson("not authorized");
                        return;
                    }

                    await this.PostResponseData(req, res, requestUtils, transactionManager, credentialsService, headerInfoProvider, serviceProvider, metaInfoProvider, operationProcessor, fileBinaryService, revisionService, jsonSerializer, messagePackSerializer, permissionInstanceFilterService, changeLogService);
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
        /// <param name="jsonSerializer">
        /// The <see cref="ICdp4JsonSerializer"/> used to serialize data to JSOIN
        /// </param>
        /// <param name="permissionInstanceFilterService">
        /// The <see cref="IPermissionInstanceFilterService"/> used to filter instances from the queried data
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/>
        /// </returns>
        protected async Task GetResponseData(HttpRequest httpRequest, HttpResponse httpResponse, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ICredentialsService credentialsService, IHeaderInfoProvider headerInfoProvider, Services.IServiceProvider serviceProvider, IMetaInfoProvider metaInfoProvider, IFileBinaryService fileBinaryService, IFileArchiveService fileArchiveService, IRevisionService revisionService, IRevisionResolver revisionResolver, ICdp4JsonSerializer jsonSerializer, IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService, IObfuscationService obfuscationService, ICherryPickService cherryPickService, IContainmentService containmentService)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            transactionManager.SetCachedDtoReadEnabled(true);

            var sw = new Stopwatch();
            sw.Start();
            var requestToken = this.GenerateRandomToken();

            var contentTypeKind = httpRequest.QueryContentTypeKind();

            try
            {
                this.logger.LogInformation(this.ConstructLog(httpRequest, $"{requestToken} started"));

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

                transaction = iterationContextRequest
                    ? transactionManager.SetupTransaction(ref connection, credentials, iterationContextId)
                    : transactionManager.SetupTransaction(ref connection, credentials);

                var processor = new ResourceProcessor(transaction, serviceProvider, requestUtils, metaInfoProvider);
                var modelSetup = this.DetermineEngineeringModelSetup(requestUtils, transactionManager, processor, routeSegments);
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

                    if (!resourceResponse.Any())
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

                    resourceResponse = this.CherryPick(requestUtils, cherryPickService, containmentService, resourceResponse);
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

                this.logger.LogInformation("{requestToken} completed in {sw} [ms]", requestToken, sw.ElapsedMilliseconds);

                // obfuscate if needed
                if (modelSetup.OrganizationalParticipant.Any())
                {
                    sw = new Stopwatch();
                    sw.Start();
                    this.logger.LogInformation("Model has Organizational Participation assigned. Obfuscation enabled.");

                    obfuscationService.ObfuscateResponse(resourceResponse, credentials);

                    this.logger.LogInformation("{requestToken} obfuscation completed in {sw} [ms]", requestToken, sw.ElapsedMilliseconds);
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

                this.logger.LogDebug(this.ConstructFailureLog(httpRequest, $"unauthorized request {requestToken} returned after {sw.ElapsedMilliseconds} [ms]"));

                httpResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (ThingNotFoundException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogWarning(this.ConstructFailureLog(httpRequest, $"{requestToken} thing not found in {sw.ElapsedMilliseconds} [ms]: {ex.Message}"));

                httpResponse.StatusCode = (int)HttpStatusCode.NotFound;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogError(ex, this.ConstructFailureLog(httpRequest, $"{requestToken} failed after {sw.ElapsedMilliseconds} [ms]"));

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

                sw.Stop();
                this.logger.LogInformation("Response {requestToken} returned in {sw} [ms]", requestToken, sw.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Cherry Picks <see cref="Thing" />s inside read <see cref="Thing"/>s based on provided <see cref="ClassKind"/> and <see cref="Category"/> filters
        /// </summary>
        /// <param name="resourceResponse">A collection of read <see cref="Thing"/>s</param>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <returns></returns>
        private List<Thing> CherryPick(IRequestUtils requestUtils, ICherryPickService cherryPickService, IContainmentService containmentService, IReadOnlyList<Thing> resourceResponse)
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
        /// Handles the POST requset
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
        /// <param name="jsonSerializer">
        /// The <see cref="ICdp4JsonSerializer"/> used to serialize data to JSOIN
        /// </param>
        /// <param name="permissionInstanceFilterService">
        /// The <see cref="IPermissionInstanceFilterService"/> used to filter instances from the queried data
        /// </param>
        /// <param name="metaInfoProvider">
        /// The <see cref="IMetaInfoProvider"/> used to provide metadata for any kind of <see cref="Thing"/>
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/>
        /// </returns>
        protected async Task PostResponseData(HttpRequest httpRequest, HttpResponse httpResponse, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ICredentialsService credentialsService, IHeaderInfoProvider headerInfoProvider, Services.IServiceProvider serviceProvider, IMetaInfoProvider metaInfoProvider, IOperationProcessor operationProcessor, IFileBinaryService fileBinaryService, IRevisionService revisionService, ICdp4JsonSerializer jsonSerializer, IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService, IChangeLogService changeLogService)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            var sw = new Stopwatch();
            sw.Start();
            var requestToken = this.GenerateRandomToken();

            var contentTypeKind = httpRequest.QueryContentTypeKind();

            try
            {
                this.logger.LogInformation(this.ConstructLog(httpRequest, $"{requestToken} started"));

                HttpRequestHelper.ValidateSupportedQueryParameter(httpRequest.Query, SupportedPostQueryParameter);
                var queryParameters = httpRequest.Query.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value.FirstOrDefault());
                requestUtils.QueryParameters = new QueryParameters(queryParameters);

                var multiPartBoundary = httpRequest.GetMultipartBoundary();

                var isMultiPart = multiPartBoundary != string.Empty;

                this.logger.LogDebug(this.ConstructLog(httpRequest, $"Request {requestToken} is mutlipart: {isMultiPart}"));

                Stream bodyStream;
                Dictionary<string, Stream> fileDictionary = null;

                if (isMultiPart)
                {
                    var requestStream = new MemoryStream();
                    await httpRequest.Body.CopyToAsync(requestStream);

                    bodyStream = await this.ExtractJsonBodyStreamFromMultiPartMessage(requestStream, multiPartBoundary);
                    fileDictionary = await this.ExtractFilesFromMultipartMessage(fileBinaryService, requestStream, multiPartBoundary);

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

                jsonSerializer.Initialize(metaInfoProvider, httpRequest.QueryDataModelVersion());
                var operationData = jsonSerializer.Deserialize<CdpPostOperation>(bodyStream);

                // get prepared data source transaction
                var credentials = credentialsService.Credentials;
                transaction = transactionManager.SetupTransaction(ref connection, credentials);

                // the route pattern enforces that there is atleast one route segment
                var routeSegments = HttpRequestHelper.ParseRouteSegments(httpRequest.Path);

                var resourceProcessor = new ResourceProcessor(transaction, serviceProvider, requestUtils, metaInfoProvider);

                var modelSetup = this.DetermineEngineeringModelSetup(requestUtils, transactionManager, resourceProcessor, routeSegments);

                var partition = requestUtils.GetEngineeringModelPartitionString(modelSetup.EngineeringModelIid);

                if (credentials != null)
                {
                    credentialsService.Credentials.EngineeringModelSetup = modelSetup;
                    credentialsService.ResolveParticipantCredentials(transaction);
                    credentialsService.Credentials.IsParticipant = true;

                    var iteration = this.DetermineIteration(resourceProcessor, partition, routeSegments);
                    credentialsService.Credentials.Iteration = iteration;
                }

                // defer all reference data check until after transaction commit
                using (var command = new NpgsqlCommand("SET CONSTRAINTS ALL DEFERRED;", transaction.Connection, transaction))
                {
                    command.ExecuteNonQuery();
                }

                // retrieve the revision for this transaction (or get next revision if it does not exist)
                var transactionRevision = revisionService.GetRevisionForTransaction(transaction, partition);

                operationProcessor.Process(operationData, transaction, partition, fileDictionary);

                var actor = credentialsService.Credentials.Person.Iid;

                if (this.AppConfigService.AppConfig.Changelog.CollectChanges)
                {
                    var initiallyChangedThings = revisionService.GetCurrentChanges(transaction, partition, transactionRevision, true).ToList();
                    changeLogService?.TryAppendModelChangeLogData(transaction, partition, actor, transactionRevision, operationData, initiallyChangedThings);
                }

                // save revision-history
                var changedThings = revisionService.SaveRevisions(transaction, partition, actor, transactionRevision).ToList();

                await transaction.CommitAsync();

                if (requestUtils.QueryParameters.RevisionNumber == -1)
                {
                    this.logger.LogInformation("{requestToken} completed in {sw} [ms]", requestToken, sw.ElapsedMilliseconds);

                    switch (contentTypeKind)
                    {
                        case ContentTypeKind.JSON:
                            await this.WriteJsonResponse(headerInfoProvider, metaInfoProvider, jsonSerializer, permissionInstanceFilterService, changedThings, httpRequest.QueryDataModelVersion(), httpResponse);
                            break;
                        case ContentTypeKind.MESSAGEPACK:
                            await this.WriteMessagePackResponse(headerInfoProvider, messagePackSerializer, permissionInstanceFilterService, changedThings, httpRequest.QueryDataModelVersion(), httpResponse);
                            break;
                    }
                    return;
                }

                this.logger.LogInformation(this.ConstructLog(httpRequest));
                var fromRevision = requestUtils.QueryParameters.RevisionNumber;

                // use new transaction to include latest database state
                transaction = transactionManager.SetupTransaction(ref connection, credentials);
                var revisionResponse = revisionService.Get(transaction, partition, fromRevision, true).ToArray();

                await transaction.CommitAsync();

                this.logger.LogInformation("{requestToken} completed in {sw} [ms]", requestToken, sw.ElapsedMilliseconds);

                switch (contentTypeKind)
                {
                    case ContentTypeKind.JSON:
                        await this.WriteJsonResponse(headerInfoProvider, metaInfoProvider, jsonSerializer, permissionInstanceFilterService, revisionResponse, httpRequest.QueryDataModelVersion(), httpResponse);
                        break;
                    case ContentTypeKind.MESSAGEPACK:
                        await this.WriteMessagePackResponse(headerInfoProvider, messagePackSerializer, permissionInstanceFilterService, revisionResponse, httpRequest.QueryDataModelVersion(), httpResponse);
                        break;
                }
            }
            catch (InvalidOperationException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogError(this.ConstructFailureLog(httpRequest, $"{requestToken} failed after {sw.ElapsedMilliseconds} [ms]"));

                // error handling
                httpResponse.StatusCode = (int) HttpStatusCode.Forbidden;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (BadRequestException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogError(this.ConstructFailureLog(httpRequest,$"{requestToken} failed after {sw.ElapsedMilliseconds} [ms] \n {ex.Message}"));

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

                this.logger.LogDebug(this.ConstructFailureLog(httpRequest, $"unauthorized request {requestToken} returned after {sw.ElapsedMilliseconds} [ms]"));

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

                this.logger.LogDebug(this.ConstructFailureLog(httpRequest, $"unauthorized (Thing Not Found) request {requestToken} returned after {sw.ElapsedMilliseconds} [ms]"));

                // error handling: Use Unauthorized as a user is not allowed to see if the thing is not there or a user is not allowed to see it
                httpResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogError(ex, this.ConstructFailureLog(httpRequest, $"{requestToken} failed after {sw.ElapsedMilliseconds} [ms]"));

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

                sw.Stop();
                this.logger.LogInformation("Response {requestToken} returned in {sw} [ms]", requestToken, sw.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Extracts the JSON part from the multi-part message
        /// </summary>
        /// <returns>
        /// A <see cref="Stream"/> that contains the posted JSON
        /// </returns>
        private async Task<Stream> ExtractJsonBodyStreamFromMultiPartMessage(Stream stream, string boundary)
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
        /// <param name="stream">
        /// The <see cref="Stream"/> that contains the multi-part messsage
        /// </param>
        /// <returns>
        /// A <see cref="Stream"/> that contains the posted multipart message
        /// </returns>
        private async Task<Dictionary<string, Stream>> ExtractFilesFromMultipartMessage(IFileBinaryService fileBinaryService, Stream stream, string boundary)
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
        private EngineeringModelSetup DetermineEngineeringModelSetup(IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IProcessor processor, string[] routeSegments)
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
        private Iteration DetermineIteration(IProcessor processor, string partition, string[] routeSegments)
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

            if (routeSegmentList.Count == domainFileStoreRouteSegment + 4 && routeSegmentList[domainFileStoreRouteSegment + 2] == "folder"
            )
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

            if (routeSegmentList.Count == commonFileStoreRouteSegment + 4 && routeSegmentList[commonFileStoreRouteSegment + 2] == "folder"
            )
            {
                return true;
            }

            return false;
        }
    }
}
