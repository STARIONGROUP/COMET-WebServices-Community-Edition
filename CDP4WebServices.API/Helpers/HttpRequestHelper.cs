﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestHelper.cs" company="RHEA System S.A.">
//   Copyright (c) 2016-2018 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Nancy;
    using NLog;
    using Services;
    using Services.Protocol;

    /// <summary>
    /// A helper class to process HTTP requests
    /// </summary>
    public static class HttpRequestHelper
    {
        /// <summary>
        /// A <see cref="NLog.Logger"/> instance
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Validate that the current request only contains supported query parameters.
        /// </summary>
        /// <param name="request">The <see cref="Request"/> that contains the query parameters</param>
        /// <param name="requestUtil"></param>
        /// <param name="supportedQueryParameters">
        /// The supported Query Parameters.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Exception if query parameter is not supported
        /// </exception>
        public static void ValidateSupportedQueryParameter(Request request, IRequestUtils requestUtil, string[] supportedQueryParameters)
        {
            var queryParameters = (Dictionary<string, object>)request.Query.ToDictionary();
            foreach (var kvp in queryParameters)
            {
                if (!supportedQueryParameters.Contains(kvp.Key))
                {
                    throw new InvalidOperationException(string.Format("Query parameter '{0}' is not supported", kvp.Key));
                }
            }

            // verify that revisionFrom can only be associated with revisionTo
            if (queryParameters.Keys.Contains(QueryParameters.RevisionFromQuery))
            {
                if (queryParameters.Keys.Count > 1 && !queryParameters.Keys.Contains(QueryParameters.RevisionToQuery))
                {
                    throw new InvalidOperationException("revisionFrom may only be associated with revisionTo.");
                }
            }
            else if (queryParameters.Keys.Contains(QueryParameters.RevisionToQuery))
            {
                if (queryParameters.Keys.Count > 1 && !queryParameters.Keys.Contains(QueryParameters.RevisionFromQuery))
                {
                    throw new InvalidOperationException("revisionTo may only be associated with revisionFrom.");
                }
            }

            // process and set the query parameter info for this request
            requestUtil.QueryParameters = new QueryParameters(queryParameters);
        }

        /// <summary>
        /// Utility function to extract the route segments from the URI.
        /// </summary>
        /// <param name="routeParams">
        /// The dynamic route parameters from the request.
        /// </param>
        /// <param name="topContainer">
        /// The top container name.
        /// </param>
        /// <returns>
        /// The split route parameters as array.
        /// </returns>
        public static string[] ParseRouteSegments(dynamic routeParams, string topContainer)
        {
            return string.Format("{0}/{1}", topContainer, routeParams.uri)
                .Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}