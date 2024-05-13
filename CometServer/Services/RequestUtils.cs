// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestUtils.cs" company="Starion Group S.A.">
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
    using System;
    using System.Collections.Generic;

    using CDP4Common.DTO;

    using CometServer.Services.Protocol;

    /// <summary>
    /// A utils class available in the context of a request.
    /// </summary>
    public class RequestUtils : IRequestUtils
    {
        /// <summary>
        /// The query parameters of the request.
        /// </summary>
        private IQueryParameters queryParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestUtils"/> class.
        /// </summary>
        public RequestUtils()
        {
            this.Cache = new List<Thing>();
        }

        /// <summary>
        /// Gets or sets the cache.
        /// </summary>
        public List<Thing> Cache { get; set; }

        /// <summary>
        /// Gets or sets the query parameters.
        /// </summary>
        public IQueryParameters QueryParameters
        {
            get
            {
                return this.OverrideQueryParameters ?? this.queryParameters;
            }

            set
            {
                this.queryParameters = value;
            }
        }

        /// <summary>
        /// Gets or sets the override query parameters to be used for specific processing logic.
        /// set to null to use request query parameters
        /// </summary>
        public IQueryParameters OverrideQueryParameters { private get; set; }

        /// <summary>
        /// Construct the engineering model partition identifier from the passed in engineeringModel id.
        /// </summary>
        /// <param name="engineeringModelIid">
        /// The engineering model id.
        /// </param>
        /// <returns>
        /// The constructed database partition string.
        /// </returns>
        public string GetEngineeringModelPartitionString(Guid engineeringModelIid)
        {
            return $"EngineeringModel_{engineeringModelIid.ToString().Replace("-", "_")}";
        }

        /// <summary>
        /// Construct the iteration partition identifier from the passed in engineeringModel id.
        /// </summary>
        /// <param name="engineeringModelIid">
        /// The engineeringModel id.
        /// </param>
        /// <returns>
        /// The constructed database partition string.
        /// </returns>
        public string GetIterationPartitionString(Guid engineeringModelIid)
        {
            return $"Iteration_{engineeringModelIid.ToString().Replace("-", "_")}";
        }
    }
}
