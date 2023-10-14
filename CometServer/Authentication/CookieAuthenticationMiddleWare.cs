// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CookieAuthenticationMiddleWare.cs" company="RHEA System S.A.">
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

namespace CometServer.Authentication
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The purpose of the <see cref="CookieAuthenticationMiddleWare"/> is to add cookie authentication to the pipeline
    /// </summary>
    public class CookieAuthenticationMiddleWare
    {
        /// <summary>
        /// The <see cref="RequestDelegate"/> of the pipeline
        /// </summary>
        private readonly RequestDelegate next;

        /// <summary>
        /// The <see cref="ILogger"/> used for logging
        /// </summary>
        private readonly ILogger<BasicAuthenticatonMiddleware> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CookieAuthenticationMiddleWare"/>
        /// </summary>
        /// <param name="next">
        /// The <see cref="RequestDelegate"/> of the request pipeline
        /// </param>
        /// <param name="loggerFactory">
        /// The <see cref="ILoggerFactory"/> to enable logging
        /// </param>
        public CookieAuthenticationMiddleWare(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;

            this.logger = loggerFactory.CreateLogger<BasicAuthenticatonMiddleware>();
        }
    }
}
