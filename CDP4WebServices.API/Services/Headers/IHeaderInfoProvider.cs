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

        /// <summary>
        /// Register the multipart CDP4 content-type header to the passed in response.
        /// </summary>
        /// <param name="response">
        /// The nancy response.
        /// </param>
        /// <param name="boundaryString">
        /// The boundary text in a Multipart MIME message <see href="https://www.w3.org/Protocols/rfc1341/7_2_Multipart.html"/>
        /// </param>
        void RegisterMultipartResponseContentTypeHeader(Response response, string boundaryString);
    }
}