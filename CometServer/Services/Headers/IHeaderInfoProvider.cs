// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHeaderInfoProvider.cs" company="Starion Group S.A.">
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
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// The HeaderInfoProvider interface.
    /// </summary>
    public interface IHeaderInfoProvider
    {
        /// <summary>
        /// Gets the CDP4 server response version.
        /// </summary>
        string Cdp4ServerVersion { get; }

        /// <summary>
        /// Gets the COMET server response version.
        /// </summary>
        string CometServerVersion { get; }

        /// <summary>
        /// Gets the CDP4 Common response version.
        /// </summary>
        string Cdp4CommonVersion { get; }

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
        /// the boundary that is used in the response
        /// </param>
        public void RegisterResponseHeaders(HttpResponse response, ContentTypeKind contentTypeKind, string boundary);
    }
}
