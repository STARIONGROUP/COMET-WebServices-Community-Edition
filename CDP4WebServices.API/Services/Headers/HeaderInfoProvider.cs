// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderInfoProvider.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft, Nathanael Smiechowski
//
//    This file is part of COMET Web Services Community Edition. 
//    The COMET Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//
//    The COMET Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The COMET Web Services Community Edition is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using CDP4WebServices.API.Configuration;
    using CDP4WebServices.API.Modules;

    using Nancy;

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
        /// Register the CDP4 headers to the passed in response.
        /// </summary>
        /// <param name="response">
        /// The nancy response.
        /// </param>
        /// <param name="contentTypeKind">
        /// The <see cref="ContentTypeKind"/> that is used to determine what the value of the
        /// Content-Type header needs to be
        /// </param>
        public void RegisterResponseHeaders(Response response, ContentTypeKind contentTypeKind)
        {
            response.Headers.Add(this.Cdp4ServerHeader, this.Cdp4ServerVersion);
            response.Headers.Add(this.Cdp4CommonHeader, this.Cdp4CommonVersion);

            switch (contentTypeKind)
            {
                case ContentTypeKind.JSON:
                    response.Headers.Add(this.ContentTypeHeader, "application/json; ecss-e-tm-10-25; version=1.0.0");
                    break;
                case ContentTypeKind.MESSAGEPACK:
                    response.Headers.Add(this.ContentTypeHeader, "application/msgpack; ecss-e-tm-10-25; version=1.0.0");
                    break;
                case ContentTypeKind.MULTIPARTMIXED:
                    response.Headers.Add(this.ContentTypeHeader, $"multipart/mixed; ecss-e-tm-10-25; version=1.0.0; boundary={ApiBase.BoundaryString}");
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
