// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderInfoProvider.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Services
{
    using Nancy;

    /// <summary>
    /// The HeaderInfoProvider interface.
    /// </summary>
    public interface IHeaderInfoProvider
    {
        /// <summary>
        /// Gets the CDP4 server response header.
        /// </summary>
        string Cdp4ServerHeader { get; }

        /// <summary>
        /// Gets the CDP4 common response header.
        /// </summary>
        string Cdp4CommonHeader { get; }

        /// <summary>
        /// Gets the content type response header.
        /// </summary>
        string ContentTypeHeader { get; }

        /// <summary>
        /// Gets the CDP4 server response version.
        /// </summary>
        string Cdp4ServerVersion { get; }

        /// <summary>
        /// Gets the CDP4 Common response version.
        /// </summary>
        string Cdp4CommonVersion { get; }

        /// <summary>
        /// Gets the Content type version.
        /// </summary>
        string ContentTypeVersion { get; }

        /// <summary>
        /// Register the CDP4 headers to the passed in response.
        /// </summary>
        /// <param name="response">
        /// The nancy response.
        /// </param>
        void RegisterResponseHeaders(Response response);
    }
}
