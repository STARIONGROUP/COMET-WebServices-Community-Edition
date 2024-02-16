// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CometTasksModule.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2024 RHEA System S.A.
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

namespace CometServer.Modules.Tasks
{
    using System;
    using System.Net;
    using System.Threading.Tasks;

    using Carter;
    using Carter.Response;

    using CometServer.Authorization;
    using CometServer.Configuration;
    using CometServer.Exceptions;
    using CometServer.Health;
    using CometServer.Modules.Health;
    using CometServer.Tasks;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;

    using Npgsql;

    /// <summary>
    /// The purpose of the <see cref="CometTasksModule"/> is to expose an EndPoint where users can request the status of
    /// long running tasks, called <see cref="CometTask"/>s
    /// </summary>
    public class CometTasksModule : CarterModule
    {
        /// <summary>
        /// The (injected) <see cref="ILogger{HealthModule}"/>;
        /// </summary>
        private readonly ILogger<HealthModule> logger;

        /// <summary>
        /// The (injected) <see cref="cometHasStartedService"/> that is used to determine whether the
        /// COMET API is ready to accept traffic
        /// </summary>
        private readonly ICometHasStartedService cometHasStartedService;

        /// <summary>
        /// The (injected) <see cref="ICometTaskService"/> used to register and access running <see cref="CometTask"/>s
        /// </summary>
        protected readonly ICometTaskService cometTaskService;

        /// <summary>
        /// The (injected) <see cref="IAppConfigService"/> 
        /// </summary>
        protected readonly IAppConfigService appConfigService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthModule"/>
        /// </summary>
        /// <param name="logger">
        /// The (injected) <see cref="ILogger{HealthModule}"/>;
        /// </param>
        /// <param name="cometHasStartedService">
        /// The (injected) <see cref="ICometHasStartedService"/> that is used to check whether CDP4-COMET is ready to start
        /// acceptng requests
        /// </param>
        public CometTasksModule(ILogger<HealthModule> logger, ICometHasStartedService cometHasStartedService, ICometTaskService cometTaskService, IAppConfigService appConfigService)
        {
            this.logger = logger;
            this.cometHasStartedService = cometHasStartedService;
            this.cometTaskService = cometTaskService;
            this.appConfigService = appConfigService;
        }

        /// <summary>
        /// Add the routes to the <see cref="IEndpointRouteBuilder"/>
        /// </summary>
        /// <param name="app">
        /// The <see cref="IEndpointRouteBuilder"/> to which the routes are added
        /// </param>
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/tasks",
                    async (HttpRequest req, HttpResponse res, ICredentialsService credentialsService)
                        => await this.QueryTasks(req, res, credentialsService));

            app.MapGet("/tasks/{taskId:guid}",
                async (HttpRequest req, HttpResponse res, ICredentialsService credentialsService, Guid taskId)
                    => await this.QueryTask(req, res, credentialsService, taskId));

            app.MapPost("/tasks/{taskId:guid}",
                async (HttpRequest req, HttpResponse res, ICredentialsService credentialsService, Guid taskId)
                    => await this.CancelTask(req, res, credentialsService, taskId));
        }

        /// <param name="req">
        /// The incoming <see cref="HttpRequest"/> 
        /// </param>
        /// <param name="res">
        /// The outgoing <see cref="HttpResponse"/> 
        /// </param>
        /// <param name="credentialsService">
        /// The service for handling <see cref="Credentials"/>.
        /// </param>
        public async Task QueryTasks(HttpRequest req, HttpResponse res, ICredentialsService credentialsService)
        {
            if (!await this.IsServerReady(res))
            {
                res.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                await res.AsJson("Server is not ready");
                return;
            }

            if (!req.HttpContext.User.Identity.IsAuthenticated)
            {
                res.UpdateWithNotAuthenticatedSettings();
                await res.AsJson("not authenticated");
                return;
            }
            
            try
            {
                await this.Authorize(credentialsService, req.HttpContext.User.Identity.Name);

                this.logger.LogInformation("retrieving CometTasks for user: {personId}", credentialsService.Credentials.Person.Iid);

                var cometTasks = this.cometTaskService.QueryTasks(credentialsService.Credentials.Person.Iid);

                await res.AsJson(cometTasks);
            }
            catch (AuthorizationException)
            {
                this.logger.LogWarning("The GET REQUEST was not authorized for {Identity}", req.HttpContext.User.Identity.Name);

                res.UpdateWithNotAutherizedSettings();
                await res.AsJson("not authorized");
                return;
            }
        }

        /// <param name="req">
        /// The incoming <see cref="HttpRequest"/> 
        /// </param>
        /// <param name="res">
        /// The outgoing <see cref="HttpResponse"/> 
        /// </param>
        /// <param name="credentialsService">
        /// The service for handling <see cref="Credentials"/>.
        /// </param>
        /// <param name="taskId">
        /// The unique identifier of the requested <see cref="CometTask"/>
        /// </param>
        public async Task QueryTask(HttpRequest req, HttpResponse res, ICredentialsService credentialsService, Guid taskId)
        {
            if (!await this.IsServerReady(res))
            {
                res.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                await res.AsJson("Server is not ready");
                return;
            }

            if (!req.HttpContext.User.Identity.IsAuthenticated)
            {
                res.UpdateWithNotAuthenticatedSettings();
                await res.AsJson("not authenticated");
                return;
            }

            try
            {
                await this.Authorize(credentialsService, req.HttpContext.User.Identity.Name);

                this.logger.LogInformation("retrieving CometTask {taskID} for user: {personId}", taskId, credentialsService.Credentials.Person.Iid);

                var cometTask = this.cometTaskService.QueryTask(taskId);

                if (cometTask != null && cometTask.Value.Actor == credentialsService.Credentials.Person.Iid)
                {
                    await res.AsJson(cometTask);
                    return;
                }

                res.StatusCode = (int)HttpStatusCode.Forbidden;
            }
            catch (AuthorizationException)
            {
                this.logger.LogWarning("The GET REQUEST was not authorized for {Identity}", req.HttpContext.User.Identity.Name);

                res.UpdateWithNotAutherizedSettings();
                await res.AsJson("not authorized");
                return;
            }
        }

        /// <summary>
        /// Cancels the <see cref="CometTask"/>
        /// </summary>
        /// <param name="req">
        /// The incoming <see cref="HttpRequest"/> 
        /// </param>
        /// <param name="res">
        /// The outgoing <see cref="HttpResponse"/> 
        /// </param>
        /// <param name="credentialsService">
        /// The service for handling <see cref="Credentials"/>.
        /// </param>
        /// <param name="taskId">
        /// The unique identifier of the requested <see cref="CometTask"/>
        /// </param>
        public async Task CancelTask(HttpRequest req, HttpResponse res, ICredentialsService credentialsService, Guid taskId)
        {
            if (!await this.IsServerReady(res))
            {
                res.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                await res.AsJson("Server is not ready");
                return;
            }

            if (!req.HttpContext.User.Identity.IsAuthenticated)
            {
                res.UpdateWithNotAuthenticatedSettings();
                await res.AsJson("not authenticated");
                return;
            }

            res.StatusCode = (int)HttpStatusCode.NotImplemented;
        }

        /// <summary>
        /// Checks if the server is ready to accept requests.
        /// </summary>
        /// <param name="response">The HTTP response object.</param>
        /// <returns>True if the server is ready; otherwise, false.</returns>
        private async Task<bool> IsServerReady(HttpResponse response)
        {
            if (this.cometHasStartedService.GetHasStartedAndIsReady().IsHealthy)
            {
                return true;
            }

            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
            await response.AsJson("not yet started and ready to accept requests");
            return false;
        }

        /// <summary>
        /// Authorizes the user on the bases of the <paramref name="username"/> and calls the
        /// <see cref="ICredentialsService.ResolveCredentials"/> to resolve and set the
        /// <see cref="ICredentialsService.Credentials"/> to be used in the following pipeline
        /// </summary>
        /// <param name="credentialsService">
        /// The <see cref="ICredentialsService"/> used to provide authorization and <see cref="Credentials"/>
        /// services while handling a request
        /// </param>
        /// <param name="username">
        /// The username used to authorize
        /// </param>
        /// <returns>
        /// an awaitable <see cref="Task"/>
        /// </returns>
        protected async Task Authorize(ICredentialsService credentialsService, string username)
        {
            try
            {
                await using var connection = new NpgsqlConnection(Services.Utils.GetConnectionString(this.appConfigService.AppConfig.Backtier, this.appConfigService.AppConfig.Backtier.Database));

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
    }
}
