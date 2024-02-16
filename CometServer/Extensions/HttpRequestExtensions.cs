// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestExtensions.cs" company="RHEA System S.A.">
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

namespace CometServer.Extensions
{
    using System;
    using System.Linq;

    using CometServer.Services;

    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Static extensioin methods for the <see cref="HttpRequest"/>
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Queries the <see cref="ContentTypeKind"/>
        /// </summary>
        /// <param name="httpRequest">
        /// The subject <see cref="HttpRequest"/>
        /// </param>
        /// <returns>
        /// a <see cref="ContentTypeKind"/> value based on the <see cref="HttpRequest.ContentType"/>
        /// </returns>
        public static ContentTypeKind QueryContentTypeKind(this HttpRequest httpRequest)
        {
            var contentTypeKind = ContentTypeKind.JSON;

            if (httpRequest.Headers.Accept.Any(x => x.Contains(HttpConstants.MimeTypeMessagePack)))
            {
                contentTypeKind = ContentTypeKind.MESSAGEPACK;
            }

            return contentTypeKind;
        }

        /// <summary>
        /// Queries the requested CDP4-COMET Model <see cref="Version"/>
        /// </summary>
        /// <returns>
        /// a <see cref="Version"/> value based on the <see cref="HttpRequest.ContentType"/>
        /// </returns>
        public static Version QueryDataModelVersion(this HttpRequest httpRequest)
        {
            if (httpRequest.Headers.TryGetValue(HttpConstants.AcceptCdpVersionHeader, out var versionHeaderValue))
            {
                string versionHeader= versionHeaderValue;
                return new Version(versionHeader);
            }

            return new Version(HttpConstants.DefaultDataModelVersion);
        }

        /// <summary>
        /// Queries the name of the user, method and path from the <see cref="HttpRequest"/>
        /// and returns it as a concatenated string
        /// </summary>
        /// <param name="httpRequest">
        /// The subject <see cref="HttpRequest"/>
        /// </param>
        /// <returns>
        /// a string that contains the user name, method and path
        /// </returns>
        public static string QueryNameMethodPath(this HttpRequest httpRequest)
        {
            if (httpRequest.HttpContext.User.Identity == null || string.IsNullOrEmpty(httpRequest.HttpContext.User.Identity.Name))
            {
                return $"ANONYMOUS:{httpRequest.Method}:{httpRequest.Path}:{httpRequest.QueryString}";
            }

            return $"{httpRequest.HttpContext.User.Identity.Name}:{httpRequest.Method}:{httpRequest.Path}:{httpRequest.QueryString}";
        }
    }
}
