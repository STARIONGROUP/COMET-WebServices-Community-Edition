// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiBase.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2025 Starion Group S.A.
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
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Autofac;

    using Carter;
    using Carter.Response;

    using CDP4Common.DTO;

    using CDP4DalCommon.Protocol.Operations;
    using CDP4DalCommon.Protocol.Tasks;

    using CDP4JsonSerializer;

    using CDP4MessagePackSerializer;

    using CDP4ServicesMessaging.Messages;
    using CDP4ServicesMessaging.Services.BackgroundMessageProducers;

    using CometServer.Authentication.Basic;
    using CometServer.Authentication.Bearer;
    using CometServer.Authorization;
    using CometServer.Configuration;
    using CometServer.Enumerations;
    using CometServer.Exceptions;
    using CometServer.Extensions;
    using CometServer.Health;
    using CometServer.Helpers;
    using CometServer.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using Npgsql;

    using Services;
    using Services.Authorization;
    using Services.Protocol;

    /// <summary>
    /// This is an API abstract base class which holds utility functionalities
    /// </summary>
    public abstract class ApiBase : CarterModule
    {
        /// <summary>
        /// The site directory data.
        /// </summary>
        protected const string SiteDirectoryData = "SiteDirectory";

        /// <summary>
        /// The model reference data library type name.
        /// </summary>
        private const string ModelReferenceDataLibraryType = "ModelReferenceDataLibrary";

        /// <summary>
        /// The site reference data library type name.
        /// </summary>
        private const string SiteReferenceDataLibraryType = "SiteReferenceDataLibrary";

        /// <summary>
        /// The (injected) <see cref="ILogger{ApiBase}"/> used to log
        /// </summary>
        private readonly ILogger<ApiBase> logger;

        /// <summary>
        /// Gets or sets the <see cref="IBackgroundThingsMessageProducer"/>
        /// </summary>
        private readonly IBackgroundThingsMessageProducer thingsMessageProducer;

        /// <summary>
        /// The (injected) <see cref="ITokenGeneratorService"/> used generate HTTP request tokens
        /// </summary>
        protected readonly ITokenGeneratorService TokenGeneratorService;

        /// <summary>
        /// The (injected) <see cref="ICometTaskService"/> used to register and access running <see cref="CometTask"/>s
        /// </summary>
        protected readonly ICometTaskService CometTaskService;

        /// <summary>
        /// The (injected) <see cref="ICometHasStartedService"/>
        /// </summary>
        protected readonly ICometHasStartedService CometHasStartedService;

        /// <summary>
        /// Gets or sets the <see cref="IDataSource"/> that provides access to the data store
        /// </summary>
        protected IDataSource DataSource { get; set; }

        /// <summary>
        /// Gets the root <see cref="ILifetimeScope"/>. Each POST gets a lifetime scope of its own from it, because a POST
        /// with a waitTime can continue after its request has ended, when the services of the request have been disposed
        /// </summary>
        protected ILifetimeScope LifetimeScope { get; }

        /// <summary>
        /// Provides an array of supported authentication schemes, to be used by routes that requires authentication
        /// </summary>
        internal static string[] AuthenticationSchemes = [BasicAuthenticationDefaults.AuthenticationScheme, JwtBearerDefaults.LocalAuthenticationScheme, JwtBearerDefaults.ExternalAuthenticationScheme];

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiBase"/> class
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
        /// <param name="dataSource">
        ///The (injected) <see cref="IDataSource"/> 
        /// </param>
        /// <param name="lifetimeScope">
        /// The (injected) root <see cref="ILifetimeScope"/> from which each POST gets a lifetime scope of its own
        /// </param>
        protected ApiBase(IAppConfigService appConfigService, ICometHasStartedService cometHasStartedService, ITokenGeneratorService tokenGeneratorService, ILoggerFactory loggerFactory, IBackgroundThingsMessageProducer thingsMessageProducer, ICometTaskService cometTaskService, IDataSource dataSource, ILifetimeScope lifetimeScope)
        {
            this.logger = loggerFactory == null ? NullLogger<ApiBase>.Instance : loggerFactory.CreateLogger<ApiBase>();
            this.CometHasStartedService = cometHasStartedService;
            this.TokenGeneratorService = tokenGeneratorService;
            this.AppConfigService = appConfigService;
            this.thingsMessageProducer = thingsMessageProducer;
            this.CometTaskService = cometTaskService;
            this.DataSource = dataSource;
            this.LifetimeScope = lifetimeScope;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiBase"/> class
        /// </summary>
        /// <param name="appConfigService">
        /// The (injected) <see cref="IAppConfigService"/>
        /// </param>
        /// <param name="cometHasStartedService">
        /// The (injected) <see cref="ICometHasStartedService"/> that is used to determine whether the servre
        /// is ready to accept incoming requests
        /// </param>
        /// <param name="tokenGeneratorService">
        /// The (injected) <see cref="ITokenGeneratorService"/> used generate HTTP request tokens
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to create typed loggers
        /// </param>
        protected ApiBase(IAppConfigService appConfigService, ICometHasStartedService cometHasStartedService, ITokenGeneratorService tokenGeneratorService, ILoggerFactory loggerFactory, IDataSource dataSource)
        {
            this.logger = loggerFactory == null ? NullLogger<ApiBase>.Instance : loggerFactory.CreateLogger<ApiBase>();
            this.CometHasStartedService = cometHasStartedService;
            this.TokenGeneratorService = tokenGeneratorService;
            this.AppConfigService = appConfigService;
            this.DataSource = dataSource;
        }

        /// <summary>
        /// Gets or sets the <see cref="IAppConfigService"/>
        /// </summary>
        public IAppConfigService AppConfigService { get; set; }

        /// <summary>
        /// Authorizes a <paramref name="request"/> and calls the
        /// <see cref="ICredentialsService.ResolveCredentialsAsync"/> to resolve and set the
        /// <see cref="ICredentialsService.Credentials"/> to be used in the following pipeline
        /// </summary>
        /// <param name="credentialsService">
        /// The <see cref="ICredentialsService"/> used to provide authorization and <see cref="Credentials"/>
        /// services while handling a request
        /// </param>
        /// <param name="request">
        /// The <see cref="HttpRequest"/> that provides that should be ahtorized
        /// </param>
        /// <returns>
        /// an awaitable <see cref="Task"/> that contains the value of the identifier uses to check authorization
        /// </returns>
        protected virtual async Task<string> AuthorizeAsync(ICredentialsService credentialsService, HttpRequest request)
        {
            var userName = request.HttpContext.User.Identity!.Name;

            if (!request.DoesAuthorizationHeaderMatches(JwtBearerDefaults.ExternalAuthenticationScheme))
            {
                await this.AuthorizeAsync(credentialsService, userName);
                return userName;
            }

            var claim = request.HttpContext.User.Claims
                .FirstOrDefault(x => x.Type == this.AppConfigService.AppConfig.AuthenticationConfig.ExternalJwtAuthenticationConfig.IdentifierClaimName);

            if (claim != null)
            {
                switch ( this.AppConfigService.AppConfig.AuthenticationConfig.ExternalJwtAuthenticationConfig.PersonIdentifierPropertyKind)
                {
                    case PersonIdentifierPropertyKind.ShortName:
                        await this.AuthorizeAsync(credentialsService, claim.Value);
                        break;
                    case PersonIdentifierPropertyKind.Iid:
                        if (!Guid.TryParse(claim.Value, out var userId))
                        {
                            throw new AuthorizationException("Provided claim value is not a valid GUID.");
                        }
                        
                        await this.AuthorizeAsync(credentialsService, userId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(PersonIdentifierPropertyKind), "Unsupported PersonIdentifierPropertyKind");
                }
                
                return claim.Value;
            }

            this.logger.LogWarning("Identifier claim {ClaimName} missing from User Claims",
                this.AppConfigService.AppConfig.AuthenticationConfig.ExternalJwtAuthenticationConfig.IdentifierClaimName);

            throw new AuthorizationException($"Identifer claim {this.AppConfigService.AppConfig.AuthenticationConfig.ExternalJwtAuthenticationConfig.IdentifierClaimName} missing from User Claims");
        }

        /// <summary>
        /// Authorizes the user on the bases of the <paramref name="username"/> and calls the
        /// <see cref="ICredentialsService.ResolveCredentialsAsync"/> to resolve and set the
        /// <see cref="ICredentialsService.Credentials"/> to be used in the following pipeline
        /// </summary>
        /// <param name="credentialsService">
        /// The <see cref="ICredentialsService"/> used to provide authorization and <see cref="Credentials"/>
        /// services while handling a request
        /// </param>
        /// <param name="username">
        /// The username used to authorize
        /// </param>
        /// <returns>
        /// an awaitable <see cref="Task"/>
        /// </returns>
        private async Task AuthorizeAsync(ICredentialsService credentialsService, string username)
        {
            try
            {
                await using var connection = await this.DataSource.OpenNewConnectionAsync();

                await using var transaction = await connection.BeginTransactionAsync();

                await credentialsService.ResolveCredentialsAsync(transaction, username);
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(ex, "Authorization failed for {Username}", username);

                // Authorize failed, after logging the exception we rethrow it
                throw;
            }
        }
        
        /// <summary>
        /// Authorizes the user on the bases of the <paramref name="userId"/> and calls the
        /// <see cref="ICredentialsService.ResolveCredentials"/> to resolve and set the
        /// <see cref="ICredentialsService.Credentials"/> to be used in the following pipeline
        /// </summary>
        /// <param name="credentialsService">
        /// The <see cref="ICredentialsService"/> used to provide authorization and <see cref="Credentials"/>
        /// services while handling a request
        /// </param>
        /// <param name="userId">
        /// The user unqiue <see cref="Guid"/> used to authorize
        /// </param>
        /// <returns>
        /// an awaitable <see cref="Task"/>
        /// </returns>
        private async Task AuthorizeAsync(ICredentialsService credentialsService, Guid userId)
        {
            try
            {
                await using var connection = await this.DataSource.OpenNewConnectionAsync();

                await using var transaction = await connection.BeginTransactionAsync();

                await credentialsService.ResolveCredentialsAsync(transaction, userId);
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(ex, "Authorization failed for {UserId}", userId);

                throw;
            }
        }

        /// <summary>
        /// Checks if the server is ready to accept requests.
        /// </summary>
        /// <param name="response">The HTTP response object.</param>
        /// <returns>True if the server is ready; otherwise, false.</returns>
        protected async Task<bool> IsServerReadyAsync(HttpResponse response)
        {
            if (this.CometHasStartedService.GetHasStartedAndIsReady().IsHealthy)
            {
                return true;
            }

            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
            await response.AsJson("not yet started and not yet ready to accept requests");
            return false;
        }

        /// <summary>
        /// Process the get request and return the requested resources.
        /// </summary>
        /// <param name="transactionManager">
        /// The <see cref="ICdp4TransactionManager"/> that provides database transaction and connection services
        /// </param>
        /// <param name="processor">
        /// The resource accessor
        /// </param>
        /// <param name="topContainer">
        /// The top container.
        /// </param>
        /// <param name="partition">
        /// The data partition.
        /// </param>
        /// <param name="routeSegments">
        /// The route segments.
        /// </param>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <returns>
        /// The collection of retrieved <see cref="CDP4Common.DTO.Thing"/>.
        /// </returns>
        public async Task<ProcessRequestPathResult> ProcessRequestPathAsync(IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IProcessor processor, string topContainer, string partition, string[] routeSegments)
        {
            var result = new ProcessRequestPathResult();

            var containmentColl = new List<Thing>();
            
            var authorizedContext = new RequestSecurityContext { ContainerReadAllowed = true };
            
            // select per route segment tuple (type and optional id)
            // processing containment from top to bottom, we populate first the containmentColl then the responseColl
            for (var i = 0; i < routeSegments.Length; i++)
            {
                if (i % 2 != 0)
                {
                    // iterate per tuple
                    continue;
                }

                // a resource containment path segment consists of a containment property name and identifier segment
                // it is preceded by the url host and is followed by the resource request
                var resourceContainmentSegment = routeSegments.Length > i + 2;

                // a general resource request path ends without any identifier segment or a wildcard character '*'
                var generalResourceRequest = (routeSegments.Length == i + 2 && routeSegments[i + 1] == "*")
                                             || routeSegments.Length == i + 1;

                // a specific resource request path ends with an identifier segment
                var specificResourceRequest = !generalResourceRequest && routeSegments.Length == i + 2;

                var containerProperty = routeSegments[i];
                var serviceType = i == 0

                                      ? topContainer
                                      : processor.GetContainmentType(containmentColl, containerProperty);

                if (serviceType == nameof(Iteration) && !generalResourceRequest)
                {
                    // switch to iteration context for further processing,
                    // in case of Iteration generalResource request, this is handled separately below
                    await transactionManager.SetIterationContextAsync(processor.Transaction, partition);
                }

                if (resourceContainmentSegment)
                {
                    // this part is always used except for the last tuple
                    var identifier = routeSegments[i + 1].ParseIdentifier();

                    processor.ValidateContainment(containmentColl, containerProperty, identifier);

                    var container =
                        await processor.GetContainmentResource(serviceType, partition, identifier, authorizedContext);

                    if (serviceType == nameof(Iteration))
                    {
                        // switch the partition (schema) for further processing, allowing retrieval of Iteration contained data
                        partition = partition.Replace("EngineeringModel", "Iteration");
                    }

                    // collect the specified containment resource
                    containmentColl.Add(container);

                    result.ResourcePath.Add(container);

                    // authorized
                    authorizedContext.ContainerReadAllowed = true;
                }
                else if (generalResourceRequest)
                {
                    // get containment info
                    var containmentInfo = processor.GetContainment(containmentColl, containerProperty);

                    if (serviceType == nameof(Iteration) && containmentInfo != null)
                    {
                        // support temporal retrieval if iteration general resource is requested
                        // should only contain 1 element
                        foreach (var containedIterationId in containmentInfo)
                        {
                            // switch to iteration context for further processing
                            // use engineering-model id to set the iteration context
                            // partition here should be EngineeringModel_<uuid>
                            await transactionManager.SetIterationContextAsync(
                                processor.Transaction,
                                partition,
                                containedIterationId);

                            // collect resources
                            result.RequestedResources.AddRange(
                                await processor.GetResourceAsync(
                                    serviceType,
                                    partition,
                                    [containedIterationId],
                                    authorizedContext));
                        }
                    }
                    else
                    {
                        // collect resources
                        result.RequestedResources.AddRange(
                            await processor.GetResourceAsync(serviceType, partition, containmentInfo, authorizedContext));
                    }
                }
                else if (specificResourceRequest)
                {
                    var identifier = routeSegments[i + 1].ParseIdentifier();

                    processor.ValidateContainment(containmentColl, containerProperty, identifier);

                    var resource = (await processor.GetResourceAsync(
                        serviceType,
                        partition,
                        [identifier],
                        authorizedContext)).ToList();

                    if (resource.Count == 0)
                    {
                        continue;
                    }

                    // collect resources
                    result.RequestedResources.AddRange(resource);

                    // set specific resource from uri request
                    result.ResourcePath.Add(resource.First());
                }
            }

            if (requestUtils.QueryParameters.IncludeAllContainers)
            {
                result.RequestedResources.InsertRange(0, containmentColl);
            }

            return result;
        }

        /// <summary>
        /// Writes <see cref="Thing"/>s to a target <see cref="HttpResponse"/>
        /// </summary>
        /// <param name="resourceResponse">
        /// The resource collection to serialize.
        /// </param>
        /// <param name="requestDataModelVersion">
        /// The data model version of this request to use with serialization.
        /// </param>
        /// <param name="httpResponse">
        /// The <see cref="HttpResponse"/> to which the results will be written
        /// </param>
        /// <param name="statusCode">
        /// The optional HTML status Code.
        /// </param>
        /// <param name="requestToken">
        /// optional request token
        /// </param>
        /// <param name="headerInfoProvider">
        /// The injected <see cref="IHeaderInfoProvider"/> instance used to process HTTP headers
        /// </param>
        /// <param name="metaInfoProvider">
        /// The <see cref="IMetaInfoProvider"/> used to provide metadata for any kind of <see cref="Thing"/>
        /// </param>
        /// <param name="jsonSerializer">
        /// The <see cref="ICdp4JsonSerializer"/> used to serialize data to JSOIN
        /// </param>
        /// <param name="permissionInstanceFilterService">
        /// The <see cref="IPermissionInstanceFilterService"/> used to filter instances from the queried data
        /// </param>
        /// <returns>
        /// an awaitable <see cref="Task"/>
        /// </returns>
        protected async Task WriteJsonResponseAsync(IHeaderInfoProvider headerInfoProvider, IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, IPermissionInstanceFilterService permissionInstanceFilterService, IReadOnlyList<Thing> resourceResponse, Version requestDataModelVersion, HttpResponse httpResponse, HttpStatusCode statusCode = HttpStatusCode.OK, string requestToken = "")
        {
            try
            {
                var sw = Stopwatch.StartNew();

                if (this.logger.IsEnabled(LogLevel.Information))
                {
                    this.logger.LogInformation("JSON serialization to response started for {Token}", requestToken);
                }

                headerInfoProvider.RegisterResponseHeaders(httpResponse, ContentTypeKind.JSON, "");

                httpResponse.StatusCode = (int)statusCode;

                var resultStream = new MemoryStream();
                await this.CreateFilteredJsonResponseStreamAsync(metaInfoProvider, jsonSerializer, permissionInstanceFilterService, resourceResponse, resultStream, requestDataModelVersion, requestToken);
                resultStream.Seek(0, SeekOrigin.Begin);
                await resultStream.CopyToAsync(httpResponse.Body);

                if (this.logger.IsEnabled(LogLevel.Information))
                {
                    this.logger.LogInformation("JSON serialization to response for {Token} completed in {ElapsedMilliseconds} [ms]", requestToken, sw.ElapsedMilliseconds);
                }
            }
            catch (ObjectDisposedException ex)
            {
                if (this.logger.IsEnabled(LogLevel.Trace))
                {
                    this.logger.LogTrace(ex, "The HttpResponse has been disposed of for {RequestToken}", requestToken);
                }
            }
        }

        /// <summary>
        /// Writes <see cref="Thing"/>s to a target <see cref="HttpResponse"/>
        /// </summary>
        /// <param name="headerInfoProvider">
        /// The injected <see cref="IHeaderInfoProvider"/> instance used to process HTTP headers
        /// </param>
        /// <param name="messagePackSerializer">
        /// The <see cref="IMessagePackSerializer"/> used to serialize data to MessagePack
        /// </param>
        /// <param name="permissionInstanceFilterService">
        /// The <see cref="IPermissionInstanceFilterService"/> used to filter instances from the queried data
        /// </param>
        /// <param name="resourceResponse">
        /// The resource collection to serialize.
        /// </param>
        /// <param name="requestDataModelVersion">
        /// The data model version of this request to use with serialization.
        /// </param>
        /// <param name="httpResponse">
        /// The <see cref="HttpResponse"/> to which the results will be written
        /// </param>
        /// <param name="statusCode">
        /// The optional HTML status Code.
        /// </param>
        /// <param name="requestToken">
        /// optional request token
        /// </param>
        /// <returns>
        /// an awaitable <see cref="Task"/>
        /// </returns>
        protected async Task WriteMessagePackResponseAsync(IHeaderInfoProvider headerInfoProvider, IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService, IReadOnlyList<Thing> resourceResponse, Version requestDataModelVersion, HttpResponse httpResponse, HttpStatusCode statusCode = HttpStatusCode.OK, string requestToken = "")
        {
            try
            {
                var sw = Stopwatch.StartNew();

                if (this.logger.IsEnabled(LogLevel.Information))
                {
                    this.logger.LogInformation("MessagePack serialization to response started for {Token}", requestToken);
                }

                headerInfoProvider.RegisterResponseHeaders(httpResponse, ContentTypeKind.MESSAGEPACK, "");

                httpResponse.StatusCode = (int)statusCode;

                var resultStream = new MemoryStream();
                await this.CreateFilteredMessagePackResponseStreamAsync(messagePackSerializer, permissionInstanceFilterService, resourceResponse, resultStream, requestDataModelVersion, requestToken);
                resultStream.Seek(0, SeekOrigin.Begin);
                await resultStream.CopyToAsync(httpResponse.Body);

                if (this.logger.IsEnabled(LogLevel.Information))
                {
                    this.logger.LogInformation("MessagePack serialization to response for {Token} completed in {ElapsedMilliseconds} [ms]", requestToken, sw.ElapsedMilliseconds);
                }
            }
            catch (ObjectDisposedException ex)
            {
                if (this.logger.IsEnabled(LogLevel.Trace))
                {
                    this.logger.LogTrace(ex, "The HttpResponse has been disposed of for {RequestToken}", requestToken);
                }
            }
        }

        /// <summary>
        /// Create a multipart response for the included file revisions.
        /// </summary>
        /// <param name="headerInfoProvider">
        /// The injected <see cref="IHeaderInfoProvider"/> instance used to process HTTP headers
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
        /// <param name="permissionInstanceFilterService">
        /// The <see cref="IPermissionInstanceFilterService"/> used to filter instances from the queried data
        /// </param>
        /// <param name="fileRevisions">
        /// The file revisions.
        /// </param>
        /// <param name="resourceResponse">
        /// The resource response.
        /// </param>
        /// <param name="version">
        /// The model <see cref="Version"/>
        /// </param>
        /// <param name="httpResponse">
        /// The <see cref="HttpResponse"/> to which the results will be written
        /// </param>
        /// <param name="statusCode">
        /// The optional HTML status Code.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponse"/>.
        /// </returns>
        protected Task WriteMultipartResponseAsync(IHeaderInfoProvider headerInfoProvider, IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, IFileBinaryService fileBinaryService, IPermissionInstanceFilterService permissionInstanceFilterService, List<FileRevision> fileRevisions, List<Thing> resourceResponse, Version version, HttpResponse httpResponse, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            headerInfoProvider.RegisterResponseHeaders(httpResponse, ContentTypeKind.MULTIPARTMIXED, HttpConstants.BoundaryString);

            httpResponse.StatusCode = (int)statusCode;

            return this.PrepareMultiPartResponseAsync(metaInfoProvider,jsonSerializer, fileBinaryService, permissionInstanceFilterService,  httpResponse.Body, fileRevisions, resourceResponse, version);
        }

        /// <summary>
        /// Create an archived response for a folder or a file store.
        /// </summary>
        /// <param name="resourceResponse">
        /// The resource response.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="routeSegments">
        /// The route segments of the HTTP request.
        /// </param>
        /// <param name="httpResponse">
        /// The <see cref="HttpResponse"/> to which the results will be written
        /// </param>
        /// <param name="statusCode">
        /// The optional HTTP status Code.
        /// </param>
        /// <param name="headerInfoProvider">
        /// The injected <see cref="IHeaderInfoProvider"/> instance used to process HTTP headers
        /// </param>
        /// <param name="metaInfoProvider">
        /// The (injected) <see cref="IMetaInfoProvider"/> used to provide metadata for any kind of <see cref="Thing"/>
        /// </param>
        /// <param name="jsonSerializer">
        /// The (injected) <see cref="ICdp4JsonSerializer"/> used to serialize data to JSOIN
        /// </param>
        /// <param name="fileArchiveService">
        /// The (injected) <see cref="IFileArchiveService"/> to store and retrieve files to and from the filesystem
        /// </param>
        /// <param name="permissionInstanceFilterService">
        /// The (injected) <see cref="IPermissionInstanceFilterService"/> used to filter instances from the queried data
        /// </param>
        /// <param name="version">
        /// The CDP4-COMET Data model version to be used for serialization
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponse"/>.
        /// </returns>
        protected Task WriteArchivedResponseAsync(IHeaderInfoProvider headerInfoProvider, IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, IFileArchiveService fileArchiveService, IPermissionInstanceFilterService permissionInstanceFilterService, List<Thing> resourceResponse, string partition, string[] routeSegments, Version version, HttpResponse httpResponse, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            headerInfoProvider.RegisterResponseHeaders(httpResponse, ContentTypeKind.MULTIPARTMIXED, HttpConstants.BoundaryString);
            httpResponse.StatusCode = (int)statusCode;

            return this.PrepareArchivedResponseAsync(metaInfoProvider,jsonSerializer, fileArchiveService, permissionInstanceFilterService, httpResponse.Body, resourceResponse, version, partition, routeSegments);
        }

        /// <summary>
        /// Collect and return the reference data library chain.
        /// </summary>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <param name="processor">
        /// The processor instance.
        /// </param>
        /// <param name="modelSetup">
        /// The model setup instance for which to retrieve the reference data library chain.
        /// </param>
        /// <returns>
        /// IEnumerable collection of data reference library items.
        /// </returns>
        protected static async Task<IEnumerable<Thing>> CollectReferenceDataLibraryChainAsync(IRequestUtils requestUtils,
            IProcessor processor,
            EngineeringModelSetup modelSetup)
        {
            var securityContext = SetupSecurityContextForLibraryRead(requestUtils);

            // retrieve the required model reference data library
            var modelReferenceDataLibraryData = await processor.GetResourceAsync(ModelReferenceDataLibraryType, SiteDirectoryData, modelSetup.RequiredRdl, securityContext);

            return await RetrieveChainedReferenceDataAsync(requestUtils, processor, securityContext, modelReferenceDataLibraryData);
        }

        /// <summary>
        /// Collect and return the reference data library chain.
        /// </summary>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <param name="processor">
        /// The processor instance.
        /// </param>
        /// <param name="modelReferenceDataLibrary">
        /// The model Reference Data Library.
        /// </param>
        /// <returns>
        /// IEnumerable collection of data reference library items.
        /// </returns>
        protected static async Task<IEnumerable<Thing>> CollectReferenceDataLibraryChainAsync(IRequestUtils requestUtils, IProcessor processor, ModelReferenceDataLibrary modelReferenceDataLibrary)
        {
            var securityContext = SetupSecurityContextForLibraryRead(requestUtils);

            // retrieve the required model reference data library with extent = deep
            var modelReferenceDataLibraryData = await processor.GetResourceAsync(ModelReferenceDataLibraryType, SiteDirectoryData, [modelReferenceDataLibrary.Iid], securityContext);

            return (await RetrieveChainedReferenceDataAsync(requestUtils, processor, securityContext, modelReferenceDataLibraryData))
                .Where(x => x.Iid != modelReferenceDataLibrary.Iid);
        }

        /// <summary>
        /// Filters supplied DTO's and creates a JSON response stream based on an <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="metaInfoProvider">
        /// The <see cref="IMetaInfoProvider"/> used to provide metadata for any kind of <see cref="Thing"/>
        /// </param>
        /// <param name="jsonSerializer">
        /// The <see cref="ICdp4JsonSerializer"/> used to serialize data to JSOIN
        /// </param>
        /// <param name="permissionInstanceFilterService">
        /// The <see cref="IPermissionInstanceFilterService"/> used to filter instances from the queried data
        /// </param>
        /// <param name="dtos">
        /// The DTO's that needs to be serialized to a stream
        /// </param>
        /// <param name="stream">
        /// Stream to which to write the serializer output
        /// </param>
        /// <param name="requestDataModelVersion">
        /// The data model version of this request to use with serialization.
        /// </param>
        /// <param name="requestToken">
        /// optional request token
        /// </param>
        private async Task CreateFilteredJsonResponseStreamAsync(IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, IPermissionInstanceFilterService permissionInstanceFilterService,
            IReadOnlyList<Thing> dtos,
            Stream stream,
            Version requestDataModelVersion,
            string requestToken = "")
        {
            var filteredDtos = (await permissionInstanceFilterService.FilterOutPermissionsAsync(dtos, requestDataModelVersion)).ToArray();

            var sw = new Stopwatch();
            sw.Start();

            if (this.logger.IsEnabled(LogLevel.Debug))
            {
                this.logger.LogDebug("{RequestToken} start serializing dtos as JSON", requestToken);
            }

            jsonSerializer.Initialize(metaInfoProvider, requestDataModelVersion);
            jsonSerializer.SerializeToStream(filteredDtos, stream);
            sw.Stop();

            if (this.logger.IsEnabled(LogLevel.Debug))
            {
                this.logger.LogDebug("serializing dtos as JSON - {RequestToken} in {ElapsedMilliseconds} [ms]", requestToken, sw.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Filters supplied DTO's and creates a JSON response stream based on an <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="messagePackSerializer">
        /// The <see cref="IMessagePackSerializer"/> used to serialize data to MessagePack
        /// </param>
        /// <param name="permissionInstanceFilterService">
        /// The <see cref="IPermissionInstanceFilterService"/> used to filter instances from the queried data
        /// </param>
        /// <param name="dtos">
        /// The DTO's that needs to be serialized to a stream
        /// </param>
        /// <param name="stream">
        /// Stream to which to write the serializer output
        /// </param>
        /// <param name="requestDataModelVersion">
        /// The data model version of this request to use with serialization.
        /// </param>
        /// <param name="requestToken">
        /// optional request token
        /// </param>
        private async Task CreateFilteredMessagePackResponseStreamAsync(IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService,
            IReadOnlyList<Thing> dtos,
            Stream stream,
            Version requestDataModelVersion,
            string requestToken = "")
        {
            var filteredDtos = (await permissionInstanceFilterService.FilterOutPermissionsAsync(dtos, requestDataModelVersion)).ToArray();

            var sw = new Stopwatch();
            sw.Start();

            if (this.logger.IsEnabled(LogLevel.Debug))
            {
                this.logger.LogDebug("{RequestToken} start serializing dtos as MessagePack", requestToken);
            }

            messagePackSerializer.SerializeToStream(filteredDtos, stream);
            sw.Stop();

            if (this.logger.IsEnabled(LogLevel.Debug))
            {
                this.logger.LogDebug("serializing dtos as MessagePack - {RequestToken} in {ElapsedMilliseconds} [ms]", requestToken, sw.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Prepare a multi part response based on all included fileRevisions in the response.
        /// </summary>
        /// <param name="metaInfoProvider">
        /// The <see cref="IMetaInfoProvider"/> used to provide metadata for any kind of <see cref="Thing"/>
        /// </param>
        /// <param name="fileBinaryService">
        ///
        /// </param>
        /// <param name="permissionInstanceFilterService">
        /// The <see cref="IPermissionInstanceFilterService"/> used to filter instances from the queried data
        /// </param>
        /// <param name="targetStream">
        /// The target Stream.
        /// </param>
        /// <param name="fileRevisions">
        /// The file Revisions.
        /// </param>
        /// <param name="resourceResponse">
        /// The resource response.
        /// </param>
        /// <param name="requestDataModelVersion">
        /// The request data model version.
        /// </param>
        /// <param name="jsonSerializer">
        /// The <see cref="ICdp4JsonSerializer"/> used to serialize data to JSOIN
        /// </param>
        private async Task PrepareMultiPartResponseAsync(IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, IFileBinaryService fileBinaryService, IPermissionInstanceFilterService permissionInstanceFilterService, Stream targetStream, List<FileRevision> fileRevisions, List<Thing> resourceResponse, Version requestDataModelVersion)
        {
            if (fileRevisions.Count == 0)
            {
                // do nothing if no file revisions are present
                return;
            }

            var content = new MultipartContent("mixed", HttpConstants.BoundaryString);

            using (var stream = new MemoryStream())
            {
                await this.CreateFilteredJsonResponseStreamAsync(metaInfoProvider, jsonSerializer, permissionInstanceFilterService, resourceResponse, stream, requestDataModelVersion);

                // rewind stream prior to reading
                stream.Position = 0;

                // write out the json content to the first multipart content entry
                var jsonContent = new StringContent(await new StreamReader(stream).ReadToEndAsync());
                jsonContent.Headers.Clear();
                jsonContent.Headers.Add(HttpConstants.ContentTypeHeader, HttpConstants.MimeTypeJson);
                content.Add(jsonContent);

                await stream.FlushAsync();
            }

            foreach (var hash in fileRevisions.Select(x => x.ContentHash).Distinct())
            {
                byte[] buffer;
                long fileSize;

                await using (var fileStream = fileBinaryService.RetrieveBinaryData(hash))
                {
                    fileSize = fileStream.Length;
                    buffer = new byte[(int)fileSize];
                    var readBytes = await fileStream.ReadAsync(buffer, 0, (int)fileSize);

                    if (readBytes != fileSize)
                    {
                        this.logger.LogWarning("Failed to read {FileSize} bytes, only read {ReadBytes}", fileSize, readBytes);
                    }
                }

                // write out the binary content to the first multipart content entry
                var binaryContent = new ByteArrayContent(buffer);
                binaryContent.Headers.Add(HttpConstants.ContentTypeHeader, HttpConstants.MimeTypeOctetStream);

                // use the file hash value to easily identify the multipart content for each respective filerevision hash entry
                binaryContent.Headers.Add(HttpConstants.ContentDispositionHeader, $"attachment; filename={hash}");
                binaryContent.Headers.Add(HttpConstants.ContentLengthHeader, fileSize.ToString());
                content.Add(binaryContent);
            }

            // stream the multipart content to the request contents target stream
            await content.CopyToAsync(targetStream);
            AddMultiPartMimeEndpoint(targetStream);
        }

        /// <summary>
        /// Prepare an archived response based on a folder or fileStore in the response.
        /// </summary>
        /// <param name="metaInfoProvider">
        /// The <see cref="IMetaInfoProvider"/> used to provide metadata for any kind of <see cref="Thing"/>
        /// </param>
        /// <param name="jsonSerializer">
        /// The <see cref="ICdp4JsonSerializer"/> used to serialize data to JSOIN
        /// </param>
        /// <param name="fileArchiveService">
        /// The (injected) <see cref="IFileArchiveService"/> to store and retrieve files to and from the filesystem
        /// </param>
        /// <param name="permissionInstanceFilterService">
        /// The <see cref="IPermissionInstanceFilterService"/> used to filter instances from the queried data
        /// </param>
        /// <param name="targetStream">
        /// The target Stream.
        /// </param>
        /// <param name="resourceResponse">
        /// The resource response.
        /// </param>
        /// <param name="requestDataModelVersion">
        /// The request data model version.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        ///  <param name="routeSegments">
        /// The route segments.
        /// </param>
        private async Task PrepareArchivedResponseAsync(IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, IFileArchiveService fileArchiveService, IPermissionInstanceFilterService permissionInstanceFilterService, Stream targetStream, List<Thing> resourceResponse, Version requestDataModelVersion, string partition, string[] routeSegments)
        {
            var temporaryTopFolder = await fileArchiveService.CreateFolderAndFileStructureOnDiskAsync(resourceResponse, partition, routeSegments);

            try
            {
                var content = new MultipartContent("mixed", HttpConstants.BoundaryString);

                using (var stream = new MemoryStream())
                {
                    await this.CreateFilteredJsonResponseStreamAsync(metaInfoProvider, jsonSerializer, permissionInstanceFilterService, resourceResponse, stream, requestDataModelVersion);

                    // rewind stream prior to reading
                    stream.Position = 0;

                    // write out the json content to the first multipart content entry
                    var jsonContent = new StringContent(await new StreamReader(stream).ReadToEndAsync());
                    jsonContent.Headers.Clear();
                    jsonContent.Headers.Add(HttpConstants.ContentTypeHeader, HttpConstants.MimeTypeJson);
                    content.Add(jsonContent);

                    await stream.FlushAsync();
                }

                fileArchiveService.CreateZipArchive(temporaryTopFolder);

                byte[] buffer;
                long fileSize;

                await using (var fileStream = new FileStream(temporaryTopFolder + ".zip", FileMode.Open))
                {
                    fileSize = fileStream.Length;
                    buffer = new byte[(int)fileSize];
                    var readBytes = await fileStream.ReadAsync(buffer, 0, (int)fileSize);
                    
                    if (readBytes != fileSize)
                    {
                        this.logger.LogWarning("Failed to read {FileSize} bytes, only read {ReadBytes}", fileSize, readBytes);
                    }
                }

                var binaryContent = new ByteArrayContent(buffer);
                binaryContent.Headers.Add(HttpConstants.ContentTypeHeader, HttpConstants.MimeTypeOctetStream);

                // use the file hash value to easily identify the multipart content for each respective filerevision hash entry

                var fileInfo = new FileInfo($"{temporaryTopFolder}.zip");
                
                binaryContent.Headers.Add(HttpConstants.ContentDispositionHeader, $"attachment; filename={fileInfo.Name}");

                binaryContent.Headers.Add(HttpConstants.ContentLengthHeader, fileSize.ToString());
                content.Add(binaryContent);

                // stream the multipart content to the request contents target stream
                await content.CopyToAsync(targetStream);

                AddMultiPartMimeEndpoint(targetStream);
            }
            finally
            {
                fileArchiveService.DeleteFolderAndFileStructureAndArchive(temporaryTopFolder);
            }
        }

        /// <summary>
        /// add ending line according to https://www.w3.org/Protocols/rfc1341/7_2_Multipart.html
        /// </summary>
        /// <param name="targetStream">The <see cref="Stream"/>where to add the end point to</param>
        /// <remarks>
        /// Changes the <param name="targetStream"></param>
        /// </remarks>
        private static void AddMultiPartMimeEndpoint(Stream targetStream)
        {
            var endLine = Encoding.Default.GetBytes($"\r\n--{HttpConstants.BoundaryString}--");
            targetStream.Write(endLine, 0, endLine.Length);
        }

        /// <summary>
        /// Setup the security context for the library data retrieval.
        /// </summary>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <returns>
        /// The <see cref="RequestSecurityContext"/>.
        /// </returns>
        private static RequestSecurityContext SetupSecurityContextForLibraryRead(IRequestUtils requestUtils)
        {
            // override query parameters to return full set
            requestUtils.OverrideQueryParameters =
                new QueryParameters { ExtentDeep = true, IncludeReferenceData = true };

            return new RequestSecurityContext { ContainerReadAllowed = true };
        }

        /// <summary>
        /// The retrieve chained reference data.
        /// </summary>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <param name="processor">
        /// The processor.
        /// </param>
        /// <param name="securityContext">
        /// The security context.
        /// </param>
        /// <param name="modelReferenceDataLibraryData">
        /// The model reference data library data.
        /// </param>
        /// <returns>
        /// A collection of retrieved reference data.
        /// </returns>
        private static async Task<ReadOnlyCollection<Thing>> RetrieveChainedReferenceDataAsync(IRequestUtils requestUtils, IProcessor processor, ISecurityContext securityContext, IEnumerable<Thing> modelReferenceDataLibraryData)
        {
            var chainedReferenceDataColl = new List<Thing>();

            // do not collect the model reference data library again, as it is already present
            chainedReferenceDataColl.AddRange(modelReferenceDataLibraryData);
            var referenceDataLibrary = (ReferenceDataLibrary)chainedReferenceDataColl.First();

            // while the requiredRdl is set retrieve the reference data library chain bottom up
            while (referenceDataLibrary.RequiredRdl != null)
            {
                var siteReferenceDataLibraryData = (await processor.GetResourceAsync(
                    SiteReferenceDataLibraryType,
                    SiteDirectoryData,
                    new List<Guid> { (Guid)referenceDataLibrary.RequiredRdl },
                    securityContext)).ToList();

                chainedReferenceDataColl.AddRange(siteReferenceDataLibraryData);
                referenceDataLibrary = (ReferenceDataLibrary)siteReferenceDataLibraryData.First();
            }

            // reset query parameters
            requestUtils.OverrideQueryParameters = null;

            return chainedReferenceDataColl.AsReadOnly();
        }

        /// <summary>
        /// Prepares and sends a message about the changed <see cref="Thing"/> instances based on the provided operation data.
        /// </summary>
        /// <param name="originalPostOperation">The operation data containing information about create, update, and delete operations.</param>
        /// <param name="changedThings">The collection of changed <see cref="Thing"/> instances.</param>
        /// <param name="actorId">The actor id.</param>
        /// <param name="serializer">The <see cref="ICdp4JsonSerializer"/></param>
        /// <returns>An asynchronous task representing the operation.</returns>
        protected async Task PrepareAndQueueThingsMessageAsync(PostOperation originalPostOperation, IEnumerable<Thing> changedThings, Guid actorId, ICdp4JsonSerializer serializer)
        {
            if (!this.AppConfigService.AppConfig.ServiceMessagingConfig.IsEnabled || this.thingsMessageProducer is null)
            {
                return;
            }

            await using var serializedPostOperation = new MemoryStream();
            serializer.SerializeToStream(originalPostOperation, serializedPostOperation);
            serializedPostOperation.Position = 0;
            using var reader = new StreamReader(serializedPostOperation, Encoding.UTF8);
            
            var message = new ThingsChangedMessage()
            {
                ChangedThings = changedThings.ToList(),
                PostOperation = await reader.ReadToEndAsync(),
                ActorId = actorId,
                TimeStamp = DateTime.Now
            };

            await this.thingsMessageProducer.EnqueueAsync(message);
        }

        /// <summary>
        /// Creates and registers a <see cref="CometTask"/> with the <see cref="ICometTaskService"/>
        /// </summary>
        /// <param name="actor">
        /// The unique identifier of the <see cref="Person"/> that mae the POST request
        /// </param>
        /// <param name="topContainerName">
        /// The name of the <see cref="TopContainer"/> for which the POST requst was made (either SiteDirectory or EngineeringModel)
        /// </param>
        /// <param name="requestToken">
        /// the token used for log correlation
        /// </param>
        /// <returns>
        /// The registered and enqueued <see cref="CometTask"/>
        /// </returns>
        protected CometTask CreateAndRegisterCometTask(Guid actor, string topContainerName, string requestToken)
        {
            var cometTask = new CometTask()
            {
                Id = Guid.NewGuid(),
                Actor = actor,
                StatusKind = StatusKind.PROCESSING,
                StartedAt = DateTime.Now,
                TopContainer = topContainerName,
                RequestToken = requestToken
            };

            this.CometTaskService.AddOrUpdateTask(cometTask);

            return cometTask;
        }

        /// <summary>
        /// The longest time <see cref="WaitForCompletionAsync"/> waits. <see cref="Task.Delay(TimeSpan)"/> does not support
        /// a delay of more than about 49 days and throws for it; this value is well within that limit.
        /// </summary>
        internal static readonly TimeSpan MaximumWaitTime = TimeSpan.FromMilliseconds(int.MaxValue);

        /// <summary>
        /// Starts the <paramref name="work"/> and waits until it has completed, but no longer than the <paramref name="waitTime"/>
        /// </summary>
        /// <param name="work">
        /// The work to start. The <see cref="Task"/> it returns has to represent all of the work: this method waits for that
        /// <see cref="Task"/>, not for the delegate to return
        /// </param>
        /// <param name="waitTime">
        /// The maximum time to wait for the <paramref name="work"/> to complete
        /// </param>
        /// <returns>
        /// true when the <paramref name="work"/> completed within the <paramref name="waitTime"/>, false when it is still running
        /// </returns>
        /// <remarks>
        /// An exception of <paramref name="work"/> that completes within the <paramref name="waitTime"/> is rethrown.
        /// A <paramref name="waitTime"/> longer than <see cref="MaximumWaitTime"/> is treated as <see cref="MaximumWaitTime"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// When <paramref name="work"/> is null
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When <paramref name="waitTime"/> is negative. The <paramref name="work"/> is not started in that case.
        /// </exception>
        internal static async Task<bool> WaitForCompletionAsync(Func<Task> work, TimeSpan waitTime)
        {
            ArgumentNullException.ThrowIfNull(work);

            // validate before the work is started: work that is running while this method throws is observed by nobody
            ArgumentOutOfRangeException.ThrowIfLessThan(waitTime, TimeSpan.Zero);

            if (waitTime > MaximumWaitTime)
            {
                waitTime = MaximumWaitTime;
            }

            // Task.Run(Func<Task>) returns a proxy for the task that the delegate returns, so it only completes when
            // the work itself has completed. With an Action it would complete as soon as the work reaches its first await.
            var workTask = Task.Run(work);

            using var delayCancellation = new CancellationTokenSource();

            var completedTask = await Task.WhenAny(workTask, Task.Delay(waitTime, delayCancellation.Token));

            if (completedTask != workTask)
            {
                return false;
            }

            await delayCancellation.CancelAsync();

            // observe the outcome of the work, an exception included
            await workTask;

            return true;
        }

        /// <summary>
        /// Handles a POST request with services of a lifetime scope of its own, not with those of the request: a POST
        /// with a waitTime can continue after the request has ended, and the services of the request are disposed when
        /// it ends. The scope is disposed when <paramref name="handlePost"/> has finished, unless it has handed the scope
        /// over to <see cref="ProcessPostWithWaitTimeAsync"/>
        /// </summary>
        /// <param name="httpResponse">
        /// The <see cref="HttpResponse"/> of the request
        /// </param>
        /// <param name="handlePost">
        /// Handles the POST with the services that it resolves from the <see cref="PostLifetimeScope"/> it is given
        /// </param>
        /// <returns>
        /// an awaitable <see cref="Task"/>
        /// </returns>
        protected async Task HandlePostAsync(HttpResponse httpResponse, Func<PostLifetimeScope, Task> handlePost)
        {
            if (!await this.IsServerReadyAsync(httpResponse))
            {
                return;
            }

            var postLifetimeScope = new PostLifetimeScope(this.LifetimeScope.BeginLifetimeScope());

            try
            {
                await handlePost(postLifetimeScope);
            }
            finally
            {
                await postLifetimeScope.DisposeUnlessHandedOverAsync();
            }
        }

        /// <summary>
        /// Processes a POST request that has a wait time, with the services of the <paramref name="postLifetimeScope"/>
        /// that the request handler resolved and passes down in <paramref name="processPost"/>. When the work completes
        /// within the wait time its <see cref="PostResult"/> is written to the <paramref name="httpResponse"/>, otherwise
        /// the <see cref="CometTask"/> is, and the work continues and reports its outcome only through the
        /// <see cref="CometTask"/>
        /// </summary>
        /// <param name="postLifetimeScope">
        /// The <see cref="PostLifetimeScope"/> from which the services in <paramref name="processPost"/> and
        /// <paramref name="writeResult"/> were resolved. This method takes it over, also when it throws: it is disposed
        /// when the work has finished and the response has been written, whichever comes last
        /// </param>
        /// <param name="waitTime">
        /// The wait time of the request, in seconds; more than 0
        /// </param>
        /// <param name="postRequestData">
        /// The <see cref="PostRequestData"/> of the request
        /// </param>
        /// <param name="requestToken">
        /// The request token used for log correlation
        /// </param>
        /// <param name="cometTask">
        /// The <see cref="CometTask"/> that is linked to the POST request
        /// </param>
        /// <param name="processPost">
        /// Processes the POST with the services of the <paramref name="postLifetimeScope"/>
        /// </param>
        /// <param name="writeResult">
        /// Writes a <see cref="PostResult"/> to the <paramref name="httpResponse"/>
        /// </param>
        /// <param name="httpResponse">
        /// The <see cref="HttpResponse"/> of the request
        /// </param>
        /// <returns>
        /// an awaitable <see cref="Task"/>
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// When <paramref name="waitTime"/> is not more than 0. The work is not started and the scope is disposed
        /// </exception>
        protected async Task ProcessPostWithWaitTimeAsync(PostLifetimeScope postLifetimeScope, int waitTime, PostRequestData postRequestData, string requestToken, CometTask cometTask, Func<Task<PostResult>> processPost, Func<PostResult, Task> writeResult, HttpResponse httpResponse)
        {
            postLifetimeScope.HandOver();

            if (waitTime <= 0)
            {
                // the work is never started, so nobody else would dispose the scope
                await postLifetimeScope.LifetimeScope.DisposeAsync();
                throw new ArgumentOutOfRangeException(nameof(waitTime), waitTime, "The wait time must be more than 0 seconds");
            }

            // the work and the writing of the response both use the services of the scope: the last one to finish disposes it
            var lifetimeScopeUsers = 2;

            async Task ReleaseLifetimeScopeAsync()
            {
                if (Interlocked.Decrement(ref lifetimeScopeUsers) == 0)
                {
                    await postLifetimeScope.LifetimeScope.DisposeAsync();
                }
            }

            PostResult result = null;

            async Task WorkAsync()
            {
                try
                {
                    result = await processPost();
                }
                catch (Exception ex)
                {
                    // nobody may be waiting for the work any more: a failure is reported through the CometTask only
                    this.logger.LogError(ex, "{Request}:{RequestToken} - The work of the POST failed", postRequestData.MethodPathName.Sanitize(), requestToken);

                    result = this.FailPost(cometTask, HttpStatusCode.InternalServerError, ex.Message);
                }
                finally
                {
                    await ReleaseLifetimeScopeAsync();
                }
            }

            try
            {
                var completedWithinWaitTime = await WaitForCompletionAsync(WorkAsync, TimeSpan.FromSeconds(waitTime));

                if (completedWithinWaitTime)
                {
                    if (this.logger.IsEnabled(LogLevel.Information))
                    {
                        this.logger.LogInformation("The task {Id} for actor {Actor} completed within the requested wait time {WaitTime}", cometTask.Id, cometTask.Actor, waitTime);
                    }

                    await writeResult(result);
                }
                else
                {
                    if (this.logger.IsEnabled(LogLevel.Information))
                    {
                        this.logger.LogInformation("The task {Id} for actor {Actor} did not complete within the requested wait time {WaitTime}; the CometTask is returned and the work continues in the background", cometTask.Id, cometTask.Actor, waitTime);
                    }

                    await httpResponse.AsJson(cometTask);
                }
            }
            finally
            {
                await ReleaseLifetimeScopeAsync();
            }
        }

        /// <summary>
        /// Rolls back the <paramref name="transaction"/> when there is one
        /// </summary>
        /// <param name="transactionManager">
        /// The <see cref="ICdp4TransactionManager"/> that provides database transaction and connection services
        /// </param>
        /// <param name="transaction">
        /// The <see cref="NpgsqlTransaction"/> to roll back, or null
        /// </param>
        /// <returns>
        /// an awaitable <see cref="Task"/>
        /// </returns>
        protected static async Task TryRollbackAsync(ICdp4TransactionManager transactionManager, NpgsqlTransaction transaction)
        {
            if (transaction != null)
            {
                await transactionManager.TryRollbackTransaction(transaction);
            }
        }

        /// <summary>
        /// Records the failure of a POST in its <see cref="CometTask"/> and creates the <see cref="PostResult"/> for it
        /// </summary>
        /// <param name="cometTask">
        /// The <see cref="CometTask"/> that is linked to the POST request
        /// </param>
        /// <param name="statusCode">
        /// The <see cref="HttpStatusCode"/> of the response
        /// </param>
        /// <param name="errorMessage">
        /// The error message
        /// </param>
        /// <param name="errorTag">
        /// The optional CDP error tag
        /// </param>
        /// <returns>
        /// The <see cref="PostResult"/>
        /// </returns>
        protected PostResult FailPost(CometTask cometTask, HttpStatusCode statusCode, string errorMessage, string errorTag = null)
        {
            var statusText = statusCode switch
            {
                HttpStatusCode.BadRequest => "BAD REQUEST",
                HttpStatusCode.Forbidden => "FORBIDDEN",
                HttpStatusCode.Unauthorized => "UNAUTHORIZED",
                _ => "INTERNAL SERVER ERROR"
            };

            this.CometTaskService.AddOrUpdateTask(cometTask, finishedAt: DateTime.Now, statusKind: StatusKind.FAILED, error: $"{statusText} - {errorMessage}");

            return PostResult.Failure(statusCode, errorMessage, errorTag);
        }

        /// <summary>
        /// Writes the <see cref="PostResult"/> of a POST request to its <see cref="HttpResponse"/>
        /// </summary>
        /// <param name="postResult">
        /// The <see cref="PostResult"/> to write
        /// </param>
        /// <param name="postRequestData">
        /// The <see cref="PostRequestData"/> of the request
        /// </param>
        /// <param name="httpResponse">
        /// The <see cref="HttpResponse"/> to which the result is written
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
        /// The <see cref="IPermissionInstanceFilterService"/> used to filter instances from the response
        /// </param>
        /// <returns>
        /// an awaitable <see cref="Task"/>
        /// </returns>
        protected async Task WritePostResultAsync(PostResult postResult, PostRequestData postRequestData, HttpResponse httpResponse, IHeaderInfoProvider headerInfoProvider, IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService)
        {
            if (!postResult.IsSuccess)
            {
                if (postResult.ErrorTag != null)
                {
                    AddErrorTagHeader(httpResponse, postResult.ErrorTag);
                }

                httpResponse.StatusCode = (int)postResult.StatusCode;
                await httpResponse.AsJson($"exception:{postResult.ErrorMessage}");
                return;
            }

            switch (postRequestData.ContentType)
            {
                case ContentTypeKind.JSON:
                    await this.WriteJsonResponseAsync(headerInfoProvider, metaInfoProvider, jsonSerializer, permissionInstanceFilterService, postResult.Things, postRequestData.Version, httpResponse);
                    break;
                case ContentTypeKind.MESSAGEPACK:
                    await this.WriteMessagePackResponseAsync(headerInfoProvider, messagePackSerializer, permissionInstanceFilterService, postResult.Things, postRequestData.Version, httpResponse);
                    break;
            }
        }

        /// <summary>
        /// Adds a specific CDP error tag to an <see cref="HttpResponse"/> message
        /// </summary>
        /// <param name="httpResponse">The <see cref="HttpResponse"/></param>
        /// <param name="errorTag">The the Error tag</param>
        protected static void AddErrorTagHeader(HttpResponse httpResponse, string errorTag)
        {
            var errorTagHeader = HttpConstants.CDPErrorTag;

            if (!httpResponse.Headers.ContainsKey(errorTagHeader))
            {
                httpResponse.Headers[errorTagHeader] = errorTag;
            }
            else
            {
                httpResponse.Headers.TryAdd(errorTagHeader, errorTag);
            }
        }
    }
}
