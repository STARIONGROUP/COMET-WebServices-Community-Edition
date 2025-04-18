﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpResponseExtensions.cs" company="Starion Group S.A.">
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

namespace CometServer.Modules
{
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Static extention mehtods for the <see cref="HttpResponse"/> class
    /// </summary>
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// Updates the <see cref="HttpResponse"/> properties to reflect that the
        /// corresponding request required authentication and that it was not authenticated
        /// </summary>
        /// <param name="httpResponse">
        /// The <see cref="HttpResponse"/> that is updated
        /// </param>
        public static void UpdateWithNotAuthenticatedSettings(this HttpResponse httpResponse)
        {
            httpResponse.ContentType = "application/json";
            httpResponse.StatusCode = 401;
            httpResponse.Headers.Append("WWW-Authenticate", "Basic");
        }

        public static void UpdateWithNotBearerAuthenticatedSettings(this HttpResponse httpResponse)
        {
            httpResponse.ContentType = "application/json";
            httpResponse.StatusCode = 401;
            httpResponse.Headers.Append("WWW-Authenticate", "Bearer");
        }

        /// <summary>
        /// Updates the <see cref="HttpResponse"/> properties to reflect that the
        /// corresponding request required authentication and that it was not authenticated
        /// </summary>
        /// <param name="httpResponse">
        /// The <see cref="HttpResponse"/> that is updated
        /// </param>
        public static void UpdateWithNotAutherizedSettings(this HttpResponse httpResponse)
        {
            httpResponse.StatusCode = 403;
        }
    }
}
