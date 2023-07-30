// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHeaderInfoProvider.cs" company="RHEA System S.A.">
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
        /// Register the CDP4 headers to the passed in response.
        /// </summary>
        /// <param name="response">
        /// The nancy response.
        /// </param>
        /// <param name="contentTypeKind">
        /// The <see cref="ContentTypeKind"/> is used to determine which for the <see cref="Response"/> will take
        /// </param>
        void RegisterResponseHeaders(Response response, ContentTypeKind contentTypeKind);
    }
}
