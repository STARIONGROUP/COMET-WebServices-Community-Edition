// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineeringModelApi.cs" company="RHEA System S.A.">
//   Copyright (c) 2015-2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Orm.Dao;
    using CDP4WebServices.API.Services.Authentication;
    using Helpers;
    using Nancy;
    using Nancy.Responses;
    using Nancy.Security;
    using NLog;
    using Npgsql;
    using Services;
    using Services.Authorization;
    using Services.Operations;
    using Services.Protocol;
    using Thing = CDP4Common.DTO.Thing;
    using Utils = CDP4WebServices.API.Services.Utils;

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
        /// Parse the url segments and return the data as serialized JSON
        /// </summary>
        /// <param name="routeParams">
        /// A dynamic dictionary holding the route parameters
        /// </param>
        /// <returns>
        /// The serialized retrieved data or exception message
        /// </returns>
        protected override Response GetResponseData(dynamic routeParams)
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
                HttpRequestHelper.ValidateSupportedQueryParameter(this.Request, this.RequestUtils, new[]
                           {
                               QueryParameters.ExtentQuery,
                               QueryParameters.IncludeReferenceDataQuery,
                               QueryParameters.IncludeAllContainersQuery,
                               QueryParameters.IncludeFileDataQuery,
                               QueryParameters.RevisionNumberQuery,
                               QueryParameters.RevisionFromQuery,
                               QueryParameters.RevisionToQuery
                           });

                // the route pattern enforces that there is at least one route segment
                var routeSegments = HttpRequestHelper.ParseRouteSegments(routeParams, TopContainer);

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
                    resourceResponse.AddRange(this.RevisionService.Get(transaction, partition, fromRevision));
                }
                else if (this.RequestUtils.QueryParameters.RevisionFrom.HasValue || this.RequestUtils.QueryParameters.RevisionTo.HasValue)
                {
                    var iid = routeSegments.Last();
                    Guid guid;
                    if (!Guid.TryParse(iid, out guid))
                    {
                        var invalidRequest = new JsonResponse("The identifier of the object to query was not found or the route is invalid.", new DefaultJsonSerializer());
                        return invalidRequest.WithStatusCode(HttpStatusCode.BadRequest);
                    }

                    resourceResponse.AddRange(this.RevisionService.Get(transaction, TopContainer, guid, this.RequestUtils.QueryParameters.RevisionFrom ?? 0, this.RequestUtils.QueryParameters.RevisionTo ?? int.MaxValue));
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
                }

                transaction.Commit();
                
                Logger.Info("{0} completed in {1} [ms]", requestToken, sw.ElapsedMilliseconds);

                var fileRevisions = resourceResponse.Where(i => i.GetType() == typeof(FileRevision)).Cast<FileRevision>().ToList();
                if (this.RequestUtils.QueryParameters.IncludeFileData && fileRevisions.Any())
                {
                    // return multipart response including file binaries
                    return this.GetMultipartResponse(fileRevisions, resourceResponse);
                }

                if (this.RequestUtils.QueryParameters.IncludeFileData 
                    && ((routeSegments.Length == 4 && routeSegments[2] == "commonFileStore") || (routeSegments.Length == 6 && routeSegments[4] == "domainFileStore") 
                    || (routeSegments[2] == "commonFileStore" && (routeSegments.Length == 6 && routeSegments[4] == "folder"))
                    || (routeSegments[4] == "domainFileStore" && (routeSegments.Length == 8 && routeSegments[6] == "folder"))))
                {
                    // return archived response including file binaries and appropriate folder structure
                    return this.GetArchivedResponse(resourceResponse, partition, routeSegments);
                }

                return this.GetJsonResponse(resourceResponse, this.RequestUtils.GetRequestDataModelVersion);
            }
            catch (Exception ex)
            {
                if (transaction != null)
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
                if (transaction != null)
                {
                    transaction.Dispose();
                }

                if (connection != null)
                {
                    connection.Dispose();
                }            
            }
        }

        /// <summary>
        /// The post response data.
        /// </summary>
        /// <param name="routeParams">
        /// The route parameters.
        /// </param>
        /// <returns>
        /// The <see cref="Response"/>.
        /// </returns>
        protected override Response PostResponseData(dynamic routeParams)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            var sw = new Stopwatch();
            sw.Start();
            var requestToken = this.GenerateRandomToken();
            string logMessage;

            try
            {
                logMessage = $"{requestToken} started";
                Logger.Info(this.ConstructLog(logMessage));

                HttpRequestHelper.ValidateSupportedQueryParameter(this.Request, this.RequestUtils, new[] { QueryParameters.RevisionNumberQuery });
                
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
                var routeSegments = string.Format("{0}/{1}", TopContainer, routeParams.uri)
                    .Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

                var resourceProcessor = new ResourceProcessor(
                    this.ServiceProvider,
                    transaction,
                    this.RequestUtils);

                var modelSetup = this.DetermineEngineeringModelSetup(resourceProcessor, routeSegments);
                var partition = this.RequestUtils.GetEngineeringModelPartitionString(modelSetup.EngineeringModelIid);

                if (credentials != null)
                {
                    credentials.EngineeringModelSetup = modelSetup;
                    this.PersonResolver.ResolveParticipantCredentials(transaction, credentials);
                    credentials.IsParticipant = true;

                    this.PermissionService.Credentials = credentials;
                }

                // defer all reference data check until after transaction commit
                using (var command = new NpgsqlCommand("SET CONSTRAINTS ALL DEFERRED;", transaction.Connection, transaction))
                {
                    command.ExecuteAndLogNonQuery(this.TransactionManager.CommandLogger);
                }

                // retrieve topcontainer to acertain the current revision
                var topContainerInstance = this.GetTopContainer(transaction, partition, TopContainer);
                var fromRevision = topContainerInstance.RevisionNumber;

                this.OperationProcessor.Process(operationData, transaction, partition, fileDictionary);
                // save revision-history
                var actor = credentials.Person.Iid;
                var changedThings = (IEnumerable<Thing>)(this.RevisionService.SaveRevisions(transaction, partition, actor, fromRevision));

                transaction.Commit();

                if (this.RequestUtils.QueryParameters.RevisionNumber == -1)
                {
                    Logger.Info("{0} completed in {1} [ms]", requestToken, sw.ElapsedMilliseconds);
                    return this.GetJsonResponse(changedThings.ToList(), this.RequestUtils.GetRequestDataModelVersion);
                }

                Logger.Info(this.ConstructLog());
                fromRevision = this.RequestUtils.QueryParameters.RevisionNumber;

                // use new transaction to include latest database state
                transaction = this.TransactionManager.SetupTransaction(ref connection, credentials);
                var revisionResponse = ((IEnumerable<Thing>)this.RevisionService.Get(transaction, partition, fromRevision)).ToArray();
                transaction.Commit();
                
                Logger.Info("{0} completed in {1} [ms]", requestToken, sw.ElapsedMilliseconds);

                return this.GetJsonResponse(revisionResponse, this.RequestUtils.GetRequestDataModelVersion);
            }            
            catch (InvalidOperationException ex)
            {
                if (transaction != null)
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
                if (transaction != null)
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
                if (transaction != null)
                {
                    transaction.Dispose();
                }

                if (connection != null)
                {
                    connection.Dispose();
                }
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
            var requestedModelId = Utils.ParseIdentifier(routeSegments[1]);
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
    }
}