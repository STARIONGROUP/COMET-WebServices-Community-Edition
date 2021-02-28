// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelApi.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2021 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Ahmed Abulwafa Ahmed
//
//    This file is part of Comet Server Community Edition. 
//    The Comet Server Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The Comet Server Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The Comet Server Community Edition is distributed in the hope that it will be useful,
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
    using System.Text.RegularExpressions;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    
    using CDP4Orm.Dao;

    using CometServer.Configuration;
    using CometServer.Services.Authentication;
    using CometServer.Services.ChangeLog;

    using Helpers;

    using Microsoft.AspNetCore.Http;

    using NLog;
    using NLog.Targets;

    using Npgsql;
    
    using Services;
    using Services.Authorization;
    using Services.Operations;
    using Services.Protocol;
    
    using Thing = CDP4Common.DTO.Thing;
    using Utils = CometServer.Services.Utils;

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
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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
                QueryParameters.RevisionNumberQuery
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineeringModelApi"/> class.
        /// </summary>
        public EngineeringModelApi()
        {
            // enable basic authentication
            this.RequiresAuthentication();

            // support path segment processing
            this.Get[string.Format(this.ApiFormat, TopContainer, this.UrlSegmentMatcher)] = 
                route => this.GetResponse(route);

            // support trailing or empty segment
            this.Post[string.Format(this.ApiFormat, TopContainer, this.UrlSegmentMatcher)] =
                route => this.PostResponse(route);
        }

        /// <summary>
        /// Gets or sets the person resolver service.
        /// </summary>
        public IPersonResolver PersonResolver { get; set; }

        /// <summary>
        /// Gets or sets the change log service
        /// </summary>
        public IChangeLogService ChangeLogService { get; set; }

        /// <summary>
        /// Gets or sets the obfuscation service.
        /// </summary>
        public IObfuscationService ObfuscationService { get; set; }
        
        /// <summary>
        /// Parse the url segments and return the data as serialized JSON
        /// </summary>
        /// <param name="routeParams">
        /// A dynamic dictionary holding the route parameters
        /// </param>
        /// <returns>
        /// The serialized retrieved data or exception message
        /// </returns>
        protected override HttpResponse GetResponseData(dynamic routeParams)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            this.TransactionManager.SetCachedDtoReadEnabled(true);

            var sw = new Stopwatch();
            sw.Start();
            var requestToken = this.GenerateRandomToken();
            
            try
            {
                Logger.Info(this.ConstructLog($"{requestToken} started"));

                // validate (and set) the supplied query parameters
                HttpRequestHelper.ValidateSupportedQueryParameter(this.Request, this.RequestUtils, SupportedGetQueryParameters);

                // the route pattern enforces that there is at least one route segment
                string[] routeSegments = HttpRequestHelper.ParseRouteSegments(routeParams, TopContainer);

                var resourceResponse = new List<Thing>();
                var fromRevision = this.RequestUtils.QueryParameters.RevisionNumber;
                var iterationContextId = Guid.Empty;
                var iterationContextRequest = routeSegments.Length >= 4 &&
                                              routeSegments[2] == "iteration" &&
                                              Guid.TryParse(routeSegments[3], out iterationContextId);

                // get prepared data source transaction
                var credentials = this.RequestUtils.Context.AuthenticatedCredentials;

                transaction = iterationContextRequest
                    ? this.TransactionManager.SetupTransaction(ref connection, credentials, iterationContextId)
                    : this.TransactionManager.SetupTransaction(ref connection, credentials);

                var processor = new ResourceProcessor(this.ServiceProvider, transaction, this.RequestUtils);
                var modelSetup = this.DetermineEngineeringModelSetup(processor, routeSegments);
                var partition = this.RequestUtils.GetEngineeringModelPartitionString(modelSetup.EngineeringModelIid);

                // set the participant information
                if (credentials != null)
                {
                    credentials.EngineeringModelSetup = modelSetup;
                    this.PersonResolver.ResolveParticipantCredentials(transaction, credentials);
                    this.PermissionService.Credentials = credentials;
                }

                if (fromRevision > -1)
                {
                    // gather all Things that are newer then the indicated revision
                    resourceResponse.AddRange(this.RevisionService.Get(transaction, partition, fromRevision, false));
                }
                else if (this.RevisionResolver.TryResolve(transaction, partition, this.RequestUtils.QueryParameters.RevisionFrom, this.RequestUtils.QueryParameters.RevisionTo, out var resolvedValues))
                {
                    var iid = routeSegments.Last();

                    if (!Guid.TryParse(iid, out var guid))
                    {
                        var invalidRequest = new JsonResponse("The identifier of the object to query was not found or the route is invalid.", new DefaultJsonSerializer());
                        return invalidRequest.WithStatusCode(HttpStatusCode.BadRequest);
                    }
                    
                    resourceResponse.AddRange(this.RevisionService.Get(transaction, partition, guid, resolvedValues.FromRevision, resolvedValues.ToRevision));
                }
                else
                {
                    if (routeSegments.Length == 4 && routeSegments[2] == "iteration" && this.RequestUtils.QueryParameters.ExtentDeep)
                    {
                        string[] engineeringModelRouteSegments = { routeSegments[0], routeSegments[1] };

                        // gather all Things at engineeringmodel level as indicated by the request URI 
                        resourceResponse.AddRange(this.GetContainmentResponse(processor, partition, modelSetup, engineeringModelRouteSegments));

                        // find and remove the engineeringModelInstance, that will be retrieved in the second go.
                        var engineeringModel = resourceResponse.SingleOrDefault(x => x.ClassKind == ClassKind.EngineeringModel);
                        resourceResponse.Remove(engineeringModel);
                    }

                    // gather all Things as indicated by the request URI 
                    resourceResponse.AddRange(this.GetContainmentResponse(processor, partition, modelSetup, routeSegments));

                    if (!resourceResponse.Any())
                    {
                        var invalidRequest = new JsonResponse("The identifier of the object to query was not found or the route is invalid.", new DefaultJsonSerializer());
                        return invalidRequest.WithStatusCode(HttpStatusCode.BadRequest);
                    }
                }

                transaction.Commit();

                Logger.Info("{0} completed in {1} [ms]", requestToken, sw.ElapsedMilliseconds);

                // obfuscate if needed
                if (modelSetup.OrganizationalParticipant.Any())
                {
                    sw = new Stopwatch();
                    sw.Start();
                    Logger.Info("Model has Organizational Participation assigned. Obfuscation enabled.", requestToken, sw.ElapsedMilliseconds);

                    this.ObfuscationService.ObfuscateResponse(resourceResponse, credentials);

                    Logger.Info("{0} obfuscation completed in {1} [ms]", requestToken, sw.ElapsedMilliseconds);
                }

                var fileRevisions = resourceResponse.OfType<FileRevision>().ToList();
                if (this.RequestUtils.QueryParameters.IncludeFileData && fileRevisions.Any())
                {
                    // return multipart response including file binaries
                    return this.GetMultipartResponse(fileRevisions, resourceResponse);
                }

                if (this.RequestUtils.QueryParameters.IncludeFileData)
                {
                    var routeSegmentList = routeSegments.ToList();

                    if (this.IsValidDomainFileStoreArchiveRoute(routeSegmentList))
                    {
                        var iterationPartition = this.RequestUtils.GetIterationPartitionString(modelSetup.EngineeringModelIid);
                        return this.GetArchivedResponse(resourceResponse, iterationPartition, routeSegments);
                    }

                    if (this.IsValidCommonFileStoreArchiveRoute(routeSegmentList))
                    {
                        return this.GetArchivedResponse(resourceResponse, partition, routeSegments);
                    }
                }

                return this.GetJsonResponse(resourceResponse, this.RequestUtils.GetRequestDataModelVersion);
            }
            catch (Exception ex)
            {
                if (transaction != null && !transaction.IsCompleted)
                {
                    transaction.Rollback();
                }

                Logger.Error(ex, this.ConstructFailureLog($"{requestToken} failed after {sw.ElapsedMilliseconds} [ms]"));

                // error handling
                var errorResponse = new JsonResponse($"exception:{ex.Message}", new DefaultJsonSerializer());
                return errorResponse.WithStatusCode(HttpStatusCode.InternalServerError);
            }
            finally
            {
                transaction?.Dispose();

                connection?.Dispose();
            }
        }

        /// <summary>
        /// The post response data.
        /// </summary>
        /// <param name="routeParams">
        /// The route parameters.
        /// </param>
        /// <returns>
        /// The <see cref="HttpResponse"/>.
        /// </returns>
        protected override HttpResponse PostResponseData(dynamic routeParams)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            var sw = new Stopwatch();
            sw.Start();
            var requestToken = this.GenerateRandomToken();

            try
            {
                var logMessage = $"{requestToken} started";
                Logger.Info(this.ConstructLog(logMessage));

                HttpRequestHelper.ValidateSupportedQueryParameter(this.Request, this.RequestUtils, SupportedPostQueryParameter);
                
                var contentTypeRegex = new Regex("^multipart/.*;\\s*boundary=(.*)$", RegexOptions.IgnoreCase);
                var isMultiPart = contentTypeRegex.IsMatch(this.Request.Headers.ContentType);

                logMessage = $"Request {requestToken} is mutlipart: {isMultiPart}";
                Logger.Debug(this.ConstructLog(logMessage));

                Stream bodyStream;
                Dictionary<string, Stream> fileDictionary = null;
                if (isMultiPart)
                {
                    bodyStream = this.ExtractJsonBodyStreamFromMultiPartMessage();

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
                    fileDictionary = new Dictionary<string, Stream>();
                    foreach (var uploadedFile in this.Request.Files.ToList().Where(f => f.ContentType == this.MimeTypeOctetStream))
                    {
                        var hash = this.FileBinaryService.CalculateHashFromStream(uploadedFile.Value);

                        logMessage = $"File with hash {hash} present in request: {requestToken}";
                        Logger.Debug(this.ConstructLog(logMessage));

                        fileDictionary.Add(hash, uploadedFile.Value);
                    }
                }
                else
                {
                    // use the single request (JSON) stream
                    bodyStream = this.Request.Body;
                }

                this.JsonSerializer.Initialize(this.RequestUtils.MetaInfoProvider, this.RequestUtils.GetRequestDataModelVersion);
                var operationData = this.JsonSerializer.Deserialize<CdpPostOperation>(bodyStream);

                // get prepared data source transaction
                var credentials = this.RequestUtils.Context.AuthenticatedCredentials;
                transaction = this.TransactionManager.SetupTransaction(ref connection, credentials);

                // the route pattern enforces that there is atleast one route segment
                string[] routeSegments = string.Format("{0}/{1}", TopContainer, routeParams.uri)
                    .Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

                var resourceProcessor = new ResourceProcessor(
                    this.ServiceProvider,
                    transaction,
                    this.RequestUtils);

                var modelSetup = this.DetermineEngineeringModelSetup(resourceProcessor, routeSegments);

                var partition = this.RequestUtils.GetEngineeringModelPartitionString(modelSetup.EngineeringModelIid);

                if (credentials != null)
                {
                    this.PermissionService.Credentials = credentials;
                    this.PermissionService.Credentials.EngineeringModelSetup = modelSetup;
                    this.PersonResolver.ResolveParticipantCredentials(transaction, this.PermissionService.Credentials);
                    this.PermissionService.Credentials.IsParticipant = true;

                    var iteration = this.DetermineIteration(resourceProcessor, partition, routeSegments);
                    this.PermissionService.Credentials.Iteration = iteration;
                }

                // defer all reference data check until after transaction commit
                using (var command = new NpgsqlCommand("SET CONSTRAINTS ALL DEFERRED;", transaction.Connection, transaction))
                {
                    command.ExecuteAndLogNonQuery(this.TransactionManager.CommandLogger);
                }

                // retrieve the revision for this transaction (or get next revision if it does not exist)
                var transactionRevision = this.RevisionService.GetRevisionForTransaction(transaction, partition);

                this.OperationProcessor.Process(operationData, transaction, partition, fileDictionary);

                var actor = this.PermissionService.Credentials.Person.Iid;

                if (AppConfig.Current.Changelog.CollectChanges)
                {
                    var initiallyChangedThings = this.RevisionService.GetCurrentChanges(transaction, partition, transactionRevision, true).ToList();
                    this.ChangeLogService?.TryAppendModelChangeLogData(transaction, partition, actor, transactionRevision, operationData, initiallyChangedThings);
                }

                // save revision-history
                var changedThings = this.RevisionService.SaveRevisions(transaction, partition, actor, transactionRevision).ToList();

                transaction.Commit();

                if (this.RequestUtils.QueryParameters.RevisionNumber == -1)
                {
                    Logger.Info("{0} completed in {1} [ms]", requestToken, sw.ElapsedMilliseconds);
                    return this.GetJsonResponse(changedThings, this.RequestUtils.GetRequestDataModelVersion);
                }

                Logger.Info(this.ConstructLog());
                var fromRevision = this.RequestUtils.QueryParameters.RevisionNumber;

                // use new transaction to include latest database state
                transaction = this.TransactionManager.SetupTransaction(ref connection, credentials);
                var revisionResponse = this.RevisionService.Get(transaction, partition, fromRevision, true).ToArray();

                transaction.Commit();

                Logger.Info("{0} completed in {1} [ms]", requestToken, sw.ElapsedMilliseconds);

                return this.GetJsonResponse(revisionResponse, this.RequestUtils.GetRequestDataModelVersion);
            }
            catch (InvalidOperationException ex)
            {
                if (transaction != null && !transaction.IsCompleted)
                {
                    transaction.Rollback();
                }

                Logger.Error(ex, this.ConstructFailureLog($"{requestToken} failed after {sw.ElapsedMilliseconds} [ms]"));

                // error handling
                var errorResponse = new JsonResponse($"exception:{ex.Message}", new DefaultJsonSerializer());
                return errorResponse.WithStatusCode(HttpStatusCode.Forbidden);
            }
            catch (Exception ex)
            {
                if (transaction != null && !transaction.IsCompleted)
                {
                    transaction.Rollback();
                }

                Logger.Error(ex, this.ConstructFailureLog($"{requestToken} failed after {sw.ElapsedMilliseconds} [ms]"));

                // error handling
                var errorResponse = new JsonResponse($"exception:{ex.Message}", new DefaultJsonSerializer());
                return errorResponse.WithStatusCode(HttpStatusCode.InternalServerError);
            }
            finally
            {
                transaction?.Dispose();
                connection?.Dispose();
            }
        }

        /// <summary>
        /// Extracts the JSON part from the multi-part message
        /// </summary>
        /// <returns>
        /// A <see cref="Stream"/> that contains the posted JSON
        /// </returns>
        private Stream ExtractJsonBodyStreamFromMultiPartMessage()
        {
            var contentType = this.Request.Headers.ContentType;
            var boundary = Regex.Match(contentType, @"boundary=""?(?<token>[^\n\;\"" ]*)").Groups["token"].Value;
            var multipart = new HttpMultipart(this.Request.Body, boundary);
            var multipartBoundaries = multipart.GetBoundaries();
            var jsonMultipartBoundary = multipartBoundaries.SingleOrDefault(x => x.ContentType == "application/json");
            if (jsonMultipartBoundary == null)
            {
                throw new InvalidOperationException("A multipart request must contain a JSON part");
            }

            var bodyStream = jsonMultipartBoundary.Value;

            if (Logger.IsTraceEnabled)
            {
                var reader = new StreamReader(bodyStream);
                var multipartjson = reader.ReadToEnd();
                bodyStream.Position = 0;

                Logger.Trace("multipart post JSON: {0}", multipartjson);
            }

            return bodyStream;
        }

        /// <summary>
        /// Get the EngineeringModel containment response from the request path.
        /// </summary>
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
        private IEnumerable<Thing> GetContainmentResponse(
            IProcessor resourceProcessor,
            string partition,
            EngineeringModelSetup modelSetup,
            string[] routeSegments)
        {
            List<Thing> resolvedResourcePath;

            foreach (var thing in this.ProcessRequestPath(resourceProcessor, TopContainer, partition, routeSegments, out resolvedResourcePath))
            {
                yield return thing;
            }

            var credentials = this.RequestUtils.Context.AuthenticatedCredentials;
            var currentParticipantFlag = credentials.IsParticipant;
            if (this.RequestUtils.QueryParameters.IncludeReferenceData)
            {
                this.TransactionManager.SetDefaultContext(resourceProcessor.Transaction);

                foreach (var thing in this.CollectReferenceDataLibraryChain(resourceProcessor, modelSetup))
                {
                    yield return thing;
                }
            }
        }

        /// <summary>
        /// Determine the engineering model setup based on the supplied routeSegments.
        /// </summary>
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
        private EngineeringModelSetup DetermineEngineeringModelSetup(IProcessor processor, string[] routeSegments)
        {
            // override query parameters to return only extent shallow
            this.RequestUtils.OverrideQueryParameters = new QueryParameters();

            var securityContext = new RequestSecurityContext { ContainerReadAllowed = true };

            // set the transaction to default context to retrieve SiteDirectory data
            this.TransactionManager.SetDefaultContext(processor.Transaction);

            // take first segment and try to resolve the engineering model setup for further processing
            var siteDir = (SiteDirectory)processor.GetResource("SiteDirectory", SiteDirectoryData, null, securityContext).Single();
            var requestedModelId = CometServer.Services.Utils.ParseIdentifier(routeSegments[1]);
            var engineeringModelSetups = processor.GetResource("EngineeringModelSetup", SiteDirectoryData, siteDir.Model, securityContext);
            var modelSetups = engineeringModelSetups.Where(x => ((EngineeringModelSetup)x).EngineeringModelIid == requestedModelId).ToList();

            if (modelSetups.Count != 1)
            {
                throw new Exception("Engineering model could not be resolved");
            }

            // override query parameters to return only extent shallow
            this.RequestUtils.OverrideQueryParameters = null;

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
                var securityContext = new RequestSecurityContext { ContainerReadAllowed = true, Credentials = this.PermissionService.Credentials };

                var requestedIterationId = CometServer.Services.Utils.ParseIdentifier(routeSegments[3]);
                var iterations = processor.GetResource("Iteration", partition, new List<Guid> { requestedIterationId }, securityContext).ToList();

                if (iterations.Count != 1)
                {
                    throw new Exception("Iteration could not be resolved");
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
