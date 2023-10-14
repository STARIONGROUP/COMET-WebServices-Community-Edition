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
    using System.Security;
    using System.Threading.Tasks;

    using Carter.Response;

    using CDP4Common.DTO;

    using CDP4JsonSerializer;

    using CometServer.Authorization;
    using CometServer.Configuration;
    using CometServer.Exceptions;
    using CometServer.Extensions;
    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Protocol;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.AspNetCore.Routing;

    using NLog;

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
        /// The site reference data library type name.
        /// </summary>
        private const string EngineeringModelZipFileName = "filename=AnnexC3ModelExport.zip";

        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
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
        public ExchangeFileExportApi(IAppConfigService appConfigService) : base(appConfigService)
        {
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
            async (HttpRequest req, HttpResponse res, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ICredentialsService credentialsService, IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, IJsonExchangeFileWriter jsonExchangeFileWriter) => {

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
                        Logger.Warn(e, $"The POST REQUEST was not authorized for {req.HttpContext.User.Identity.Name}");

                        res.UpdateWithNotAutherizedSettings();
                        await res.AsJson("not authorized");
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
        /// <returns>
        /// An awaitable <see cref="Task"/>
        /// </returns>
        protected async Task PostResponseData(HttpRequest httpRequest, HttpResponse httpResponse, IRequestUtils requestUtils, ICdp4TransactionManager transactionManager, ICredentialsService credentialsService, IMetaInfoProvider metaInfoProvider, ICdp4JsonSerializer jsonSerializer, IJsonExchangeFileWriter jsonExchangeFileWriter)
        {
            if (!this.AppConfigService.AppConfig.Midtier.IsExportEnabled)
            {
                httpResponse.StatusCode = (int)HttpStatusCode.Forbidden;
                await httpResponse.AsJson("The export is not enabled on this server");
                return;
            }
            
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            transactionManager.SetCachedDtoReadEnabled(true);

            var sw = Stopwatch.StartNew();
            var requestToken = this.GenerateRandomToken();

            var zipFilePath = "";

            try
            {
                Logger.Info(this.ConstructLog(httpRequest, $"{requestToken} started"));

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

                var engineeringModelSetups = this.QueryExportEngineeringModelSetups(credentialsService, iids);

                transaction = transactionManager.SetupTransaction(ref connection, credentialsService.Credentials);

                zipFilePath = jsonExchangeFileWriter.Create(transaction, this.AppConfigService.AppConfig.Midtier.ExportDirectory, engineeringModelSetups, version);

                await transaction.CommitAsync();

                httpResponse.StatusCode = (int)HttpStatusCode.OK;

                await using var filestream = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read);
                
                await httpResponse.FromStream(filestream, HttpConstants.MimeTypeOctetStream, new ContentDisposition("EngineeringModelZipFileName"));
            }
            catch (InvalidOperationException ex)
            {
                if (transaction != null)
                {
                    await transaction.RollbackAsync();
                }

                Logger.Error(ex, this.ConstructFailureLog(httpRequest, $"{requestToken} failed after {sw.ElapsedMilliseconds} [ms]"));

                // error handling
                httpResponse.StatusCode = (int)HttpStatusCode.Forbidden;
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
                httpResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                await httpResponse.AsJson($"exception:{ex.Message}");
            }
            finally
            {
                transaction?.Dispose();
                connection?.Dispose();

                if (System.IO.File.Exists(zipFilePath))
                {
                    System.IO.File.Delete(zipFilePath);
                }
            }
        }

        /// <summary>
        /// Queries the <see cref="EngineeringModelSetup"/>s that are to be exported on the basis of the provided unique identifiers
        /// </summary>
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
        private IEnumerable<EngineeringModelSetup> QueryExportEngineeringModelSetups (ICredentialsService credentialsService, IEnumerable<Guid> iids)
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
