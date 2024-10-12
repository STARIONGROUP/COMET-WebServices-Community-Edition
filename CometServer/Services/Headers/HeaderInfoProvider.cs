// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderInfoProvider.cs" company="Starion Group S.A.">
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

namespace CometServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// The header info provider.
    /// </summary>
    public class HeaderInfoProvider : IHeaderInfoProvider
    {
        /// <summary>
        /// The response headers.
        /// </summary>
        private readonly Dictionary<string, string> responseHeaders = new();

        /// <summary>
        /// The version of the <see cref="HttpConstants.Cdp4ServerHeader"/>
        /// </summary>
        private static readonly Lazy<string> Cdp4ServerHeaderVersion = new(() => GetAssemblyVersion(typeof(Startup)));

        /// <summary>
        /// The version of the <see cref="HttpConstants.Cdp4ServerHeader"/>
        /// </summary>
        private static readonly Lazy<string> CometServerHeaderVersion = new(() => GetAssemblyVersion(typeof(Startup)));

        /// <summary>
        /// The version of the <see cref="HttpConstants.Cdp4CommonHeader"/>
        /// </summary>
        private static readonly Lazy<string> Cdp4CommonHeaderVersion = new(() => GetAssemblyVersion(typeof(CDP4Common.DTO.Thing)));

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderInfoProvider"/> class.
        /// </summary>
        public HeaderInfoProvider()
        {
            // setup version info at runtime bootstrap
            this.responseHeaders.Add(HttpConstants.Cdp4ServerHeader, Cdp4ServerHeaderVersion.Value);
            this.responseHeaders.Add(HttpConstants.CometServerHeader, CometServerHeaderVersion.Value);
            this.responseHeaders.Add(HttpConstants.Cdp4CommonHeader, Cdp4CommonHeaderVersion.Value);
        }

        /// <summary>
        /// Gets the CDP4 server response version.
        /// </summary>
        public string Cdp4ServerVersion
        {
            get
            {
                return this.responseHeaders[HttpConstants.Cdp4ServerHeader];
            }
        }

        /// <summary>
        /// Gets the CDP4 server response version.
        /// </summary>
        public string CometServerVersion
        {
            get
            {
                return this.responseHeaders[HttpConstants.CometServerHeader];
            }
        }

        /// <summary>
        /// Gets the CDP4 Common response version.
        /// </summary>
        public string Cdp4CommonVersion
        {
            get
            {
                return this.responseHeaders[HttpConstants.Cdp4CommonHeader];
            }
        }

        /// <summary>
        /// Gets the Content type version.
        /// </summary>
        public string ContentTypeVersion
        {
            get
            {
                return this.responseHeaders[HttpConstants.ContentTypeHeader];
            }
        }

        /// <summary>
        /// Register the CDP4 headers to the passed in response.
        /// </summary>
        /// <param name="response">
        /// The <see cref="HttpResponse"/> response.
        /// </param>
        /// <param name="contentTypeKind">
        /// The <see cref="ContentTypeKind"/> that is used to determine what the value of the
        /// Content-Type header needs to be
        /// </param>
        /// <param name="boundary">
        /// the boundary string used in the HTTP request
        /// </param>
        public void RegisterResponseHeaders(HttpResponse response, ContentTypeKind contentTypeKind, string boundary)
        {
            response.Headers.TryAdd(HttpConstants.Cdp4ServerHeader, this.Cdp4ServerVersion);
            response.Headers.TryAdd(HttpConstants.CometServerHeader, this.CometServerVersion);
            response.Headers.TryAdd(HttpConstants.Cdp4CommonHeader, this.Cdp4CommonVersion);

            switch (contentTypeKind)
            {
                case ContentTypeKind.JSON:
                    response.Headers.TryAdd(HttpConstants.ContentTypeHeader, "application/json; ecss-e-tm-10-25; version=1.0.0");
                    break;
                case ContentTypeKind.MESSAGEPACK:
                    response.Headers.TryAdd(HttpConstants.ContentTypeHeader, "application/msgpack; ecss-e-tm-10-25; version=1.0.0");
                    break;
                case ContentTypeKind.MULTIPARTMIXED:
                    response.Headers.TryAdd(HttpConstants.ContentTypeHeader, $"multipart/mixed; ecss-e-tm-10-25; version=1.0.0; boundary={boundary}");
                    break;
                case ContentTypeKind.IGNORE:
                    // The Content-Type header does not need to be set as it is already present
                    break;
            }
        }

        /// <summary>
        /// Query the assembly version for the given type.
        /// </summary>
        /// <param name="type">
        /// The type info for which to retrieve the assembly version.
        /// </param>
        /// <returns>
        /// The formatted version string (i.e. [Major].[Minor].[Build].
        /// </returns>
        private static string GetAssemblyVersion(Type type)
        {
            var asm = Assembly.GetAssembly(type);

            if (asm == null)
            {
                // return default if there was an error loading the assembly
                return "0.0.0";
            }

            var version = asm.GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }
    }
}
