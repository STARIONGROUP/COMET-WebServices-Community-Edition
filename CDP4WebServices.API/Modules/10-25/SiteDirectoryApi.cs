// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteDirectoryApi.cs" company="RHEA System S.A.">
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

    using CDP4Common.DTO;

    using CDP4Orm.Dao;

    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.Operations;
    using CometServer.Services.Protocol;

    using Helpers;

    using Microsoft.AspNetCore.Http;

    using Newtonsoft.Json;

    using NLog;
    using NLog.Targets;

    using Npgsql;

    using Thing = CDP4Common.DTO.Thing;

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
        /// The Site Directory root path format template.
        /// </summary>
        private const string SiteDirectoryRootFormat = "/{0}";

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
                QueryParameters.RevisionNumberQuery, 
                QueryParameters.ExportQuery
            };

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteDirectoryApi"/> class.
        /// </summary>
        public SiteDirectoryApi()
        {
            // enable basic authentication
            this.RequiresAuthentication();

            // support trailing or empty segment
            this.Get[string.Format(SiteDirectoryRootFormat, TopContainer)] = 
                route => this.GetResponse(route);

            // support path segment processing
            this.Get[string.Format(this.ApiFormat, TopContainer, this.UrlSegmentMatcher)] =
                route => this.GetResponse(route);

            // support trailing or empty segment
            this.Post[string.Format(SiteDirectoryRootFormat, TopContainer)] = 
                route => this.PostResponse(route);

            this.Post[string.Format(this.ApiFormat, TopContainer, this.UrlSegmentMatcher)] =
                route => this.PostResponse(route);
        }

        /// <summary>
        /// Gets or sets the model creator manager service.
        /// </summary>
        public IModelCreatorManager ModelCreatorManager { get; set; }

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
                Logger.Info(this.ConstructLog($"{requestToken} database operations started"));

                // validate (and set) the supplied query parameters
                HttpRequestHelper.ValidateSupportedQueryParameter(this.Request, this.RequestUtils, SupportedGetQueryParameters);

                var fromRevision = this.RequestUtils.QueryParameters.RevisionNumber;
                var resourceResponse = new List<Thing>();

                // get prepared data source transaction
                var credentials = this.RequestUtils.Context.AuthenticatedCredentials;
                transaction = this.TransactionManager.SetupTransaction(ref connection, credentials);

                if (fromRevision > -1)
                {
                    // gather all Things that are newer then the indicated revision
                    resourceResponse.AddRange(this.RevisionService.Get(transaction, TopContainer, fromRevision, true).ToList());
                }
                else if (this.RevisionResolver.TryResolve(transaction, TopContainer, this.RequestUtils.QueryParameters.RevisionFrom, this.RequestUtils.QueryParameters.RevisionTo, out var resolvedValues))
                {
                    string[] routeSegments = HttpRequestHelper.ParseRouteSegments(routeParams, TopContainer);
                    var iid = routeSegments.Last();

                    if (!Guid.TryParse(iid, out var guid))
                    {
                        var invalidRequest = new JsonResponse(
                            "The identifier of the object to query was not found or the route is invalid.",
                            new DefaultJsonSerializer());

                        return invalidRequest.WithStatusCode(HttpStatusCode.BadRequest);
                    }

                    resourceResponse.AddRange(this.RevisionService.Get(transaction, TopContainer, guid, resolvedValues.FromRevision, resolvedValues.ToRevision));
                }
                else
                {
                    // gather all Things as indicated by the request URI 
                    resourceResponse.AddRange(this.GetContainmentResponse(transaction, TopContainer, routeParams));
                }

                transaction.Commit();

                sw.Stop();
                Logger.Info("Database operations {0} completed in {1} [ms]", requestToken, sw.ElapsedMilliseconds);

                sw.Start();
                Logger.Info("return {0} response started", requestToken);

                return this.GetJsonResponse(
                    resourceResponse,
                    this.RequestUtils.GetRequestDataModelVersion,
                    HttpStatusCode.OK,
                    requestToken);
            }
            catch (Exception ex)
            {
                if (transaction != null && !transaction.IsCompleted)
                {
                    transaction.Rollback();
                }

                Logger.Error(ex, this.ConstructFailureLog($"{requestToken} failed after {sw.ElapsedMilliseconds} [ms]"));

                // error handling
                var errorResponse = new JsonResponse(string.Format("exception:{0}", ex.Message), new DefaultJsonSerializer());
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

                sw.Stop();
                Logger.Info("Response {0} returned in {1} [ms]", requestToken, sw.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// The post response data.
        /// </summary>
        /// <param name="routeParams">
        /// The route parameters.
        /// </param>
        /// <remarks>
        /// multipart handling sample source <see cref="http://www.applandeo.com/en/net-and-nancy-parsing-multipartform-data-requests/"/>
        /// </remarks>
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
                Logger.Info(this.ConstructLog($"{requestToken} started"));

                HttpRequestHelper.ValidateSupportedQueryParameter(this.Request, this.RequestUtils, SupportedPostQueryParameter);

                var contentTypeRegex = new Regex("^multipart/.*;\\s*boundary=(.*)$", RegexOptions.IgnoreCase);
                Stream bodyStream;
                bool isMultiPart = contentTypeRegex.IsMatch(this.Request.Headers.ContentType);

                if (isMultiPart)
                {
                    // multipart message received
                    throw new InvalidOperationException(
                        string.Format("Multipart post messages are not allowed for the {0} route", TopContainer));
                }
                else
                {
                    bodyStream = this.Request.Body;
                }

                if (this.RequestUtils.QueryParameters.Export)
                {
                    // convert stream to string
                    StreamReader reader = new StreamReader(bodyStream);
                    string text = reader.ReadToEnd();

                    var modelSetupGuidList = JsonConvert.DeserializeObject<List<Guid>>(text);

                    // use jsonb for the zipped response
                    this.TransactionManager.SetCachedDtoReadEnabled(true);

                    return this.GetZippedModelsResponse(this.RequestUtils.GetRequestDataModelVersion, modelSetupGuidList, HttpStatusCode.OK, requestToken);
                }

                this.JsonSerializer.Initialize(this.RequestUtils.MetaInfoProvider, this.RequestUtils.GetRequestDataModelVersion);
                var operationData = this.JsonSerializer.Deserialize<CdpPostOperation>(bodyStream);

                // get prepared data source transaction
                var credentials = this.RequestUtils.Context.AuthenticatedCredentials;

                transaction = this.TransactionManager.SetupTransaction(ref connection, credentials);

                // defer all reference data check until after transaction commit
                using (var command = new NpgsqlCommand("SET CONSTRAINTS ALL DEFERRED", transaction.Connection, transaction))
                {
                    command.ExecuteAndLogNonQuery(this.TransactionManager.CommandLogger);
                }

                // retrieve the revision for this transaction (or get next revision if it does not exist)
                var transactionRevision = this.RevisionService.GetRevisionForTransaction(transaction, TopContainer);

                this.OperationProcessor.Process(operationData, transaction, TopContainer);

                // save revision-history
                var actor = credentials.Person.Iid;
                var changedThings = this.RevisionService.SaveRevisions(transaction, TopContainer, actor, transactionRevision).ToList();

                // commit the operation + revision-history
                transaction.Commit();
                if (this.ModelCreatorManager.IsUserTriggerDisable)
                {
                    // re-enable user triggers
                    transaction = this.TransactionManager.SetupTransaction(ref connection, credentials);
                    this.ModelCreatorManager.EnableUserTrigger(transaction);
                    transaction.Commit();
                }

                if (this.RequestUtils.QueryParameters.RevisionNumber == -1)
                {
                    Logger.Info("{0} completed in {1} [ms]", requestToken, sw.ElapsedMilliseconds);
                    return this.GetJsonResponse(changedThings, this.RequestUtils.GetRequestDataModelVersion);
                }

                // get the latest revision state including revisions that may have happened meanwhile
                Logger.Info(this.ConstructLog());
                var fromRevision = this.RequestUtils.QueryParameters.RevisionNumber;

                // use new transaction to include latest database state
                transaction = this.TransactionManager.SetupTransaction(ref connection, credentials);
                var revisionResponse = this.RevisionService.Get(transaction, TopContainer, fromRevision, true).ToArray();
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
                var errorResponse = new JsonResponse(string.Format("exception:{0}", ex.Message), new DefaultJsonSerializer());
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
                var errorResponse = new JsonResponse(string.Format("exception:{0}", ex.Message), new DefaultJsonSerializer());
                return errorResponse.WithStatusCode(HttpStatusCode.InternalServerError);
            }
            finally
            {
                transaction?.Dispose();
                connection?.Dispose();
            }
        }

        /// <summary>
        /// Get the SiteDirectory containment response from the request path.
        /// </summary>
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
        private IEnumerable<Thing> GetContainmentResponse(
            NpgsqlTransaction transaction,
            string partition,
            dynamic routeParams)
        {
            var processor = new ResourceProcessor(
                this.ServiceProvider,
                transaction,
                this.RequestUtils);

            if (routeParams.Count == 0)
            {
                // sitedirectory singleton resource request (IncludeReferenceData is handled in the sitedirectory service logic)
                foreach (var thing in processor.GetResource(TopContainer, partition, null, new RequestSecurityContext { ContainerReadAllowed = true }))
                {
                    yield return thing;
                }
            }
            else
            {
                // process containment path
                var routeSegments = HttpRequestHelper.ParseRouteSegments(routeParams, TopContainer);

                List<Thing> resolvedResourcePath;
                foreach (var thing in this.ProcessRequestPath(processor, TopContainer, partition, routeSegments, out resolvedResourcePath))
                {
                    yield return thing;
                }

                if (resolvedResourcePath.Count() > 1 && this.RequestUtils.QueryParameters.IncludeReferenceData)
                {
                    // add reference data information if the resource is a model reference data library
                    if (resolvedResourcePath.Last().GetType() == typeof(ModelReferenceDataLibrary))
                    {
                        foreach (var thing in this.CollectReferenceDataLibraryChain(processor, (ModelReferenceDataLibrary)resolvedResourcePath.Last()))
                        {
                            yield return thing;
                        }
                    }
                }
            }
        }
    }
}
