// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CometAuthorizationMiddleware.cs" company="RHEA System S.A.">
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

namespace CometServer.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CometServer.Configuration;
    using CometServer.Services;

    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// The purpose of the <see cref="CometAuthorizationMiddleware"/> is to perform 10-25 authorization
    /// which results in a constructed <see cref="ICredentials"/> exposed via the <see cref="IRequestUtils"/>
    /// for the current scoped request
    /// </summary>
    /// <remarks>
    /// This is not a terminating middleware
    /// </remarks>
    public class CometAuthorizationMiddleware
    {
        private readonly RequestDelegate next;

        private readonly IAppConfigService appConfigService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CometAuthorizationMiddleware"/>
        /// </summary>
        /// <param name="next">
        /// The <see cref="RequestDelegate"/> that needs to be invoked at the end of
        /// the middleware invokation 
        /// </param>
        /// <param name="appConfigService">
        /// 
        /// </param>
        public CometAuthorizationMiddleware(RequestDelegate next, IAppConfigService appConfigService)
        {
            this.next = next;
            this.appConfigService = appConfigService;
        }

        /// <summary>
        /// Invokes the middleware
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // run authorization logic
            // TODO: put code here

            // Call the next delegate/middleware in the pipeline
            await this.next(context);
        }
    }
}
