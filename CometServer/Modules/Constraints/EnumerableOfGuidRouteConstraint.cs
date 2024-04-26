// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumerableOfGuidRouteConstraint.cs" company="Starion Group S.A.">
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

namespace CometServer.Modules.Constraints
{
    using System;

    using CometServer.Extensions;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    /// <summary>
    /// The purpose of the <see cref="EnumerableOfGuidRouteConstraint" /> is to enable url routing for collection of url safe unique identifiers
    /// </summary>
    public class EnumerableOfGuidRouteConstraint : IRouteConstraint
    {
        /// <summary>
        ///     Determines whether the URL parameter contains a valid value for this constraint.
        /// </summary>
        /// <param name="httpContext">An object that encapsulates information about the HTTP request.</param>
        /// <param name="route">The router that this constraint belongs to.</param>
        /// <param name="routeKey">The name of the parameter that is being checked.</param>
        /// <param name="values">A dictionary that contains the parameters for the URL.</param>
        /// <param name="routeDirection">
        ///     An object that indicates whether the constraint check is being performed
        ///     when an incoming request is being handled or when a URL is being generated.
        /// </param>
        /// <returns><c>true</c> if the URL parameter contains a valid value; otherwise, <c>false</c>.</returns>
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            ArgumentNullException.ThrowIfNull(routeKey, nameof(routeKey));

            if (!values.TryGetValue(routeKey, out var value) || value is not string stringValues)
            {
                return false;
            }

            return stringValues.TryParseEnumerableOfGuid(out _);
        }
    }
}
