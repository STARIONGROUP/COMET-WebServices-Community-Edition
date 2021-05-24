// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExchangeFileExportApi.cs" company="RHEA System S.A.">
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
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Mime;
    using System.Security;
    using System.Text;
    using System.Threading.Tasks;

    using Carter;
    using Carter.ModelBinding;
    using Carter.Response;

    using CDP4Common.CommonData;
    using CDP4Common.DTO;
    using CDP4Common.Helpers;

    using CDP4Orm.Dao;
    using CDP4Orm.MigrationEngine;

    using CometServer.Authorization;
    using CometServer.Configuration;
    using CometServer.Helpers;
    using CometServer.Services;
    using CometServer.Services.Authorization;
    using CometServer.Services.DataStore;
    using CometServer.Services.FileHandling;
    using CometServer.Services.Operations.SideEffects;
    using CometServer.Services.Protocol;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;

    using Newtonsoft.Json;

    using NLog;

    using Npgsql;

    using File = CDP4Common.DTO.File;
    using IServiceProvider = CometServer.Services.IServiceProvider;

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
        /// Gets or sets the (injected) <see cref="IJsonExchangeFileWriter"/> used to create an
        /// E-TM-10-25 Annex C3 JSON export file.
        /// </summary>
        public IJsonExchangeFileWriter JsonExchangeFileWriter { get; set; }
        
        /// <summary>
        /// The supported post query parameter.
        /// </summary>
        private static readonly string[] SupportedPostQueryParameter =
        {
            QueryParameters.IncludeFileDataQuery
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeFileExportApi"/> class.
        /// </summary>
        public ExchangeFileExportApi()
        {
            // Seed the data store from the provided (uploaded) exchange file
            this.Post("/export", async (req, res) =>
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
                }
            );
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
            if (!this.AppConfigService.AppConfig.Midtier.IsExportEnabled)
            {
                httpResponse.StatusCode = (int)HttpStatusCode.Forbidden;
                await httpResponse.AsJson("The export is not enabled on this server");
                return;
            }
            
            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            this.TransactionManager.SetCachedDtoReadEnabled(true);

            var sw = Stopwatch.StartNew();
            var requestToken = this.GenerateRandomToken();

            var zipFilePath = "";

            try
            {
                Logger.Info(this.ConstructLog(httpRequest, $"{requestToken} started"));

                HttpRequestHelper.ValidateSupportedQueryParameter(httpRequest.Query, SupportedPostQueryParameter);
                this.RequestUtils.QueryParameters = new QueryParameters(httpRequest.Query);

                var isMultiPart = httpRequest.GetMultipartBoundary() != string.Empty;
                if (isMultiPart)
                {
                    // multipart message received
                    throw new InvalidOperationException($"Multipart post messages are not allowed for the {TopContainer} route");
                }

                var version = this.RequestUtils.GetRequestDataModelVersion(httpRequest);
                this.JsonSerializer.Initialize(this.MetaInfoProvider, version);
                
                var iids = this.JsonSerializer.Deserialize<List<Guid>>(httpRequest.Body);

                var engineeringModelSetups = this.QueryExportEngineeringModelSetups(iids);

                transaction = this.TransactionManager.SetupTransaction(ref connection, this.CredentialsService.Credentials);

                zipFilePath = this.JsonExchangeFileWriter.Create(transaction, this.AppConfigService.AppConfig.Midtier.ExportDirectory, engineeringModelSetups, version);

                await transaction.CommitAsync();

                httpResponse.StatusCode = (int)HttpStatusCode.OK;

                await using var filestream = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read);
                
                await httpResponse.FromStream(filestream, this.MimeTypeOctetStream, new ContentDisposition("EngineeringModelZipFileName"));
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
        private IEnumerable<EngineeringModelSetup> QueryExportEngineeringModelSetups (IEnumerable<Guid> iids)
        {
            if (!iids.Any())
            {
                throw new InvalidOperationException();
            }

            foreach (var iid in iids)
            {
                var engineeringModelSetup = this.CredentialsService.Credentials.EngineeringModelSetups.SingleOrDefault(x => x.Iid == iid);

                if (engineeringModelSetup == null)
                {
                    throw new UnauthorizedAccessException($"$The user {this.CredentialsService.Credentials.Person.UserName} does not have access to one of the EngineeringModels that are requested to be exported");
                }
                else
                {
                    yield return engineeringModelSetup;
                }
            }
        }
    }
}
