// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HeaderInfoProvider.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2020 RHEA System S.A.
//
//    Author: Sam Gerené, Merlin Bieze, Alex Vorobiev, Naron Phou, Alexander van Delft.
//
//    This file is part of CDP4 Web Services Community Edition. 
//    The CDP4 Web Services Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
//    This is an auto-generated class. Any manual changes to this file will be overwritten!
//
//    The CDP4 Web Services Community Edition is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Affero General Public
//    License as published by the Free Software Foundation; either
//    version 3 of the License, or (at your option) any later version.
//
//    The CDP4 Web Services Community Edition is distributed in the hope that it will be useful,
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