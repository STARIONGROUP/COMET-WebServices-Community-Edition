// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderInfoProvider.cs" company="RHEA System S.A.">
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

namespace CometServer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using CometServer.Configuration;

    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// The header info provider.
    /// </summary>
    public class HeaderInfoProvider : IHeaderInfoProvider
    {
        /// <summary>
        /// The response headers.
        /// </summary>
        private readonly Dictionary<string, string> responseHeaders = new Dictionary<string, string>();

        /// <summary>
        /// The version of the <see cref="Cdp4ServerHeader"/>
        /// </summary>
        private static readonly Lazy<string> Cdp4ServerHeaderVersion = new Lazy<string>(() => GetAssemblyVersion(typeof(AppConfig)));

        /// <summary>
        /// The version of the <see cref="Cdp4CommonHeader"/>
        /// </summary>
        private static readonly Lazy<string> Cdp4CommonHeaderVersion = new Lazy<string>(() => GetAssemblyVersion(typeof(CDP4Common.DTO.Thing)));

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderInfoProvider"/> class.
        /// </summary>
        public HeaderInfoProvider()
        {
            // setup version info at runtime bootstrap
            this.responseHeaders.Add(this.Cdp4ServerHeader, Cdp4ServerHeaderVersion.Value);
            this.responseHeaders.Add(this.Cdp4CommonHeader, Cdp4CommonHeaderVersion.Value);
            this.responseHeaders.Add(this.ContentTypeHeader, "application/json; ecss-e-tm-10-25; version=1.0.0");
        }

        /// <summary>
        /// Gets the CDP4 server response header.
        /// </summary>
        public string Cdp4ServerHeader
        {
            get
            {
                return "CDP4-Server";
            }
        }

        /// <summary>
        /// Gets the CDP4 common response header.
        /// </summary>
        public string Cdp4CommonHeader
        {
            get
            {
                return "CDP4-Common";
            }
        }

        /// <summary>
        /// Gets the content type response header.
        /// </summary>
        public string ContentTypeHeader
        {
            get
            {
                return "Content-Type";
            }
        }

        /// <summary>
        /// Gets the CDP4 server response version.
        /// </summary>
        public string Cdp4ServerVersion
        {
            get
            {
                return this.responseHeaders[this.Cdp4ServerHeader];
            }
        }

        /// <summary>
        /// Gets the CDP4 Common response version.
        /// </summary>
        public string Cdp4CommonVersion
        {
            get
            {
                return this.responseHeaders[this.Cdp4CommonHeader];
            }
        }

        /// <summary>
        /// Gets the Content type version.
        /// </summary>
        public string ContentTypeVersion
        {
            get
            {
                return this.responseHeaders[this.ContentTypeHeader];
            }
        }

        /// <summary>
        /// Register the CDP4 headers to the passed in response.
        /// </summary>
        /// <param name="response">
        /// The <see cref="HttpResponse"/> response.
        /// </param>
        public void RegisterResponseHeaders(HttpResponse response)
        {
            response.Headers.Add(this.Cdp4ServerHeader, this.Cdp4ServerVersion);
            response.Headers.Add(this.Cdp4CommonHeader, this.Cdp4CommonVersion);

            if (!response.Headers.ContainsKey(this.ContentTypeHeader))
            {
                response.Headers.Add(this.ContentTypeHeader, this.ContentTypeVersion);
            }            
        }

        /// <summary>
        /// Register the multipart CDP4 content-type header to the passed in response.
        /// </summary>
        /// <param name="response">
        /// The <see cref="HttpResponse"/> response.
        /// </param>
        /// <param name="boundaryString">
        /// The boundary text in a Multipart MIME message <see href="https://www.w3.org/Protocols/rfc1341/7_2_Multipart.html"/>
        /// </param>
        public void RegisterMultipartResponseContentTypeHeader(HttpResponse response, string boundaryString)
        {
            response.Headers.Add(this.ContentTypeHeader, $"multipart/mixed; ecss-e-tm-10-25; version=1.0.0; boundary={boundaryString}");
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
