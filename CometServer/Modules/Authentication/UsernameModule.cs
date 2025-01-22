// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UsernameModule.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2024 Starion Group S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the STARION implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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
    using System.Linq;
    using System.Net;
    using System.Security.Claims;

    using Carter;
    using Carter.Response;

    using CDP4DalCommon.Tasks;

    using CDP4ServicesMessaging.Services.BackgroundMessageProducers;

    using CometServer.Authentication.Bearer;
    using CometServer.Authorization;
    using CometServer.Configuration;
    using CometServer.Enumerations;
    using CometServer.Exceptions;
    using CometServer.Extensions;
    using CometServer.Health;
    using CometServer.Services;
    using CometServer.Tasks;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    using Npgsql;

    /// <summary>
    /// handle request on the logged-in users
    /// </summary>
    public class UsernameModule : ApiBase
    {
        /// <summary>
        /// The (injected) <see cref="ILogger{UsernameModule}"/> instance
        /// </summary>
        private readonly ILogger<UsernameModule> logger;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiBase"/> class
        /// </summary>
        /// <param name="appConfigService">
        /// The (injected) <see cref="IAppConfigService"/>
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
        /// <param name="thingsMessageProducer">
        /// The (injected) <see cref="IBackgroundThingsMessageProducer"/> used to schedule things messages to be sent
        /// </param>
        /// <param name="cometTaskService">
        /// The (injected) <see cref="ICometTaskService"/> used to register and access running <see cref="CometTask"/>s
        /// </param>
        public UsernameModule(IAppConfigService appConfigService, ICometHasStartedService cometHasStartedService, ITokenGeneratorService tokenGeneratorService, ILoggerFactory loggerFactory, IBackgroundThingsMessageProducer thingsMessageProducer, ICometTaskService cometTaskService) :
            base(appConfigService, cometHasStartedService, tokenGeneratorService, loggerFactory, thingsMessageProducer, cometTaskService)
        {
            this.logger = loggerFactory == null ? NullLogger<UsernameModule>.Instance : loggerFactory.CreateLogger<UsernameModule>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiBase"/> class
        /// </summary>
        /// <param name="appConfigService">
        /// The (injected) <see cref="IAppConfigService"/>
        /// </param>
        /// <param name="cometHasStartedService">
        /// The (injected) <see cref="ICometHasStartedService"/> that is used to determine whether the servre
        /// is ready to accept incoming requests
        /// </param>
        /// <param name="tokenGeneratorService">
        /// The (injected) <see cref="ITokenGeneratorService"/> used generate HTTP request tokens
        /// </param>
        /// <param name="loggerFactory">
        /// The (injected) <see cref="ILoggerFactory"/> used to create typed loggers
        /// </param>
        public UsernameModule(IAppConfigService appConfigService, ICometHasStartedService cometHasStartedService, ITokenGeneratorService tokenGeneratorService, ILoggerFactory loggerFactory) :
            base(appConfigService, cometHasStartedService, tokenGeneratorService, loggerFactory)
        {
            this.logger = loggerFactory == null ? NullLogger<UsernameModule>.Instance : loggerFactory.CreateLogger<UsernameModule>();
        }

        /// <summary>
        /// Add the routes to the <see cref="IEndpointRouteBuilder"/>
        /// </summary>
        /// <param name="app">
        /// The <see cref="IEndpointRouteBuilder"/> to which the routes are added
        /// </param>
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/username", async (HttpRequest req, HttpResponse res, IAppConfigService appConfigService, ICredentialsService credentialsService) =>
            {
                if (!await this.IsServerReady(res))
                {
                    return;
                }

                var identity = req.HttpContext.User.Identity!.Name;
                
                try
                {
                    identity = await this.Authorize(appConfigService, credentialsService, req);
                    await res.WriteAsync(credentialsService.Credentials.UserName);
                }
                catch (AuthorizationException)
                {
                    this.logger.LogWarning("The GET UserName was not authorized for {Identity}", identity);

                    res.UpdateWithNotAutherizedSettings();
                    await res.AsJson("not authorized");
                }
            }).RequireAuthorization(ApiBase.AuthenticationSchemes);
        }
    }
}