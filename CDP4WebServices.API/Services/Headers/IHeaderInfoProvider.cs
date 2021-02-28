// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHeaderInfoProvider.cs" company="RHEA System S.A.">
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
    using Microsoft.AspNetCore.Http;

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
        void RegisterResponseHeaders(HttpResponse response);

        /// <summary>
        /// Register the multipart CDP4 content-type header to the passed in response.
        /// </summary>
        /// <param name="response">
        /// The nancy response.
        /// </param>
        /// <param name="boundaryString">
        /// The boundary text in a Multipart MIME message <see href="https://www.w3.org/Protocols/rfc1341/7_2_Multipart.html"/>
        /// </param>
        void RegisterMultipartResponseContentTypeHeader(HttpResponse response, string boundaryString);
    }
}
