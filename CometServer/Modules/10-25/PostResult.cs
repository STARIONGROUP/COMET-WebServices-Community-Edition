// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PostResult.cs" company="Starion Group S.A.">
//    Copyright (c) 2015-2026 Starion Group S.A.
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
    using System.Collections.Generic;
    using System.Net;

    using CDP4Common.DTO;

    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// The outcome of the processing of a POST request: the <see cref="Thing"/>s to return, or an error with its status.
    /// It is plain data, so that work that continues after its <see cref="HttpRequest"/> has ended does not need the
    /// <see cref="HttpResponse"/>: only the request handler writes a <see cref="PostResult"/> to the response.
    /// </summary>
    public sealed class PostResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostResult"/> class
        /// </summary>
        private PostResult()
        {
        }

        /// <summary>
        /// Gets the <see cref="HttpStatusCode"/> of the response
        /// </summary>
        public HttpStatusCode StatusCode { get; private init; }

        /// <summary>
        /// Gets the <see cref="Thing"/>s to return when the POST succeeded, otherwise null
        /// </summary>
        public IReadOnlyList<Thing> Things { get; private init; }

        /// <summary>
        /// Gets the error message when the POST failed, otherwise null
        /// </summary>
        public string ErrorMessage { get; private init; }

        /// <summary>
        /// Gets the CDP error tag to add to the response headers, or null
        /// </summary>
        public string ErrorTag { get; private init; }

        /// <summary>
        /// Gets a value indicating whether the POST succeeded
        /// </summary>
        public bool IsSuccess => this.ErrorMessage == null;

        /// <summary>
        /// Creates a <see cref="PostResult"/> for a POST that succeeded
        /// </summary>
        /// <param name="things">
        /// The <see cref="Thing"/>s to return
        /// </param>
        /// <returns>
        /// The <see cref="PostResult"/>
        /// </returns>
        public static PostResult Success(IReadOnlyList<Thing> things)
        {
            return new PostResult { StatusCode = HttpStatusCode.OK, Things = things };
        }

        /// <summary>
        /// Creates a <see cref="PostResult"/> for a POST that failed
        /// </summary>
        /// <param name="statusCode">
        /// The <see cref="HttpStatusCode"/> of the response
        /// </param>
        /// <param name="errorMessage">
        /// The error message
        /// </param>
        /// <param name="errorTag">
        /// The optional CDP error tag
        /// </param>
        /// <returns>
        /// The <see cref="PostResult"/>
        /// </returns>
        public static PostResult Failure(HttpStatusCode statusCode, string errorMessage, string errorTag = null)
        {
            return new PostResult { StatusCode = statusCode, ErrorMessage = errorMessage ?? string.Empty, ErrorTag = errorTag };
        }
    }
}
