// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CherryPickApi.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
// 
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski,
//                 Ahmed Abulwafa Ahmed
// 
//    This file is part of CDP4 Web Services Community Edition.
//    The CDP4 Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
// 
//    The CDP4 Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4 Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.Extensions;

    using CDP4WebServices.API.Helpers;
    using CDP4WebServices.API.Services;
    using CDP4WebServices.API.Services.Authentication;
    using CDP4WebServices.API.Services.Authorization;
    using CDP4WebServices.API.Services.CherryPick;
    using CDP4WebServices.API.Services.Protocol;

    using Nancy;
    using Nancy.Responses;
    using Nancy.Security;

    using NLog;

    using Npgsql;

    using Thing = CDP4Common.DTO.Thing;

    /// <summary>
    /// This is an API endpoint class to support cherry picking of <see cref="Thing" /> inside an
    /// <see cref="Iteration" />
    /// based on <see cref="Category" /> filter
    /// </summary>
    public class CherryPickApi : ApiBase
    {
        /// <summary>
        /// The url to reach this api
        /// </summary>
        private const string RouteUrl = "cherrypick/EngineeringModel/{engineeringModelId}/Iteration/{iterationId}";

        /// <summary>
        /// A <see cref="NLog.Logger" /> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The supported get query parameters.
        /// </summary>
        private static readonly string[] SupportedGetQueryParameters =
        {
            QueryParameters.ClassKindQuery,
            QueryParameters.CategoryQuery
        };

        /// <summary>
        /// Initializes a new instance of <see cref="CherryPickApi" />
        /// </summary>
        public CherryPickApi()
        {
            this.RequiresAuthentication();

            this.Get[RouteUrl] =
                route => this.GetResponse(route);

            this.Post[RouteUrl] =
                route => this.PostResponse(route);
        }

        /// <summary>
        /// Gets or sets the person resolver service.
        /// </summary>
        public IPersonResolver PersonResolver { get; set; }

        /// <summary>
        /// Gets or sets the obfuscation service.
        /// </summary>
        public IObfuscationService ObfuscationService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ICherryPickService"/>
        /// </summary>
        public ICherryPickService CherryPickService { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IContainmentService"/>
        /// </summary>
        public IContainmentService ContainmentService { get; set; }

        /// <summary>
        /// Abstract method contract for get response data processing.
        /// </summary>
        /// <param name="routeParams">
        /// The route parameters from the request.
        /// </param>
        /// <returns>
        /// The <see cref="Response" />.
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
                HttpRequestHelper.ValidateSupportedQueryParameter(this.Request, this.RequestUtils, SupportedGetQueryParameters);

                if (!this.RequestUtils.QueryParameters.ClassKinds.Any() || !this.RequestUtils.QueryParameters.CategoriesId.Any())
                {
                    var invalidRequest = new JsonResponse($"The {QueryParameters.ClassKindQuery} and {QueryParameters.CategoryQuery} parameters are required", new DefaultJsonSerializer());
                    return invalidRequest.WithStatusCode(HttpStatusCode.BadRequest);
                }

                var routeSegments = new string[] { "EngineeringModel", routeParams["engineeringModelId"], "iteration", routeParams["iterationId"] };

                if (!Guid.TryParse(routeSegments[3], out var iterationContextId))
                {
                    var invalidRequest = new JsonResponse("The route parameter iterationId does not match to a UUID", new DefaultJsonSerializer());
                    return invalidRequest.WithStatusCode(HttpStatusCode.BadRequest);
                }

                var credentials = this.RequestUtils.Context.AuthenticatedCredentials;
                transaction = this.TransactionManager.SetupTransaction(ref connection, credentials, iterationContextId);

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

                var securityContext = new RequestSecurityContext();

                if (this.RequestUtils.QueryParameters.ClassKinds.Any(classKind => !this.PermissionService.CanRead(classKind.ToString(), securityContext, partition)))
                {
                    var invalidRequest = new JsonResponse("", new DefaultJsonSerializer());
                    return invalidRequest.WithStatusCode(HttpStatusCode.MethodNotAllowed);
                }

                var resourceResponse = new List<Thing>();

                // gather all Things of the Iteration
                this.RequestUtils.QueryParameters.ExtentDeep = true;
                resourceResponse.AddRange(this.GetContainmentResponse(processor, partition, modelSetup, routeSegments));

                if (!resourceResponse.Any())
                {
                    var invalidRequest = new JsonResponse("The identifier of the object to query was not found or the route is invalid.", new DefaultJsonSerializer());
                    return invalidRequest.WithStatusCode(HttpStatusCode.BadRequest);
                }

                var cherryPickedThings = this.CherryPickService.CherryPick(resourceResponse, this.RequestUtils.QueryParameters.ClassKinds, this.RequestUtils.QueryParameters.CategoriesId)
                    .ToList();

                var containedThings = this.ContainmentService.QueryContainedThings(cherryPickedThings, resourceResponse, true,
                    ClassKind.Parameter, ClassKind.ParameterOverride, ClassKind.ParameterValueSet, ClassKind.ParameterOverrideValueSet);

                var containers = this.ContainmentService.QueryContainersTree(cherryPickedThings, resourceResponse);

                cherryPickedThings.AddRange(containedThings);
                cherryPickedThings.AddRange(containers);
                cherryPickedThings = cherryPickedThings.DistinctBy(x => x.Iid).ToList();

                // obfuscate if needed
                if (!modelSetup.OrganizationalParticipant.Any())
                {
                    return this.GetJsonResponse(cherryPickedThings, this.RequestUtils.GetRequestDataModelVersion);
                }

                sw = new Stopwatch();
                sw.Start();
                Logger.Info("Model has Organizational Participation assigned. Obfuscation enabled.");

                this.ObfuscationService.ObfuscateResponse(resourceResponse, credentials);

                Logger.Info("{0} obfuscation completed in {1} [ms]", requestToken, sw.ElapsedMilliseconds);

                return this.GetJsonResponse(cherryPickedThings, this.RequestUtils.GetRequestDataModelVersion);
            }
            catch (Exception ex)
            {
                if (transaction is { IsCompleted: false })
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
        /// Abstract method contract for post response data processing.
        /// </summary>
        /// <param name="routeParams">
        /// The route parameters from the request.
        /// </param>
        /// <returns>
        /// The <see cref="Response" />.
        /// </returns>
        protected override Response PostResponseData(dynamic routeParams)
        {
            return new Response { StatusCode = HttpStatusCode.MethodNotAllowed };
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
        /// The resolved <see cref="EngineeringModelSetup" />.
        /// </returns>
        /// <exception cref="Exception">
        /// If engineering model could not be resolved
        /// </exception>
        private EngineeringModelSetup DetermineEngineeringModelSetup(IProcessor processor, IReadOnlyList<string> routeSegments)
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
                throw new InvalidDataException("Engineering model could not be resolved");
            }

            // override query parameters to return only extent shallow
            this.RequestUtils.OverrideQueryParameters = null;

            return (EngineeringModelSetup)modelSetups.Single();
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
        /// The list of containment <see cref="Thing" />.
        /// </returns>
        private IEnumerable<Thing> GetContainmentResponse(
            IProcessor resourceProcessor,
            string partition,
            EngineeringModelSetup modelSetup,
            string[] routeSegments)
        {
            foreach (var thing in this.ProcessRequestPath(resourceProcessor, "EngineeringModel", partition, routeSegments, out _))
            {
                yield return thing;
            }

            this.TransactionManager.SetDefaultContext(resourceProcessor.Transaction);

            foreach (var thing in this.CollectReferenceDataLibraryChain(resourceProcessor, modelSetup))
            {
                yield return thing;
            }
        }
    }
}
