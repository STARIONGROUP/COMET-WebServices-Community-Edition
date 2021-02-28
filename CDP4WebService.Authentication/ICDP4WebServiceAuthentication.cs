// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICDP4WebServiceAuthentication.cs" company="RHEA System S.A.">
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

namespace CometServer.Authentication
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Http;

    public interface ICDP4WebServiceAuthentication
    {
        /// <summary>
        /// Gets or sets the CDP4 authentication cookie name
        /// </summary>
        string CDP4AuthenticationCookieName { get; set; }

        /// <summary>
        /// Enables basic authentication for the application
        /// </summary>
        /// <param name="pipelines">
        /// Pipelines to add handlers to (usually "this")
        /// </param>
        /// <param name="configuration">
        /// Forms authentication configuration
        /// </param>
        /// <param name="basicAuthenticationPaths">
        /// The basic Authentication Paths. Each supplied path shall be preceded by a '/' character.
        /// </param>
        void Enable(IPipelines pipelines, CDP4WebServiceAuthenticationConfiguration configuration, IEnumerable<string> basicAuthenticationPaths);

        /// <summary>
        /// Gets the pre request hook for loading the authenticated user's details
        /// from the authentication header.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContext"/></param>
        /// <returns>null response</returns>
        HttpResponse GetCredentialRetrievalHook(HttpContext context);

        /// <summary>
        /// Logs the user out and redirects them to a URL
        /// </summary>
        /// <param name="context">the current <see cref="HttpContext"/></param>
        /// <returns>
        /// an instance of <see cref="HttpResponse"/>
        /// </returns>
        HttpResponse LogOutResponse(HttpContext context);

        /// <summary>
        /// Logs the user out and redirects them to a URL
        /// </summary>
        /// <param name="context">the current <see cref="HttpContext"/></param>
        /// <param name="redirectUrl"> URL to redirect to </param>
        /// <returns>
        /// an instance of <see cref="HttpResponse"/>
        /// </returns>
        HttpResponse LogOutAndRedirectResponse(HttpContext context, string redirectUrl);

        /// <summary>
        /// Invalidate all entries in the credential cache.
        /// </summary>
        /// <returns>
        /// True if successfully cleared.
        /// </returns>
        void ResetCredentialCache();

        /// <summary>
        /// Evict a cached credential by supplying the userName.
        /// </summary>
        /// <param name="userName">
        /// The user name of the credentials that are to be evicted.
        /// </param>
        /// <returns>
        /// True if the user credentials where evicted.
        /// </returns>
        bool EvictCredentialFromCache(string userName);
    }
}