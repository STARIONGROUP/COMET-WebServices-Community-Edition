// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiBase.cs" company="RHEA System S.A.">
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
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Carter;

    using CDP4Common.DTO;

    using CDP4JsonSerializer;

    using CDP4MessagePackSerializer;

    using CometServer.Authorization;
    using CometServer.Configuration;
    using CometServer.Extensions;
    using CometServer.Helpers;
    
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
        /// The (injected) <see cref="ITokenGeneratorService"/> used generate HTTP request tokens
        /// </summary>
        protected readonly ITokenGeneratorService TokenGeneratorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiBase"/> class
        /// </summary>
        /// <param name="appConfigService">
        /// The (injected) <see cref="IAppConfigService"/>
        /// </param>
        /// <param name="tokenGeneratorService">
        /// The (injected) <see cref="ITokenGeneratorService"/> used generate HTTP request tokens
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to create typed loggers
        /// </param>
        protected ApiBase(IAppConfigService appConfigService, ITokenGeneratorService tokenGeneratorService, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory == null ? NullLogger<ApiBase>.Instance : loggerFactory.CreateLogger<ApiBase>();
            this.TokenGeneratorService = tokenGeneratorService;
            this.AppConfigService = appConfigService;
        }

        /// <summary>
        /// Gets or sets the <see cref="IAppConfigService"/>
        /// </summary>
        public IAppConfigService AppConfigService { get; set; }

        /// <summary>
        /// Authorizes the user on the bases of the <paramref name="username"/> and calls the
        /// <see cref="ICredentialsService.ResolveCredentials"/> to resolve and set the
        /// <see cref="ICredentialsService.Credentials"/> to be used in the following pipeline
        /// </summary>
        /// <param name="credentialsService">
        /// The <see cref="ICredentialsService"/> used to provide authorization and <see cref="Credentials"/>
        /// services while handling a request
        /// </param>
        /// <param name="appConfigService">
        /// The <see cref="IAppConfigService"/> used to read applicaton settings
        /// </param>
        /// <param name="username">
        /// The username used to authorize
        /// </param>
        /// <returns>
        /// an awaitable <see cref="Task"/>
        /// </returns>
        protected async Task Authorize(IAppConfigService appConfigService, ICredentialsService credentialsService, string username)
        {
            try
            {
                await using var connection = new NpgsqlConnection(Services.Utils.GetConnectionString(appConfigService.AppConfig.Backtier, appConfigService.AppConfig.Backtier.Database));

                await connection.OpenAsync();

                await using var transaction = await connection.BeginTransactionAsync();

                await credentialsService.ResolveCredentials(transaction, username);
            }
            catch (Exception)
            {
                this.logger.LogWarning("Authorization failed for {username}", username);

                throw;
            }
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
        /// <param name="resourcePath">
        /// The resource Path.
        /// </param>
        /// <param name="requestUtils">
        /// The <see cref="IRequestUtils"/> that provides utilities that are valid for the current HttpRequest handling
        /// </param>
        /// <returns>
        /// The collection of retrieved <see cref="CDP4Common.DTO.Thing"/>.
        /// </returns>
        public IEnumerable<Thing> ProcessRequestPath(IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, IProcessor processor, string topContainer, string partition, string[] routeSegments, out List<Thing> resourcePath)
        {
            var containmentColl = new List<Thing>();
            var responseColl = new List<Thing>();
            var authorizedContext = new RequestSecurityContext { ContainerReadAllowed = true };
            resourcePath = new List<Thing>();

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

                if (serviceType == typeof(Iteration).Name && !generalResourceRequest)
                {
                    // switch to iteration context for further processing,
                    // in case of Iteration generalResource request, this is handled separately below
                    transactionManager.SetIterationContext(processor.Transaction, partition);
                }

                if (resourceContainmentSegment)
                {
                    // this part is always used except for the last tuple
                    var identifier = routeSegments[i + 1].ParseIdentifier();

                    processor.ValidateContainment(containmentColl, containerProperty, identifier);

                    var container =
                        processor.GetContainmentResource(serviceType, partition, identifier, authorizedContext);

                    if (serviceType == typeof(Iteration).Name)
                    {
                        // switch the partition (schema) for further processing, allowing retrieval of Iteration contained data
                        partition = partition.Replace("EngineeringModel", "Iteration");
                    }

                    // collect the specified containment resource
                    containmentColl.Add(container);
                    resourcePath.Add(container);

                    // authorized
                    authorizedContext.ContainerReadAllowed = true;
                }
                else if (generalResourceRequest)
                {
                    // get containment info
                    var containmentInfo = processor.GetContainment(containmentColl, containerProperty);

                    if (serviceType == typeof(Iteration).Name && containmentInfo != null)
                    {
                        // support temporal retrieval if iteration general resource is requested
                        // should only contain 1 element
                        foreach (var containedIterationId in containmentInfo)
                        {
                            // switch to iteration context for further processing
                            // use engineering-model id to set the iteration context
                            // partition here should be EngineeringModel_<uuid>
                            transactionManager.SetIterationContext(
                                processor.Transaction,
                                partition,
                                containedIterationId);

                            // collect resources
                            responseColl.AddRange(
                                processor.GetResource(
                                    serviceType,
                                    partition,
                                    new[] { containedIterationId },
                                    authorizedContext));
                        }
                    }
                    else
                    {
                        // collect resources
                        responseColl.AddRange(
                            processor.GetResource(serviceType, partition, containmentInfo, authorizedContext));
                    }
                }
                else if (specificResourceRequest)
                {
                    var identifier = routeSegments[i + 1].ParseIdentifier();

                    processor.ValidateContainment(containmentColl, containerProperty, identifier);

                    var resource = processor.GetResource(
                        serviceType,
                        partition,
                        new[] { identifier },
                        authorizedContext).ToList();
                    if (resource.Count == 0)
                    {
                        continue;
                    }

                    // collect resources
                    responseColl.AddRange(resource);

                    // set specific resource from uri request
                    resourcePath.Add(resource.First());
                }
            }

            if (requestUtils.QueryParameters.IncludeAllContainers)
            {
                responseColl.InsertRange(0, containmentColl);
            }

            return responseColl;
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
        protected async Task WriteJsonResponse(IHeaderInfoProvider headerInfoProvider, IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, IPermissionInstanceFilterService permissionInstanceFilterService, IReadOnlyList<Thing> resourceResponse, Version requestDataModelVersion, HttpResponse httpResponse, HttpStatusCode statusCode = HttpStatusCode.OK, string requestToken = "")
        {
            headerInfoProvider.RegisterResponseHeaders(httpResponse, ContentTypeKind.JSON, "");

            httpResponse.StatusCode = (int)statusCode;

            var resultStream = new MemoryStream();
            this.CreateFilteredJsonResponseStream(metaInfoProvider, jsonSerializer, permissionInstanceFilterService, resourceResponse, resultStream, requestDataModelVersion, requestToken);
            resultStream.Seek(0, SeekOrigin.Begin);
            await resultStream.CopyToAsync(httpResponse.Body);
        }

        /// <summary>
        /// Writes <see cref="Thing"/>s to a target <see cref="HttpResponse"/>
        /// </summary>
        /// <param name="headerInfoProvider">
        /// The injected <see cref="IHeaderInfoProvider"/> instance used to process HTTP headers
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
        protected async Task WriteMessagePackResponse(IHeaderInfoProvider headerInfoProvider, IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService, IReadOnlyList<Thing> resourceResponse, Version requestDataModelVersion, HttpResponse httpResponse, HttpStatusCode statusCode = HttpStatusCode.OK, string requestToken = "")
        {
            headerInfoProvider.RegisterResponseHeaders(httpResponse, ContentTypeKind.MESSAGEPACK, "");

            httpResponse.StatusCode = (int)statusCode;

            var resultStream = new MemoryStream();
            this.CreateFilteredMessagePackResponseStream(messagePackSerializer, permissionInstanceFilterService, resourceResponse, resultStream, requestDataModelVersion, requestToken);
            resultStream.Seek(0, SeekOrigin.Begin);
            await resultStream.CopyToAsync(httpResponse.Body);
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
        /// <param name="permissionInstanceFilterService">
        /// The <see cref="IPermissionInstanceFilterService"/> used to filter instances from the queried data
        /// </param>
        /// <param name="fileRevisions">
        /// The file revisions.
        /// </param>
        /// <param name="resourceResponse">
        /// The resource response.
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
        protected void WriteMultipartResponse(IHeaderInfoProvider headerInfoProvider, IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, IFileBinaryService fileBinaryService, IPermissionInstanceFilterService permissionInstanceFilterService, List<FileRevision> fileRevisions, List<Thing> resourceResponse, Version version, HttpResponse httpResponse, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            headerInfoProvider.RegisterResponseHeaders(httpResponse, ContentTypeKind.MULTIPARTMIXED, HttpConstants.BoundaryString);

            httpResponse.StatusCode = (int)statusCode;

            this.PrepareMultiPartResponse(metaInfoProvider,jsonSerializer, fileBinaryService, permissionInstanceFilterService,  httpResponse.Body, fileRevisions, resourceResponse, version);
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
        /// <returns>
        /// The <see cref="HttpResponse"/>.
        /// </returns>
        protected void WriteArchivedResponse(IHeaderInfoProvider headerInfoProvider, IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, IFileArchiveService fileArchiveService, IPermissionInstanceFilterService permissionInstanceFilterService, List<Thing> resourceResponse, string partition, string[] routeSegments, Version version, HttpResponse httpResponse, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            headerInfoProvider.RegisterResponseHeaders(httpResponse, ContentTypeKind.MULTIPARTMIXED, HttpConstants.BoundaryString);
            httpResponse.StatusCode = (int)statusCode;

            this.PrepareArchivedResponse(metaInfoProvider,jsonSerializer, fileArchiveService, permissionInstanceFilterService, httpResponse.Body, resourceResponse, version, partition, routeSegments);
        }

        /// <summary>
        /// Read the current state of the top container.
        /// </summary>
        /// <param name="serviceProvider">
        /// The <see cref="Services.IServiceProvider"/> that provides the service registry
        /// </param>
        /// <param name="transaction">
        /// The databse transaction.
        /// </param>
        /// <param name="partition">
        /// The database partition (schema) where the requested resource is stored.
        /// </param>
        /// <param name="topContainer">
        /// The Top Container type name.
        /// </param>
        /// <returns>
        /// A top container instance.
        /// </returns>
        protected Thing GetTopContainer(Services.IServiceProvider serviceProvider, NpgsqlTransaction transaction, string partition, string topContainer)
        {
            return serviceProvider.MapToReadService(topContainer).GetShallow(transaction, partition, null, new RequestSecurityContext { ContainerReadAllowed = true }).FirstOrDefault();
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
        protected IEnumerable<Thing> CollectReferenceDataLibraryChain(IRequestUtils requestUtils,
            IProcessor processor,
            EngineeringModelSetup modelSetup)
        {
            var securityContext = SetupSecurityContextForLibraryRead(requestUtils);

            // retrieve the required model reference data library
            var modelReferenceDataLibraryData = processor.GetResource(ModelReferenceDataLibraryType, SiteDirectoryData, modelSetup.RequiredRdl, securityContext);

            return RetrieveChainedReferenceData(requestUtils, processor, securityContext, modelReferenceDataLibraryData);
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
        protected IEnumerable<Thing> CollectReferenceDataLibraryChain(IRequestUtils requestUtils, IProcessor processor, ModelReferenceDataLibrary modelReferenceDataLibrary)
        {
            var securityContext = SetupSecurityContextForLibraryRead(requestUtils);

            // retrieve the required model reference data library with extent = deep
            var modelReferenceDataLibraryData = processor.GetResource(ModelReferenceDataLibraryType, SiteDirectoryData, new[] { modelReferenceDataLibrary.Iid }, securityContext);

            return RetrieveChainedReferenceData(requestUtils, processor, securityContext, modelReferenceDataLibraryData)
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
        private void CreateFilteredJsonResponseStream(IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, IPermissionInstanceFilterService permissionInstanceFilterService,
            IReadOnlyList<Thing> dtos,
            Stream stream,
            Version requestDataModelVersion,
            string requestToken = "")
        {
            var filteredDtos = permissionInstanceFilterService.FilterOutPermissions(dtos, requestDataModelVersion).ToArray();

            var sw = new Stopwatch();
            sw.Start();
            this.logger.LogDebug("{requestToken} start serializing dtos as JSON", requestToken);
            jsonSerializer.Initialize(metaInfoProvider, requestDataModelVersion);
            jsonSerializer.SerializeToStream(filteredDtos, stream);
            sw.Stop();

            this.logger.LogDebug("serializing dtos as JSON - {requestToken} in {sw} [ms]", requestToken, sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// Filters supplied DTO's and creates a JSON response stream based on an <see cref="IEnumerable{T}"/>
        /// </summary>
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
        private void CreateFilteredMessagePackResponseStream(IMessagePackSerializer messagePackSerializer, IPermissionInstanceFilterService permissionInstanceFilterService,
            IReadOnlyList<Thing> dtos,
            Stream stream,
            Version requestDataModelVersion,
            string requestToken = "")
        {
            var filteredDtos = permissionInstanceFilterService.FilterOutPermissions(dtos, requestDataModelVersion).ToArray();

            var sw = new Stopwatch();
            sw.Start();
            this.logger.LogDebug("{requestToken} start serializing dtos as MessagePack", requestToken);
            messagePackSerializer.SerializeToStream(filteredDtos, stream);
            sw.Stop();

            this.logger.LogDebug("serializing dtos as MessagePack - {requestToken} in {sw} [ms]", requestToken, sw.ElapsedMilliseconds);
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
        private void PrepareMultiPartResponse(IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, IFileBinaryService fileBinaryService, IPermissionInstanceFilterService permissionInstanceFilterService, Stream targetStream, List<FileRevision> fileRevisions, List<Thing> resourceResponse, Version requestDataModelVersion)
        {
            if (fileRevisions.Count == 0)
            {
                // do nothing if no file revisions are present
                return;
            }

            var content = new MultipartContent("mixed", HttpConstants.BoundaryString);
            using (var stream = new MemoryStream())
            {
                this.CreateFilteredJsonResponseStream(metaInfoProvider, jsonSerializer, permissionInstanceFilterService, resourceResponse, stream, requestDataModelVersion);

                // rewind stream prior to reading
                stream.Position = 0;

                // write out the json content to the first multipart content entry
                var jsonContent = new StringContent(new StreamReader(stream).ReadToEnd());
                jsonContent.Headers.Clear();
                jsonContent.Headers.Add(HttpConstants.ContentTypeHeader, HttpConstants.MimeTypeJson);
                content.Add(jsonContent);

                stream.Flush();
            }

            foreach (var hash in fileRevisions.Select(x => x.ContentHash).Distinct())
            {
                byte[] buffer;
                long fileSize;
                using (var fileStream = fileBinaryService.RetrieveBinaryData(hash))
                {
                    fileSize = fileStream.Length;
                    buffer = new byte[(int)fileSize];
                    fileStream.Read(buffer, 0, (int)fileSize);
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
            content.CopyToAsync(targetStream).Wait();
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
        private void PrepareArchivedResponse(IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, IFileArchiveService fileArchiveService, IPermissionInstanceFilterService permissionInstanceFilterService, Stream targetStream, List<Thing> resourceResponse, Version requestDataModelVersion, string partition, string[] routeSegments)
        {
            var folderPath = fileArchiveService.CreateFileStructure(resourceResponse, partition, routeSegments);

            try
            {
                var content = new MultipartContent("mixed", HttpConstants.BoundaryString);

                using (var stream = new MemoryStream())
                {
                    this.CreateFilteredJsonResponseStream(metaInfoProvider, jsonSerializer, permissionInstanceFilterService, resourceResponse, stream, requestDataModelVersion);

                    // rewind stream prior to reading
                    stream.Position = 0;

                    // write out the json content to the first multipart content entry
                    var jsonContent = new StringContent(new StreamReader(stream).ReadToEnd());
                    jsonContent.Headers.Clear();
                    jsonContent.Headers.Add(HttpConstants.ContentTypeHeader, HttpConstants.MimeTypeJson);
                    content.Add(jsonContent);

                    stream.Flush();
                }

                fileArchiveService.CreateZipArchive(folderPath);

                byte[] buffer;
                long fileSize;
                using (var fileStream = new FileStream(folderPath + ".zip", FileMode.Open))
                {
                    fileSize = fileStream.Length;
                    buffer = new byte[(int)fileSize];
                    fileStream.Read(buffer, 0, (int)fileSize);
                }

                var binaryContent = new ByteArrayContent(buffer);
                binaryContent.Headers.Add(HttpConstants.ContentTypeHeader, HttpConstants.MimeTypeOctetStream);

                // use the file hash value to easily identify the multipart content for each respective filerevision hash entry

                var fileInfo = new FileInfo($"{folderPath}.zip");
                
                binaryContent.Headers.Add(HttpConstants.ContentDispositionHeader, $"attachment; filename={fileInfo.Name}");

                binaryContent.Headers.Add(HttpConstants.ContentLengthHeader, fileSize.ToString());
                content.Add(binaryContent);

                // stream the multipart content to the request contents target stream
                content.CopyToAsync(targetStream).Wait();

                AddMultiPartMimeEndpoint(targetStream);
            }
            finally
            {
                fileArchiveService.DeleteFileStructureWithArchive(folderPath);
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
        private static ReadOnlyCollection<Thing> RetrieveChainedReferenceData(IRequestUtils requestUtils, IProcessor processor, ISecurityContext securityContext, IEnumerable<Thing> modelReferenceDataLibraryData)
        {
            var chainedReferenceDataColl = new List<Thing>();

            // do not collect the model reference data library again, as it is already present
            chainedReferenceDataColl.AddRange(modelReferenceDataLibraryData);
            var referenceDataLibrary = (ReferenceDataLibrary)chainedReferenceDataColl.First();

            // while the requiredRdl is set retrieve the reference data library chain bottom up
            while (referenceDataLibrary.RequiredRdl != null)
            {
                var siteReferenceDataLibraryData = processor.GetResource(
                    SiteReferenceDataLibraryType,
                    SiteDirectoryData,
                    new List<Guid> { (Guid)referenceDataLibrary.RequiredRdl },
                    securityContext).ToList();
                chainedReferenceDataColl.AddRange(siteReferenceDataLibraryData);
                referenceDataLibrary = (ReferenceDataLibrary)siteReferenceDataLibraryData.First();
            }

            // reset query parameters
            requestUtils.OverrideQueryParameters = null;

            return chainedReferenceDataColl.AsReadOnly();
        }
    }
}
