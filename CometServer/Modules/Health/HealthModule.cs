// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HealthModule.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
// 
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
// 
//    This file is part of CDP4-COMET Webservices Community Edition.
//    The CDP4-COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
// 
//    The CDP4-COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
// 
//    The CDP4-COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
// 
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CometServer.Modules.Health
{
    using System.Net;
    using System.Threading.Tasks;

    using Carter;

    using CDP4Orm.Dao;

    using CometServer.Configuration;
    using CometServer.Extensions;
    using CometServer.Health;
    using CometServer.Helpers;
    using CometServer.Services;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="HealthModule"/> is to expose a Readiness and Liveness endpoint
    /// </summary>
    public class HealthModule : CarterModule
    {
        /// <summary>
        /// The (injected) <see cref="IAppConfigService"/>
        /// </summary>
        private readonly IAppConfigService appConfigService;

        /// <summary>
        /// The (injected) <see cref="ILogger{HealthModule}"/>;
        /// </summary>
        private readonly ILogger<HealthModule> logger;

        /// <summary>
        /// The (injected) <see cref="ITokenGeneratorService"/> used generate HTTP request tokens
        /// </summary>
        private readonly ITokenGeneratorService tokenGeneratorService;

        /// <summary>
        /// The (injected) <see cref="cometHasStartedService"/> that is used to determine whether the
        /// COMET API is ready to accept traffic
        /// </summary>
        private readonly ICometHasStartedService cometHasStartedService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthModule"/>
        /// </summary>
        /// <param name="logger">
        /// The (injected) <see cref="ILogger{HealthModule}"/>;
        /// </param>
        /// <param name="appConfigService">
        /// The (injected) <see cref="IAppConfigService"/>
        /// </param>
        /// <param name="tokenGeneratorService">
        /// The (injected) <see cref="ITokenGeneratorService"/> used generate HTTP request tokens
        /// </param>
        public HealthModule(ILogger<HealthModule> logger, IAppConfigService appConfigService, ITokenGeneratorService tokenGeneratorService, ICometHasStartedService cometHasStartedService)
        {
            this.logger = logger;
            this.appConfigService = appConfigService;
            this.tokenGeneratorService = tokenGeneratorService;
            this.cometHasStartedService = cometHasStartedService;
        }

        /// <summary>
        /// Add the routes to the <see cref="IEndpointRouteBuilder"/>
        /// </summary>
        /// <param name="app">
        /// The <see cref="IEndpointRouteBuilder"/> to which the routes are added
        /// </param>
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            // map the startup endpoint to support Startup probes
            app.MapGet("/health/startup",
                    (HttpRequest req, HttpResponse res)
                        => this.QueryStartp(req, res))
                .RequireHost(this.appConfigService.AppConfig.HealthConfig.RequireHost);

            // map the Liveness endpoint to support Liveness probes
            app.MapGet("/healthz",
                 (HttpResponse res)
                    => this.QueryHealth(res))
                .RequireHost(this.appConfigService.AppConfig.HealthConfig.RequireHost);

            // map the Liveness endpoint to support readyness probes
            app.MapGet("/ready",
                    async (HttpRequest req, HttpResponse res, ICdp4TransactionManager transactionManager, ISiteDirectoryDao siteDirectoryDao)
                        => await this.QueryReadiness(req, res, transactionManager, siteDirectoryDao))
                .RequireHost(this.appConfigService.AppConfig.HealthConfig.RequireHost);
        }

        /// <summary>
        /// Queries startup conditions to determine whether the server is ready to start accepting connections
        /// </summary>
        /// <param name="req">
        /// The incoming <see cref="HttpRequest"/> 
        /// </param>
        /// <param name="res">
        /// The outgoing <see cref="HttpResponse"/> 
        /// </param>
        /// <returns>
        /// an instance of <see cref="IResult"/>
        /// </returns>
        public IResult QueryStartp(HttpRequest req, HttpResponse res)
        {
            var requestToken = this.tokenGeneratorService.GenerateRandomToken();

            this.logger.LogDebug("{request}:{requestToken} - START HTTP REQUEST PROCESSING", req.QueryNameMethodPath(), requestToken);

            var serverStatus = this.cometHasStartedService.GetHasStartedAndIsReady();

            if (this.cometHasStartedService.GetHasStartedAndIsReady().IsHealthy)
            {
                res.StatusCode = (int)HttpStatusCode.OK;
                return Results.Text($"Started since {serverStatus.DateTime}");
            }
            
            res.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
            return Results.Text("Startup in Progress");
        }

        /// <summary>
        /// Queries whether the server is ready to process requests, a ready server is able to query the database and retrieve the
        /// <see cref="CDP4Common.DTO.SiteDirectory"/> DTO from the database
        /// </summary>
        /// <param name="req">
        /// The incoming <see cref="HttpRequest"/> 
        /// </param>
        /// <param name="res">
        /// The outgoing <see cref="HttpResponse"/> 
        /// </param>
        /// <param name="transactionManager">
        /// The <see cref="ICdp4TransactionManager"/> that is used to manage connections and transactions to the cdp4-comet database
        /// </param>
        /// <param name="siteDirectoryDao">
        /// The <see cref="ISiteDirectoryDao"/> used to query the database to verify that a dbase connection can be made and that
        /// data can be retrieved
        /// </param>
        /// <returns>
        /// an instance of <see cref="IResult"/>
        /// </returns>
        public async Task<IResult> QueryReadiness(HttpRequest req, HttpResponse res, ICdp4TransactionManager transactionManager, ISiteDirectoryDao siteDirectoryDao)
        {
            var requestToken = this.tokenGeneratorService.GenerateRandomToken();

            this.logger.LogDebug("{request}:{requestToken} - START HTTP REQUEST PROCESSING", req.QueryNameMethodPath(), requestToken);

            NpgsqlConnection connection = null;
            NpgsqlTransaction transaction = null;

            try
            {
                transactionManager.SetCachedDtoReadEnabled(false);

                transaction = transactionManager.SetupTransaction(ref connection, null);

                siteDirectoryDao.Read(transaction, "SiteDirectory", null, false);

                res.StatusCode = (int)HttpStatusCode.OK;
                return Results.Text("Healthy");
            }
            catch (NpgsqlException ex)
            {
                this.logger.LogWarning("{request}:{requestToken} - The CDP4-COMET Server is not healthy: {excption}", req.QueryNameMethodPath(), requestToken, ex.Message);

                res.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                return Results.Text("Unhealthy");
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
            }
        }

        /// <summary>
        /// Queries whether the server is alive
        /// </summary>
        /// <param name="res">
        /// The outgoing <see cref="HttpResponse"/> 
        /// </param>
        /// <returns>
        /// an instance of <see cref="IResult"/>
        /// </returns>
        public IResult QueryHealth(HttpResponse res)
        {
            res.StatusCode = (int)HttpStatusCode.OK;
            return Results.Text("Healthy");
        }
    }
}
