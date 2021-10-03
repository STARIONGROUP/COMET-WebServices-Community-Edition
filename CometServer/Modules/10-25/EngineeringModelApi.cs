﻿// --------------------------------------------------------------------------------------------------------------------
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
    using System.Security;
    using System.Threading.Tasks;

    using Carter.Response;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.Authorization;
    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.ChangeLog;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations;
    using CometServer.Services.Protocol;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.WebUtilities;

    using NLog;

    using Npgsql;

    using Thing = CDP4Common.DTO.Thing;

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
        /// Gets or sets the change log service
        /// </summary>
        public IChangeLogService ChangeLogService { get; set; }

        /// <summary>
        /// Gets or sets the obfuscation service.
        /// </summary>
        public IObfuscationService ObfuscationService { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineeringModelApi"/> class.
        /// </summary>
        public EngineeringModelApi()
        {
            this.Get("EngineeringModel/{*path}", async (req, res) =>
            {
                if (!req.HttpContext.User.Identity.IsAuthenticated)
                {
                    res.UpdateWithNotAuthenticatedSettings();
                    await res.AsJson("not authenticated");
                }
                else
                {
                    await this.Authorize(req.HttpContext.User.Identity.Name);

                    await this.GetResponseData(req, res);
                }
            });

            this.Post("EngineeringModel/{engineeringModelIid:guid}/iteration/{iterationIid:guid}", async (req, res) =>
            {
                if (!req.HttpContext.User.Identity.IsAuthenticated)
                {
                    res.UpdateWithNotAuthenticatedSettings();
                    await res.AsJson("not authenticated");
                }
                else
                {
                    await this.Authorize(req.HttpContext.User.Identity.Name);

                    await this.PostResponseData(req, res);
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
        /// <returns>
        /// An awaitable <see cref="Task"/>
        /// </returns>
        protected async Task GetResponseData(HttpRequest httpRequest, HttpResponse httpResponse)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            this.TransactionManager.SetCachedDtoReadEnabled(true);

            var sw = new Stopwatch();
            sw.Start();
            var requestToken = this.GenerateRandomToken();
            
            try
            {
                Logger.Info(this.ConstructLog(httpRequest, $"{requestToken} started"));

                // validate (and set) the supplied query parameters
                HttpRequestHelper.ValidateSupportedQueryParameter(httpRequest.Query, SupportedGetQueryParameters);

                this.RequestUtils.QueryParameters = new QueryParameters(httpRequest.Query);
                
                var version = this.RequestUtils.GetRequestDataModelVersion(httpRequest);

                // the route pattern enforces that there is at least one route segment
                var routeSegments = HttpRequestHelper.ParseRouteSegments(httpRequest.Path);

                var resourceResponse = new List<Thing>();
                var fromRevision = this.RequestUtils.QueryParameters.RevisionNumber;
                var iterationContextId = Guid.Empty;
                var iterationContextRequest = routeSegments.Length >= 4 &&
                                              routeSegments[2] == "iteration" &&
                                              Guid.TryParse(routeSegments[3], out iterationContextId);

                // get prepared data source transaction
                var credentials = this.CredentialsService.Credentials;

                transaction = iterationContextRequest
                    ? this.TransactionManager.SetupTransaction(ref connection, credentials, iterationContextId)
                    : this.TransactionManager.SetupTransaction(ref connection, credentials);

                var processor = new ResourceProcessor(transaction, this.ServiceProvider , this.RequestUtils, this.MetaInfoProvider);
                var modelSetup = this.DetermineEngineeringModelSetup(processor, routeSegments);
                var partition = this.RequestUtils.GetEngineeringModelPartitionString(modelSetup.EngineeringModelIid);

                // set the participant information
                if (credentials != null)
                {
                    credentials.EngineeringModelSetup = modelSetup;
                    this.CredentialsService.ResolveParticipantCredentials(transaction);
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
                        await httpResponse.AsJson("The identifier of the object to query was not found or the route is invalid.");
                        httpResponse.StatusCode = (int) HttpStatusCode.BadRequest;
                        return;
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
                        await httpResponse.AsJson("The identifier of the object to query was not found or the route is invalid.");
                        httpResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                        return;
                    }
                }

                await transaction.CommitAsync();

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
                    await this.WriteMultipartResponse(fileRevisions, resourceResponse, version, httpResponse);
                    return;
                }

                if (this.RequestUtils.QueryParameters.IncludeFileData)
                {
                    var routeSegmentList = routeSegments.ToList();

                    if (this.IsValidDomainFileStoreArchiveRoute(routeSegmentList))
                    {
                        var iterationPartition = this.RequestUtils.GetIterationPartitionString(modelSetup.EngineeringModelIid);

                        await this.WriteArchivedResponse(resourceResponse, iterationPartition, routeSegments, version, httpResponse);
                        return;
                    }

                    if (this.IsValidCommonFileStoreArchiveRoute(routeSegmentList))
                    {
                        await this.WriteArchivedResponse(resourceResponse, partition, routeSegments, version, httpResponse);
                        return;
                    }
                }

                await this.WriteJsonResponse(resourceResponse, this.RequestUtils.GetRequestDataModelVersion(httpRequest), httpResponse, HttpStatusCode.OK, requestToken);
            }
            catch (SecurityException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                Logger.Debug(ex, this.ConstructFailureLog(httpRequest, $"unauthorized request {requestToken} returned after {sw.ElapsedMilliseconds} [ms]"));

                httpResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                Logger.Error(ex, this.ConstructFailureLog(httpRequest, $"{requestToken} failed after {sw.ElapsedMilliseconds} [ms]"));

                // error handling
                httpResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            finally
            {
                transaction?.Dispose();

                connection?.Dispose();
            }
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
        /// <returns>
        /// An awaitable <see cref="Task"/>
        /// </returns>
        protected async Task PostResponseData(HttpRequest httpRequest, HttpResponse httpResponse)
        {
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            var sw = new Stopwatch();
            sw.Start();
            var requestToken = this.GenerateRandomToken();

            try
            {
                Logger.Info(this.ConstructLog(httpRequest, $"{requestToken} started"));

                HttpRequestHelper.ValidateSupportedQueryParameter(httpRequest.Query, SupportedPostQueryParameter);
                this.RequestUtils.QueryParameters = new QueryParameters(httpRequest.Query);

                var multiPartBoundary = httpRequest.GetMultipartBoundary();

                var isMultiPart = multiPartBoundary != string.Empty;

                Logger.Debug(this.ConstructLog(httpRequest, $"Request {requestToken} is mutlipart: {isMultiPart}"));

                Stream bodyStream;
                Dictionary<string, Stream> fileDictionary = null;

                if (isMultiPart)
                {
                    var requestStream = new MemoryStream();
                    await httpRequest.Body.CopyToAsync(requestStream);

                    bodyStream = await this.ExtractJsonBodyStreamFromMultiPartMessage(requestStream, multiPartBoundary);
                    fileDictionary = await this.ExtractFilesFromMultipartMessage(requestStream, multiPartBoundary);

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

                this.JsonSerializer.Initialize(this.MetaInfoProvider, this.RequestUtils.GetRequestDataModelVersion(httpRequest));
                var operationData = this.JsonSerializer.Deserialize<CdpPostOperation>(bodyStream);

                // get prepared data source transaction
                var credentials = this.CredentialsService.Credentials;
                transaction = this.TransactionManager.SetupTransaction(ref connection, credentials);

                // the route pattern enforces that there is atleast one route segment
                var routeSegments = HttpRequestHelper.ParseRouteSegments(httpRequest.Path);

                var resourceProcessor = new ResourceProcessor(transaction, this.ServiceProvider, this.RequestUtils, this.MetaInfoProvider);

                var modelSetup = this.DetermineEngineeringModelSetup(resourceProcessor, routeSegments);

                var partition = this.RequestUtils.GetEngineeringModelPartitionString(modelSetup.EngineeringModelIid);

                if (credentials != null)
                {
                    this.CredentialsService.Credentials.EngineeringModelSetup = modelSetup;
                    this.CredentialsService.ResolveParticipantCredentials(transaction);
                    this.CredentialsService.Credentials.IsParticipant = true;

                    var iteration = this.DetermineIteration(resourceProcessor, partition, routeSegments);
                    this.CredentialsService.Credentials.Iteration = iteration;
                }

                // defer all reference data check until after transaction commit
                using (var command = new NpgsqlCommand("SET CONSTRAINTS ALL DEFERRED;", transaction.Connection, transaction))
                {
                    command.ExecuteAndLogNonQuery(this.TransactionManager.CommandLogger);
                }

                // retrieve the revision for this transaction (or get next revision if it does not exist)
                var transactionRevision = this.RevisionService.GetRevisionForTransaction(transaction, partition);

                this.OperationProcessor.Process(operationData, transaction, partition, fileDictionary);

                var actor = this.CredentialsService.Credentials.Person.Iid;

                if (this.AppConfigService.AppConfig.Changelog.CollectChanges)
                {
                    var initiallyChangedThings = this.RevisionService.GetCurrentChanges(transaction, partition, transactionRevision, true).ToList();
                    this.ChangeLogService?.TryAppendModelChangeLogData(transaction, partition, actor, transactionRevision, operationData, initiallyChangedThings);
                }

                // save revision-history
                var changedThings = this.RevisionService.SaveRevisions(transaction, partition, actor, transactionRevision).ToList();

                await transaction.CommitAsync();

                if (this.RequestUtils.QueryParameters.RevisionNumber == -1)
                {
                    Logger.Info("{0} completed in {1} [ms]", requestToken, sw.ElapsedMilliseconds);
                    await this.WriteJsonResponse(changedThings, this.RequestUtils.GetRequestDataModelVersion(httpRequest), httpResponse);
                    return;
                }

                Logger.Info(this.ConstructLog(httpRequest));
                var fromRevision = this.RequestUtils.QueryParameters.RevisionNumber;

                // use new transaction to include latest database state
                transaction = this.TransactionManager.SetupTransaction(ref connection, credentials);
                var revisionResponse = this.RevisionService.Get(transaction, partition, fromRevision, true).ToArray();

                await transaction.CommitAsync();

                Logger.Info("{0} completed in {1} [ms]", requestToken, sw.ElapsedMilliseconds);

                await this.WriteJsonResponse(revisionResponse, this.RequestUtils.GetRequestDataModelVersion(httpRequest), httpResponse);
            }
            catch (InvalidOperationException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                Logger.Error(ex, this.ConstructFailureLog(httpRequest, $"{requestToken} failed after {sw.ElapsedMilliseconds} [ms]"));

                // error handling
                httpResponse.StatusCode = (int) HttpStatusCode.Forbidden;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (SecurityException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                Logger.Debug(ex, this.ConstructFailureLog(httpRequest, $"unauthorized request {requestToken} returned after {sw.ElapsedMilliseconds} [ms]"));

                // error handling
                httpResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                Logger.Error(ex, this.ConstructFailureLog(httpRequest, $"{requestToken} failed after {sw.ElapsedMilliseconds} [ms]"));

                // error handling
                httpResponse.StatusCode = (int) HttpStatusCode.InternalServerError;
                await httpResponse.AsJson($"exception:{ex.Message}");
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
        /// <param name="httpRequest">
        /// The <see cref="HttpRequest"/> that contains the multi-part messsage
        /// </param>
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

                    if (Logger.IsTraceEnabled)
                    {
                        var streamReader = new StreamReader(bodyStream);
                        var multipartjson = streamReader.ReadToEnd();
                        bodyStream.Position = 0;

                        Logger.Trace("multipart post JSON: {0}", multipartjson);
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
        private async Task<Dictionary<string, Stream>> ExtractFilesFromMultipartMessage(Stream stream, string boundary)
        {
            var fileDictionary = new Dictionary<string, Stream>();

            stream.Seek(0, SeekOrigin.Begin);
            var multipartReader = new MultipartReader(boundary, stream);

            var section = await multipartReader.ReadNextSectionAsync();

            while (section != null)
            {
                if (section.ContentType == this.MimeTypeOctetStream)
                {
                    var hash = this.FileBinaryService.CalculateHashFromStream(section.Body);

                    fileDictionary.Add(hash, section.Body);
                }

                section = await multipartReader.ReadNextSectionAsync();
            }

            return fileDictionary;
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
        private IEnumerable<Thing> GetContainmentResponse(IProcessor resourceProcessor, string partition, EngineeringModelSetup modelSetup, string[] routeSegments)
        {
            List<Thing> resolvedResourcePath;

            foreach (var thing in this.ProcessRequestPath(resourceProcessor, TopContainer, partition, routeSegments, out resolvedResourcePath))
            {
                yield return thing;
            }

            var credentials = this.CredentialsService.Credentials;
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
            var requestedModelId = routeSegments[1].ParseIdentifier();
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
                var securityContext = new RequestSecurityContext { ContainerReadAllowed = true };

                var requestedIterationId = routeSegments[3].ParseIdentifier();
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