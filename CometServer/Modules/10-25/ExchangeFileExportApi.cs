// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeFileExportApi.cs" company="RHEA System S.A.">
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
    using System.Net.Mime;
    using System.Threading.Tasks;

    using Carter.Response;

    using CDP4Common.DTO;

    using CDP4JsonSerializer;

    using CometServer.Authorization;
    using CometServer.Configuration;
    using CometServer.Exceptions;
    using CometServer.Extensions;
    using CometServer.Health;
    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Protocol;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using Npgsql;

    /// <summary>
    /// This is an API endpoint class to support the ECSS-E-TM-10-25-AnnexC exchange file format export
    /// </summary>
    public class ExchangeFileExportApi : ApiBase
    {
        /// <summary>
        /// The top container.
        /// </summary>
        private const string TopContainer = "SiteDirectory";

        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private readonly ILogger<ExchangeFileExportApi> logger;

        /// <summary>
        /// The supported post query parameter.
        /// </summary>
        private static readonly string[] SupportedPostQueryParameter =
        {
            QueryParameters.IncludeFileDataQuery
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeFileExportApi"/> class
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
        public ExchangeFileExportApi(IAppConfigService appConfigService, ICometHasStartedService cometHasStartedService, ITokenGeneratorService tokenGeneratorService, ILoggerFactory loggerFactory) : base(appConfigService, cometHasStartedService, tokenGeneratorService, loggerFactory)
        {
            this.logger = loggerFactory == null ? NullLogger<ExchangeFileExportApi>.Instance : loggerFactory.CreateLogger<ExchangeFileExportApi>();
        }

        /// <summary>
        /// Add the routes to the <see cref="IEndpointRouteBuilder"/>
        /// </summary>
        /// <param name="app">
        /// The <see cref="IEndpointRouteBuilder"/> to which the routes are added
        /// </param>
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/export",
            async (HttpRequest req, HttpResponse res, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ICredentialsService credentialsService, IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, IJsonExchangeFileWriter jsonExchangeFileWriter) =>
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
                        this.logger.LogWarning("The POST REQUEST was not authorized for {Identity}", req.HttpContext.User.Identity.Name);

                        res.UpdateWithNotAutherizedSettings();
                        await res.AsJson("not authorized");
                        return;
                    }

                    await this.PostResponseData(req, res, requestUtils, transactionManager, credentialsService, metaInfoProvider, jsonSerializer, jsonExchangeFileWriter);
                }
            });
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
        /// <param name="jsonSerializer">
        /// The <see cref="ICdp4JsonSerializer"/> used to serialize data to JSOIN
        /// </param>
        /// <param name="metaInfoProvider">
        /// The <see cref="IMetaInfoProvider"/> used to provide metadata for any kind of <see cref="Thing"/>
        /// </param>
        /// <returns>
        /// An awaitable <see cref="Task"/>
        /// </returns>
        protected async Task PostResponseData(HttpRequest httpRequest, HttpResponse httpResponse, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ICredentialsService credentialsService, IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, IJsonExchangeFileWriter jsonExchangeFileWriter)
        {
            var reqsw = Stopwatch.StartNew();
            var requestToken = this.TokenGeneratorService.GenerateRandomToken();
            
            this.logger.LogInformation("{request}:{requestToken} - START HTTP REQUEST PROCESSING", httpRequest.QueryNameMethodPath(), requestToken);

            if (!this.AppConfigService.AppConfig.Midtier.IsExportEnabled)
            {
                httpResponse.StatusCode = (int)HttpStatusCode.Forbidden;
                await httpResponse.AsJson("Annex C3 Export is not enabled on this server");
                return;
            }

            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            this.logger.LogDebug("{request}:{requestToken} - Database operations started", httpRequest.QueryNameMethodPath(), requestToken);

            transactionManager.SetCachedDtoReadEnabled(true);

            var zipFilePath = "";

            try
            {
                HttpRequestHelper.ValidateSupportedQueryParameter(httpRequest.Query, SupportedPostQueryParameter);
                var queryParameters = httpRequest.Query.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value.FirstOrDefault());
                requestUtils.QueryParameters = new QueryParameters(queryParameters);

                var isMultiPart = httpRequest.GetMultipartBoundary() != string.Empty;

                if (isMultiPart)
                {
                    // multipart message received
                    throw new InvalidOperationException($"Multipart post messages are not allowed for the {TopContainer} route");
                }

                var version = httpRequest.QueryDataModelVersion();
                jsonSerializer.Initialize(metaInfoProvider, version);

                var iids = jsonSerializer.Deserialize<List<Guid>>(httpRequest.Body);

                var engineeringModelSetups = QueryExportEngineeringModelSetups(credentialsService, iids);

                transaction = transactionManager.SetupTransaction(ref connection, credentialsService.Credentials);

                zipFilePath = jsonExchangeFileWriter.Create(transaction, this.AppConfigService.AppConfig.Midtier.ExportDirectory, engineeringModelSetups, version);

                await transaction.CommitAsync();

                httpResponse.StatusCode = (int)HttpStatusCode.OK;

                await using var filestream = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read);

                await httpResponse.FromStream(filestream, HttpConstants.MimeTypeOctetStream, new ContentDisposition("EngineeringModelZipFileName"));
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                this.logger.LogError(ex, "{request}:{requestToken} - Request failed after {ElapsedMilliseconds} [ms]", httpRequest.QueryNameMethodPath(), requestToken, reqsw.ElapsedMilliseconds);

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

                if (System.IO.File.Exists(zipFilePath))
                {
                    System.IO.File.Delete(zipFilePath);
                }

                this.logger.LogInformation("{request}:{requestToken} - Response returned in {sw} [ms]", httpRequest.QueryNameMethodPath(), requestToken, reqsw.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Queries the <see cref="EngineeringModelSetup"/>s that are to be exported on the basis of the provided unique identifiers
        /// </summary>
        /// <param name="credentialsService">
        /// The <see cref="ICredentialsService"/> used to provide authorization and <see cref="Credentials"/>
        /// services while handling a request
        /// </param>
        /// <param name="iids">
        /// The unique identifiers of the <see cref="EngineeringModelSetup"/>s that are to be exported
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{EngineeringModelSetup}"/> that is to be exported
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// thrown when the <paramref name="iids"/> is null or empty
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// thrown when at least one of the provided <paramref name="iids"/> does not correspond to an <see cref="EngineeringModelSetup"/>
        /// the user has access to.
        /// </exception>
        private static IEnumerable<EngineeringModelSetup> QueryExportEngineeringModelSetups (ICredentialsService credentialsService, IEnumerable<Guid> iids)
        {
            if (!iids.Any())
            {
                throw new InvalidOperationException();
            }

            foreach (var iid in iids)
            {
                var engineeringModelSetup = credentialsService.Credentials.EngineeringModelSetups.SingleOrDefault(x => x.Iid == iid);

                if (engineeringModelSetup == null)
                {
                    throw new UnauthorizedAccessException($"$The user {credentialsService.Credentials.Person.UserName} does not have access to one of the EngineeringModels that are requested to be exported");
                }
                else
                {
                    yield return engineeringModelSetup;
                }
            }
        }
    }
}
