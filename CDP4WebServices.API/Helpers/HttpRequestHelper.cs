// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestHelper.cs" company="RHEA System S.A.">
//   Copyright (c) 2016 RHEA System S.A.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace CDP4WebServices.API.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using CDP4Common.DTO;
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

        /// <summary>
        /// Filter out permission depending on the supported version
        /// </summary>
        /// <param name="things">The collection of <see cref="Thing"/> to filter</param>
        /// <param name="requestUtils">The <see cref="IRequestUtils"/></param>
        /// <returns>The filtered collection of <see cref="Thing"/></returns>
        public static IEnumerable<Thing> FilterOutPermissions(IReadOnlyCollection<Thing> things, IRequestUtils requestUtils)
        {
            var timer = Stopwatch.StartNew();

            var personRoles = things.OfType<PersonRole>().ToArray();
            var participantRoles = things.OfType<ParticipantRole>().ToArray();
            var personPermissions = things.OfType<PersonPermission>().ToArray();
            var participantPermissions = things.OfType<ParticipantPermission>().ToArray();

            var excludedPersonPermission = new List<PersonPermission>();
            var excludedParticipantPermission = new List<ParticipantPermission>();

            foreach (var personPermission in personPermissions)
            {
                var metainfo = requestUtils.MetaInfoProvider.GetMetaInfo(personPermission.ObjectClass.ToString());
                if (string.IsNullOrEmpty(metainfo.ClassVersion) || requestUtils.GetRequestDataModelVersion > new Version(metainfo.ClassVersion))
                {
                    continue;
                }

                excludedPersonPermission.Add(personPermission);
            }

            foreach (var participantPermission in participantPermissions)
            {
                var metainfo = requestUtils.MetaInfoProvider.GetMetaInfo(participantPermission.ObjectClass.ToString());
                if (string.IsNullOrEmpty(metainfo.ClassVersion) || requestUtils.GetRequestDataModelVersion > new Version(metainfo.ClassVersion))
                {
                    continue;
                }

                excludedParticipantPermission.Add(participantPermission);
            }

            foreach (var personRole in personRoles)
            {
                personRole.PersonPermission.RemoveAll(x => excludedPersonPermission.Select(pp => pp.Iid).Contains(x));
            }

            foreach (var participantRole in participantRoles)
            {
                participantRole.ParticipantPermission.RemoveAll(x => excludedParticipantPermission.Select(pp => pp.Iid).Contains(x));
            }

            foreach (var thing in things.Except(excludedParticipantPermission).Except(excludedPersonPermission))
            {
                yield return thing;
            }

            Logger.Info(string.Format("permission filter operation took {0} ms", timer.ElapsedMilliseconds));
        }
    }
}