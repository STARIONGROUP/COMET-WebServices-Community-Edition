// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestHelper.cs" company="RHEA System S.A.">
//    Copyright (c) 2015-2023 RHEA System S.A.
//
//    Author: Sam Gerené, Alex Vorobiev, Alexander van Delft, Nathanael Smiechowski, Antoine Théate
//
//    This file is part of CDP4-COMET Webservices Community Edition. 
//    The CDP4-COMET Webservices Community Edition is the RHEA implementation of ECSS-E-TM-10-25 Annex A and Annex C.
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

namespace CometServer.Helpers
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Http;

    using Services.Protocol;

    /// <summary>
    /// A helper class to process HTTP requests
    /// </summary>
    public static class HttpRequestHelper
    {
        /// <summary>
        /// Validate that the <see cref="IQueryCollection"/> only contains supported query parameters.
        /// </summary>
        /// <param name="queryCollection">
        /// The <see cref="IQueryCollection"/> that are to be validated
        /// </param>
        /// <param name="supportedQueryParameters">
        /// The supported Query Parameters.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Exception if query parameter is not supported
        /// </exception>
        public static void ValidateSupportedQueryParameter(IQueryCollection queryCollection, string[] supportedQueryParameters)
        {
            foreach (var kvp in queryCollection)
            {
                if (!supportedQueryParameters.Contains(kvp.Key))
                {
                    throw new InvalidOperationException($"Query parameter '{kvp.Key}' is not supported");
                }
            }

            // verify that revisionFrom can only be associated with revisionTo
            if (queryCollection.Keys.Contains(QueryParameters.RevisionFromQuery))
            {
                if (queryCollection.Keys.Count > 1 && !queryCollection.Keys.Contains(QueryParameters.RevisionToQuery))
                {
                    throw new InvalidOperationException("revisionFrom may only be associated with revisionTo.");
                }
            }
            else if (queryCollection.Keys.Contains(QueryParameters.RevisionToQuery))
            {
                if (queryCollection.Keys.Count > 1 && !queryCollection.Keys.Contains(QueryParameters.RevisionFromQuery))
                {
                    throw new InvalidOperationException("revisionTo may only be associated with revisionFrom.");
                }
            }
        }

        /// <summary>
        /// Utility function to extract the route segments from the URI.
        /// </summary>
        /// <param name="requestPath">
        /// The path from the request.
        /// </param>
        /// <returns>
        /// The split route parameters as array.
        /// </returns>
        public static string[] ParseRouteSegments(string requestPath)
        {
            return requestPath.TrimStart('/').Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}