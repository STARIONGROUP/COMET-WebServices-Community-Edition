// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderInfoProvider.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using CDP4WebServices.API.Configuration;

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
        /// Initializes a new instance of the <see cref="HeaderInfoProvider"/> class.
        /// </summary>
        public HeaderInfoProvider()
        {
            // setup version info at runtime bootstrap
            this.responseHeaders.Add(this.Cdp4ServerHeader, this.GetAssemblyVersion(typeof(AppConfig)));
            this.responseHeaders.Add(this.Cdp4CommonHeader, this.GetAssemblyVersion(typeof(CDP4Common.DTO.Thing)));
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
        /// The nancy response.
        /// </param>
        public void RegisterResponseHeaders(Response response)
        {
            response.Headers.Add(this.Cdp4ServerHeader, this.Cdp4ServerVersion);
            response.Headers.Add(this.Cdp4CommonHeader, this.Cdp4CommonVersion);
            if (!response.Headers.ContainsKey(this.ContentTypeHeader))
            {
                response.Headers.Add(this.ContentTypeHeader, this.ContentTypeVersion);
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
        private string GetAssemblyVersion(Type type)
        {
            var asm = Assembly.GetAssembly(type);
            if (asm != null)
            {
                var version = asm.GetName().Version;
                return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }

            // return default if there was an error loading the assembly
            return "0.0.0";
        }
    }
}
